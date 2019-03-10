namespace CryoDI.Tests
{
	public class LifetimeTest
	{
		public const string GlobalSingleton = "GlobalSingleton";
		public const string SceneSingleton = "SceneSingleton";
		public const string ExternalSingleton = "ExternalSingleton";
		public const string GlobalType = "GlobalType";
		public const string SceneType = "SceneType";
		public const string ExternalType = "ExternalType";
		public const string GlobalInstance = "GlobalInstance";
		public const string SceneInstance = "SceneInstance";
		public const string ExternalInstance = "ExternalInstance";
		
		private static CryoContainer _container;
		
		public static CryoContainer Container
		{
			get
			{
				if (_container == null)
					CreateContainer();
				return _container;
			}
		}

		private static void CreateContainer()
		{
			_container = new CryoContainer();
			_container.RegisterSingleton<SingletonCounter>(GlobalSingleton, LifeTime.Global);
			_container.RegisterSingleton<SingletonCounter>(SceneSingleton, LifeTime.Scene);
			_container.RegisterSingleton<SingletonCounter>(ExternalSingleton, LifeTime.External);
            
			_container.RegisterType<TypeCounter>(GlobalType, LifeTime.Global);
			_container.RegisterType<TypeCounter>(SceneType, LifeTime.Scene);
			_container.RegisterType<TypeCounter>(ExternalType, LifeTime.External);

			_container.RegisterInstance(new InstanceCounter(), GlobalInstance, LifeTime.Global);
			_container.RegisterInstance(new InstanceCounter(), SceneInstance, LifeTime.Scene);
			_container.RegisterInstance(new InstanceCounter(), ExternalInstance, LifeTime.External);
		}
	}
}