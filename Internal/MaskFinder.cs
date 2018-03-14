#if UNITY_5_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CryoDI
{
	/// <summary>
	/// Класс, реализующий поиск игровых объектов в дереве сцены по пути
	/// Путь должен начинаться с указания активного объекта!
	/// В пути может встречаться символ '*', что обозначает первый активный объект
	/// Также можно предоставить трансформ объекта, начиная с коротого надо искать
	/// </summary>
	internal class MaskFinder
	{
		private const string PathSeparator = "/";
		private const string AnyActiveMask = "*";

		public GameObject Find(string path)
		{
			bool rootSearch;
			var tokens = SplitPath(path, out rootSearch);
			return FindGameObject(tokens, rootSearch);
		}

		public GameObject Find(string path, Transform startingFrom)
		{
			bool rootSearch;
			var tokens = SplitPath(path, out rootSearch);
			if (rootSearch)
			{
				throw new ContainerException("Remove first '/' to search from definite object");
			}
			return FindFrom(tokens, startingFrom);
		}

		private string[] SplitPath(string path, out bool searchFromRoot)
		{
			string[] tokens;
			if (path.StartsWith(PathSeparator))
			{
				searchFromRoot = true;
				tokens = path.Substring(1).Split(new[] { PathSeparator }, StringSplitOptions.None);
			}
			else
			{
				searchFromRoot = false;
				tokens = path.Split(new[] { PathSeparator }, StringSplitOptions.None);
			}
			if (tokens.Length == 0)
				throw new ContainerException("Invalid path: " + path);

			if (tokens.Any(string.IsNullOrEmpty))
				throw new ContainerException("Invalid path: " + path);

			if (tokens[0] == AnyActiveMask)
				throw new ContainerException("Path can't start with symbol \"" + AnyActiveMask + "\"!");
			return tokens;
		}

		private GameObject FindGameObject(string[] names, bool rootSearch)
		{
			GameObject start = FindStartNode(names[0], rootSearch);
			return FindFrom(names.Skip(1), start.transform);
		}

		private GameObject FindFrom(IEnumerable<string> names, Transform startTransform)
		{
			var transform = startTransform;
			Transform childTransform;
			foreach (var name in names)
			{
				if (name == AnyActiveMask)
				{
					childTransform = GetFirstActiveChild(transform);
				}
				else
				{
					childTransform = transform.Find(name);
				}
				if (childTransform == null)
					throw new ContainerException("Can't find game object \"" + GetFullPath(transform) + PathSeparator + name + "\"");
				transform = childTransform;
			}
			return transform.gameObject;
		}

		/// <summary>
		/// Найти первый активный дочерний элемент
		/// </summary>
		private Transform GetFirstActiveChild(Transform transform)
		{
			return transform.Cast<Transform>().FirstOrDefault(child => child.gameObject.activeInHierarchy);
		}

		private GameObject FindStartNode(string name, bool rootSearch)
		{
			return rootSearch ? FindRootNode(name) : FindNode(name);
		}

		private GameObject FindNode(string name)
		{
			var node = FindObjectRecursively(go => go.name == name);
			if (node == null)
				throw new ContainerException("Can't find game object \"" + name + "\"");
			return node;
		}

		private GameObject FindRootNode(string name)
		{
			var node = FindObjectRecursively(go => go.transform.parent == null && go.name == name);
			if (node == null)
				throw new ContainerException("Can't find game object \"" + PathSeparator + name + "\"");
			return node;
		}

		private string GetFullPath(Transform transform)
		{
			var builder = new StringBuilder();
			if (transform.parent != null)
				builder.Append(GetFullPath(transform.parent));
			
			builder.Append(PathSeparator).Append(transform.name);
			return builder.ToString();
		}

		// ищем объект указанного типа, втом числе и среди неактивных
		GameObject FindObjectRecursively(Func<GameObject, bool> pred)
		{
			foreach (GameObject obj in GameObject.FindObjectsOfType(typeof(GameObject)))
			{
				if (obj.transform.parent == null)
				{
					var comp = FindObjectRecursively(obj.transform, pred);
					if (comp != null)
						return comp;
				}
			}
			return null;
		}

		GameObject FindObjectRecursively(Transform root, Func<GameObject,bool> pred)
		{
			if (pred(root.gameObject))
				return root.gameObject;
			for (int i = 0, c = root.childCount; i < c; ++i)
			{
				var obj = FindObjectRecursively(root.GetChild(i), pred);
				if (obj != null)
					return obj;
			}
			return null;
		}
	}
}

#endif
