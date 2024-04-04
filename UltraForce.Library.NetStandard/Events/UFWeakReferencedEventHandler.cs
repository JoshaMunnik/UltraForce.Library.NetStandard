// <copyright file="UFWeakReferencedEventHandler.cs" company="Ultra Force Development">
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

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using UltraForce.Library.NetStandard.Delegates;

namespace UltraForce.Library.NetStandard.Events
{
  /// <summary>
  /// Stores an event handler as method and weak reference to the target.
  /// <para>
  /// Either an instance can be created or a <see cref="Wrap(EventHandler)"/> can be used to create an instance for
  /// a handler.
  /// </para>
  /// </summary>
  public class UFWeakReferencedEventHandler : UFWeakReferencedDelegateBase
  {
    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFWeakReferencedEventHandler"/>
    /// for a <see cref="EventHandler"/>.
    /// </summary>
    /// <param name="anHandler">event handler</param>
    public UFWeakReferencedEventHandler(EventHandler anHandler) : base(anHandler)
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFWeakReferencedEventHandler"/>
    /// for a <see cref="PropertyChangedEventHandler"/>.
    /// </summary>
    /// <param name="anHandler"></param>
    public UFWeakReferencedEventHandler(PropertyChangedEventHandler anHandler) : base(anHandler)
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFWeakReferencedEventHandler"/>
    /// for a <see cref="NotifyCollectionChangedEventHandler"/>.
    /// </summary>
    /// <param name="anHandler"></param>
    public UFWeakReferencedEventHandler(NotifyCollectionChangedEventHandler anHandler) : base(anHandler)
    {
    }

    /// <inheritdoc />
    protected UFWeakReferencedEventHandler(Delegate aDelegate) : base(aDelegate)
    {
    }

    #endregion

    #region public methods

    /// <summary>
    /// Calls the handler method with <see cref="EventArgs.Empty"/> if the
    /// target has not been garbage collected.
    /// </summary>
    /// <param name="aSender"></param>
    public void Invoke(object aSender)
    {
      this.Invoke(aSender, EventArgs.Empty);
    }

    /// <summary>
    /// Calls the handler method if the target has not been garbage collected.
    /// </summary>
    /// <param name="aSender"></param>
    /// <param name="anEventArgs"></param>
    public void Invoke(object aSender, EventArgs anEventArgs)
    {
      base.Invoke(aSender, anEventArgs);
    }

    /// <summary>
    /// Wraps an event handler by creating an instance of <see cref="UFWeakReferencedEventHandler"/> for the
    /// handler and return its <see cref="Invoke(object,EventArgs)"/> method.
    /// <para>
    /// Take note that when attaching the result of this method to an event the
    /// <see cref="UFWeakReferencedEventHandler"/> instance will NOT get garbage collected until the event provider
    /// gets garbage collected.
    /// </para>
    /// <para>
    /// If the handler is a static handler, the method just returns the handler.
    /// </para>
    /// </summary>
    /// <param name="anHandler">
    /// Handler method to wrap. If it is a static handler, the method just returns anHandler.
    /// </param>
    /// <returns>
    /// Handler method of the wrapper object or the value of <c>anHandler</c> if it was a static handler.
    /// </returns>
    public static EventHandler Wrap(EventHandler anHandler)
    {
      if (anHandler.Target == null)
      {
        return anHandler;
      }
      UFWeakReferencedEventHandler helper = new UFWeakReferencedEventHandler(anHandler);
      return helper.Invoke;
    }

    /// <summary>
    /// Wraps an event handler by creating an instance of <see cref="UFWeakReferencedEventHandler{T}"/> for the
    /// handler and return its <see cref="Invoke(object,EventArgs)"/> method.
    /// </summary>
    /// <param name="anHandler">
    /// Handler method to wrap. If it is a static handler, the method just returns anHandler.
    /// </param>
    /// <returns>
    /// Handler method of the wrapper object or the value of <c>anHandler</c> if it was a static handler.
    /// </returns>
    public static EventHandler<UFDataChangedEventArgs> Wrap(EventHandler<UFDataChangedEventArgs> anHandler)
    {
      return Wrap<UFDataChangedEventArgs>(anHandler);
    }

    /// <summary>
    /// Wraps an event handler by creating an instance of <see cref="UFWeakReferencedEventHandler{T}"/> for the
    /// handler and return its <see cref="UFWeakReferencedEventHandler.Invoke(object,EventArgs)"/> method.
    /// </summary>
    /// <typeparam name="TEventArgs">
    /// <see cref="EventArgs"/> type to use
    /// </typeparam>
    /// <param name="anHandler">
    /// Handler method to wrap. If it is a static handler, the method just returns anHandler.
    /// </param>
    /// <returns>
    /// Handler method of the wrapper object or the value of <c>anHandler</c> if it was a static handler.
    /// </returns>
    public static EventHandler<TEventArgs> Wrap<TEventArgs>(EventHandler<TEventArgs> anHandler)
      where TEventArgs : EventArgs
    {
      if (anHandler.Target == null)
      {
        return anHandler;
      }
      UFWeakReferencedEventHandler<TEventArgs> helper = new UFWeakReferencedEventHandler<TEventArgs>(anHandler);
      return helper.Invoke;
    }

    /// <summary>
    /// Wraps an event handler by creating an instance of <see cref="UFWeakReferencedEventHandler"/> for the handler and
    /// return its <see cref="UFWeakReferencedEventHandler.Invoke(object)"/> method.
    /// <para>
    /// Take note that when attaching the result of this method to an event the
    /// <see cref="UFWeakReferencedEventHandler"/> instance will NOT get garbage collected until the event provider
    /// gets garbage collected.
    /// </para>
    /// <para>
    /// If the handler is a static handler, the method just returns the handler.
    /// </para>
    /// </summary>
    /// <param name="anHandler">
    /// Handler method to wrap. If it is a static handler, the method just returns anHandler.
    /// </param>
    /// <returns>
    /// Handler method of the wrapper object or the value of <c>anHandler</c> if it was a static handler.
    /// </returns>
    public static PropertyChangedEventHandler Wrap(PropertyChangedEventHandler anHandler)
    {
      if (anHandler.Target == null)
      {
        return anHandler;
      }
      UFWeakReferencedEventHandler helper = new UFWeakReferencedEventHandler(anHandler);
      return helper.Invoke;
    }

    #endregion
  }

  /// <summary>
  /// A generic version of <see cref="UFWeakReferencedEventHandler"/>.
  /// </summary>
  /// <typeparam name="TEventArgs">Event arguments type</typeparam>
  public class UFWeakReferencedEventHandler<TEventArgs> : UFWeakReferencedEventHandler where TEventArgs : EventArgs
  {
    #region constructors

    /// <summary>
    /// Constructs an instance of
    /// <see cref="UFWeakReferencedEventHandler{TEventArgs}"/>
    /// </summary>
    /// <param name="anHandler">event handler</param>
    public UFWeakReferencedEventHandler(EventHandler<TEventArgs> anHandler) : base(anHandler)
    {
    }

    #endregion

    #region public methods

    /// <summary>
    /// Calls the handler method if the target has not been garbage collected.
    /// </summary>
    /// <param name="aSender"></param>
    /// <param name="anEventArgs"></param>
    public void Invoke(object aSender, TEventArgs anEventArgs)
    {
      base.Invoke(aSender, anEventArgs);
    }

    #endregion
  }
}