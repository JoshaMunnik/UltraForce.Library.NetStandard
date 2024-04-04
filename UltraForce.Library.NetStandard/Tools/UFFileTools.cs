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
using System.IO;
using System.Text.RegularExpressions;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Static support methods related to files and folders.
  /// </summary>
  public static class UFFileTools
  {
    /// <summary>
    /// Validates a file or folder name. 
    /// </summary>
    /// <param name="aName">name to validate</param>
    /// <param name="aFileOnly">When <c>true</c> aName should not contain path, 
    /// else aName also can contain path parts</param>
    /// <param name="aRelativeDir">When <c>true</c> the path part can
    /// contain '..'</param>
    /// <returns><c>True</c>=valid name, <c>false</c>=name contains invalid 
    /// chars or start with a path separator</returns>
    public static bool ValidateFilename(
      string aName,
      bool aFileOnly,
      bool aRelativeDir
    )
    {
      // get parts from name
      string? directoryName = Path.GetDirectoryName(aName);
      string? baseName = Path.GetFileName(aName);
      string? extension = Path.GetExtension(aName);
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
      if (aFileOnly && (directoryName.Length > 0))
      {
        return false;
      }
      // directory name should not contain ..
      if (
        !aRelativeDir &&
        (directoryName.IndexOf("..", StringComparison.Ordinal) >= 0)
      )
      {
        return false;
      }
      // base name should not contain ..
      if (
        !aRelativeDir &&
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
    /// <param name="aName">name to validate</param>
    /// <param name="aFileOnly">When <c>true</c> aName should not contain path, 
    /// else aName also contains path part</param>
    /// <returns>
    /// <c>True</c>=valid name, <c>false</c>=name contains invalid chars or 
    /// starts with path separator or contains relative folder parts
    /// </returns>
    public static bool ValidateFilename(string aName, bool aFileOnly)
    {
      return ValidateFilename(aName, aFileOnly, false);
    }

    /// <summary>
    /// Validates a file name. 
    /// </summary>
    /// <param name="aName">name to validate</param>
    /// <returns><c>True</c>=valid name, <c>false</c>=name contains invalid 
    /// chars or contains folder specifications</returns>
    public static bool ValidateFilename(string aName)
    {
      return ValidateFilename(aName, true, false);
    }

    /// <summary>
    /// Replace forward or backward slash with the opposite.
    /// </summary>
    /// <param name="aPath">Path to update</param>
    /// <param name="aPathSeparator">Separator to use</param>
    /// <returns>updated path</returns>
    public static string UpdateSeparator(
      string aPath,
      char aPathSeparator = '\\'
    )
    {
      return aPath.Replace(aPathSeparator == '\\' ? '/' : '\\', aPathSeparator);
    }

    /// <summary>
    /// This method first calls <see cref="UpdateSeparator"/> and then checks
    /// if the path specification starts with a root character and adds it
    /// if needed.
    /// </summary>
    /// <param name="aPath">Path to update</param>
    /// <param name="aPathSeparator">Separator to use</param>
    /// <returns>Path starting with separator</returns>
    public static string AddRootPath(
      string aPath,
      char aPathSeparator = '\\'
    )
    {
      if (string.IsNullOrEmpty(aPath))
      {
        return aPathSeparator.ToString();
      }
      string result = UpdateSeparator(aPath, aPathSeparator);
      if (result[0] != aPathSeparator)
      {
        result = aPathSeparator + result;
      }
      return result;
    }

    /// <summary>
    /// Removes forward or backward slash if it is the first character.
    /// </summary>
    /// <param name="aPath">Path to check</param>
    /// <returns>Updated path</returns>
    public static string RemoveRootPath(string aPath)
    {
      if (string.IsNullOrEmpty(aPath))
      {
        return aPath;
      }
      if ((aPath[0] == '\\') || (aPath[0] == '/'))
      {
        return aPath.Substring(1);
      }
      return aPath;
    }
  }
}