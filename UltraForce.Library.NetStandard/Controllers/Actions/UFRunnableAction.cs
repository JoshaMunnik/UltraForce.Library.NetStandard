// <copyright file="UFRunnableAction.cs" company="Ultra Force Development">
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

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFRunnableAction" /> implements a simple version of  <see cref="IUFQueueableAction" />. It can be used
  /// by actions which are not stoppable or pausable.
  /// </summary>
  /// <remarks>
  /// Subclasses must implement the <see cref="Run" /> method.
  /// </remarks>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public abstract class UFRunnableAction : IUFQueueableAction
  {
    #region public methods

    /// <summary>
    /// Runs the queue without a cancellation token.
    /// </summary>
    /// <returns>
    /// A value indicating of the action was successful or not.
    /// </returns>
    public Task<bool> RunAsync()
    {
      return this.RunAsync(CancellationToken.None);
    }

    #endregion

    #region IUFQueueableAction

    /// <inheritdoc />
    public Task<bool> RunAsync(CancellationToken aToken)
    {
      return Task.FromResult(this.Run());
    }

    #endregion

    #region protected methods

    /// <summary>
    /// Runs the action.
    /// </summary>
    /// <returns>
    /// A value indicating of the action was successful or not.
    /// </returns>
    protected abstract bool Run();

    #endregion
  }
}