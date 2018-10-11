#if UNITY_5_3_OR_NEWER
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace CryoDI.Tests
{
    public class LifetimeTestStarter : MonoBehaviour
    {
        private CryoContainer _container;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            //Assert.raiseExceptions = true;

            _container = new CryoContainer();
            _container.RegisterSingleton<SingletonCounter>("GlobalSingleton", LifeTime.Global);
            _container.RegisterSingleton<SingletonCounter>("SceneSingleton", LifeTime.Scene);
            _container.RegisterSingleton<SingletonCounter>("ExternalSingleton", LifeTime.External);
            
            _container.RegisterType<TypeCounter>("GlobalType", LifeTime.Global);
            _container.RegisterType<TypeCounter>("SceneType", LifeTime.Scene);
            _container.RegisterType<TypeCounter>("ExternalType", LifeTime.External);

            _container.RegisterInstance(new InstanceCounter(), "GlobalInstance", LifeTime.Global);
            _container.RegisterInstance(new InstanceCounter(), "SceneInstance", LifeTime.Scene);
            _container.RegisterInstance(new InstanceCounter(), "ExternalInstance", LifeTime.External);

            CheckSingletones1();
            CheckTypes1();
            CheckInstances1();

            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.LoadScene("LifetimeTest2");
        }
        
        private void CheckSingletones1()
        {
            Debug.Log("Checking singletones #1");
            Assert.AreEqual(1, _container.ResolveByName<SingletonCounter>("GlobalSingleton").InstanceId, "Wrong InstanceId of GlobalSingleton");
            Assert.AreEqual(2, _container.ResolveByName<SingletonCounter>("SceneSingleton").InstanceId, "Wrong InstanceId of SceneSingleton");
            Assert.AreEqual(3, _container.ResolveByName<SingletonCounter>("ExternalSingleton").InstanceId, "Wrong InstanceId of ExternalSingleton");
            
            Assert.AreEqual(3, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(0, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");

            Assert.AreEqual(1, _container.ResolveByName<SingletonCounter>("GlobalSingleton").InstanceId, "Wrong InstanceId of GlobalSingleton");
            Assert.AreEqual(2, _container.ResolveByName<SingletonCounter>("SceneSingleton").InstanceId, "Wrong InstanceId of SceneSingleton");
            Assert.AreEqual(3, _container.ResolveByName<SingletonCounter>("ExternalSingleton").InstanceId, "Wrong InstanceId of ExternalSingleton");
                
            Assert.AreEqual(3, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(0, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");
        }
        
        private void CheckTypes1()
        {
            Debug.Log("Checking types #1");
            Assert.AreEqual(1, _container.ResolveByName<TypeCounter>("GlobalType").InstanceId);
            Assert.AreEqual(2, _container.ResolveByName<TypeCounter>("SceneType").InstanceId);
            Assert.AreEqual(3, _container.ResolveByName<TypeCounter>("ExternalType").InstanceId);
            
            Assert.AreEqual(3, TypeCounter.CreatedCount);
            Assert.AreEqual(0, TypeCounter.DisposedCount);
            
            Assert.AreEqual(4, _container.ResolveByName<TypeCounter>("GlobalType").InstanceId);
            Assert.AreEqual(5, _container.ResolveByName<TypeCounter>("SceneType").InstanceId);
            Assert.AreEqual(6, _container.ResolveByName<TypeCounter>("ExternalType").InstanceId);
            
            Assert.AreEqual(6, TypeCounter.CreatedCount);
            Assert.AreEqual(0, TypeCounter.DisposedCount);
        }

        private void CheckInstances1()
        {
            Debug.Log("Checking instances #1");
            Assert.AreEqual(1, _container.ResolveByName<InstanceCounter>("GlobalInstance").InstanceId);
            Assert.AreEqual(2, _container.ResolveByName<InstanceCounter>("SceneInstance").InstanceId);
            Assert.AreEqual(3, _container.ResolveByName<InstanceCounter>("ExternalInstance").InstanceId);
            
            Assert.AreEqual(3, InstanceCounter.CreatedCount);
            Assert.AreEqual(0, InstanceCounter.DisposedCount);
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            CheckSingletones2();
            CheckTypes2();
            CheckInstances2();

            _container.Dispose();

            CheckGlobalObjectsDisposed();
            Debug.Log("Test finished");
        }

        private void CheckSingletones2()
        {
            Debug.Log("Checking singletones #2");

            Assert.AreEqual(1, _container.ResolveByName<SingletonCounter>("GlobalSingleton").InstanceId, "Wrong InstanceId of GlobalSingleton");
            Assert.AreEqual(4, _container.ResolveByName<SingletonCounter>("SceneSingleton").InstanceId, "Wrong InstanceId of SceneSingleton");
            Assert.AreEqual(3, _container.ResolveByName<SingletonCounter>("ExternalSingleton").InstanceId, "Wrong InstanceId of ExternalSingleton");

            Assert.AreEqual(4, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(1, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");
        }

        private void CheckTypes2()
        {
            Debug.Log("Checking types #2");
            Assert.AreEqual(6, TypeCounter.CreatedCount);
            Assert.AreEqual(2, TypeCounter.DisposedCount);
            
            
            Assert.AreEqual(7, _container.ResolveByName<TypeCounter>("GlobalType").InstanceId);
            Assert.AreEqual(8, _container.ResolveByName<TypeCounter>("SceneType").InstanceId);
            Assert.AreEqual(9, _container.ResolveByName<TypeCounter>("ExternalType").InstanceId);
            
            Assert.AreEqual(9, TypeCounter.CreatedCount);
            Assert.AreEqual(2, TypeCounter.DisposedCount);
        }
        
        private void CheckInstances2()
        {
            Debug.Log("Checking instances #2");
            Assert.AreEqual(1, _container.ResolveByName<InstanceCounter>("GlobalInstance").InstanceId);

            try
            {
                _container.ResolveByName<InstanceCounter>("SceneInstance");
                Assert.IsTrue(false, "Expected exception was not thrown");
            }
            catch (ContainerException)
            {
                // expected exception
                Debug.Log("Got expected exception");
            }
            
            Assert.AreEqual(3, _container.ResolveByName<InstanceCounter>("ExternalInstance").InstanceId);

            Assert.AreEqual(3, InstanceCounter.CreatedCount);
            Assert.AreEqual(1, InstanceCounter.DisposedCount, "Instance was not disposed");
        }
        
        private void CheckGlobalObjectsDisposed()
        {
            Debug.Log("Checking are global objects disposed");
            Assert.AreEqual(4, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(3, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");
            
            
            Assert.AreEqual(9, TypeCounter.CreatedCount);
            Assert.AreEqual(6, TypeCounter.DisposedCount, "Wrong number of disposed objects");
            
            Assert.AreEqual(3, InstanceCounter.CreatedCount);
            Assert.AreEqual(2, InstanceCounter.DisposedCount, "Instance was not disposed");
        }
    }
}

#endif