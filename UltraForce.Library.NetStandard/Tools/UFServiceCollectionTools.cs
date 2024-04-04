// <copyright file="UFServiceCollectionTools.cs" company="Ultra Force Development">
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
using System.Linq;
using System.Reflection;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Services;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for <see cref="IUFServiceCollection"/> and <see cref="IServiceProvider"/>
  /// providing generic versions of various methods.
  /// </summary>
  public static class UFServiceCollectionTools
  {
    #region public methods

    /// <summary>
    /// Returns a helper to register various bindings using generics.
    /// </summary>
    /// <param name="aCollection">Collection to register at</param>
    /// <returns>A helper to register various binding</returns>
    public static UFServiceCollectionHelper With(IUFServiceCollection aCollection)
    {
      return new UFServiceCollectionHelper(aCollection);
    }

    /// <summary>
    /// Gets a service instance for a specific type.
    /// </summary>
    /// <typeparam name="TService">Type to get service for</typeparam>
    /// <param name="aProvider">Provider of services</param>
    /// <returns>instance of <c>T</c> or null if the service is not known</returns>
    public static TService? GetService<TService>(IServiceProvider aProvider)
    {
      return (TService?) aProvider.GetService(typeof(TService));
    }

    /// <summary>
    /// Processes a list of parameters and tries to create argument values for them by resolving
    /// them as a service or else using their default value.
    /// </summary>
    /// <param name="aProvider">Service provider to use</param>
    /// <param name="aParameters">Parameters to resolve</param>
    /// <returns>argument values</returns>
    /// <exception cref="Exception">
    /// Thrown if a parameter type is not a known service and the parameter has no default value.
    /// </exception>
    public static object[] GetArguments(IServiceProvider aProvider, ParameterInfo[] aParameters)
    {
      // arguments will contain the arguments to pass on to the constructor
      object[] arguments = new object[aParameters.Length];
      // process each parameter, determine value and store it in arguments
      for (int index = 0; index < aParameters.Length; index++)
      {
        ParameterInfo parameter = aParameters[index];
        // try to get instance for service
        object? value = aProvider.GetService(parameter.ParameterType);
        // if null then parameter type is not known as service, try to use default value
        if (value == null)
        {
          value = parameter.DefaultValue;
          bool found = parameter.HasDefaultValue;
          // throw exception if there is no value for parameter
          if (!found)
          {
            throw new Exception($"Could not find a value for: {parameter.ParameterType.Name} {parameter.Name}");
          }
        }
        arguments[index] = value!;
      }
      return arguments;
    }

    /// <summary>
    /// Calls a method, assuming its parameters are either service types or have a default value.
    /// </summary>
    /// <param name="anInstance">Instance to call method with</param>
    /// <param name="aMethod">Method to call</param>
    /// <param name="aProvider">Service provider</param>
    /// <returns>result from call</returns>
    public static object CallMethod(object anInstance, MethodInfo aMethod, IServiceProvider aProvider)
    {
      ParameterInfo[] parameters = aMethod.GetParameters();
      return aMethod.Invoke(anInstance, parameters.Length == 0 ? null : GetArguments(aProvider, parameters));
    }

    /// <summary>
    /// Calls a method of a certain name, assuming its parameters are either
    /// service types or have a default value.
    /// </summary>
    /// <param name="anInstance">Instance to call method with</param>
    /// <param name="aMethodName">Name of method to call</param>
    /// <param name="aProvider">Service provider</param>
    /// <returns>result from call</returns>
    public static object CallMethod(object anInstance, string aMethodName, IServiceProvider aProvider)
    {
      TypeInfo typeInfo = anInstance.GetType().GetTypeInfo();
      return CallMethod(anInstance, typeInfo.GetDeclaredMethod(aMethodName), aProvider);
    }

    /// <summary>
    /// Calls all methods of a certain name, assuming their parameters are
    /// either service types or have a default value.
    /// <para>
    /// This method will call all methods from the instance and its parent
    /// classes starting at the base class and ending with <c>anInstance</c>.
    /// </para>
    /// </summary>
    /// <param name="anInstance">Instance to call method with</param>
    /// <param name="aMethodName">Name of method to call</param>
    /// <param name="aProvider">Service provider</param>
    public static void CallMethods(object anInstance, string aMethodName, IServiceProvider aProvider)
    {
      IEnumerable<MethodInfo> methods =
        UFReflectionTools.GetAllMethods(anInstance.GetType(), aMethodName);
      foreach (MethodInfo method in methods.Reverse())
      {
        CallMethod(anInstance, method, aProvider);
      }
    }

    /// <summary>
    /// Processes all methods, properties and fields in an instance. Methods annotated with the
    /// <see cref="UFInjectAttribute"/> will be invoked via
    /// <see cref="CallMethod(object,System.Reflection.MethodInfo,System.IServiceProvider)"/>.
    /// Properties and fields annotated with <see cref="UFInjectAttribute"/> will be assigned a value from
    /// <c>aProvider</c> using their type to get a service instance.
    /// </summary>
    /// <param name="anInstance">Instance to process</param>
    /// <param name="aProvider">Provider to get service instances from</param>
    /// <exception cref="Exception">
    /// Thrown if a readonly property has been annotated with <see cref="UFInjectAttribute"/>
    /// </exception>
    public static void ProcessInjects(object anInstance, IServiceProvider aProvider)
    {
      Type instanceType = anInstance.GetType();
      IEnumerable<MethodInfo> methods = instanceType
        .GetMethods()
        .Where(method => method.GetCustomAttributes(typeof(UFInjectAttribute), false).Length > 0);
      foreach (MethodInfo method in methods)
      {
        CallMethod(anInstance, method, aProvider);
      }
      IEnumerable<PropertyInfo> properties = instanceType
        .GetProperties()
        .Where(property => property.GetCustomAttributes(typeof(UFInjectAttribute), false).Length > 0);
      foreach (PropertyInfo property in properties)
      {
        if (property.CanWrite)
        {
          property.SetValue(anInstance, aProvider.GetService(property.PropertyType));
        }
        else
        {
          throw new Exception($"Trying to inject a value in a readonly property {instanceType.Name}.{property.Name}");
        }
      }
      IEnumerable<FieldInfo> fields = instanceType
        .GetFields()
        .Where(field => field.GetCustomAttributes(typeof(UFInjectAttribute), false).Length > 0);
      foreach (FieldInfo field in fields)
      {
        field.SetValue(anInstance, aProvider.GetService(field.FieldType));
      }
    }

    #endregion

    #region public types

    /// <summary>
    /// A helper class with various registration methods. Each registration method returns the
    /// instance, allowing for chaining of registration calls.
    /// </summary>
    public class UFServiceCollectionHelper
    {
      #region private variables

      /// <summary>
      /// Reference to the collection to register service bindings at
      /// </summary>
      private readonly IUFServiceCollection m_collection;

      #endregion

      #region constructors

      /// <summary>
      /// Creates an instance of <see cref="UFServiceCollectionHelper"/>
      /// </summary>
      /// <param name="aCollection">Collection to use</param>
      internal UFServiceCollectionHelper(IUFServiceCollection aCollection)
      {
        this.m_collection = aCollection;
      }

      #endregion

      #region public methods

      /// <summary>
      /// Registers a service that also implements itself. A new instance will be created every time
      /// a parameter uses that type.
      /// </summary>
      /// <typeparam name="TService">
      /// Service type register
      /// </typeparam>
      public UFServiceCollectionHelper Register<TService>() where TService : class
      {
        this.m_collection.Register(typeof(TService), typeof(TService));
        return this;
      }

      /// <summary>
      /// Registers a class provider type for a certain service type. A new instance will be created
      /// every time a parameter uses the service type.
      /// </summary>
      /// <remarks>
      /// <c>TService</c> may also refer to a subclass of <c>TProvider</c>.
      /// </remarks>
      /// <typeparam name="TService">
      /// Service type to register class type for
      /// </typeparam>
      /// <typeparam name="TProvider">
      /// Provider type to register
      /// </typeparam>
      public UFServiceCollectionHelper Register<TService, TProvider>() where TProvider : class, TService
      {
        this.m_collection.Register(typeof(TService), typeof(TProvider));
        return this;
      }

      /// <summary>
      /// Registers a singleton service type. The first time the service is requested by a parameter an instance is
      /// created. This instance is reused whenever a parameter requests the same service.
      /// </summary>
      /// <typeparam name="TService">
      /// Service type to register both as service and provider.
      /// </typeparam>
      public UFServiceCollectionHelper RegisterSingleton<TService>() where TService : class
      {
        this.m_collection.RegisterSingleton(typeof(TService), typeof(TService));
        return this;
      }

      /// <summary>
      /// Registers a singleton provider type for a certain service type. The first time the service is requested by a
      /// parameter an instance is created. This is instance is reused whenever a parameter requests
      /// the same service.
      /// </summary>
      /// <typeparam name="TService">
      /// service type to register provider type for
      /// </typeparam>
      /// <typeparam name="TProvider">
      /// provider type to register
      /// </typeparam>
      public UFServiceCollectionHelper RegisterSingleton<TService, TProvider>() where TProvider : class, TService
      {
        this.m_collection.RegisterSingleton(typeof(TService), typeof(TProvider));
        return this;
      }

      /// <summary>
      /// Registers a singleton instance for a service type. Whenever a parameter request the service, the instance will
      /// be used.
      /// </summary>
      /// <typeparam name="TService">
      /// Service type to register
      /// </typeparam>
      /// <param name="anInstance">
      /// Provider instance implementing the service
      /// </param>
      public UFServiceCollectionHelper RegisterSingleton<TService>(TService anInstance) 
        where TService : notnull
      {
        this.m_collection.RegisterSingleton(typeof(TService), anInstance);
        return this;
      }

      /// <summary>
      /// Registers multiple types (both as service and provider implementing the service). The method checks if type is
      /// not registered before registering it.
      /// </summary>
      /// <param name="aTypes">One or more types to register</param>
      public UFServiceCollectionHelper Register(params Type[] aTypes)
      {
        foreach (Type type in aTypes)
        {
          if (!this.m_collection.IsRegistered(type))
          {
            this.m_collection.Register(type, type);
          }
        }
        return this;
      }

      /// <summary>
      /// Registers multiple types (both as service and singleton provider implementing the service).
      /// </summary>
      /// <param name="aTypes">One or more types to register</param>
      public UFServiceCollectionHelper RegisterSingleton(params Type[] aTypes)
      {
        foreach (Type type in aTypes)
        {
          this.m_collection.RegisterSingleton(type, type);
        }
        return this;
      }

      /// <summary>
      /// Registers all types in a namespace in an assembly.
      /// </summary>
      /// <param name="anAssembly">Assembly to get types from</param>
      /// <param name="aNameSpace">Name space to get types from</param>
      public UFServiceCollectionHelper Register(Assembly anAssembly, string aNameSpace)
      {
        return this.Register(UFAssemblyTools.GetTypesInNamespace(anAssembly, aNameSpace).ToArray());
      }

      #endregion
    }

    #endregion
  }
}