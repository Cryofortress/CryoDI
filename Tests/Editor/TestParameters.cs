using System;
using System.IO;
using NUnit.Framework;

namespace CryoDI.Tests
{
	[TestFixture]
	public class TestParameters
	{
		private class ManyParamsClass
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
		
		[Test]
		public void TestResolveFailed()
		{
			var container = new CryoContainer();
			container.RegisterType<ManyParamsClass>();

			try
			{
				var val = container.Resolve<ManyParamsClass>();
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
				var val = new ManyParamsClass();
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
			
			var val = new ManyParamsClass();
			container.BuildUp(val, stream, now, 3, 1.12f);
			
			Assert.AreEqual(now, val.DateTimeParam);
			Assert.AreEqual(stream, val.DisposableParam);
			Assert.AreEqual(3, val.IntParam);
			Assert.AreEqual(1.12f, val.FloatParam, 0.000001);
		}
		
		[Test]
		public void TestResolve()
		{
			var container = new CryoContainer();
			container.RegisterType<ManyParamsClass>();

			var now = DateTime.Now;
			var stream = new MemoryStream();
			
			var val = container.Resolve<ManyParamsClass>(stream, now, 3, 1.12f);
			
			Assert.AreEqual(now, val.DateTimeParam);
			Assert.AreEqual(stream, val.DisposableParam);
			Assert.AreEqual(3, val.IntParam);
			Assert.AreEqual(1.12f, val.FloatParam, 0.000001);
		}
	}
}