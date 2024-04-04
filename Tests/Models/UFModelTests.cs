using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Interfaces;
using UltraForce.Library.NetStandard.Models;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Models {
  /// <summary>
  /// Tests UFModel
  /// </summary>
  [TestClass]
  public class UFModelTests {
    /// <summary>
    /// Simple UFModel subclass that contains an integer property
    /// </summary>
    private class IntModel : UFModel {
      private int m_value;

      public int Value {
        get => this.m_value;
        set => this.Assign(ref this.m_value, value);
      }
    }

    /// <summary>
    /// Simple UFModel subclass that contains an string property
    /// </summary>
    private class StringModel : UFModel {
      private string? m_value;

      public string? Value {
        get => this.m_value;
        set => this.Assign(ref this.m_value, value);
      }
    }

    /// <summary>
    /// ParentModel contains a child property which is also an UFModel and
    /// uses Option.TrackChildChange and Option.LockChildren
    /// </summary>
    private class ParentModel : UFModel {
      private IntModel m_child;

      public ParentModel() : base(Option.TrackChildChange, Option.LockChildren) {
        this.m_child = new IntModel();
        this.UpdateTrackedProperties();
      }

      public IntModel Child {
        get => this.m_child;
        set => this.Assign(ref this.m_child, value);
      }
    }

    /// <summary>
    /// Simple UFModel subclass that contains an integer property
    /// </summary>
    private class InternalIntModel : UFModel {
      public int Value {
        get => this.Get(0);
        set => this.Set(value);
      }
    }

    /// <summary>
    /// Simple UFModel subclass that contains an string property
    /// </summary>
    private class InternalStringModel : UFModel {
      public string Value {
        get => this.Get("");
        set => this.Set(value);
      }
    }

    /// <summary>
    /// ParentModel contains a child property which is also an UFModel and
    /// uses Option.TrackChildChange and Option.LockChildren
    /// </summary>
    private class InternalParentModel : UFModel {
      public InternalParentModel() : base(Option.TrackChildChange, Option.LockChildren) {
        this.Child = new InternalIntModel();
        this.UpdateTrackedProperties();
      }

      public InternalIntModel Child {
        get => this.Get<InternalIntModel>(() => null);
        set => this.Set(value);
      }
    }

    /// <summary>
    /// Helper class to track number of DataChanged events
    /// </summary>
    private class DataChangedListener {
      private readonly UFModelTests m_parent;

      public int Fired { get; private set; }

      public DataChangedListener(IUFNotifyDataChanged aModel, UFModelTests aParent) {
        this.m_parent = aParent;
        aModel.DataChanged += this.HandleDataChanged;
      }

      private void HandleDataChanged(object sender, UFDataChangedEventArgs e) {
        this.m_parent.DataChangedFired++;
        this.Fired++;
      }
    }

    /// <summary>
    /// Helper class to track number of PropertyChanged events
    /// </summary>
    private class PropertyChangedListener {
      private readonly UFModelTests m_parent;

      public int Fired { get; private set; }

      public PropertyChangedListener(INotifyPropertyChanged aModel, UFModelTests aParent) {
        this.m_parent = aParent;
        aModel.PropertyChanged += this.HandlePropertyChanged;
      }

      private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
        this.m_parent.PropertyChangedFired++;
        this.Fired++;
      }
    }

    /// <summary>
    /// Helper class to track number of ChildChanged events
    /// </summary>
    private class ChildChangedListener {
      private readonly UFModelTests m_parent;

      public int Fired { get; private set; }

      public ChildChangedListener(UFModel aModel, UFModelTests aParent) {
        this.m_parent = aParent;
        aModel.ChildChanged += this.HandleChildChanged;
      }

      private void HandleChildChanged(object sender, UFDataChangedEventArgs e) {
        this.m_parent.ChildChangedFired++;
        this.Fired++;
      }
    }

    public int DataChangedFired { get; set; }
    public int PropertyChangedFired { get; set; }
    public int ChildChangedFired { get; set; }

    [TestMethod]
    public void CreateTest_Int() {
      IntModel data = new IntModel();
      Assert.AreEqual(data.Value, 0, "Int value is not 0");
    }

    [TestMethod]
    public void CreateTest_String() {
      StringModel stringData = new StringModel();
      Assert.AreEqual(stringData.Value, null, "String value is not null");
    }

    [TestMethod]
    public void AssignTest_IntDirectReadDirectWrite() {
      IntModel data = new IntModel();
      data.Value = 10;
      Assert.AreEqual(data.Value, 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_IntIndirectReadDirectWrite() {
      IntModel data = new IntModel();
      data.Value = 10;
      Assert.AreEqual((int) data.GetPropertyValue("Value"), 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_IntDirectReadIndirectWrite() {
      IntModel data = new IntModel();
      data.SetPropertyValue("Value", 10);
      Assert.AreEqual(data.Value, 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_IntIndirectReadIndirectWrite() {
      IntModel data = new IntModel();
      data.SetPropertyValue("Value", 10);
      Assert.AreEqual((int)data.GetPropertyValue("Value"), 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_StringDirectReadDirectWrite() {
      StringModel data = new StringModel();
      data.Value = "1234";
      Assert.AreEqual(data.Value, "1234", "String value is not 1234");
    }

    [TestMethod]
    public void AssignTest_StringIndirectReadDirectWrite() {
      StringModel data = new StringModel();
      data.Value = "1234";
      Assert.AreEqual((string)data.GetPropertyValue("Value"), "1234", "String value is not 1234");
    }

    [TestMethod]
    public void AssignTest_StringDirectReadIndirectWrite() {
      StringModel data = new StringModel();
      data.SetPropertyValue("Value", "1234");
      Assert.AreEqual(data.Value, "1234", "String value is not 1234");
    }

    [TestMethod]
    public void AssignTest_StringIndirectReadIndirectWrite() {
      StringModel data = new StringModel();
      data.SetPropertyValue("Value", "1234");
      Assert.AreEqual((string)data.GetPropertyValue("Value"), "1234", "String value is not 1234");
    }

    [TestMethod]
    public void DataChangedOneTimeTest() {
      IntModel data = new IntModel();
      DataChangedListener listener = new DataChangedListener(data, this);
      data.Value = data.Value + 1;
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }

    [TestMethod]
    public void PropertyChangedOneTimeTest() {
      IntModel data = new IntModel();
      PropertyChangedListener listener = new PropertyChangedListener(data, this);
      data.Value = data.Value + 1;
      Assert.AreEqual(listener.Fired, 1, "PropertyChanged did not get fired 1x");
    }

    [TestMethod]
    public void DataChangedTenTimeTest() {
      IntModel data = new IntModel();
      DataChangedListener listener = new DataChangedListener(data, this);
      for (int count = 0; count < 10; count++) {
        data.Value = data.Value + 1;
      }
      Assert.AreEqual(listener.Fired, 10, "DataChanged did not get fired 10x");
    }

    [TestMethod]
    public void PropertyChangedTenTimeTest() {
      IntModel data = new IntModel();
      PropertyChangedListener listener = new PropertyChangedListener(data, this);
      for (int count = 0; count < 10; count++) {
        data.Value = data.Value + 1;
      }
      Assert.AreEqual(listener.Fired, 10, "PropertyChanged did not get fired 10x");
    }

    [TestMethod]
    public void DataChangedReferencesTest() {
      this.DataChangedFired = 0;
      IntModel data = new IntModel();
      DataChangedListener? listener = new DataChangedListener(data, this);
      data.Value = data.Value + 1;
      Assert.AreEqual(this.DataChangedFired, 1, "DataChanged did not get fired 1x");
      listener = null;
      UFSystemTools.TriggerGarbageCollector();
      data.Value = data.Value + 1;
      Assert.AreEqual(this.DataChangedFired, 1, "DataChanged did get fired again");
    }

    [TestMethod]
    public void DataChangedLockTest() {
      IntModel data = new IntModel();
      DataChangedListener listener = new DataChangedListener(data, this);
      data.Lock();
      for (int count = 0; count < 10; count++) {
        data.Value = data.Value + 1;
      }
      Assert.AreEqual(listener.Fired, 0, "DataChanged did get fired");
      data.Unlock();
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }

    [TestMethod]
    public void PropertyChangedLockTest() {
      IntModel data = new IntModel();
      PropertyChangedListener listener = new PropertyChangedListener(data, this);
      data.Lock();
      for (int count = 0; count < 10; count++) {
        data.Value = data.Value + 1;
      }
      Assert.AreEqual(listener.Fired, 0, "PropertyChanged did get fired");
      data.Unlock();
      Assert.AreEqual(listener.Fired, 1, "PropertyChanged did not get fired 1x");
    }

    [TestMethod]
    public void DataChangedMultipleLockTest() {
      IntModel data = new IntModel();
      DataChangedListener listener = new DataChangedListener(data, this);
      for (int count = 0; count < 20; count++) {
        data.Lock();
      }
      for (int count = 0; count < 10; count++) {
        data.Value = data.Value + 1;
      }
      for (int count = 0; count < 20; count++) {
        Assert.AreEqual(listener.Fired, 0, "DataChanged did get fired");
        data.Unlock();
      }
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }

    [TestMethod]
    public void AssignTest_ChildDirectReadDirectWrite() {
      ParentModel data = new ParentModel();
      IntModel newChild = new IntModel() { Value = 20 };
      data.Child.Value = 10;
      data.Child = newChild;
      Assert.AreEqual(data.Child, newChild, "Child value is not the new child");
      Assert.AreEqual(data.Child.Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void AssignTest_ChildIndirectReadDirectWrite() {
      ParentModel data = new ParentModel();
      IntModel newChild = new IntModel() { Value = 20 };
      data.Child.Value = 10;
      data.Child = newChild;
      Assert.AreEqual((IntModel) data.GetPropertyValue("Child"), newChild, "Child value is not the new child");
      Assert.AreEqual(((IntModel) data.GetPropertyValue("Child")).Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void AssignTest_ChildDirectReadIndirectWrite() {
      ParentModel data = new ParentModel();
      IntModel newChild = new IntModel() { Value = 20 };
      data.Child.Value = 10;
      data.SetPropertyValue("Child", newChild);
      Assert.AreEqual(data.Child, newChild, "Child value is not the new child");
      Assert.AreEqual(data.Child.Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void AssignTest_ChildIndirectReadIndirectWrite() {
      ParentModel data = new ParentModel();
      IntModel newChild = new IntModel() { Value = 20 };
      data.Child.Value = 10;
      data.SetPropertyValue("Child", newChild);
      Assert.AreEqual((IntModel)data.GetPropertyValue("Child"), newChild, "Child value is not the new child");
      Assert.AreEqual(((IntModel)data.GetPropertyValue("Child")).Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void ChildChangedTest() {
      ParentModel data = new ParentModel();
      ChildChangedListener listener = new ChildChangedListener(data, this);
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 1, "ChildChanged did not get fired 1x");
    }

    [TestMethod]
    public void ChildNewChildChangedTest() {
      ParentModel data = new ParentModel();
      ChildChangedListener listener = new ChildChangedListener(data, this);
      IntModel newChild = new IntModel() { Value = 20 };
      data.Child = newChild;
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 1, "ChildChanged did not get fired 1x");
    }

    [TestMethod]
    public void ChildChangedLockTest() {
      ParentModel data = new ParentModel();
      DataChangedListener listener = new DataChangedListener(data.Child, this);
      data.Lock();
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 0, "DataChanged did get fired");
      data.Unlock();
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }

    [TestMethod]
    public void ChildNewChildChangedLockTest() {
      ParentModel data = new ParentModel();
      IntModel newChild = new IntModel() { Value = 20 };
      DataChangedListener listener = new DataChangedListener(newChild, this);
      data.Child = newChild;
      data.Lock();
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 0, "DataChanged did get fired");
      data.Unlock();
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }

    [TestMethod]
    public void ChildChangedReferencesTest() {
      ParentModel data = new ParentModel();
      this.ChildChangedFired = 0;
      ChildChangedListener? listener = new ChildChangedListener(data, this);
      data.Child.Value = 10;
      Assert.AreEqual(this.ChildChangedFired, 1, "ChildChanged did not get fired 1x");
      listener = null;
      UFSystemTools.TriggerGarbageCollector();
      data.Child.Value++;
      Assert.AreEqual(this.ChildChangedFired, 1, "DataChanged did get fired again");
    }

    [TestMethod]
    public void CreateTest_InternalInt() {
      InternalIntModel data = new InternalIntModel();
      Assert.AreEqual(data.Value, 0, "Int value is not 0");
    }

    [TestMethod]
    public void CreateTest_InternalString() {
      InternalStringModel stringData = new InternalStringModel();
      Assert.AreEqual(stringData.Value, "", "String value is not empty");
    }

    [TestMethod]
    public void AssignTest_InternalIntDirectReadDirectWrite() {
      InternalIntModel data = new InternalIntModel();
      data.Value = 10;
      Assert.AreEqual(data.Value, 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_InternalIntIndirectReadDirectWrite() {
      InternalIntModel data = new InternalIntModel();
      data.Value = 10;
      Assert.AreEqual((int)data.GetPropertyValue("Value"), 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_InternalIntDirectReadIndirectWrite() {
      InternalIntModel data = new InternalIntModel();
      data.SetPropertyValue("Value", 10);
      Assert.AreEqual(data.Value, 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_InternalIntIndirectReadIndirectWrite() {
      InternalIntModel data = new InternalIntModel();
      data.SetPropertyValue("Value", 10);
      Assert.AreEqual((int)data.GetPropertyValue("Value"), 10, "Int value is not 10");
    }

    [TestMethod]
    public void AssignTest_InternalStringDirectReadDirectWrite() {
      InternalStringModel data = new InternalStringModel();
      data.Value = "1234";
      Assert.AreEqual(data.Value, "1234", "String value is not 1234");
    }

    [TestMethod]
    public void AssignTest_InternalStringIndirectReadDirectWrite() {
      InternalStringModel data = new InternalStringModel();
      data.Value = "1234";
      Assert.AreEqual((string)data.GetPropertyValue("Value"), "1234", "String value is not 1234");
    }

    [TestMethod]
    public void AssignTest_InternalStringDirectReadIndirectWrite() {
      InternalStringModel data = new InternalStringModel();
      data.SetPropertyValue("Value", "1234");
      Assert.AreEqual(data.Value, "1234", "String value is not 1234");
    }

    [TestMethod]
    public void AssignTest_InternalStringIndirectReadIndirectWrite() {
      InternalStringModel data = new InternalStringModel();
      data.SetPropertyValue("Value", "1234");
      Assert.AreEqual((string)data.GetPropertyValue("Value"), "1234", "String value is not 1234");
    }

    [TestMethod]
    public void DataChangedInternalOneTimeTest() {
      InternalIntModel data = new InternalIntModel();
      DataChangedListener listener = new DataChangedListener(data, this);
      data.Value = data.Value + 1;
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }

    [TestMethod]
    public void PropertyChangedInternalOneTimeTest() {
      InternalIntModel data = new InternalIntModel();
      PropertyChangedListener listener = new PropertyChangedListener(data, this);
      data.Value = data.Value + 1;
      Assert.AreEqual(listener.Fired, 1, "PropertyChanged did not get fired 1x");
    }

    [TestMethod]
    public void AssignTest_InternalChild() {
      InternalParentModel data = new InternalParentModel();
      data.Child.Value = 10;
      Assert.AreEqual(data.Child.Value, 10, "Child value is not 10");
    }

    [TestMethod]
    public void AssignTest_InternalChildDirectReadDirectWrite() {
      InternalParentModel data = new InternalParentModel();
      InternalIntModel newChild = new InternalIntModel() { Value = 20 };
      data.Child.Value = 10;
      data.Child = newChild;
      Assert.AreEqual(data.Child, newChild, "Child value is not the new child");
      Assert.AreEqual(data.Child.Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void AssignTest_InternalChildIndirectReadDirectWrite() {
      InternalParentModel data = new InternalParentModel();
      InternalIntModel newChild = new InternalIntModel() { Value = 20 };
      data.Child.Value = 10;
      data.Child = newChild;
      Assert.AreEqual((InternalIntModel)data.GetPropertyValue("Child"), newChild, "Child value is not the new child");
      Assert.AreEqual(((InternalIntModel)data.GetPropertyValue("Child")).Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void AssignTest_InternalChildDirectReadIndirectWrite() {
      InternalParentModel data = new InternalParentModel();
      InternalIntModel newChild = new InternalIntModel() { Value = 20 };
      data.Child.Value = 10;
      data.SetPropertyValue("Child", newChild);
      Assert.AreEqual(data.Child, newChild, "Child value is not the new child");
      Assert.AreEqual(data.Child.Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void AssignTest_InternalChildIndirectReadIndirectWrite() {
      InternalParentModel data = new InternalParentModel();
      InternalIntModel newChild = new InternalIntModel() { Value = 20 };
      data.Child.Value = 10;
      data.SetPropertyValue("Child", newChild);
      Assert.AreEqual((InternalIntModel)data.GetPropertyValue("Child"), newChild, "Child value is not the new child");
      Assert.AreEqual(((InternalIntModel)data.GetPropertyValue("Child")).Value, 20, "Child value is not 20");
    }

    [TestMethod]
    public void InternalChildChangedTest() {
      InternalParentModel data = new InternalParentModel();
      ChildChangedListener listener = new ChildChangedListener(data, this);
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 1, "ChildChanged did not get fired 1x");
    }

    [TestMethod]
    public void InternalChildNewChildChangedTest() {
      InternalParentModel data = new InternalParentModel();
      ChildChangedListener listener = new ChildChangedListener(data, this);
      InternalIntModel newChild = new InternalIntModel() { Value = 20 };
      data.Child = newChild;
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 1, "ChildChanged did not get fired 1x");
    }

    [TestMethod]
    public void InternalChildChangedLockTest() {
      InternalParentModel data = new InternalParentModel();
      DataChangedListener listener = new DataChangedListener(data.Child, this);
      data.Lock();
      data.Child.Value = 10;
      Assert.AreEqual(listener.Fired, 0, "DataChanged did get fired");
      data.Unlock();
      Assert.AreEqual(listener.Fired, 1, "DataChanged did not get fired 1x");
    }
  }
}
