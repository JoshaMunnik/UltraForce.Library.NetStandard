// <copyright file="UFWeakNotifyCollectionChangedHandlerHelper.cs" company="Ultra Force Development">
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
using System.Reflection;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace UltraForce.Library.NetStandard.Internal
{
  /// <summary>
  /// Stores an event handler as method and weak reference to the target.
  /// </summary>
  internal class UFWeakNotifyCollectionChangedHandlerHelper
  {
    #region private variables

    /// <summary>
    /// Reference to the target instance
    /// </summary>
    private readonly WeakReference m_instance;

    /// <summary>
    /// Reference to method information
    /// </summary>
    private readonly MethodInfo m_method;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of
    /// <see cref="UFWeakNotifyCollectionChangedHandlerHelper"/>
    /// </summary>
    /// <param name="aHandler"></param>
    public UFWeakNotifyCollectionChangedHandlerHelper(
      NotifyCollectionChangedEventHandler aHandler
    )
    {
      // store the target object and method instead of storing a weak 
      // reference to aHandler because a weak reference to aHandler
      // gets removed with the next garbage collection round even if the
      // target is still active.
      this.m_instance = new WeakReference(aHandler.Target);
      this.m_method = aHandler.GetMethodInfo();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Calls the wrapped handler method if it not has been 
    /// garbage collected.
    /// </summary>
    /// <param name="aSender"></param>
    /// <param name="anEventArgs"></param>
    public void Invoke(object aSender, NotifyCollectionChangedEventArgs anEventArgs)
    {
      if (this.m_instance.IsAlive)
      {
        this.m_method.Invoke(
          this.m_instance.Target,
          new[] { aSender, anEventArgs }
        );
      }
    }

    /// <inheritdoc />
    public override bool Equals(object? anObject)
    {
      if (anObject is UFWeakNotifyCollectionChangedHandlerHelper handler)
      {
        return (handler.m_instance.Target == this.m_instance.Target)
          && (handler.m_method == this.m_method);
      }
      return false;
    }

    #endregion
  }
}