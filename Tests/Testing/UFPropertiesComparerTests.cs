using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Testing;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Testing;

[TestClass]
public class UFPropertiesComparerTests
{
  public class SimpleDataClass
  {
    public SimpleDataClass()
    {
      this.Text = UFStringTools.GenerateTextCode(20);
      this.Number = UFRandomTools.Next(100);
    }

    public SimpleDataClass(
      SimpleDataClass aSource
    )
    {
      this.Text = aSource.Text;
      this.Number = aSource.Number;
    }

    public string Text { get; set; }
    public int Number { get; set; }
  }

  public class DataClassWithIgnoreProperty
  {
    public DataClassWithIgnoreProperty()
    {
      this.Text = UFStringTools.GenerateTextCode(20);
      this.Number = UFRandomTools.Next(100);
      this.Ignore = UFStringTools.GenerateTextCode(18);
    }

    public DataClassWithIgnoreProperty(
      DataClassWithIgnoreProperty aSource
    )
    {
      this.Text = aSource.Text;
      this.Number = aSource.Number;
      this.Ignore = UFStringTools.GenerateTextCode(18);
    }

    public string Text { get; set; }
    public int Number { get; set; }

    [UFCompareIgnore]
    public string Ignore { get; set; }
  }

  public class DataClassWithObjectProperty
  {
    public DataClassWithObjectProperty()
    {
      this.Text = UFStringTools.GenerateTextCode(20);
      this.Number = UFRandomTools.Next(100);
      this.Data = new SimpleDataClass();
    }

    public DataClassWithObjectProperty(
      DataClassWithObjectProperty aSource
    )
    {
      this.Text = aSource.Text;
      this.Number = aSource.Number;
      this.Data = new SimpleDataClass(aSource.Data);
    }

    public string Text { get; set; }
    public int Number { get; set; }

    [UFCompareProperties]
    public SimpleDataClass Data { get; set; }
  }

  [TestMethod]
  public void Compare_simple_data()
  {
    SimpleDataClass data1 = new();
    SimpleDataClass data2 = new(data1);
    UFPropertiesComparer<SimpleDataClass> comparer = new();
    Assert.IsTrue(comparer.Equals(data1, data2));
  }
  
  [TestMethod]
  public void Compare_data_with_ignoreProperty()
  {
    DataClassWithIgnoreProperty data1 = new();
    DataClassWithIgnoreProperty data2 = new(data1);
    UFPropertiesComparer<DataClassWithIgnoreProperty> comparer = new();
    Assert.IsTrue(comparer.Equals(data1, data2));
  }

  [TestMethod]
  public void Compare_data_with_compareProperties()
  {
    DataClassWithObjectProperty data1 = new();
    DataClassWithObjectProperty data2 = new(data1);
    UFPropertiesComparer<DataClassWithObjectProperty> comparer = new();
    Assert.IsTrue(comparer.Equals(data1, data2));
  }
}