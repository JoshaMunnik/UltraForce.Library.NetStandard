// <copyright file="UFSimpleServiceCollection.cs" company="Ultra Force Development">
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
using System.Linq;
using System.Reflection;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Services
{
  /// <summary>
  /// <see cref="UFSimpleServiceCollection"/> is a basic implementation
  /// of <see cref="IUFServiceCollection"/>.
  /// <para>
  /// The class does not implement any constructor matching code. If there are multiple
  /// constructors, the class will skip constructors without parameters and use the first
  /// constructor with parameters to create an instance with.
  /// </para>
  /// <para>
  /// It is possible to annotate a constructor with <see cref="UFInjectAttribute"/> to force the
  /// use of that constructor. If multiple constructors are annotated, the class will
  /// use the first annotated constructor.
  /// </para>
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class UFSimpleServiceCollection : IUFServiceCollection
  {
    #region private variables

    /// <summary>
    /// Registration of types
    /// </summary>
    private readonly Dictionary<Type, UFTypeEntry> m_registrations;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFSimpleServiceCollection"/>
    /// </summary>
    public UFSimpleServiceCollection()
    {
      this.m_registrations = new Dictionary<Type, UFTypeEntry>();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Creates an instance of certain class type. The type does not have to be a registered
    /// service. The method will still use dependency injection to resolve constructor parameters.
    /// </summary>
    /// <param name="aType">Type to create instance of</param>
    /// <returns>Instance of the requested type</returns>
    public object CreateInstance(Type aType)
    {
      return this.CreateInstance(aType, true);
    }

    /// <summary>
    /// Generic version of <see cref="CreateInstance(Type)"/>
    /// </summary>
    /// <typeparam name="T">Type of class to create instance of</typeparam>
    /// <returns>Instance of the specified type</returns>
    public T CreateInstance<T>()
    {
      return (T)this.CreateInstance(typeof(T));
    }

    #endregion

    #region IUFServiceCollection

    /// <inheritdoc />
    public void Register(Type aServiceType, Type aProvider)
    {
      this.Register(aServiceType, aProvider, false);
    }

    /// <inheritdoc />
    public void RegisterSingleton(Type aServiceType, Type aProvider)
    {
      this.Register(aServiceType, aProvider, true);
    }

    /// <inheritdoc />
    public void RegisterSingleton(Type aServiceType, object aProviderInstance)
    {
      this.Register(aServiceType, aServiceType, true, aProviderInstance);
    }

    /// <inheritdoc />
    public bool IsRegistered(Type aServiceType)
    {
      return this.m_registrations.ContainsKey(aServiceType);
    }

    #endregion

    #region IServiceProvider

    /// <inheritdoc />
    public object? GetService(Type aServiceType)
    {
      return this.CreateInstance(aServiceType, true);
    }

    #endregion

    #region private methods

    /// <summary>
    /// Registers a type with parameters.
    /// </summary>
    /// <param name="aServiceType">
    /// Service to register
    /// </param>
    /// <param name="aProvider">
    /// Class implementing the service
    /// </param>
    /// <param name="anIsSingleton">
    /// When <c>true</c> reuse instance
    /// </param>
    /// <param name="aProviderInstance">
    /// Singleton instance to use or <c>null</c> to create instance first time the type is requested.
    /// </param>
    /// <exception cref="Exception">
    /// Thrown when trying to register a service for a service type that already has a service
    /// registered for it.
    /// </exception>
    private void Register(
      Type aServiceType, Type aProvider, bool anIsSingleton, object? aProviderInstance = null
      )
    {
      if (this.m_registrations.ContainsKey(aServiceType))
      {
        throw new Exception("There is already a registration for type " + aServiceType.Name);
      }
      this.m_registrations.Add(aServiceType, new UFTypeEntry(anIsSingleton, aProviderInstance, aProvider));
    }

    /// <summary>
    /// Creates an instance of a type, resolving constructor parameters with registered service types.
    /// <para>
    /// The method will use the first constructor that has been annotated with
    /// <see cref="UFInjectAttribute"/>. If no constructors are annotated the method will use the
    /// first constructor that uses any parameters.
    /// </para>
    /// </summary>
    /// <param name="aType">
    /// Type to create instance of
    /// </param>
    /// <param name="aCheckSingleton">
    /// When <c>true</c> first check if type matches a registered singleton type. If it does, create the singleton if
    /// it does not exists and then return it. When <c>false</c> do not check and just create an instance.
    /// </param>
    /// <returns>
    /// Instance of <c>aType</c>.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown if no public constructors could be found for <c>aType</c>.
    /// </exception>
    private object CreateInstance(Type aType, bool aCheckSingleton)
    {
      // type to use for result
      Type resultType = aType;
      // type is a known service
      if (this.m_registrations.TryGetValue(aType, out UFTypeEntry entry))
      {
        // shortcut
        // first check if the registration is singleton, if so return the
        // instance if an instance was already created
        if (aCheckSingleton)
        {
          if (entry.IsSingleton)
          {
            return entry.Singleton ?? (entry.Singleton = this.CreateInstance(aType, false));
          }
        }
        resultType = entry.ValueType;
      }
      TypeInfo typeInfo = resultType.GetTypeInfo();
      // parameters for constructor
      ParameterInfo[]? parameters = null;
      // get all public constructors 
      ConstructorInfo[] constructors = typeInfo.DeclaredConstructors.Where(c => c.IsPublic).ToArray();
      // make sure there are is at least 1 public constructors
      if (constructors.Length == 0)
      {
        throw new Exception($"Type {aType.Name} has no public constructor.");
      }
      // get first constructor that has been annotated
      ConstructorInfo? annotatedConstructor = constructors.FirstOrDefault(
        c => c.GetCustomAttributes(typeof(UFInjectAttribute), false).Length > 0
      );
      // use first annotated constructor if any
      if (annotatedConstructor != null)
      {
        parameters = annotatedConstructor.GetParameters();
      }
      else
      {
        // process constructors until one is found that uses one or more parameters, else the first constructor is used
        foreach (ConstructorInfo constructor in constructors)
        {
          if ((parameters == null) || (parameters.Length == 0))
          {
            parameters = constructor.GetParameters();
          }
          if (parameters.Length > 0)
          {
            break;
          }
        }
      }
      // create instance, resolving parameters if any
      // ReSharper disable once PossibleNullReferenceException
      return (parameters == null) || (parameters.Length == 0)
        ? Activator.CreateInstance(resultType)
        : Activator.CreateInstance(resultType, UFServiceCollectionTools.GetArguments(this, parameters));
    }

    /// <summary>
    /// Processes methods, properties and fields annotated with <see cref="UFInjectAttribute"/> by calling
    /// <see cref="UFServiceCollectionTools.ProcessInjects"/>
    /// </summary>
    /// <param name="anInstance">Object instance to check methods, properties and fields for</param>
    public void ProcessInjects(object anInstance)
    {
      UFServiceCollectionTools.ProcessInjects(anInstance, this);
    }

    #endregion

    #region private classes

    /// <summary>
    /// Stores a registration entry for a certain type.
    /// </summary>
    private class UFTypeEntry
    {
      /// <summary>
      /// When true use a singleton instance
      /// </summary>
      public readonly bool IsSingleton;

      /// <summary>
      /// Singleton instance to use, when null create it the first time the type is requested.
      /// </summary>
      public object? Singleton;

      /// <summary>
      /// Type to create instance of
      /// </summary>
      public readonly Type ValueType;

      /// <summary>
      /// Constructs an instance of <see cref="UFTypeEntry"/>
      /// </summary>
      /// <param name="anIsSingleton"></param>
      /// <param name="aSingleton"></param>
      /// <param name="aValueType"></param>
      public UFTypeEntry(bool anIsSingleton, object? aSingleton, Type aValueType)
      {
        this.IsSingleton = anIsSingleton;
        this.Singleton = aSingleton;
        this.ValueType = aValueType;
      }
    }

    #endregion
  }
}