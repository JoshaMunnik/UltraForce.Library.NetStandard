// <copyright file="IUFServiceCollection.cs" company="Ultra Force Development">
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
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Services
{
  /// <summary>
  /// <see cref="IUFServiceCollection"/> extends <see cref="IServiceProvider"/> and adds minimal
  /// number of methods to register services and providers.
  /// </summary>
  /// <remarks>
  /// Use <see cref="UFServiceCollectionTools.With"/> for registration types using generic methods.
  /// </remarks>
  public interface IUFServiceCollection : IServiceProvider
  {
    /// <summary>
    /// Registers a provider type for a certain service type. A new instance will be created every
    /// time the service type is requested.
    /// </summary>
    /// <param name="aServiceType">
    /// Service type to register provider type for.
    /// </param>
    /// <param name="aProvider">
    /// Type to create instance of when then service is requested.
    /// </param>
    void Register(Type aServiceType, Type aProvider);

    /// <summary>
    /// Registers a provider type for a certain service type. The first time the service is
    /// requested an instance is created. Subsequent requests will use the same instance.
    /// <para>
    /// Use <see cref="RegisterSingleton(Type,object)"/> to register an existing instance.
    /// </para>
    /// </summary>
    /// <param name="aServiceType">
    /// Service type to register provider type for.
    /// </param>
    /// <param name="aProvider">
    /// Type to create singleton instance of when the service is requested.
    /// </param>
    void RegisterSingleton(Type aServiceType, Type aProvider);

    /// <summary>
    /// Registers a singleton provider instance for a certain service type. 
    /// </summary>
    /// <param name="aServiceType">
    /// Service type to register provider type for.
    /// </param>
    /// <param name="aProviderInstance">
    /// Instance to return when the service is requested.
    /// </param>
    void RegisterSingleton(Type aServiceType, object aProviderInstance);

    /// <summary>
    /// Checks if there is a registration for a service type.
    /// </summary>
    /// <param name="aServiceType">
    /// Service type to check registration for
    /// </param>
    /// <returns>
    /// <c>True</c> if there is a registration; otherwise <c>false</c>.
    /// </returns>
    bool IsRegistered(Type aServiceType);
  }
}