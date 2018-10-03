using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryoDI.ViewMediatorBinding
{
	public class ViewMediatorContainer : CryoContainer, IMediatorFactory
	{
		private readonly Dictionary<Type, Type> _mediators = new Dictionary<Type, Type>();
		private static ViewMediatorContainer _instance;

		public ViewMediatorContainer()
		{
			View.MediatorFactory = this;
		}

		public T CreateAndBindMediator<T>(View<T> view) where T : class
		{			
			var viewType = view.GetType();
			Type mediatorType;
			if (!_mediators.TryGetValue(viewType, out mediatorType))
				throw new ContainerException("Can't find mediator for view type " + viewType.Name);

			var mediator = Resolve(mediatorType);
			SetupView(mediator, view);

			EventBinder.BindEventHandlers(mediator, view);
			var m = mediator as IMediator;
			if (m != null)
				m.AfterViewBinded(view);
			return (T)mediator;
		}

		public void UnbindMediator<T>(T mediator, View<T> view) where T : class
		{
			EventBinder.UnbindEventHandlers(mediator, view);
			var m = mediator as IMediator;
			if (m != null)
				m.BeforeViewDestroyed();
		}

		public IMediatorFactory RegisterMediator<V, M>()
			where V : View<M>
			where M : class, new()
		{
			_mediators[typeof(V)] = typeof(M);
			if (!IsRegistered<M>())
				this.RegisterType<M>();

			return this;
		}

		private void SetupView(object mediator, object view)
		{
#if !NETFX_CORE
            Type mediatorType = mediator.GetType();

#if NETFX_CORE
            IEnumerable<MemberInfo> members = mediatorType.GetTypeInfo().DeclaredMembers;
#else

            MemberInfo[] members = mediatorType.FindMembers(MemberTypes.Property,
                                                   BindingFlags.FlattenHierarchy | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
                                                   null, null);
#endif



            foreach (MemberInfo member in members)
			{
				var attrs = member.GetCustomAttributes(typeof (ViewAttribute), true);
				if (!attrs.Any())
					continue;

				var propertyInfo = (PropertyInfo) member;
				propertyInfo.SetValue(mediator, view, null);
				break;
			}
#endif
        }
	}
}
