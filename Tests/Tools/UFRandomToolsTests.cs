using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFRandomToolsTests {
    [TestMethod]
    public void NextTest() {
      int previous = UFRandomTools.Next();
      for (int index = 0; index < 100; index++) {
        int current = UFRandomTools.Next();
        Assert.AreNotEqual(previous, current);
        previous = current;
      }
    }

    [TestMethod]
    public void NextBytesTest() {
      byte[] test = new byte[100];
      int[] count = new int[256];
      UFRandomTools.NextBytes(test);
      for (int index = 0; index < test.Length - 1; index++) {
        count[test[index]]++;
      }
      for (int index = 0; index < 256; index++) {
        Assert.AreNotEqual(count[index], test.Length);
      }
    }

    [TestMethod]
    public void NextFloatTest() {
      float previous = UFRandomTools.NextFloat();
      for (int index = 0; index < 100; index++) {
        float current = UFRandomTools.NextFloat();
        Assert.AreNotEqual(previous, current);
        previous = current;
      }
    }

    [TestMethod]
    public void NextDoubleTest() {
      double previous = UFRandomTools.NextDouble();
      for (double index = 0; index < 100; index++) {
        double current = UFRandomTools.NextDouble();
        Assert.AreNotEqual(previous, current);
        previous = current;
      }
    }

    [TestMethod]
    public void RangeTest() {
      for (int index = 0; index < 1000; index++) {
        int current = UFRandomTools.Range(100, 200);
        Assert.IsTrue((current >= 100) && (current < 200));
      }
    }
  }
}