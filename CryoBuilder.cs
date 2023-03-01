#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{
	public sealed class CryoBuilder : CryoBehaviour
	{
		protected override void BuildUp(CryoContainer container)
		{
			var monoBehaviours = GetComponents<MonoBehaviour>();
			foreach (var monoBehaviour in monoBehaviours)
			{
				if (monoBehaviour is CryoBehaviour)
					continue;
				container.BuildUp(monoBehaviour);
			}
		}
	}
}
#endif