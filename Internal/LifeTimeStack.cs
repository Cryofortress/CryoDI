using System.Collections.Generic;

namespace CryoDI
{
	internal static class LifeTimeStack
	{
		private static List<Entry> _stack = new List<Entry>();

		private class Entry
		{
			public ContainerKey Key { get; set; }
			public LifeTime LifeTime { get; set; }
		}

		public static void Push(ContainerKey key, LifeTime lifetime)
		{
			_stack.Add(new Entry
			{
				Key = key,
				LifeTime = lifetime
			});

			CheckLifeTime();
		}

		public static void Pop()
		{
			if (_stack.Count == 0)
				throw new ContainerException("Unexpected state: stack is empty");
			_stack.RemoveAt(_stack.Count - 1);
		}

		private static void CheckLifeTime()
		{
			if (_stack.Count <= 1)
				return;

			var parent = _stack[_stack.Count - 2];
			var child = _stack[_stack.Count - 1];
			if (parent.LifeTime == LifeTime.Global && child.LifeTime == LifeTime.Scene)
			{
				var message = "You are trying to inject the object with lifetime = Scene {" +
					child.Key + "} into object with lifetime = Global {" + parent.Key +"}";
				DILog.LogError(message);
			}
		}
	}
}