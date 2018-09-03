#if UNITY_EDITOR

using CryoDI;
using UnityEngine;
using NUnit.Framework;

namespace CryoDI.Tests
{
	public class ClassA : IInitializable
	{
		[Dependency]
		public ClassB ClassB { get; set; }

		public ClassA BackRef { get; private set; }

		public bool InitializeCalled { get; private set; }

		public void Initialize()
		{
			InitializeCalled = true;
			BackRef = ClassB.ClassA;
		}
	}

	public class ClassB : IInitializable
	{
		[Dependency]
		public ClassA ClassA { get; set; }

		public ClassB BackRef { get; private set; }

		public bool InitializeCalled { get; private set; }

		public void Initialize()
		{
			InitializeCalled = true;
			BackRef = ClassA.ClassB;
		}
	}

	[TestFixture]
	public class TestCrossDependency
	{
		[Test]
		public void RefResolved()
		{
			var container = new Container();
			container.RegisterSingleton<ClassA>();
			container.RegisterSingleton<ClassB>();

			var a = container.Resolve<ClassA>();
			Assert.IsNotNull(a);
			Assert.IsNotNull(a.ClassB);
			Assert.IsNotNull(a.ClassB.ClassA);
			Assert.AreSame(a, a.ClassB.ClassA);
		}

		[Test]
		public void InitializeCalled()
		{
			var container = new Container();
			container.RegisterSingleton<ClassA>();
			container.RegisterSingleton<ClassB>();

			var a = container.Resolve<ClassA>();
			Assert.IsTrue(a.InitializeCalled);
			Assert.IsTrue(a.ClassB.InitializeCalled);
		}

		[Test]
		public void RefSet()
		{
			var container = new Container();
			container.RegisterSingleton<ClassA>();
			container.RegisterSingleton<ClassB>();

			var a = container.Resolve<ClassA>();
			Assert.IsNotNull(a.BackRef);
			Assert.AreSame(a, a.BackRef);

			var b = a.ClassB;
			Assert.IsNotNull(b.BackRef);
			Assert.AreSame(b, b.BackRef);
		}
	}
}

#endif
