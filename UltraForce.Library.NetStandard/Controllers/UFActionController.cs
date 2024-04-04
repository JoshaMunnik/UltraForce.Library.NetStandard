// <copyright file="UFActionController.cs" company="Ultra Force Development">
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
using System.Diagnostics.CodeAnalysis;
using UltraForce.Library.NetStandard.Controllers.Actions;

namespace UltraForce.Library.NetStandard.Controllers
{
  /// <summary>
  /// <see cref="UFActionController{T}" /> can be used as a base class for controllers that bind elements of a certain
  /// type to <see cref="IUFQueueableAction"/>.
  /// </summary>
  /// <remarks>
  /// The class also supports binding to callbacks using <see cref="UFCallbackAction" /> to encapsulate the callbacks
  /// and store the bindings.
  /// <para>
  /// Actions can also be started directly via <see cref="StartAction(IUFQueueableAction)" /> or
  /// <see cref="StartActionQueue"/>
  /// </para>
  /// </remarks>
  /// <typeparam name="TElement">
  /// Type of element, the class assumes the type supports the <see cref="object.Equals(object)" /> method.
  /// </typeparam>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  [SuppressMessage("ReSharper", "UnusedMemberHierarchy.Global")]
  [SuppressMessage("ReSharper", "UnusedParameter.Global")]
  public class UFActionController<TElement> : UFBindingController<TElement, IUFQueueableAction>
  where TElement : class
  {
    #region private vars

    /// <summary>
    /// List of running actions
    /// </summary>
    private readonly UFActionCollection m_runningActions;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFActionController{T}" />.
    /// </summary>
    public UFActionController()
    {
      this.m_runningActions = new UFActionCollection();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Pauses the controller. The  implementations tells all running actions to pause themselves via 
    /// <see cref="IUFPausableAction.Pause" />.
    /// </summary>
    public override void Pause()
    {
      base.Pause();
      this.m_runningActions.Pause();
    }

    /// <summary>
    /// Resumes the controller. The default implementations tells all running actions to resume themselves via 
    /// <see cref="IUFPausableAction.Resume" />.
    /// </summary>
    public override void Resume()
    {
      base.Resume();
      this.m_runningActions.Resume();
    }

    #endregion

    #region protected methods

    /// <summary>
    /// Binds a callback to a specific element. The method will use a <see cref="UFCallbackAction" />.
    /// <para>
    /// It is possible to make multiple bindings to the same element.
    /// </para>
    /// </summary>
    /// <param name="anElement">Element to bind action to</param>
    /// <param name="aCallback">Callback to bind to element</param>
    protected void BindAction(TElement anElement, Action aCallback)
    {
      this.BindAction(anElement, new UFCallbackAction(aCallback));
    }

    /// <summary>
    /// Binds a callback to a specific element with a specific parent. The method will use
    /// a <see cref="UFCallbackAction" />.
    /// <para>
    /// This method can be used to distinguish between elements if an element is present at multiple locations.
    /// </para>
    /// <para>
    /// It is possible to make multiple bindings to the same element.
    /// </para>
    /// </summary>
    /// <param name="anElement">Element to bind action to</param>
    /// <param name="aParent">Parent in which the element is contained</param>
    /// <param name="aCallback">Callback to bind to element</param>
    protected void BindAction(
      TElement anElement,
      TElement aParent,
      Action aCallback
    )
    {
      this.BindAction(anElement, aParent, new UFCallbackAction(aCallback));
    }

    /// <summary>
    /// Removes a binding between an element and a callback.
    /// </summary>
    /// <param name="anElement">Element to remove binding for</param>
    /// <param name="aCallback">Callback bound to the element</param>
    protected void UnbindAction(TElement anElement, Action aCallback)
    {
      this.UnbindAction((element, action) => 
        element.Equals(anElement) 
        && (action is UFCallbackAction callbackAction) 
        && (callbackAction.Callback == aCallback)
      );
    }

    /// <summary>
    /// Removes a binding between an element and a callback.
    /// </summary>
    /// <param name="anElement">Element to remove binding for</param>
    /// <param name="aParent">Parent in which the element is contained</param>
    /// <param name="aCallback">Callback bound to the element</param>
    protected void UnbindAction(TElement anElement, TElement aParent, Action aCallback)
    {
      this.UnbindAction((element, parent, action) => 
        element!.Equals(anElement) 
        && parent!.Equals(aParent) 
        && (action is UFCallbackAction callbackAction) 
        && (callbackAction.Callback == aCallback)
      );
    }

    /// <summary>
    /// Adds an action to running actions collection and starts it.
    /// </summary>
    /// <param name="anAction">Action to start</param>
    protected virtual void StartAction(IUFQueueableAction anAction)
    {
      this.m_runningActions.Start(anAction);
    }

    /// <summary>
    /// Helper method to create a <see cref="UFSerialQueueAction"/> and 
    /// call <see cref="StartAction(IUFQueueableAction)"/>
    /// </summary>
    /// <param name="anActions">Additional actions to run</param>
    protected void StartActionQueue(params IUFQueueableAction[] anActions)
    {
      this.StartAction(new UFSerialQueueAction(anActions));
    }

    #endregion

    #region UFBindingController

    /// <inheritdoc />
    protected override void StartAction(TElement anElement, IUFQueueableAction anAction)
    {
      // ignore element and start action like any other action
      this.StartAction(anAction);
    }

    #endregion
  }
}