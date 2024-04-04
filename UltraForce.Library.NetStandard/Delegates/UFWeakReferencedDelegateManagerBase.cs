// <copyright file="UFWeakReferencedDelegateManagerBase.cs" company="Ultra Force Development">
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Delegates
{
  /// <summary>
  /// <see cref="UFWeakReferencedDelegateManagerBase"/> is a base class for classes that need to manage a group of
  /// weak referenced delegates using a <see cref="UFWeakReferencedDelegateBase"/> subclass.
  /// <para>
  /// It is an abstract class and should never be instantiated directly. 
  /// </para>
  /// </summary>
  public abstract class UFWeakReferencedDelegateManagerBase
  {
    #region private variables

    /// <summary>
    /// List of weak referenced delegates.
    /// </summary>
    private readonly List<UFWeakReferencedDelegateBase> m_delegates;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of
    /// <see cref="UFWeakReferencedDelegateManagerBase"/>
    /// </summary>
    protected UFWeakReferencedDelegateManagerBase()
    {
      this.m_delegates = new List<UFWeakReferencedDelegateBase>();
    }

    #endregion

    #region protected methods

    /// <summary>
    /// Adds a delegate to the managed list. If there is already a delegate with the same method and target stored,
    /// nothing happens.
    /// <para>
    /// The method tries first to replace an entry that has a target that is no longer available before adding it to the
    /// end of the list.
    /// </para>
    /// </summary>
    /// <param name="aDelegate">Delegate to add</param>
    protected void Add(UFWeakReferencedDelegateBase aDelegate)
    {
      lock (this.m_delegates)
      {
        if (this.Find(aDelegate) == null)
        {
          int index = this.FindUnusedIndex();
          if (index < 0)
          {
            this.m_delegates.Add(aDelegate);
          }
          else
          {
            this.m_delegates[index] = aDelegate;
          }
        }
      }
    }

    /// <summary>
    /// Removes a delegate from the managed list. 
    /// </summary>
    /// <param name="aDelegate">Delegate to remove</param>
    protected void Remove(UFWeakReferencedDelegateBase aDelegate)
    {
      lock (this.m_delegates)
      {
        UFWeakReferencedDelegateBase? item = this.Find(aDelegate);
        if (item != null)
        {
          this.m_delegates.Remove(item);
        }
      }
    }

    /// <summary>
    /// Removes all entries that have targets that are no longer available.
    /// </summary>
    protected void CleanUp()
    {
      lock (this.m_delegates)
      {
        UFListTools.Compact(this.m_delegates, item => item.IsAlive);
      }
    }

    /// <summary>
    /// Invokes the delegates for the targets that are still available.
    /// </summary>
    /// <param name="anArguments"></param>
    protected void Invoke(params object[] anArguments)
    {
      // use copy in case invoking the delegate might change the delegate list
      List<UFWeakReferencedDelegateBase> copy;
      lock (this.m_delegates)
      {
        copy = new List<UFWeakReferencedDelegateBase>(this.m_delegates);
      }
      foreach (UFWeakReferencedDelegateBase handler in copy)
      {
        handler.InternalInvoke(anArguments);
      }
    }

    #endregion

    #region private methods

    /// <summary>
    /// Processes the stored delegates and return the one which matches
    /// the method and target.
    /// </summary>
    /// <param name="aDelegate"></param>
    /// <returns>stored delegate or null if there is none</returns>
    private UFWeakReferencedDelegateBase? Find(UFWeakReferencedDelegateBase aDelegate)
    {
      return this.m_delegates.FirstOrDefault(handler => handler.Equals(aDelegate));
    }

    /// <summary>
    /// Tries to find an entry with a target that is no longer available.
    /// </summary>
    /// <returns>Index to entry or -1 if no entry could be found</returns>
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    private int FindUnusedIndex()
    {
      for (int index = this.m_delegates.Count - 1; index >= 0; index--)
      {
        if (!this.m_delegates[index].IsAlive)
        {
          return index;
        }
      }
      return -1;
    }

    #endregion
  }
}