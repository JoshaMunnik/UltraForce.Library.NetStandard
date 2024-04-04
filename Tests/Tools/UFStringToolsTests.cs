using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Tools {
  [TestClass]
  public class UFStringToolsTests {
    [TestMethod]
    public void Add_Normal() {
      Assert.AreEqual("1, 2", UFStringTools.Add("1", "2"));
    }

    [TestMethod]
    public void Add_NullSource() {
      Assert.AreEqual("2", UFStringTools.Add(null, "2"));
    }

    [TestMethod]
    public void Add_EmptySource() {
      Assert.AreEqual("2", UFStringTools.Add("", "2"));
    }

    [TestMethod]
    public void Add_NullValue() {
      Assert.AreEqual("1, ", UFStringTools.Add("1", null));
    }

    [TestMethod]
    public void Add_EmptyValue() {
      Assert.AreEqual("1, ", UFStringTools.Add("1", ""));
    }

    [TestMethod]
    public void Add_CustomSeparator() {
      Assert.AreEqual("1 + 2", UFStringTools.Add("1", "2", " + "));
    }

    [TestMethod]
    public void SelectString_Normal() {
      Assert.AreEqual("1234", UFStringTools.SelectString("1234"));
    }

    [TestMethod]
    public void SelectString_Empty() {
      Assert.AreEqual("", UFStringTools.SelectString(""));
    }

    [TestMethod]
    public void SelectString_Null() {
      Assert.AreEqual("", UFStringTools.SelectString(null));
    }

    [TestMethod]
    public void SelectString_Alternative_Normal() {
      Assert.AreEqual("1234", UFStringTools.SelectString("1234", "5678"));
    }

    [TestMethod]
    public void SelectString_Alternative_Empty() {
      Assert.AreEqual("5678", UFStringTools.SelectString("", "5678"));
    }

    [TestMethod]
    public void SelectString_Alternative_Null() {
      Assert.AreEqual("5678", UFStringTools.SelectString(null, "5678"));
    }

    [TestMethod]
    public void SelectString_NullAlternative_Normal() {
      Assert.AreEqual("1234", UFStringTools.SelectString("1234", null));
    }

    [TestMethod]
    public void SelectString_NullAlternative_Empty() {
      Assert.AreEqual("", UFStringTools.SelectString("", null));
    }

    [TestMethod]
    public void SelectString_NullAlternative_Null() {
      Assert.AreEqual("", UFStringTools.SelectString(null, null));
    }

    [TestMethod]
    public void GenerateCode_Length() {
      for (int index = 0; index < 300; index++) {
        string code = UFStringTools.GenerateCode(index);
        Assert.AreEqual(index, code.Length, $"Code should have length {index}");
      }
    }

    [TestMethod]
    public void GenerateCode_ValidChars() {
      string code = UFStringTools.GenerateCode(1000000);
      for (int index = code.Length - 1; index >= 0; index--) {
        char codeChar = code[index];
        switch (codeChar) {
          case '0':
          case 'O':
          case '1':
          case 'l':
            Assert.Fail($"Code contains invalid char ${codeChar} at ${index}");
            break;
        }
      }
    }

    [TestMethod]
    public void GenerateCode_ValidBlocks() {
      string code = UFStringTools.GenerateCode(1000000);
      int letterCount = 0;
      for (int index = code.Length - 1; index >= 0; index--) {
        char codeChar = code[index];
        switch (codeChar) {
          case '0':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            letterCount = 0;
            break;
          default:
            letterCount++;
            break;
        }
        Assert.IsFalse(letterCount >= 3);
      }
    }

    [TestMethod]
    public void GenerateTextCode_Length() {
      for (int index = 0; index < 300; index++) {
        string code = UFStringTools.GenerateTextCode(index);
        Assert.AreEqual(index, code.Length, $"Code should have length {index}");
      }
    }

    [TestMethod]
    public void GenerateTextCode_ValidChars() {
      string code = UFStringTools.GenerateTextCode(1000000);
      for (int index = code.Length - 1; index >= 0; index--) {
        char codeChar = code[index];
        switch (codeChar) {
          case 'O':
          case 'l':
            Assert.Fail($"Code contains invalid char ${codeChar} at ${index}");
            break;
        }
      }
    }

    [TestMethod]
    public void Count_None() {
      Assert.AreEqual(0, UFStringTools.Count('a', "bcdefg"));
    }

    [TestMethod]
    public void Count_CaseSensitive() {
      Assert.AreEqual(5, UFStringTools.Count('a', "abacAdaeafga"));
    }

    [TestMethod]
    public void Count_CaseInsensitive() {
      Assert.AreEqual(6, UFStringTools.Count('a', "abacAdaeafga", true));
    }

    [TestMethod]
    public void GetHexStringTest() {
      Assert.AreEqual("00010A10A0A1", UFStringTools.GetHexString(new byte[] {
        0, 1, 10, 16, 160, 161
    }));
  }
  }
}
