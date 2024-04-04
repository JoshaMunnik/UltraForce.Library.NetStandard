using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Models;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Wrappers {
  /// <summary>
  /// Some tests require Release version; GC behaves differently while 
  /// debugging see following topic:
  /// https://stackoverflow.com/a/17131389/968451
  /// </summary>
  [TestClass]
  public class UFWeakEventHandlerTests {
    public int CallCount { get; set; }
    public int DisposeCount { get; set; }
    public int AddCount { get; set; }
    public int RemoveCount { get; set; }

    private class DataClass : UFModel {
      private readonly UFWeakEventHandlerTests m_parent;

      public DataClass(UFWeakEventHandlerTests aParent) {
        this.m_parent = aParent;
      }

      public string Test {
        get => this.Get("");
        set => this.Set(value);
      }

      public new event EventHandler<UFDataChangedEventArgs> DataChanged {
        add {
          base.DataChanged += value;
          this.m_parent.AddCount++;
        }
        remove {
          base.DataChanged -= value;
          this.m_parent.RemoveCount++;
        }
      }
    }

    private class HandlerClass {
      private readonly UFWeakEventHandlerTests? m_parent;

      public HandlerClass(UFWeakEventHandlerTests? aParent) {
        this.m_parent = aParent;
      }

      ~HandlerClass() {
        if (this.m_parent != null) {
          this.m_parent.DisposeCount++;
        }
      }

      public void HandleDataChanged(object aSender, UFDataChangedEventArgs anEvent) {
        if (anEvent.HasChanged(nameof(DataClass.Test))) {
          this.m_parent.CallCount++;
        }
      }

      public UFWeakReferencedEventHandlerManager<UFDataChangedEventArgs> Handle { get; set; }
    }

    [TestMethod]
    public void WeakReferenceSimple_SingleEvent() {
      this.CallCount = 0;
      this.DisposeCount = 0;
      this.AddCount = 0;
      this.RemoveCount = 0;
      DataClass data = new DataClass(this);
      HandlerClass handler = new HandlerClass(this);
      data.DataChanged += UFWeakReferencedEventHandler.Wrap(handler.HandleDataChanged);
      UFSystemTools.TriggerGarbageCollector();
      data.Test = "1234";
      Assert.AreNotEqual(handler, null);
      Assert.AreEqual(this.CallCount, 1, "HandleDataChanged has not been called");
      Assert.AreEqual(this.AddCount, 1, "no DataChanged handler has been installed");
    }

    [TestMethod]
    public void WeakReferenceSimple_TwoEvent() {
#if DEBUG
      throw new Exception("This test will fail in debug mode");
#endif
      this.CallCount = 0;
      this.DisposeCount = 0;
      this.AddCount = 0;
      this.RemoveCount = 0;
      DataClass data = new DataClass(this);
      HandlerClass handler = new HandlerClass(this);
      data.DataChanged += UFWeakReferencedEventHandler.Wrap(handler.HandleDataChanged);
      UFSystemTools.TriggerGarbageCollector();
      data.Test = "1234";
      Assert.AreNotEqual(handler, null);
      Assert.AreEqual(this.CallCount, 1, "HandleDataChanged has not been called");
      Assert.AreEqual(this.AddCount, 1, "no DataChanged handler has been installed");
      handler = null;
      UFSystemTools.TriggerGarbageCollector();
      Assert.AreEqual(this.DisposeCount, 1, "Handler has not been disposed");
      data.Test = "5678";
      Assert.AreEqual(this.CallCount, 1, "HandleDataChanged has been called again");
      Assert.AreEqual(this.RemoveCount, 0, "a DataChanged handler has been removed");
    }

    [TestMethod]
    public void WeakReferenceManager_SingleEvent() {
      this.CallCount = 0;
      this.DisposeCount = 0;
      this.AddCount = 0;
      this.RemoveCount = 0;
      DataClass data = new DataClass(this);
      HandlerClass handler = new HandlerClass(this);
      handler.Handle = UFWeakReferencedEventHandlerManager.Create(
        handler.HandleDataChanged!, data
      );
      UFSystemTools.TriggerGarbageCollector();
      data.Test = "1234";
      Assert.AreNotEqual(handler, null);
      Assert.AreEqual(this.CallCount, 1, "HandleDataChanged has not been called");
      Assert.AreEqual(this.AddCount, 1, "no DataChanged handler has been installed");
    }

    [TestMethod]
    public void WeakReferenceManager_TwoEvent() {
#if DEBUG
      throw new Exception("This test will fail in debug mode");
#endif
      this.CallCount = 0;
      this.DisposeCount = 0;
      this.AddCount = 0;
      this.RemoveCount = 0;
      DataClass data = new DataClass(this);
      HandlerClass handler = new HandlerClass(this);
      handler.Handle = UFWeakReferencedEventHandlerManager.Create(
        handler.HandleDataChanged, data
      );
      WeakReference reference = new WeakReference(handler.Handle);
      data.Test = "1234";
      Assert.AreEqual(this.CallCount, 1, "HandleDataChanged has not been called");
      Assert.AreEqual(this.AddCount, 1, "no DataChanged handler has been installed");
      handler = new HandlerClass(null);
      UFSystemTools.TriggerGarbageCollector();
      Assert.AreEqual(this.DisposeCount, 1, "Handler has not been disposed");
      Assert.IsFalse(reference.IsAlive, "UFWeakEventHandler instance is still active");
      data.Test = "5678";
      Assert.AreEqual(this.CallCount, 1, "HandleDataChanged has been called again");
      Assert.AreEqual(this.RemoveCount, 1, "no DataChanged handler has been removed");
    }

  }
}
