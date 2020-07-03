using System;
using GMM.Types;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ResourceServers
{
    namespace Registries
    {
        namespace V2
        {
            /// <summary>
            ///   V2 registres have 4 levels to search in:
            ///     Registry -> Package -> Space -> List.
            /// </summary>
            public class Registry : Registries.Registry
            {
                /// <summary>
                ///   This exception is thrown when a V2 registry
                ///     fails to find an asset from a path.
                /// </summary>
                public new class FindError : Registries.Registry.FindError
                {
                    public const string Format = "format";
                    public const string Package = "package";
                    public const string Space = "space";
                    public const string List = "list";
                    public const string IdFormat = "id-format";
                    public const string IdValue = "id-value";

                    private static string GetMessage(string type, string value)
                    {
                        switch(type)
                        {
                            case Format:
                                return "Invalid path format: " + value;
                            case Package:
                                return "Package not found: " + value;
                            case Space:
                                return "Space not found: " + value;
                            case List:
                                return "List not found: " + value;
                            case IdFormat:
                                return "Invalid id format (not an UInt64): " + value;
                            case IdValue:
                                return "Id not found: " + value;
                            default:
                                return "Unknown error: " + type + " / " + value;
                        }
                    }

                    public FindError(string type, string value) : base(GetMessage(type, value)) {}
                }

                /// <summary>
                ///   Lists have to be implemented to return an
                ///     arbitrary Unity object given an id. If
                ///     somehow this fails (e.g. invalid ID)
                ///     then a V2.Registry.FetchError must be
                ///     raised.
                /// </summary>
                public abstract class List : ScriptableObject
                {
                    /// <summary>
                    ///   Populates its content via a received
                    ///   JSON body. Each list will know how to
                    ///   do this by itself.
                    /// </summary>
                    /// <param name="body">The JSON body to parse and populate from</param>
                    public abstract void Populate(string body);

                    /// <summary>
                    ///   Finds an object by its ID. Must throw
                    ///     <see cref="FindError"/> if a resource
                    ///     with that ID is not found.
                    /// </summary>
                    /// <param name="id">The ID to look for</param>
                    /// <returns>The object for the given ID</returns>
                    public abstract UnityEngine.Object Find(ulong id);
                }

                /// <summary>
                ///   A dictionary of url keys and their lists.
                /// </summary>
                [Serializable]
                public class Space : SerializableDictionary<string, List> {}

                /// <summary>
                ///   A dictionary of url keys and their spaces.
                /// </summary>
                [Serializable]
                public class Package : SerializableDictionary<string, Space> {}

                /// <summary>
                ///   A dictionary of url keys and packages.
                /// </summary>
                [Serializable]
                public class PackageSet : SerializableDictionary<string, Package> {}

                [SerializeField]
                private PackageSet packages;

                private void OnEnable()
                {
                    if (packages == null)
                    {
                        packages = new PackageSet();
                    }
                }

                private const string pattern = @"^/([^/]+)/([^/]+)/([^/]+)/(\d+)$";

                /// <summary>
                ///   Takes a 4-parts url like /foo/bar/baz/3
                ///     and looks for a resource in package "foo",
                ///     space "bar", list "baz", with id 3. If not
                ///     found, then an error will be raised.
                /// </summary>
                /// <param name="path"></param>
                /// <returns></returns>
                public override UnityEngine.Object Find(string path)
                {
                    Match match = Regex.Match(path, pattern);
                    if (match.Success)
                    {
                        GroupCollection groups = match.Groups;
                        string packagekey = groups[1].Value;
                        string spacekey = groups[2].Value;
                        string listkey = groups[3].Value;
                        string idstr = groups[4].Value;
                        ulong id;
                        if (!ulong.TryParse(idstr, out id))
                        {
                            throw new FindError(FindError.IdFormat, idstr);
                        }

                        Package package;
                        if (!packages.TryGetValue(packagekey, out package))
                        {
                            throw new FindError(FindError.Package, packagekey);
                        }

                        Space space;
                        if (!package.TryGetValue(spacekey, out space))
                        {
                            throw new FindError(FindError.Space, spacekey);
                        }

                        List list;
                        if (!space.TryGetValue(listkey, out list))
                        {
                            throw new FindError(FindError.List, listkey);
                        }

                        return list.Find(id);
                    }
                    else
                    {
                        throw new FindError(FindError.Format, path);
                    }
                }
            }
        }
    }
}