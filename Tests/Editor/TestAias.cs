using NUnit.Framework;

namespace CryoDI.Tests
{
	public interface IFirstInterface
	{
	}

	public interface ISecondInterface
	{
	}

	public class MainClass : IFirstInterface, ISecondInterface
	{
	}

	[TestFixture]
	public class TestAias
	{
		/// <summary>
		/// Create aliases for singleton
		/// </summary>
		[Test]
		public void TestSingleton()
		{
			var container = new CryoContainer();
			container.RegisterSingleton<MainClass>();
			container.RegisterAlias<IFirstInterface, MainClass>();
			container.RegisterAlias<ISecondInterface, MainClass>();

			var firstInterface = container.Resolve<IFirstInterface>();
			var secondInterface = container.Resolve<ISecondInterface>();
			var mainClass = container.Resolve<MainClass>();
			Assert.AreSame(mainClass, firstInterface);
			Assert.AreSame(mainClass, secondInterface);
		}

		/// <summary>
		/// Creating alias for singleton with name
		/// </summary>
		[Test]
		public void TestNamedSingleton()
		{
			var container = new CryoContainer();
			container.RegisterSingleton<MainClass>("MainClass");
			container.RegisterAlias<IFirstInterface, MainClass>(null, "MainClass");
			container.RegisterAlias<ISecondInterface, MainClass>();

			var firstInterface = container.Resolve<IFirstInterface>();
			var mainClass = container.ResolveByName<MainClass>("MainClass");
			Assert.AreSame(mainClass, firstInterface);

			try
			{
				container.Resolve<ISecondInterface>();
				Assert.Fail("Expected CryoDI.ContainerException : Can't resolve type CryoDI.Tests.MainClass");
			}
			catch (ContainerException ex)
			{
				// expected CryoDI.ContainerException : Can't resolve type CryoDI.Tests.MainClass
			}
		}

		/// <summary>
		/// Trying to resolve alias with its own name
		/// </summary>
		[Test]
		public void TestAiasWithName()
		{
			var container = new CryoContainer();
			container.RegisterSingleton<MainClass>();
			container.RegisterAlias<IFirstInterface, MainClass>("AliasWithName");

			var firstInterface = container.ResolveByName<IFirstInterface>("AliasWithName");
			var mainClass = container.Resolve<MainClass>();
			Assert.AreSame(mainClass, firstInterface);

			try
			{
				container.Resolve<IFirstInterface>();
				Assert.Fail("Expected CryoDI.ContainerException : Can't resolve type CryoDI.Tests.IFirstInterface");
			}
			catch (ContainerException ex)
			{
				// expected CryoDI.ContainerException : Can't resolve type CryoDI.Tests.MainClass
			}
		}

		/// <summary>
		/// Create aliases for instance
		/// </summary>
		[Test]
		public void TestInstance()
		{
			var instance = new MainClass();

			var container = new CryoContainer();
			container.RegisterInstance(instance);

			container.RegisterAlias<IFirstInterface, MainClass>();
			container.RegisterAlias<ISecondInterface, MainClass>();

			var firstInterface = container.Resolve<IFirstInterface>();
			var secondInterface = container.Resolve<ISecondInterface>();
			Assert.AreSame(instance, firstInterface);
			Assert.AreSame(instance, secondInterface);
		}
	}
}