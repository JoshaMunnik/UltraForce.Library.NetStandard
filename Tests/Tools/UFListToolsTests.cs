using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFListToolsTests {
    [TestMethod]
    public void ShuffleTest() {
      List<byte> source = new List<byte>() {
        0, 1, 2, 3, 4, 5
      };
      List<byte> test = (List<byte>) source.GetRange(0, 6);
      UFListTools.Shuffle(test);
      Assert.IsFalse(source.SequenceEqual(test));
    }

    [TestMethod]
    public void SwapTest() {
      List<byte> source = new List<byte>() {
        0, 1, 2, 3, 4, 5
      };
      List<byte> test = new List<byte>() {
        4, 1, 2, 3, 0, 5
      };
      UFListTools.Swap(source, 0, 4);
      Assert.IsTrue(source.SequenceEqual(test));
    }

    [TestMethod]
    public void RandomItem_WholeArray() {
      List<byte> source = new List<byte>() {
        0, 1, 2, 3, 4, 5
      };
      for (int index = 0; index < 1000; index++) {
        byte item = UFListTools.RandomItem(source);
        Assert.IsTrue(item <= 5);
      }
    }

    [TestMethod]
    public void RandomItem_Range() {
      List<byte> source = new List<byte>() {
        0, 1, 2, 3, 4, 5
      };
      for (int index = 0; index < 1000; index++) {
        byte item = UFListTools.RandomItem(source, 2, 2);
        Assert.IsTrue((item >= 2) && (item <= 4));
      }
    }
  }
}