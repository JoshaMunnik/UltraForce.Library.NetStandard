// <copyright file="UFQueueableAction.cs" company="Ultra Force Development">
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

using System.Threading.Tasks;
using System.Threading;
using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// This class implements <see cref="IUFQueueableAction" /> the interface. 
  /// </summary>
  /// <remarks>
  /// The class also implements <see cref="IUFWeightedProgress" /> so queue classes can calculate an overall progress.
  /// <para>
  /// Subclasses must implement <see cref="RunAsync(CancellationToken)" />. 
  /// </para>
  /// </remarks>
  public abstract class UFQueueableAction : IUFWeightedProgress, IUFQueueableAction
  {
    #region public methods

    /// <summary>
    /// Runs the action without a cancellation token.
    /// </summary>
    /// <returns>True if all actions returned true</returns>
    public Task<bool> RunAsync()
    {
      return this.RunAsync(CancellationToken.None);
    }

    #endregion

    #region IUFQueueableAction

    /// <summary>
    /// This method runs the action.
    /// </summary>
    /// <remarks>
    /// Subclasses must implement this method. 
    /// </remarks>
    /// <param name="aToken">Cancellation token</param>
    /// <returns>
    /// <c>True</c> when successful, <c>false</c> to stop other actions in the queue.
    /// </returns>
    public abstract Task<bool> RunAsync(CancellationToken aToken);

    #endregion

    #region IUFWeightedProgress

    /// <summary>
    /// This property defines the progress weight. It can be used to adjust
    /// the amount of progress change this method has within a containing
    /// progress queue.
    /// <para>
    /// The default implementation return 1.0
    /// </para>
    /// </summary>
    public virtual double ProgressWeight => 1.0;

    /// <summary>
    /// This property contains the progress of the action.
    /// <para>
    /// The default implementation just returns 0.0
    /// </para>
    /// </summary>
    public virtual double Progress => 0.0;

    #endregion
  }
}