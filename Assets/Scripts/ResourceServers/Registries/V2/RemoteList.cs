using System.Collections;
using System.Collections.Generic;
using GMM.Types;
using UnityEngine;

namespace ResourceServers
{
    namespace Registries
    {
        namespace V2
        {
            /// <summary>
            ///   Lists have to be implemented to return an
            ///     arbitrary Unity object given an id. If
            ///     somehow this fails (e.g. invalid ID)
            ///     then a V2.Registry.FetchError must be
            ///     raised.
            /// </summary>
            public abstract class RemoteList<JSONType, ObjectType> : Registry.List where ObjectType : Object
            {
                /// <summary>
                ///   This class is used as the receiver of
                ///     the body in a V2 list endpoint, with
                ///     all the entries.
                /// </summary>
                /// <typeparam name="JSONType">The type of the resource elements' data</typeparam>
                private class ListContent<JSONType>
                {
                    public string Caption { get; set; }
                    public Dictionary<string, JSONType> Resources { get; set; }
                }

                [SerializeField]
                private SerializableDictionary<ulong, ObjectType> objects = new SerializableDictionary<ulong, ObjectType>();

                /// <summary>
                ///   Inflates an object of the target type
                ///     from the given resource data. It must
                ///     be implemented per-list.
                /// </summary>
                /// <param name="resourceData">The source data to hidrate from</param>
                /// <returns>The inflated object</returns>
                protected abstract ObjectType Inflate(JSONType resourceData);

                /// <summary>
                ///   Populates its content via a received
                ///     JSON body, by parsing each resource's
                ///     content and hidrating an instance of
                ///     the target object type.
                /// </summary>
                /// <param name="body">The JSON body to parse and populate from</param>
                public override void Populate(string body)
                {
                    ListContent<JSONType> content = JSON.Parse<ListContent<JSONType>>(body);
                    foreach(KeyValuePair<string, JSONType> pair in content.Resources)
                    {
                        objects[ulong.Parse(pair.Key)] = Inflate(pair.Value);
                    }
                }

                /// <summary>
                ///   Finding an object involves checking the
                ///   inner dictionary.
                /// </summary>
                /// <param name="id">The ID to lookup</param>
                /// <returns>The object with the given ID</returns>
                public override Object Find(ulong id)
                {
                    ObjectType value;
                    if (!objects.TryGetValue(id, out value))
                    {
                        throw new Registry.FindError(Registry.FindError.IdValue, id.ToString());
                    }
                    return value;
                }
            }
        }
    }
}
