using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Storage;

namespace Tests.Storage {
  [TestClass]
  public class UFDictionaryStorageTests {
    private UFDictionaryStorage CreateTestData() {
      UFDictionaryStorage result = new UFDictionaryStorage();
      result.SetString("testString", "abcdef");
      result.SetInt("testInt", 12345);
      result.SetBool("testBool", true);
      result.SetBytes("testBytes", new byte[] {0, 1, 2, 3, 4, 5, 6});
      return result;
    }

    public bool TestStorage(UFDictionaryStorage aStorage) {
      bool result = aStorage.GetString("testString") == "abcdef";
      if (aStorage.GetInt("testInt") != 12345) {
        result = false;
      }
      if (!aStorage.GetBool("testBool")) {
        result = false;
      }
      byte[] test1 = {0, 1, 2, 3, 4, 5, 6};
      if (!test1.SequenceEqual(aStorage.GetBytes("testBytes"))) {
        result = false;
      }
      return result;
    }

    [TestMethod]
    public void SaveToString_NoCompression() {
      this.CreateTestData()
        .SaveToString();
    }

    [TestMethod]
    public void LoadFromStringTest_NoCompression() {
      UFDictionaryStorage test1 = this.CreateTestData();
      string data = test1.SaveToString();
      UFDictionaryStorage test2 = new UFDictionaryStorage();
      test2.LoadFromString(data);
      Assert.IsTrue(this.TestStorage(test2));
    }

    [TestMethod]
    public void LoadBytesTest_NoCompression() {
      UFDictionaryStorage test1 = this.CreateTestData();
      byte[] data = test1.SaveToBytes();
      UFDictionaryStorage test2 = new UFDictionaryStorage();
      test2.LoadFromBytes(data);
      Assert.IsTrue(this.TestStorage(test2));
    }

    [TestMethod]
    public void LoadStreamTest_NoCompression() {
      UFDictionaryStorage test1 = this.CreateTestData();
      MemoryStream stream = new MemoryStream();
      test1.SaveToStream(stream);
      stream.Position = 0;
      UFDictionaryStorage test2 = new UFDictionaryStorage();
      test2.LoadFromStream(stream);
      Assert.IsTrue(this.TestStorage(test2));
    }

    [TestMethod]
    public void GetStringTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetString("test", "1234");
      Assert.AreEqual("1234", storage.GetString("test"));
    }

    [TestMethod]
    public void SetStringTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetString("test", "1234");
      Assert.IsTrue(storage.HasKey("test"));
      Assert.AreEqual("1234", storage.GetString("test"));
    }

    [TestMethod]
    public void DeleteKeyTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetString("test", "1234");
      Assert.IsTrue(storage.HasKey("test"));
      storage.DeleteKey("test");
      Assert.IsFalse(storage.HasKey("test"));
    }

    [TestMethod]
    public void HasKeyTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetString("test", "1234");
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void HasKeyTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetString("test", "1234");
      Assert.IsFalse(storage.HasKey("test1"));
    }

    [TestMethod]
    public void GetByteTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetByte("test", 231);
      Assert.AreEqual(231, storage.GetByte("test"));
    }

    [TestMethod]
    public void GetByteTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(45, storage.GetByte("test", 45));
    }

    [TestMethod]
    public void SetByteTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetByte("test", 123);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetSByteTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetSByte("test", -123);
      Assert.AreEqual(-123, storage.GetSByte("test"));
    }

    [TestMethod]
    public void GetSByteTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(-45, storage.GetSByte("test", -45));
    }

    [TestMethod]
    public void SetSByteTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetSByte("test", -123);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetUShortTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetUShort("test", 43210);
      Assert.AreEqual(43210, storage.GetUShort("test"));
    }

    [TestMethod]
    public void GetUShortTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(43210, storage.GetUShort("test", 43210));
    }

    [TestMethod]
    public void SetUShortTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetUShort("test", 43210);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetShortTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetShort("test", -12345);
      Assert.AreEqual(-12345, storage.GetShort("test"));
    }

    [TestMethod]
    public void GetShortTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(-12345, storage.GetShort("test", -12345));
    }

    [TestMethod]
    public void SetShortTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetShort("test", -12345);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetUIntTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetUInt("test", 0xaaaaaaaa);
      Assert.AreEqual(0xaaaaaaaa, storage.GetUInt("test"));
    }

    [TestMethod]
    public void GetUIntTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(0xaaaaaaaa, storage.GetUInt("test", 0xaaaaaaaa));
    }

    [TestMethod]
    public void SetUIntTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetUInt("test", 0xaaaaaaaa);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetIntTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetInt("test", -0x3aaaaaaa);
      Assert.AreEqual(-0x3aaaaaaa, storage.GetInt("test"));
    }

    [TestMethod]
    public void GetIntTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(-0x3aaaaaaa, storage.GetInt("test", -0x3aaaaaaa));
    }

    [TestMethod]
    public void SetIntTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetInt("test", -0x3aaaaaaa);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetULongTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetULong("test", 0xaaaaaaaaaaaaaaaa);
      Assert.AreEqual(0xaaaaaaaaaaaaaaaa, storage.GetULong("test"));
    }

    [TestMethod]
    public void GetULongTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(0xaaaaaaaaaaaaaaaa, storage.GetULong("test", 0xaaaaaaaaaaaaaaaa));
    }

    [TestMethod]
    public void SetULongTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetULong("test", 0xaaaaaaaaaaaaaaaa);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetLongTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetLong("test", -0x3aaaaaaaaaaaaaaa);
      Assert.AreEqual(-0x3aaaaaaaaaaaaaaa, storage.GetLong("test"));
    }

    [TestMethod]
    public void GetLongTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(-0x3aaaaaaaaaaaaaaa, storage.GetLong("test", -0x3aaaaaaaaaaaaaaa));
    }

    [TestMethod]
    public void SetLongTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetLong("test", -0x3aaaaaaaaaaaaaaa);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetFloatTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetFloat("test", 1.23456f);
      Assert.AreEqual(1.23456f, storage.GetFloat("test"));
    }

    [TestMethod]
    public void GetFloatTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(1.23456f, storage.GetFloat("test", 1.23456f));
    }

    [TestMethod]
    public void SetFloatTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetFloat("test", 1.23456f);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetDoubleTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetDouble("test", 1.2345678901);
      Assert.AreEqual(1.2345678901, storage.GetDouble("test"));
    }

    [TestMethod]
    public void GetDoubleTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(1.2345678901, storage.GetDouble("test", 1.2345678901));
    }

    [TestMethod]
    public void SetDoubleTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetDouble("test", 1.2345678901);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetBoolTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetBool("test", true);
      Assert.AreEqual(true, storage.GetBool("test"));
    }

    [TestMethod]
    public void GetBoolTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual(true, storage.GetBool("test", true));
    }

    [TestMethod]
    public void SetBoolTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetBool("test", true);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetCharTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetChar("test", 'a');
      Assert.AreEqual('a', storage.GetChar("test"));
    }

    [TestMethod]
    public void GetCharTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      Assert.AreEqual('a', storage.GetChar("test", 'a'));
    }

    [TestMethod]
    public void SetCharTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetChar("test", 'a');
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetBytesTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      byte[] test = {0, 1, 2, 3, 4, 5, 6};
      storage.SetBytes("test", test);
      Assert.IsTrue(test.SequenceEqual(storage.GetBytes("test")));
    }

    [TestMethod]
    public void GetBytesTest1() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      byte[] test1 = { 0, 1, 2, 3, 4, 5, 6 };
      byte[] test2 = { 0, 1, 2, 3, 4, 5, 6 };
      Assert.IsTrue(test1.SequenceEqual(storage.GetBytes("test", test2)));
    }

    [TestMethod]
    public void SetBytesTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      byte[] test1 = { 0, 1, 2, 3, 4, 5, 6 };
      storage.SetBytes("test", test1);
      Assert.IsTrue(storage.HasKey("test"));
    }

    [TestMethod]
    public void GetObjectTest() {
    }

    [TestMethod]
    public void GetObjectTest1() {
    }

    [TestMethod]
    public void SetObjectTest() {
    }

    [TestMethod]
    public void DeleteAllTest() {
      UFDictionaryStorage storage = new UFDictionaryStorage();
      storage.SetString("string", "1234");
      storage.SetInt("int", 1234);
      storage.DeleteAll();
      Assert.IsFalse(storage.HasKey("string"));
      Assert.IsFalse(storage.HasKey("int"));
    }
  }
}