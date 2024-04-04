// <copyright file="UFExternalAction.cs" company="Ultra Force Development">
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
  /// <see cref="UFExternalAction"/> can be used to implement actions that start something and have to wait for it to
  /// finish (for example via a callback).
  /// <para>
  /// <see cref="RunAsync"/> will call <see cref="StartAsync"/> and then waits until a call is made
  /// to <see cref="Done"/>.
  /// </para>
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public abstract class UFExternalAction : UFQueueableAction, IUFPausableAction
  {
    #region private variables

    /// <summary>
    /// This task is created when the action is being run.
    /// </summary>
    private TaskCompletionSource<bool>? m_finished;

    /// <summary>
    /// Result which done was called with
    /// </summary>
    private bool m_doneResult;

    /// <summary>
    /// True if the action is being paused
    /// </summary>
    private bool m_paused;

    /// <summary>
    /// True when done was called
    /// </summary>
    private bool m_done;

    #endregion

    #region ufqueueableaction

    /// <inheritdoc />
    public override async Task<bool> RunAsync(CancellationToken aToken)
    {
      this.m_finished = new TaskCompletionSource<bool>();
      await this.StartAsync(aToken);
      if (aToken.IsCancellationRequested)
      {
        return false;
      }
      await this.m_finished.Task;
      this.m_finished = null;
      return this.m_doneResult;
    }

    #endregion

    #region IUFPausableAction

    /// <inheritdoc />
    public void Pause()
    {
      this.m_paused = true;
    }

    /// <inheritdoc />
    public void Resume()
    {
      if (!this.m_paused)
      {
        return;
      }
      this.m_paused = false;
      // resume running if Done was called while the action was paused
      if (this.m_done)
      {
        this.ResumeRun();
      }
    }

    #endregion

    #region protected methods

    /// <summary>
    /// This method is called by <see cref="RunAsync"/> and must be implemented by subclasses.
    /// </summary>
    /// <param name="aToken">
    /// <see cref="CancellationToken"/> to cancel the action
    /// </param>
    protected abstract Task StartAsync(CancellationToken aToken);

    /// <summary>
    /// Resumes <see cref="RunAsync"/> (if the action is not paused) returning the value from <c>aResult</c>.
    /// </summary>
    /// <param name="aResult">
    /// Result to return from <see cref="RunAsync"/>
    /// </param>
    protected void Done(bool aResult)
    {
      this.m_doneResult = aResult;
      this.m_done = true;
      if (!this.m_paused)
      {
        this.ResumeRun();
      }
    }

    #endregion

    #region private methods

    /// <summary>
    /// Resumes the RunAsync (if is active).
    /// </summary>
    private void ResumeRun()
    {
      this.m_finished?.SetResult(true);
    }

    #endregion
  }
}