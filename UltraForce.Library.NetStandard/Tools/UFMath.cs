// <copyright file="UFMath.cs" company="Ultra Force Development">
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

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// Additional Math support methods.
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static class UFMath
  {
    /// <summary>
    /// Gets the maximum item of a list of items
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="anItems">one or more item values</param>
    /// <returns>item with the highest value</returns>
    public static T Max<T>(params T[] anItems) where T : IComparable
    {
      if (anItems.Length <= 0)
      {
        throw new ArgumentException("Need at least one argument");
      }
      T item = anItems[0];
      for (int index = 1; index < anItems.Length; index++)
      {
        if (item.CompareTo(anItems[index]) < 0)
        {
          item = anItems[index];
        }
      }
      return item;
    }

    /// <summary>
    /// Gets the minimum item of a list of items
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    /// <param name="anItems">one or more item values</param>
    /// <returns>item with the lowest value</returns>
    public static T Min<T>(params T[] anItems) where T : IComparable
    {
      if (anItems.Length <= 0)
      {
        throw new ArgumentException("Need at least one argument");
      }
      T item = anItems[0];
      for (int index = 1; index < anItems.Length; index++)
      {
        if (item.CompareTo(anItems[index]) > 0)
        {
          item = anItems[index];
        }
      }
      return item;
    }

    /// <summary>
    /// Moves a value to a target with certain distance with a certain step
    /// size.
    /// <para>
    /// If the new value surpasses the target, the target is returned.
    /// </para>
    /// </summary>
    /// <param name="aCurrent">Current value to update</param>
    /// <param name="aTarget">Target value to reach</param>
    /// <param name="aStep">Size of step</param>
    /// <returns>A value closer or equal to aTarget</returns>
    public static int MoveTo(int aCurrent, int aTarget, int aStep)
    {
      return aCurrent > aTarget
        ? Math.Max(aTarget, aCurrent - aStep)
        : Math.Min(aTarget, aCurrent + aStep);
    }

    /// <summary>
    /// Moves a value to a target with certain distance with a certain step
    /// size.
    /// <para>
    /// If the new value surpasses the target, the target is returned.
    /// </para>
    /// </summary>
    /// <param name="aCurrent">Current value to update</param>
    /// <param name="aTarget">Target value to reach</param>
    /// <param name="aStep">Size of step</param>
    /// <returns>A value closer or equal to aTarget</returns>
    public static float MoveTo(float aCurrent, float aTarget, float aStep)
    {
      return aCurrent > aTarget
        ? Math.Max(aTarget, aCurrent - aStep)
        : Math.Min(aTarget, aCurrent + aStep);
    }

    /// <summary>
    /// Moves a value to a target with certain distance with a certain step
    /// size.
    /// <para>
    /// If the new value surpasses the target, the target is returned.
    /// </para>
    /// </summary>
    /// <param name="aCurrent">Current value to update</param>
    /// <param name="aTarget">Target value to reach</param>
    /// <param name="aStep">Size of step</param>
    /// <returns>A value closer or equal to aTarget</returns>
    public static double MoveTo(double aCurrent, double aTarget, double aStep)
    {
      return aCurrent > aTarget
        ? Math.Max(aTarget, aCurrent - aStep)
        : Math.Min(aTarget, aCurrent + aStep);
    }

    /// <summary>
    /// Normalizes an angle in degrees so that it always is a value between
    /// 0 and 360.
    /// </summary>
    /// <param name="anAngle">Angle to normalize</param>
    /// <returns>Normalized angle</returns>
    public static float NormalizeAngle(float anAngle)
    {
      float result = anAngle % 360f;
      return result < 0f ? result + 360f : result;
    }

    /// <summary>
    /// Normalizes an angle in degrees so that it always is a value between -180f and 180f
    /// </summary>
    /// <param name="anAngle">Angle to normalize</param>
    /// <returns>Normalized angle</returns>
    public static float NormalizeAngleAroundZero(float anAngle)
    {
      float result = NormalizeAngle(anAngle);
      return result > 180f ? result - 360f : result;
    }

    /// <summary>
    /// Move towards an angle over a circle using the shortest distance to
    /// move over. 
    /// </summary>
    /// <param name="aCurrent">Current angle to update</param>
    /// <param name="aTarget">Target angle to reach</param>
    /// <param name="aStep">Distance to move with</param>
    /// <returns>Normalized angle closer or equal to aTarget.</returns>
    public static float MoveToAngle(
      float aCurrent,
      float aTarget,
      float aStep
    )
    {
      float current = NormalizeAngle(aCurrent);
      float target = NormalizeAngle(aTarget);
      float delta = current - target;
      if (delta > 180f)
      {
        return NormalizeAngle(MoveTo(current, target + 360f, aStep));
      }
      if (delta < -180f)
      {
        return NormalizeAngle(MoveTo(current, target - 360f, aStep));
      }
      return MoveTo(current, target, aStep);
    }

    /// <summary>
    /// Calculates the smallest distance to travel on a circle between two
    /// angles. 
    /// </summary>
    /// <param name="aStartAngle">Starting angle</param>
    /// <param name="anEndAngle">Ending angle</param>
    /// <returns>
    /// Nearest distance, when traveling clockwise it will be positive. When
    /// traveling counter clock wise the value will be negative.
    /// </returns>
    /// 
    public static float DistanceOnCircle(float aStartAngle, float anEndAngle)
    {
      float startAngle = NormalizeAngle(aStartAngle);
      float endAngle = NormalizeAngle(anEndAngle);
      float distance = endAngle - startAngle;
      if (distance < -180f)
      {
        return 360f + distance;
      }
      if (distance > 180f)
      {
        return distance - 360f;
      }
      return distance;
    }

    /// <summary>
    /// Clamps an angle within a min and max angle range. The angles are
    /// normalized via <see cref="NormalizeAngle"/> .
    /// </summary>
    /// <param name="aValue">Value to limit</param>
    /// <param name="aMinAngle">Minimum angle allowed</param>
    /// <param name="aMaxAngle">Maximum angle allowed</param>
    /// <returns>angle within within min and max range</returns>
    public static float ClampAngle(
      float aValue,
      float aMinAngle,
      float aMaxAngle
    )
    {
      float current = NormalizeAngle(aValue);
      float min = NormalizeAngle(aMinAngle);
      float max = NormalizeAngle(aMaxAngle);
      // normal min and max values (no wrap around at 360) 
      if (min < max)
      {
        // return current if it is within the range
        if ((current >= min) && (current <= max))
        {
          return current;
        }
      }
      else
      {
        // return current if it is within the range (min..360 or 0..max)
        if ((current >= min) || (current <= max))
        {
          return current;
        }
      }
      // return min or max whichever one is more near
      return DistanceOnCircle(current, min) < DistanceOnCircle(current, max)
        ? min
        : max;
    }

    /// <summary>
    /// Checks if two double are equal when their difference is less then a certain value.
    /// </summary>
    /// <param name="aValue0">First value to compare</param>
    /// <param name="aValue1">Second value to compare</param>
    /// <param name="aMaxDelta">Max difference between values to be considered equal</param>
    /// <returns>
    /// True if the difference between the two values is equal or less then specified maximum difference
    /// </returns>
    public static bool Equal(double aValue0, double aValue1, double aMaxDelta = 0.001)
    {
      return Math.Abs(aValue0 - aValue1) <= aMaxDelta;
    }

    /// <summary>
    /// Checks if two double are equal when their difference is less then a certain value.
    /// </summary>
    /// <param name="aValue0">First value to compare</param>
    /// <param name="aValue1">Second value to compare</param>
    /// <param name="aMaxDelta">Max difference between values to be considered equal</param>
    /// <returns>
    /// True if the difference between the two values is equal or less then specified maximum difference
    /// </returns>
    public static bool Equal(float aValue0, float aValue1, float aMaxDelta = 0.001f)
    {
      return Math.Abs(aValue0 - aValue1) <= aMaxDelta;
    }

    /// <summary>
    /// Gets the percentage value from a value that contains a % (either %xxx.xx or xxx.xx%)
    /// </summary>
    /// <param name="aValue">Value to get percentage from</param>
    /// <returns>Percentage or 0.0f if no '%' was found or value without '%' is an invalid number</returns>
    public static float GetPercentage(string aValue)
    {
      if (aValue.IndexOf('%') >= 0)
      {
        if (float.TryParse(aValue.Replace("%", ""), out float value))
        {
          return value;
        }
      }
      return 0f;
    }
  }
}