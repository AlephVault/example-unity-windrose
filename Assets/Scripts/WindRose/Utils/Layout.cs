using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Utils
    {
        public class Layout
        {
            public class MissingParentException : Types.Exception
            {
                public MissingParentException() { }
                public MissingParentException(string message) : base(message) { }
                public MissingParentException(string message, Exception inner) : base(message, inner) { }
            }

            public class MissingComponentInParentException : Types.Exception
            {
                public MissingComponentInParentException() { }
                public MissingComponentInParentException(string message) : base(message) { }
                public MissingComponentInParentException(string message, Exception inner) : base(message, inner) { }
            }

            public class MissingComponentInChildrenException : Types.Exception
            {
                public MissingComponentInChildrenException() { }
                public MissingComponentInChildrenException(string message) : base(message) { }
                public MissingComponentInChildrenException(string message, Exception inner) : base(message, inner) { }
            }

            public class UnserializableFieldException : Types.Exception
            {
                public UnserializableFieldException() { }
                public UnserializableFieldException(string message) : base(message) { }
                public UnserializableFieldException(string message, System.Exception inner) : base(message, inner) { }
            }

            public static T RequireComponentInParent<T>(MonoBehaviour script) where T : Component
            {
                return RequireComponentInParent<T>(script.gameObject);
            }

            public static T RequireComponentInParent<T>(GameObject current) where T : Component
            {
                try
                {
                    Transform parentTransform = current.transform.parent;
                    GameObject parentGameObject = (parentTransform != null) ? parentTransform.gameObject : null;
                    T component = parentGameObject.GetComponent<T>();
                    if (component == null)
                    {
                        throw new MissingComponentInParentException("Current object's parent needs a component of type " + typeof(T).FullName);
                    }
                    else
                    {
                        return component;
                    }
                }
                catch (NullReferenceException)
                {
                    throw new MissingParentException("Current object needs a parent object");
                }
            }

            public static T RequireComponentInChildren<T>(GameObject current) where T : Component
            {
                T component = current.GetComponentInChildren<T>();
                if (component == null)
                {
                    throw new MissingComponentInChildrenException("Current object's children must, at least, have one component of type " + typeof(T).FullName);
                }
                else
                {
                    return component;
                }
            }

            public static T[] RequireComponentsInChildren<T>(GameObject current, uint howMany) where T : Component
            {
                T[] components = current.GetComponentsInChildren<T>();
                if (components == null || components.Length < howMany)
                {
                    throw new MissingComponentInChildrenException("Current object's children must, at least, have " + howMany + " component(s) of type " + typeof(T).FullName);
                }
                else
                {
                    T[] result = new T[howMany];
                    Array.ConstrainedCopy(components, 0, result, 0, (int) howMany);
                    return result;
                }
            }

            public static T AddComponent<T>(GameObject gameObject, Dictionary<string, object> data = null) where T : Component
            {
                if (data == null)
                {
                    return gameObject.AddComponent<T>();
                }
                else
                {
                    gameObject.SetActive(false);
                    T component = gameObject.AddComponent<T>();
                    Type componentType = component.GetType();
                    BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                    foreach (KeyValuePair<string, object> pair in data)
                    {
                        FieldInfo field = componentType.GetField(pair.Key, all);
                        if (field != null && !field.IsStatic && (field.IsPublic || field.IsDefined(typeof(SerializeField), true)))
                        {
                            field.SetValue(component, pair.Value);
                        }
                        else
                        {
                            throw new UnserializableFieldException("The field " + pair.Key + " cannot be populated for type " + typeof(T).FullName);
                        }
                    }
                    gameObject.SetActive(true);
                    return component;
                }
            }
        }
    }
}