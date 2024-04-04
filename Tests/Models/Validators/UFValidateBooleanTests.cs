using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators
{
  [TestClass]
    public class UFValidateBooleanTests
    {
      [TestMethod]
      public void IsValidTest_TrueAndTrue() {
        IUFValidateValue validator = new UFValidateBoolean(true);
        Assert.IsTrue(validator.IsValid(true), "True is not true");
      }

      [TestMethod]
      public void IsValidTest_FalseAndFalse() {
        IUFValidateValue validator = new UFValidateBoolean(false);
        Assert.IsTrue(validator.IsValid(false), "False is not false");
      }

      [TestMethod]
      public void IsInvalidTest_TrueAndFalse() {
        IUFValidateValue validator = new UFValidateBoolean(true);
        Assert.IsFalse(validator.IsValid(false), "True is false");
      }

      [TestMethod]
      public void IsInvalidTest_FalseAndTrue() {
        IUFValidateValue validator = new UFValidateBoolean(true);
        Assert.IsFalse(validator.IsValid(false), "False is true");
      }
    }
}
