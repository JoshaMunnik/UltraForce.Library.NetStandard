// <copyright file="UFStringTools.cs" company="Ultra Force Development">
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

// ReSharper disable AccessToStaticMemberViaDerivedType

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// An utility class that adds extra string related methods.
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static class UFStringTools
  {
    #region private constants

    /// <summary>
    /// Lower case hex nibble values
    /// </summary>
    private static readonly char[] s_lowerCaseHexNibbles =
    {
      '0', '1', '2', '3', '4', '5', '6', '7',
      '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'
    };

    /// <summary>
    /// Upper case hex nibble values
    /// </summary>
    private static readonly char[] s_upperCaseHexNibbles =
    {
      '0', '1', '2', '3', '4', '5', '6', '7',
      '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
    };

    #endregion

    #region public methods

    /// <summary>
    /// Generates a code existing of a random sequence of upper and lowercase letters and numbers.
    /// </summary>
    /// <remarks>
    /// The code makes sure every 3rd char is a numeric value (to prevent offensive words). Also the code will not
    /// contain any zero, uppercase o, one and lowercase l characters since these might look similar with certain fonts.
    /// </remarks>
    /// <param name="aLength">Size of the generated code</param>
    /// <returns>a random code of aLength characters</returns>
    public static string GenerateCode(int aLength)
    {
      StringBuilder result = new StringBuilder(aLength);
      int letterCount = 1;
      for (int index = 0; index < aLength; index++)
      {
        // 00..09: '0'..'9'
        // 10..35: 'A'..'Z'
        // 36..61: 'a'..'z' 
        int charCode = 0;
        // get character, repeat if char-code is zero or uppercase o or one or lowercase l 
        while ((charCode == 0) || (charCode == 24) || (charCode == 1) || (charCode == 47))
        {
          // make sure every 3rd char is a number 
          // (also to prevent offensive words)
          charCode = UFRandomTools.Next(letterCount > 2 ? 10 : 62);
        }
        // number, upper or lower case?
        if (charCode < 10)
        {
          // add number
          result.Append(charCode.ToString());
          // reset letter counter
          letterCount = 0;
        }
        else if (charCode < 36)
        {
          // add upper case
          result.Append((char)(charCode + 65 - 10));
        }
        else
        {
          // add lower case
          result.Append((char)(charCode + 97 - 10 - 26));
        }
        // increase letter counter
        letterCount++;
      }
      return result.ToString();
    }

    /// <summary>
    /// Generates a code existing of a random sequence of upper and lowercase letters.  
    /// </summary>
    /// <remarks>
    /// The code will not contain any uppercase o and lowercase l characters since these might look
    /// similar with certain fonts.
    /// </remarks>
    /// <param name="aLength">Size of the generated code</param>
    /// <returns>a random code of aLength characters</returns>
    public static string GenerateTextCode(int aLength)
    {
      StringBuilder result = new StringBuilder(aLength);
      for (int index = 0; index < aLength; index++)
      {
        int charCode = 14;
        // get char-code, repeat if char-code is an uppercase o or lowercase l 
        while ((charCode == 14) || (charCode == 37))
        {
          charCode = UFRandomTools.Next(52);
        }
        if (charCode < 26)
        {
          // add upper case
          result.Append((char)(charCode + 65));
        }
        else
        {
          // add lower case
          result.Append((char)(charCode + 97 - 26));
        }
      }
      return result.ToString();
    }

    /// <summary>
    /// Generates a numeric code. The method makes sure the first digit is not 0.
    /// </summary>
    /// <param name="aLength">Number of digits</param>
    /// <returns>A code of aLength characters existing of characters '0' - '9'</returns>
    public static string GenerateNumericCode(int aLength)
    {
      StringBuilder result = new StringBuilder(aLength);
      // make sure the first digit is not a 0
      result.Append((1 + UFRandomTools.Next(9)).ToString());
      for (int index = 1; index < aLength; index++)
      {
        result.Append(UFRandomTools.Next(10).ToString());
      }
      return result.ToString();
    }

    /// <summary>
    /// Generates a code using a custom alphabet.
    /// </summary>
    /// <param name="aLength">Number of digits</param>
    /// <param name="anAlphabet">Alphabet to get characters from</param>
    /// <returns>
    /// A code of aLength characters existing of random characters from anAlphabet
    /// </returns>
    public static string GenerateCode(int aLength, string anAlphabet)
    {
      StringBuilder result = new StringBuilder(aLength);
      for (int index = 0; index < aLength; index++)
      {
        result.Append(anAlphabet[UFRandomTools.Next(anAlphabet.Length)]);
      }
      return result.ToString();
    }

    /// <summary>
    /// Creates an unique code by converting a new guid to a hex string.
    /// </summary>
    /// <returns>Unique code based on a guid</returns>
    public static string CodeFromGuid()
    {
      return CodeFromGuid(Guid.NewGuid());
    }

    /// <summary>
    /// Converts a <see cref="Guid"/> to a hex string.
    /// </summary>
    /// <param name="aGuid">Guid to convert</param>
    /// <returns>Unique code based on a guid</returns>
    public static string CodeFromGuid(Guid aGuid)
    {
      return GetHexString(aGuid.ToByteArray());
    }
    
    /// <summary>
    /// Checks the string and returns the first one that is not null and not empty.
    /// If all texts fail, return <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="aTexts">Texts to check to check</param>
    /// <returns>
    /// One of the values from aTexts or <see cref="string.Empty"/>
    /// </returns>
    public static string SelectString(params string?[]? aTexts)
    {
      if (aTexts == null)
      {
        return string.Empty;
      }
      foreach(string? text in aTexts)
      {
        if (!string.IsNullOrEmpty(text))
        {
          return text!;
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// Adds a string to the end of a string. Use a separator if the string being added to is not empty.
    /// </summary>
    /// <param name="aSource">
    /// String to add to
    /// </param>
    /// <param name="aValue">
    /// String to add
    /// </param>
    /// <param name="aSeparator">
    /// Separator to use if aSource is not empty
    /// </param>
    /// <returns>[aSource + aSeparator] + aValue</returns>
    public static string Add(string? aSource, string? aValue, string aSeparator = ", ")
    {
      aValue ??= "";
      return string.IsNullOrEmpty(aSource) ? aValue : (aSource + aSeparator + aValue);
    }

    /// <summary>
    /// Count the number of occurrences of a character inside another string
    /// </summary>
    /// <param name='aValue'>
    /// A char value to count
    /// </param>
    /// <param name='aText'>
    /// A string to count in
    /// </param>
    /// <param name='aCaseInsensitive'>
    /// <c>true</c>, ignore casing; <c>false</c>, differentiate between lower- and uppercase.
    /// </param>
    /// <returns>Number of occurrences</returns>
    public static int Count(char aValue, string aText, bool aCaseInsensitive = false)
    {
      if (aCaseInsensitive)
      {
        aText = aText.ToLower();
        aValue = aValue.ToString().ToLower()[0];
      }
      return aText.Split(aValue).Length - 1;
    }

    /// <summary>
    /// Count the number of occurrences of a character inside another string
    /// </summary>
    /// <param name='aValue'>
    /// A char value to count
    /// </param>
    /// <param name='aText'>
    /// A string to count in
    /// </param>
    /// <param name='aCaseInsensitive'>
    /// <c>true</c>, ignore casing; <c>false</c>, differentiate between lower- and uppercase.
    /// </param>
    /// <returns>Number of occurrences or -1 if aValue is empty</returns>
    public static int Count(string aValue, string aText, bool aCaseInsensitive = false)
    {
      if (aValue == "")
      {
        return -1;
      }
      int position = 0;
      int count = 0;
      while (
        (position = aText.IndexOf(
          aValue,
          position,
          aCaseInsensitive
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.CurrentCulture)
        ) != -1
      )
      {
        position += aValue.Length;
        ++count;
      }
      return count;
    }

    /// <summary>
    /// Create an hexadecimal representation of a byte array.
    /// </summary>
    /// <remarks>
    /// The method will return uppercase A..F characters.
    /// </remarks>
    /// <param name="aBytes">An array of bytes to convert to string.</param>
    /// <returns>The bytes as hexadecimal string.</returns>
    public static string GetHexString(byte[] aBytes)
    {
      return GetHexString(aBytes, s_upperCaseHexNibbles);
    }

    /// <summary>
    /// Create an hexadecimal representation of a byte array.
    /// </summary>
    /// <remarks>
    /// The method will return lowercase a..f characters.
    /// </remarks>
    /// <param name="aBytes">An array of bytes to convert to string.</param>
    /// <returns>The bytes as hexadecimal string.</returns>
    public static string GetLowerCaseHexString(byte[] aBytes)
    {
      return GetHexString(aBytes, s_lowerCaseHexNibbles);
    }

    /// <summary>
    /// Converts a byte array to a hexadecimal presentation.
    /// </summary>
    /// <param name="aBytes">
    /// Bytes to convert
    /// </param>
    /// <param name="aNibbleMap">
    /// Maps nibbles (0..15) to character equivalent
    /// </param>
    /// <returns>
    /// A string containing a textual representation of the array
    /// </returns>
    public static string GetHexString(byte[] aBytes, char[] aNibbleMap)
    {
#if UFDEBUG
      if (aNibbleMap.Length != 16)
      {
        throw new Exception(
          "aNibbleMap is not 16 chars but " + aNibbleMap.Length + " chars"
        );
      }
#endif
      if (aBytes == null)
      {
        return "";
      }
      int bytesIndex = 0;
      int bytesLength = aBytes.Length;
      int resultIndex = 0;
      char[] result = new char[bytesLength * 2];
      while (bytesIndex < bytesLength)
      {
        byte value = aBytes[bytesIndex++];
        result[resultIndex++] = aNibbleMap[value >> 4];
        result[resultIndex++] = aNibbleMap[value & 0x0F];
      }
      return new string(result);
    }

    /// <summary>
    /// Replaces all keys with their value in a string.
    /// </summary>
    /// <param name="anOriginal">
    /// string to update, when it is <c>null</c> the method just returns <c>null</c>.
    /// </param>
    /// <param name="aMapping">
    /// keys and values, when it is <c>null</c> the method just returns the value of <c>anOriginal</c>.
    /// </param>
    /// <returns>updated string</returns>
    public static string? Replace(string? anOriginal, IDictionary<string, string>? aMapping)
    {
      if (anOriginal == null)
      {
        return null;
      }
      return aMapping == null
        ? anOriginal
        : aMapping.Keys.Aggregate(
          anOriginal,
          (current, key) => current.Replace(key, aMapping[key])
        );
    }

    /// <summary>
    /// Checks if a string is made up of unique characters.
    /// </summary>
    /// <param name="aText">Text to check</param>
    /// <returns>true if all characters only occur once</returns>
    public static bool HasUniqueCharacters(string aText)
    {
      // go from last to first character (no need to check the first character)
      for (int index = aText.Length - 1; index >= 1; index--)
      {
        // exit if the same character can be found before the current position
        if (aText.IndexOf(aText[index]) < index)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Joins the values of an enumerable type by using <see cref="Object.ToString"/> and joining
    /// them using <see cref="Add"/>.
    /// </summary>
    /// <param name="anEnumerable">Enumerable to combine into one string</param>
    /// <param name="aSeparator">Text to use separator between each value</param>
    /// <typeparam name="T">Enumerable type</typeparam>
    /// <returns>A string with all values joined together</returns>
    public static string Join<T>(IEnumerable<T> anEnumerable, string aSeparator = ", ") 
      where T : notnull
    {
      return anEnumerable.Aggregate("",
        (result, value) => Add(result, value.ToString(), aSeparator));
    }

    /// <summary>
    /// Creates a normalized version of a name, which can be used with case insensitive comparisons.
    /// </summary>
    /// <param name="aName"></param>
    /// <returns></returns>
    public static string NormalizeName(string aName)
    {
      return aName.ToLower();
    }

    /// <summary>
    /// Creates a normalized version of an email, which can be used with case insensitive
    /// comparisons.
    /// </summary>
    /// <param name="anEmail"></param>
    /// <returns></returns>
    public static string NormalizeEmail(string anEmail)
    {
      return anEmail.ToLower();
    }
    
    #endregion
  }
}