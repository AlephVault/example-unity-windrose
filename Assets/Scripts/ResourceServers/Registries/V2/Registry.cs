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
                    private static string GetMessage(string type, string value)
                    {
                        switch(type)
                        {
                            case "format":
                                return "Invalid path format: " + value;
                            case "package":
                                return "Package not found: " + value;
                            case "space":
                                return "Space not found: " + value;
                            case "list":
                                return "List not found: " + value;
                            case "id-format":
                                return "Invalid id format (not an UInt64): " + value;
                            case "id-value":
                                return "Id not found: " + value;
                            default:
                                return "Unknown error: " + type + " / " + value;
                        }
                    }

                    public FindError(string type, string value) : base(GetMessage(type, value)) {}
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
                            throw new FindError("id-format", idstr);
                        }

                        Package package;
                        if (!packages.TryGetValue(packagekey, out package))
                        {
                            throw new FindError("package", packagekey);
                        }

                        Space space;
                        if (!package.TryGetValue(spacekey, out space))
                        {
                            throw new FindError("space", spacekey);
                        }

                        List list;
                        if (!space.TryGetValue(listkey, out list))
                        {
                            throw new FindError("list", listkey);
                        }

                        return list.Find(id);
                    }
                    else
                    {
                        throw new FindError("format", path);
                    }
                }
            }
        }
    }
}