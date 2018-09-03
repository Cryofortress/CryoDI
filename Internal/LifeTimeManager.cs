using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;

namespace CryoDI
{
    internal static class LifeTimeManager
    {
        private class SceneDisposables
        {
	        private readonly List<IDisposable> _disposables = new List<IDisposable>();
	        
            public string SceneName { get; private set; }

	        public SceneDisposables(string sceneName)
	        {
		        SceneName = sceneName;
	        }
	        
	        public void Add(IDisposable disposable)
	        {
		        _disposables.Add(disposable);
	        }

            public void Dispose()
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
        }

        private static List<SceneDisposables> _scenes;
	    private static SceneDisposables _lastScene;
	    private static readonly List<IDisposable> _global = new List<IDisposable>();

        public static void TryToAdd(object obj, LifeTime lifeTime)
        {
	        if (lifeTime == LifeTime.External) return;
	        
            var disposable = obj as IDisposable;
	        if (disposable == null) return;
	        
	        if (lifeTime == LifeTime.Scene)
		        AddSceneDisposable(disposable);
	        else
	        	_global.Add(disposable);
        }

	    public static void DisposeAll()
	    {
		    // среди этих списков может быть ссылка на сам контейнер. Поэтому создаем временную копию
		    
		    SceneDisposables[] scenesToDispose = null;

		    if (_scenes != null)
		    {
			    scenesToDispose = _scenes.ToArray();
			    _scenes.Clear();
		    }

		    var globalsToDispose = _global.ToArray();
		    _global.Clear();

		    if (scenesToDispose != null)
		    {
			    foreach (var scene in scenesToDispose)
			    {
				    scene.Dispose();
			    }
		    }

		    foreach (var disposable in globalsToDispose)
		    {
			    disposable.Dispose();
		    }
	    }

	    private static void AddSceneDisposable(IDisposable disposable)
        {
            GetCurScene().Add(disposable);
        }

        private static SceneDisposables GetCurScene()
        {
            if (_scenes == null)
		        Init();

	        var activeSceneName = SceneManager.GetActiveScene().name;
	        if (_lastScene != null && _lastScene.SceneName == activeSceneName)
		        return _lastScene;
	        
	        var coll = _scenes.FirstOrDefault(o => o.SceneName == activeSceneName);
			if (coll == null)
			{
				coll = new SceneDisposables(activeSceneName);
				_scenes.Add(coll);
			}
	        _lastScene = coll;
            return coll;
        }

	    private static void Init()
	    {
		    _scenes = new List<SceneDisposables>();
		    SceneManager.sceneUnloaded += OnSceneUnloaded;
	    }

	    private static void OnSceneUnloaded(Scene scene)
        {
            var coll = _scenes.FirstOrDefault(o => o.SceneName == SceneManager.GetActiveScene().name);
            if (coll != null)
            {
                coll.Dispose();
                _scenes.Remove(coll);
            }
	        if (Object.ReferenceEquals(_lastScene, coll))
		        _lastScene = null;
        }
    }
}
#else

namespace CryoDI
{
	internal static class LifeTimeManager
	{
		private static readonly List<IDisposable> _global = new List<IDisposable>();

		public static void TryToAdd(object obj, LifeTime lifeTime)
		{
			if (lifeTime == LifeTime.External) return;
			
			var disposable = obj as IDisposable;
			if (disposable != null)
				_global.Add(disposable);
		}

		public static void DisposeAll()
		{
			// среди этого списка может быть ссылка на сам контейнер. Поэтому создаем временную копию
			var objectsToDispose = _global.ToArray();
			_global.Clear();
			
			foreach (var disposable in objectsToDispose)
			{
				disposable.Dispose();
			}
		}
	}
}

#endif