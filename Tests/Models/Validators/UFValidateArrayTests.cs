using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators {
  [TestClass]
  public class UFValidateArrayTests {
    [TestMethod]
    public void UFValidateArrayTest() {
      IUFValidateValue validator = new UFValidateArray(0, 1);
    }

    [TestMethod]
    public void IsValidTest_InRange() {
      IUFValidateValue validator = new UFValidateArray(2, 4);
      Assert.IsTrue(validator.IsValid(new byte[] { 0, 1}));
      Assert.IsTrue(validator.IsValid(new byte[] { 0, 1, 2 }));
      Assert.IsTrue(validator.IsValid(new byte[] {0, 1, 2, 3}));
    }

    [TestMethod]
    public void IsValidTest_ToSmall() {
      IUFValidateValue validator = new UFValidateArray(2, 4);
      Assert.IsFalse(validator.IsValid(new byte[] { 0 }));
    }

    [TestMethod]
    public void IsValidTest_ToBig() {
      IUFValidateValue validator = new UFValidateArray(2, 4);
      Assert.IsFalse(validator.IsValid(new byte[] { 0, 1, 2, 3, 4 }));
    }
  }
}