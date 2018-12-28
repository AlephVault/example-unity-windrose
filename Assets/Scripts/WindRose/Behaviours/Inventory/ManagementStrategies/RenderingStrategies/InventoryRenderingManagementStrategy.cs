using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace RenderingStrategies
                {
                    using Types.Inventory.Stacks;

                    public abstract class InventoryRenderingManagementStrategy : InventoryManagementStrategy
                    {
                        /**
                         * Provides methods to reflect changes on the stacks being added, modified, or
                         *   removed on certain (inventory, stack) positions. Those methods are invoked
                         *   over the registered listeners.
                         * 
                         * Quite often the rendering strategies WILL depend on specific:
                         * - Positioning strategies: they deal with potentially many simultaneous inventories
                         *     managed in the same way (e.g. drop in the floor).
                         * - Spatial strategies: they deal with the contents of an inventory.
                         * - Usage strategies: they deal with the in-game logic of the objects. They could also
                         *     provide hints of how to render an item or UI component.
                         * - Per-object quantifying strategy: how to render the amount of elements in a stack.
                         * 
                         * Each rendering strategy will do its own, but WILL attend to these callbacks: they
                         *   MUST be implemented.
                         * 
                         * Perhaps these calls do not reflect changes immediately, but they may collect appropriate
                         *   data to be rendered later. It is up to the implementor to decide what to do.
                         */

                        public class InvalidListenerException : Types.Exception
                        {
                            public InvalidListenerException(string message) : base(message) {}
                        }

                        private HashSet<MonoBehaviour> listeners = new HashSet<MonoBehaviour>();

                        /**
                         * Adds a listener to this rendering strategy.
                         */
                        public bool AddListener(MonoBehaviour listener)
                        {
                            if (listener == null)
                            {
                                throw new InvalidListenerException("Listener to add cannot be null");
                            }
                            if (!AllowsListener(listener))
                            {
                                throw new InvalidListenerException(string.Format("Listener not accepted: {0}", listener));
                            }

                            if (listeners.Contains(listener))
                            {
                                return false;
                            }

                            listeners.Add(listener);
                            ListenerHasBeenAdded(listener);
                            return true;
                        }

                        /**
                         * Removes a listener from this rendering strategy.
                         */
                        public bool RemoveListener(MonoBehaviour listener)
                        {
                            if (!listeners.Contains(listener))
                            {
                                return false;
                            }

                            listeners.Remove(listener);
                            ListenerHasBeenRemoved(listener);
                            return true;
                        }

                        /**
                         * Triggers an update: clears everything.
                         */
                        public void EverythingWasCleared()
                        {
                            foreach (MonoBehaviour listener in listeners)
                            {
                                EverythingWasCleared(listener);
                            }
                        }

                        /**
                         * Triggers an update: stack added/refreshed.
                         */
                        public void StackWasUpdated(object containerPosition, object stackPosition, Stack stack)
                        {
                            foreach(MonoBehaviour listener in listeners)
                            {
                                StackWasUpdated(listener, containerPosition, stackPosition, stack);
                            }
                        }

                        /**
                         * Triggers an update: stack removed.
                         */
                        public void StackWasRemoved(object containerPosition, object stackPosition)
                        {
                            foreach(MonoBehaviour listener in listeners)
                            {
                                StackWasRemoved(listener, containerPosition, stackPosition);
                            }
                        }

                        /**
                         * Tells whether a listener may be added or not.
                         */
                        protected abstract bool AllowsListener(MonoBehaviour listener);
                        /**
                         * When a listener is added, this method initializes the content of the whole inventory on the listener.
                         */
                        protected abstract void ListenerHasBeenAdded(MonoBehaviour listener);
                        /**
                         * When a listener is removed, this method helps us clear the inventory display (emptying or hiding).
                         */
                        protected abstract void ListenerHasBeenRemoved(MonoBehaviour listener);
                        /**
                         * Event handler to clear everything.
                         */
                        protected abstract void EverythingWasCleared(MonoBehaviour listener);
                        /**
                         * Event handler to add/refresh a stack on the listener.
                         */
                        protected abstract void StackWasUpdated(MonoBehaviour listener, object containerPosition, object stackPosition, Stack stack);
                        /**
                         * Event handler to remove a stack from the listener.
                         */
                        protected abstract void StackWasRemoved(MonoBehaviour listener, object containerPosition, object stackPosition);
                    }
                }
            }
        }
    }
}
