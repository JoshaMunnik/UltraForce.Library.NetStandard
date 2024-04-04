using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  /// <summary>
  /// Tests the UFArrayTools methods.
  /// </summary>
  [TestClass]
  public class UFArrayToolsTests {
    [TestMethod]
    public void ShuffleTest() {
      byte[] source = {
        0, 1, 2, 3, 4, 5
      };
      byte[] test = (byte[]) source.Clone();
      UFArrayTools.Shuffle(test);
      Assert.IsFalse(source.SequenceEqual(test));
    }

    [TestMethod]
    public void SwapTest() {
      byte[] source = {
        0, 1, 2, 3, 4, 5
      };
      byte[] test = {
        4, 1, 2, 3, 0, 5
      };
      UFArrayTools.Swap(source, 0, 4);
      Assert.IsTrue(source.SequenceEqual(test));
    }

    [TestMethod]
    public void RandomItem_WholeArray() {
      byte[] source = {
        0, 1, 2, 3, 4, 5
      };
      for (int index = 0; index < 1000; index++) {
        byte item = UFArrayTools.RandomItem(source);
        Assert.IsTrue(item <= 5);
      }
    }

    [TestMethod]
    public void RandomItem_Range() {
      byte[] source = {
        0, 1, 2, 3, 4, 5
      };
      for (int index = 0; index < 1000; index++) {
        byte item = UFArrayTools.RandomItem(source, 2, 2);
        Assert.IsTrue((item >= 2) && (item <= 4));
      }
    }
  }
}