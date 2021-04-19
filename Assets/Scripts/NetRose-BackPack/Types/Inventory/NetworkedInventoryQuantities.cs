using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using Mirror;
using AlephVault.Unity.Support.Utils;
using System.Collections.Generic;

namespace NetRose
{
    namespace Types
    {
        namespace Inventory
        {
            /// <summary>
            ///   This class manages the available quantity types
            ///     for the serialization/deserialization where
            ///     the type is not known beforehand (which is
            ///     not the case for the container/internal
            ///     positions, since each combination is known
            ///     to all the items and to the specific
            ///     implementation of an inventory).
            /// </summary>
            public static class NetworkedInventoryQuantities
            {
                /// <summary>
                ///   An exception class for the serialization / deserialization of
                ///     a networked quantity.
                /// </summary>
                public class Exception : Types.Exception
                {
                    public Exception(string message) : base(message) { }
                }

                /// <summary>
                ///   This exception is thrown when attempting to serialize a quantity
                ///     of an unexpected type.
                /// </summary>
                public class BadQuantityType : Exception
                {
                    public readonly Type QuantityType;
                    public BadQuantityType(Type quantityType) : base("Unexpected quantity type to serialize: " + quantityType.FullName) { QuantityType = quantityType; }
                }

                /// <summary>
                ///   This exception is thrown when attempting to deserialize a quantity
                ///     of an unexpected code.
                /// </summary>
                public class BadQuantityCode : Exception
                {
                    public readonly int QuantityCode;
                    public BadQuantityCode(int quantityCode) : base("Unexpected quantity code to deserialize: " + quantityCode) { QuantityCode = quantityCode; }
                }

                /// <summary>
                ///   Sets the assemblies we care about when tracking
                ///     definitions for the registrar methods for the
                ///     quantities serialization.
                /// </summary>
                public static Assembly[] AssemblesToInspect = new Assembly[] { Assembly.GetExecutingAssembly() };

                // A track of the by-type reader and writer. By default
                // registers handlers for int, float and bool (the three
                // default quantities in the base BackPack).
                private static SortedDictionary<Type, Tuple<Func<NetworkReader, object>, Action<NetworkWriter, object>>> registered = new SortedDictionary<Type, Tuple<Func<NetworkReader, object>, Action<NetworkWriter, object>>>()
                {
                    { typeof(bool), new Tuple<Func<NetworkReader, object>, Action<NetworkWriter, object>>(delegate(NetworkReader r) {
                        return r.ReadBoolean();
                    }, delegate(NetworkWriter w, object o) {
                        w.WriteBoolean((bool)o);
                    }) },
                    { typeof(int), new Tuple<Func<NetworkReader, object>, Action<NetworkWriter, object>>(delegate(NetworkReader r) {
                        return r.ReadInt32();
                    }, delegate(NetworkWriter w, object o) {
                        w.WriteInt32((int)o);
                    }) },
                    { typeof(float), new Tuple<Func<NetworkReader, object>, Action<NetworkWriter, object>>(delegate(NetworkReader r) {
                        return r.ReadSingle();
                    }, delegate(NetworkWriter w, object o) {
                        w.WriteSingle((float)o);
                    }) }
                };

                // A track of the integer codes for the type.
                private static Dictionary<Type, int> codeByType;

                // A track of the types for the given integer code.
                private static Type[] typeByCode;

                /// <summary>
                ///   Traverses ALL the non-generic STATIC types in the
                ///     currently executing assembly to find all the
                ///     static classes and run their static method named:
                ///     RegisterInventoryQuantityType, if defined.
                /// </summary>
                public static void AutodiscoverQuantityTypes()
                {
                    // This method is meant to be called just once, until
                    // the codeByType and typeByCode members are filled.
                    if (codeByType != null) return;

                    foreach(Type type in Classes.GetTypes(AssemblesToInspect))
                    {
                        foreach(MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                        {
                            if (method.Name == "AutodiscoverNetworkedInventoryQuantityTypes" && method.ReturnType == typeof(void) && method.GetParameters().Length == 0)
                            {
                                try
                                {
                                    method.Invoke(null, null);
                                }
                                catch(Exception e)
                                {
                                    Debug.LogError("While autodiscovering quantity types, an exception occurred for type " + type.FullName + ": " + e.Message);
                                }
                                break;
                            }
                        }
                    }
                    // After all the autodiscovery execution, we can be sure that all of the
                    // types were set in the registered member. So we populate the direct and
                    // inverse lookup dictionaries.
                    int index = 0;
                    codeByType = new Dictionary<Type, int>();
                    typeByCode = new Type[registered.Count];
                    foreach(Type type in registered.Keys)
                    {
                        codeByType[type] = index;
                        typeByCode[index] = type;
                        index++;
                    }
                }

                /// <summary>
                ///   Registers a way to read and write a quantity type via Mirror, IN A NON-POLYMORPHIC WAY.
                ///     Both the writer procedure and the reader function must be specified. This will fail
                ///     almost silently if reader/writer was already specified for a given type.
                /// </summary>
                /// <typeparam name="T">The datatype to serialize</typeparam>
                /// <param name="reader">The function to read from a <see cref="NetworkReader"/></param>
                /// <param name="writer">The function to write into a <see cref="NetworkWriter"/></param>
                public static void RegisterQuantityType<T>(Func<NetworkReader, T> reader, Action<NetworkWriter, T> writer)
                {
                    Type type = typeof(T);
                    if (!registered.ContainsKey(type))
                    {
                        registered[type] = new Tuple<Func<NetworkReader, object>, Action<NetworkWriter, object>>(
                            delegate (NetworkReader r) { return reader(r); },
                            delegate (NetworkWriter w, object value) { writer(w, (T)value); }
                        );
                    }
                    else
                    {
                        Debug.LogWarning("While registering quantity types, could not register type " + type.FullName + " since it was already registered");
                    }
                }

                /// <summary>
                ///   Writes a quantity value (if it is of an expected, discovered, type)
                ///     into a network writer.
                /// </summary>
                /// <param name="writer">The writer to write into</param>
                /// <param name="quantity">The quantity to write</param>
                private static void WriteRawQuantity(NetworkWriter writer, object quantity)
                {
                    // First, an attempt to build the registries will be used.
                    // If you don't want to have this call while writing a quantity
                    //   to cause the overhead on first write/read, invoke such
                    //   method on your own, on initialization.
                    AutodiscoverQuantityTypes();
                    Type quantityType = quantity.GetType();
                    int index;
                    if (!codeByType.TryGetValue(quantityType, out index))
                    {
                        throw new BadQuantityType(quantityType);
                    }
                    else
                    {
                        writer.WritePackedInt32(index);
                        registered[quantityType].Item2(writer, quantity);
                    }
                }

                /// <summary>
                ///   Reads a quantity value (if it is of an expected, discovered, code)
                ///     from a network reader, considering the appropriate type.
                /// </summary>
                /// <param name="reader">The reader to read from</param>
                /// <returns>The read quantity, of the appropriate type</returns>
                private static object ReadRawQuantity(NetworkReader reader)
                {
                    // First, an attempt to build the registries will be used.
                    // If you don't want to have this call while writing a quantity
                    //   to cause the overhead on first write/read, invoke such
                    //   method on your own, on initialization.
                    AutodiscoverQuantityTypes();
                    int index = reader.ReadPackedInt32();
                    if (index < 0 || index >= registered.Count)
                    {
                        throw new BadQuantityCode(index);
                    }
                    else
                    {
                        return registered[typeByCode[index]].Item1(reader);
                    }
                }

                /// <summary>
                ///   Wraps a quantity, to be able to serialize it using the
                ///     registered functions.
                /// </summary>
                public class Quantity
                {
                    /// <summary>
                    ///   The raw, underlying, quantity.
                    /// </summary>
                    public readonly object Raw;

                    public Quantity(object raw)
                    {
                        Raw = raw;
                    }
                }

                /// <summary>
                ///   Writes a wrapped quantity on the network writer.
                /// </summary>
                /// <param name="writer">The network writer to write the quantity on</param>
                /// <param name="quantity">The wrapped quantity to write</param>
                public static void WriteQuantity(this NetworkWriter writer, Quantity quantity)
                {
                    WriteRawQuantity(writer, quantity.Raw);
                }

                /// <summary>
                ///   Reads a wrapped quantity from the network reader.
                /// </summary>
                /// <param name="reader">The network reader to read from</param>
                /// <returns>The read quantity from the network reader</returns>
                public static Quantity ReadQuantity(this NetworkReader reader)
                {
                    return new Quantity(ReadRawQuantity(reader));
                }
            }
        }
    }
}
