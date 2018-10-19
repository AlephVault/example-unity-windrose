using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Support
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

            public class CircularDependencyUnsupportedException : Types.Exception
            {
                public CircularDependencyUnsupportedException() { }
                public CircularDependencyUnsupportedException(string message) : base(message) { }
                public CircularDependencyUnsupportedException(string message, System.Exception inner) : base(message, inner) { }
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

            public static T RequireComponentInChildren<T>(MonoBehaviour current) where T : Component
            {
                return RequireComponentInChildren<T>(current.gameObject);
            }

            public static T RequireComponentInChildren<T>(GameObject current) where T : Component
            {
                T[] components = current.GetComponentsInChildren<T>(true);
                if (components.Length == 0)
                {
                    throw new MissingComponentInChildrenException("Current object's children must, at least, have one component of type " + typeof(T).FullName);
                }
                else
                {
                    return components.First();
                }
            }

            public static T[] RequireComponentsInChildren<T>(MonoBehaviour current, uint howMany, bool includeInactive = true) where T : Component
            {
                return RequireComponentsInChildren<T>(current.gameObject, howMany, includeInactive);
            }

            public static T[] RequireComponentsInChildren<T>(GameObject current, uint howMany, bool includeInactive = true) where T : Component
            {
                T[] components = current.GetComponentsInChildren<T>(includeInactive);
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
                    SetObjectFieldValues(component, data);
                    gameObject.SetActive(true);
                    return component;
                }
            }

            public static void SetObjectFieldValues(UnityEngine.Object target, Dictionary<string, object> data)
            {
                Type targetType = target.GetType();
                BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                foreach (KeyValuePair<string, object> pair in data)
                {
                    FieldInfo field = targetType.GetField(pair.Key, all);
                    if (field != null && !field.IsStatic && (field.IsPublic || field.IsDefined(typeof(SerializeField), true)))
                    {
                        field.SetValue(target, pair.Value);
                    }
                    else
                    {
                        throw new UnserializableFieldException("The field " + pair.Key + " cannot be populated for type " + targetType.FullName);
                    }
                }
            }

            /**
             * Gets a set of all the types this component depends on.
             */
            public static HashSet<Type> GetDependencies(Component component)
            {
                return GetDependencies(component.GetType());
            }

            /**
             * Gets a set of all the types the given component type depends on.
             */
            public static HashSet<Type> GetDependencies<C>() where C : Component
            {
                return GetDependencies(typeof(C));
            }

            /**
             * Private implementation to get the component dependencies.
             */
            private static HashSet<Type> GetDependencies(Type componentType)
            {
                IEnumerable<RequireComponent> attributes = (from attribute in componentType.GetCustomAttributes(typeof(RequireComponent), false) select (attribute as RequireComponent));
                HashSet<Type> types = new HashSet<Type>();
                foreach (var attribute in attributes)
                {
                    types.Add(attribute.m_Type0);
                    types.Add(attribute.m_Type1);
                    types.Add(attribute.m_Type2);
                }
                types.Remove(null);
                return types;
            }

            /**
             * Flattens the dependencies of the components. The criteria of dependencies will
             *   only account for types among the components, and not other potential types
             *   they could depend on.
             */
            public static Component[] SortByDependencies(Component[] components)
            {
                HashSet<Type> consideredComponentTypes = new HashSet<Type>(from component in components select component.GetType());
                List<Component> sourceComponentsList = new List<Component>(components);
                List<Component> endComponentsList = new List<Component>();
                HashSet<Type> fetchedTypes = new HashSet<Type>();

                while(true)
                {
                    // We end the loop if there is nothing else in the
                    //   source. The fetching terminated.
                    if (sourceComponentsList.Count == 0) break;

                    // On each iteration we will try adding at least one
                    //   element to the final components list. Otherwise, we
                    //   fail because the underlying reason is a circular
                    //   dependency.
                    bool foundTypeToAdd = false;

                    // We iterate over all the components trying to find
                    //   one to add. The criterion to accept one to add
                    //   is that either it doesn't have dependencies or
                    //   all its dependencies have already been processed.
                    foreach(var component in sourceComponentsList)
                    {
                        HashSet<Type> dependencies = new HashSet<Type>(GetDependencies(component).Intersect(consideredComponentTypes));
                        if (fetchedTypes.IsSupersetOf(dependencies))
                        {
                            // We mark that we found. Then we remove from source, add to end, and mark the type as fetched.
                            foundTypeToAdd = true;
                            sourceComponentsList.Remove(component);
                            endComponentsList.Add(component);
                            fetchedTypes.Add(component.GetType());
                            break;
                        }
                    }

                    // If no element was found to be added, we raise an
                    //   exception because we found a circular dependency.
                    if (!foundTypeToAdd)
                    {
                        throw new CircularDependencyUnsupportedException("Circular dependencies among components is unsupported when trying to sort them by dependencies");
                    }
                }

                // This is the end result. Elements were added in-order.
                return endComponentsList.ToArray();
            }
        }
    }
}