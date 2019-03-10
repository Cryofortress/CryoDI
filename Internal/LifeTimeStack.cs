using System.Collections.Generic;

namespace CryoDI
{
	internal class LifeTimeStack
	{
		private readonly List<Entry> _stack = new List<Entry>();

		private class Entry
		{
			public ContainerKey Key { get; set; }
			public LifeTime LifeTime { get; set; }
		}

		public LifeTimeStack()
		{
			OnLifetimeError = Reaction.LogError;
		}

		public void Push(ContainerKey key, LifeTime lifetime)
		{
			_stack.Add(new Entry
			{
				Key = key,
				LifeTime = lifetime
			});

			CheckLifeTime();
		}

		public void Pop()
		{
			if (_stack.Count == 0)
				throw new ContainerException("Unexpected state: stack is empty");
			_stack.RemoveAt(_stack.Count - 1);
		}
		
		public Reaction OnLifetimeError { get; set; }

		private void CheckLifeTime()
		{
			if (_stack.Count <= 1)
				return;

			var parent = _stack[_stack.Count - 2];
			var child = _stack[_stack.Count - 1];
			if (parent.LifeTime == LifeTime.Global && child.LifeTime == LifeTime.Scene)
			{
				var message = "You are trying to inject the object with lifetime = " + child.LifeTime + " {" +
					child.Key + "} into object with lifetime = " + parent.LifeTime + " {" + parent.Key +"}";
				HandleLifetimeError(message);
			}
		}

		private void HandleLifetimeError(string message)
		{
			switch (OnLifetimeError)
			{
			case Reaction.LogWarning:
				DILog.LogWarning(message);
				break;
			case Reaction.LogError:
				DILog.LogError(message);
				break;
			case Reaction.ThrowException:
				throw new WrongLifetimeException(message);
			}
		}
	}
}