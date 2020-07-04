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
                    public const string NotReady = "not-ready";
                    public const string Format = "format";
                    public const string Package = "package";
                    public const string Space = "space";
                    public const string List = "list";
                    public const string IdFormat = "id-format";
                    public const string IdValue = "id-value";

                    private static string GetMessage(string type, string value)
                    {
                        switch (type)
                        {
                            case NotReady:
                                return "List not ready (needs fetch & inflate): " + value;
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

                    public string Type { get; private set; }

                    public FindError(string type, string value) : base(GetMessage(type, value)) {
                        Type = type;
                    }
                }

                /// <summary>
                ///   A dictionary of url keys and their lists.
                /// </summary>
                [Serializable]
                public class Space : SerializableDictionary<string, List> { }

                /// <summary>
                ///   A dictionary of url keys and their spaces.
                /// </summary>
                [Serializable]
                public class Package : SerializableDictionary<string, Space> { }

                /// <summary>
                ///   A dictionary of url keys and packages.
                /// </summary>
                [Serializable]
                public class PackageSet : SerializableDictionary<string, Package> { }

                /// <summary>
                ///   The registered packages, spaces and lists in thie registry.
                /// </summary>
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
                ///   Gets a list in some specific resource path.
                /// </summary>
                /// <param name="package">The package key</param>
                /// <param name="space">The space key</param>
                /// <param name="list">The list key</param>
                /// <returns>The list under those keys, if present</returns>
                public List this[string package, string space, string list]
                {
                    get
                    {
                        return packages[package][space][list];
                    }
                }

                /// <summary>
                ///   Takes a 4-parts url like /foo/bar/baz/3
                ///     and looks for a resource in package "foo",
                ///     space "bar", list "baz", with id 3. If not
                ///     found, then an error will be raised.
                /// </summary>
                /// <param name="path">The full path</param>
                /// <returns>The found object, if present</returns>
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

                        try
                        {
                            return list.Find(id);
                        }
                        catch(List.StageError)
                        {
                            throw new FindError(FindError.NotReady, listkey);
                        }
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