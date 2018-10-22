using NUnit.Framework;

namespace CryoDI.Tests
{
	class ClassBase
	{
		[Dependency("PrivateBase")]
		private string PrivateProperty { get; set; }
		[Dependency("ProtectedBase")]
		protected string ProtectedProperty { get; set; }
		[Dependency("PublicBase")]
		public string PublicProperty { get; set; }

		public virtual void Check()
		{
			Assert.AreEqual("private base", PrivateProperty);
			Assert.AreEqual("protected base", ProtectedProperty);
			Assert.AreEqual("public base", PublicProperty);
		}
	}

	class ClassDerived : ClassBase
	{
		[Dependency("PrivateDerived")]
		private string PrivateProperty { get; set; }
		[Dependency("ProtectedDerived")]
		protected new string ProtectedProperty { get; set; }
		[Dependency("PublicDerived")]
		public new string PublicProperty { get; set; }

		public override void Check()
		{
			base.Check();
			
			Assert.AreEqual("private derived", PrivateProperty);
			Assert.AreEqual("protected derived", ProtectedProperty);
			Assert.AreEqual("public derived", PublicProperty);
		}
	}	
		
	[TestFixture]
	public class TestNonPublicProperties
	{
		[Test]
		public void TestSimplePropertiesBuildup()
		{
			var container = new CryoContainer();
			container.RegisterInstance("private base", "PrivateBase");
			container.RegisterInstance("protected base", "ProtectedBase");
			container.RegisterInstance("public base", "PublicBase");

			var a = new ClassBase();
			container.BuildUp(a);
			a.Check();
		}
		
		[Test]
		public void TestDerivedPropertiesBuildup()
		{
			var container = new CryoContainer();
			container.RegisterInstance("private base", "PrivateBase");
			container.RegisterInstance("protected base", "ProtectedBase");
			container.RegisterInstance("public base", "PublicBase");
			
			container.RegisterInstance("private derived", "PrivateDerived");
			container.RegisterInstance("protected derived", "ProtectedDerived");
			container.RegisterInstance("public derived", "PublicDerived");
			
			var a = new ClassDerived();
			container.BuildUp(a);
			a.Check();
		}
		
		[Test]
		public void TestSimplePropertiesResolve()
		{
			var container = new CryoContainer();
			container.RegisterInstance("private base", "PrivateBase");
			container.RegisterInstance("protected base", "ProtectedBase");
			container.RegisterInstance("public base", "PublicBase");
			container.RegisterType<ClassBase>();


			var a = container.Resolve<ClassBase>();
			a.Check();
		}
		
		[Test]
		public void TestDerivedPropertiesResolve()
		{
			var container = new CryoContainer();
			container.RegisterInstance("private base", "PrivateBase");
			container.RegisterInstance("protected base", "ProtectedBase");
			container.RegisterInstance("public base", "PublicBase");
			
			container.RegisterInstance("private derived", "PrivateDerived");
			container.RegisterInstance("protected derived", "ProtectedDerived");
			container.RegisterInstance("public derived", "PublicDerived");
			container.RegisterType<ClassDerived>();

			var a = container.Resolve<ClassDerived>();
			a.Check();
		}
	}
}