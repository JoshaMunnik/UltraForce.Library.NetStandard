// <copyright file="UFModel.cs" company="Ultra Force Development">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Development;
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Models.Validators;
using UltraForce.Library.NetStandard.Storage;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Models
{
  /// <summary>
  /// Base class for data classes. 
  /// </summary>
  /// <remarks>
  /// <para>
  /// Subclasses should define properties, the set part should use the <see cref="Assign{T}" /> or
  /// <see cref="Set"/> method; so <see cref="UFModel" /> can fire <see cref="DataChanged" /> and
  /// <see cref="PropertyChanged" /> events when a property changes value.
  /// </para>
  /// <para>
  /// A subclass can use <see cref="Assign{T}"/> in combination with a private variable or a subclass can use
  /// <see cref="Set"/> and <see cref="Get{T}(Func{T},string)"/> or <see cref="Get{T}(T,string)"/>
  /// which will use an internally stored value. Using <see cref="Get{T}(Func{T},string)"/> or
  /// <see cref="Get{T}(T,string)"/> will be slightly slower then directly referencing a private variable.
  /// </para>
  /// <para>
  /// It is possible to <see cref="Lock" /> the instance; this prevent the class from firing events. While locked the
  /// class will keep track of any changed properties. When the class is unlocked with <see cref="Unlock" /> a
  /// single <see cref="DataChanged" /> event might fire and multiple <see cref="PropertyChanged" /> events might fire.
  /// </para>
  /// <para>
  /// <see cref="UFModel" /> can be used to access properties and fields of another class by passing the instance to
  /// the constructor. Change events will only be fired when values are assigned trough
  /// <see cref="SetPropertyValue(string,object)" />.
  /// </para>
  /// <para>
  /// If <see cref="Option.TrackChildChange" /> option is used <see cref="UFModel"/> installs change listeners at all
  /// property values that implement <see cref="IUFNotifyDataChanged" />. If the listener detects a change,
  /// <see cref="UFModel" /> will fire also a change event for that property. 
  /// </para>
  /// <para>
  /// For correct working, <see cref="Changed(string,object,object)" /> must be called when ever the property gets
  /// assigned a new value so that listeners can be removed from the old value and added to the new value.
  /// </para>
  /// <para>
  /// The methods <see cref="Assign{T}"/> and <see cref="Set"/> both call the
  /// <see cref="Changed(string,object,object)"/> whenever a new value is used.
  /// </para>
  /// <para>
  /// Annotate properties and fields with <see cref="UFIgnoreAttribute" /> to ignore them. These properties and fields
  /// can not be accessed trough their names and will be ignored with IO operations.
  /// </para>
  /// <para>
  /// <see cref="UFModel" /> also contains support for storing and retrieving itself from a
  /// <see cref="UFKeyedStorage" />. Annotate properties and fields with <see cref="UFIOIgnoreAttribute" /> to prevent
  /// them from being saved. Annotate with <see cref="UFIONameAttribute" /> to set a different name to store data with.
  /// A subclass can also override <see cref="CanSave"/> to control which properties and fields gets saved.
  /// </para>
  /// <para>
  /// If <c>UFDEBUG</c> is defined a <see cref="UFHtmlLog" /> instance can be set via the static <see cref="UseLog"/> to
  /// log any changes to properties and fields which are annotated with <see cref="UFLogAttribute"/>.
  /// </para>
  /// <para>
  /// Validators added via calls to <see cref="AddValidator(string,IUFValidateValue)"/> or
  /// <see cref="AddValidator(string,IUFValidateProperty)"/> from within <see cref="InitMeta"/> will be stored at
  /// meta level and will be shared by all instances.
  /// </para>
  /// <para>
  /// The <see cref="DataChanged"/>, <see cref="ChildChanged"/> and <see cref="PropertyChanged"/> use a weak reference
  /// to the object implementing the handler method.
  /// </para>
  /// </remarks>
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  public class UFModel : IUFStorableObject, IUFJsonExport, IUFLockable, INotifyPropertyChanged, IUFClearable, IUFModel
  {
    #region public types

    /// <summary>
    /// Options for the <see cref="UFModel"/>
    /// </summary>
    public enum Option
    {
      /// <summary>
      /// Calls to <see cref="Lock" /> and <see cref="Unlock" /> also processes all properties and will call 
      /// <see cref="IUFLockable.Lock" /> and <see cref="IUFLockable.Unlock" />  if a property is an object that
      /// implements <see cref="IUFLockable"/>.
      /// </summary>
      LockChildren,

      /// <summary>
      /// Do not process any properties. This option is only checked when the first instance is created (since property
      /// meta data is shared between all instances of a certain subclass)
      /// </summary>
      IgnoreProperties,

      /// <summary>
      /// Also process the fields. This option is only checked when the first instance is created (since property meta
      /// data is shared between all instances of a certain subclass)
      /// </summary>
      IncludeFields,

      /// <summary>
      /// Checks if a property implements <see cref="IUFNotifyDataChanged" />. If it does, listen for changes and fire
      /// <see cref="ChildChanged" />
      /// <para>
      /// When this option is used, changes to property values should result in calls to
      /// <see cref="UFModel.Changed(string,object,object)"/>. This will happen automatically when using 
      /// <see cref="UFModel.Assign{T}"/> or <see cref="UFModel.Set"/>.
      /// </para>
      /// </summary>
      TrackChildChange
    }

    #endregion

    #region private vars

    /// <summary>
    /// The meta data for every UFModel subclass.
    /// </summary>
    private static readonly Dictionary<Type, Dictionary<string, UFPropertyMetaData>> s_propertyMeta =
      new Dictionary<Type, Dictionary<string, UFPropertyMetaData>>();

    /// <summary>
    /// Additional data for every property in an instance.
    /// </summary>
    private readonly Dictionary<string, UFPropertyInstanceData> m_propertyInstances;

    /// <summary>
    /// Lock counter, if non zero the data instance is locked and no events are fired.
    /// </summary>
    private int m_locked;

    /// <summary>
    /// The options.
    /// </summary>
    private readonly List<Option> m_options;

    /// <summary>
    /// The data structure whose properties are managed.
    /// </summary>
    private readonly object m_data;

    /// <summary>
    /// Map property values to list of property names that contain the value.
    /// </summary>
    private readonly Dictionary<object, List<string>>? m_trackedProperties;

    /// <summary>
    /// Used when locked and Changed() without parameters is called.
    /// </summary>
    private bool m_selfChanged;

    /// <summary>
    /// Set to true while busy initializing the meta data.
    /// </summary>
    private bool m_initializingMeta;

    /// <summary>
    /// Current data changed token.
    /// </summary>
    private int m_dataChangedToken;

    /// <summary>
    /// Manager for <see cref="DataChanged"/>
    /// </summary>
    private readonly UFWeakReferencedEventManager<UFDataChangedEventArgs> m_dataChangedManager;

    /// <summary>
    /// Manager for <see cref="ChildChanged"/>
    /// </summary>
    private readonly UFWeakReferencedEventManager<UFDataChangedEventArgs> m_childChangedManager;

    /// <summary>
    /// Manager for <see cref="PropertyChanged"/>
    /// </summary>
    private readonly UFWeakReferencedPropertyChangedManager m_propertyChangedManager;

#if UFDEBUG
    /// <summary>
    /// Log to log changes to.
    /// </summary>
    private static UFHtmlLog? s_log;
#endif

    #endregion

    #region constructors

    /// <summary>
    /// The constructor, creates an instance and processes all properties.
    /// <para>
    /// If <see cref="Option.TrackChildChange" /> is used, subclasses must call <see cref="UpdateTrackedProperties"/>
    /// after assigning initial values to the properties.
    /// </para>
    /// </summary>
    /// <param name='aData'>
    /// An instance to manage the properties and fields of, when <c>null</c> <see cref="UFModel" /> uses itself.
    /// </param>
    /// <param name='anOptions'>
    /// Array of options to use.
    /// </param>
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public UFModel(object? aData = null, params Option[] anOptions)
    {
      // store options
      this.m_options = new List<Option>(anOptions);
      // use external data?
      this.m_data = aData ?? this;
      // not locked
      this.m_locked = 0;
      // not changed
      this.m_selfChanged = false;
      // create manager
      this.m_dataChangedManager = new UFWeakReferencedEventManager<UFDataChangedEventArgs>();
      this.m_childChangedManager = new UFWeakReferencedEventManager<UFDataChangedEventArgs>();
      this.m_propertyChangedManager = new UFWeakReferencedPropertyChangedManager();
      // get type
      Type type = this.m_data.GetType();
      // initialize meta when not yet available
      if (!s_propertyMeta.ContainsKey(type))
      {
        this.CreatePropertyMeta();
      }
      // create instance for every meta
      Dictionary<string, UFPropertyMetaData> meta = s_propertyMeta[type];
      this.m_propertyInstances = new Dictionary<string, UFPropertyInstanceData>(meta.Count);
      foreach (string key in meta.Keys)
      {
        this.m_propertyInstances[key] = new UFPropertyInstanceData(meta[key]);
      }
      // support bubbling?
      if (this.HasOption(Option.TrackChildChange))
      {
        this.m_trackedProperties = new Dictionary<object, List<string>>(this.m_propertyInstances.Count);
      }
    }

    /// <summary>
    /// The constructor, creates an instance and processes all properties.
    /// <para>
    /// If <see cref="Option.TrackChildChange" /> is used, subclasses must call <see cref="UpdateTrackedProperties"/>
    /// after assigning initial values to the properties.
    /// </para>
    /// </summary>
    /// <param name='anOptions'>
    /// Array of options to use.
    /// </param>
    public UFModel(params Option[] anOptions) : this(null, anOptions)
    {
    }

    #endregion

    #region public methods

    /// <summary>
    /// Sets a log to use to log changes for properties that are annotated with <see cref="UFLogAttribute"/>.
    /// <para>
    /// This method only does something if <c>UFDEBUG</c> has been defined.
    /// </para>
    /// </summary>
    /// <param name="aLog">Log to use</param>
    [Conditional("UFDEBUG")]
    public static void UseLog(UFHtmlLog aLog)
    {
#if UFDEBUG
      s_log = aLog;
#endif
    }

    /// <summary>
    /// Initializes or clears the properties of the data structure to default values. 
    /// <para>
    /// The default implementation sets all the properties that have a <see cref="DefaultValueAttribute"/> attached to
    /// the value specified by that attribute. 
    /// </para>
    /// <para>
    /// If the property has no <see cref="DefaultValueAttribute"/> attached the method will check if the property
    /// value has a method named <c>Clear</c> and will call that method if it does.
    /// </para>
    /// <para>
    /// As last step the method will call <see cref="UpdateTrackedProperties"/>
    /// </para>
    /// <para>
    /// Subclasses can override this method to initialize or clear additional properties.
    /// </para>
    /// <para>
    /// If there are properties that implement <see cref="IUFNotifyDataChanged"/> and
    /// <see cref="Option.TrackChildChange" /> is used, the clear method should make sure the instances are existing.
    /// </para>
    /// <param name='aCallChanged'>
    /// When <c>true</c> call <see cref="Changed()"/>. 
    /// </param>
    /// </summary>
    public virtual void Clear(bool aCallChanged)
    {
      Dictionary<string, UFPropertyMetaData> meta = s_propertyMeta[this.m_data.GetType()];
      if (aCallChanged)
      {
        this.Lock();
      }
      this.ClearInternalPropertyValues(aCallChanged);
      this.ClearPropertyValues(aCallChanged, meta);
      // update tracked properties
      this.UpdateTrackedProperties();
      // notify listeners
      if (aCallChanged)
      {
        this.Unlock();
      }
    }

    /// <summary>
    /// Gets the type for a property.
    /// </summary>
    /// <returns>The type.</returns>
    /// <param name="aPropertyName">A property name.</param>
    public Type GetPropertyType(string aPropertyName)
    {
      return this.m_propertyInstances[aPropertyName].Meta.Type;
    }

    /// <summary>
    /// Resets the internal lock counter, bringing the data class back to the unlocked state. No events are fired.
    /// <para>
    /// If <see cref="Option.LockChildren" /> is set, the <see cref="UFModel.ResetLock" /> method of all properties that 
    /// implement <see cref="UFModel" /> instance gets called as well.
    /// </para>
    /// </summary>
    public virtual void ResetLock()
    {
      this.m_locked = 0;
      // process child UFModel instances?
      if (this.HasOption(Option.LockChildren))
      {
        // call Lock on all child UFModel instances
        foreach (string name in this.m_propertyInstances.Keys)
        {
          if (this.GetPropertyValue(name) is UFModel item)
          {
            item.ResetLock();
          }
        }
      }
    }

    /// <summary>
    /// Checks if one or more properties have changed value while the <see cref="UFModel"/> instance is locked.
    /// <para>
    /// If the data instance is not locked, this method will always return <c>false</c>.
    /// </para>
    /// </summary>
    /// <returns>
    /// <c>true</c> one or more properties have changed value; otherwise, <c>false</c>.
    /// </returns>
    public bool HasChanged()
    {
      return this.m_selfChanged || this.m_propertyInstances.Values.Any(property => property.Changed);
    }

    /// <summary>
    /// Clears the changed state of properties. A call to unlock after a call to this method will not generate a
    /// DataChanged event.
    /// <para>
    /// Calling this method while the data instance is not locked, has no use.
    /// </para>
    /// </summary>
    public void ResetChanged()
    {
      foreach (UFPropertyInstanceData property in this.m_propertyInstances.Values)
      {
        property.Changed = false;
      }
      this.m_selfChanged = false;
    }

    /// <summary>
    /// Gets the name of all the properties managed by <see cref="UFModel"/>.
    /// </summary>
    /// <returns>
    /// The property names.
    /// </returns>
    public string[] GetPropertyNames()
    {
      string[] names = new string[this.m_propertyInstances.Keys.Count];
      this.m_propertyInstances.Keys.CopyTo(names, 0);
      return names;
    }

    /// <summary>
    /// Checks if the <see cref="UFModel"/> (sub)class contains a certain property.
    /// </summary>
    /// <param name="aName">A property name.</param>
    /// <returns>
    /// <c>true</c> if the property exists; otherwise, <c>false</c>.
    /// </returns>
    public bool HasProperty(string aName)
    {
      return this.m_propertyInstances.ContainsKey(aName);
    }

    /// <summary>
    /// Checks if all property have valid values. This method calls <see cref="IsValidPropertyValue"/> for every
    /// property.
    /// </summary>
    /// <returns>
    /// <c>true</c> if all values are valid; <c>false</c> if not
    /// </returns>
    public virtual bool IsValid()
    {
      return this.m_propertyInstances.Keys.All(
        propertyName => this.IsValidPropertyValue(propertyName, this.GetPropertyValue(propertyName))
      );
    }

    /// <summary>
    /// Sets the value of a property. 
    /// </summary>
    /// <param name='aPropertyName'>
    /// A property name.
    /// </param>
    /// <param name='aValue'>
    /// A value.
    /// </param>
    /// <param name="aCallChanged">
    /// When <c>true</c> call <see cref="Changed(string,object,object)"/> if the <c>aValue</c> is not equal to the
    /// current value of <c>aPropertyName</c>
    /// </param>
    public bool SetPropertyValue(string aPropertyName, object? aValue, bool aCallChanged)
    {
      object old = this.GetPropertyValue(aPropertyName);
      if (UFObjectTools.AreEqual(old, aValue))
      {
        return false;
      }
      this.m_propertyInstances[aPropertyName].Meta.SetValue(this.m_data, aValue);
      if (aCallChanged)
      {
        this.Changed(aPropertyName, old, aValue);
      }
      return true;
    }

    /// <summary>
    /// Copies the property values from this instance to another instance.
    /// <para>
    /// The method will use <see cref="SetPropertyValue(string,object,bool)" /> and <see cref="GetPropertyValue" /> to
    /// copy the values.
    /// </para>
    /// </summary>
    /// <param name="aTarget">
    /// Target to copy to
    /// </param>
    /// <param name="aCallChanged">
    /// When <c>true</c> call <see cref="Changed(string,object,object)"/> for every property value that is different.
    /// </param>
    public virtual void CopyTo(UFModel aTarget, bool aCallChanged = true)
    {
      aTarget.Lock();
      foreach (string propertyName in this.GetPropertyNames())
      {
        aTarget.SetPropertyValue(propertyName, this.GetPropertyValue(propertyName), aCallChanged);
      }
      aTarget.Unlock();
    }

    /// <summary>
    /// Copies the property values from another instance to this instance.
    /// <para>
    /// This method just calls <see cref="CopyTo" /> on <c>aSource</c> using this instance as the target.
    /// </para>
    /// </summary>
    /// <param name="aSource">
    /// Source to copy from
    /// </param>
    /// <param name="aCallChanged">
    /// When <c>true</c> call <see cref="Changed(string,object,object)"/> for every property value that is different.
    /// </param>
    public virtual void CopyFrom(UFModel aSource, bool aCallChanged = true)
    {
      aSource.CopyTo(this, aCallChanged);
    }

    /// <summary>
    /// The data changed token value will be used when firing a <see cref="DataChanged" /> event and can be used to
    /// match events to certain changes. 
    /// <para>
    /// The value is increased and copied every time when creating an <see cref="UFDataChangedEventArgs"/> instance. If
    /// the event handler changes the data resulting in new events the token will be increased again.
    /// </para>
    /// </summary>
    public int GetDataChangedToken()
    {
      return this.m_dataChangedToken;
    }

    /// <summary>
    /// Saves the model to a stream together with a version.
    /// </summary>
    /// <remarks>
    /// The method use <see cref="UFDictionaryStorage"/> to save the data.
    /// </remarks>
    /// <param name="aVersion">Version to save</param>
    /// <param name="aStream">Stream to write to</param>
    // ReSharper disable once BuiltInTypeReferenceStyle
    public void SaveToStream(Int32 aVersion, Stream aStream)
    {
      aStream.Write(BitConverter.GetBytes(aVersion), 0, 4);
      UFDictionaryStorage storage = new UFDictionaryStorage();
      this.SaveToStorage(storage);
      storage.SaveToStream(aStream);
    }

    /// <summary>
    /// Loads the model from a stream after validating the version.
    /// </summary>
    /// <param name="aVersion">Version that should be matched</param>
    /// <param name="aStream">Stream to read from</param>
    /// <returns>
    /// True if loaded successfully, false if the version is incorrect.
    /// </returns>
    // ReSharper disable once BuiltInTypeReferenceStyle
    public bool LoadFromStream(Int32 aVersion, Stream aStream)
    {
      byte[] buffer = new byte[4];
      if (aStream.Read(buffer, 0, 4) < 4)
      {
        return false;
      }
      if (BitConverter.ToInt32(buffer, 0) != aVersion)
      {
        return false;
      }
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.LoadFromStream(aStream);
      this.LoadFromStorage(storage);
      return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
      string result = "{" + this.GetType().Name + ":";
      foreach (string propertyName in this.GetPropertyNames())
      {
        object value = this.GetPropertyValue(propertyName);
        result += " " + propertyName + "=" + value;
      }
      return result + "}";
    }

    /// <summary>
    /// Gets the value for a property casting it to a certain type. 
    /// </summary>
    /// <typeparam name="T">Type to cast to</typeparam>
    /// <param name="aPropertyName">Name of property</param>
    /// <returns>Value</returns>
    public T GetPropertyValue<T>(string aPropertyName) where T : struct 
    {
      return (T) this.GetPropertyValue(aPropertyName);
    }

    #endregion

    #region public json methods

    /// <summary>
    /// Create JSON formatted string from the data.
    /// </summary>
    /// <returns>JSON formatted string</returns>
    public string SaveJson()
    {
      StringBuilder result = new StringBuilder();
      this.SaveJson(result);
      return result.ToString();
    }

    #endregion

    #region IUFClearable

    /// <summary>
    /// Clears the instance by calling <see cref="Clear(bool)"/> with 
    /// <c>true</c>.
    /// </summary>
    public void Clear()
    {
      this.Clear(true);
    }

    #endregion

    #region IUFLockable

    /// <summary>
    /// Call lock to prevent <see cref="DataChanged"/> events from occurring until <see cref="Unlock"/> is called.
    /// <para>
    /// While the instances is locked, changes are still tracked and with the 
    /// last unlock an event will be fired containing all changed properties.
    /// </para>
    /// <para>
    /// The number of lock and unlock calls must match before events are fired 
    /// again.
    /// </para>
    /// <para>
    /// If <see cref="Option.LockChildren"/> is set, the 
    /// <see cref="IUFLockable.Lock"/> method of all properties that implement 
    /// <see cref="IUFLockable" /> gets called as well.
    /// </para>
    /// </summary>
    /// <returns>
    /// Current lock counter
    /// </returns>
    public virtual int Lock()
    {
      this.m_locked++;
      // process child UFModel instances?
      if (this.HasOption(Option.LockChildren))
      {
        // call Lock on all child instances that implement IUFLockable
        foreach (
          IUFLockable item in this.m_propertyInstances.Keys.Select(this.GetPropertyValue).OfType<IUFLockable>()
        )
        {
          item.Lock();
        }
      }
      return this.m_locked;
    }

    /// <summary>
    /// Unlock to allow <see cref="DataChanged"/> events. If any property changed while the data was locked, a
    /// <see cref="DataChanged"/> event is fired with all properties that changed.
    /// <para>
    /// If <see cref="Option.LockChildren"/> is set, the unlock method of all properties that implement
    /// <see cref="IUFLockable" /> instance gets called as well.
    /// </para>
    /// </summary>
    /// <returns>
    /// Current lock counter
    /// </returns>
    public virtual int Unlock()
    {
      // if unlock has been called to many times, just return 0 and do nothing
      if (this.m_locked <= 0)
      {
        return 0;
      }
      // process child UFModel instances?
      if (this.HasOption(Option.LockChildren))
      {
        // call Unlock on all child IUFLockable instances; this model instance is still locked so any change events
        // fired by the children while Option.TrackChildren is used will still be captured.
        foreach (
          IUFLockable item in this.m_propertyInstances.Keys.Select(this.GetPropertyValue).OfType<IUFLockable>()
        )
        {
          item.Unlock();
        }
      }
      // decrease lock
      this.m_locked--;
      // build event if anything changed
      if (this.m_locked == 0)
      {
        // get current token
        int token = this.m_dataChangedToken;
        // create event with current token
        UFDataChangedEventArgs changedEvent = new UFDataChangedEventArgs(token);
        bool doEvent = false;
        foreach (
          UFPropertyInstanceData property in this.m_propertyInstances.Values.Where(property => property.Changed)
        )
        {
          changedEvent.AddChanged(property.Meta.Name, property.OldValue, property.Meta.GetValue(this));
          doEvent = true;
          property.Changed = false;
          // need to fire INotifyPropertyChanged individually for every property
          this.OnPropertyChanged(this, new PropertyChangedEventArgs(property.Meta.Name));
        }
        // at least one property changed?
        if (doEvent)
        {
          // advance to next token
          this.m_dataChangedToken++;
          // fire event
          this.OnDataChanged(changedEvent);
        }
        // no properties changed, but a call to Changed() was made?
        else if (this.m_selfChanged)
        {
          // advance to next token
          this.m_dataChangedToken++;
          // fire DataChanged without any property reference
          this.m_selfChanged = false;
          this.OnDataChanged(changedEvent);
        }
      }
      return this.m_locked;
    }

    #endregion

    #region IUFModel

    /// <summary>
    /// Gets the value for a property.
    /// </summary>
    /// <returns>
    /// The value.
    /// </returns>
    /// <param name='aPropertyName'>
    /// A property name.
    /// </param>
    public object GetPropertyValue(string aPropertyName)
    {
      return this.m_propertyInstances[aPropertyName].Meta.GetValue(this.m_data);
    }

    /// <summary>
    /// Sets the value of a property. If the value is different the <see cref="Changed(string,object,object)"/> method
    /// is called.
    /// </summary>
    /// <param name='aPropertyName'>
    /// A property name.
    /// </param>
    /// <param name='aValue'>
    /// A value.
    /// </param>
    /// <returns>
    /// True if <c>aValue</c> is different from the current value and 
    /// has been assigned.
    /// </returns>
    public bool SetPropertyValue(string aPropertyName, object? aValue)
    {
      return this.SetPropertyValue(aPropertyName, aValue, true);
    }

    /// <summary>
    /// Validate a value for a certain property. 
    /// <para>
    /// The default implementation checks the registered validators. Subclasses can override this method to use
    /// alternative validation.
    /// </para>
    /// </summary>
    /// <returns>
    /// <c>true</c> if the value is valid for the property; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='aPropertyName'>
    /// Property to validate value for
    /// </param>
    /// <param name='aValue'>
    /// Value to validate.
    /// </param>
    public virtual bool IsValidPropertyValue(string aPropertyName, object? aValue)
    {
      // non existing properties are always valid
      if (!this.m_propertyInstances.ContainsKey(aPropertyName))
      {
        return true;
      }
      // shortcut
      UFPropertyInstanceData property = this.m_propertyInstances[aPropertyName];
      // store the value as most recent value
      property.RecentValue = aValue;
      // validate property
      return property.IsValid(aValue, this);
    }

    /// <summary>
    /// This event is fired when one or more property value changed.
    /// </summary>
    public event EventHandler<UFDataChangedEventArgs> DataChanged {
      add => this.m_dataChangedManager.Add(value);
      remove => this.m_dataChangedManager.Remove(value);
    }

    #endregion

    #region iufstorableobject

    /// <summary>
    /// Stores data in a keyed storage. 
    /// <para>
    /// The method uses the <see cref="UFIOIgnoreAttribute"/> and <see cref="CanSave"/> to determine if a property can
    /// be stored.
    /// </para>
    /// <para>
    /// The method supports the <see cref="UFIONameAttribute"/>.
    /// </para>
    /// <para>
    /// If a property implements <see cref="IUFStorableObject"/>, the data is stored via
    /// <see cref="IUFStorableObject.SaveToStorage"/>.
    /// </para>
    /// </summary>
    /// <param name="aStorage">A storage to store data in.</param>
    public virtual void SaveToStorage(UFKeyedStorage aStorage)
    {
      foreach (
        UFPropertyInstanceData property in this.m_propertyInstances.Values.Where(property => !property.Meta.IOIgnore)
      )
      {
        object value = this.GetPropertyValue(property.Meta.Name);
        if (this.CanSave(property.Meta.Name, value))
        {
          aStorage.SetObject(property.Meta.IOName!.GetIOName(property.Meta.Name), value, property.Meta.Type);
        }
      }
    }

    /// <summary>
    /// Gets data from a keyed storage. 
    /// <para>
    /// The method uses the <see cref="UFIOIgnoreAttribute"/> to determine 
    /// if a property can be retrieved.
    /// </para>
    /// <para>
    /// The method supports the <see cref="UFIONameAttribute"/>.
    /// </para>
    /// <para>
    /// If a property is read only but implements the 
    /// <see cref="IUFStorableObject"/>, it will get initialized from the
    /// storage via <see cref="IUFStorableObject.LoadFromStorage"/>.
    /// </para>
    /// </summary>
    /// <param name="aStorage">A storage to get data from.</param>
    public virtual void LoadFromStorage(UFKeyedStorage aStorage)
    {
      // prevent events
      this.Lock();
      this.Clear();
      foreach (
        UFPropertyInstanceData property in this.m_propertyInstances.Values.Where(property => !property.Meta.IOIgnore)
      )
      {
        // set new value from storage if not readonly
        if (!property.Meta.ReadOnly)
        {
          this.SetPropertyValue(
            property.Meta.Name,
            aStorage.GetObject(property.Meta.IOName!.GetIOName(property.Meta.Name), property.Meta.Type)
          );
        }
        else
        {
          // handle readonly objects that implement the IUFStorableObject
          if (property.Meta.GetValue(this.m_data) is IUFStorableObject storable)
          {
            aStorage.GetStorableObject(property.Meta.IOName!.GetIOName(property.Meta.Name), storable);
          }
        }
      }
      this.Unlock();
    }

    #endregion

    #region IUFJsonExport

    /// <summary>
    /// Add data to <see cref="StringBuilder"/> using JSON formatting.
    /// <para>
    /// The method uses the <see cref="UFIOIgnoreAttribute"/> annotation and <see cref="CanSave"/> to determine 
    /// if a property can be saved.
    /// </para>
    /// <para>
    /// The method supports <see cref="UFIONameAttribute"/> annotation.
    /// </para>
    /// </summary>
    /// <param name="aBuilder">A builder to add data to.</param>
    public virtual void SaveJson(StringBuilder aBuilder)
    {
      bool firstValue = true;
      aBuilder.Append("{");
      foreach (
        UFPropertyInstanceData property in this.m_propertyInstances.Values.Where(property => !property.Meta.IOIgnore)
      )
      {
        object value = this.GetPropertyValue(property.Meta.Name);
        if (this.CanSave(property.Meta.Name, value))
        {
          if (!firstValue)
          {
            aBuilder.Append(",");
          }
          UFJsonTools.SaveString(aBuilder, property.Meta.IOName!.GetJSONName(property.Meta.Name));
          aBuilder.Append(":");
          UFJsonTools.SaveValue(aBuilder, value);
        }
        firstValue = false;
      }
      aBuilder.Append("}");
    }

    #endregion

    #region INotifyPropertyChanged

    /// <summary>
    /// This event is fired when a property changes. If the data was locked and is unlocked, multiple events will
    /// fire (once for every property that is changed).
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged {
      add => this.m_propertyChangedManager.Add(value);
      remove => this.m_propertyChangedManager.Remove(value);
    }

    #endregion

    #region events

    /// <summary>
    /// This event is fired when <see cref="Option.TrackChildChange"/> is used and one of the child properties that
    /// implements the <see cref="IUFNotifyDataChanged"/> fires an <see cref="IUFNotifyDataChanged.DataChanged"/> event.
    /// <para>
    /// The sender of this event will be the original child property instance that fired the event.
    /// </para>
    /// </summary>
    public event EventHandler<UFDataChangedEventArgs> ChildChanged {
      add => this.m_childChangedManager.Add(value);
      remove => this.m_childChangedManager.Remove(value);
    }

    #endregion

    #region protected overridable methods

    /// <summary>
    /// This method gets called the first time an instance of certain <see cref="UFModel" /> subclass is created.
    /// <para>
    /// This subclass can contain calls to  <see cref="AddValidator(string,IUFValidateValue)" /> and 
    /// <see cref="AddValidator(string,IUFValidateProperty)"/>. The validators are stored in the meta and will be
    /// shared by all instances.
    /// </para>
    /// <para>
    /// The default implementation does nothing.
    /// </para>
    /// </summary>
    protected virtual void InitMeta()
    {
    }

    /// <summary>
    /// Check if a certain property can be saved. 
    /// <para>
    /// The default implementation checks if a default value has been set via <see cref="DefaultValueAttribute"/> and
    /// returns <c>true</c> if <c>aValue</c> is not equal to the default value.
    /// </para>
    /// <para>
    /// If no default value has been specified, the method just returns true.
    /// </para>
    /// </summary>
    /// <param name="aPropertyName">A property name.</param>
    /// <param name="aValue">Current property value.</param>
    /// <returns>
    /// <c>true</c> if the property and its value should be saved; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanSave(string aPropertyName, object? aValue)
    {
      UFPropertyMetaData meta = s_propertyMeta[this.m_data.GetType()][aPropertyName];
      if (meta.DefaultValue != null)
      {
        return !UFObjectTools.AreEqual(meta.DefaultValue.Value, aValue);
      }
      return true;
    }

    /// <summary>
    /// Fire a DataChanged event without specifying a property. If the instance is locked, the change gets stored and
    /// the event will fire when the instance gets unlocked.
    /// </summary>
    protected virtual void Changed()
    {
      if (this.m_locked > 0)
      {
        this.m_selfChanged = true;
      }
      else
      {
        this.m_dataChangedToken++;
        this.OnDataChanged(
          new UFDataChangedEventArgs(this.m_dataChangedToken)
        );
      }
    }

    /// <summary>
    /// Fire a DataChanged event for a property. If the instance is locked, the changed info gets stored and it is
    /// used when the instance gets unlocked.
    /// </summary>
    /// <param name='aPropertyName'>
    /// The property that changed.
    /// </param>
    /// <param name='anOldValue'>
    /// The old value.
    /// </param>
    /// <param name='aNewValue'>
    /// The new value. If <c>null</c>, the method uses <see cref="GetPropertyValue"/> to get the current value.
    /// </param>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    protected virtual void Changed(string aPropertyName, object? anOldValue = null, object? aNewValue = null)
    {
      // shortcut to property instance and meta data
      UFPropertyInstanceData property = this.m_propertyInstances[aPropertyName];
      // if aNewValue is the default value null, use the current property value as new value
      if (aNewValue == null)
      {
        aNewValue = this.GetPropertyValue(aPropertyName);
      }
      this.LogNewValue(aPropertyName, aNewValue);
      // just store value if instance is locked
      if (this.m_locked > 0)
      {
        property.RecentValue = aNewValue;
        // only store old value once
        if (!property.Changed)
        {
          property.OldValue = anOldValue;
          property.Changed = true;
        }
      }
      else
      {
        // advance to next value
        this.m_dataChangedToken++;
        // fire event
        this.OnDataChanged(new UFDataChangedEventArgs(
          this.m_dataChangedToken,
          aPropertyName,
          anOldValue,
          aNewValue
        ));
        // fire INotifyPropertyChanged event
        this.OnPropertyChanged(
          this,
          new PropertyChangedEventArgs(aPropertyName)
        );
      }
      // update handlers when tracking child changes
      if (this.HasOption(Option.TrackChildChange))
      {
        this.UpdateTrackedProperty(property, aNewValue);
      }
    }

    /// <summary>
    /// This method is called when the <see cref="Option.TrackChildChange"/> is used.
    /// <para>
    /// The default implementation does nothing.
    /// </para>
    /// </summary>
    /// <param name="aValue">
    /// Property value (is the sender of the event)  </param>
    /// <param name="aPropertyNames">
    /// One or more property names the value is assigned to 
    /// </param>
    /// <param name="anEvent">
    /// The change event
    /// </param>
    protected virtual void ChildHasChanged(object aValue, string[] aPropertyNames, UFDataChangedEventArgs anEvent)
    {
    }

    /// <summary>
    /// Triggers the <see cref="ChildChanged"/> event.
    /// </summary>
    /// <param name="aSender">Sender of the event</param>
    /// <param name="anEvent">Event data</param>
    protected virtual void OnChildChanged(
      object aSender,
      UFDataChangedEventArgs anEvent
    )
    {
      this.m_childChangedManager.Invoke(aSender, anEvent);
    }

    /// <summary>
    /// Triggers the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="aSender">Sender of event</param>
    /// <param name="anEvent">Event data</param>
    protected virtual void OnPropertyChanged(
      object aSender,
      PropertyChangedEventArgs anEvent
    )
    {
      this.m_propertyChangedManager.Invoke(aSender, anEvent);
    }

    /// <summary>
    /// Triggers the <see cref="DataChanged"/> event.
    /// </summary>
    /// <param name='anEvent'>
    /// The event
    /// </param>
    protected virtual void OnDataChanged(UFDataChangedEventArgs anEvent)
    {
      this.m_dataChangedManager.Invoke(this, anEvent);
    }

    #endregion

    #region protected methods

    /// <summary>
    /// If <see cref="Option.TrackChildChange"/> has been set, this method will check all properties and updates
    /// listeners for properties that contain values that implement the <see cref="IUFNotifyDataChanged"/>.
    /// <para>
    /// This method should be called from within the <see cref="Clear()" /> method if new instances are assigned to one
    /// or more properties directly without any calls to <see cref="Changed(string,object,object)"/>.
    /// </para>
    /// </summary>
    protected void UpdateTrackedProperties()
    {
      if (this.HasOption(Option.TrackChildChange))
      {
        foreach (UFPropertyInstanceData property in this.m_propertyInstances.Values)
        {
          this.UpdateTrackedProperty(property);
        }
      }
    }

    /// <summary>
    /// Adds a value validator for a certain property. If this method is called from within the
    /// <see cref="InitMeta"/> method, the validator is stored at meta level and shared by all property instances.
    /// </summary>
    /// <param name='aPropertyName'>
    /// A property name.
    /// </param>
    /// <param name='aValidator'>
    /// The validator to attach.
    /// </param>
    protected void AddValidator(string aPropertyName, IUFValidateValue aValidator)
    {
      Dictionary<string, UFPropertyMetaData> meta = s_propertyMeta[this.m_data.GetType()];
      if (meta.ContainsKey(aPropertyName))
      {
        if (this.m_initializingMeta)
        {
          meta[aPropertyName].ValueValidators!.Add(aValidator);
        }
        else
        {
          this.m_propertyInstances[aPropertyName].ValueValidators.Add(aValidator);
        }
      }
    }

    /// <summary>
    /// Adds a property validator for a certain property. If this method is called from within the
    /// <see cref="InitMeta"/> method, the validator is stored at meta level and shared by all property instances.
    /// </summary>
    /// <param name='aPropertyName'>
    /// A property name.
    /// </param>
    /// <param name='aValidator'>
    /// The validator to attach.
    /// </param>
    protected void AddValidator(string aPropertyName, IUFValidateProperty aValidator)
    {
      Dictionary<string, UFPropertyMetaData> meta = s_propertyMeta[this.m_data.GetType()];
      if (meta.ContainsKey(aPropertyName))
      {
        if (this.m_initializingMeta)
        {
          meta[aPropertyName].PropertyValidators!.Add(aValidator);
        }
        else
        {
          this.m_propertyInstances[aPropertyName].PropertyValidators.Add(aValidator);
        }
      }
    }

    /// <summary>
    /// Checks if a value is different; assigns a new value to a private var that represents a property and calls
    /// <see cref="Changed(string,object,object)" /> to fire an event.
    /// </summary>
    /// <typeparam name='T'>
    /// The type of the property.
    /// </typeparam>
    /// <param name='aPrivateVar'>
    /// The private variable to get the current value from and assign the new value to.
    /// </param>
    /// <param name='aNewValue'>
    /// A new value.
    /// </param>
    /// <param name='aPropertyName'>
    /// A property name. This parameter is optional and does not need to be set when calling this method from inside
    /// the property setter.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value has changed; otherwise <c>false</c>.
    /// </returns>
    protected virtual bool Assign<T>(ref T aPrivateVar, T aNewValue, [CallerMemberName] string aPropertyName = "")
    {
      if (!UFObjectTools.AreEqual(aPrivateVar, aNewValue))
      {
        T oldValue = aPrivateVar;
        aPrivateVar = aNewValue;
        this.m_propertyInstances[aPropertyName].RecentValue = aNewValue;
        this.Changed(aPropertyName, oldValue, aNewValue);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Checks if a value is different; if it is the method stores the value internally and
    /// calls <see cref="Changed(string,object,object)" /> to fire an event.
    /// <para>
    /// Use <see cref="Get{T}(T,string)"/> or <see cref="Get{T}(Func{T},string)"/> to retrieve the internally stored
    /// value.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Subclasses can override this method to store the value at some other location.
    /// <para>
    /// The default implementation either stores the value (the first time) or calls <see cref="Assign{T}"/> to assign
    /// the new value.
    /// </para>
    /// </remarks>
    /// <param name='aNewValue'>
    /// wA new value.
    /// </param>
    /// <param name='aPropertyName'>
    /// A property name. This parameter is optional and does not need to be set
    /// when calling this method from inside the property setter.
    /// </param>
    /// <returns>
    /// <c>true</c> if the value has changed; otherwise <c>false</c>.
    /// </returns>
    protected virtual bool Set(object? aNewValue, [CallerMemberName] string aPropertyName = "")
    {
      UFPropertyInstanceData data = this.m_propertyInstances[aPropertyName];
      if (data.CurrentValueUsed)
      {
        return this.Assign(ref data.CurrentValue, aNewValue, aPropertyName);
      }
      data.CurrentValueUsed = true;
      data.CurrentValue = aNewValue;
      data.RecentValue = aNewValue;
      this.Changed(aPropertyName, null, aNewValue);
      return true;
    }

    /// <summary>
    /// Gets an internally stored value for a property.
    /// <para>
    /// Use <see cref="Set"/> to store a value internally.
    /// </para>
    /// <para>
    /// This method will call <see cref="Get{T}(Func{T},string)"/> with a simple factory function that
    /// returns <c>aDefaultValue</c>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the property
    /// </typeparam>
    /// <param name="aDefaultValue">
    /// Default value to use when no value was stored.
    /// </param>
    /// <param name="aPropertyName">
    /// A property name. This parameter is optional and does not need to be set
    /// when calling this method from inside the property getter.
    /// </param>
    /// <returns>The value of the property</returns>
    protected T Get<T>(T aDefaultValue, [CallerMemberName] string aPropertyName = "")
    {
      return this.Get(() => aDefaultValue, aPropertyName);
    }

    /// <summary>
    /// Gets an internally stored value for a property.
    /// <para>
    /// Use <see cref="Set"/> to store a value internally.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Subclasses can override this method to retrieve the value from some other location.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the property
    /// </typeparam>
    /// <param name="aFactory">
    /// Factory function that returns a value to be used as default value.
    /// </param>
    /// <param name="aPropertyName">
    /// A property name. This parameter is optional and does not need to be set
    /// when calling this method from inside the property getter.
    /// </param>
    /// <returns>The value of the property</returns>
    protected virtual T Get<T>(Func<T> aFactory, [CallerMemberName] string aPropertyName = "")
    {
      UFPropertyInstanceData data = this.m_propertyInstances[aPropertyName];
      return data.CurrentValueUsed ? (T)data.CurrentValue! : aFactory();
    }

    /// <summary>
    /// Check if a certain option is enabled.
    /// </summary>
    /// <returns>
    /// <c>true</c> if anOption is enabled; otherwise, <c>false</c>.
    /// </returns>
    /// <param name='anOption'>
    /// Option to check
    /// </param>
    protected bool HasOption(Option anOption)
    {
      return this.m_options.Contains(anOption);
    }

    /// <summary>
    /// Checks if <see cref="UFLogAttribute"/> has been set for the property.
    /// If it set, a log entry for the new value is added to the log set
    /// by <see cref="UseLog"/>.
    /// <remarks>
    /// This method only does something if <c>UFDEBUG</c> has been defined.
    /// </remarks>
    /// </summary>
    /// <param name="aPropertyName">
    /// Property name to log value for
    /// </param>
    /// <param name="aNewValue">
    /// Value to log
    /// </param>
    [Conditional("UFDEBUG")]
    protected void LogNewValue(string aPropertyName, object? aNewValue)
    {
#if UFDEBUG
      if (s_log != null)
      {
        UFPropertyInstanceData property = this.m_propertyInstances[aPropertyName];
        if (property.Meta.LogChanges)
        {
          string valueAsString = aNewValue != null ? aNewValue.ToString() : "null";
          s_log.Add(this, "new value: {0}={1}", aPropertyName, valueAsString);
        }
      }
#endif
    }

    #endregion

    #region private methods

    /// <summary>
    /// Creates property meta instances for the m_data.type
    /// </summary>
    private void CreatePropertyMeta()
    {
      // shortcut
      Type type = this.m_data.GetType();
      // get properties or use empty array when properties should be ignored
      PropertyInfo[] properties = this.HasOption(Option.IgnoreProperties)
        ? new PropertyInfo[] { }
        : type.GetRuntimeProperties().ToArray();
      // get fields or use empty array when fields should be ignored
      FieldInfo[] fields = !this.HasOption(Option.IncludeFields)
        ? new FieldInfo[] { }
        : type.GetRuntimeFields().ToArray();
      // create dictionary with enough capacity to hold all properties and fields
      Dictionary<string, UFPropertyMetaData> meta = new Dictionary<string, UFPropertyMetaData>(
        properties.Length + fields.Length
      );
      // add all properties and fields (except those that should be ignored)
      foreach (PropertyInfo propertyInfo in properties)
      {
        if ((UFAttributeTools.Find<UFIgnoreAttribute>(propertyInfo) == null))
        {
          meta[propertyInfo.Name] = new UFPropertyMetaData(propertyInfo);
        }
      }
      foreach (FieldInfo fieldInfo in fields)
      {
        if ((UFAttributeTools.Find<UFIgnoreAttribute>(fieldInfo) == null))
        {
          meta[fieldInfo.Name] = new UFPropertyMetaData(fieldInfo);
        }
      }
      // store meta data
      s_propertyMeta[type] = meta;
      // add validators 
      try
      {
        this.m_initializingMeta = true;
        this.InitMeta();
      }
      finally
      {
        this.m_initializingMeta = false;
      }
    }

    /// <summary>
    /// Installs a data changed listener for a property value if the value implements
    /// <see cref="IUFNotifyDataChanged"/>.
    /// <para>
    /// Also updates the previously tracked instance (if there is any)
    /// </para>
    /// </summary>
    /// <param name="aProperty">Property to update listener for</param>
    /// <param name="aValue">Property value or <c>null</c> to get value</param>
    private void UpdateTrackedProperty(UFPropertyInstanceData aProperty, object? aValue = null)
    {
      // get value if null
      aValue ??= aProperty.Meta.GetValue(this.m_data);
      // get new tracking object and typecast to IUFNotifyDataChanged (will be null if it can not be typecast).
      IUFNotifyDataChanged? newTracking = aValue as IUFNotifyDataChanged;
      // only proceed if the property contains a new value
      if (newTracking == aProperty.CurrentTracking)
      {
        return;
      }
      // the property currently has a IUFNotifyDataChanged instance value that is being tracked?
      if ((aProperty.CurrentTracking != null) && this.m_trackedProperties!.ContainsKey(aProperty.CurrentTracking))
      {
        // get names list for current tracking value 
        List<string> names = this.m_trackedProperties[aProperty.CurrentTracking];
        // remove name of this property 
        names.Remove(aProperty.Meta.Name);
        // stop tracking if there are no other properties the current tracked child is assigned to
        if (names.Count == 0)
        {
          aProperty.CurrentTracking.DataChanged -= this.HandlePropertyDataChanged;
          this.m_trackedProperties.Remove(aProperty.CurrentTracking);
        }
      }
      // store new tracked child for this property
      aProperty.CurrentTracking = newTracking;
      // add listener and property name
      if (newTracking == null)
      {
        return;
      }
      if (!this.m_trackedProperties!.ContainsKey(newTracking))
      {
        this.m_trackedProperties[newTracking] = new List<string>();
        newTracking.DataChanged += this.HandlePropertyDataChanged;
      }
      this.m_trackedProperties[newTracking].Add(aProperty.Meta.Name);
    }

    /// <summary>
    /// Clears all property values by either assigning the default value or by checking of the property value is an
    /// object and trying to locate and call a <code>Clear()</code> on it.
    /// </summary>
    /// <param name="aCallChanged">True to call <see cref="Changed(string,object,object)"/></param>
    /// <param name="aMeta">Cached meta data</param>
    private void ClearPropertyValues(bool aCallChanged, Dictionary<string, UFPropertyMetaData> aMeta)
    {
      foreach (string propertyName in aMeta.Keys)
      {
        UFPropertyMetaData propertyMeta = aMeta[propertyName];
        if (propertyMeta.DefaultValue != null)
        {
          this.SetPropertyValue(propertyName, aMeta[propertyName].DefaultValue!.Value, aCallChanged);
        }
        else
        {
          object currentValue = this.GetPropertyValue(propertyName);
          MethodInfo? clearMethod = propertyMeta.TypeInfo
            .GetDeclaredMethods("Clear")
            .FirstOrDefault(x => x.GetParameters().Length == 0);
          // call clear method if it does not expect any parameters
          if (clearMethod != null)
          {
            clearMethod.Invoke(currentValue, null);
          }
        }
      }
    }

    /// <summary>
    /// Clears all internal stored property values by setting <see cref="UFPropertyInstanceData.CurrentValueUsed"/> to
    /// false.
    /// </summary>
    /// <param name="aCallChanged">When true call changed for every property that had a value assigned to it</param>
    private void ClearInternalPropertyValues(bool aCallChanged)
    {
      foreach (UFPropertyInstanceData property in this.m_propertyInstances.Values)
      {
        bool callChanged = property.CurrentValueUsed && aCallChanged;
        property.CurrentValueUsed = false;
        if (callChanged)
        {
          this.Changed(property.Meta.Name, property.CurrentValue);
        }
      }
    }

    #endregion

    #region event handlers

    /// <summary>
    /// Handle changes to a property. Call Changed on every property name the value belongs to. Also trigger the
    /// OnChildChanged event.
    /// </summary>
    /// <param name='aSender'>
    /// The property value that changed.
    /// </param>
    /// <param name='anEvent'>
    /// The event.
    /// </param>
    private void HandlePropertyDataChanged(object aSender, UFDataChangedEventArgs anEvent)
    {
      if (this.m_trackedProperties!.ContainsKey(aSender))
      {
        this.Lock();
        foreach (string name in this.m_trackedProperties[aSender])
        {
          this.Changed(name, aSender, aSender);
        }
        this.Unlock();
        this.ChildHasChanged(aSender, this.m_trackedProperties[aSender].ToArray(), anEvent);
      }
      this.OnChildChanged(aSender, anEvent);
    }

    #endregion

    #region private classes

    /// <summary>
    /// Property meta data.
    /// </summary>
    private class UFPropertyMetaData
    {
      #region private vars

      /// <summary>
      /// Property information or null when the meta data represents a field.
      /// </summary>
      private readonly PropertyInfo? m_property;

      /// <summary>
      /// Field information or null when the meta data represents a property.
      /// </summary>
      private readonly FieldInfo? m_field;

      #endregion

      #region public properties

      /// <summary>
      /// Value validators for the property.
      /// </summary>
      public List<IUFValidateValue>? ValueValidators { get; private set; }

      /// <summary>
      /// Property validators for the property.
      /// </summary>
      public List<IUFValidateProperty>? PropertyValidators { get; private set; }

      /// <summary>
      /// Gets the name of the property.
      /// </summary>
      /// <value>
      /// The name of the property.
      /// </value>
      public string Name => this.m_property != null ? this.m_property.Name : this.m_field!.Name;

      /// <summary>
      /// Gets the type of the property or field.
      /// </summary>
      /// <value>
      /// The type.
      /// </value>
      public Type Type => this.m_property != null ? this.m_property.PropertyType : this.m_field!.FieldType;

      /// <summary>
      /// Gets the type information.
      /// </summary>
      public TypeInfo TypeInfo => this.Type.GetTypeInfo();

      /// <summary>
      /// Custom name for IO actions.
      /// </summary>
      public UFIONameAttribute? IOName { get; private set; }

      /// <summary>
      /// Default value or null if none is set
      /// </summary>
      public DefaultValueAttribute? DefaultValue { get; private set; }

      /// <summary>
      /// When <c>true</c> ignore the property with IO methods.
      /// </summary>
      public bool IOIgnore { get; private set; }

      /// <summary>
      /// Gets a value indicating if the property is a read only field.
      /// </summary>
      /// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
      public bool ReadOnly => (this.m_property != null) && !this.m_property.CanWrite;

#if UFDEBUG

      /// <summary>
      /// When true log value changes.
      /// </summary>
      public bool LogChanges { get; private set; }

#endif

      #endregion

      #region public methods

      /// <summary>
      /// Initializes a new instance of the <see cref="UFPropertyMetaData"/> class 
      /// using PropertyInfo.
      /// </summary>
      /// <param name='anInfo'>
      /// Information about property.
      /// </param>
      public UFPropertyMetaData(PropertyInfo anInfo)
      {
        this.m_property = anInfo;
        this.m_field = null;
        this.Init();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="UFPropertyMetaData"/> class
      /// using FieldInfo.
      /// </summary>
      /// <param name='anInfo'>
      /// Information about the field.
      /// </param>
      public UFPropertyMetaData(FieldInfo anInfo)
      {
        this.m_field = anInfo;
        this.m_property = null;
        this.Init();
      }

      /// <summary>
      /// Process all validators to validate the value.
      /// </summary>
      /// <returns>
      /// <c>true</c> if all validators returned true; otherwise, <c>false</c>.
      /// </returns>
      /// <param name='aValue'>
      /// Value to validate.
      /// </param>
      /// <param name="aData">
      /// Data to get property value from
      /// </param>
      public bool IsValid(object? aValue, IUFModel aData)
      {
        return this.ValueValidators!.All(validator => validator.IsValid(aValue))
          && this.PropertyValidators!.All(validator => validator.IsValid(this.Name, aData));
      }

      /// <summary>
      /// Gets the value of the property or field.
      /// </summary>
      /// <returns>
      /// The value.
      /// </returns>
      /// <param name='anInstance'>
      /// An instance to get the value from.
      /// </param>
      public object GetValue(object anInstance)
      {
        return this.m_property != null
          ? this.m_property!.GetValue(anInstance)
          : this.m_field!.GetValue(anInstance);
      }

      /// <summary>
      /// Sets the value of the property or field.
      /// </summary>
      /// <param name='anInstance'>
      /// An instance to set the value at.
      /// </param>
      /// <param name='aValue'>
      /// The value to set.
      /// </param>
      public void SetValue(object anInstance, object? aValue)
      {
        if (this.m_property != null)
        {
          this.m_property.SetValue(anInstance, aValue);
        }
        else
        {
          this.m_field!.SetValue(anInstance, aValue);
        }
      }

      #endregion

      #region private methods

      /// <summary>
      /// Initialize this instance.
      /// </summary>
      private void Init()
      {
        this.ValueValidators = new List<IUFValidateValue>();
        this.PropertyValidators = new List<IUFValidateProperty>();
        if (this.m_property != null)
        {
          this.IOName = UFAttributeTools.Find<UFIONameAttribute>(this.m_property) ?? new UFIONameAttribute(this.Name);
          this.IOIgnore = UFAttributeTools.Find<UFIOIgnoreAttribute>(this.m_property) != null;
          this.DefaultValue = UFAttributeTools.Find<DefaultValueAttribute>(this.m_property);
#if UFDEBUG
          this.LogChanges = UFAttributeTools.Find<UFLogAttribute>(this.m_property) != null;
#endif
        }
        else
        {
          this.IOName = UFAttributeTools.Find<UFIONameAttribute>(this.m_field!) ?? new UFIONameAttribute(this.Name);
          this.IOIgnore = UFAttributeTools.Find<UFIOIgnoreAttribute>(this.m_field!) != null;
          this.DefaultValue = UFAttributeTools.Find<DefaultValueAttribute>(this.m_field!);
#if UFDEBUG
          this.LogChanges = UFAttributeTools.Find<UFLogAttribute>(this.m_field!) != null;
#endif
        }
      }

      #endregion
    }

    /// <summary>
    /// Property instance data.
    /// </summary>
    private class UFPropertyInstanceData
    {
      #region public properties

      /// <summary>
      /// True if the property has changed; otherwise <c>false</c>.
      /// </summary>
      public bool Changed { get; set; }

      /// <summary>
      /// The old value of the property.
      /// </summary>
      public object? OldValue { get; set; }

      /// <summary>
      /// The last value used with IsValidPropertyValue, Changed or Assign.
      /// </summary>
      public object? RecentValue { get; set; }

      /// <summary>
      /// The current value (when value is stored via <see cref="UFModel.Set"/>)
      /// </summary>
      /// <remarks>
      /// This is a field and not a property, so it can be used as reference parameter.
      /// </remarks>
      public object? CurrentValue;

      /// <summary>
      /// When true the <see cref="CurrentValue"/> property is in use.
      /// </summary>
      public bool CurrentValueUsed { get; set; }

      /// <summary>
      /// Value used when tracking children.
      /// </summary>
      public IUFNotifyDataChanged? CurrentTracking { get; set; }

      /// <summary>
      /// The meta data for the property (shared between all instances).
      /// </summary>
      public UFPropertyMetaData Meta { get; }

      /// <summary>
      /// Value validators for the property.
      /// </summary>
      public List<IUFValidateValue> ValueValidators { get; }

      /// <summary>
      /// Property validators for the property.
      /// </summary>
      public List<IUFValidateProperty> PropertyValidators { get; }

      #endregion

      #region public methods

      /// <summary>
      /// Initializes a new instance of the <see cref="UFPropertyInstanceData"/> class using property meta data.
      /// </summary>
      /// <param name='aMeta'>
      /// Meta information for property.
      /// </param>
      public UFPropertyInstanceData(UFPropertyMetaData aMeta)
      {
        this.Meta = aMeta;
        this.Changed = false;
        this.CurrentValueUsed = false;
        this.ValueValidators = new List<IUFValidateValue>();
        this.PropertyValidators = new List<IUFValidateProperty>();
      }

      /// <summary>
      /// Process all validators to validate the value (including the ones stored at meta level).
      /// </summary>
      /// <returns>
      /// <c>true</c> if all validators returned true; otherwise, <c>false</c>.
      /// </returns>
      /// <param name='aValue'>
      /// Value to validate.
      /// </param>
      /// <param name="aData">
      /// Data to get property value from
      /// </param>
      public bool IsValid(object? aValue, IUFModel aData)
      {
        return
          this.Meta.IsValid(aValue, aData) &&
          this.ValueValidators.All(validator => validator.IsValid(aValue)) &&
          this.PropertyValidators.All(validator => validator.IsValid(this.Meta.Name, aData));
      }

      #endregion
    }

    #endregion
  }
}