using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Annotations;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFAttributeToolsTests {
    public class Foo {
      public void NotIgnoredMethod() {
      }

      [UFIgnore]
      public void IgnoredMethod() {
      }

      public string NotIgnoredProperty { get; set; }

      [UFIgnore]
      public string IgnoredProperty { get; set; }

      public string NotIgnoredField;

      [UFIgnore] public string IgnoredField;
    }

    [UFIgnore]
    public class IgnoredFoo {
    }

    [TestMethod]
    public void Find_IgnoredMethod() {
      Assert.AreNotEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo).GetMethod("IgnoredMethod")));
    }

    [TestMethod]
    public void Find_NotIgnoredMethod() {
      Assert.AreEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo).GetMethod("NotIgnoredMethod")));
    }

    [TestMethod]
    public void Find_IgnoredProperty() {
      Assert.AreNotEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo).GetProperty("IgnoredProperty")));
    }

    [TestMethod]
    public void Find_NotIgnoredProperty() {
      Assert.AreEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo).GetProperty("NotIgnoredProperty")));
    }

    [TestMethod]
    public void Find_IgnoredField() {
      Assert.AreNotEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo).GetField("IgnoredField")));
    }

    [TestMethod]
    public void Find_NotIgnoredField() {
      Assert.AreEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo).GetField("NotIgnoredField")));
    }

    [TestMethod]
    public void Find_IgnoredObject() {
      Assert.AreNotEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(IgnoredFoo)));
    }

    [TestMethod]
    public void Find_NotIgnoredObject() {
      Assert.AreEqual(null, UFAttributeTools.Find<UFIgnoreAttribute>(typeof(Foo)));
    }
  }
}