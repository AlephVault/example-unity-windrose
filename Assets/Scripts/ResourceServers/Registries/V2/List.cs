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
            ///   Lists preserve in runtime all the server-side
            ///     fetched and/or validated locally-added elements
            ///     against the server specification. They allow
            ///     the developer to use those objects at runtime.
            /// </summary>
            public abstract class List : ScriptableObject
            {
                /// <summary>
                ///   A stage error tells whether an operation like populate,
                ///     inflate, or find is not allowed in the current context
                ///     of this object: it has not yet populated / already has,
                ///     it has not yet inflated the resources / already has.
                /// </summary>
                public class StageError : Exception
                {
                    public const string NoFetchedData = "no-fetched-data";
                    public const string DataAlreadyFetched = "data-already-fetched";
                    public const string NoInflatedResources = "no-inflated-resources";
                    public const string ResourcesAlreadyInflated = "resources-already-inflated";

                    private static string GetMessage(string type)
                    {
                        switch (type)
                        {
                            case NoFetchedData:
                                return "No data is currently fetched for this list";
                            case DataAlreadyFetched:
                                return "Data is already fetched for this list";
                            case NoInflatedResources:
                                return "No resources are currently inflated for this list";
                            case ResourcesAlreadyInflated:
                                return "Resources are already inflated for this list";
                            default:
                                return "Unkown error: " + type;
                        }
                    }

                    public string Type { get; private set; }

                    public StageError(string type) : base(GetMessage(type))
                    {
                        Type = type;
                    }
                }

                /// <summary>
                ///   This class represents arbitrary data received
                ///     from the V2 list structure.
                /// </summary>
                public class ListContent
                {
                    public string Caption { get; set; }
                    public Dictionary<string, Dictionary<string, object>> Resources { get; set; }
                }

                /// <summary>
                ///   <para>
                ///     These objects are local resources that may
                ///       be added. These local resources may be
                ///       set at design time and will be merged
                ///       in the final objects mapping.
                ///   </para>
                ///   <para>
                ///     Please note: If another object with the
                ///       same id of a local object is fetched, it
                ///       will override the local object in this
                ///       list.
                ///   </para>
                /// </summary>
                [SerializeField]
                protected SerializableDictionary<ulong, Object> localObjects = new SerializableDictionary<ulong, Object>();

                // This is the downloaded data. This data will be
                // downloaded first from the servers and then the
                // whole populated list will be inflated. This
                // data will be populated only once.
                private ListContent fetchedData = null; 

                // These objects are the final, fetched, objects
                // in the list. When trying to find an object
                // by its ID, this list will be used. This data
                // will be populated only once.
                protected Dictionary<ulong, Object> finalObjects = null;

                /// <summary>
                ///   Inflates an object of the target type
                ///     from the given resource data. It must
                ///     be implemented per-list. This process
                ///     may involve one of: 1. Process the
                ///     body of the resource data; 2. Check
                ///     ID only, and that the object is locally
                ///     present among the local objects.
                /// </summary>
                /// <param name="id">The ID of the object being inflated</param>
                /// <param name="resourceData">The source data to hidrate from</param>
                /// <param name="registry">The registry to feed external dependencies from</param>
                /// <returns>The inflated object</returns>
                protected abstract Object Inflate(ulong id, Dictionary<string, object> resourceData, Registry registry);

                /// <summary>
                ///   Parses the json body and keeps it prepopulated
                ///     to be inflated later.
                /// </summary>
                /// <param name="body">The JSON body to parse and populate from</param>
                public void Populate(string body)
                {
                    if (fetchedData != null)
                    {
                        throw new StageError(StageError.DataAlreadyFetched);
                    }
                    fetchedData = JSON.Parse<ListContent>(body);
                }

                /// <summary>
                ///   Inflates all the fetched data, based on an existing registry.
                /// </summary>
                /// <param name="registry">The registry to feed external dependencies from</param>
                public void InflateAll(Registry registry)
                {
                    if (fetchedData == null)
                    {
                        throw new StageError(StageError.NoFetchedData);
                    }
                    if (finalObjects != null)
                    {
                        throw new StageError(StageError.ResourcesAlreadyInflated);
                    }

                    var objects = new SerializableDictionary<ulong, Object>();
                    foreach (KeyValuePair<ulong, Object> pair in objects)
                    {
                        objects[pair.Key] = pair.Value;
                    }
                    foreach (KeyValuePair<string, Dictionary<string, object>> pair in fetchedData.Resources)
                    {
                        ulong id = ulong.Parse(pair.Key);
                        objects[id] = Inflate(id, pair.Value, registry);
                    }
                    finalObjects = objects;
                }

                /// <summary>
                ///   Finding an object involves checking the
                ///   inner dictionary.
                /// </summary>
                /// <param name="id">The ID to lookup</param>
                /// <returns>The object with the given ID</returns>
                public Object Find(ulong id)
                {
                    Object value;
                    if (finalObjects == null)
                    {
                        throw new StageError(StageError.NoInflatedResources);
                    }
                    if (!finalObjects.TryGetValue(id, out value))
                    {
                        throw new Registry.FindError(Registry.FindError.IdValue, id.ToString());
                    }
                    return value;
                }
            }
        }
    }
}
