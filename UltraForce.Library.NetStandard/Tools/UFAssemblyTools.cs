// <copyright file="UFAssemblyTools.cs" company="Ultra Force Development">
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
using System.IO;
using System.Reflection;
using System.Linq;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Support methods for the <see cref="Assembly"/> type.
  /// </summary>
  public static class UFAssemblyTools
  {
    /// <summary>
    /// Gets all types defined in a namespace.
    /// </summary>
    /// <param name="anAssembly"></param>
    /// <param name="aNameSpace"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetTypesInNamespace(
      Assembly anAssembly,
      string aNameSpace
    )
    {
      return anAssembly.ExportedTypes.Where(
        type => string.Equals(
          type.Namespace,
          aNameSpace,
          StringComparison.Ordinal
        )
      );
    }

    /// <summary>
    /// Gets a resource and return as text.
    /// </summary>
    /// <param name="anAssembly">Assembly to get resource from</param>
    /// <param name="aResourceId">ID of resource</param>
    /// <returns>text</returns>
    public static string GetResourceAsText(
      Assembly anAssembly,
      string aResourceId
    )
    {
      Stream? stream = anAssembly.GetManifestResourceStream(aResourceId);
      if (stream == null)
      {
        throw new Exception(
          $"Resource '{aResourceId}' not found in assembly '{anAssembly.FullName}'"
        );
      }
      using StreamReader reader = new StreamReader(stream);
      return reader.ReadToEnd();
    }
  }
}