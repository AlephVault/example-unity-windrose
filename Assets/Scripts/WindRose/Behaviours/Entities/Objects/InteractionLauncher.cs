using UnityEngine;
using GabTab.Behaviours;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            /// <summary>
            ///   <para>
            ///     This component's sole purpose is to retrieve the topmost
            ///       <see cref="UI.InteractionProvider"/> instance. This, to be able to
            ///       trigger (launch) any interaction using such retrieved component.
            ///   </para>
            ///   <para>
            ///     To understand the role of <see cref="UI.InteractionProvider"/> and why
            ///       such component must be in the topmost object of the current
            ///       WindRose level objects' hierarchy, see that class' documentation and
            ///       then come back.
            ///   </para>
            ///   <para>
            ///     The spirit of this component is that it may provide a mean for other
            ///       components (perhaps reacting to events in <see cref="TriggerZone"/>
            ///       or related classes) to retrieve the interaction provider and start
            ///       an interaction with the player.
            ///   </para>
            /// </summary>
            public class InteractionLauncher : MonoBehaviour
            {
                /// <summary>
                ///   Retrieves the <see cref="UI.InteractionProvider"/> up in the
                ///     objects' hierarchy. See this class' documentation to understand
                ///     more.
                /// </summary>
                public InteractiveInterface InteractionTab
                {
                    get
                    {
                        return GetComponentInParent<UI.InteractionProvider>().InteractionTab;
                    }
                }
            }
        }
    }
}
