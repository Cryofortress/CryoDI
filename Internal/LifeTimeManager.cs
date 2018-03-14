using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;

namespace CryoDI
{
    internal static class LifeTimeManager
    {
        private class DisposablesCollection
        {
	        private readonly List<IDisposable> _disposables = new List<IDisposable>();
	        
            public string SceneName { get; private set; }

	        public DisposablesCollection(string sceneName)
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

        private static List<DisposablesCollection> _storage;
	    private static DisposablesCollection _lastUsedColl;
	    private static readonly List<IDisposable> _global = new List<IDisposable>();

        public static void TryToAdd(object obj, LifeTime lifeTime)
        {
	        if (lifeTime == LifeTime.External) return;
            var disposable = obj as IDisposable;
	        if (disposable == null) return;
	        
	        if (lifeTime == LifeTime.Scene)
		        Add(disposable);
	        else
	        	_global.Add(disposable);
        }

	    public static void DisposeAll()
	    {
		    // среди этого списка может быть ссылка на сам контейнер. Поэтому создаем временную копию
		    var collectionsToDispose = _storage.ToArray();
		    _storage.Clear();

		    var objectsToDispose = _global.ToArray();
		    _global.Clear();
		    
		    foreach (var collection in collectionsToDispose)
		    {
			    collection.Dispose();
		    }
		    
		    foreach (var disposable in objectsToDispose)
		    {
			    disposable.Dispose();
		    }
	    }

	    private static void Add(IDisposable disposable)
        {
            GetCurCollection().Add(disposable);
        }

        private static DisposablesCollection GetCurCollection()
        {
            if (_storage == null)
		        CreateStorage();

	        var activeSceneName = SceneManager.GetActiveScene().name;
	        if (_lastUsedColl != null && _lastUsedColl.SceneName == activeSceneName)
		        return _lastUsedColl;
	        
	        var coll = _storage.FirstOrDefault(o => o.SceneName == activeSceneName);
			if (coll == null)
			{
				coll = new DisposablesCollection(activeSceneName);
				_storage.Add(coll);
			}
	        _lastUsedColl = coll;
            return coll;
        }

	    private static void CreateStorage()
	    {
		    _storage = new List<DisposablesCollection>();
		    SceneManager.sceneUnloaded += OnSceneUnloaded;
	    }

	    private static void OnSceneUnloaded(Scene scene)
        {
            var coll = _storage.FirstOrDefault(o => o.SceneName == SceneManager.GetActiveScene().name);
            if (coll != null)
            {
                coll.Dispose();
                _storage.Remove(coll);
            }
	        if (Object.ReferenceEquals(_lastUsedColl, coll))
		        _lastUsedColl = null;
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