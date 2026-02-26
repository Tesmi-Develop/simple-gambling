using System;
using System.Collections.Generic;

namespace Shared
{
    public class EventBus
    {
        public event Action<Type, object> OnEvent = null!; 
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();
        private readonly HashSet<Delegate> _scheduledRemoveHandlers = new HashSet<Delegate>();
        private readonly Dictionary<Type, List<Delegate>> _scheduledAddHandlers = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            var type = typeof(TEvent);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            if (!_scheduledAddHandlers.TryGetValue(type, out var scheduledAddHandlers))
            {
                scheduledAddHandlers = new List<Delegate>();
                _scheduledAddHandlers.Add(type, scheduledAddHandlers);
            }

            scheduledAddHandlers.Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
        {
            var type = typeof(TEvent);
            if (!_handlers.TryGetValue(type, out var handlers) || !handlers.Contains(handler))
                return;

            _scheduledRemoveHandlers.Add(handler);
        }

        public void Publish(object eventData, Type type)
        {
            OnEvent?.Invoke(type, eventData);
            if (!_handlers.TryGetValue(type, out var handlers)) 
                return;

            foreach (var handler in _scheduledRemoveHandlers)
                handlers.Remove(handler);

            if (_scheduledAddHandlers.TryGetValue(type, out var scheduledAddHandlers))
            {
                foreach (var handler in scheduledAddHandlers)
                    handlers.Add(handler);

                _scheduledAddHandlers.Remove(type);
            }
            
            foreach (var handler in handlers)
            {
                try
                {
                    handler.DynamicInvoke(eventData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
        public void Publish<TEvent>(TEvent evt) where TEvent : class
        {
            var type = typeof(TEvent);
            Publish(evt, type);
        }
    }
}