// <copyright file="UFCsvHelper.cs" company="Ultra Force Development">
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

using System.Collections.Generic;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Data
{
  /// <summary>
  /// Parse a CSV formatted text. The code does not trim any text.
  /// </summary>
  public class UFCsvHelper
  {
    #region private variables

    /// <summary>
    /// Contains the records, an array of arrays.
    /// </summary>
    private List<List<string>>? m_records;

    #endregion

    #region public methods

    /// <summary>
    /// Initializes a new instance of the <see cref="UFCsvHelper"/> class.
    /// </summary>
    public UFCsvHelper()
    {
      // clear data
      this.Clear();
    }

    /// <summary>
    /// Parses a CSV text. After parsing <see cref="GetRecord"/> can be called
    /// and <see cref="Header"/> contains the header if 
    /// <see cref="HasHeader"/> is <c>true</c>.
    /// </summary>
    /// <param name='aText'>
    /// A text.
    /// </param>
    public void Parse(string aText)
    {
      // filter out \r
      aText = aText.Replace("\r", "");
      // get list of records as text 
      List<string> records = this.Split(aText, this.RecordSeparator);
      // remove empty lines
      for (int index = records.Count - 1; index >= 0; index--)
        if (records[index].Length == 0)
        {
          records.RemoveAt(index);
        }
      // convert text line to fields
      this.m_records = new List<List<string>>(records.Count);
      foreach (string record in records)
      {
        this.m_records.Add(this.ParseRecordText(record));
      }
      // records contain header? Y: store it separately, N: reset header data
      if (this.HasHeader)
      {
        this.Header = this.m_records[0];
        this.m_records.RemoveAt(0);
      }
      else
      {
        this.Header = new List<string>();
      }
    }

    /// <summary>
    /// Removes existing data. After this call, use <see cref="Parse"/> to
    /// fill the instance with new data.
    /// </summary>
    public void Clear()
    {
      this.Header = new List<string>();
      this.m_records = new List<List<string>>();
    }

    /// <summary>
    /// Gets a record (row).
    /// </summary>
    /// <param name="anIndex">
    /// record index (first record has index of 0)
    /// </param>
    /// <returns>
    /// The record (a list of strings)
    /// </returns>
    public List<string> GetRecord(int anIndex)
    {
      return this.m_records![anIndex];
    }

    #endregion

    #region public properties

    /// <summary>
    /// Gets a record for a certain index using array index.
    /// </summary>
    /// <param name='anIndex'>
    /// An index.
    /// </param>
    public List<string> this[int anIndex] => this.m_records![anIndex];

    /// <summary>
    /// Record separator text.
    /// </summary>
    /// <value>
    /// The record separator, default is "\n"
    /// </value>
    public char RecordSeparator { get; set; } = '\n';

    /// <summary>
    /// Field separator text.
    /// </summary>
    /// <value>
    /// The field separator, default ","
    /// </value>
    public char FieldSeparator { get; set; } = ',';

    /// <summary>
    /// Field enclose text.
    /// </summary>
    /// <value>
    /// The field enclose value, default " (double quote)
    /// </value>
    public char FieldEnclose { get; set; } = '"';

    /// <summary>
    /// Number of records.
    /// </summary>
    public int Count => this.m_records!.Count;

    /// <summary>
    /// Returns the headers.
    /// </summary>
    public List<string>? Header { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the first row of the CSV data 
    /// is a header.
    /// <para>
    /// This property is only used within the <see cref="Parse" />; assigning
    /// a value after will not change the other properties.
    /// </para>
    /// </summary>
    /// <value>
    /// <c>true</c> (default) if the first row is a header; 
    /// otherwise, <c>false</c>
    /// </value>
    public bool HasHeader { get; set; }

    #endregion

    #region private methods

    /// <summary>
    /// Break a text into separate elements using a separator. Ignore separators which are inside text enclosed by
    /// fieldEnclose tokens.
    /// <para>
    /// Algorithm based on decode method from <a href="http://code.google.com/p/csvlib/">csvlib</a>
    /// </para>
    /// </summary>
    /// <returns>
    /// list of strings
    /// </returns>
    /// <param name='aText'>
    /// text to split
    /// </param>
    /// <param name='aSeparator'>
    /// separator to split with
    /// </param>
    private List<string> Split(string aText, char aSeparator)
    {
      // result contains the resulting array
      List<string> result = new List<string>();
      // encloseCount keeps track of the number of field enclose texts encountered so far
      int encloseCount = 0;
      // split text using the separator, note that this also will break strings 
      // which have the separator enclosed by fieldEnclose text. 
      string[] list = aText.Split(aSeparator);
      // process each part 
      foreach (string text in list)
      {
        // encloseCount is even?
        if ((encloseCount & 1) == 0)
        {
          // yes, every starting fieldEnclose is matched by an closing 
          // fieldEnclose; so text is (the start of) a new item
          result.Add(text);
        }
        else
        {
          // no, missing an closing fieldEnclose; so add text to last stored 
          // record preceding it with a record separator which caused
          // it to split in the first place.
          result[result.Count - 1] += aSeparator + text;
        }
        // increase the number of FieldEnclose characters encountered.
        encloseCount += UFStringTools.Count(this.FieldEnclose, text);
      }
      return result;
    }

    /// <summary>
    /// Parse a single record text line and get fields and put them back into 
    /// the same array.
    /// </summary> 
    /// <param name="aText">Text to parse</param>
    /// <returns>List of fields</returns>
    private List<string> ParseRecordText(string aText)
    {
      // result contains every field
      List<string> result = this.Split(aText, this.FieldSeparator);
      for (int index = result.Count - 1; index >= 0; index--)
      {
        result[index] = this.CleanValue(result[index]);
      }
      return result;
    }

    /// <summary>
    /// If anItem starts with fieldEnclose, remove it and also at the end.
    /// </summary> 
    /// <param name="aText">Text to clean</param>
    /// <returns>Cleaned text</returns>
    private string CleanValue(string aText)
    {
      string fieldEnclose = this.FieldEnclose.ToString();
      // remove starting and closing fieldEnclose (if any)
      if (aText.StartsWith(fieldEnclose))
      {
        aText = aText.Substring(1, aText.Length - 2);
      }
      // replace double occurrences of field enclose with single one
      aText = aText.Replace(fieldEnclose + fieldEnclose, fieldEnclose);
      // return new text
      return aText;
    }

    #endregion
  }
}