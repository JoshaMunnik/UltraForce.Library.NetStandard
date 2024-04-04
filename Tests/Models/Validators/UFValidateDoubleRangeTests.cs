using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators {
  [TestClass]
  public class UFValidateDoubleRangeTests {
    [TestMethod]
    public void WithinDoubleRange() {
      IUFValidateValue validator = new UFValidateDoubleRange(-1000.13, 1000.13);
      for (double value = -1000.13; value <= 1000.13; value += 0.13) {
        Assert.IsTrue(validator.IsValid(value), $"Value {value} is not in range");
      }
      Assert.IsTrue(validator.IsValid(1000.13), $"Value 1000.13 is not in range");
    }

    [TestMethod]
    public void BelowDoubleRange() {
      IUFValidateValue validator = new UFValidateDoubleRange(-1000.13, 1000.13);
      for (double value = -1500.13; value < -1000.13; value += 0.13) {
        Assert.IsFalse(validator.IsValid(value), $"Value {value} is in range");
      }
      Assert.IsFalse(validator.IsValid(-1000.13000001), $"Value -1000.13000001 is in range");
    }

    [TestMethod]
    public void AboveDoubleRange() {
      IUFValidateValue validator = new UFValidateDoubleRange(-1000.13, 1000.13);
      for (double value = 1000.1300001; value < 2000.13; value += 0.13) {
        Assert.IsFalse(validator.IsValid(value), $"Value {value} is in range");
      }
    }
  }
}
