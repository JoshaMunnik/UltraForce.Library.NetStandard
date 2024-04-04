using System;
using System.Reflection;

namespace UltraForce.Library.NetStandard.Tools
{
  public static class UFEnumTools
  {
    /// <summary>
    /// Gets a random value from an enum.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Random<T>() where T : Enum
    {
      Array values = Enum.GetValues(typeof(T));
      return (T)values.GetValue(UFRandomTools.Next(values.Length));
    }

    /// <summary>
    /// Gets the enum value for a given integer.
    /// </summary>
    /// <param name="aValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindValue<T>(int aValue) where T : Enum
    {
      return (T) FindValue(typeof(T), aValue);
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