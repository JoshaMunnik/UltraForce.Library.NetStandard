// <copyright file="UFActionCollection.cs" company="Ultra Force Development">
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFActionCollection" /> can be used to manage a collection of active actions.
  /// <para>
  /// It differs from a queue in that actions can be added at later stage and actions can not stop other actions
  /// from running in the list.
  /// </para>
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class UFActionCollection
  {
    #region private vars

    /// <summary>
    /// Contains all started actions.
    /// </summary>
    private readonly List<IUFQueueableAction> m_actions;

    /// <summary>
    /// Contains all actions added while the list is paused.
    /// </summary>
    private readonly List<IUFQueueableAction> m_newActions;

    /// <summary>
    /// Token factory
    /// </summary>
    private CancellationTokenSource? m_tokenSource;

    #endregion

    #region public methods

    /// <summary>
    /// Constructs an instance of <see cref="UFActionCollection" />
    /// </summary>
    public UFActionCollection()
    {
      this.m_actions = new List<IUFQueueableAction>();
      this.m_newActions = new List<IUFQueueableAction>();
      this.Paused = false;
    }

    /// <summary>
    /// Starts an action and adds it to the internal list.
    /// <remarks>
    /// If the list is paused, the action is stored and started once the list is resumed via <see cref="Resume" />.
    /// </remarks>
    /// </summary>
    /// <param name="anAction">Action to start</param>
    public void Start(IUFQueueableAction anAction)
    {
      if (this.Paused)
      {
        lock (this.m_newActions)
        {
          this.m_newActions.Add(anAction);
        }
      }
      else
      {
        this.m_tokenSource ??= new CancellationTokenSource();
        lock (this.m_actions)
        {
          this.m_actions.Add(anAction);
        }
        this.RunActionAsync(anAction, this.m_tokenSource.Token);
      }
    }

    /// <summary>
    /// Stops all running actions in the list. The list will be empty after 
    /// this call.
    /// </summary>
    public void Stop()
    {
      // notify all actions that they should cancel running
      this.m_tokenSource?.Cancel();
      // create new source with first call to start
      this.m_tokenSource = null;
      // clear lists
      lock (this.m_actions)
      {
        this.m_actions.Clear();
      }
      lock (this.m_newActions)
      {
        this.m_newActions.Clear();
      }
    }

    /// <summary>
    /// Pauses the list and all active actions.
    /// </summary>
    public void Pause()
    {
      if (!this.Paused)
      {
        this.Paused = true;
        lock (this.m_actions)
        {
          foreach (IUFQueueableAction action in this.m_actions)
          {
            if (action is IUFPausableAction pausable)
            {
              pausable.Pause();
            }
          }
        }
      }
    }

    /// <summary>
    /// Resumes the list and all active actions.
    /// <para>
    /// Actions that were added via <see cref="Start" /> while the list was
    /// paused will be started.
    /// </para>
    /// </summary>
    public void Resume()
    {
      if (!this.Paused)
      {
        return;
      }
      this.Paused = false;
      lock (this.m_actions)
      {
        // use copy, since resume might remove an action from the list
        List<IUFQueueableAction> actionsCopy = new List<IUFQueueableAction>(this.m_actions);
        foreach (IUFQueueableAction action in actionsCopy)
        {
          if (action is IUFPausableAction pausable)
          {
            pausable.Resume();
          }
        }
      }
      lock (this.m_newActions)
      {
        foreach (IUFQueueableAction action in this.m_newActions)
        {
          // since the collection is no longer paused, this will add and start the action normally
          this.Start(action);
        }
        this.m_newActions.Clear();
      }
    }

    #endregion

    #region public properties

    /// <summary>
    /// Number of running actions.
    /// </summary>
    public int Count {
      get {
        lock (this.m_actions)
        {
          return this.m_actions.Count;
        }
      }
    }

    /// <summary>
    /// <c>True</c> when the list is paused.
    /// </summary>
    public bool Paused { get; private set; }

    #endregion

    #region private methods

    /// <summary>
    /// Runs the action, wait for it and removes it from the list.
    /// </summary>
    /// <param name="anAction">Action to run</param>
    /// <param name="aToken">Cancellation token</param>
    private async void RunActionAsync(IUFQueueableAction anAction, CancellationToken aToken)
    {
      await anAction.RunAsync(aToken);
      // only place task can be cancelled from is the stop method, which will
      // clear the whole list. So only remove if task was not cancelled.
      if (aToken.IsCancellationRequested)
      {
        return;
      }
      lock (this.m_actions)
      {
        this.m_actions.Remove(anAction);
      }
    }

    #endregion
  }
}