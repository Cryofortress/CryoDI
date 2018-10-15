using System;
using System.IO;
using NUnit.Framework;

namespace CryoDI.Tests
{
	[TestFixture]
	public class TestParameters
	{
		private class ClassWithParams
		{
			[Param]
			public int IntParam { get; private set; }
			
			[Param]
			public float FloatParam { get; private set; }
			
			[Param]
			public DateTime DateTimeParam { get; private set; }
			
			[Param]
			public IDisposable DisposableParam { get; private set; }
		}
		
		private class ClassWithSimilarParams
		{
			[Param]
			public int One { get; private set; }
			
			[Param]
			public int Two { get; private set; }
									
			[Param]
			public string Str { get; private set; }
		}
		
		[Test]
		public void TestResolveFailed()
		{
			var container = new CryoContainer();
			container.RegisterType<ClassWithParams>();

			try
			{
				container.Resolve<ClassWithParams>();
				Assert.Fail("Expected exception not happened");
			}
			catch (ContainerException)
			{
			}
		}
		
		[Test]
		public void TestBuildUpFailed()
		{
			var container = new CryoContainer();

			try
			{
				var val = new ClassWithParams();
				container.BuildUp(val);
				Assert.Fail("Expected exception not happened");
			}
			catch (ContainerException)
			{
			}
		}
		
		[Test]
		public void TestBuildUp()
		{
			var container = new CryoContainer();

			var now = DateTime.Now;
			var stream = new MemoryStream();
			
			var val = new ClassWithParams();
			container.BuildUp(val, stream, now, 3, 1.12f);
			
			Assert.AreEqual(now, val.DateTimeParam);
			Assert.AreEqual(stream, val.DisposableParam);
			Assert.AreEqual(3, val.IntParam);
			Assert.AreEqual(1.12f, val.FloatParam, 0.000001);
		}
		
		[Test]
		public void TestBuildUpSimilarParams()
		{
			var container = new CryoContainer();
			
			var val = new ClassWithSimilarParams();
			
			container.BuildUp(val, new Param { Name = "Two", Value = 2}, "Hello, world!", new Param("One", 1));
			Assert.AreEqual(1, val.One);
			Assert.AreEqual(2, val.Two);
			Assert.AreEqual("Hello, world!", val.Str);
		}
		
		[Test]
		public void TestBuildUpSimilarParamsFailed()
		{
			var container = new CryoContainer();
			
			var val = new ClassWithSimilarParams();
			try
			{
				container.BuildUp(val, 1, 2, "Hello, world!");
				Assert.Fail("Expected exception not happened");
			}
			catch (ContainerException)
			{
				
			}
		}
		
		[Test]
		public void TestBuildUpSimilarParamsFailed2()
		{
			var container = new CryoContainer();
			
			var val = new ClassWithSimilarParams();
			try
			{
				container.BuildUp(val, 1, "Hello, world!", 2);
				Assert.Fail("Expected exception not happened");
			}
			catch (ContainerException)
			{
				
			}
		}
		
		[Test]
		public void TestResolve()
		{
			var container = new CryoContainer();
			container.RegisterType<ClassWithParams>();

			var now = DateTime.Now;
			var stream = new MemoryStream();
			
			var val = container.Resolve<ClassWithParams>(stream, now, 3, 1.12f);
			
			Assert.AreEqual(now, val.DateTimeParam);
			Assert.AreEqual(stream, val.DisposableParam);
			Assert.AreEqual(3, val.IntParam);
			Assert.AreEqual(1.12f, val.FloatParam, 0.000001);
		}
		
		[Test]
		public void TestResolveSimilarParams()
		{
			var container = new CryoContainer();
			container.RegisterType<ClassWithSimilarParams>();

			var val = container.Resolve<ClassWithSimilarParams>(new Param { Name = "Two", Value = 2}, "Hello, world!", new Param("One", 1));
			Assert.AreEqual(1, val.One);
			Assert.AreEqual(2, val.Two);
			Assert.AreEqual("Hello, world!", val.Str);
		}
		
		[Test]
		public void TestResolveSimilarParamsFailed()
		{
			var container = new CryoContainer();
			container.RegisterType<ClassWithSimilarParams>();
			
			try
			{
				container.Resolve<ClassWithSimilarParams>(1, 2, "Hello, world!");
				Assert.Fail("Expected exception not happened");
			}
			catch (ContainerException)
			{
				
			}
		}
		
		[Test]
		public void TestResolveSimilarParamsFailed2()
		{
			var container = new CryoContainer();
			container.RegisterType<ClassWithSimilarParams>();
			
			try
			{
				container.Resolve<ClassWithSimilarParams>(1, "Hello, world!", 2);
				Assert.Fail("Expected exception not happened");
			}
			catch (ContainerException)
			{
				
			}
		}
	}
}