using UnityEngine;
using System;
using System.Collections.Generic;

namespace CardGame.Events
{
    /// <summary>
    /// 事件管理器
    /// 统一的事件发布和订阅系统
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        // 事件字典：事件类型 -> 事件处理器列表
        private Dictionary<string, List<Action<object>>> eventHandlers = new Dictionary<string, List<Action<object>>>();

        private static EventManager instance;
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EventManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("EventManager");
                        instance = go.AddComponent<EventManager>();
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(string eventType, Action<object> handler)
        {
            if (string.IsNullOrEmpty(eventType) || handler == null)
            {
                Debug.LogWarning("EventManager: 无效的事件类型或处理器！");
                return;
            }

            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<Action<object>>();
            }

            if (!eventHandlers[eventType].Contains(handler))
            {
                eventHandlers[eventType].Add(handler);
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe(string eventType, Action<object> handler)
        {
            if (string.IsNullOrEmpty(eventType) || handler == null)
            {
                return;
            }

            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Remove(handler);
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish(string eventType, object eventData = null)
        {
            if (string.IsNullOrEmpty(eventType))
            {
                Debug.LogWarning("EventManager: 事件类型不能为空！");
                return;
            }

            if (eventHandlers.ContainsKey(eventType))
            {
                // 创建副本以避免在迭代时修改列表
                var handlers = new List<Action<object>>(eventHandlers[eventType]);
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler?.Invoke(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"EventManager: 处理事件 {eventType} 时发生错误: {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 清空指定类型的所有订阅
        /// </summary>
        public void ClearEvent(string eventType)
        {
            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Clear();
            }
        }

        /// <summary>
        /// 清空所有事件订阅
        /// </summary>
        public void ClearAllEvents()
        {
            eventHandlers.Clear();
        }

        /// <summary>
        /// 获取事件订阅数量（用于调试）
        /// </summary>
        public int GetSubscriberCount(string eventType)
        {
            if (eventHandlers.ContainsKey(eventType))
            {
                return eventHandlers[eventType].Count;
            }
            return 0;
        }
    }
}
