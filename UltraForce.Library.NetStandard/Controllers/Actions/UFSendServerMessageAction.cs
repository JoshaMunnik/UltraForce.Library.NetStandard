// <copyright file="UFSendServerMessageAction.cs" company="Ultra Force Development">
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFSendServerMessageAction" /> extends <see cref="UFHttpAction{TResponse}"/> and assumes the data
  /// received from the server will be of type string.
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public abstract class UFSendServerMessageAction : UFHttpAction<string>
  {
    #region protected override methods

    /// <inheritdoc />
    protected override async Task<UFServerResponse> SendMessageAsync(
      HttpRequestMessage aRequest,
      CancellationToken aToken
    )
    {
      UFServerResponse result = new UFServerResponse();
      try
      {
        HttpResponseMessage httpResponse = await StaticHttpClient
          .SendAsync(
            aRequest,
            HttpCompletionOption.ResponseHeadersRead,
            aToken
          )
          .ConfigureAwait(true);
        if (!aToken.IsCancellationRequested)
        {
          if (httpResponse.Content != null)
          {
            // read content in chunks so progress can be updated
            double totalLength = httpResponse.Content.Headers.ContentLength ?? 0;
            Stream stream = await httpResponse.Content.ReadAsStreamAsync();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StringBuilder builder = new StringBuilder((int)totalLength);
            double position = 0;
            char[] buffer = new char[16384];
            while (true)
            {
              int count = await reader.ReadAsync(buffer, 0, 16384);
              if ((count == 0) || aToken.IsCancellationRequested)
              {
                break;
              }
              builder.Append(buffer, 0, count);
              if (totalLength > 0.1)
              {
                position += Encoding.UTF8.GetByteCount(buffer, 0, count);
                await this.SetProgressAsync(position / totalLength);
              }
              else
              {
                // show 80% progress with unknown lengths
                await this.SetProgressAsync(0.8);
              }
            }
            if (!aToken.IsCancellationRequested)
            {
              await this.SetProgressAsync(1.0);
              result.ResponseContent = builder.ToString();
            }
          }
          else
          {
            result.ResponseContent = "";
          }
          result.StatusCode = httpResponse.StatusCode;
          // in case of errors, use response as error response
          if ((int)result.StatusCode >= 300)
          {
            result.ErrorResponse = result.ResponseContent;
          }
        }
      }
      catch (Exception error)
      {
        result.Error = error;
      }
      return result;
    }

    #endregion
  }
}