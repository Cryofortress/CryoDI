#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{
    public class CryoBehaviour : MonoBehaviour
    {
	    private static CryoContainer _rootContainer;
	    
	    protected CryoBehaviour() {}

	    public bool BuiltUp { get; private set; }

        protected virtual void Awake()
        {
	        BuildUp();
        }

		public void BuildUp()
		{
			if (!BuiltUp)
			{
				BuiltUp = true;
				if (_rootContainer == null)
				{
					((IInternalContainerBuidler)UnityStarter.Instance).CreateRootContainer();
				}
				_rootContainer.BuildUp(this);
			}
		}

	    public static void SetRootContainer(CryoContainer container)
	    {
		    if (_rootContainer != null)
			    throw new ContainerException("Root container already set");
		    _rootContainer = container;
	    }
	    
    }
}
#endif