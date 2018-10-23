using System.Collections.Generic;
using System.Text;

namespace CryoDI
{
	internal class BuildUpStack
	{
		private List<IInitializable> _initializables = new List<IInitializable>();
		private List<Entry> _stack = new List<Entry>();

		internal class Entry
		{
			public object Object { get; set; }
			public string PropertyName { get; set; }
		}

		public void PushObject(object obj)
		{
			_stack.Add(new Entry
			{
				Object = obj
			});
			CheckCircularDependency(obj);

			var initializable = obj as IInitializable;
			if (initializable != null)
				AddInitializable(initializable);
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

		private void AddInitializable(IInitializable obj)
		{
			_initializables.Add(obj);
		}

		private void CheckCircularDependency(object obj)
		{
			for (int i = _stack.Count - 2; i >= 0; --i)
			{
				if (ReferenceEquals(_stack[i].Object, obj))
				{
					DumpCircularDependency(i);
				}
			}
		}

		private void DumpCircularDependency(int from)
		{
			var builder = new StringBuilder();
			builder.Append("Type: " + _stack[from].Object.GetType() + ". Property: " + _stack[from].PropertyName);
			for (int i = from; i < _stack.Count; ++i)
			{
				builder.AppendLine(" -> ");
				builder.Append("Type: " + _stack[i].Object.GetType() + ". Property: " + _stack[i].PropertyName);
			}

			DILog.LogWarning("Circular dependency found: ");
			DILog.LogWarning(builder.ToString());
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
		}
	}
}