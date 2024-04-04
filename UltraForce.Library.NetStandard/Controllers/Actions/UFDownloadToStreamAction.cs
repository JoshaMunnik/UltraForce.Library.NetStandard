// <copyright file="UFDownloadToStreamAction.cs" company="Ultra Force Development">
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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFDownloadToStreamAction" /> extends <see cref="UFHttpAction{TResponse}"/> and downloads the server
  /// response to a stream.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UFDownloadToStreamAction : UFHttpAction<Stream>
  {
    #region private variables

    /// <summary>
    /// Stream to write data to
    /// </summary>
    private Stream? m_outputStream;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFDownloadToStreamAction"/>.
    /// <para>
    /// Subclasses must call <see cref="SetOutputStream"/> to set the stream.
    /// </para>
    /// </summary>
    public UFDownloadToStreamAction()
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFDownloadToStreamAction"/>.
    /// </summary>
    /// <param name="anOutputStream">Stream to download to</param>
    public UFDownloadToStreamAction(Stream anOutputStream)
    {
      this.m_outputStream = anOutputStream;
    }

    #endregion

    #region public methods

    /// <inheritdoc />
    public override async Task<bool> RunAsync(CancellationToken aToken)
    {
      bool result = await base.RunAsync(aToken);
      this.m_outputStream = null;
      return result;
    }

    #endregion

    #region protected methods

    /// <summary>
    /// Sets the output stream to use.
    /// </summary>
    /// <param name="anOutputStream">Output stream</param>
    protected void SetOutputStream(Stream anOutputStream)
    {
      this.m_outputStream = anOutputStream;
    }

    /// <inheritdoc />
    protected override async Task<UFServerResponse> SendMessageAsync(
      HttpRequestMessage aRequest,
      CancellationToken aToken
    )
    {
      UFServerResponse result = new UFServerResponse();
      try
      {
        HttpResponseMessage httpResponse = await StaticHttpClient.SendAsync(
          aRequest,
          HttpCompletionOption.ResponseHeadersRead,
          aToken
        );
        result.StatusCode = httpResponse.StatusCode;
        if ((httpResponse.Content != null) && !aToken.IsCancellationRequested)
        {
          // read content in chunks so progress can be updated
          long totalLength = httpResponse.Content.Headers.ContentLength ?? 0;
          Stream httpStream = await httpResponse.Content.ReadAsStreamAsync();
          byte[] buffer = new byte[1024];
          while (true)
          {
            int read = await httpStream.ReadAsync(buffer, 0, 1024, aToken);
            if ((read == 0) || aToken.IsCancellationRequested)
            {
              break;
            }
            await this.m_outputStream!.WriteAsync(buffer, 0, read, aToken);
            if (!aToken.IsCancellationRequested)
            {
              await this.UpdateProgressAsync(httpStream, totalLength);
            }
          }
          if (!aToken.IsCancellationRequested)
          {
            await this.SetProgressAsync(1.0);
            result.ResponseContent = this.m_outputStream!;
          }
        }
        else
        {
          result.ResponseContent = null;
        }
      }
      catch (Exception error)
      {
        result.Error = error;
      }
      return result;
    }
    
    #endregion
    
    #region private methods

    /// <summary>
    /// Updates the progress. If total length is 0 (unknown), the progress is set to 0.80.
    /// </summary>
    /// <param name="aStream">Stream to get position from</param>
    /// <param name="aTotalLength">Length to calculate % with</param>
    private async Task UpdateProgressAsync(Stream aStream, long aTotalLength)
    {
      // show 80% progress with unknown lengths
      if (aTotalLength == 0)
      {
        await this.SetProgressAsync(0.8);
      }
      else
      {
        // ReSharper disable once PossibleLossOfFraction
        await this.SetProgressAsync(aStream.Position / aTotalLength);
      }
    }

    #endregion
  }
}