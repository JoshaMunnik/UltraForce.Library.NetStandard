using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFJavaScriptToolsTests {
    [TestMethod]
    public void GetTime_Start() {
      DateTime test = new DateTime(
        1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc
        );
      Assert.AreEqual(0, UFJavaScriptTools.GetTime(test));
    }

    [TestMethod]
    public void GetTime_OneYear() {
      DateTime test = new DateTime(
        1971, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc
        );
      Assert.AreEqual(1000L * 60 * 60 * 24 * 365, UFJavaScriptTools.GetTime(test));
    }

    [TestMethod]
    public void GetString_Normal() {
      Assert.AreEqual("'a\\'\\t\\n'", UFJavaScriptTools.GetString("a'\t\n\r"));
    }

    public void GetString_Null() {
      Assert.AreEqual("null", UFJavaScriptTools.GetString(aText: null));
    }

    [TestMethod]
    public void GetString_List() {
      string[] list = {"1", "2", "3"};
      Assert.AreEqual("'1, 2, 3'", UFJavaScriptTools.GetString(list));
    }

    [TestMethod]
    public void GetStringTest2() {
      string[] list = { "1", "2", "3" };
      Assert.AreEqual("'1+2+3'", UFJavaScriptTools.GetString("+", list));
    }
  }
}