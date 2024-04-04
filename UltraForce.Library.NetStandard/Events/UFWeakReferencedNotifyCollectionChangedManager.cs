// <copyright file="UFWeakReferencedNotifyCollectionChangedManager.cs" company="Ultra Force Development">
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
using System.Collections.Specialized;
using System.Linq;
using UltraForce.Library.NetStandard.Internal;

namespace UltraForce.Library.NetStandard.Events
{
  /// <summary>
  /// <see cref="UFWeakReferencedNotifyCollectionChangedManager"/> stores event
  /// handlers as methods and target. It uses a weak reference for
  /// the target and only invokes the handler only if the target is still
  /// available.
  /// </summary>
  public class UFWeakReferencedNotifyCollectionChangedManager
  {
    #region private variables

    /// <summary>
    /// List of handler helpers.
    /// </summary>
    private readonly List<UFWeakNotifyCollectionChangedHandlerHelper>
      m_handlers;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of
    /// <see cref="UFWeakReferencedNotifyCollectionChangedManager"/>
    /// </summary>
    public UFWeakReferencedNotifyCollectionChangedManager()
    {
      this.m_handlers = new List<UFWeakNotifyCollectionChangedHandlerHelper>();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Adds a handler to the managed list. If there is already a handler
    /// with the same method and target stored, nothing happens.
    /// </summary>
    /// <param name="aHandler">Handler to add</param>
    public void Add(NotifyCollectionChangedEventHandler aHandler)
    {
      UFWeakNotifyCollectionChangedHandlerHelper handler =
        new UFWeakNotifyCollectionChangedHandlerHelper(aHandler);
      lock (this.m_handlers)
      {
        if (this.Find(handler) == null)
        {
          this.m_handlers.Add(handler);
        }
      }
    }

    /// <summary>
    /// Removes a handler from the managed list. 
    /// </summary>
    /// <param name="aHandler">Handler to remove</param>
    public void Remove(NotifyCollectionChangedEventHandler aHandler)
    {
      UFWeakNotifyCollectionChangedHandlerHelper? handler =
        new UFWeakNotifyCollectionChangedHandlerHelper(aHandler);
      lock (this.m_handlers)
      {
        handler = this.Find(handler);
        if (handler != null)
        {
          this.m_handlers.Remove(handler);
        }
      }
    }

    /// <summary>
    /// Invokes the handlers for the targets that are still available.
    /// </summary>
    /// <param name="aSender">Sender to use</param>
    /// <param name="anArguments">Arguments to use</param>
    public void Invoke(
      object aSender,
      NotifyCollectionChangedEventArgs anArguments
    )
    {
      List<UFWeakNotifyCollectionChangedHandlerHelper> copy;
      lock (this.m_handlers)
      {
        copy = new List<UFWeakNotifyCollectionChangedHandlerHelper>(this.m_handlers);
      }
      foreach (UFWeakNotifyCollectionChangedHandlerHelper handler in copy)
      {
        handler.Invoke(aSender, anArguments);
      }
    }

    #endregion

    #region private methods

    /// <summary>
    /// Processes the stored handlers and return the one which matches
    /// the method and target.
    /// </summary>
    /// <param name="anHandler"></param>
    /// <returns>stored handler or null if there is none</returns>
    private UFWeakNotifyCollectionChangedHandlerHelper? Find(
      UFWeakNotifyCollectionChangedHandlerHelper anHandler
    )
    {
      return this.m_handlers.FirstOrDefault(handler => handler.Equals(anHandler));
    }

    #endregion
  }
}