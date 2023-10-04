using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Notero.Unity.Networking.Mirror
{
    public interface IEventHandler
    {
        public void Subscribe(IEventSubscribable subscribable);

        public void Unsubscribe(IEventSubscribable subscribable);
    }
    
    public interface IEventSubscribable
    {
        public void Subscribe<TEvent>(Action<NetworkConnectionToClient, TEvent> callback);
        public void Unsubscribe<TEvent>(Action<NetworkConnectionToClient, TEvent> callback);

        public void Subscribe<TEvent>(Action<TEvent> callback);
        public void Unsubscribe<TEvent>(Action<TEvent> callback);
    }

    public interface IEventPublishable
    {
        public void Publish<TEvent>(NetworkConnectionToClient connection, TEvent eventMessage);
        public void Publish<TEvent>(TEvent eventMessage);
    }

    public interface IEventBus : IEventSubscribable, IEventPublishable { }

    public class EventBus : IEventBus
    {
        public static bool IsQueueProcessActive
        {
            set
            {
                m_IsQueueProcessActive = value;
                if(m_IsQueueProcessActive) ProcessQueue();
            }
        }

        private static bool m_IsQueueProcessActive = true;

        public static EventBus Default => m_Default ??= new EventBus();
        private static EventBus m_Default;

        private readonly Dictionary<string, HashSet<object>> m_EventServerHandlers = new Dictionary<string, HashSet<object>>();
        private readonly Dictionary<string, HashSet<object>> m_EventClientHandlers = new Dictionary<string, HashSet<object>>();
        private static readonly Queue<Action> m_ExecutionQueue = new Queue<Action>();

        public void Subscribe<TEvent>(Action<NetworkConnectionToClient, TEvent> callback)
        {
            string key = typeof(TEvent).Name;

            if(!m_EventServerHandlers.ContainsKey(key))
            {
                m_EventServerHandlers.Add(key, new HashSet<object>() { callback });
            }
            else if(m_EventServerHandlers.TryGetValue(key, out var handlers))
            {
                handlers.Add(callback);
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> callback)
        {
            string key = typeof(TEvent).Name;

            if(!m_EventClientHandlers.ContainsKey(key))
            {
                m_EventClientHandlers.Add(key, new HashSet<object>() { callback });
            }
            else if(m_EventClientHandlers.TryGetValue(key, out var handlers))
            {
                handlers.Add(callback);
            }
        }

        public void Unsubscribe<TEvent>(Action<NetworkConnectionToClient, TEvent> callback)
        {
            string key = typeof(TEvent).Name;

            if(!m_EventServerHandlers.ContainsKey(key)) return;

            if(m_EventServerHandlers.TryGetValue(key, out var handlers))
            {
                handlers.Remove(callback);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> callback)
        {
            string key = typeof(TEvent).Name;

            if(!m_EventClientHandlers.ContainsKey(key)) return;

            if(m_EventClientHandlers.TryGetValue(key, out var handlers))
            {
                handlers.Remove(callback);
            }
        }

        public void Publish<TEvent>(NetworkConnectionToClient connection, TEvent eventMessage)
        {
            RunQueueEventMessge(() =>
            {
                var key = eventMessage.GetType().Name;

                if(!m_EventServerHandlers.TryGetValue(key, out var handlers)) return;
                foreach(var handler in handlers.ToList())
                {
                    handler.GetType().GetMethod("Invoke")?.Invoke(handler, new object[] { connection, eventMessage });
                }
            });
        }

        public void Publish<TEvent>(TEvent eventMessage)
        {
            RunQueueEventMessge(() =>
            {
                var key = eventMessage.GetType().Name;

                if(!m_EventClientHandlers.TryGetValue(key, out var handlers)) return;

                foreach(var handler in handlers.ToList())
                {
                    handler.GetType().GetMethod("Invoke")?.Invoke(handler, new object[] { eventMessage });
                }
            });
        }

        public static void Dispose()
        {
            IsQueueProcessActive = false;

            lock(m_ExecutionQueue)
            {
                m_ExecutionQueue.Clear();
            }

            m_Default = null;
        }

        private static void RunQueueEventMessge(Action action)
        {
            lock(m_ExecutionQueue)
            {
                m_ExecutionQueue.Enqueue(action);
                ProcessQueue();
            }
        }

        private static void ProcessQueue()
        {
            lock(m_ExecutionQueue)
            {
                if(m_ExecutionQueue.Count == 0 || !m_IsQueueProcessActive) return;

                //Kept for debugging Network Messages.
                //DebugProcessQueue();

                m_ExecutionQueue.Dequeue().Invoke();
                ProcessQueue();
            }
        }

        private static void DebugProcessQueue()
        {
            lock(m_ExecutionQueue)
            {
                Debug.Log($"{m_ExecutionQueue.Count} events in queue.\nProcessing: {m_ExecutionQueue.First().Target}");
            }
        }
    }
}