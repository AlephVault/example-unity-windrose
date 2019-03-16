using System;
using UnityEngine;
using UnityEngine.Events;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Entities.Objects
        {
            namespace CommandExchange
            {
                namespace Misc
                {
                    /// <summary>
                    ///   A command is a simple game object that will be cast by a <see cref="CloseCommandSender"/>
                    ///     or other classes creating it (like dependent classes), and received by
                    ///     <see cref="CommandReceiver"/> or dependent classes.
                    /// </summary>
                    [RequireComponent(typeof(CircleCollider2D))]
                    class Command : MonoBehaviour
                    {
                        /// <summary>
                        ///   Usually, the sender of the command (this is the case when being cast by
                        ///     <see cref="CloseCommandSender"/>).
                        /// </summary>
                        [HideInInspector]
                        public GameObject sender;

                        /// <summary>
                        ///   The name of the command.
                        /// </summary>
                        [HideInInspector]
                        public string name;

                        /// <summary>
                        ///   Additional arguments this command may have.
                        /// </summary>
                        [HideInInspector]
                        public object[] arguments;

                        void Start()
                        {
                            CircleCollider2D collider = GetComponent<CircleCollider2D>();
                            collider.radius = 0;
                            collider.isTrigger = true;
                        }
                    }
                }
            }
        }
    }
}
