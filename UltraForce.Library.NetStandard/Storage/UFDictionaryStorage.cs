// <copyright file="UFDictionaryStorage.cs" company="Ultra Force Development">
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
using System.IO;
using System.Text;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Storage
{
  /// <summary>
  /// <see cref="UFDictionaryStorage" /> extends <see cref="UFKeyedStorage" />
  /// and stores the values into a <see cref="Dictionary{TKey,TValue}" />.
  /// <para>
  /// The class adds also various save and load methods.
  /// </para>
  /// </summary>
  public class UFDictionaryStorage : UFKeyedStorage, IUFJsonExport
  {
    #region private vars

    /// <summary>
    /// The dictionary used to store the values
    /// </summary>
    private readonly Dictionary<string, string> m_dictionary;

    #endregion

    #region public methods

    /// <summary>
    /// Constructs an instance of <see cref="UFDictionaryStorage" />.
    /// </summary>
    public UFDictionaryStorage() 
    {
      this.m_dictionary = new Dictionary<string, string>();
    }

    #endregion

    #region public io methods

    /// <summary>
    /// Saves the storage to a base64 encoded string.
    /// </summary>
    /// <returns>The property data stored as string.</returns>
    public string SaveToString()
    {
      return Convert.ToBase64String(this.SaveToBytes());
    }

    /// <summary>
    /// Loads the storage from a base64 encoded string.
    /// </summary>
    /// <param name="aData">
    /// The string data to get values from
    /// </param>
    public void LoadFromString(string aData)
    {
      this.LoadFromBytes(Convert.FromBase64String(aData));
    }

    /// <summary>
    /// Saves storage to a byte array.
    /// <para>
    /// This method creates a <see cref="MemoryStream"/> and calls
    /// <see cref="SaveToStream"/>.
    /// </para>
    /// </summary>
    /// <returns>A byte array</returns>
    public byte[] SaveToBytes()
    {
      MemoryStream stream = new MemoryStream();
      this.SaveToStream(stream);
      stream.Flush();
      return stream.ToArray();
    }

    /// <summary>
    /// Loads the storage from a byte array.
    /// <para>
    /// This method creates a <see cref="MemoryStream"/> from the byte array 
    /// and calls <see cref="LoadFromStream"/>.
    /// </para>
    /// </summary>
    /// <param name="aBytes">
    /// The data to get values from
    /// </param>
    public void LoadFromBytes(byte[] aBytes)
    {
      MemoryStream stream = new MemoryStream(aBytes) { Position = 0 };
      this.LoadFromStream(stream);
    }

    /// <summary>
    /// Saves the storage to a stream. 
    /// <para>
    /// This method creates a <see cref="BinaryWriter"/> and calls 
    /// <see cref="SaveToWriter"/>.
    /// </para>
    /// </summary>
    /// <param name="aStream">
    /// Stream to save values to
    /// </param>
    public void SaveToStream(Stream aStream)
    {
      this.SaveToWriter(new BinaryWriter(aStream));
    }

    /// <summary>
    /// Loads the storage from a stream. 
    /// <para>
    /// This method creates a <see cref="BinaryReader"/> and calls 
    /// <see cref="LoadFromReader"/>.
    /// </para>
    /// </summary>
    public void LoadFromStream(Stream aStream)
    {
      this.LoadFromReader(new BinaryReader(aStream));
    }

    /// <summary>
    /// Saves the storage to a BinaryWriter by writing both the key and data.
    /// <para>
    /// An unique marker is determined which is written first and after all
    /// data has been stored.
    /// </para>
    /// </summary>
    /// <param name="aWriter">
    /// The writer to save to.
    /// </param>
    public void SaveToWriter(BinaryWriter aWriter)
    {
      // generate marker that does not match any of the key names
      string marker;
      do
      {
        marker = UFStringTools.GenerateCode(16);
      } while (this.m_dictionary.ContainsKey(marker));
      // writer marker first
      aWriter.Write(marker);
      // process all keys
      foreach (string key in this.m_dictionary.Keys)
      {
        aWriter.Write(key);
        aWriter.Write(this.m_dictionary[key]);
      }
      // write marker again to indicate end of data
      aWriter.Write(marker);
    }

    /// <summary>
    /// Load property values from a BinaryReader.
    /// </summary>
    /// <param name="aReader">
    /// A reader to read values from.
    /// </param>
    public void LoadFromReader(BinaryReader aReader)
    {
      this.m_dictionary.Clear();
      // get marker
      string marker = aReader.ReadString();
      // get keys and values until marker is encountered again
      while (true)
      {
        string key = aReader.ReadString();
        if (!key.Equals(marker))
        {
          string value = aReader.ReadString();
          this.m_dictionary[key] = value;
        }
        else
        {
          break;
        }
      }
    }

    #endregion

    #region public json methods

    /// <summary>
    /// Create JSON formatted string from the data.
    /// <para>
    /// The method returns an object definition, using the keys for property
    /// names and the values as their value.
    /// </para>
    /// </summary>
    /// <returns>JSON formatted string</returns>
    public string SaveJson()
    {
      return UFJsonTools.SaveDictionary(this.m_dictionary);
    }

    #endregion

    #region IUFJsonExport

    /// <summary>
    /// Add data to <see cref="StringBuilder" /> using json formatting.
    /// <para>
    /// The method returns an object definition, using the keys for property
    /// names and the values as their value.
    /// </para>
    /// </summary>
    /// <param name="aBuilder">A builder to add data to.</param>
    public void SaveJson(StringBuilder aBuilder)
    {
      UFJsonTools.SaveDictionary(aBuilder, this.m_dictionary);
    }

    #endregion

    #region UFKeyedStorage

    /// <inheritdoc />
    public override void DeleteAll()
    {
      this.m_dictionary.Clear();
    }

    /// <inheritdoc />
    public override string GetString(string aKey, string aDefault)
    {
      return this.HasKey(aKey) ? this.m_dictionary[aKey] : aDefault;
    }

    /// <inheritdoc />
    public override void SetString(string aKey, string aValue)
    {
      this.m_dictionary[aKey] = aValue;
    }

    /// <inheritdoc />
    public override void DeleteKey(string aKey)
    {
      this.m_dictionary.Remove(aKey);
    }

    /// <inheritdoc />
    public override bool HasKey(string aKey)
    {
      return this.m_dictionary.ContainsKey(aKey);
    }

    #endregion
  }
}