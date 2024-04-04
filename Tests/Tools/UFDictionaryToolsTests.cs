using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFDictionaryToolsTests {
    private readonly Dictionary<string, string> m_test = new Dictionary<string, string>() {
        { "key1", "value1" },
        { "key2", "true" },
        { "key3", "10" },
        { "key4", "1.1234" },
        { "key5", "1.1234567890123"}
      };

    [TestMethod]
    public void GetValue_Existing() {
      Assert.AreEqual("value1", UFDictionaryTools.GetValue(this.m_test, "key1", "apple"));
    }

    [TestMethod]
    public void GetValue_Default() {
      Assert.AreEqual("apple", UFDictionaryTools.GetValue(this.m_test, "key6", "apple"));
    }

    [TestMethod]
    public void GetValueAsString_Existing() {
      Assert.AreEqual("value1", UFDictionaryTools.GetValueAsString(this.m_test, "key1", "apple"));
    }

    public void GetValueAsString_Default() {
      Assert.AreEqual("apple", UFDictionaryTools.GetValueAsString(this.m_test, "key6", "apple"));
    }

    [TestMethod]
    public void GetValueAsBoolean_Existing() {
      Assert.AreEqual(true, UFDictionaryTools.GetValueAsBoolean(this.m_test, "key2", false));
    }

    public void GetValueAsBoolean_Default() {
      Assert.AreEqual(false, UFDictionaryTools.GetValueAsBoolean(this.m_test, "key6", false));
    }

    [TestMethod]
    public void GetValueAsInt_Existing() {
      Assert.AreEqual(10, UFDictionaryTools.GetValueAsInt(this.m_test, "key3", 1234));
    }

    public void GetValueAsInt_Default() {
      Assert.AreEqual(1234, UFDictionaryTools.GetValueAsInt(this.m_test, "key6", 1234));
    }

    [TestMethod]
    public void GetValueAsFloat_Existing() {
      Assert.AreEqual(1.1234f, UFDictionaryTools.GetValueAsFloat(this.m_test, "key4", 0.1234f));
    }

    public void GetValueAsFloat_Default() {
      Assert.AreEqual(0.1234f, UFDictionaryTools.GetValueAsFloat(this.m_test, "key6", 0.1234f));
    }

    [TestMethod]
    public void GetValueAsDouble_Existing() {
      Assert.AreEqual(1.1234567890123, UFDictionaryTools.GetValueAsDouble(this.m_test, "key5", 0.123456789));
    }

    public void GetValueAsDouble_Default() {
      Assert.AreEqual(0.123456789, UFDictionaryTools.GetValueAsDouble(this.m_test, "key6", 0.123456789));
    }
  }
}