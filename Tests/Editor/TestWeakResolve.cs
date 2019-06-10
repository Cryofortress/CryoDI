using NUnit.Framework;

namespace CryoDI.Tests
{
	[TestFixture]
	public class TestWeakResolve
	{
		class MyClazz
		{
		}

		[Test]
		public void TestSingleton()
		{
			var container = new CryoContainer();
			container.RegisterSingleton<MyClazz>();
			
			Assert.IsNull(container.WeakResolve<MyClazz>());
			var clazz = container.Resolve<MyClazz>();
			Assert.IsNotNull(clazz);

			var clazz2 = container.WeakResolve<MyClazz>();
			Assert.IsNotNull(clazz2);
			Assert.AreSame(clazz, clazz2);
		}
		
		[Test]
		public void TestSingletonByName()
		{
			var container = new CryoContainer();
			container.RegisterSingleton<MyClazz>("MyClazz");
			
			Assert.IsNull(container.WeakResolveByName<MyClazz>("MyClazz"));
			var clazz = container.ResolveByName<MyClazz>("MyClazz");
			Assert.IsNotNull(clazz);

			var clazz2 = container.WeakResolveByName<MyClazz>("MyClazz");
			Assert.IsNotNull(clazz2);
			Assert.AreSame(clazz, clazz2);

			try
			{
				container.WeakResolve<MyClazz>();
				Assert.Fail("Exception expected");
				
				//ascxasxasxasx
			}
			catch (ContainerException)
			{
				return;
			}
		}
		
		[Test]
		public void TestType()
		{
			var container = new CryoContainer();
			container.RegisterType<MyClazz>();
			
			Assert.IsNull(container.WeakResolve<MyClazz>());
			var clazz = container.Resolve<MyClazz>();
			Assert.IsNotNull(clazz);
			Assert.IsNull(container.WeakResolve<MyClazz>());
		}
		
		[Test]
		public void TestInstance()
		{
			var container = new CryoContainer();
			var myClazz = new MyClazz();
			container.RegisterInstance(myClazz);

			var myClazz2 = container.WeakResolve<MyClazz>();
			Assert.IsNotNull(myClazz2);
			Assert.AreSame(myClazz, myClazz2);
		}
	}
}