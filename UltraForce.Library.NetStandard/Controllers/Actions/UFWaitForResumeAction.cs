// <copyright file="UFWaitForResumeAction.cs" company="Ultra Force Development">
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

using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFWaitForResumeAction"/> can be used to implement actions that will pause the action.
  /// <para>
  /// The class will call <see cref="Start"/>, wait for the action to get paused and then wait for the action to get
  /// resumed which will finish the action.
  /// </para>
  /// </summary>
  public abstract class UFWaitForResumeAction : UFQueueableAction, IUFPausableAction
  {
    #region private variables

    /// <summary>
    /// This task is created when the action is being paused.
    /// </summary>
    private TaskCompletionSource<bool>? m_paused;

    /// <summary>
    /// This task is created when the action is being resumed.
    /// </summary>
    private TaskCompletionSource<bool>? m_resumed;

    #endregion

    #region UFQueueableAction

    /// <inheritdoc />
    public override async Task<bool> RunAsync(CancellationToken aToken)
    {
      this.m_paused = new TaskCompletionSource<bool>();
      this.m_resumed = new TaskCompletionSource<bool>();
      if (!this.Start())
      {
        return false;
      }
      await this.m_paused.Task;
      this.m_paused = null;
      await this.m_resumed.Task;
      this.m_resumed = null;
      return true;
    }

    #endregion

    #region IUFPausableAction

    /// <inheritdoc />
    public void Pause()
    {
      this.m_paused?.SetResult(true);
    }

    /// <inheritdoc />
    public void Resume()
    {
      this.m_resumed?.SetResult(true);
    }

    #endregion

    #region protected overidable methods

    /// <summary>
    /// This method is called by <see cref="RunAsync"/> and should perform an action that will result in the pausing of
    /// this action.
    /// </summary>
    /// <returns>
    /// <c>True</c> to wait for a pause / resume cycle. Or <c>False</c> to skip and let <see cref="RunAsync"/> return
    /// <c>false</c> as well.
    /// </returns>
    protected abstract bool Start();

    #endregion
  }
}