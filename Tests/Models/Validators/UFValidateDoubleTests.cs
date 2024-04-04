using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators {
  [TestClass]
  public class UFValidateDoubleTests {
    [TestMethod]
    public void TestValidDoubles() {
      IUFValidateValue validator = new UFValidateDouble();
      for (double value = -10000.13; value < 10000.13; value += 4.13) {
        Assert.IsTrue(validator.IsValid(value), $"Invalid double value {value}");
      }
    }

    [TestMethod]
    public void TestValidDoubleStrings() {
      IUFValidateValue validator = new UFValidateDouble();
      for (double value = -10000.13; value < 10000.13; value += 4.13) {
        Assert.IsTrue(validator.IsValid(value.ToString(CultureInfo.InvariantCulture.NumberFormat)), $"Invalid double string value {value}");
      }
    }

    [TestMethod]
    public void TestInvalidDate() {
      IUFValidateValue validator = new UFValidateDouble();
      Assert.IsFalse(validator.IsValid(DateTime.Now), $"Valid date value");
    }

    [TestMethod]
    public void TestInvalidString() {
      IUFValidateValue validator = new UFValidateDouble();
      Assert.IsFalse(validator.IsValid("1000.121a"), $"Valid string value 1000a");
    }
  }
}
