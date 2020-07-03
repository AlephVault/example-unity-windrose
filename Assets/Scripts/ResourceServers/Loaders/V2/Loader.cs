using System;
using System.Collections;

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
                    // TODO
                    return null;
				}
            }
        }
    }
}
