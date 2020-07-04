using System;
using System.Collections;
using System.Linq;
using static ResourceServers.Registries.V2.List;

namespace ResourceServers
{
    namespace Loaders
	{
        namespace V2
		{
            /// <summary>
            ///   This is a loader for the V2 resources roots.
            /// </summary>
            public class Loader : Loaders.Loader
            {
                private class Reference
                {
                    public string Key { get; set; }
                    public string Caption { get; set; }
                }

                private class RootResult
                {
                    public string Caption { get; set; }
                    public Reference[] Packages { get; set; }
                }

                private class PackageResult
                {
                    public string Caption { get; set; }
                    public Reference[] Spaces { get; set; }
                }

                private class SpaceResult
                {
                    public string Caption { get; set; }
                    public Reference[] Lists { get; set; }
                }

                public override IEnumerable Populate(string baseUrl, string jsonBody, Registries.Registry target)
				{
                    Registries.V2.Registry v2Target = (Registries.V2.Registry)target;
                    RootResult body = JSON.Parse<RootResult>(jsonBody);
                    foreach(Reference packageRef in body.Packages)
                    {
                        string packageUrl = baseUrl + "/" + packageRef.Key;
                        Func<string, IEnumerable> processor = delegate (string packageJsonBody) { return processPackage(baseUrl, packageJsonBody, v2Target, packageRef.Key); };
                        foreach(var obj in JSON.Fetch(packageUrl, processor)) {
                            yield return obj;
                        }
                    }
				}

                private IEnumerable processPackage(string packageUrl, string packageJsonBody, Registries.V2.Registry target, string packageKey)
                {
                    PackageResult body = JSON.Parse<PackageResult>(packageJsonBody);
                    foreach (var spaceRef in body.Spaces)
                    {
                        string spaceUrl = packageUrl + "/" + spaceRef.Key;
                        Func<string, IEnumerable> processor = delegate (string spaceJsonBody) { return processSpace(spaceUrl, spaceJsonBody, target, packageKey, spaceRef.Key); };
                        foreach (var obj in JSON.Fetch(spaceUrl, processor))
                        {
                            yield return obj;
                        }
                    }
                }

                private IEnumerable processSpace(string spaceUrl, string spaceJsonBody, Registries.V2.Registry target, string packageKey, string spaceKey)
                {
                    SpaceResult body = JSON.Parse<SpaceResult>(spaceJsonBody);
                    foreach (var listRef in body.Lists)
                    {
                        string listUrl = spaceUrl + "/" + listRef.Key;
                        Func<string, IEnumerable> processor = delegate (string listJsonBody) { return processList(listUrl, listJsonBody, target, packageKey, spaceKey, listRef.Key); };
                        foreach (var obj in JSON.Fetch(listUrl, processor))
                        {
                            yield return obj;
                        }
                    }
                }

                private IEnumerable processList(string spaceUrl, string spaceJsonBody, Registries.V2.Registry target, string packageKey, string spaceKey, string listKey)
                {
                    SpaceResult body = JSON.Parse<SpaceResult>(spaceJsonBody);
                    foreach (var spaceRef in body.Lists)
                    {
                        string listUrl = spaceUrl + "/" + spaceRef.Key;
                        Func<string, IEnumerable> processor = delegate (string listJsonBody) {
                            try
                            {
                                target[packageKey, spaceKey, listKey].Populate(listJsonBody);
                            }
                            catch (StageError)
                            {
                                // Do nothing.
                            }
                            return Enumerable.Empty<object>();
                        };
                        foreach (var obj in JSON.Fetch(listUrl, processor))
                        {
                            yield return obj;
                        }
                    }
                }
            }
        }
    }
}
