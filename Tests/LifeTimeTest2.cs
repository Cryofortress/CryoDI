using System.Collections;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CryoDI.Tests
{
	public class LifeTimeTest2 : MonoBehaviour
	{
		private static int _loadCounter = 0;
		private void Awake()
		{
			Debug.Log("---------------------------------------");
			Debug.Log("Scene 2 Awake");
			++_loadCounter;
			if (_loadCounter == 1)
			{
				CheckSingletonesInScene2();
				CheckTypesInScene2();
				CheckInstancesInScene2();
				
				StartCoroutine(LoadSceneIn2Sec("LifetimeTest2"));
			}
			else
			{
				CheckSingletonesInScene3();
				LifetimeTest.Container.Dispose();

				CheckGlobalObjectsDisposed();
				Debug.Log("---------------------------------------");
				Debug.Log("Test finished");	
			}
		}
		
		private IEnumerator LoadSceneIn2Sec(string sceneName)
		{
			yield return new WaitForSeconds(2);
			
			Debug.Log("Loading scene " + sceneName);
			SceneManager.LoadScene(sceneName);
		}
		
		private void CheckSingletonesInScene2()
        {
            Debug.Log("Checking singletones #2");

			var container = LifetimeTest.Container;
            Assert.AreEqual(1, container.ResolveByName<SingletonCounter>(LifetimeTest.GlobalSingleton).InstanceId, "Wrong InstanceId of GlobalSingleton");
            Assert.AreEqual(4, container.ResolveByName<SingletonCounter>(LifetimeTest.SceneSingleton).InstanceId, "Wrong InstanceId of SceneSingleton");
            Assert.AreEqual(3, container.ResolveByName<SingletonCounter>(LifetimeTest.ExternalSingleton).InstanceId, "Wrong InstanceId of ExternalSingleton");

            Assert.AreEqual(4, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(1, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");
        }

        private void CheckTypesInScene2()
        {
            Debug.Log("Checking types #2");
			var container = LifetimeTest.Container;
            Assert.AreEqual(6, TypeCounter.CreatedCount);
            Assert.AreEqual(2, TypeCounter.DisposedCount);
            
            
            Assert.AreEqual(7, container.ResolveByName<TypeCounter>(LifetimeTest.GlobalType).InstanceId);
            Assert.AreEqual(8, container.ResolveByName<TypeCounter>(LifetimeTest.SceneType).InstanceId);
            Assert.AreEqual(9, container.ResolveByName<TypeCounter>(LifetimeTest.ExternalType).InstanceId);
            
            Assert.AreEqual(9, TypeCounter.CreatedCount);
            Assert.AreEqual(2, TypeCounter.DisposedCount);
        }
        
        private void CheckInstancesInScene2()
        {
            Debug.Log("Checking instances #2");
			var container = LifetimeTest.Container;
            Assert.AreEqual(1, container.ResolveByName<InstanceCounter>(LifetimeTest.GlobalInstance).InstanceId);

            try
            {
                container.ResolveByName<InstanceCounter>(LifetimeTest.SceneInstance);
                Assert.IsTrue(false, "Expected exception was not thrown");
            }
            catch (ContainerException)
            {
                // expected exception
                Debug.Log("Got expected exception");
            }
            
            Assert.AreEqual(3, container.ResolveByName<InstanceCounter>(LifetimeTest.ExternalInstance).InstanceId);

            Assert.AreEqual(3, InstanceCounter.CreatedCount);
            Assert.AreEqual(1, InstanceCounter.DisposedCount, "Instance was not disposed");
        }
        
		private void CheckGlobalObjectsDisposed()
		{
			Debug.Log("Checking are global objects disposed");
			Assert.AreEqual(5, SingletonCounter.CreatedCount, "Wrong number of created singletones");
			Assert.AreEqual(4, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");            
            
			Assert.AreEqual(9, TypeCounter.CreatedCount);
			Assert.AreEqual(6, TypeCounter.DisposedCount, "Wrong number of disposed objects");
            
			Assert.AreEqual(3, InstanceCounter.CreatedCount);
			Assert.AreEqual(2, InstanceCounter.DisposedCount, "Instance was not disposed");
		}
		
		private void CheckSingletonesInScene3()
		{
			Debug.Log("Checking singletones #3");
			
			var container = LifetimeTest.Container;
			Assert.AreEqual(1, container.ResolveByName<SingletonCounter>(LifetimeTest.GlobalSingleton).InstanceId, "Wrong InstanceId of GlobalSingleton");
			Assert.AreEqual(5, container.ResolveByName<SingletonCounter>(LifetimeTest.SceneSingleton).InstanceId, "Wrong InstanceId of SceneSingleton");
			Assert.AreEqual(3, container.ResolveByName<SingletonCounter>(LifetimeTest.ExternalSingleton).InstanceId, "Wrong InstanceId of ExternalSingleton");
            
			Assert.AreEqual(5, SingletonCounter.CreatedCount, "Wrong number of created singletones");
			Assert.AreEqual(2, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");
		}
	}
}