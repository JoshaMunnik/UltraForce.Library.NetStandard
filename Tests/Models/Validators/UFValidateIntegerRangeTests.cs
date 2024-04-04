using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators {
  [TestClass]
  public class UFValidateIntegerRangeTests {
    [TestMethod]
    public void WithinIntegerRange() {
      IUFValidateValue validator = new UFValidateIntegerRange(-1000, 1000);
      for (int value = -1000; value <= 1000; value++) {
        Assert.IsTrue(validator.IsValid(value), $"Value {value} is not in range");
      }
    }

    [TestMethod]
    public void BelowIntegerRange() {
      IUFValidateValue validator = new UFValidateIntegerRange(-1000, 1000);
      for (int value = -1500; value < -1000; value++) {
        Assert.IsFalse(validator.IsValid(value), $"Value {value} is in range");
      }
    }

    [TestMethod]
    public void AboveIntegerRange() {
      IUFValidateValue validator = new UFValidateIntegerRange(-1000, 1000);
      for (int value = 1001; value < 2000; value++) {
        Assert.IsFalse(validator.IsValid(value), $"Value {value} is in range");
      }
    }
  }
}
