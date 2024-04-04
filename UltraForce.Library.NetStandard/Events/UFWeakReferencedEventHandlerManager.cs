// <copyright file="UFWeakReferencedEventHandlerManager.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2018 Ultra Force Development
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Events
{
  /// <summary>
  /// This class can be used to manage a <see cref="UFWeakReferencedEventHandler{TEventArgs}"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When adding an event handler to an event provider, the object implementing the event handler will not get garbage
  /// collected until either the event handler is removed or the object implementing the event provider is also
  /// garbage collected.
  /// </para>
  /// <para>
  /// This class can be used with objects that can not remove event handlers. It will manages
  /// <see cref="UFWeakReferencedEventHandler{TEventArgs}"/> so that the target still can get garbage collected.
  /// </para>
  /// <para>
  /// To use this class, create an instance of <see cref="UFWeakReferencedEventHandlerManager{TEventArgs}"/> and assign
  /// it to some private property or field in the target instance.
  /// </para>
  /// <para>
  /// When the instance is getting garbage collected it removes also the
  /// <see cref="UFWeakReferencedEventHandler{TEventArgs}"/> instance from the event provider.
  /// </para>
  /// <para>
  /// Do not forget to assign the instance of  <see cref="UFWeakReferencedEventHandlerManager{TEventArgs}"/> to a
  /// private field or property so that the instance only gets garbage collected once the target also gets garbage
  /// collected. Else the instance might get garbage collected sooner, resulting in removal of the event handler
  /// while it is still needed.
  /// </para>
  /// </remarks>
  [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
  public class UFWeakReferencedEventHandlerManager : IDisposable
  {
    #region private variables

    /// <summary>
    /// Stores references to event providers.
    /// </summary>
    /// <remarks>
    /// Use an static dictionary to store references. This will keep the WeakReference valid inside the finalizer.
    /// <para>
    /// When using a WeakReference directly as instance private field, the WeakReference finalizer might be called
    /// before this objects finalizer resulting in the WeakReference.Target being null.
    /// </para>
    /// <para>
    /// Quote from MSDN magazine:
    /// The runtime doesn't make any guarantees as to the order in which 
    /// Finalize methods are called. For example, let's say there is an
    /// object that contains a pointer to an inner object. The garbage 
    /// collector has detected that both objects are garbage. Furthermore, 
    /// say that the inner object's Finalize method gets called first. Now, 
    /// the outer object's Finalize method is allowed to access the inner
    /// object and call methods on it, but the inner object has been finalized 
    /// and the results may be unpredictable. For this reason, it is strongly 
    /// recommended that Finalize methods not access any inner, member objects.
    /// </para>
    /// <para>
    /// Use an object as key instead of the instance, else the instance would never get garbage collected.
    /// </para>
    /// </remarks>
    private static readonly Dictionary<object, WeakReference> s_providerReferences =
      new Dictionary<object, WeakReference>();

    /// <summary>
    /// Weak reference to handler method.
    /// </summary>
    // ReSharper disable once NotAccessedField.Local
    private readonly UFWeakReferencedEventHandler m_weakReferencedHandler;

    /// <summary>
    /// Key into m_providerReferences
    /// </summary>
    private readonly object m_providerReferenceKey;

    /// <summary>
    /// Name of event
    /// </summary>
    private readonly string m_eventName;

    /// <summary>
    /// True when event handler has been added
    /// </summary>
    private bool m_added;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance
    /// <see cref="UFWeakReferencedEventHandlerManager"/>.
    /// </summary>
    /// <remarks>
    /// The handler can not be static. Trying to use a static handler will 
    /// throw an exception.
    /// </remarks>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Provider that will fire events.
    /// </param>
    /// <param name="anEventName">
    /// Name of event to add handler to.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider; else call <see cref="Add"/> 
    /// to add the handler.
    /// </param>
    public UFWeakReferencedEventHandlerManager(
      EventHandler anHandler,
      object aProvider,
      string anEventName,
      bool anAdd = true
    ) : this(new UFWeakReferencedEventHandler(anHandler), aProvider, anEventName, anAdd)
    {
    }

    /// <summary>
    /// Constructs an instance
    /// <see cref="UFWeakReferencedEventHandlerManager"/>.
    /// </summary>
    /// <remarks>
    /// The handler can not be static. Trying to use a static handler will 
    /// throw an exception.
    /// </remarks>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Provider that will fire events.
    /// </param>
    /// <param name="anEventName">
    /// Name of event to add handler to.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider; else call <see cref="Add"/> 
    /// to add the handler.
    /// </param>
    public UFWeakReferencedEventHandlerManager(
      PropertyChangedEventHandler anHandler,
      object aProvider,
      string anEventName,
      bool anAdd = true
    ) : this(new UFWeakReferencedEventHandler(anHandler), aProvider, anEventName, anAdd)
    {
    }

    /// <summary>
    /// Constructs an instance
    /// <see cref="UFWeakReferencedEventHandlerManager"/>.
    /// </summary>
    /// <remarks>
    /// The handler can not be static. Trying to use a static handler will 
    /// throw an exception.
    /// </remarks>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Provider that will fire events.
    /// </param>
    /// <param name="anEventName">
    /// Name of event to add handler to.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider; else call <see cref="Add"/> 
    /// to add the handler.
    /// </param>
    public UFWeakReferencedEventHandlerManager(
      NotifyCollectionChangedEventHandler anHandler,
      object aProvider,
      string anEventName,
      bool anAdd = true
    ) : this(new UFWeakReferencedEventHandler(anHandler), aProvider, anEventName, anAdd)
    {
    }

    /// <summary>
    /// Constructs an instance
    /// <see cref="UFWeakReferencedEventHandlerManager"/>.
    /// </summary>
    /// <remarks>
    /// The handler can not be static. Trying to use a static handler will 
    /// throw an exception.
    /// </remarks>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Provider that will fire events.
    /// </param>
    /// <param name="anEventName">
    /// Name of event to add handler to.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider; else call <see cref="Add"/> 
    /// to add the handler.
    /// </param>
    protected UFWeakReferencedEventHandlerManager(
      UFWeakReferencedEventHandler anHandler,
      object aProvider,
      string anEventName,
      bool anAdd = true
    )
    {
      this.m_weakReferencedHandler = anHandler;
      this.m_providerReferenceKey = new object();
      s_providerReferences.Add(this.m_providerReferenceKey, new WeakReference(aProvider));
      this.m_eventName = anEventName;
      if (anAdd)
      {
        this.AddOrRemove(true);
      }
    }

    #endregion

    #region finalizer

    /// <summary>
    /// Removes the event handler from the provider.
    /// </summary>
    ~UFWeakReferencedEventHandlerManager()
    {
      this.CleanUp();
    }

    #endregion

    #region idisposable

    /// <inheritdoc />
    public void Dispose()
    {
      this.CleanUp();
      GC.SuppressFinalize(this);
    }

    #endregion

    #region public support methods

    /// <summary>
    /// Adds the managed handler to the event provider. If the handler is already added this method does nothing.
    /// </summary>
    public void Add()
    {
      this.AddOrRemove(true);
    }

    /// <summary>
    /// Removes the managed handler from the event provider. If the handler is already removed this method does nothing.
    /// </summary>
    public void Remove()
    {
      this.AddOrRemove(false);
    }

    #endregion

    #region public factory methods

    /// <summary>
    /// Create instance of <see cref="UFWeakReferencedEventHandlerManager{TEventArgs}"/> for
    /// <see cref="UFDataChangedEventArgs"/>.
    /// </summary>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Object that implements <see cref="IUFNotifyDataChanged"/>.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider.
    /// </param>
    /// <returns>
    /// Instance of
    /// <see cref="UFWeakReferencedEventHandlerManager{TEventArgs}"/> for
    /// <see cref="UFDataChangedEventArgs"/>.
    /// </returns>
    public static UFWeakReferencedEventHandlerManager<UFDataChangedEventArgs> Create(
      EventHandler<UFDataChangedEventArgs> anHandler,
      IUFNotifyDataChanged aProvider,
      bool anAdd = true
    )
    {
      return new UFWeakReferencedEventHandlerManager<UFDataChangedEventArgs>(
        anHandler,
        aProvider,
        nameof(aProvider.DataChanged),
        anAdd
      );
    }

    /// <summary>
    /// Create instance of <see cref="UFWeakReferencedEventHandlerManager"/> for
    /// <see cref="PropertyChangedEventHandler"/>.
    /// </summary>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Object that implements <see cref="INotifyPropertyChanged"/>.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider.
    /// </param>
    /// <returns>
    /// Instance of
    /// <see cref="UFWeakReferencedEventHandlerManager"/>.
    /// </returns>
    public static UFWeakReferencedEventHandlerManager Create(
      PropertyChangedEventHandler anHandler,
      INotifyPropertyChanged aProvider,
      bool anAdd = true
    )
    {
      return new UFWeakReferencedEventHandlerManager(anHandler, aProvider, nameof(aProvider.PropertyChanged), anAdd);
    }

    /// <summary>
    /// Create instance of <see cref="UFWeakReferencedEventHandlerManager{TEventArgs}"/> for
    /// <see cref="NotifyCollectionChangedEventArgs"/>.
    /// </summary>
    /// <param name="anHandler">
    /// Handler method to manage.
    /// </param>
    /// <param name="aProvider">
    /// Object that implements <see cref="INotifyCollectionChanged"/>.
    /// </param>
    /// <param name="anAdd">
    /// When <c>true</c> add handler to provider.
    /// </param>
    /// <returns>
    /// Instance of
    /// <see cref="UFWeakReferencedEventHandlerManager{TEventArgs}"/> for
    /// <see cref="NotifyCollectionChangedEventArgs"/>.
    /// </returns>
    public static UFWeakReferencedEventHandlerManager Create(
      NotifyCollectionChangedEventHandler anHandler,
      INotifyCollectionChanged aProvider,
      bool anAdd = true
    )
    {
      return new UFWeakReferencedEventHandlerManager(anHandler, aProvider, nameof(aProvider.CollectionChanged), anAdd);
    }

    #endregion

    #region private methods

    /// <summary>
    /// Cleans up references and removes event handler.
    /// </summary>
    private void CleanUp()
    {
      this.Remove();
      s_providerReferences.Remove(this.m_providerReferenceKey);
    }

    /// <summary>
    /// Adds or removes the event handler to or from the provider.
    /// </summary>
    /// <remarks>
    /// Based on code from:
    /// https://msdn.microsoft.com/en-us/library/ms228976(v=vs.95).aspx#Y899
    /// </remarks>
    /// <param name="anAdd">When true add handler.</param>
    private void AddOrRemove(bool anAdd)
    {
      // exit if already in the correct added state
      if (this.m_added == anAdd)
      {
        return;
      }
      // try to get reference
      bool hasValue = s_providerReferences.TryGetValue(
        this.m_providerReferenceKey,
        out WeakReference providerReference
      );
      // copy reference to local var, this will also prevent it from getting garbage collected while this method is
      // being executed
      object? provider = hasValue ? providerReference!.Target : null;
      // only proceed if provider still exists
      if (provider == null)
      {
        return;
      }
      EventInfo eventInfo = provider.GetType().GetRuntimeEvent(this.m_eventName);
      Type handlerType = eventInfo.EventHandlerType;
      IEnumerable<MethodInfo> list = typeof(UFWeakReferencedEventHandler).GetTypeInfo()
        .GetDeclaredMethods(
          nameof(UFWeakReferencedEventHandler.Invoke)
        );
      MethodInfo? handleEventMethodInfo =
        (from info in list let p = info.GetParameters() where p.Length == 2 select info).FirstOrDefault();
      if (handleEventMethodInfo == null)
      {
        throw new Exception("Could not find an invoke method with two parameters");
      }
      Delegate handleEventDelegate = handleEventMethodInfo.CreateDelegate(handlerType, this.m_weakReferencedHandler);
      MethodInfo changeMethod = anAdd ? eventInfo.AddMethod : eventInfo.RemoveMethod;
      object[] changeArguments = { handleEventDelegate };
      changeMethod.Invoke(provider, changeArguments);
      this.m_added = anAdd;
    }

    #endregion
  }

  /// <summary>
  /// A generic version of <see cref="UFWeakReferencedEventHandlerManager"/>
  /// </summary>
  /// <typeparam name="TEventArgs">Event arguments type</typeparam>
  public class UFWeakReferencedEventHandlerManager<TEventArgs> : UFWeakReferencedEventHandlerManager
    where TEventArgs : EventArgs
  {
    public UFWeakReferencedEventHandlerManager(
      EventHandler<TEventArgs> anHandler,
      object aProvider,
      string anEventName,
      bool anAdd = true
    ) : base(new UFWeakReferencedEventHandler<TEventArgs>(anHandler), aProvider, anEventName, anAdd)
    {
    }
  }
}