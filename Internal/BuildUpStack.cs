using System.Collections.Generic;
using System.Text;
using DefaultNamespace;

namespace CryoDI
{
	internal class BuildUpStack
	{
		private List<IInitializable> _initializables = new List<IInitializable>();
		private List<ILateInitializable> _lateInitializables = new List<ILateInitializable>();
		private List<Entry> _stack = new List<Entry>();

		internal class Entry
		{
			public object Object { get; set; }
			public string PropertyName { get; set; }
		}

		public BuildUpStack()
		{
			OnCircularDependency = Reaction.LogError;
		}
		
		public Reaction OnCircularDependency { get; set; }

		public void PushObject(object obj)
		{
			_stack.Add(new Entry
			{
				Object = obj
			});
			CheckCircularDependencyOnPush(obj);

			var initializable = obj as IInitializable;
			if (initializable != null)
				AddInitializable(initializable);
			
			var lateInitializable = obj as ILateInitializable;
			if (lateInitializable != null)
				AddLateInitializable(lateInitializable);
		}

		public void SetPropertyName(string propertyName)
		{
			Peek().PropertyName = propertyName;
		}

		public void Pop()
		{
			if (_stack.Count == 0)
				throw new ContainerException("Unexpected state: stack is empty");
			_stack.RemoveAt(_stack.Count - 1);

			if (_stack.Count == 0)
				CallInitialize();
		}

		public void CheckCircularDependency(object obj)
		{
			for (int i = _stack.Count - 1; i >= 0; --i)
			{
				if (ReferenceEquals(_stack[i].Object, obj))
				{
					HandleCircularDependency(i);
				}
			}
		}

		private void AddInitializable(IInitializable obj)
		{
			_initializables.Add(obj);
		}
		
		private void AddLateInitializable(ILateInitializable obj)
		{
			_lateInitializables.Add(obj);
		}

		private void CheckCircularDependencyOnPush(object obj)
		{
			for (int i = _stack.Count - 2; i >= 0; --i)
			{
				if (ReferenceEquals(_stack[i].Object, obj))
				{
					HandleCircularDependency(i);
				}
			}
		}

		private void HandleCircularDependency(int from)
		{
			if (OnCircularDependency == Reaction.Ignore)
				return;
			
			var builder = new StringBuilder();
			builder.Append("Type: " + _stack[from].Object.GetType() + ". Property: " + _stack[from].PropertyName);
			for (int i = from + 1; i < _stack.Count; ++i)
			{
				builder.AppendLine(" -> ");
				builder.Append("Type: " + _stack[i].Object.GetType() + ". Property: " + _stack[i].PropertyName);
			}

			switch (OnCircularDependency)
			{
			case Reaction.LogWarning:
				DILog.LogWarning("Circular dependency found: " + builder.ToString());
				break;
			case Reaction.LogError:
				DILog.LogError("Circular dependency found: " + builder.ToString());
				break;
			case Reaction.ThrowException:
				throw new CircularDependencyException(builder.ToString());
			}
			
		}

		private Entry Peek()
		{
			if (_stack.Count == 0)
				throw new ContainerException("Unexpected state: stack is empty");
			return _stack[_stack.Count - 1];
		}

		private void CallInitialize()
		{
			var initializables = _initializables;
			_initializables = new List<IInitializable>();
			foreach (var initializable in initializables)
			{
				initializable.Initialize();
			}
			
			var lateInitializables = _lateInitializables;
			_lateInitializables = new List<ILateInitializable>();
			foreach (var lateInitializable in lateInitializables)
			{
				lateInitializable.LateInitialize();
			}
		}
	}
}