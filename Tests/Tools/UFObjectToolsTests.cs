using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFObjectToolsTests {
    [TestMethod]
    public void SelectValue_Normal() {
      int? test = 1234;
      Assert.AreEqual(1234, UFObjectTools.SelectValue(test, 5678));
    }

    [TestMethod]
    public void SelectValue_Null() {
      int? test = null;
      Assert.AreEqual(5678, UFObjectTools.SelectValue(test, 5678));
    }

    [TestMethod]
    public void SelectValue_Normal_CheckType() {
      int? test = 1234;
      Assert.AreEqual(typeof(int), UFObjectTools.SelectValue(test, 5678).GetType());
    }

    [TestMethod]
    public void SelectValue_Null_CheckType() {
      int? test = null;
      Assert.AreEqual(typeof(int), UFObjectTools.SelectValue(test, 5678).GetType());
    }

  }
}