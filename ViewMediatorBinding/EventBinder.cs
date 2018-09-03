using System;
using System.Linq;
using System.Reflection;

namespace CryoDI.ViewMediatorBinding
{
	public static class EventBinder
	{
		public static void BindEventHandlers(object subscriber, object publisher)
		{
			Type viewType = publisher.GetType();
			Type mediatorType = subscriber.GetType();


#if NETFX_CORE
              var methods = mediatorType.GetRuntimeMethods();

#else
            var methods = mediatorType.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(SubscribeAttribute), true);
                if (!attrs.Any())
                    continue;
                foreach (var attr in attrs)
                {
                    var attrib = (SubscribeAttribute)attr;
                    var eventInfo = viewType.GetEvent(attrib.Name);
                    if (eventInfo == null)
                        throw new ContainerException("Can't find event \"" + attrib.Name + "\" in type \"" + viewType.Name + "\"");
                    var dlgate = Delegate.CreateDelegate(eventInfo.EventHandlerType, subscriber, method.Name, false, true);
                    eventInfo.AddEventHandler(publisher, dlgate);
                }
            }
#endif
        }

        public static void UnbindEventHandlers(object subscriber, object publisher)
		{
			if (publisher == null || subscriber == null)
				return;

			Type viewType = publisher.GetType();
			Type mediatorType = subscriber.GetType();

#if NETFX_CORE
              var methods = mediatorType.GetRuntimeMethods();
#else
            var methods = mediatorType.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(SubscribeAttribute), true);
                if (!attrs.Any())
                    continue;
                foreach (var attr in attrs)
                {
                    var attrib = (SubscribeAttribute)attr;
                    var eventInfo = viewType.GetEvent(attrib.Name);
                    if (eventInfo == null)
                        throw new ContainerException("Can't find event \"" + attrib.Name + "\" in type \"" + viewType.Name + "\"");

                    var dlgate = GetSubscribedDelegate(publisher, eventInfo);
                    eventInfo.RemoveEventHandler(publisher, dlgate);
                }
            }
#endif

        }

		private static Delegate GetSubscribedDelegate(object target, EventInfo eventInfo)
		{
			FieldInfo eventFieldInfo;

			var type = target.GetType();

#if NETFX_CORE
			while((eventFieldInfo = type.GetRuntimeField(eventInfo.Name)) == null)
			{				
				type = type.GetTypeInfo().BaseType;
			} 
#else
            while ((eventFieldInfo = type.GetField(eventInfo.Name,
                 BindingFlags.FlattenHierarchy |
                 BindingFlags.NonPublic |
                 BindingFlags.Instance |
                 BindingFlags.GetField)) == null)
            {
                type = type.BaseType;
            }
#endif


            Delegate eventFieldValue = (Delegate)eventFieldInfo.GetValue(target);
			return eventFieldValue;
		}
	}
}
