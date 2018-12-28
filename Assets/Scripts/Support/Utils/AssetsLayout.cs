using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Support
{
    namespace Utils
    {
        public static class AssetsLayout
        {
            /**
             * This class can track arbitrary dependencies from a particular array of objects.
             * This array of objects is intended to serve as a set of data bundles for different
             *   ScriptableObject or regular (non-GameObject) classes. This class will work in
             *   consideration with different parameters involving:
             *   
             * 1. The T class of the T[] input array.
             * 2. The A class, descendant of Depends (descending from System.Attribute), to seek.
             * 3. An exception class, being by default the exception defined in this class, and
             *      mandatory being subclass of such exception type.
             */

            public class DependencyException : Types.Exception
            {
                public DependencyException(string message) : base(message) { }
            }

            public class MainComponentException : DependencyException
            {
                public MainComponentException(string message) : base(message) { }
            }

            public abstract class Depends : Attribute
            {
                private Type dependency;

                public Depends(Type dependency)
                {
                    Type baseDependency = BaseDependency();
                    if (!baseDependency.IsAssignableFrom(dependency))
                    {
                        throw new DependencyException(string.Format("Invalid requirement class. It must descend from {0} for {1} attribute", baseDependency.FullName, GetType().FullName));
                    }
                    this.dependency = dependency;
                }

                protected abstract Type BaseDependency();

                public Type Dependency
                {
                    get { return dependency; }
                }
            }

            private static void throwException(Type type, string message)
            {
                throw (Exception)Activator.CreateInstance(type, new object[] { message });
            }

            /**
             * Gets all the dependencies from an object, by inspecting a particular attribute being present
             *   on the object's class.
             */

            private static HashSet<Type> GetDependencies(Type attributeType, Type type, Type exceptionType)
            {
                return new HashSet<Type>(
                    from attribute in type.GetCustomAttributes(attributeType, true) select ((Depends)attribute).Dependency
                );
            }

            public static HashSet<Type> GetDependencies<A, E, T>() where A : Depends where E : DependencyException
            {
                return GetDependencies(typeof(A), typeof(T), typeof(E));
            }

            public static HashSet<Type> GetDependencies<A, T>() where A : Depends
            {
                return GetDependencies(typeof(A), typeof(T), typeof(DependencyException));
            }

            private static void CheckAssignability(Type attributeType, Type type, Type exceptionType)
            {
                if (!typeof(Depends).IsAssignableFrom(attributeType))
                {
                    throwException(exceptionType, "Invalid attribute class / component class combination - attribute class must descend from AssetsLayout.Depends");
                }
            }

            public static HashSet<Type> GetDependencies<A, E>(Type type) where A : Attribute where E : DependencyException
            {
                Type attributeType = typeof(A);
                Type exceptionType = typeof(E);
                CheckAssignability(attributeType, type, exceptionType);
                return GetDependencies(attributeType, type, exceptionType);
            }

            public static HashSet<Type> GetDependencies<A>(Type type) where A : Attribute
            {
                Type attributeType = typeof(A);
                Type exceptionType = typeof(DependencyException);
                CheckAssignability(attributeType, type, exceptionType);
                return GetDependencies(attributeType, type, exceptionType);
            }

            public static HashSet<Type> GetDependencies<A, E>(object component) where A : Attribute where E : DependencyException
            {
                return GetDependencies<A, E>(component.GetType());
            }

            public static HashSet<Type> GetDependencies<A>(object component) where A : Attribute
            {
                return GetDependencies<A, DependencyException>(component.GetType());
            }

            /**
             * Flattens / sorts the dependencies of an object['s type] or a type. The criterion will imply that
             *   the first element will not depend on anything, while the last element will be of the most dependent
             *   type among all.
             */

            public static T[] FlattenDependencies<T, A, E>(T[] components, bool errorOnMissingDependency = true) where A : Depends where E : DependencyException
            {
                HashSet<Type> consideredComponentTypes = new HashSet<Type>(from component in components select component.GetType());
                List<T> sourceComponentsList = new List<T>(components);
                List<T> endComponentsList = new List<T>();
                HashSet<Type> fetchedTypes = new HashSet<Type>();

                while (true)
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
                    foreach (var component in sourceComponentsList)
                    {
                        // Getting dependencies of the component.
                        HashSet<Type> componentDependencies = GetDependencies<A, E>(component);
                        if (errorOnMissingDependency && componentDependencies.Except(consideredComponentTypes).Count() > 0)
                        {
                            throwException(typeof(E), "At least one component has a dependency requirement not satisfied in the given input list");
                        }

                        if (fetchedTypes.IsSupersetOf(componentDependencies))
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
                        throwException(typeof(E), "Circular dependencies among components is unsupported when trying to sort them by dependencies");
                    }
                }

                // This is the end result. Elements were added in-order.
                return endComponentsList.ToArray();
            }

            public static T[] FlattenDependencies<T, A>(T[] componentsList, bool errorOnMissingDependency = true) where A : Depends
            {
                return FlattenDependencies<T, A, DependencyException>(componentsList, errorOnMissingDependency);
            }

            /**
             * Avoids duplicated instances (i.e. more than one instance of the same type) in the components list.
             * Returns the type => component mapping being created.
             */

            public static Dictionary<Type, T> AvoidDuplicateDependencies<T, E>(T[] componentsList) where E : DependencyException
            {
                Dictionary<Type, T> dictionary = new Dictionary<Type, T>();
                foreach(T component in componentsList)
                {
                    Type type = component.GetType();
                    if (dictionary.ContainsKey(type))
                    {
                        throwException(typeof(E), "Cannot add more than one component instance per component type");
                    }
                    dictionary[type] = component;
                }
                return dictionary;
            }

            public static Dictionary<Type, T> AvoidDuplicateDependencies<T>(T[] componentsList)
            {
                return AvoidDuplicateDependencies<T, DependencyException>(componentsList);
            }

            /**
             * Cross-check dependencies from one list to another list.
             */

            public static void CrossCheckDependencies<TargetType, DependencyType, A, E>(TargetType[] componentsList, DependencyType[] dependencies) where A : Depends where E : DependencyException
            {
                HashSet<Type> requiredDependencies = new HashSet<Type>();
                foreach (TargetType component in componentsList)
                {
                    foreach (A attribute in component.GetType().GetCustomAttributes(typeof(A), true))
                    {
                        requiredDependencies.Add(attribute.Dependency);
                    }
                }
                HashSet<Type> installed = new HashSet<Type>(from dependency in dependencies select dependency.GetType());
                HashSet<Type> unsatisfiedDependencies = new HashSet<Type>(requiredDependencies.Except(installed));
                if (unsatisfiedDependencies.Count > 0)
                {
                    if (unsatisfiedDependencies.Count == 1)
                    {
                        throwException(typeof(E), "Unsatisfied dependency: " + unsatisfiedDependencies.First().FullName);
                    }
                    else
                    {
                        throwException(typeof(E), "Unsatisfied dependencies: " + string.Join(",", (from unsatisfiedDependency in unsatisfiedDependencies select unsatisfiedDependency.FullName).ToArray()));
                    }
                }
            }

            public static void CrossCheckDependencies<TargetType, DependencyType, A>(TargetType[] componentsList, DependencyType[] dependencies) where A : Depends
            {
                CrossCheckDependencies<TargetType, DependencyType, A, DependencyException>(componentsList, dependencies);
            }

            /**
             * And now the same but against a single element on either side.
             */

            // Array, Item -> Array, [Item]
            public static void CrossCheckDependencies<TargetType, DependencyType, A, E>(TargetType[] componentsList, DependencyType dependency) where A : Depends where E : DependencyException
            {
                CrossCheckDependencies<TargetType, DependencyType, A, E>(componentsList, new DependencyType[] { dependency });
            }

            public static void CrossCheckDependencies<TargetType, DependencyType, A>(TargetType[] componentsList, DependencyType dependency) where A : Depends
            {
                CrossCheckDependencies<TargetType, DependencyType, A, DependencyException>(componentsList, dependency);
            }

            // Item, Array -> [Item], Array
            public static void CrossCheckDependencies<TargetType, DependencyType, A, E>(TargetType component, DependencyType[] dependencies) where A : Depends where E : DependencyException
            {
                CrossCheckDependencies<TargetType, DependencyType, A, E>(new TargetType[] { component }, dependencies);
            }

            public static void CrossCheckDependencies<TargetType, DependencyType, A>(TargetType component, DependencyType[] dependencies) where A : Depends
            {
                CrossCheckDependencies<TargetType, DependencyType, A, DependencyException>(component, dependencies);
            }

            // Item, Item -> [Item], [Item]
            public static void CrossCheckDependencies<TargetType, DependencyType, A, E>(TargetType component, DependencyType dependency) where A : Depends where E : DependencyException
            {
                CrossCheckDependencies<TargetType, DependencyType, A, E>(new TargetType[] { component }, new DependencyType[] { dependency });
            }

            public static void CrossCheckDependencies<TargetType, DependencyType, A>(TargetType component, DependencyType dependency) where A : Depends
            {
                CrossCheckDependencies<TargetType, DependencyType, A, DependencyException>(component, dependency);
            }

            /**
             * Check in-selection element.
             */

            public static void CheckMainComponent<T, E>(T[] components, T mainComponent) where E : MainComponentException
            {
                if (!components.Contains(mainComponent))
                {
                    throwException(typeof(E), string.Format("An instance of {0} must be selected as main component", typeof(T).FullName));
                }
            }

            public static void CheckMainComponent<T>(T[] components, T mainComponent)
            {
                CheckMainComponent<T, MainComponentException>(components, mainComponent);
            }

            /**
             * Check presence
             */

            public static void CheckPresence<T, E>(T component, string fieldName = "") where E : DependencyException
            {
                if (component == null)
                {
                    fieldName = fieldName != "" ? fieldName : string.Format("[unspecified field of type {0}] is required: it must not be null", component.GetType().Name);
                    throwException(typeof(E), fieldName);
                }
            }

            public static void CheckPresence<T>(T component, string fieldName = "")
            {
                CheckPresence<T, DependencyException>(component);
            }
        }
    }
}
