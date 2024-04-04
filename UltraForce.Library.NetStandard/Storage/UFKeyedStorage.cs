// <copyright file="UFKeyedStorage.cs" company="Ultra Force Development">
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
using System.Globalization;
using System.Reflection;

namespace UltraForce.Library.NetStandard.Storage
{
  /// <summary>
  /// <see cref="UFKeyedStorage" /> is a base class for storage classes that
  /// store different type of values using unique keys.
  /// </summary>
  /// <remarks>
  /// The default implementations of the storage methods for the various types
  /// use the <see cref="GetString(string)" /> and
  /// <see cref="SetString" /> method to store textual representations of 
  /// the data.
  /// <para>
  /// The class itself is an abstract class, the following methods need to be 
  /// implemented in a subclass: <see cref="GetString(string,string)" />,
  /// <see cref="SetString" />, <see cref="HasKey" /> and
  /// <see cref="DeleteKey" />.
  /// </para>
  /// <para>
  /// If possible a subclass should also implement <see cref="DeleteAll" />.
  /// </para>
  /// </remarks>
  public abstract class UFKeyedStorage
  {
    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFKeyedStorage"/>
    /// </summary>
    protected UFKeyedStorage()
    {
    }

    #endregion

    #region public methods

    /// <summary>
    /// Gets a string.
    /// </summary>
    /// <param name="aKey">A key to get the string for.</param>
    /// <param name="aDefault">
    /// A default value to use when there is no string stored for the key.
    /// </param>
    /// <returns>The stored string or aDefault.</returns>
    public abstract string GetString(string aKey, string aDefault);

    /// <summary>
    /// Gets a string.
    /// </summary>
    /// <param name="aKey">A key to get the string for.</param>
    /// <returns>The stored string or "" if none could be found.</returns>
    public string GetString(string aKey)
    {
      return this.GetString(aKey, "");
    }

    /// <summary>
    /// Stores a string in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    public abstract void SetString(string aKey, string aValue);

    /// <summary>
    /// Gets a byte.
    /// <para>
    /// The default implementation calls <see cref="GetInt(string)" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">A key to get the byte for.</param>
    /// <param name="aDefault">
    /// A default value to use when there is no byte stored for the key.
    /// </param>
    /// <returns>The stored byte or aDefault.</returns>
    public virtual byte GetByte(string aKey, byte aDefault)
    {
      return this.HasKey(aKey) ? (byte)this.GetInt(aKey) : aDefault;
    }

    /// <summary>
    /// Gets a byte.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the byte for.
    /// </param>
    /// <returns>
    /// The stored byte or 0 if none could be found.
    /// </returns>
    public byte GetByte(string aKey)
    {
      return this.GetByte(aKey, 0);
    }

    /// <summary>
    /// Stores a byte in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetInt" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetByte(string aKey, byte aValue)
    {
      this.SetInt(aKey, aValue);
    }

    /// <summary>
    /// Gets a sbyte.
    /// <para>
    /// The default implementation calls <see cref="GetInt(string)" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the sbyte for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no sbyte stored for the key.
    /// </param>
    /// <returns>
    /// The stored sbyte or aDefault.
    /// </returns>
    public virtual sbyte GetSByte(string aKey, sbyte aDefault)
    {
      return this.HasKey(aKey) ? (sbyte)this.GetInt(aKey) : aDefault;
    }

    /// <summary>
    /// Gets a sbyte.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the sbyte for.
    /// </param>
    /// <returns>
    /// The stored sbyte or 0 if none could be found.
    /// </returns>
    public sbyte GetSByte(string aKey)
    {
      return this.GetSByte(aKey, 0);
    }

    /// <summary>
    /// Stores a sbyte in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetInt" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetSByte(string aKey, sbyte aValue)
    {
      this.SetInt(aKey, aValue);
    }

    /// <summary>
    /// Gets a short.
    /// <para>
    /// The default implementation calls <see cref="GetInt(string)" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the short for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no short stored for the key.
    /// </param>
    /// <returns>
    /// The stored short or aDefault.
    /// </returns>
    public virtual short GetShort(string aKey, short aDefault)
    {
      return this.HasKey(aKey) ? (short)this.GetInt(aKey) : aDefault;
    }

    /// <summary>
    /// Gets a short.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the short for.
    /// </param>
    /// <returns>
    /// The stored short or 0 if none could be found.
    /// </returns>
    public short GetShort(string aKey)
    {
      return this.GetShort(aKey, 0);
    }

    /// <summary>
    /// Stores a short in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetInt" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetShort(string aKey, short aValue)
    {
      this.SetInt(aKey, aValue);
    }

    /// <summary>
    /// Gets a ushort.
    /// <para>
    /// The default implementation calls <see cref="GetInt(string)" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the ushort for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no ushort stored for the key.
    /// </param>
    /// <returns>
    /// The stored ushort or aDefault.
    /// </returns>
    public virtual ushort GetUShort(string aKey, ushort aDefault)
    {
      return this.HasKey(aKey) ? (ushort)this.GetInt(aKey) : aDefault;
    }

    /// <summary>
    /// Gets a ushort.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the ushort for.
    /// </param>
    /// <returns>
    /// The stored ushort or 0 if none could be found.
    /// </returns>
    public ushort GetUShort(string aKey)
    {
      return this.GetUShort(aKey, 0);
    }

    /// <summary>
    /// Stores a ushort in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetInt" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetUShort(string aKey, ushort aValue)
    {
      this.SetInt(aKey, aValue);
    }

    /// <summary>
    /// Gets an integer.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> and
    /// uses <see cref="int.Parse(string)"/> to convert it back to a integer.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the integer for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no integer stored for the key.
    /// </param>
    /// <returns>
    /// The stored integer or aDefault.
    /// </returns>
    public virtual int GetInt(string aKey, int aDefault)
    {
      return this.HasKey(aKey) ? int.Parse(this.GetString(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets an integer.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the integer for.
    /// </param>
    /// <returns>
    /// The stored integer or 0 if none could be found.
    /// </returns>
    public int GetInt(string aKey)
    {
      return this.GetInt(aKey, 0);
    }

    /// <summary>
    /// Stores an integer in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetInt(string aKey, int aValue)
    {
      this.SetString(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets an unsigned integer.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> and
    /// uses <see cref="uint.Parse(string)"/> to convert it back to an
    /// unsigned integer.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the unsigned integer for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no unsigned integer stored for 
    /// the key.
    /// </param>
    /// <returns>
    /// The stored unsigned integer or aDefault.
    /// </returns>
    public virtual uint GetUInt(string aKey, uint aDefault)
    {
      return this.HasKey(aKey) ? uint.Parse(this.GetString(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets an unsigned integer.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the unsigned integer for.
    /// </param>
    /// <returns>
    /// The stored unsigned integer or 0 if none could be found.
    /// </returns>
    public uint GetUInt(string aKey)
    {
      return this.GetUInt(aKey, 0);
    }

    /// <summary>
    /// Stores an unsigned integer in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetUInt(string aKey, uint aValue)
    {
      this.SetString(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets a long.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> and
    /// uses <see cref="long.Parse(string)"/> to convert it back to a long.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the long for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no long stored for the key.
    /// </param>
    /// <returns>
    /// The stored long or aDefault.
    /// </returns>
    public virtual long GetLong(string aKey, long aDefault)
    {
      return this.HasKey(aKey) ? long.Parse(this.GetString(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets a long.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the long for.
    /// </param>
    /// <returns>
    /// The stored long or 0 if none could be found.
    /// </returns>
    public long GetLong(string aKey)
    {
      return this.GetLong(aKey, 0);
    }

    /// <summary>
    /// Stores a long in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetLong(string aKey, long aValue)
    {
      this.SetString(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets a ulong.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> and
    /// uses <see cref="ulong.Parse(string)"/> to convert it back to a ulong.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the ulong for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no ulong stored for the key.
    /// </param>
    /// <returns>
    /// The stored ulong or aDefault.
    /// </returns>
    public virtual ulong GetULong(string aKey, ulong aDefault)
    {
      return this.HasKey(aKey) ? ulong.Parse(this.GetString(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets a ulong.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the ulong for.
    /// </param>
    /// <returns>
    /// The stored ulong or 0 if none could be found.
    /// </returns>
    public ulong GetULong(string aKey)
    {
      return this.GetULong(aKey, 0);
    }

    /// <summary>
    /// Stores a ulong in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetULong(string aKey, ulong aValue)
    {
      this.SetString(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets a floating number.
    /// <para>
    /// The default implementation just calls <see cref="GetDouble(string)"/>
    /// and typecast the value back to float.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the floating number for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no floating number stored for
    /// the key.
    /// </param>
    /// <returns>
    /// The stored floating number or aDefault.
    /// </returns>
    public virtual float GetFloat(string aKey, float aDefault)
    {
      return this.HasKey(aKey) ? (float)this.GetDouble(aKey) : aDefault;
    }

    /// <summary>
    /// Gets a floating number.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the floating number for.
    /// </param>
    /// <returns> 
    /// The stored floating number or 0.0 when missing.
    /// </returns>
    public float GetFloat(string aKey)
    {
      return this.GetFloat(aKey, 0.0f);
    }

    /// <summary>
    /// Stores a floating number in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetDouble"/>.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param> 
    public virtual void SetFloat(string aKey, float aValue)
    {
      this.SetDouble(aKey, aValue);
    }

    /// <summary>
    /// Gets a double number.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> and
    /// uses <see cref="double.Parse(string)"/> to convert it back to a double.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the double number for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no double number stored for
    /// the key.
    /// </param>
    /// <returns>
    /// The stored floating number or aDefault.
    /// </returns>
    public virtual double GetDouble(string aKey, double aDefault)
    {
      return this.HasKey(aKey) ? double.Parse(this.GetString(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets a double number.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the double number for.
    /// </param>
    /// <returns>
    /// The stored double number or 0.0 when missing.
    /// </returns>
    public double GetDouble(string aKey)
    {
      return this.GetDouble(aKey, 0.0);
    }

    /// <summary>
    /// Stores a double number in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetDouble(string aKey, double aValue)
    {
      this.SetString(aKey, aValue.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Gets a bool.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> and
    /// converts its value back to a bool.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the bool for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no bool stored for the key.
    /// </param>
    /// <returns>
    /// The stored bool or aDefault.
    /// </returns>
    public virtual bool GetBool(string aKey, bool aDefault)
    {
      return this.HasKey(aKey) ? !this.GetString(aKey).Equals("0") : aDefault;
    }

    /// <summary>
    /// Gets a bool.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the bool for.
    /// </param>
    /// <returns>
    /// The stored bool or true when missing. 
    /// </returns>
    public bool GetBool(string aKey)
    {
      return this.GetBool(aKey, true);
    }

    /// <summary>
    /// Stores a bool in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetBool(string aKey, bool aValue)
    {
      this.SetString(aKey, aValue ? "1" : "0");
    }

    /// <summary>
    /// Gets a char.
    /// <para>
    /// The default implementation uses <see cref="GetString(string)"/> to get
    /// the stored character.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// A key to get the char for.
    /// </param>
    /// <param name="aDefault">
    /// A default value to use when there is no char stored for the key.
    /// </param>
    /// <returns>
    /// The stored char or aDefault.
    /// </returns>
    public virtual char GetChar(string aKey, char aDefault)
    {
      return this.HasKey(aKey) ? this.GetString(aKey)[0] : aDefault;
    }

    /// <summary>
    /// Gets a char.
    /// </summary>
    /// <param name="aKey">
    /// A key to get the char for.
    /// </param>
    /// <returns>
    /// The stored char or '\0' when missing. 
    /// </returns>
    public char GetChar(string aKey)
    {
      return this.GetChar(aKey, '\0');
    }

    /// <summary>
    /// Stores a char in the storage.
    /// <para>
    /// The default implementation just uses <see cref="SetString"/> to store
    /// the value.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual void SetChar(string aKey, char aValue)
    {
      this.SetString(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets an array of bytes.
    /// <para> 
    /// The default implementation uses <see cref="GetString(string)"/> and 
    /// assumes the data is is a base64 encoded string.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to get array for
    /// </param>
    /// <param name="aDefault">
    /// Default value to use when no data exists for aKey
    /// </param>
    /// <returns> 
    /// Array of byte or aDefault
    /// </returns>
    public virtual byte[]? GetBytes(string aKey, byte[]? aDefault)
    {
      return this.HasKey(aKey)
        ? Convert.FromBase64String(this.GetString(aKey))
        : aDefault;
    }

    /// <summary>
    /// Gets an array of bytes.
    /// </summary>
    /// <param name="aKey">
    /// Key to get array for
    /// </param>
    /// <returns>
    /// Array of byte or null when missing
    /// </returns>
    public byte[]? GetBytes(string aKey)
    {
      return this.GetBytes(aKey, null);
    }

    /// <summary>
    /// Stores an array of bytes to the storage. 
    /// <para>
    /// The default implementation converts the array to a base64 encoded
    /// string and uses <see cref="SetString"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for
    /// </param>
    /// <param name="aValue">
    /// Array of byte to store
    /// </param>
    public virtual void SetBytes(string aKey, byte[] aValue)
    {
      this.SetString(aKey, Convert.ToBase64String(aValue));
    }

    /// <summary>
    /// Gets a <see cref="DateTime"/> value using <see cref="GetLong(string)"/> 
    /// and <see cref="DateTime.FromBinary"/>.
    /// </summary>
    /// <param name="aKey">Key to get date and time for</param>
    /// <param name="aDefault">Default value to use</param>
    /// <returns>Store value or aDefault</returns>
    public virtual DateTime GetDateTime(string aKey, DateTime aDefault)
    {
      return this.HasKey(aKey) ? DateTime.FromBinary(this.GetLong(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets a date and time.
    /// </summary>
    /// <param name="aKey">Key to get date and time for</param>
    /// <returns>
    /// Date and time or <see cref="DateTime.Now"/> when missing
    /// </returns>
    public DateTime GetDateTime(string aKey)
    {
      return this.GetDateTime(aKey, DateTime.Now);
    }

    /// <summary>
    /// Stores a date and time using <see cref="SetLong"/> and
    /// <see cref="DateTime.ToBinary"/>.
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="aValue">Value to store</param>
    public virtual void SetDateTime(string aKey, DateTime aValue)
    {
      this.SetLong(aKey, aValue.ToBinary());
    }

    /// <summary>
    /// Gets a guid using <see cref="GetBytes(string)"/>.
    /// </summary>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">Default value to use</param>
    /// <returns>Stored value or aDefault</returns>
    public virtual Guid GetGuid(string aKey, Guid aDefault)
    {
      return this.HasKey(aKey) ? new Guid(this.GetBytes(aKey)!) : aDefault;
    }

    /// <summary>
    /// Gets a guid.
    /// </summary>
    /// <param name="aKey">Key to get value for</param>
    /// <returns>
    /// Stored value or <see cref="Guid.Empty"/> when missing
    /// </returns>
    public Guid GetGuid(string aKey)
    {
      return this.GetGuid(aKey, Guid.Empty);
    }

    /// <summary>
    /// Stores a guid using <see cref="SetBytes"/> and 
    /// <see cref="Guid.ToByteArray"/>
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="aValue">Value to store</param>
    public virtual void SetGuid(string aKey, Guid aValue)
    {
      this.SetBytes(aKey, aValue.ToByteArray());
    }

    /// <summary>
    /// Gets the data by loading it into an object that implements the <see cref="IUFStorableObject"/> interface.
    /// </summary>
    /// <para>
    /// The default implementation assumes a <see cref="UFDictionaryStorage"/> was used to store the data as string.
    /// </para>
    /// <param name="aKey">Key to get data for</param>
    /// <param name="anObject">Object to load from the storage</param>
    /// <returns>Value of <c>anObject</c></returns>
    public virtual IUFStorableObject GetStorableObject(string aKey, IUFStorableObject anObject)
    {
      if (this.HasKey(aKey))
      {
        UFDictionaryStorage storage = new UFDictionaryStorage();
        storage.LoadFromString(this.GetString(aKey));
        anObject.LoadFromStorage(storage);
      }
      return anObject;
    }

    /// <summary>
    /// Sets the data by saving it from an object that implements the <see cref="IUFStorableObject"/> interface.
    /// <para>
    /// The default implementation uses a <see cref="UFDictionaryStorage"/> to store the data as string.
    /// </para>
    /// </summary>
    /// <param name="aKey">Key to set data for</param>
    /// <param name="anObject">Object to save to the storage</param>
    public virtual void SetStorableObject(string aKey, IUFStorableObject anObject)
    {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      anObject.SaveToStorage(storage);
      this.SetString(aKey, storage.SaveToString());
    }

    /// <summary>
    /// Calls <see cref="GetObject(string,Type,Func{Type,object})"/>
    /// using <see cref="Activator"/> to create an instance of <c>aType</c>.
    /// </summary>
    /// <param name="aKey">Key to get object for</param>
    /// <param name="aType">type of object</param>
    /// <returns>instance or <c>null</c> if object could not be found</returns>
    public object? GetObject(string aKey, Type aType)
    {
      return this.GetObject(
        aKey,
        aType,
        Activator.CreateInstance
      );
    }

    /// <summary>
    /// A generic version of <see cref="GetObject(string,Type)"/>.
    /// </summary>
    /// <typeparam name="T">Type of value to get</typeparam>
    /// <param name="aKey">Key value was stored with</param>
    /// <returns>Value or null if none could be found</returns>
    public T? GetObject<T>(string aKey) where T : struct
    {
      return (T?) this.GetObject(aKey, typeof(T));
    }

    /// <summary>
    /// Gets an object of a specific type.
    /// </summary>
    /// <remarks>
    /// The default implementation checks for certain type situations.
    /// <para>
    /// If the object is a primitive type, its value gets retrieved with one
    /// of the GetXXXX methods.
    /// </para>
    /// <para>
    /// If the object implements the <see cref="IUFStorableObject"/> interface
    /// the method uses <see cref="GetStorableObject"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="DateTime"/> the data is retrieved via
    /// <see cref="GetDateTime(string)"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="Guid"/> the data is retrieved via
    /// <see cref="GetGuid(string)"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="string"/> the method uses 
    /// <see cref="GetString(string)"/> to get its value.
    /// </para>
    /// <para>
    /// If the object is a <c>enum</c> the method assumes the value was stored
    /// as integer.
    /// </para>
    /// <para>
    /// If the object is a nullable value, the method will return null if
    /// no data exists (null values are not stored) or will call itself
    /// recursively using the underlying type.
    /// </para>
    /// <para>
    /// If none of the above fits, the method will call 
    /// <see cref="DeserializeObject"/>.
    /// </para>
    /// </remarks>
    /// <param name="aKey">
    /// Key to get object for
    /// </param>
    /// <param name="aType">
    /// Type of object or null if type is unknown
    /// </param>
    /// <param name="aFactory">
    /// A factory to create a new instance of <c>aType</c>
    /// </param>
    /// <returns>Object instance or null if none was found and no type was provided</returns>
    public virtual object? GetObject(
      string aKey,
      Type? aType,
      Func<Type, object> aFactory
    )
    {
      if (aType == null)
      {
        return !this.HasKey(aKey)
          ? null
          : this.DeserializeObject(aKey, null, aFactory);
      }
      TypeInfo typeInfo = aType.GetTypeInfo();
      TypeInfo storableObjectTypeInfo =
        typeof(IUFStorableObject).GetTypeInfo();
      // anObject is a primitive type?
      if (typeInfo.IsPrimitive)
      {
        return this.GetPrimitive(aKey, aType);
      }
      // anObject implements the IUFStorableObject interface?
      if (storableObjectTypeInfo.IsAssignableFrom(typeInfo))
      {
        object result = aFactory(aType);
        return this.GetStorableObject(aKey, (IUFStorableObject)result);
      }
      // anObject is a DateTime?
      if (aType == typeof(DateTime))
      {
        return this.GetDateTime(aKey);
      }
      // anObject is a string?
      if (aType == typeof(string))
      {
        return this.GetString(aKey);
      }
      // anObject is a Guid?
      if (aType == typeof(Guid))
      {
        return this.GetGuid(aKey);
      }
      // anObject is an enum?
      if (typeInfo.IsEnum)
      {
        return this.GetInt(aKey, (int)Activator.CreateInstance(aType));
      }
      // anObject is a nullable type?
      if (
        typeInfo.IsGenericType &&
        (aType.GetGenericTypeDefinition() == typeof(Nullable<>))
      )
      {
        // null values are never stored, so if there is no data for the key
        // it has the value null. Else repeat with type without nullable
        // definition to get the stored data.
        return !this.HasKey(aKey)
          ? null
          : this.GetObject(aKey, Nullable.GetUnderlyingType(aType));
      }
      // try to get an object using serialization
      return this.DeserializeObject(aKey, aType, aFactory);
    }

    /// <summary>
    /// Stores an object to the storage using a certain type. 
    /// </summary>
    /// <remarks>
    /// The default implementation checks for certain type situations.
    /// <para>
    /// If the object is null, the method will remove the data stored with
    /// the key value by calling <see cref="DeleteKey"/>
    /// </para>
    /// <para>
    /// If the object is a primitive type, its value gets stored with one
    /// of the SetXXXX methods.
    /// </para>
    /// <para>
    /// If the object implements the <see cref="IUFStorableObject"/> interface
    /// the method uses <see cref="SetStorableObject"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="DateTime"/> the data is stored via
    /// <see cref="SetDateTime"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="Guid"/> the data is stored via
    /// <see cref="SetGuid"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="string"/> the method uses 
    /// <see cref="SetString"/> to store its value.
    /// </para>
    /// <para>
    /// If the object is a nullable value, the method will remove any value
    /// stored with aKey if the object is null else it will call itself
    /// recursively using the underlying type.
    /// </para>
    /// <para>
    /// If none of the above fits, the method will call 
    /// <see cref="SerializeObject"/>.
    /// </para>
    /// </remarks>
    /// <param name="aKey">Key to store object for</param>
    /// <param name="anObject">object to store</param>
    /// <param name="aType">type of object</param>
    public virtual void SetObject(string aKey, object? anObject, Type? aType)
    {
      // trying to store null value?
      if (anObject == null)
      {
        // yes, delete key (if any) and exit
        if (this.HasKey(aKey))
        {
          this.DeleteKey(aKey);
        }
        return;
      }
      // no type is specified?
      if (aType == null)
      {
        // try to store using serialization 
        this.SerializeObject(aKey, anObject);
        return;
      }
      TypeInfo typeInfo = aType.GetTypeInfo();
      TypeInfo storableObjectTypeInfo = typeof(IUFStorableObject).GetTypeInfo();
      // anObject is a primitive type?
      if (typeInfo.IsPrimitive)
      {
        this.SetPrimitive(aKey, anObject, aType);
      }
      // anObject implements the IUFStorableObject interface?
      else if (storableObjectTypeInfo.IsAssignableFrom(typeInfo))
      {
        this.SetStorableObject(aKey, (IUFStorableObject)anObject);
      }
      // anObject is a DateTime instance?
      else if (aType == typeof(DateTime))
      {
        this.SetDateTime(aKey, (DateTime)anObject);
      }
      // anObject is a string?
      else if (aType == typeof(string))
      {
        this.SetString(aKey, (string)anObject);
      }
      // anObject is a guid?
      else if (aType == typeof(Guid))
      {
        this.SetGuid(aKey, (Guid)anObject);
      }
      // anObject is an enum?
      else if (typeInfo.IsEnum)
      {
        this.SetInt(aKey, Convert.ToInt32(anObject));
      }
      // anObject is a nullable type?
      else if (typeInfo.IsGenericType && (aType.GetGenericTypeDefinition() == typeof(Nullable<>)))
      {
        // repeat with type without nullable definition since value is not null
        this.SetObject(aKey, anObject, Nullable.GetUnderlyingType(aType));
      }
      // try to store anObject using serialization
      else
      {
        this.SerializeObject(aKey, anObject);
      }
    }

    /// <summary>
    /// Stores an object. It calls <see cref="SetObject(string,object,Type)"/>
    /// using <see cref="Object.GetType"/> (if anObject is non null).
    /// </summary>
    /// <param name="aKey">Key to store object for</param>
    /// <param name="anObject">Object to store</param>
    public void SetObject(string aKey, object anObject)
    {
      this.SetObject(aKey, anObject, anObject?.GetType());
    }

    /// <summary>
    /// Generic version of <see cref="SetObject(string,object,Type)"/>.
    /// </summary>
    /// <typeparam name="T">Type to store</typeparam>
    /// <param name="aKey">Key to store object for</param>
    /// <param name="anObject">Object to store</param>
    public void SetObject<T>(string aKey, object anObject)
    {
      this.SetObject(aKey, anObject, typeof(T));
    }

    /// <summary>
    /// Deletes all stored data.
    /// </summary>
    public virtual void DeleteAll()
    {
    }

    /// <summary>
    /// Deletes the data for specific key.
    /// </summary>
    /// <param name="aKey">
    /// A key to Delete the data for.
    /// </param>
    public abstract void DeleteKey(string aKey);

    /// <summary>
    /// Checks if there is a locally stored data for a specific key.
    /// </summary>
    /// <param name="aKey">
    /// A key to check.
    /// </param>
    /// <returns>
    /// True if has there is data for the key; otherwise, false.
    /// </returns>
    public abstract bool HasKey(string aKey);

    #endregion

    #region protected overridable methods

    /// <summary>
    /// Serializes an object. This method is called when the class can not store the object in any other way.
    /// <para>
    /// The default implementation throws an exception. 
    /// </para>
    /// </summary>
    /// <param name="aKey">Key to store serializes object with</param>
    /// <param name="anObject">Object to store</param>
    protected virtual void SerializeObject(string aKey, object anObject)
    {
      throw new NotImplementedException($"Unknown object type for {aKey}: {anObject.GetType().Name}");
    }

    /// <summary>
    /// Deserializes an object that was stored previously with 
    /// <see cref="SerializeObject" />.
    /// </summary>
    /// <para>
    /// The default implementation throws an exception. 
    /// </para>
    /// <param name="aKey">Key to retrieve serialized object with</param>
    /// <param name="aType">Type of object to create (might be <c>null</c>)</param>
    /// <param name="aFactory">A factory to create a new instance of <c>aType</c></param>
    /// <returns>Retrieved object</returns>
    protected virtual object DeserializeObject(string aKey, Type? aType, Func<Type, object> aFactory)
    {
      throw new NotImplementedException($"Unknown object type for {aKey}: {aType?.Name ?? "null"}");
    }

    #endregion

    #region private methods

    /// <summary>
    /// Calls one of the SetXXXX methods depending on the type.
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="anObject">Value to store</param>
    /// <param name="aType">Type of value</param>
    private void SetPrimitive(string aKey, object anObject, Type aType)
    {
      // save with correct typecast.
      if (aType == typeof(byte))
      {
        this.SetByte(aKey, (byte)anObject);
      }
      else if (aType == typeof(sbyte))
      {
        this.SetSByte(aKey, (sbyte)anObject);
      }
      else if (aType == typeof(short))
      {
        this.SetShort(aKey, (short)anObject);
      }
      else if (aType == typeof(ushort))
      {
        this.SetUShort(aKey, (ushort)anObject);
      }
      else if (aType == typeof(int))
      {
        this.SetInt(aKey, (int)anObject);
      }
      else if (aType == typeof(uint))
      {
        this.SetUInt(aKey, (uint)anObject);
      }
      else if (aType == typeof(long))
      {
        this.SetLong(aKey, (long)anObject);
      }
      else if (aType == typeof(ulong))
      {
        this.SetLong(aKey, (long)anObject);
      }
      else if (aType == typeof(float))
      {
        this.SetFloat(aKey, (float)anObject);
      }
      else if (aType == typeof(double))
      {
        this.SetDouble(aKey, (double)anObject);
      }
      else if (aType == typeof(bool))
      {
        this.SetBool(aKey, (bool)anObject);
      }
      else if (aType == typeof(char))
      {
        this.SetChar(aKey, (char)anObject);
      }
      else
      {
        throw new Exception("Unknown primitive type: " + aType.Name);
      }
    }

    /// <summary>
    /// Calls one of the GetXXXX methods depending on the type.
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="aType">Type of value</param>
    /// <returns>Value for aKey</returns>
    private object GetPrimitive(string aKey, Type aType)
    {
      // save with correct typecast.
      if (aType == typeof(byte))
      {
        return this.GetByte(aKey);
      }
      if (aType == typeof(sbyte))
      {
        return this.GetSByte(aKey);
      }
      if (aType == typeof(short))
      {
        return this.GetShort(aKey);
      }
      if (aType == typeof(ushort))
      {
        return this.GetUShort(aKey);
      }
      if (aType == typeof(int))
      {
        return this.GetInt(aKey);
      }
      if (aType == typeof(uint))
      {
        return this.GetUInt(aKey);
      }
      if (aType == typeof(long))
      {
        return this.GetLong(aKey);
      }
      if (aType == typeof(ulong))
      {
        return this.GetLong(aKey);
      }
      if (aType == typeof(float))
      {
        return this.GetFloat(aKey);
      }
      if (aType == typeof(double))
      {
        return this.GetDouble(aKey);
      }
      if (aType == typeof(bool))
      {
        return this.GetBool(aKey);
      }
      if (aType == typeof(char))
      {
        return this.GetChar(aKey);
      }
      throw new Exception("Unknown primitive type: " + aType.Name);
    }

    #endregion
  }
}