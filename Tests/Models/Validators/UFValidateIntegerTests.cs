using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators {
  [TestClass]
  public class UFValidateIntegerTests {
    [TestMethod]
    public void TestValidIntegers() {
      IUFValidateValue validator = new UFValidateInteger();
      for (int value = -10000; value < 10000; value += 4) {
        Assert.IsTrue(validator.IsValid(value), $"Invalid integer value {value}");
      }
    }

    [TestMethod]
    public void TestValidIntegerStrings() {
      IUFValidateValue validator = new UFValidateInteger();
      for (int value = -10000; value < 10000; value += 4) {
        Assert.IsTrue(validator.IsValid(value.ToString()), $"Invalid integer string value {value}");
      }
    }

    [TestMethod]
    public void TestInvalidDouble() {
      IUFValidateValue validator = new UFValidateInteger();
      Assert.IsFalse(validator.IsValid(1.1), $"Valid double value 1.1");
    }

    [TestMethod]
    public void TestInvalidDate() {
      IUFValidateValue validator = new UFValidateInteger();
      Assert.IsFalse(validator.IsValid(DateTime.Now), $"Valid date value");
    }

    [TestMethod]
    public void TestInvalidString() {
      IUFValidateValue validator = new UFValidateInteger();
      Assert.IsFalse(validator.IsValid("1000a"), $"Valid string value 1000a");
    }
  }
}
