// <copyright file="UFCryptoHelper.cs" company="Ultra Force Development">
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Security
{
  /// <summary>
  /// <see cref="UFCryptoHelper" /> library is a library that implements
  /// various encryption and decryption methods.
  /// </summary>
  public class UFCryptoHelper
  {
    #region private constants

    /// <summary>
    /// This constant determines the number of iterations for the password 
    /// bytes generation function.
    /// </summary>
    private readonly int m_derivationIterations;

    /// <summary>
    /// Nibble map used to convert hash to 'hex' string.
    /// </summary>
    private readonly char[] m_hashNibbles;

    /// <summary>
    /// Salt to use
    /// </summary>
    private readonly byte[] m_salt;

    /// <summary>
    /// Magic is used when creating public key string.
    /// </summary>
    private const long Magic = 0x7A14C94C53DDF1A5;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFCryptoHelper"/>.
    /// </summary>
    /// <param name="aSalt">
    /// Salt to use (must at least be 8 bytes in length)</param>
    /// <param name="aDerivationIterations">
    /// Number of derivations to use</param>
    /// <param name="aHashNibbles">
    /// A string of 16 unique characters to use as nibble values when
    /// processing 'hex' strings.
    /// </param>
    public UFCryptoHelper(
      byte[] aSalt,
      int aDerivationIterations = 1000,
      string aHashNibbles = "0123456789ABCDEF"
    )
    {
#if UFDEBUG
      // validate parameters
      if (aSalt == null)
      {
        throw new ArgumentException($"{nameof(aSalt)} parameter is null");
      }
      if (aSalt.Length < 8)
      {
        throw new ArgumentException($"{nameof(aSalt)} must contain 8 or more bytes");
      }
      if (aHashNibbles == null)
      {
        throw new ArgumentException($"{nameof(aHashNibbles)} parameter is null");
      }
      if (aHashNibbles.Length != 16)
      {
        throw new ArgumentException($"{nameof(aHashNibbles)} must be exactly 16 characters long");
      }
      if (!UFStringTools.HasUniqueCharacters(aHashNibbles))
      {
        throw new ArgumentException($"{nameof(aHashNibbles)} does not contain unique characters");
      }
#endif
      this.m_salt = aSalt;
      this.m_derivationIterations = aDerivationIterations;
      this.m_hashNibbles = aHashNibbles.ToCharArray();
    }

    #endregion

    #region public methods

    /// <summary>
    /// Generates an hash value using SHA512.
    /// </summary>
    /// <param name="aData">Data to get hash from</param>
    /// <returns>hashed data</returns>
    public string CalcHash(string aData)
    {
      using SHA512 hasher = SHA512.Create();
      byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(aData));
      return UFStringTools.GetHexString(hash, this.m_hashNibbles);
    }

    /// <summary>
    /// Encrypt a text with a key.
    /// </summary>
    /// <remarks>
    /// Based on:
    /// https://xamarinhelp.com/cryptography-in-xamarin-forms/
    /// </remarks>
    /// <param name="aText">Text to encrypt</param>
    /// <param name="aKey">Key to use</param>
    /// <returns>Encrypted text (base64 encoded with one ';' character)</returns>
    public string SymmetricEncrypt(string aText, string aKey)
    {
      using Aes aes = Aes.Create();
      aes.Key = this.CreateKey(aKey);
      byte[] encrypted = this.AesEncryptStringToBytes(aText, aes.Key, aes.IV);
      return Convert.ToBase64String(encrypted) + ";" + Convert.ToBase64String(aes.IV);
    }

    /// <summary>
    /// Decrypt a text previously encrypted with <see cref="SymmetricEncrypt"/>.
    /// </summary>
    /// <remarks>
    /// Based on:
    /// https://xamarinhelp.com/cryptography-in-xamarin-forms/
    /// </remarks>
    /// <param name="aEncryptedValue">Encrypted text</param>
    /// <param name="aKey">Key used to encrypt</param>
    /// <returns>Decrypted text</returns>
    public string SymmetricDecrypt(string aEncryptedValue, string aKey)
    {
      int semicolonPosition = aEncryptedValue.IndexOf(';');
      if (semicolonPosition < 0)
      {
        throw new FormatException("Encrypted value is missing ';'");
      }
      string iv = aEncryptedValue.Substring(
        semicolonPosition + 1,
        aEncryptedValue.Length - semicolonPosition - 1
      );
      aEncryptedValue = aEncryptedValue.Substring(0, semicolonPosition);
      return this.AesDecryptStringFromBytes(
        Convert.FromBase64String(aEncryptedValue),
        this.CreateKey(aKey),
        Convert.FromBase64String(iv)
      );
    }

    /// <summary>
    /// Encrypts text using asymmetric encryption with RSA.
    /// </summary>
    /// <remarks>
    /// Assumes public key is a base64 encoded key using Pkcs1key format
    /// </remarks>
    /// <param name="aData">Data to encrypt</param>
    /// <param name="aPublicKey">Key to encrypt with (Pkcs1key format)</param>
    /// <returns>base64 encoded data</returns>
    public string AsymmetricEncrypt(string aData, string aPublicKey)
    {
      RSA rsa = RSA.Create();
      RSAParameters parameters = this.GetPublicParameters(aPublicKey);
      rsa.ImportParameters(parameters);
      byte[] data = Encoding.UTF8.GetBytes(aData);
      byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1)
        .ToArray();
      return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// Decrypts text using asymmetric encryption with RSA.
    /// </summary>
    /// <param name="aData">Data to decrypt</param>
    /// <param name="aPrivateKey">Private key to decrypt with</param>
    /// <returns>Decrypted data</returns>
    public string AsymmetricDecrypt(string aData, object aPrivateKey)
    {
      RSA rsa = RSA.Create();
      rsa.ImportParameters((RSAParameters)aPrivateKey);
      byte[] data = Convert.FromBase64String(aData);
      byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
      return Encoding.UTF8.GetString(decrypted);
    }

    /// <summary>
    /// Gets a public and private key pair for RSA encryption.
    /// </summary>
    /// <param name="aPublicKey">Public key in private format</param>
    /// <param name="aPrivateKey">Private key</param>
    public void GetAsymmetricKeys(
      out string aPublicKey,
      out object aPrivateKey
    )
    {
      RSA rsa = RSA.Create();
      rsa.KeySize = 2048;
      RSAParameters parameters = rsa.ExportParameters(true);
      aPrivateKey = parameters;
      aPublicKey = this.GetPublicString(parameters);
    }

    /// <summary>
    /// Gets a public and private key pair for RSA encryption.
    /// </summary>
    /// <param name="aPublicKey">Public key in PEM format</param>
    /// <param name="aPrivateKey">Private key</param>
    public void GetAsymmetricKeysPEM(
      out string aPublicKey,
      out object aPrivateKey
    )
    {
      RSA rsa = RSA.Create();
      rsa.KeySize = 2048;
      RSAParameters parameters = rsa.ExportParameters(true);
      aPrivateKey = parameters;
      aPublicKey = this.ExportPublicKey(parameters);
    }

    /// <summary>
    /// Generates a signature using an asymmetric key using SHA256.
    /// </summary>
    /// <param name="aData">Data to create signature for</param>
    /// <param name="aPrivateKey">Private key to sign with</param>
    /// <returns>Signature base64 encoded</returns>
    public string AsymmetricSign(string aData, object aPrivateKey)
    {
      RSA rsa = RSA.Create();
      rsa.ImportParameters((RSAParameters)aPrivateKey);
      byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(aData),
        HashAlgorithmName.SHA256,
        RSASignaturePadding.Pkcs1
      );
      return Convert.ToBase64String(signature);
    }

    #endregion

    #region private methods

    /*
    /// <summary>
    /// Generates 256 random bits
    /// </summary>
    /// <returns>32 byte array</returns>
    private byte[] Generate256BitsOfRandomEntropy() {
      // 32 bytes = 256 bits
      byte[] randomBytes = new byte[32];
      using (RandomNumberGenerator generator = RandomNumberGenerator.Create()) {
        generator.GetBytes(randomBytes);
      }
      return randomBytes;
    }

    /// <summary>
    /// Generates 128 random bits
    /// </summary>
    /// <returns>16 byte array</returns>
    private byte[] Generate128BitsOfRandomEntropy() {
      // 16 bytes = 128 bits
      byte[] randomBytes = new byte[16];
      using (RandomNumberGenerator generator = RandomNumberGenerator.Create()) {
        generator.GetBytes(randomBytes);
      }
      return randomBytes;
    }
    */

    /// <summary>
    /// Creates a key from a password for use with AES.
    /// </summary>
    /// <remarks>
    /// Based on:
    /// https://xamarinhelp.com/cryptography-in-xamarin-forms/
    /// </remarks>
    /// <param name="aPassword">Password to get key for</param>
    /// <param name="aKeyBytes">Size of key</param>
    /// <returns></returns>
    private byte[] CreateKey(string aPassword, int aKeyBytes = 32)
    {
      Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(
        aPassword,
        this.m_salt,
        this.m_derivationIterations
      );
      return keyGenerator.GetBytes(aKeyBytes);
    }

    /// <summary>
    /// Encrypts a string to a byte array.
    /// </summary>
    /// <remarks>
    /// Based on:
    /// https://xamarinhelp.com/cryptography-in-xamarin-forms/
    /// </remarks>
    /// <param name="aPlainText">Text to encrypt</param>
    /// <param name="aKey">Key to use for encryption</param>
    /// <param name="anIV">IV to use for encryption</param>
    /// <returns>encrypted data</returns>
    private byte[] AesEncryptStringToBytes(string aPlainText, byte[] aKey, byte[] anIV)
    {
      if (aPlainText == null || aPlainText.Length <= 0)
      {
        throw new ArgumentNullException($"{nameof(aPlainText)}");
      }
      if (aKey == null || aKey.Length <= 0)
      {
        throw new ArgumentNullException($"{nameof(aKey)}");
      }
      if (anIV == null || anIV.Length <= 0)
      {
        throw new ArgumentNullException($"{nameof(anIV)}");
      }

      using Aes aes = Aes.Create();
      aes.Key = aKey;
      aes.IV = anIV;

      using MemoryStream memoryStream = new MemoryStream();
      using (ICryptoTransform encryptor = aes.CreateEncryptor())
      using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
      using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
      {
        streamWriter.Write(aPlainText);
      }
      byte[] encrypted = memoryStream.ToArray();
      return encrypted;
    }

    /// <summary>
    /// Encrypts a value using AES encryption.
    /// </summary>
    /// <param name="aValue">Value to encrypt</param>
    /// <param name="aKey">Key to encrypt with</param>
    /// <returns></returns>
    public string Encrypt(string aValue, string aKey)
    {
      using Aes aes = Aes.Create();
      aes.Key = this.CreateKey(aKey);
      byte[] encrypted = this.AesEncryptStringToBytes(aValue, aes.Key, aes.IV);
      return Convert.ToBase64String(encrypted)
             + ";" + Convert.ToBase64String(aes.IV);
    }

    /// <summary>
    /// Decrypts a byte array to a string
    /// </summary>
    /// <remarks>
    /// Based on:
    /// https://xamarinhelp.com/cryptography-in-xamarin-forms/
    /// </remarks>
    /// <param name="aCipherText">Encrypted byte array</param>
    /// <param name="aKey">Key to decrypt with</param>
    /// <param name="anIV">VI used during encryption</param>
    /// <returns></returns>
    private string AesDecryptStringFromBytes(byte[] aCipherText, byte[] aKey, byte[] anIV)
    {
      if (aCipherText == null || aCipherText.Length <= 0)
      {
        throw new ArgumentNullException($"{nameof(aCipherText)}");
      }
      if (aKey == null || aKey.Length <= 0)
      {
        throw new ArgumentNullException($"{nameof(aKey)}");
      }
      if (anIV == null || anIV.Length <= 0)
      {
        throw new ArgumentNullException($"{nameof(anIV)}");
      }
      using Aes aes = Aes.Create();
      aes.Key = aKey;
      aes.IV = anIV;
      using MemoryStream memoryStream = new MemoryStream(aCipherText);
      using ICryptoTransform decryptor = aes.CreateDecryptor();
      using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
      using StreamReader streamReader = new StreamReader(cryptoStream);
      return streamReader.ReadToEnd();
    }

    /// <summary>
    /// Gets the public key parameter parts from a string.
    /// </summary>
    /// <param name="aData">Data</param>
    /// <returns>RSA public key parameters</returns>
    private RSAParameters GetPublicParameters(string aData)
    {
      RSAParameters result = new RSAParameters();
      byte[] bytes = Convert.FromBase64String(aData);
      if (bytes.Length <= 8)
      {
        return result;
      }
      using MemoryStream stream = new MemoryStream(bytes);
      using BinaryReader reader = new BinaryReader(stream);
      if (reader.ReadInt64() != Magic)
      {
        return result;
      }
      result.Exponent = UFIOTools.ReadByteArray(reader);
      result.Modulus = UFIOTools.ReadByteArray(reader);
      return result;
    }

    /// <summary>
    /// Gets a string from the public key RSA parameter parts.
    /// </summary>
    /// <param name="aParameters">Parameters to get string from</param>
    /// <returns>Public string (base64 encoded)</returns>
    private string GetPublicString(RSAParameters aParameters)
    {
      //return ExportPublicKey(parameters);
      using MemoryStream stream = new MemoryStream();
      using (BinaryWriter writer = new BinaryWriter(stream))
      {
        writer.Write(Magic);
        UFIOTools.WriteByteArray(writer, aParameters.Exponent);
        UFIOTools.WriteByteArray(writer, aParameters.Modulus);
      }
      return Convert.ToBase64String(stream.ToArray());
    }

    /// <summary>
    /// Exports public key part of RSA parameters as PEM format.
    /// </summary>
    /// <remarks>
    /// Based on code from:
    /// http://stackoverflow.com/a/23739932/968451
    /// http://stackoverflow.com/a/28407693/968451
    /// </remarks>
    /// <param name="aParameters"></param>
    /// <returns>PEM string</returns>
    private string ExportPublicKey(RSAParameters aParameters)
    {
      using MemoryStream stream = new MemoryStream();
      BinaryWriter writer = new BinaryWriter(stream);
      // SEQUENCE
      writer.Write((byte)0x30);
      using (MemoryStream innerStream = new MemoryStream())
      {
        BinaryWriter innerWriter = new BinaryWriter(innerStream);
        // SEQUENCE
        innerWriter.Write((byte)0x30);
        this.WriteLength(innerWriter, 13);
        // OBJECT IDENTIFIER
        innerWriter.Write((byte)0x06);
        byte[] rsaEncryptionOid = new byte[]
          { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
        this.WriteLength(innerWriter, rsaEncryptionOid.Length);
        innerWriter.Write(rsaEncryptionOid);
        // NULL
        innerWriter.Write((byte)0x05);
        this.WriteLength(innerWriter, 0);
        // BIT STRING
        innerWriter.Write((byte)0x03);
        using (MemoryStream bitStringStream = new MemoryStream())
        {
          BinaryWriter bitStringWriter = new BinaryWriter(bitStringStream);
          // # of unused bits
          bitStringWriter.Write((byte)0x00);
          // SEQUENCE
          bitStringWriter.Write((byte)0x30);
          using (MemoryStream paramsStream = new MemoryStream())
          {
            BinaryWriter paramsWriter = new BinaryWriter(paramsStream);
            // Modulus
            this.WriteIntegerBigEndian(paramsWriter, aParameters.Modulus);
            // Exponent
            this.WriteIntegerBigEndian(paramsWriter, aParameters.Exponent);
            int paramsLength = (int)paramsStream.Length;
            this.WriteLength(bitStringWriter, paramsLength);
            bitStringWriter.Write(paramsStream.ToArray(), 0, paramsLength);
          }
          int bitStringLength = (int)bitStringStream.Length;
          this.WriteLength(innerWriter, bitStringLength);
          innerWriter.Write(bitStringStream.ToArray(), 0, bitStringLength);
        }
        int length = (int)innerStream.Length;
        this.WriteLength(writer, length);
        writer.Write(innerStream.ToArray(), 0, length);
      }
      return this.ConvertToPEM("PUBLIC KEY", stream.ToArray());
    }

    /// <summary>
    /// Converts a byte array to PEM compatible string format.
    /// </summary>
    /// <param name="aKeyName">name of key</param>
    /// <param name="aData">data to convert to base64</param>
    /// <returns>PEM string</returns>
    private string ConvertToPEM(string aKeyName, byte[] aData)
    {
      using StringWriter textWriter = new StringWriter();
      char[] base64 = Convert
        .ToBase64String(aData, 0, aData.Length)
        .ToCharArray();
      textWriter.WriteLine($"-----BEGIN {aKeyName}-----");
      // write lines in 64 char chunks
      for (int index = 0; index < base64.Length; index += 64)
      {
        textWriter.WriteLine(
          base64,
          index,
          Math.Min(64, base64.Length - index)
        );
      }
      textWriter.WriteLine($"-----END {aKeyName}-----");
      return textWriter.ToString();
    }

    /*
    /// <summary>
    /// Exports all of the RSA parameters as PEM format.
    /// </summary>
    /// <remarks>
    /// Based on code from:
    /// http://stackoverflow.com/a/23739932/968451
    /// http://stackoverflow.com/a/28407693/968451
    /// </remarks>
    /// <param name="parameters"></param>
    /// <returns>PEM string</returns>
    private string ExportPrivateKey(RSAParameters parameters) {
      using (MemoryStream stream = new MemoryStream()) {
        BinaryWriter writer = new BinaryWriter(stream);
        // SEQUENCE
        writer.Write((byte)0x30);
        using (MemoryStream innerStream = new MemoryStream()) {
          BinaryWriter innerWriter = new BinaryWriter(innerStream);
          // Version
          this.WriteIntegerBigEndian(innerWriter, new byte[] { 0x00 });
          this.WriteIntegerBigEndian(innerWriter, parameters.Modulus);
          this.WriteIntegerBigEndian(innerWriter, parameters.Exponent);
          this.WriteIntegerBigEndian(innerWriter, parameters.D);
          this.WriteIntegerBigEndian(innerWriter, parameters.P);
          this.WriteIntegerBigEndian(innerWriter, parameters.Q);
          this.WriteIntegerBigEndian(innerWriter, parameters.DP);
          this.WriteIntegerBigEndian(innerWriter, parameters.DQ);
          this.WriteIntegerBigEndian(innerWriter, parameters.InverseQ);
          int length = (int)innerStream.Length;
          this.WriteLength(writer, length);
          writer.Write(innerStream.ToArray(), 0, length);
        }
        return this.ConvertToPEM("RSA PRIVATE KEY", stream.ToArray());
      }
    }
    */

    /// <summary>
    /// Write length to stream.
    /// </summary>
    /// <param name="aStream"></param>
    /// <param name="aLength"></param>
    private void WriteLength(BinaryWriter aStream, int aLength)
    {
      if (aLength < 0)
      {
        throw new ArgumentOutOfRangeException(
          nameof(aLength),
          "Length must be non-negative"
        );
      }
      if (aLength < 0x80)
      {
        // Short form
        aStream.Write((byte)aLength);
      }
      else
      {
        // Long form
        int temp = aLength;
        int bytesRequired = 0;
        while (temp > 0)
        {
          temp >>= 8;
          bytesRequired++;
        }
        aStream.Write((byte)(bytesRequired | 0x80));
        for (int index = bytesRequired - 1; index >= 0; index--)
        {
          aStream.Write((byte)((aLength >> (8 * index)) & 0xff));
        }
      }
    }

    /// <summary>
    /// Writes interger as big endian
    /// </summary>
    /// <param name="aStream"></param>
    /// <param name="aValue"></param>
    /// <param name="aForceUnsigned"></param>
    private void WriteIntegerBigEndian(
      BinaryWriter aStream,
      IReadOnlyList<byte> aValue,
      bool aForceUnsigned = true
    )
    {
      aStream.Write((byte)0x02); // INTEGER
      int prefixZeros = aValue.TakeWhile(t => t == 0).Count();
      if (aValue.Count - prefixZeros == 0)
      {
        this.WriteLength(aStream, 1);
        aStream.Write((byte)0);
      }
      else
      {
        if (aForceUnsigned && aValue[prefixZeros] > 0x7f)
        {
          // Add a prefix zero to force unsigned if the MSB is 1
          this.WriteLength(aStream, aValue.Count - prefixZeros + 1);
          aStream.Write((byte)0);
        }
        else
        {
          this.WriteLength(aStream, aValue.Count - prefixZeros);
        }
        for (int index = prefixZeros; index < aValue.Count; index++)
        {
          aStream.Write(aValue[index]);
        }
      }
    }

    #endregion
  }
}