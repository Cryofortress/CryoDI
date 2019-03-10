#if UNITY_5_3_OR_NEWER
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CryoDI.Tests
{
    public class LifetimeTest1 : MonoBehaviour
    {	
        private void Awake()
        {
			Debug.Log("---------------------------------------");
			Debug.Log("Scene 1 Awake");
            CheckSingletonesInScene1();
            CheckTypesInScene1();
            CheckInstancesInScene1();

            StartCoroutine(LoadSceneIn2Sec("LifetimeTest2"));
        }
        
        private void CheckSingletonesInScene1()
        {
			Debug.Log("Checking singletones #1");

			var container = LifetimeTest.Container;
            Assert.AreEqual(1, container.ResolveByName<SingletonCounter>(LifetimeTest.GlobalSingleton).InstanceId, "Wrong InstanceId of GlobalSingleton");
            Assert.AreEqual(2, container.ResolveByName<SingletonCounter>(LifetimeTest.SceneSingleton).InstanceId, "Wrong InstanceId of SceneSingleton");
            Assert.AreEqual(3, container.ResolveByName<SingletonCounter>(LifetimeTest.ExternalSingleton).InstanceId, "Wrong InstanceId of ExternalSingleton");
            
            Assert.AreEqual(3, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(0, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");

            Assert.AreEqual(1, container.ResolveByName<SingletonCounter>(LifetimeTest.GlobalSingleton).InstanceId, "Wrong InstanceId of GlobalSingleton");
            Assert.AreEqual(2, container.ResolveByName<SingletonCounter>(LifetimeTest.SceneSingleton).InstanceId, "Wrong InstanceId of SceneSingleton");
            Assert.AreEqual(3, container.ResolveByName<SingletonCounter>(LifetimeTest.ExternalSingleton).InstanceId, "Wrong InstanceId of ExternalSingleton");
                
            Assert.AreEqual(3, SingletonCounter.CreatedCount, "Wrong number of created singletones");
            Assert.AreEqual(0, SingletonCounter.DisposedCount, "Wrong number of disposed singletones");
        }
        
        private void CheckTypesInScene1()
        {
			Debug.Log("Checking types #1");
			var container = LifetimeTest.Container;
            Assert.AreEqual(1, container.ResolveByName<TypeCounter>("GlobalType").InstanceId);
            Assert.AreEqual(2, container.ResolveByName<TypeCounter>(LifetimeTest.SceneType).InstanceId);
            Assert.AreEqual(3, container.ResolveByName<TypeCounter>(LifetimeTest.ExternalType).InstanceId);
            
            Assert.AreEqual(3, TypeCounter.CreatedCount);
            Assert.AreEqual(0, TypeCounter.DisposedCount);
            
            Assert.AreEqual(4, container.ResolveByName<TypeCounter>("GlobalType").InstanceId);
            Assert.AreEqual(5, container.ResolveByName<TypeCounter>(LifetimeTest.SceneType).InstanceId);
            Assert.AreEqual(6, container.ResolveByName<TypeCounter>(LifetimeTest.ExternalType).InstanceId);
            
            Assert.AreEqual(6, TypeCounter.CreatedCount);
            Assert.AreEqual(0, TypeCounter.DisposedCount);
        }

        private void CheckInstancesInScene1()
        {
			Debug.Log("Checking instances #1");
			var container = LifetimeTest.Container;
            Assert.AreEqual(1, container.ResolveByName<InstanceCounter>(LifetimeTest.GlobalInstance).InstanceId);
            Assert.AreEqual(2, container.ResolveByName<InstanceCounter>(LifetimeTest.SceneInstance).InstanceId);
            Assert.AreEqual(3, container.ResolveByName<InstanceCounter>(LifetimeTest.ExternalInstance).InstanceId);
            
            Assert.AreEqual(3, InstanceCounter.CreatedCount);
            Assert.AreEqual(0, InstanceCounter.DisposedCount);
        }
        
		private IEnumerator LoadSceneIn2Sec(string sceneName)
		{
			yield return new WaitForSeconds(2);
			
			Debug.Log("Loading scene " + sceneName);
			SceneManager.LoadScene(sceneName);
		}
    }
}

#endif