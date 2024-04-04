using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltraForce.Library.NetStandard.Services;
using UltraForce.Library.NetStandard.Tools;

namespace Tests.Services {
  [TestClass]
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class UFSimpleServiceCollectionTests {
    public class SingletonClass {
    }

    public class MultipleClass {
    }

    public class UsingSingletonClass(SingletonClass anInstance)
    {
      public SingletonClass Instance { get; } = anInstance;
    }

    public class UsingMultipleClass(MultipleClass anInstance)
    {
      public MultipleClass Instance { get; } = anInstance;
    }

    public interface ITestService {
      void TestMethod();
    }

    public class TestServiceClass() : ITestService {
      public void TestMethod() {
      }
    }

    public class UsingServiceClass(ITestService aService)
    {
      public ITestService Service { get; } = aService;
    }

    public class UsingUsingServiceClass(UsingServiceClass anInstance)
    {
      public UsingServiceClass Instance { get; } = anInstance;
    }

    [TestMethod]
    public void LazySingletonTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).Register<SingletonClass>();
      UsingSingletonClass? instance = UFServiceCollectionTools.GetService<UsingSingletonClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Instance, "property missing value");
    }

    [TestMethod]
    public void ExistingSingletonTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      SingletonClass singleton = new SingletonClass();
      UFServiceCollectionTools.With(factory).RegisterSingleton(singleton);
      UsingSingletonClass? instance = UFServiceCollectionTools.GetService<UsingSingletonClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Instance, "property missing value");
      Assert.AreEqual(singleton, instance.Instance, "Instance is not the singleton");
    }

    [TestMethod]
    public void LazyMultipleSingletonTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).RegisterSingleton<SingletonClass>();
      UsingSingletonClass? instance0 = UFServiceCollectionTools.GetService<UsingSingletonClass>(factory);
      UsingSingletonClass? instance1 = UFServiceCollectionTools.GetService<UsingSingletonClass>(factory);
      Assert.IsNotNull(instance0, "instance0 not created");
      Assert.IsNotNull(instance0.Instance, "instance0 property missing value");
      Assert.IsNotNull(instance1, "instance1 not created");
      Assert.IsNotNull(instance1.Instance, "instance0 property missing value");
      Assert.AreEqual(instance0.Instance, instance1.Instance, "singleton Instance is different");
    }

    [TestMethod]
    public void ExistingMultipleSingletonTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      SingletonClass singleton = new SingletonClass();
      UFServiceCollectionTools.With(factory).RegisterSingleton(singleton);
      UsingSingletonClass? instance0 = UFServiceCollectionTools.GetService<UsingSingletonClass>(factory);
      UsingSingletonClass? instance1 = UFServiceCollectionTools.GetService<UsingSingletonClass>(factory);
      Assert.IsNotNull(instance0, "instance0 not created");
      Assert.IsNotNull(instance0.Instance, "instance0 property missing value");
      Assert.AreEqual(singleton, instance0.Instance, "instance0.Instance is not the singleton");
      Assert.IsNotNull(instance1, "instance1 not created");
      Assert.IsNotNull(instance1.Instance, "instance0 property missing value");
      Assert.AreEqual(singleton, instance1.Instance, "instance1.Instance is not the singleton");
    }

    [TestMethod]
    public void MultipleTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).Register<MultipleClass>();
      UsingMultipleClass instance0 = UFServiceCollectionTools.GetService<UsingMultipleClass>(factory);
      UsingMultipleClass instance1 = UFServiceCollectionTools.GetService<UsingMultipleClass>(factory);
      Assert.IsNotNull(instance0, "instance0 not created");
      Assert.IsNotNull(instance0.Instance, "instance0 property missing value");
      Assert.IsNotNull(instance1, "instance1 not created");
      Assert.IsNotNull(instance1.Instance, "instance0 property missing value");
      Assert.AreNotEqual(instance0.Instance, instance1.Instance, "Instance is shared between instances");
    }

    [TestMethod]
    public void LazySingletonServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService, TestServiceClass>();
      UsingServiceClass instance = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Service, "instance0 property missing value");
      Assert.IsTrue(instance.Service is TestServiceClass, "not correct class");
    }

    [TestMethod]
    public void LazyMultipleSingletonServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService, TestServiceClass>();
      UsingServiceClass? instance0 = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      UsingServiceClass? instance1 = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      Assert.IsNotNull(instance0, "instance not created");
      Assert.IsNotNull(instance0.Service, "instance0 property missing value");
      Assert.IsNotNull(instance1, "instance not created");
      Assert.IsNotNull(instance1.Service, "instance1 property missing value");
      Assert.AreEqual(instance0.Service, instance1.Service, "properties are not equal");
    }

    [TestMethod]
    public void ExistingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      TestServiceClass singleton = new TestServiceClass();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService>(singleton);
      UsingServiceClass instance = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.AreEqual(singleton, instance.Service, "Service property is not equal to singleton");
    }

    [TestMethod]
    public void ExistingMultipleSingletonServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      TestServiceClass singleton = new TestServiceClass();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService>(singleton);
      UsingServiceClass instance0 = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      UsingServiceClass instance1 = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      Assert.IsNotNull(instance0, "instance not created");
      Assert.IsNotNull(instance0.Service, "instance0 property missing value");
      Assert.AreEqual(singleton, instance0.Service, "instance0 Service property is not equal to singleton");
      Assert.IsNotNull(instance1, "instance not created");
      Assert.IsNotNull(instance1.Service, "instance1 property missing value");
      Assert.AreEqual(singleton, instance1.Service, "instance1 Service property is not equal to singleton");
    }

    [TestMethod]
    public void MultipleServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).Register<ITestService, TestServiceClass>();
      UsingServiceClass instance = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Service, "instance0 property missing value");
      Assert.IsTrue(instance.Service is TestServiceClass, "not correct class");
    }

    [TestMethod]
    public void MultipleMultipleServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).Register<ITestService, TestServiceClass>();
      UsingServiceClass instance0 = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      UsingServiceClass instance1 = UFServiceCollectionTools.GetService<UsingServiceClass>(factory);
      Assert.IsNotNull(instance0, "instance not created");
      Assert.IsNotNull(instance0.Service, "instance0 property missing value");
      Assert.IsNotNull(instance1, "instance not created");
      Assert.IsNotNull(instance1.Service, "instance1 property missing value");
      Assert.AreNotEqual(instance0.Service, instance1.Service, "properties are equal");
    }

    [TestMethod]
    public void LazySingletonUsingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService, TestServiceClass>();
      UFServiceCollectionTools.With(factory).Register<UsingServiceClass>();
      UsingUsingServiceClass instance = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Instance, "Instance not created");
      Assert.IsTrue(instance.Instance.Service is TestServiceClass, "not correct class");
    }

    [TestMethod]
    public void LazyMultipleSingletonUsingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService, TestServiceClass>();
      UFServiceCollectionTools.With(factory).Register<UsingServiceClass>();
      UsingUsingServiceClass instance0 = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      UsingUsingServiceClass instance1 = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      Assert.IsNotNull(instance0, "instance0 not created");
      Assert.IsNotNull(instance0.Instance, "instance0.Instance property missing value");
      Assert.IsNotNull(instance0.Instance.Service, "instance0.Instance.Service property missing value");
      Assert.IsNotNull(instance1, "instance not created");
      Assert.IsNotNull(instance1.Instance, "instance1.Instance property missing value");
      Assert.IsNotNull(instance1.Instance.Service, "instance1.Instance.Service property missing value");
      Assert.AreEqual(instance0.Instance.Service, instance1.Instance.Service, "properties are not equal");
    }

    [TestMethod]
    public void ExistingUsingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      TestServiceClass singleton = new TestServiceClass();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService>(singleton);
      UFServiceCollectionTools.With(factory).Register<UsingServiceClass>();
      UsingUsingServiceClass instance = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Instance, "Instance not created");
      Assert.AreEqual(singleton, instance.Instance.Service, "Service property is not equal to singleton");
    }

    [TestMethod]
    public void ExistingMultipleSingletonUsingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      TestServiceClass singleton = new TestServiceClass();
      UFServiceCollectionTools.With(factory).RegisterSingleton<ITestService>(singleton);
      UFServiceCollectionTools.With(factory).Register<UsingServiceClass>();
      UsingUsingServiceClass instance0 = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      UsingUsingServiceClass instance1 = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      Assert.IsNotNull(instance0, "instance not created");
      Assert.IsNotNull(instance0.Instance.Service, "instance0.Instance.Service property missing value");
      Assert.AreEqual(singleton, instance0.Instance.Service, "instance0 Service property is not equal to singleton");
      Assert.IsNotNull(instance1, "instance not created");
      Assert.IsNotNull(instance1.Instance, "instance1.Instance property missing value");
      Assert.IsNotNull(instance1.Instance.Service, "instance1.Instance.Service property missing value");
      Assert.AreEqual(singleton, instance1.Instance.Service, "instance1 Service property is not equal to singleton");
    }

    [TestMethod]
    public void MultipleUsingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).Register<ITestService, TestServiceClass>();
      UFServiceCollectionTools.With(factory).Register<UsingServiceClass>();
      UsingUsingServiceClass instance = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      Assert.IsNotNull(instance, "instance not created");
      Assert.IsNotNull(instance.Instance, "Instance not created");
      Assert.IsTrue(instance.Instance.Service is TestServiceClass, "not correct class");
    }

    [TestMethod]
    public void MultipleMultipleUsingServiceTest() {
      UFSimpleServiceCollection factory = new UFSimpleServiceCollection();
      UFServiceCollectionTools.With(factory).Register<ITestService, TestServiceClass>();
      UFServiceCollectionTools.With(factory).Register<UsingServiceClass>();
      UsingUsingServiceClass instance0 = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      UsingUsingServiceClass instance1 = UFServiceCollectionTools.GetService<UsingUsingServiceClass>(factory);
      Assert.IsNotNull(instance0, "instance not created");
      Assert.IsNotNull(instance0.Instance.Service, "instance0.Instance.Service property missing value");
      Assert.IsNotNull(instance1, "instance not created");
      Assert.IsNotNull(instance1.Instance, "instance1.Instance property missing value");
      Assert.IsNotNull(instance1.Instance.Service, "instance1.Instance.Service property missing value");
      Assert.AreNotEqual(instance0.Instance.Service, instance1.Instance.Service, "Service properties are equal");
    }
  }
}
