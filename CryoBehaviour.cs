#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{
    public class CryoBehaviour : MonoBehaviour
    {
	    public bool BuiltUp { get; private set; }

        public virtual void Awake()
        {
	        BuildUp();
        }

		public void BuildUp()
		{
			if (!BuiltUp)
			{
				BuiltUp = true;
				UnityContainerBuilder.Container.BuildUp(this);
			}
		}
    }
}
#endif