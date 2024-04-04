// <copyright file="UFIOTools.cs" company="Ultra Force Development">
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

using System.IO;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// IO support methods.
  /// </summary>
  public static class UFIOTools
  {
    /// <summary>
    /// Gets a byte array from a reader, that was previously written to it
    /// with <see cref="WriteByteArray"/>.
    /// </summary>
    /// <param name="aReader">Reader to get data from.</param>
    /// <returns>Byte array or null</returns>
    public static byte[]? ReadByteArray(BinaryReader aReader)
    {
      int length = aReader.ReadInt32();
      return (length < 0) ? null : aReader.ReadBytes(length);
    }

    /// <summary>
    /// Writes a byte array to a writer. Supports also null values.
    /// <para>
    /// The method first writes the length and then the bytes. If aValue is
    /// <c>null</c> the method will write -1 as length.
    /// </para>
    /// <para>
    /// Use <see cref="ReadByteArray"/> to read the byte array back
    /// </para>
    /// </summary>
    /// <param name="aWriter">Writer to write data to</param>
    /// <param name="aValue">Byte array or null</param>
    public static void WriteByteArray(BinaryWriter aWriter, byte[]? aValue)
    {
      if (aValue == null)
      {
        aWriter.Write(-1);
      }
      else
      {
        aWriter.Write(aValue.Length);
        aWriter.Write(aValue);
      }
    }
  }
}