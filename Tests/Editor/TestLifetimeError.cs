using NUnit.Framework;

namespace CryoDI.Tests
{
	public class MySingleton
	{
	}

	public class MyInstance
	{
		[Dependency]
		private MySingleton MySingleton { get; set; }
	}
	
	[TestFixture]
	public class TestLifetimeError
	{
		
		
		[Test] 
		public void GlobalInExternal()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.Global);
			container.RegisterType<MyInstance>(LifeTime.External);
			container.Resolve<MyInstance>();
		}

		[Test] 
		public void GlobalInGlobal()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.Global);
			container.RegisterType<MyInstance>(LifeTime.Global);
			container.Resolve<MyInstance>();
		}
		
		[Test]
		public void GlobalInScene()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.Global);
			container.RegisterType<MyInstance>(LifeTime.Scene);
			container.Resolve<MyInstance>();
		}
		
		
		[Test]
		public void ExternalInExternal()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.External);
			container.RegisterType<MyInstance>(LifeTime.External);
			container.Resolve<MyInstance>();
		}
		
		// Мы не можем предсказать время жизни external объектов. 
		// Оно может быть как больше времени жизни контейнера, так и меньше сцены
		[Test] 
		public void ExternalInGlobal()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.External);
			container.RegisterType<MyInstance>(LifeTime.Global);
			container.Resolve<MyInstance>();
		}
		
		// Мы не можем предсказать время жизни external объектов. 
		// Оно может быть как больше времени жизни контейнера, так и меньше сцены
		[Test]
		public void ExternalInScene()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.External);
			container.RegisterType<MyInstance>(LifeTime.Scene);
			container.Resolve<MyInstance>();
		}

		[Test]
		public void SceneInExternal()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.Scene);
			container.RegisterType<MyInstance>(LifeTime.External);
			container.Resolve<MyInstance>();
		}		
		
		[Test]
		public void SceneInGlobal()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.Scene);
			container.RegisterType<MyInstance>(LifeTime.Global);

			try
			{
				container.Resolve<MyInstance>();
				Assert.Fail("Exception expected");
			}
			catch (WrongLifetimeException)
			{
				// expected
			}
		}
		
		[Test]
		public void SceneInScene()
		{
			var container = new CryoContainer {OnLifetimeError = Reaction.ThrowException};
			container.RegisterSingleton<MySingleton>(LifeTime.Scene);
			container.RegisterType<MyInstance>(LifeTime.Scene);
			container.Resolve<MyInstance>();
		}
	}
}