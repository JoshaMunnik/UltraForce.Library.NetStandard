// <copyright file="UFKeyedStorageAsync.cs" company="Ultra Force Development">
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
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Storage
{
  /// <summary>
  /// <see cref="UFKeyedStorageAsync"/> is a base class for storage classes
  /// that store different type of values using unique keys.
  /// </summary>
  /// <remarks>
  /// It is a async version of <see cref="UFKeyedStorage"/>.
  /// </remarks>
  /// <remarks>
  /// The default implementations of storage methods for the various types
  /// use the <see cref="GetStringAsync(string)" /> and
  /// <see cref="SetStringAsync(string, string)" /> method to store textual 
  /// representations of the data.
  /// <para>
  /// The class itself is an abstract class, the following methods need to be 
  /// implemented in a subclass: <see cref="GetStringAsync(string,string)" />,
  /// <see cref="SetStringAsync" />, <see cref="HasKeyAsync" /> and
  /// <see cref="DeleteKeyAsync" />.
  /// </para>
  /// <para>
  /// If possible a subclass should also implement
  /// <see cref="DeleteAllAsync" />.
  /// </para>
  /// </remarks>
  [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
  public abstract class UFKeyedStorageAsync
  {
    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFKeyedStorageAsync"/>
    /// </summary>
    protected UFKeyedStorageAsync()
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
    public abstract Task<string> GetStringAsync(string aKey, string aDefault);

    /// <summary>
    /// Gets a string.
    /// </summary>
    /// <param name="aKey">A key to get the string for.</param>
    /// <returns>The stored string or "" if none could be found.</returns>
    public async Task<string> GetStringAsync(string aKey)
    {
      return await this.GetStringAsync(aKey, string.Empty);
    }

    /// <summary>
    /// Stores a string in the storage.
    /// </summary>
    /// <param name="aKey">Key to store value for.</param>
    /// <param name="aValue">A value to store.</param>
    public abstract Task SetStringAsync(string aKey, string aValue);

    /// <summary>
    /// Gets a byte.
    /// <para>
    /// The default implementation calls <see cref="GetIntAsync(string)" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">A key to get the byte for.</param>
    /// <param name="aDefault">
    /// A default value to use when there is no byte stored for the key.
    /// </param>
    /// <returns>The stored byte or aDefault.</returns>
    public virtual async Task<byte> GetByteAsync(string aKey, byte aDefault)
    {
      return await this.HasKeyAsync(aKey) ? (byte)await this.GetIntAsync(aKey) : aDefault;
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
    public async Task<byte> GetByteAsync(string aKey)
    {
      return await this.GetByteAsync(aKey, 0);
    }

    /// <summary>
    /// Stores a byte in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetIntAsync" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetByteAsync(string aKey, byte aValue)
    {
      await this.SetIntAsync(aKey, aValue);
    }

    /// <summary>
    /// Gets a sbyte.
    /// <para>
    /// The default implementation calls <see cref="GetIntAsync(string)" />.
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
    public virtual async Task<sbyte> GetSByteAsync(string aKey, sbyte aDefault)
    {
      return await this.HasKeyAsync(aKey) ? (sbyte)await this.GetIntAsync(aKey) : aDefault;
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
    public async Task<sbyte> GetSByteAsync(string aKey)
    {
      return await this.GetSByteAsync(aKey, 0);
    }

    /// <summary>
    /// Stores a sbyte in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetIntAsync" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetSByteAsync(string aKey, sbyte aValue)
    {
      await this.SetIntAsync(aKey, aValue);
    }

    /// <summary>
    /// Gets a short.
    /// <para>
    /// The default implementation calls <see cref="GetIntAsync(string)" />.
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
    public virtual async Task<short> GetShortAsync(
      string aKey,
      short aDefault
    )
    {
      return await this.HasKeyAsync(aKey)
        ? (short)await this.GetIntAsync(aKey)
        : aDefault;
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
    public async Task<short> GetShortAsync(string aKey)
    {
      return await this.GetShortAsync(aKey, 0);
    }

    /// <summary>
    /// Stores a short in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetIntAsync" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetShortAsync(string aKey, short aValue)
    {
      await this.SetIntAsync(aKey, aValue);
    }

    /// <summary>
    /// Gets a ushort.
    /// <para>
    /// The default implementation calls <see cref="GetIntAsync(string)" />.
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
    public virtual async Task<ushort> GetUShortAsync(
      string aKey,
      ushort aDefault
    )
    {
      return await this.HasKeyAsync(aKey) ? (ushort)await this.GetIntAsync(aKey) : aDefault;
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
    public async Task<ushort> GetUShortAsync(string aKey)
    {
      return await this.GetUShortAsync(aKey, 0);
    }

    /// <summary>
    /// Stores a ushort in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetIntAsync" />.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetUShortAsync(string aKey, ushort aValue)
    {
      await this.SetIntAsync(aKey, aValue);
    }

    /// <summary>
    /// Gets an integer.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and uses <see cref="int.Parse(string)"/> to convert it back to
    /// a integer.
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
    public virtual async Task<int> GetIntAsync(string aKey, int aDefault)
    {
      return await this.HasKeyAsync(aKey) ? int.Parse(await this.GetStringAsync(aKey)) : aDefault;
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
    public async Task<int> GetIntAsync(string aKey)
    {
      return await this.GetIntAsync(aKey, 0);
    }

    /// <summary>
    /// Stores an integer in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetIntAsync(string aKey, int aValue)
    {
      await this.SetStringAsync(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets an unsigned integer.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and uses <see cref="uint.Parse(string)"/> to convert it back to an
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
    public virtual async Task<uint> GetUIntAsync(string aKey, uint aDefault)
    {
      return await this.HasKeyAsync(aKey) ? uint.Parse(await this.GetStringAsync(aKey)) : aDefault;
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
    public Task<uint> GetUIntAsync(string aKey)
    {
      return this.GetUIntAsync(aKey, 0);
    }

    /// <summary>
    /// Stores an unsigned integer in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetUIntAsync(string aKey, uint aValue)
    {
      await this.SetStringAsync(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets a long.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and uses <see cref="long.Parse(string)"/> to convert it back to a long.
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
    public virtual async Task<long> GetLongAsync(string aKey, long aDefault)
    {
      return await this.HasKeyAsync(aKey) ? long.Parse(await this.GetStringAsync(aKey)) : aDefault;
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
    public async Task<long> GetLongAsync(string aKey)
    {
      return await this.GetLongAsync(aKey, 0);
    }

    /// <summary>
    /// Stores a long in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetLongAsync(string aKey, long aValue)
    {
      await this.SetStringAsync(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets a ulong.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and uses <see cref="ulong.Parse(string)"/> to convert it back to 
    /// a ulong.
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
    public virtual async Task<ulong> GetULongAsync(string aKey, ulong aDefault)
    {
      return await this.HasKeyAsync(aKey) ? ulong.Parse(await this.GetStringAsync(aKey)) : aDefault;
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
    public async Task<ulong> GetULongAsync(string aKey)
    {
      return await this.GetULongAsync(aKey, 0);
    }

    /// <summary>
    /// Stores a ulong in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetULongAsync(string aKey, ulong aValue)
    {
      await this.SetStringAsync(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets a floating number.
    /// <para>
    /// The default implementation just calls 
    /// <see cref="GetDoubleAsync(string)"/> and typecast the value back to 
    /// float.
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
    public virtual async Task<float> GetFloatAsync(string aKey, float aDefault)
    {
      return await this.HasKeyAsync(aKey) ? (float)await this.GetDoubleAsync(aKey) : aDefault;
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
    public Task<float> GetFloatAsync(string aKey)
    {
      return this.GetFloatAsync(aKey, 0.0f);
    }

    /// <summary>
    /// Stores a floating number in the storage.
    /// <para>
    /// The default implementation calls <see cref="SetDoubleAsync"/>.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param> 
    public virtual async Task SetFloatAsync(string aKey, float aValue)
    {
      await this.SetDoubleAsync(aKey, aValue);
    }

    /// <summary>
    /// Gets a double number.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and uses <see cref="double.Parse(string)"/> to convert it back to 
    /// a double.
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
    public virtual async Task<double> GetDoubleAsync(
      string aKey,
      double aDefault
    )
    {
      return await this.HasKeyAsync(aKey) ? double.Parse(await this.GetStringAsync(aKey)) : aDefault;
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
    public async Task<double> GetDoubleAsync(string aKey)
    {
      return await this.GetDoubleAsync(aKey, 0.0);
    }

    /// <summary>
    /// Stores a double number in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetDoubleAsync(string aKey, double aValue)
    {
      await this.SetStringAsync(
        aKey,
        aValue.ToString(CultureInfo.InvariantCulture)
      );
    }

    /// <summary>
    /// Gets a bool.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and converts its value back to a bool.
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
    public virtual async Task<bool> GetBoolAsync(string aKey, bool aDefault)
    {
      return await this.HasKeyAsync(aKey) ? !(await this.GetStringAsync(aKey)).Equals("0") : aDefault;
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
    public async Task<bool> GetBoolAsync(string aKey)
    {
      return await this.GetBoolAsync(aKey, true);
    }

    /// <summary>
    /// Stores a bool in the storage.
    /// <para>
    /// The default implementation convert the value to a string and uses
    /// <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetBoolAsync(string aKey, bool aValue)
    {
      await this.SetStringAsync(aKey, aValue ? "1" : "0");
    }

    /// <summary>
    /// Gets a char.
    /// <para>
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// to get the stored character.
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
    public virtual async Task<char> GetCharAsync(string aKey, char aDefault)
    {
      return await this.HasKeyAsync(aKey) ? (await this.GetStringAsync(aKey))[0] : aDefault;
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
    public async Task<char> GetCharAsync(string aKey)
    {
      return await this.GetCharAsync(aKey, '\0');
    }

    /// <summary>
    /// Stores a char in the storage.
    /// <para>
    /// The default implementation just uses <see cref="SetStringAsync"/> to 
    /// store the value.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for.
    /// </param>
    /// <param name="aValue">
    /// A value to store.
    /// </param>
    public virtual async Task SetCharAsync(string aKey, char aValue)
    {
      await this.SetStringAsync(aKey, aValue.ToString());
    }

    /// <summary>
    /// Gets an array of bytes.
    /// <para> 
    /// The default implementation uses <see cref="GetStringAsync(string)"/> 
    /// and assumes the data is is a base64 encoded string.
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
    public virtual async Task<byte[]?> GetBytesAsync(
      string aKey,
      byte[]? aDefault
    )
    {
      return await this.HasKeyAsync(aKey)
        ? Convert.FromBase64String(await this.GetStringAsync(aKey))
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
    public async Task<byte[]?> GetBytesAsync(string aKey)
    {
      return await this.GetBytesAsync(aKey, null);
    }

    /// <summary>
    /// Stores an array of bytes to the storage. 
    /// <para>
    /// The default implementation converts the array to a base64 encoded
    /// string and uses <see cref="SetStringAsync"/> to store it.
    /// </para>
    /// </summary>
    /// <param name="aKey">
    /// Key to store value for
    /// </param>
    /// <param name="aValue">
    /// Array of byte to store
    /// </param>
    public virtual async Task SetBytesAsync(string aKey, byte[] aValue)
    {
      await this.SetStringAsync(aKey, Convert.ToBase64String(aValue));
    }

    /// <summary>
    /// Gets a <see cref="DateTime"/> value using 
    /// <see cref="GetLongAsync(string)"/> and 
    /// <see cref="DateTime.FromBinary"/>.
    /// </summary>
    /// <param name="aKey">Key to get date and time for</param>
    /// <param name="aDefault">Default value to use</param>
    /// <returns>Store value or aDefault</returns>
    public virtual async Task<DateTime> GetDateTimeAsync(
      string aKey,
      DateTime aDefault
    )
    {
      return await this.HasKeyAsync(aKey) ? DateTime.FromBinary(await this.GetLongAsync(aKey)) : aDefault;
    }

    /// <summary>
    /// Gets a date and time.
    /// </summary>
    /// <param name="aKey">Key to get date and time for</param>
    /// <returns>
    /// Date and time or <see cref="DateTime.Now"/> when missing
    /// </returns>
    public async Task<DateTime> GetDateTimeAsync(string aKey)
    {
      return await this.GetDateTimeAsync(aKey, DateTime.Now);
    }

    /// <summary>
    /// Stores a date and time using <see cref="SetLongAsync"/> and
    /// <see cref="DateTime.ToBinary"/>.
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="aValue">Value to store</param>
    public virtual async Task SetDateTimeAsync(string aKey, DateTime aValue)
    {
      await this.SetLongAsync(aKey, aValue.ToBinary());
    }

    /// <summary>
    /// Gets a guid using <see cref="GetBytesAsync(string)"/>.
    /// </summary>
    /// <param name="aKey">Key to get value for</param>
    /// <param name="aDefault">Default value to use</param>
    /// <returns>Stored value or aDefault</returns>
    public virtual async Task<Guid> GetGuidAsync(string aKey, Guid aDefault)
    {
      if (!await this.HasKeyAsync(aKey))
      {
        return aDefault;
      }
      byte[]? bytes = await this.GetBytesAsync(aKey);
      return bytes != null ? new Guid(bytes) : aDefault;
    }

    /// <summary>
    /// Gets a guid.
    /// </summary>
    /// <param name="aKey">Key to get value for</param>
    /// <returns>
    /// Stored value or <see cref="Guid.Empty"/> when missing
    /// </returns>
    public async Task<Guid> GetGuidAsync(string aKey)
    {
      return await this.GetGuidAsync(aKey, Guid.Empty);
    }

    /// <summary>
    /// Stores a guid using <see cref="SetBytesAsync"/> and 
    /// <see cref="Guid.ToByteArray"/>
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="aValue">Value to store</param>
    public virtual async Task SetGuidAsync(string aKey, Guid aValue)
    {
      await this.SetBytesAsync(aKey, aValue.ToByteArray());
    }

    /// <summary>
    /// Gets the data by loading it into an object that implements the
    /// <see cref="IUFStorableObject"/> interface.
    /// </summary>
    /// <para>
    /// The default implementation assumes a <see cref="UFDictionaryStorage"/>
    /// was used to store the data as string.
    /// </para>
    /// <param name="aKey">Key to get data for</param>
    /// <param name="anObject">Object to load from the storage</param>
    /// <returns>Value of <c>anObject</c></returns>
    public virtual async Task<IUFStorableObject> GetStorableObjectAsync(
      string aKey,
      IUFStorableObject anObject
    )
    {
      if (await this.HasKeyAsync(aKey))
      {
        UFDictionaryStorage storage = new UFDictionaryStorage();
        storage.LoadFromString(await this.GetStringAsync(aKey));
        anObject.LoadFromStorage(storage);
      }
      return anObject;
    }

    /// <summary>
    /// Sets the data by saving it from an object that implements the
    /// <see cref="IUFStorableObject"/> interface.
    /// <para>
    /// The default implementation uses a <see cref="UFDictionaryStorage"/>
    /// to store the data as string.
    /// </para>
    /// </summary>
    /// <param name="aKey">Key to set data for</param>
    /// <param name="anObject">Object to save to the storage</param>
    public virtual async Task SetStorableObjectAsync(
      string aKey,
      IUFStorableObject anObject
    )
    {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      anObject.SaveToStorage(storage);
      await this.SetStringAsync(aKey, storage.SaveToString());
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
    /// the method uses <see cref="GetStorableObjectAsync"/> using aKey.
    /// </para>
    /// <para>
    /// If the object is a <see cref="DateTime"/> the data is retrieved via
    /// <see cref="GetDateTimeAsync(string)"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="Guid"/> the data is retrieved via
    /// <see cref="GetGuidAsync(string)"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="string"/> the method uses 
    /// <see cref="GetStringAsync(string)"/> to get its value.
    /// </para>
    /// <para>
    /// If the object is a nullable value, the method will return null if
    /// no data exists (null values are not stored) or will call itself
    /// recursively using the underlying type.
    /// </para>
    /// <para>
    /// If none of the above fits, the method will call 
    /// <see cref="DeserializeObjectAsync"/>.
    /// </para>
    /// </remarks>
    /// <param name="aKey">Key to get object for</param>
    /// <param name="aType">Type of object or null if type is unknown</param>
    /// <returns>Object instance or null if none was found</returns>
    public virtual async Task<object?> GetObjectAsync(string aKey, Type? aType)
    {
      if (!await this.HasKeyAsync(aKey))
      {
        return null;
      }
      if (aType == null)
      {
        return await this.DeserializeObjectAsync(aKey, null);
      }
      TypeInfo typeInfo = aType.GetTypeInfo();
      TypeInfo storableObjectTypeInfo =
        typeof(IUFStorableObjectAsync).GetTypeInfo();
      // anObject is a primitive type?
      if (typeInfo.IsPrimitive)
      {
        return await this.GetPrimitiveAsync(aKey, aType);
      }
      // anObject implements the IUFStorableObject interface?
      if (storableObjectTypeInfo.IsAssignableFrom(typeInfo))
      {
        object result = Activator.CreateInstance(aType);
        return await this.GetStorableObjectAsync(
          aKey,
          (IUFStorableObject)result
        );
      }
      // anObject is a DateTime instance?
      if (aType == typeof(DateTime))
      {
        // yes, use FromBinary to get its contents as long
        return await this.GetDateTimeAsync(aKey);
      }
      // anObject is a string?
      if (aType == typeof(string))
      {
        return await this.GetStringAsync(aKey);
      }
      // anObject is a guid?
      if (aType == typeof(Guid))
      {
        return await this.GetGuidAsync(aKey);
      }
      // anObject is a nullable type?
      if (
        typeInfo.IsGenericType &&
        (aType.GetGenericTypeDefinition() == typeof(Nullable<>))
      )
      {
        // repeat with type without nullable definition since value is not null
        return await this.GetObjectAsync(
          aKey,
          Nullable.GetUnderlyingType(aType)
        );
      }
      // try to get an object using serialization
      return await this.DeserializeObjectAsync(aKey, aType);
    }

    /// <summary>
    /// A generic version of <see cref="GetObjectAsync"/>.
    /// </summary>
    /// <typeparam name="T">Type of object to get</typeparam>
    /// <param name="aKey">Key to get object for</param>
    /// <returns>object</returns>
    public async Task<T?> GetObjectAsync<T>(string aKey)
    {
      return (T?) await this.GetObjectAsync(aKey, typeof(T));
    }

    /// <summary>
    /// Stores an object to the storage. 
    /// </summary>
    /// <remarks>
    /// The default implementation checks for certain type situations.
    /// <para>
    /// If the object is null, the method will remove the data stored with
    /// the key value by calling <see cref="DeleteKeyAsync"/>
    /// </para>
    /// <para>
    /// If the object is a primitive type, its value gets stored with one
    /// of the SetXXXX methods.
    /// </para>
    /// <para>
    /// If the object implements the <see cref="IUFStorableObject"/> interface
    /// the method uses <see cref="SetStorableObjectAsync"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="DateTime"/> the data is stored via
    /// <see cref="SetDateTimeAsync"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="Guid"/> the data is stored via
    /// <see cref="SetGuidAsync"/>.
    /// </para>
    /// <para>
    /// If the object is a <see cref="string"/> the method uses 
    /// <see cref="SetStringAsync"/> to store its value.
    /// </para>
    /// <para>
    /// If the object is a nullable value, the method will remove any value
    /// stored with aKey if the object is null else it will call itself
    /// recursively using the underlying type.
    /// </para>
    /// <para>
    /// If none of the above fits, the method will call 
    /// <see cref="SerializeObjectAsync"/>.
    /// </para>
    /// </remarks>
    /// <param name="aKey"></param>
    /// <param name="anObject"></param>
    /// <param name="aType"></param>
    public virtual async Task SetObjectAsync(
      string aKey,
      object? anObject,
      Type? aType
    )
    {
      // trying to store null value?
      if (anObject == null)
      {
        // yes, delete key (if any) and exit
        if (await this.HasKeyAsync(aKey))
        {
          await this.DeleteKeyAsync(aKey);
        }
        return;
      }
      // no type is specified?
      if (aType == null)
      {
        // try to store using serialization to a byte array
        await this.SerializeObjectAsync(aKey, anObject);
        return;
      }
      TypeInfo typeInfo = aType.GetTypeInfo();
      TypeInfo storableObjectTypeInfo =
        typeof(IUFStorableObjectAsync).GetTypeInfo();
      // anObject is a primitive type?
      if (typeInfo.IsPrimitive)
      {
        await this.SetPrimitiveAsync(aKey, anObject, aType);
      }
      // anObject implements the IUFStorableObject interface?
      else if (storableObjectTypeInfo.IsAssignableFrom(typeInfo))
      {
        await this.SetStorableObjectAsync(aKey, (IUFStorableObject)anObject);
      }
      // anObject is a DateTime instance?
      else if (aType == typeof(DateTime))
      {
        // yes, use ToBinary to store its contents as long
        await this.SetDateTimeAsync(aKey, (DateTime)anObject);
      }
      // anObject is a string?
      else if (aType == typeof(string))
      {
        await this.SetStringAsync(aKey, (string)anObject);
      }
      // anObject is a guid?
      else if (aType == typeof(Guid))
      {
        await this.SetGuidAsync(aKey, (Guid)anObject);
      }
      // anObject is a nullable type?
      else if (
        typeInfo.IsGenericType
        && (aType.GetGenericTypeDefinition() == typeof(Nullable<>))
      )
      {
        // repeat with type without nullable definition since value is not null
        await this.SetObjectAsync(
          aKey,
          anObject,
          Nullable.GetUnderlyingType(aType)
        );
      }
      // try to store anObject using serialization
      else
      {
        await this.SerializeObjectAsync(aKey, anObject);
      }
    }

    /// <summary>
    /// Stores an object. 
    /// </summary>
    /// <param name="aKey">Key to store object for</param>
    /// <param name="anObject">Object to store</param>
    public async Task SetObjectAsync(string aKey, object? anObject)
    {
      await this.SetObjectAsync(aKey, anObject, anObject?.GetType());
    }
    
    /// <summary>
    /// Generic version of <see cref="SetObjectAsync(string,object,Type)"/>.
    /// </summary>
    /// <typeparam name="T">Type to store</typeparam>
    /// <param name="aKey">Key to store object for</param>
    /// <param name="anObject">Object to store</param>
    public async Task SetObjectAsync<T>(
      string aKey,
      T anObject
    )
    {
      await this.SetObjectAsync(aKey, anObject, typeof(T));
    }

    /// <summary>
    /// Deletes all stored data.
    /// <para>
    /// The default implemetnation does nothing.
    /// </para>
    /// </summary>
    public virtual Task DeleteAllAsync()
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes the data for specific key.
    /// </summary>
    /// <param name="aKey">
    /// A key to Delete the data for.
    /// </param>
    public abstract Task DeleteKeyAsync(string aKey);

    /// <summary>
    /// Checks if there is a locally stored data for a specific key.
    /// </summary>
    /// <param name="aKey">
    /// A key to check.
    /// </param>
    /// <returns>
    /// True if has there is data for the key; otherwise, false.
    /// </returns>
    public abstract Task<bool> HasKeyAsync(string aKey);

    #endregion

    #region protected methods

    /// <summary>
    /// Serializes an object. This method is called when the class can not
    /// store the object in any other way.
    /// <para>
    /// The default implementation throws an exception. 
    /// </para>
    /// </summary>
    /// <param name="aKey">Key to store serializes object with</param>
    /// <param name="anObject">Object to store</param>
    protected virtual Task SerializeObjectAsync(string aKey, object anObject)
    {
      throw new NotImplementedException(
        $"Unknown object type for {aKey}: {anObject.GetType().Name}"
      );
    }

    /// <summary>
    /// Deserializes an object that was stored previously with 
    /// <see cref="SerializeObjectAsync" />.
    /// </summary>
    /// <para>
    /// The default implementation throws an exception. 
    /// </para>
    /// <param name="aKey">Key to retrieve serialized object with</param>
    /// <param name="aType">Type of object to create (might be null)</param>
    /// <returns>Retrieved object</returns>
    protected virtual Task<object> DeserializeObjectAsync(
      string aKey,
      Type? aType
    )
    {
      throw new NotImplementedException(
        $"Unknown object type for {aKey}: {aType?.Name ?? "(null)"}"
      );
    }

    #endregion

    #region private methods

    /// <summary>
    /// Calls one of the SetXXXX methods depending on the type.
    /// </summary>
    /// <param name="aKey">Key to store value for</param>
    /// <param name="anObject">Value to store</param>
    /// <param name="aType">Type of value</param>
    private async Task SetPrimitiveAsync(string aKey, object anObject, Type aType)
    {
      // save with correct typecast.
      if (aType == typeof(byte))
      {
        await this.SetByteAsync(aKey, (byte)anObject);
      }
      else if (aType == typeof(sbyte))
      {
        await this.SetSByteAsync(aKey, (sbyte)anObject);
      }
      else if (aType == typeof(short))
      {
        await this.SetShortAsync(aKey, (short)anObject);
      }
      else if (aType == typeof(ushort))
      {
        await this.SetUShortAsync(aKey, (ushort)anObject);
      }
      else if (aType == typeof(int))
      {
        await this.SetIntAsync(aKey, (int)anObject);
      }
      else if (aType == typeof(uint))
      {
        await this.SetUIntAsync(aKey, (uint)anObject);
      }
      else if (aType == typeof(long))
      {
        await this.SetLongAsync(aKey, (long)anObject);
      }
      else if (aType == typeof(ulong))
      {
        await this.SetLongAsync(aKey, (long)anObject);
      }
      else if (aType == typeof(float))
      {
        await this.SetFloatAsync(aKey, (float)anObject);
      }
      else if (aType == typeof(double))
      {
        await this.SetDoubleAsync(aKey, (double)anObject);
      }
      else if (aType == typeof(bool))
      {
        await this.SetBoolAsync(aKey, (bool)anObject);
      }
      else if (aType == typeof(char))
      {
        await this.SetCharAsync(aKey, (char)anObject);
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
    private async Task<object> GetPrimitiveAsync(string aKey, Type aType)
    {
      // save with correct typecast.
      if (aType == typeof(byte))
      {
        return await this.GetByteAsync(aKey);
      }
      else if (aType == typeof(sbyte))
      {
        return await this.GetSByteAsync(aKey);
      }
      else if (aType == typeof(short))
      {
        return await this.GetShortAsync(aKey);
      }
      else if (aType == typeof(ushort))
      {
        return await this.GetUShortAsync(aKey);
      }
      else if (aType == typeof(int))
      {
        return await this.GetIntAsync(aKey);
      }
      else if (aType == typeof(uint))
      {
        return await this.GetUIntAsync(aKey);
      }
      else if (aType == typeof(long))
      {
        return await this.GetLongAsync(aKey);
      }
      else if (aType == typeof(ulong))
      {
        return await this.GetLongAsync(aKey);
      }
      else if (aType == typeof(float))
      {
        return await this.GetFloatAsync(aKey);
      }
      else if (aType == typeof(double))
      {
        return await this.GetDoubleAsync(aKey);
      }
      else if (aType == typeof(bool))
      {
        return await this.GetBoolAsync(aKey);
      }
      else if (aType == typeof(char))
      {
        return await this.GetCharAsync(aKey);
      }
      else
      {
        throw new Exception("Unknown primitive type: " + aType.Name);
      }
    }

    #endregion
  }
}