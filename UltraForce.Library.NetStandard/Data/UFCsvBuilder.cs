// <copyright file="UFCsvBuilder.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Data
{
  /// <summary>
  /// A simple class to help create a CSV formatted structure.
  /// </summary>
  public class UFCsvBuilder
  {
    #region private variables

    /// <summary>
    /// The CSV result
    /// </summary>
    private string m_result;

    /// <summary>
    /// Current row in the CSV data
    /// </summary>
    private string m_row;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFCsvBuilder"/>.
    /// </summary>
    /// <param name="aColumnSeparator">
    /// Column separator to use (default = ',')
    /// </param>
    /// <param name="aRowSeparator">
    /// Row separator to use (default = '\r\n')
    /// </param>
    public UFCsvBuilder(
      string aColumnSeparator = ",",
      string aRowSeparator = "\r\n"
    )
    {
      this.ColumnSeparator = aColumnSeparator;
      this.RowSeparator = aRowSeparator;
      this.m_row = "";
      this.m_result = "";
    }

    #endregion

    #region public methods

    /// <summary>
    /// Adds a list of strings as first row
    /// </summary>
    /// <param name="aList"></param>
    public void AddHeaders(params string[] aList)
    {
      foreach (string title in aList)
      {
        this.Add(title);
      }
      this.NewRow();
    }

    /// <summary>
    /// Adds one or more values.
    /// </summary>
    /// <param name="aValue"></param>
    public void AddValue(params object[] aValue)
    {
      foreach (object value in aValue)
      {
        this.Add(value.ToString());
      }
    }

    /// <summary>
    /// Starts a new row.
    /// </summary>
    public void NewRow()
    {
      if (this.m_row.Length > 0)
      {
        if (this.m_result.Length > 0)
        {
          this.m_result += this.RowSeparator;
        }
        this.m_result += this.m_row;
      }
      this.m_row = "";
    }

    /// <summary>
    /// Finalize the csv string and return it. 
    /// </summary>
    /// <returns>CSV formatted data</returns>
    public string Build()
    {
      this.NewRow();
      return this.m_result + this.RowSeparator;
    }

    #endregion

    #region public properties

    /// <summary>
    /// Separator to use for the columns
    /// </summary>
    public string ColumnSeparator { get; set; }

    /// <summary>
    /// Separator to use for the rows
    /// </summary>
    public string RowSeparator { get; set; }

    #endregion

    #region private methods

    /// <summary>
    /// Adds a value to the row, prefix it with the column separator if there
    /// is already data.
    /// </summary>
    /// <param name="aValue"></param>
    private void Add(string aValue)
    {
      if (this.m_row.Length > 0)
      {
        this.m_row += this.ColumnSeparator;
      }
      this.m_row += "\"" + aValue.Replace("\"", "\"\"") + "\"";
    }

    #endregion
  }
}