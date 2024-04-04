// <copyright file="UFBindingController.cs" company="Ultra Force Development">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UltraForce.Library.NetStandard.Controllers
{
  /// <summary>
  /// <see cref="UFBindingController{TElement,TAction}" /> can be used as a base class for controllers that bind some
  /// type of element to some type of action.
  /// <para>
  /// Bindings can be set via <see cref="BindAction(TElement,TAction)" /> or via
  /// <see cref="BindAction(TElement,TElement,TAction)" />.
  /// </para>
  /// <para>
  /// Actions for specific element can be search and started via <see cref="FindAndStartAction" />. This method will
  /// call <see cref="StartAction(TElement, TAction)" />, which must be implemented by a subclass.
  /// </para>
  /// <para>
  /// Subclasses can override <see cref="AreRelated" /> to see if two elements are related (in case an element is
  /// available at multiple places in the application).
  /// </para>
  /// </summary>
  /// <typeparam name="TElement">
  /// Type of element.
  /// </typeparam>
  /// <typeparam name="TAction">
  /// Type of action to bind to element.
  /// </typeparam>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  [SuppressMessage("ReSharper", "UnusedMemberHierarchy.Global")]
  [SuppressMessage("ReSharper", "UnusedParameter.Global")]
  public abstract class UFBindingController<TElement, TAction>
  where TElement : class
  where TAction : class
  {
    #region private vars

    /// <summary>
    /// Bindings between elements and actions
    /// </summary>
    private readonly List<UFBinding> m_bindings;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of 
    /// <see cref="UFBindingController{TElement,TAction}" />.
    /// </summary>
    protected UFBindingController()
    {
      this.m_bindings = new List<UFBinding>();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Initializes the controller.
    /// <para>
    /// The default implementation does nothing.
    /// </para> 
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// Starts the controller.
    /// <para>
    /// The default implementation does nothing.
    /// </para> 
    /// </summary>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Pauses the controller. 
    /// <para>
    /// The default implementation does nothing.
    /// </para> 
    /// </summary>
    public virtual void Pause()
    {
    }

    /// <summary>
    /// Resumes the controller. 
    /// <para>
    /// The default implementation does nothing.
    /// </para> 
    /// </summary>
    public virtual void Resume()
    {
    }

    #endregion

    #region protected abstract methods

    /// <summary>
    /// Starts an action for the specific element.
    /// <para>
    /// This method is called by <see cref="FindAndStartAction" />; subclasses
    /// must implement this method to do something meaningful.
    /// </para>
    /// </summary>
    /// <param name="anElement">Element action is started for</param>
    /// <param name="anAction">Action that is started</param>
    protected abstract void StartAction(TElement anElement, TAction anAction);

    #endregion

    #region protected methods

    /// <summary>
    /// Binds an action to a specific element.
    /// <para>
    /// It is possible to make multiple bindings to the same element.
    /// </para>
    /// </summary>
    /// <param name="anElement">Element to bind action to</param>
    /// <param name="anAction">Action to bind to element</param>
    protected void BindAction(TElement anElement, TAction anAction)
    {
      this.m_bindings.Add(new UFBinding(anElement, anAction));
    }

    /// <summary>
    /// Binds an action to a specific element with a specific parent.
    /// <para>
    /// This method can be used to distinguish between elements if an element 
    /// is present at multiple locations.
    /// </para>
    /// <para>
    /// It is possible to make multiple bindings to the same element.
    /// </para>
    /// </summary>
    /// <param name="anElement">Element to bind action to</param>
    /// <param name="aParent">Parent in which the element is contained</param>
    /// <param name="anAction">Action to bind to element</param>
    protected void BindAction(TElement anElement, TElement aParent, TAction anAction)
    {
      this.m_bindings.Add(new UFBinding(anElement, aParent, anAction));
    }

    /// <summary>
    /// Removes bindings between an element and action using a filter function.
    /// </summary>
    /// <param name="aFilter">
    /// Filter function that should return <c>true</c> if the binding should
    /// be removed.
    /// </param>
    protected void UnbindAction(Func<TElement, TAction, bool> aFilter)
    {
      for (int index = this.m_bindings.Count - 1; index >= 0; index--)
      {
        UFBinding binding = this.m_bindings[index];
        if ((binding.Parent == null) && aFilter(binding.Element, binding.Action))
        {
          this.m_bindings.RemoveAt(index);
        }
      }
    }

    /// <summary>
    /// Removes bindings between an element and action using a filter function.
    /// </summary>
    /// <param name="aFilter">
    /// Filter function that should return <c>true</c> if the binding should
    /// be removed.
    /// </param>
    protected void UnbindAction(Func<TElement, TElement, TAction, bool> aFilter)
    {
      for (int index = this.m_bindings.Count - 1; index >= 0; index--)
      {
        UFBinding binding = this.m_bindings[index];
        if ((binding.Parent != null) && aFilter(binding.Element, binding.Parent, binding.Action))
        {
          this.m_bindings.RemoveAt(index);
        }
      }
    }

    /// <summary>
    /// Removes a binding between an element and an action.
    /// </summary>
    /// <param name="anElement">Element to remove binding for</param>
    /// <param name="anAction">Action bound to the element</param>
    protected void UnbindAction(TElement anElement, TAction anAction)
    {
      this.UnbindAction((element, action) => element!.Equals(anElement) && action!.Equals(anAction));
    }

    /// <summary>
    /// Removes a binding between an element and an action.
    /// </summary>
    /// <param name="anElement">Element to remove binding for</param>
    /// <param name="aParent">Parent in which the element is contained</param>
    /// <param name="anAction">Action bound to the element</param>
    protected void UnbindAction(TElement anElement, TElement aParent, TAction anAction)
    {
      this.UnbindAction(
        (element, parent, action) => element.Equals(anElement) && parent.Equals(aParent) && action.Equals(anAction)
      );
    }

    /// <summary>
    /// Remove all bindings for a specific element (both actions and 
    /// callbacks).
    /// </summary>
    /// <param name="anElement">Element to remove binding for</param>
    protected void UnbindAll(TElement anElement)
    {
      this.UnbindAction((element, action) => element.Equals(anElement));
    }

    /// <summary>
    /// Removes all bindings for an element contained within a specific
    /// parent element.
    /// </summary>
    /// <param name="anElement">Element to remove binding for</param>
    /// <param name="aParent">Parent in which the element is contained</param>
    protected void UnbindAll(TElement anElement, TElement aParent)
    {
      this.UnbindAction((element, parent, action) => element.Equals(anElement) && parent.Equals(aParent));
    }

    /// <summary>
    /// Checks if an element is related to a parent element.
    /// <para>
    /// Subclasses must override this method to provider a meaningful implementation. The default implementation always
    /// returns true.
    /// </para>
    /// </summary>
    /// <param name="anElement">Child element</param>
    /// <param name="aParent">Parent element</param>
    /// <returns>
    /// <c>true</c> if the child element is contained within the parent 
    /// element.
    /// </returns>
    protected virtual bool AreRelated(TElement anElement, TElement aParent)
    {
      return true;
    }

    /// <summary>
    /// Searches for actions for a specific element and calls <see cref="StartAction(TElement, TAction)" /> for every
    /// found action.
    /// <para>
    /// If a parent element was set with the binding, the method will call <see cref="AreRelated" /> to check if
    /// <c>anElement</c> is valid for the binding.
    /// </para>
    /// </summary>
    /// <param name="anElement">Element to start action(s) for</param>
    protected void FindAndStartAction(TElement anElement)
    {
      foreach (UFBinding binding in this.m_bindings)
      {
        if (binding.Parent != null)
        {
          if (binding.Element.Equals(anElement) && this.AreRelated(anElement, binding.Parent))
          {
            this.StartAction(anElement, binding.Action);
          }
        }
        else
        {
          if (binding.Element.Equals(anElement))
          {
            this.StartAction(anElement, binding.Action);
          }
        }
      }
    }

    #endregion

    #region private classes

    /// <summary>
    /// This class stores a binding between an element, optionally a parent element and an action.
    /// </summary>
    private class UFBinding
    {
      public TElement Element { get; }
      public TElement? Parent { get; }
      public TAction Action { get; }

      public UFBinding(TElement anElement, TAction anAction)
      {
        this.Element = anElement;
        this.Action = anAction;
      }

      public UFBinding(TElement anElement, TElement aParent, TAction anAction)
      {
        this.Element = anElement;
        this.Parent = aParent;
        this.Action = anAction;
      }
    }

    #endregion
  }
}