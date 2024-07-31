using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UltraForce.Library.NetStandard.Tools
{
  public static class UFEnumTools
  {
    /// <summary>
    /// Gets a random value from an enum.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static TEnum Random<TEnum>() where TEnum : Enum
    {
      Array values = Enum.GetValues(typeof(TEnum));
      return (TEnum)values.GetValue(UFRandomTools.Next(values.Length));
    }
    
    /// <summary>
    /// Tries to get an enum equivalent for a given integer.
    /// </summary>
    /// <param name="aValue"></param>
    /// <param name="anEnumValue"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns>True if the integer matches an enum; false when not</returns>
    public static bool TryGet<TEnum>(int aValue, out TEnum anEnumValue) where TEnum : Enum
    {
      if (Enum.IsDefined(typeof(TEnum), aValue))
      {
        anEnumValue = (TEnum)Enum.ToObject(typeof(TEnum), aValue);
        return true;
      }
      anEnumValue = default!;
      return false;
    }

    /// <summary>
    /// Gets the enum value for a given integer.
    /// </summary>
    /// <param name="aValue"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static TEnum FindValue<TEnum>(int aValue) where TEnum : Enum
    {
      return (TEnum) FindValue(typeof(TEnum), aValue);
    }

    /// <summary>
    /// Gets all values of an enum as a <see cref="List{T}"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static List<TEnum> AsList<TEnum>() where TEnum : Enum
    {
      return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
    }

    /// <summary>
    /// Gets the enum value for a given integer.
    /// </summary>
    /// <param name="anEnumType"></param>
    /// <param name="aValue"></param>
    /// <returns>Enum equivalent</returns>
    /// <exception cref="ArgumentException"></exception>
    public static object FindValue(Type anEnumType, int aValue)
    {
      foreach (object enumValue in Enum.GetValues(anEnumType))
      {
        string enumValueAsString = enumValue.ToString();
        if (string.IsNullOrEmpty(enumValueAsString))
        {
          continue;
        }
        FieldInfo fieldInfo = anEnumType.GetField(enumValueAsString);
        if (fieldInfo == null)
        {
          continue;
        }
        object fieldValue = fieldInfo.GetValue(enumValue);
        if (fieldValue == null)
        {
          continue;
        }
        if ((int)fieldValue == aValue)
        {
          return enumValue;
        }
      }
      throw new ArgumentException($"No enum value found for {aValue} in {anEnumType.Name}");
    }
  }
}