#if UNITY_EDITOR

using CryoDI;
using DefaultNamespace;
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
			var container = new CryoContainer {OnCircularDependency = Reaction.LogWarning};
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
			var container = new CryoContainer {OnCircularDependency = Reaction.LogWarning};
			container.RegisterSingleton<ClassA>();
			container.RegisterSingleton<ClassB>();

			var a = container.Resolve<ClassA>();
			Assert.IsTrue(a.InitializeCalled);
			Assert.IsTrue(a.ClassB.InitializeCalled);
		}

		[Test]
		public void RefSet()
		{
			var container = new CryoContainer {OnCircularDependency = Reaction.LogWarning};
			container.RegisterSingleton<ClassA>();
			container.RegisterSingleton<ClassB>();

			var a = container.Resolve<ClassA>();
			Assert.IsNotNull(a.BackRef);
			Assert.AreSame(a, a.BackRef);

			var b = a.ClassB;
			Assert.IsNotNull(b.BackRef);
			Assert.AreSame(b, b.BackRef);
		}
		
		[Test]
		public void ExceptionThrown()
		{
			var container = new CryoContainer {OnCircularDependency = Reaction.ThrowException};
			container.RegisterSingleton<ClassA>();
			container.RegisterSingleton<ClassB>();

			try
			{
				container.Resolve<ClassA>();
				Assert.Fail("Exception expected");
			}
			catch (CircularDependencyException)
			{
				// expected
			}
		}
		
		public class ClassC
		{
			[Dependency]
			private ClassC Property { get; set; }
		}

		[Test]
		public void SelfReference()
		{
			var container = new CryoContainer {OnCircularDependency = Reaction.LogWarning};
			container.RegisterSingleton<ClassC>();
			container.Resolve<ClassC>();
		}
		
		[Test]
		public void SelfReferenceThrow()
		{
			var container = new CryoContainer {OnCircularDependency = Reaction.ThrowException};
			container.RegisterSingleton<ClassC>();

			try
			{
				container.Resolve<ClassC>();
				Assert.Fail("Exception expected");
			}
			catch (CircularDependencyException)
			{
				// expected
			}
		}
		
		public class Class1
		{
			[Dependency]
			private Class2 Class2 { get; set; } 
		}
	
		public class Class2
		{
			[Dependency]
			private Class3 Class3 { get; set; } 
		}
	
		public class Class3
		{
			[Dependency]
			private Class4 Class4 { get; set; } 
		}
	
		public class Class4
		{
			[Dependency]
			private Class1 Class1 { get; set; } 
		}

		[Test]
		public void LongChain()
		{
			var container = new CryoContainer {OnCircularDependency = Reaction.LogWarning};
			container.RegisterSingleton<Class1>();
			container.RegisterSingleton<Class2>();
			container.RegisterSingleton<Class3>();
			container.RegisterSingleton<Class4>();
			container.Resolve<Class2>();
		}
		
		[Test]
		public void LongChainThrow()
		{
			var container = new CryoContainer {OnCircularDependency = Reaction.ThrowException};
			container.RegisterSingleton<Class1>();
			container.RegisterSingleton<Class2>();
			container.RegisterSingleton<Class3>();
			container.RegisterSingleton<Class4>();

			try
			{
				container.Resolve<Class3>();
				Assert.Fail("Exception expected");
			}
			catch (CircularDependencyException)
			{
				
			}
		}
	}
}

#endif
