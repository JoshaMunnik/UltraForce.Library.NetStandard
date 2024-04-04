// <copyright file="UFHttpAction.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
// Copyright (C) 2018 Ultra Force Development
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltraForce.Library.NetStandard.Development;
using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFHttpAction{TResponse}"/> base class for sending and receiving data from a server via HTTP.
  /// </summary>
  /// <remarks>
  /// Subclasses must implement to <see cref="SendMessageAsync"/> to send the data and process the response from
  /// the server.
  /// <para>
  /// Subclasses can call one of the SetXXXXRequest methods either from the constructor or from the
  /// <see cref="SetRequestAsync"/> to set the request before the IO starts.
  /// </para>
  /// <para>
  /// There three different types of errors. Subclasses can override
  /// <see cref="ProcessErrorAsync(HttpStatusCode, string)"/>, <see cref="ProcessErrorAsync(Exception)"/> and
  /// <see cref="ProcessTimeoutAsync"/>. 
  /// </para>
  /// <para>
  /// Subclasses can override <see cref="BuildRequestAsync"/> to do additional pre processing.
  /// </para>
  /// </remarks>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
  [SuppressMessage("ReSharper", "UnusedParameter.Global")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public abstract class UFHttpAction<TResponse> : UFQueueableAction
  where TResponse : class
  {
    #region private variables

    /// <summary>
    /// Data to post or null to send nothing
    /// </summary>
    private HttpContent? m_sendData;

    /// <summary>
    /// Method to use when sending
    /// </summary>
    private HttpMethod? m_method;

    /// <summary>
    /// See <see cref="MaxSendTime"/>
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static int s_maxSendTime = 30000;

    /// <summary>
    /// See <see cref="Progress"/>
    /// </summary>
    private double m_progress;

    /// <summary>
    /// See <see cref="StaticHttpClient"/> property
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static HttpClient? s_httpClient;

#if UFDEBUG
    /// <summary>
    /// Log to use (if any)
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static UFHtmlLog? s_log;
#endif

    #endregion

    #region public methods

    /// <inheritdoc />
    public override async Task<bool> RunAsync(CancellationToken aToken)
    {
      // set a request
      await this.SetRequestAsync(aToken);
      // keep repeating IO until either successful or one of the error methods indicates to stop.
      while (true)
      {
        // token that will cancel after MaxSendTime
        CancellationTokenSource timeoutSource = new CancellationTokenSource(MaxSendTime);
        // create token that combines timeout with aToken, so it will cancel if either one of those cancels
        CancellationTokenSource combinedSource = CancellationTokenSource.CreateLinkedTokenSource(
          aToken,
          timeoutSource.Token
        );
        // get request
        HttpRequestMessage request = await this.BuildRequestAsync(this.Uri!, this.m_sendData!, this.m_method!);
        //request.Headers.Host = request.RequestUri.Host;
#if UFDEBUG
        await this.LogRequest(request);
#endif
        // perform request
        UFServerResponse response = await this.SendMessageAsync(request, combinedSource.Token);
        // action was cancelled from outside?
        if (aToken.IsCancellationRequested)
        {
#if UFDEBUG
          s_log?.Error(this, "[IO] Cancelled, url={0}", request.RequestUri);
#endif
          return this.ErrorResult;
        }
        // action was cancelled because of timeout?
        if (timeoutSource.IsCancellationRequested)
        {
#if UFDEBUG
          s_log?.Error(this, "[IO] Timeout, url={0}", request.RequestUri);
#endif
          if (!await this.ProcessTimeoutAsync())
          {
            return this.ErrorResult;
          }
        }
        // an exception occurred while sending the data?
        else if (response.Error != null)
        {
#if UFDEBUG
          s_log?.Error(this, response.Error, "[IO] Exception, url=" + request.RequestUri);
#endif
          if (!await this.ProcessErrorAsync(response.Error))
          {
            return this.ErrorResult;
          }
        }
        // server returned a status code indicating success?
        else if ((int)response.StatusCode < 300)
        {
#if UFDEBUG
          this.LogResponse(request, response);
#endif
          return await this.ProcessResponseAsync(response.ResponseContent!);
        }
        // request was send successfully to server, but server returned an error code
        else
        {
#if UFDEBUG
          this.LogServerError(request, response);
#endif
          if (!await this.ProcessErrorAsync(response.StatusCode, response.ErrorResponse!))
          {
            return this.ErrorResult;
          }
        }
      }
    }
   

#if UFDEBUG

    /// <summary>
    /// Sets a <see cref="UFHtmlLog" /> instance to send log messages to
    /// </summary>
    /// <param name="aLog">
    /// <see cref="UFHtmlLog" /> instance to use or <c>null</c> to disable logging.
    /// </param>
    public static void UseLog(UFHtmlLog aLog)
    {
      s_log = aLog;
    }

#endif

    #endregion

    #region public properties

    /// <summary>
    /// MaxSendTime determines the maximum time in milliseconds an IO action make take. 
    /// <para>
    /// The minimum value that can be assigned is 1000. Any lower value is changed to 1000.
    /// </para>
    /// </summary>
    public static int MaxSendTime {
      get => s_maxSendTime;
      set => s_maxSendTime = Math.Max(value, 1000);
    }

    /// <summary>
    /// Uri being communicated with.
    /// </summary>
    public string? Uri { get; private set; }

    #endregion

    #region IUFProgress

    /// <inheritdoc />
    public override double Progress => this.m_progress;

    #endregion

    #region protected properties

    /// <summary>
    /// The value to return by <see cref="RunAsync"/> if the communication fails.
    /// <para>
    /// The default value is <c>false</c>
    /// </para>
    /// </summary>
    protected bool ErrorResult { get; set; }

    /// <summary>
    /// The HttpClient property is a static property and can be used to minimize the number of HttpClient instances.
    /// <para>
    /// The instance is created the first time the property is accessed.
    /// </para>
    /// </summary>
    protected static HttpClient StaticHttpClient => s_httpClient ??= new HttpClient();

    #endregion

    #region protected overridable methods

    /// <summary>
    /// This method can be overridden by subclasses. It will be called at the start of the IO action.
    /// <para>
    /// The default implementation does nothing and just returns <see cref="Task.CompletedTask"/>.
    /// </para>
    /// </summary>
    /// <param name="aToken">Cancellation token</param>
    protected virtual Task SetRequestAsync(CancellationToken aToken)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the progress property.
    /// <para>
    /// The default implementation updates the internal progress value. Subclasses can override to provide additional
    /// actions.
    /// </para>
    /// </summary>
    /// <param name="aValue">New value (might be outside valid range of 0.0 .. 1.0)</param>
    protected virtual Task SetProgressAsync(double aValue)
    {
      this.m_progress = Math.Max(0.0, Math.Min(1.0, aValue));
      return Task.CompletedTask;
    }

    #endregion

    #region protected SetRequest methods

    /// <summary>
    /// Initialize action to send a data to the server via a certain method.
    /// </summary>
    /// <param name="anUri">url to server</param>
    /// <param name="aSendData">Data to send null to not send any data</param>
    /// <param name="aMethod">Method to use</param>
    protected void SetRequest(string anUri, HttpContent? aSendData, HttpMethod aMethod)
    {
      this.Uri = anUri;
      this.m_sendData = aSendData;
      this.m_method = aMethod;
    }

    /// <summary>
    /// Initialize action to query the server via a certain method.
    /// </summary>
    /// <param name="anUri">url to server</param>
    /// <param name="aMethod">Method to use</param>
    protected void SetRequest(string anUri, HttpMethod aMethod)
    {
      this.SetRequest(anUri, null, aMethod);
    }

    /// <summary>
    /// Initialize action to send a data to the server via a certain method using a certain encoding and media type.
    /// </summary>
    /// <param name="anUri">url to server</param>
    /// <param name="aSendData"></param>
    /// <param name="anEncoding"></param>
    /// <param name="aMediaType"></param>
    /// <param name="aMethod"></param>
    protected void SetStringRequest(
      string anUri,
      string aSendData,
      Encoding anEncoding,
      string aMediaType,
      HttpMethod aMethod
    )
    {
      this.SetRequest(anUri, new StringContent(aSendData, anEncoding, aMediaType), aMethod);
    }

    /// <summary>
    /// Initialize action to send a json encoded data to the server via a certain method.
    /// </summary>
    /// <param name="anUri">
    /// url to server
    /// </param>
    /// <param name="aSendData">
    /// Data to encode as JSON
    /// </param>
    /// <param name="aMethod">
    /// Method to use
    /// </param>
    protected void SetJsonRequest(string anUri, IUFJsonExport aSendData, HttpMethod aMethod)
    {
      StringBuilder builder = new StringBuilder();
      aSendData.SaveJson(builder);
      this.SetStringRequest(
        anUri,
        builder.ToString(),
        Encoding.UTF8,
        "application/json",
        aMethod
      );
    }

    /// <summary>
    /// Initialize action to send a json encoded data to the server via the 
    /// POST method.
    /// </summary>
    /// <param name="anUri">
    /// url to server
    /// </param>
    /// <param name="aSendData">
    /// Data to encode as JSON
    /// </param>
    protected void SetJsonRequest(string anUri, IUFJsonExport aSendData)
    {
      this.SetJsonRequest(anUri, aSendData, HttpMethod.Post);
    }

    /// <summary>
    /// Initialize action to query server via the <see cref="HttpMethod.Get"/> method.
    /// </summary>
    /// <param name="anUri">url to server</param>
    protected void SetRequest(string anUri)
    {
      this.SetRequest(anUri, HttpMethod.Get);
    }

    #endregion

    #region protected process methods

    /// <summary>
    /// Processes the data received from the server.
    /// </summary>
    /// <remarks>
    /// The default implementation returns <c>true</c>.
    /// </remarks>
    /// <param name="aResponse">
    /// Response (can be <c>null</c> if there was no data)
    /// </param>
    /// <returns>
    /// <c>True</c> if data was processed successfully. This result will be returned as the result of the action.
    /// </returns>
    protected virtual Task<bool> ProcessResponseAsync(TResponse aResponse)
    {
      return Task.FromResult(true);
    }

    /// <summary>
    /// Processes the exception that occurred while sending the data.
    /// </summary>
    /// <remarks>
    /// The default implementation just returns <c>false</c>.
    /// </remarks>
    /// <param name="anError">Error to process</param>
    /// <returns>
    /// <c>True</c> to repeat the send action, <c>false</c> to cancel the action and return <see cref="ErrorResult"/>.
    /// </returns>
    protected virtual Task<bool> ProcessErrorAsync(Exception anError)
    {
      return Task.FromResult(false);
    }

    /// <summary>
    /// Process error statuses from the server (status code >= 300)
    /// </summary>
    /// <remarks>
    /// The default implementation just returns <c>false</c>.
    /// </remarks>
    /// <param name="aStatus">Status returned by server</param>
    /// <param name="anErrorResponse">Response if any</param>
    /// <returns>
    /// <c>True</c> to repeat the send action, <c>false</c> to cancel the action and return <see cref="ErrorResult"/>.
    /// </returns>
    protected virtual Task<bool> ProcessErrorAsync(HttpStatusCode aStatus, string anErrorResponse)
    {
      return Task.FromResult(false);
    }

    /// <summary>
    /// Processes timeout error.
    /// </summary>
    /// <remarks>
    /// The default implementation just returns <c>false</c>.
    /// </remarks>
    /// <returns>
    /// <c>True</c> to repeat the send action, <c>false</c> to cancel the action and return <see cref="ErrorResult"/>.
    /// </returns>
    protected virtual Task<bool> ProcessTimeoutAsync()
    {
      return Task.FromResult(false);
    }

    /// <summary>
    /// Builds a request to send to the server.
    /// <para>
    /// The default implementation just creates the <see cref="HttpRequestMessage"/> instance. Subclasses can override
    /// to perform additional actions (for example setting additional header values).
    /// </para>
    /// </summary>
    /// <param name="anUri">
    /// Uri to send to
    /// </param>
    /// <param name="aContent">
    /// Content to send (or null if there is none)
    /// </param>
    /// <param name="aMethod">
    /// Method to use
    /// </param>
    /// <returns>
    /// A <see cref="HttpRequestMessage"/> to use for the server IO.
    /// </returns>
    protected virtual Task<HttpRequestMessage> BuildRequestAsync(string anUri, HttpContent? aContent, HttpMethod aMethod)
    {
      // create request for method
      HttpRequestMessage result = new HttpRequestMessage(aMethod, anUri);
      // add data if there is any
      if (aContent != null)
      {
        // set content
        result.Content = aContent;
      }
      return Task.FromResult(result);
    }

    /// <summary>
    /// Sends a message to the server and process the response.
    /// <para>
    /// This is an abstract method and must be implemented by subclasses.
    /// </para>
    /// </summary>
    /// <param name="aRequest">
    /// Request to send
    /// </param>
    /// <param name="aToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// The result of sending the message.
    /// </returns>
    protected abstract Task<UFServerResponse> SendMessageAsync(HttpRequestMessage aRequest, CancellationToken aToken);

    #endregion
    
    #region private methods
    
#if UFDEBUG    

    /// <summary>
    /// Writes a server error to the log.
    /// </summary>
    /// <param name="aRequest"></param>
    /// <param name="aResponse"></param>
    private void LogServerError(HttpRequestMessage aRequest, UFServerResponse aResponse)
    {
      s_log?.Error(
        this,
        "[IO][Response] url={0}, status={1} ({2}), content={3}",
        aRequest.RequestUri,
        (int)aResponse.StatusCode,
        aResponse.StatusCode,
        aResponse.ErrorResponse ?? ""
      );
    }

    /// <summary>
    /// Writes a response to the log.
    /// </summary>
    /// <param name="aRequest"></param>
    /// <param name="aResponse"></param>
    private void LogResponse(HttpRequestMessage aRequest, UFServerResponse aResponse)
    {
      if (s_log != null)
      {
        string content = aResponse.ResponseContent == null ? "null" : aResponse.ResponseContent.ToString();
        if (content.Length > 1024)
        {
          content = content.Substring(0, 1024) + "...";
        }
        else if (content.Length == 0)
        {
          content = "(empty)";
        }
        s_log.Add(
          this,
          "[IO][Response] url={0}, status={1} ({2}), content={3}",
          aRequest.RequestUri,
          (int)aResponse.StatusCode,
          aResponse.StatusCode,
          content
        );
      }
    }

    /// <summary>
    /// Writes a request to the log
    /// </summary>
    /// <param name="aRequest"></param>
    private async Task LogRequest(HttpRequestMessage aRequest)
    {
      if (s_log != null)
      {
        if (this.m_sendData != null)
        {
          string data = await this.m_sendData.ReadAsStringAsync();
          s_log.Add(
            this,
            "[IO][Request] url={0}, method={1}, data=({2}):{3}",
            aRequest.RequestUri,
            this.m_method!,
            this.m_sendData.GetType().Name,
            data
          );
        }
        else
        {
          s_log.Add(
            this,
            "[IO][Request] url={0}, method={1}",
            aRequest.RequestUri,
            this.m_method!
          );
        }
      }
    }
    
#endif
    
    #endregion

    #region UFServerResponse

    /// <summary>
    /// Response from server.
    /// </summary>
    protected class UFServerResponse
    {
      /// <summary>
      /// StatusCode returned by server
      /// </summary>
      public HttpStatusCode StatusCode { get; set; }

      /// <summary>
      /// Response content or null if there was none
      /// </summary>
      public TResponse? ResponseContent { get; set; }

      /// <summary>
      /// Response as string when server returned an error
      /// </summary>
      public string? ErrorResponse { get; set; } 

      /// <summary>
      /// If an exception occurred during IO it will be assigned to this
      /// property.
      /// </summary>
      public Exception? Error { get; set; }
    }

    #endregion
  }
}