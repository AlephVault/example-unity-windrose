namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Types
    {
        namespace Constants
        {
            /// <summary>
            ///   Several constants related to scopes and their prefabs.
            /// </summary>
            public static class Scope
            {
                ////// About the scope prefab index.

                /// <summary>
                ///   The maximum number of allowed scope prefabs.
                /// </summary>
                public static uint MaxScopePrefabs = 0xffffff00; 

                /// <summary>
                ///   The dummy prefab index to select one of the
                ///   defined prefabs set as "default" ones in the
                ///   client side (i.e. those that are already
                ///   well known both in the client side and the
                ///   server side to be spawned as default, static,
                ///   maps, in contrast to those that might be
                ///   spawned dynamically).
                /// </summary>
                public static uint DefaultPrefab = 0xffffffff;

                /// <summary>
                ///   The dummy prefab index for "Limbo".
                /// </summary>
                public static uint LimboPrefab = 0xffffff00;

                /// <summary>
                ///   The dummy prefab index for "Maintenance".
                /// </summary>
                public static uint MaintenancePrefab = 0xffffff01;

                ////// About the scope index.

                /// <summary>
                ///   The maximum number of allowed scopes.
                /// </summary>
                public static uint MaxScopes = 0xffffff00;

                // Please note: there is no "Default" prefab index.

                /// <summary>
                ///   The "Limbo" scope.
                /// </summary>
                public static uint Limbo = 0xffffff00;

                /// <summary>
                ///   The "Maintenance" scope.
                /// </summary>
                public static uint Maintenance = 0xffffff01;
            }
        }
    }
}
