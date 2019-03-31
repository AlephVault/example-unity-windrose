using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities
        {
            namespace Visuals
            {
                /// <summary>
                ///   Visual behaviours have a special contract that is related
                ///     to its visual management only. The contract involves
                ///     one method that are documented: <see cref="DoUpdate"/>.
                /// </summary>
                [RequireComponent(typeof(Visual))]
                public class VisualBehaviour : MonoBehaviour
                {
                    /// <summary>
                    ///   Triggered when the underlying visual is updated.
                    /// </summary>
                    public virtual void DoUpdate() { }
                }
            }
        }
    }
}
