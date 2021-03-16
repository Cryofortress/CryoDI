using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;

namespace CryoDI
{
    internal class LifeTimeManager : ILifeTimeManager
    {
        private readonly Dictionary<int, Entry> _entries = new Dictionary<int, Entry>();
	    private readonly List<IDisposable> _global = new List<IDisposable>();

		public LifeTimeManager()
		{
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

        public void Add(IDisposable disposable, LifeTime lifeTime)
        {
	        if (lifeTime == LifeTime.External) return;
	        
	        if (lifeTime == LifeTime.Scene)
		        AddSceneDisposable(disposable);
	        else
	        	_global.Add(disposable);
        }

	    public void DisposeAll()
	    {
		    // среди этих списков может быть ссылка на сам контейнер. Поэтому создаем временную копию
		    Entry[] entries = null;

		    if (_entries != null)
		    {
			    entries = _entries.Values.ToArray();
			    _entries.Clear();
		    }

		    var globalsToDispose = _global.ToArray();
		    _global.Clear();

		    if (entries != null)
		    {
			    foreach (var entry in entries)
			    {
				    entry.Dispose();
			    }
		    }

		    foreach (var disposable in globalsToDispose)
		    {
			    disposable.Dispose();
		    }
	    }

	    private void AddSceneDisposable(IDisposable disposable)
		{
			var entry = GetCurEntry();
			entry.Add(disposable);
		}

        private Entry GetCurEntry()
        {
			var activeScene = SceneManager.GetActiveScene();

			Entry entry;
			if (!_entries.TryGetValue(activeScene.handle, out entry))
			{
				entry = new Entry(activeScene.name);
				_entries.Add(activeScene.handle, entry);
			}
            return entry;
        }

	    private void OnSceneUnloaded(Scene scene)
        {
			DILog.Log($"[LifetimeManager] Scene unloaded: {scene.name} ({scene.handle})");
			if (_entries.TryGetValue(scene.handle, out var entry))
			{
				entry.Dispose();
				_entries.Remove(scene.handle);
			}
        }
	    
		private class Entry
		{
			private readonly List<IDisposable> _disposables = new List<IDisposable>();
	        
			public string SceneName { get; private set; }

			public Entry(string sceneName)
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
				_disposables.Clear();
			}
		}
    }
}
#else

namespace CryoDI
{
	internal class LifeTimeManager : ILifeTimeManager
	{
		private readonly List<IDisposable> _global = new List<IDisposable>();

		public void Add(IDisposable disposable, LifeTime lifeTime)
		{
			if (lifeTime == LifeTime.External) return;
			
			if (disposable != null)
				_global.Add(disposable);
		}

		public void DisposeAll()
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