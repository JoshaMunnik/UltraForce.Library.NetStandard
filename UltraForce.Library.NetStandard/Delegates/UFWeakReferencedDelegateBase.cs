// <copyright file="UFWeakReferencedDelegateBase.cs" company="Ultra Force Development">
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
using System.Reflection;

namespace UltraForce.Library.NetStandard.Delegates
{
  /// <summary>
  /// A base class that stores a delegate as a weak reference to its target and the method the delegate invokes.
  /// <para>
  /// It is an abstract class and should never be instantiated directly. Use <see cref="UFWeakReferencedDelegate"/> for
  /// a general purpose implementation.
  /// </para>
  /// </summary>
  public abstract class UFWeakReferencedDelegateBase
  {
    #region private variables

    /// <summary>
    /// Reference to the target instance
    /// </summary>
    private readonly WeakReference? m_target;

    /// <summary>
    /// Reference to method information
    /// </summary>
    private readonly MethodInfo? m_method;

    #endregion

    #region constructors

    /// <summary>
    /// Stores a delegate as a weak reference to the target and the method that should be invoked.
    /// <para>
    /// Trying to use a static delegate will throw an exception.
    /// </para>
    /// </summary>
    /// <param name="aDelegate">
    /// Delegate to store
    /// </param>
    protected UFWeakReferencedDelegateBase(Delegate aDelegate)
    {
      // throw exception if target is not set
      if (aDelegate.Target == null)
      {
        throw new Exception("UFWeakReferencedDelegateBase: can not use static delegates");
      }
      // store the target object and method instead of storing a weak reference to the delegate because a weak reference
      // to the delegate gets removed with the next garbage collection round even if the target is still active.
      this.m_target = new WeakReference(aDelegate.Target);
      this.m_method = aDelegate.GetMethodInfo();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Instances are equal if both targets are still available and the targets are equal and the methods are equal.
    /// </summary>
    /// <param name="anObject">Object to compare to</param>
    /// <returns>
    /// <c>true</c> if object is a <see cref="UFWeakReferencedDelegateBase"/>
    /// and both targets and methods are equal.
    /// </returns>
    public override bool Equals(object? anObject)
    {
      if (anObject is UFWeakReferencedDelegateBase other)
      {
        return this.IsAlive && other.IsAlive
          && Equals(this.m_target?.Target, other.m_target?.Target)
          && Equals(this.m_method, other.m_method);
      }
      return false;
    }

    /// <summary>
    /// Generates an hash code based on the method and stored target.
    /// </summary>
    /// <returns>Hash code</returns>
    public override int GetHashCode()
    {
      unchecked
      {
        int targetHash = this.m_target is { IsAlive: true, Target: { } }
          ? this.m_target.Target.GetHashCode()
          : 0;
        int methodHash = this.m_method != null ? this.m_method.GetHashCode() : 0;
        return (targetHash * 397) ^ methodHash;
      }
    }

    #endregion

    #region public properties

    /// <summary>
    /// This read-only property returns <c>true</c> if the target is still available.
    /// </summary>
    public bool IsAlive => this.m_target?.IsAlive ?? false;

    #endregion

    #region protected methods

    /// <summary>
    /// Invokes the delegate if the target is still alive.
    /// </summary>
    /// <param name="anArguments">Arguments to invoke with</param>
    /// <returns>
    /// Result returned from delegate or null if target is no longer available.
    /// </returns>
    protected object? Invoke(params object[] anArguments)
    {
      if ((this.m_target == null) || (this.m_method == null))
      {
        return null;
      }
      return this.m_target.IsAlive ? this.m_method.Invoke(this.m_target.Target, anArguments) : null;
    }

    #endregion

    #region internal methods

    /// <summary>
    /// Calls <see cref="Invoke"/>. It is used by <see cref="UFWeakReferencedDelegateManagerBase"/>.
    /// </summary>
    /// <param name="anArguments">Arguments to invoke with</param>
    /// <returns>
    /// Result returned from delegate or null if target is no longer available.
    /// </returns>
    internal object? InternalInvoke(params object[] anArguments)
    {
      return this.Invoke(anArguments);
    }

    #endregion
  }
}