using NUnit.Framework;

#if UNITY_EDITOR

namespace CryoDI.Tests
{
	public class MyBaseClass
	{
	}
	
	public class MyClass : MyBaseClass
	{
		public MyClass(int instanceId)
		{
			InstanceId = instanceId;
		}
		
		public int InstanceId { get; private set; }
	}
	
	[TestFixture]
	public class TestFactoryMethods
	{
		private int _counter;
		private MyClass CreateMyClass()
		{
			_counter++;
			return new MyClass(_counter);
		}
		
		[Test]
		public void TestType()
		{
			_counter = 0;
			var container = new CryoContainer();
			container.RegisterType<MyClass>(CreateMyClass);

			var a = container.Resolve<MyClass>();
			Assert.IsNotNull(a);
			Assert.AreEqual(1, a.InstanceId);
			Assert.AreEqual(1, _counter);
			
			a = container.Resolve<MyClass>();
			Assert.IsNotNull(a);
			Assert.AreEqual(2, a.InstanceId);
			Assert.AreEqual(2, _counter);
			
			a = container.Resolve<MyClass>();
			Assert.IsNotNull(a);
			Assert.AreEqual(3, a.InstanceId);
			Assert.AreEqual(3, _counter);
		}
		
		[Test]
		public void TestBaseType()
		{
			_counter = 0;
			var container = new CryoContainer();
			container.RegisterType<MyBaseClass, MyClass>(CreateMyClass);

			var a = container.Resolve<MyBaseClass>();
			Assert.IsNotNull(a);
			Assert.IsInstanceOf(typeof(MyClass), a);
			Assert.AreEqual(1, ((MyClass)a).InstanceId);
			Assert.AreEqual(1, _counter);
			
			a = container.Resolve<MyBaseClass>();
			Assert.IsNotNull(a);
			Assert.IsInstanceOf(typeof(MyClass), a);
			Assert.AreEqual(2, ((MyClass)a).InstanceId);
			Assert.AreEqual(2, _counter);
			
			a = container.Resolve<MyBaseClass>();
			Assert.IsNotNull(a);
			Assert.IsInstanceOf(typeof(MyClass), a);
			Assert.AreEqual(3, ((MyClass)a).InstanceId);
			Assert.AreEqual(3, _counter);
		}
		
		[Test]
		public void TestSingleton()
		{
			_counter = 0;
			var container = new CryoContainer();
			container.RegisterSingleton<MyClass>(CreateMyClass);

			var a = container.Resolve<MyClass>();
			Assert.IsNotNull(a);
			Assert.AreEqual(1, a.InstanceId);
			Assert.AreEqual(1, _counter);
			
			a = container.Resolve<MyClass>();
			Assert.IsNotNull(a);
			Assert.AreEqual(1, a.InstanceId);
			Assert.AreEqual(1, _counter);
			
			a = container.Resolve<MyClass>();
			Assert.IsNotNull(a);
			Assert.AreEqual(1, a.InstanceId);
			Assert.AreEqual(1, _counter);
		}
		
		[Test]
		public void TestBaseSingleton()
		{
			_counter = 0;
			var container = new CryoContainer();
			container.RegisterSingleton<MyBaseClass, MyClass>(CreateMyClass);

			var a = container.Resolve<MyBaseClass>();
			Assert.IsNotNull(a);
			Assert.IsInstanceOf(typeof(MyClass), a);
			Assert.AreEqual(1, ((MyClass)a).InstanceId);
			Assert.AreEqual(1, _counter);
			
			a = container.Resolve<MyBaseClass>();
			Assert.IsNotNull(a);
			Assert.IsInstanceOf(typeof(MyClass), a);
			Assert.AreEqual(1, ((MyClass)a).InstanceId);
			Assert.AreEqual(1, _counter);
			
			a = container.Resolve<MyBaseClass>();
			Assert.IsNotNull(a);
			Assert.IsInstanceOf(typeof(MyClass), a);
			Assert.AreEqual(1, ((MyClass)a).InstanceId);
			Assert.AreEqual(1, _counter);
		}
	}
}

#endif