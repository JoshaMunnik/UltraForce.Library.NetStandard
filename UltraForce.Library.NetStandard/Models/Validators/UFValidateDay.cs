// <copyright file="UFValidateDay.cs" company="Ultra Force Development">
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

using System.Text.RegularExpressions;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Models.Validators
{
  /// <summary>
  /// Validate a day by using year and month values.
  /// </summary>
  public class UFValidateDay : IUFValidateValue
  {
    #region private vars

    /// <summary>
    /// Name of month field
    /// </summary>
    private readonly string m_month;

    /// <summary>
    /// Name of year field
    /// </summary>
    private readonly string m_year;

    /// <summary>
    /// Data instance
    /// </summary>
    private readonly IUFAccessProperty m_data;

    /// <summary>
    /// Maximum days per month (first value is 0, since months start at 1)
    /// </summary>
    private static readonly int[] s_maxDays = {
      0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31
    };

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UFValidateDay"/> class.
    /// </summary>
    /// <param name='aMonthFieldName'>
    /// The name of month property in data.
    /// </param>
    /// <param name='aYearFieldName'>
    /// The name of year property in data.
    /// </param>
    /// <param name='aData'>
    /// A data to obtain month and year from.
    /// </param>
    public UFValidateDay(
      string aMonthFieldName,
      string aYearFieldName,
      IUFAccessProperty aData
    )
    {
      this.m_month = aMonthFieldName;
      this.m_year = aYearFieldName;
      this.m_data = aData;
    }

    #endregion

    #region iufvalidatevalue

    /// <summary>
    /// Validate a day value. Use year and month values to determine maximum 
    /// day.If the month and/or year are invalid(not an integer
    /// or in the case of month outside the valid range), the method will
    /// only validate day at the maximum range.
    /// </summary>
    /// <param name="aValue">value to compare</param>
    /// <returns>
    /// <c>True</c> if day is valid day value or if month or year values are 
    /// invalid themselves.
    /// </returns>
    public bool IsValid(object? aValue)
    {
      if (aValue == null)
      {
        return false;
      }
      // regular expression to validate number
      Regex intNumber = new Regex(@"^\d+$");
      if (!intNumber.IsMatch(aValue.ToString()))
      {
        return false;
      }
      int day = (int)aValue;
      // day is outside max possible range? Y: exit, invalid value in all cases
      if ((day < 1) || (day > 31))
      {
        return false;
      }
      object? yearValue = this.m_data.GetPropertyValue(this.m_year);
      object? monthValue = this.m_data.GetPropertyValue(this.m_month);
      if ((yearValue == null)
          || (monthValue == null)
          || !intNumber.IsMatch(monthValue.ToString())
          || !intNumber.IsMatch(yearValue.ToString()))
      {
        return true;
      }
      int year = (int)yearValue;
      int month = (int)monthValue;
      // valid month
      if (month is < 1 or >= 13)
      {
        return true;
      }
      // yes, get maximum day for month
      int maxDay = (month == 2) && UFDateTimeTools.IsLeapYear(year) ? 29 : s_maxDays[month];
      // there was already a check on day being 1 or more, so only check if day is small enough
      return day <= maxDay;
    }

    #endregion
  }
}