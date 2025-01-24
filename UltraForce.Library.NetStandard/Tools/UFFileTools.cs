// <copyright file="UFFileTools.cs" company="Ultra Force Development">
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
using System.IO;
using System.Text.RegularExpressions;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Static support methods related to files and folders.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static class UFFileTools
  {
    /// <summary>
    /// Validates a file or folder name. 
    /// </summary>
    /// <param name="name">name to validate</param>
    /// <param name="fileOnly">When <c>true</c> aName should not contain path, 
    /// else aName also can contain path parts</param>
    /// <param name="relativeDir">When <c>true</c> the path part can contain '..'</param>
    /// <returns><c>True</c>=valid name, <c>false</c>=name contains invalid 
    /// chars or start with a path separator</returns>
    public static bool ValidateFilename(
      string name,
      bool fileOnly,
      bool relativeDir
    )
    {
      // get parts from name
      string? directoryName = Path.GetDirectoryName(name);
      string? baseName = Path.GetFileName(name);
      string? extension = Path.GetExtension(name);
      if ((extension == null) || (baseName == null) || (directoryName == null))
      {
        return false;
      }
      // remove . from extension
      if (extension.Length > 0)
      {
        extension = extension.Substring(1);
      }
      // directory name should be empty with file only
      if (fileOnly && (directoryName.Length > 0))
      {
        return false;
      }
      // directory name should not contain ..
      if (
        !relativeDir &&
        (directoryName.IndexOf("..", StringComparison.Ordinal) >= 0)
      )
      {
        return false;
      }
      // base name should not contain ..
      if (
        !relativeDir &&
        (baseName.IndexOf("..", StringComparison.Ordinal) >= 0)
      )
      {
        return false;
      }
      // base name should not start with . and contain certain characters
      if (Regex.IsMatch(
        baseName,
        "/^[^\\/?*:;{}\\\\\\.~\"'][^\\/?*:;{}\\\\~\"']*$/"
      ))
      {
        return false;
      }
      // extension should not contain a . and certain other characters
      if ((extension.Length > 0) &&
        Regex.IsMatch(extension, "/^[^\\/?*:;{}\\\\\\.~\"']*$/")
      )
      {
        return false;
      }
      // directory name should not contain certain characters
      if (Regex.IsMatch(directoryName, "/^[^?*:;{}~\"']*$/"))
      {
        return false;
      }
      // valid name
      return true;
    }

    /// <summary>
    /// Validates a file or folder name. 
    /// </summary>
    /// <param name="name">name to validate</param>
    /// <param name="fileOnly">When <c>true</c> aName should not contain path, 
    /// else aName also contains path part</param>
    /// <returns>
    /// <c>True</c>=valid name, <c>false</c>=name contains invalid chars or 
    /// starts with path separator or contains relative folder parts
    /// </returns>
    public static bool ValidateFilename(string name, bool fileOnly)
    {
      return ValidateFilename(name, fileOnly, false);
    }

    /// <summary>
    /// Validates a file name. 
    /// </summary>
    /// <param name="name">name to validate</param>
    /// <returns><c>True</c>=valid name, <c>false</c>=name contains invalid 
    /// chars or contains folder specifications</returns>
    public static bool ValidateFilename(string name)
    {
      return ValidateFilename(name, true, false);
    }

    /// <summary>
    /// Replace forward or backward slash with the opposite.
    /// </summary>
    /// <param name="path">Path to update</param>
    /// <param name="pathSeparator">Separator to use</param>
    /// <returns>updated path</returns>
    public static string UpdateSeparator(
      string path,
      char pathSeparator = '\\'
    )
    {
      return path.Replace(pathSeparator == '\\' ? '/' : '\\', pathSeparator);
    }

    /// <summary>
    /// This method first calls <see cref="UpdateSeparator"/> and then checks
    /// if the path specification starts with a root character and adds it
    /// if needed.
    /// </summary>
    /// <param name="path">Path to update</param>
    /// <param name="pathSeparator">Separator to use</param>
    /// <returns>Path starting with separator</returns>
    public static string AddRootPath(
      string path,
      char pathSeparator = '\\'
    )
    {
      if (string.IsNullOrEmpty(path))
      {
        return pathSeparator.ToString();
      }
      string result = UpdateSeparator(path, pathSeparator);
      if (result[0] != pathSeparator)
      {
        result = pathSeparator + result;
      }
      return result;
    }

    /// <summary>
    /// Removes forward or backward slash if it is the first character.
    /// </summary>
    /// <param name="path">Path to check</param>
    /// <returns>Updated path</returns>
    public static string RemoveRootPath(string path)
    {
      if (string.IsNullOrEmpty(path))
      {
        return path;
      }
      if ((path[0] == '\\') || (path[0] == '/'))
      {
        return path[1..];
      }
      return path;
    }
    
    /// <summary>
    /// Combines parts of a URL into a single URL. The method inserts a '/' between parts if needed.
    /// </summary>
    public static string CombineUrl(params string[] parts)
    {
      if (parts.Length == 0)
      {
        return string.Empty;
      }
      string result = parts[0];
      for (int index = 1; index < parts.Length; index++)
      {
        string part = parts[index];
        result = result.TrimEnd('/') + "/" + part.TrimStart('/');
      }
      return result;
    }

    /// <summary>
    /// Adds a timestamp ('yyyyMMdd_HHmmss') to a filename. 
    /// </summary>
    /// <param name="fileName">Name of file (can include an extension)</param>
    /// <param name="dateTime">Date/time to add or null to use current UTC date/time.</param>
    /// <returns>Returns fileName with timestamp added to it.</returns>
    /// <example>
    /// <code>
    /// string name = UFFileTools.AddTimestamp("example.txt");
    /// // name is "example_20210801_123456.txt"
    /// </code>
    /// </example>
    public static string AddTimestamp(
      string fileName,
      DateTime? dateTime = null
    )
    {
      dateTime ??= DateTime.UtcNow;
      string extension = Path.GetExtension(fileName);
      string name = Path.GetFileNameWithoutExtension(fileName);
      return $"{name}_{dateTime:yyyyMMdd_HHmmss}{extension}";
    }
  }
}