// <copyright file="UFDownloadToFileAction.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (c) 2018 Ultra Force Development
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFDownloadToFileAction" /> extends <see cref="UFDownloadToStreamAction"/> and saves the stream to
  /// a local file.
  /// </summary>
  public class UFDownloadToFileAction : UFDownloadToStreamAction
  {
    #region private variables

    /// <summary>
    /// Name of file (including path).
    /// </summary>
    private readonly string m_filename;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFDownloadToFileAction"/>.
    /// <para>
    /// The fill will be created when the action is run.
    /// </para>
    /// </summary>
    /// <param name="aFilename">
    /// Filename (including path) to download to
    /// </param>
    public UFDownloadToFileAction(string aFilename)
    {
      this.m_filename = aFilename;
    }

    #endregion

    #region overriden methods

    /// <inheritdoc />
    protected override Task SetRequestAsync(CancellationToken aToken)
    {
      this.SetOutputStream(new FileStream(this.m_filename, FileMode.Create));
      return base.SetRequestAsync(aToken);
    }

    #endregion
  }
}