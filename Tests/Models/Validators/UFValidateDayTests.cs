using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Models;
using UltraForce.Library.NetStandard.Models.Validators;

namespace Tests.Models.Validators {
  [TestClass]
  public class UFValidateDayTests {
    private class DateModel : UFModel {
      public int Month { get; set; }
      public int Year { get; set; }
    }

    [TestMethod]
    public void TestJanuary0() {
      DateModel date = new DateModel {
        Month = 1,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "January 0 is valid");
    }

    [TestMethod]
    public void TestJanuary1() {
      DateModel date = new DateModel {
        Month = 1,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "January 1 is invalid");
    }

    [TestMethod]
    public void TestJanuary31() {
      DateModel date = new DateModel {
        Month = 1,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "January 31 is invalid");
    }

    [TestMethod]
    public void TestJanuary32() {
      DateModel date = new DateModel {
        Month = 1,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "January 32 is valid");
    }

    [TestMethod]
    public void TestFebruary0() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "February 0 is valid");
    }

    [TestMethod]
    public void TestFebruary1() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "February 1 is invalid");
    }

    [TestMethod]
    public void TestFebruary28() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(28), "February 28 is invalid");
    }

    [TestMethod]
    public void TestFebruary291999() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 1999
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(29), "February 29-1999 is valid");
    }

    [TestMethod]
    public void TestFebruary292000() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(29), "February 29-2000 is invalid");
    }

    [TestMethod]
    public void TestFebruary292004() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 2004
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(29), "February 29-2004 is invalid");
    }

    [TestMethod]
    public void TestFebruary292100() {
      DateModel date = new DateModel {
        Month = 2,
        Year = 2100
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(29), "February 29-2100 is valid");
    }

    [TestMethod]
    public void TestMarch0() {
      DateModel date = new DateModel {
        Month = 3,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "March 0 is valid");
    }

    [TestMethod]
    public void TestMarch1() {
      DateModel date = new DateModel {
        Month = 3,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "March 1 is invalid");
    }

    [TestMethod]
    public void TestMarch31() {
      DateModel date = new DateModel {
        Month = 3,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "March 31 is invalid");
    }

    [TestMethod]
    public void TestMarch32() {
      DateModel date = new DateModel {
        Month = 3,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "March 32 is valid");
    }

    [TestMethod]
    public void TestApril0() {
      DateModel date = new DateModel {
        Month = 4,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "April 0 is valid");
    }

    [TestMethod]
    public void TestApril1() {
      DateModel date = new DateModel {
        Month = 4,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "April 1 is invalid");
    }

    [TestMethod]
    public void TestApril30() {
      DateModel date = new DateModel {
        Month = 4,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(30), "April 30 is invalid");
    }

    [TestMethod]
    public void TestApril31() {
      DateModel date = new DateModel {
        Month = 4,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(31), "April 31 is valid");
    }

    [TestMethod]
    public void TestMay0() {
      DateModel date = new DateModel {
        Month = 5,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "May 0 is valid");
    }

    [TestMethod]
    public void TestMay1() {
      DateModel date = new DateModel {
        Month = 5,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "May 1 is invalid");
    }

    [TestMethod]
    public void TestMay31() {
      DateModel date = new DateModel {
        Month = 5,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "May 31 is invalid");
    }

    [TestMethod]
    public void TestMay32() {
      DateModel date = new DateModel {
        Month = 5,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "May 32 is valid");
    }

    [TestMethod]
    public void TestJune0() {
      DateModel date = new DateModel {
        Month = 6,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "June 0 is valid");
    }

    [TestMethod]
    public void TestJune1() {
      DateModel date = new DateModel {
        Month = 6,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "June 1 is invalid");
    }

    [TestMethod]
    public void TestJune30() {
      DateModel date = new DateModel {
        Month = 6,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(30), "June 30 is invalid");
    }

    [TestMethod]
    public void TestJune31() {
      DateModel date = new DateModel {
        Month = 6,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(31), "June 31 is valid");
    }

    [TestMethod]
    public void TestJuly0() {
      DateModel date = new DateModel {
        Month = 7,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "July 0 is valid");
    }

    [TestMethod]
    public void TestJuly1() {
      DateModel date = new DateModel {
        Month = 7,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "July 1 is invalid");
    }

    [TestMethod]
    public void TestJuly31() {
      DateModel date = new DateModel {
        Month = 7,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "July 31 is invalid");
    }

    [TestMethod]
    public void TestJuly32() {
      DateModel date = new DateModel {
        Month = 7,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "July 32 is valid");
    }

    [TestMethod]
    public void TestAugust0() {
      DateModel date = new DateModel {
        Month = 8,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "August 0 is valid");
    }

    [TestMethod]
    public void TestAugust1() {
      DateModel date = new DateModel {
        Month = 8,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "August 1 is invalid");
    }

    [TestMethod]
    public void TestAugust31() {
      DateModel date = new DateModel {
        Month = 8,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "August 31 is invalid");
    }

    [TestMethod]
    public void TestAugust32() {
      DateModel date = new DateModel {
        Month = 8,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "August 32 is valid");
    }

    [TestMethod]
    public void TestSeptember0() {
      DateModel date = new DateModel {
        Month = 9,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "September 0 is valid");
    }

    [TestMethod]
    public void TestSeptember1() {
      DateModel date = new DateModel {
        Month = 9,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "September 1 is invalid");
    }

    [TestMethod]
    public void TestSeptember30() {
      DateModel date = new DateModel {
        Month = 9,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(30), "September 30 is invalid");
    }

    [TestMethod]
    public void TestSeptember31() {
      DateModel date = new DateModel {
        Month = 9,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(31), "September 31 is valid");
    }

    [TestMethod]
    public void TestOctober0() {
      DateModel date = new DateModel {
        Month = 10,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "October 0 is valid");
    }

    [TestMethod]
    public void TestOctober1() {
      DateModel date = new DateModel {
        Month = 10,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "October 1 is invalid");
    }

    [TestMethod]
    public void TestOctober31() {
      DateModel date = new DateModel {
        Month = 10,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "October 31 is invalid");
    }

    [TestMethod]
    public void TestOctober32() {
      DateModel date = new DateModel {
        Month = 10,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "October 32 is valid");
    }

    [TestMethod]
    public void TestNovember0() {
      DateModel date = new DateModel {
        Month = 11,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "November 0 is valid");
    }

    [TestMethod]
    public void TestNovember1() {
      DateModel date = new DateModel {
        Month = 11,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "November 1 is invalid");
    }

    [TestMethod]
    public void TestNovember30() {
      DateModel date = new DateModel {
        Month = 11,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(30), "November 30 is invalid");
    }

    [TestMethod]
    public void TestNovember31() {
      DateModel date = new DateModel {
        Month = 11,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(31), "November 31 is valid");
    }

    [TestMethod]
    public void TestDecember0() {
      DateModel date = new DateModel {
        Month = 12,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(0), "December 0 is valid");
    }

    [TestMethod]
    public void TestDecember1() {
      DateModel date = new DateModel {
        Month = 12,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(1), "December 1 is invalid");
    }

    [TestMethod]
    public void TestDecember31() {
      DateModel date = new DateModel {
        Month = 12,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsTrue(validator.IsValid(31), "December 31 is invalid");
    }

    [TestMethod]
    public void TestDecember32() {
      DateModel date = new DateModel {
        Month = 12,
        Year = 2000
      };
      IUFValidateValue validator = new UFValidateDay("Month", "Year", date);
      Assert.IsFalse(validator.IsValid(32), "December 32 is valid");
    }
  }
}
