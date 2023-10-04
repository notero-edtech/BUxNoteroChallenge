using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        //Defined in the common base class for all mono behaviours
        public static I GetInterfaceComponent<I>(this GameObject o) where I : class
        {
            return o.GetComponent(typeof(I)) as I;
        }

        public static List<I> FindObjectsOfInterface<I>(this GameObject o) where I : class
        {
            MonoBehaviour[] monoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
            List<I> list = new List<I>();

            foreach (MonoBehaviour behaviour in monoBehaviours)
            {
                I component = behaviour.GetComponent(typeof(I)) as I;

                if (component != null)
                {
                    list.Add(component);
                }
            }

            return list;
        }

        public static T AddOrGetComponent<T>(this GameObject o) where T : Component
        {
            var c = o.GetComponent<T>();
            if (!c) c = o.AddComponent<T>();
            return c;
        }

        public static T GetComponentUltimate<T>(this GameObject o) where T : class
        {
            T r = default;
            if (!o || !o.transform) return r;
            if (r == null) r = o?.transform?.GetComponent<T>();
            if (r == null) r = o?.transform?.GetComponentInChildren<T>();
            if (r == null) r = o?.transform?.GetComponentInParent<T>();

            return r;
        }

        public static T[] GetComponentsUltimate<T>(this GameObject o) where T : class
        {
            T[] r = new T[0];
            if (!o || !o.transform) return r;
            if (r == null) r = o?.transform?.GetComponents<T>();
            if (r == null) r = o?.transform?.GetComponentsInChildren<T>();
            if (r == null) r = o?.transform?.GetComponentsInParent<T>();

            return r;
        }

        public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError("Expected to find component of type "
                + typeof(T) + " but found none", obj);
            }

            return component;
        }

        public static void SetRenderer(this GameObject o, bool enabled, bool recursively = false)
        {
            if (o.GetComponent<Renderer>())
                o.GetComponent<Renderer>().enabled = enabled;
            if (recursively)
            {
                foreach (Transform t in o.transform)
                {
                    t.gameObject.SetRenderer(enabled, recursively);
                }
            }
        }

        public static void SetCollider(this GameObject o, bool enabled, bool recursively = false)
        {
            if (o.GetComponent<Collider>())
                o.GetComponent<Collider>().enabled = enabled;
            if (recursively)
            {
                foreach (Transform t in o.transform)
                {
                    t.gameObject.SetCollider(enabled, recursively);
                }
            }
        }

        public static void ChangeLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.ChangeLayersRecursively(layer);
            }
        }

        public static GameObject Add(this GameObject parent, GameObject gameObject, bool reset = false)
        {
            if (!parent || !gameObject) return null;

            gameObject.transform.SetParent(parent.transform);
                       
            if (parent.GetComponent<RectTransform>())
            {
                var rt = gameObject.AddOrGetComponent<RectTransform>();
                rt.Reset();
            } else
            {
                if (reset) gameObject.transform.Reset();
            }

            return gameObject;
        }
    }
}
