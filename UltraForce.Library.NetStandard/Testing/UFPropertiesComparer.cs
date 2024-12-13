using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Testing
{
  /// <summary>
  /// <see cref="UFPropertiesComparer{T}"/> implements <see cref="IEqualityComparer{T}"/> by
  /// comparing all public properties of a type.
  /// <para>
  /// Based on code from: https://stackoverflow.com/a/49825057/968451
  /// </para>
  /// <para>
  /// Use [UFCustomIgnore] to ignore a property. Use [UFCompareProperties] with object type to
  /// compare the properties of the object and not the object value itself. 
  /// </para>
  /// </summary>
  /// <typeparam name="T">Type to compare properties from</typeparam>
  public class UFPropertiesComparer<T> : IEqualityComparer<T>
  {

  #region private variables

  /// <summary>
  /// When true throw an exception, false just return false
  /// </summary>
  private readonly bool m_throwException;

  /// <summary>
  /// Property names to ignore
  /// </summary>
  private readonly IList<string> m_notEqualProperties;

  #endregion

  #region constructors

  /// <summary>
  /// Constructs an instance of <see cref="UFPropertiesComparer{T}"/>
  /// </summary>
  /// <param name="aThrowException">
  /// When true throw an exception with <see cref="Equals"/> if there is a value mismatch instead of returning false.
  /// </param>
  /// <param name="aNotEqualProperties">
  /// A list of property names that should not be equal when comparing.
  /// </param>
  public UFPropertiesComparer(
    bool aThrowException = false,
    IEnumerable<string>? aNotEqualProperties = null
  )
  {
    this.m_throwException = aThrowException;
    this.m_notEqualProperties = aNotEqualProperties?.ToList() ?? new List<string>();
  }

  #endregion

  #region IEqualityComparer

  /// <summary>
  /// Compare two instances of <typeparamref name="T"/> by comparing all public properties.
  ///
  /// Properties using <see cref="UFCompareIgnoreAttribute"/> are skipped.
  /// </summary>
  /// <param name="anExpected"></param>
  /// <param name="anActual"></param>
  /// <returns>True if all properties are equal</returns>
  /// <exception cref="Exception">Might be thrown depending on the constructor.</exception>
  public bool Equals(
    T anExpected,
    T anActual
  )
  {
    return this.CompareProperties(
      anExpected,
      anActual,
      typeof(T).GetProperties(
        BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance
      )
    );
  }
  
  /// <summary>
  /// Compares the properties of two objects.
  /// </summary>
  /// <param name="anExpected"></param>
  /// <param name="anActual"></param>
  /// <param name="propertyInfos"></param>
  /// <returns></returns>
  /// <exception cref="Exception"></exception>
  private bool CompareProperties(
    object anExpected,
    object anActual,
    PropertyInfo[] propertyInfos
  )
  {
    foreach (PropertyInfo propertyInfo in propertyInfos)
    {
      if (
        this.m_notEqualProperties.Contains(propertyInfo.Name) ||
        (propertyInfo.GetCustomAttribute<UFCompareIgnoreAttribute>() != null)
      )
      {
        continue;
      }
      object expectedValue = propertyInfo.GetValue(anExpected, null);
      object actualValue = propertyInfo.GetValue(anActual, null);
      if ((expectedValue == null) && (actualValue == null))
      {
        continue;
      }
      if ((expectedValue == null) || (actualValue == null))
      {
        if (!this.m_throwException)
        {
          return false;
        }
        throw new Exception(
          $"One of the values of property '{propertyInfo.Name}' is null and the other is not"
        );
      }
      if (propertyInfo.GetCustomAttribute<UFComparePropertiesAttribute>() != null)
      {
        bool compareResult = this.CompareProperties(
          expectedValue,
          actualValue, 
          expectedValue.GetType().GetProperties(
            BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance
          )
        );
        if (!compareResult)
        {
          return false;
        }
        continue;
      }
      if (UFObjectTools.AreEqual(expectedValue, actualValue))
      {
        continue;
      }
      if (!this.m_throwException)
      {
        return false;
      }
      if (this.m_notEqualProperties.Contains(propertyInfo.Name))
      {
        throw new Exception(
          $"A value of '{expectedValue}' for property '{propertyInfo.Name}' should not be equal"
        );
      }
      else
      {
        throw new Exception(
          $"A value of '{expectedValue}' for property '{propertyInfo.Name}' does not match '{actualValue}'"
        );
      }
    }
    return true;
  }

  /// <inheritdoc />
  public int GetHashCode(
    T aParameterValue
  )
  {
    return Tuple.Create(aParameterValue).GetHashCode();
  }

  #endregion

  }
}