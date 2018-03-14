#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{
    public class SceneDependent : MonoBehaviour
    {
	    protected static Container _container;

	    public static void SetContainer(Container container)
	    {
		    _container = container;
	    }

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
				_container.BuildUp(this);
			}
		}
	}
}
#endif