using BackPack.ScriptableObjects.Inventory.Items;
using BackPack.ScriptableObjects.Inventory.Items.RenderingStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackPack
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Standard
            {
                /// <summary>
                ///   A broadcaster has a lot of listeners who will receive updates
                ///     when commanded from this class. This applies both for direct
                ///     and indirect rendering broadcast (i.e. the sender may be an
                ///     inventory owner, or also receiving broadcast from another
                ///     source).
                /// </summary>
                [Serializable]
                public class RenderingBroadcaster
                {
                    /// <summary>
                    ///   Tells when trying to add a null <see cref="RenderingListener"/>
                    ///     when calling <see cref="AddListener(RenderingListener)"/>.
                    /// </summary>
                    public class InvalidListenerException : GMM.Types.Exception
                    {
                        public InvalidListenerException(string message) : base(message) { }
                    }

                    // This function will be invoked when needed to perform a full
                    //   start on a listener.
                    private Action<RenderingListener> fullStartCallback;

                    // Effective set of the listeners to be used, either by preloading from the editor or by
                    //   adding / removing listeners.
                    private HashSet<RenderingListener> listenersSet = new HashSet<RenderingListener>();

                    public RenderingBroadcaster(Action<RenderingListener> fullStart)
                    {
                        fullStartCallback = fullStart;
                    }

                    /// <summary>
                    ///   Adds a rendering listener to this rendering broadcaster. The listener will
                    ///     refresh with this listener's data accordingly, and will be synchronized until
                    ///     it is removed by a call to <see cref="RemoveListener(RenderingListener)"/>.
                    /// </summary>
                    /// <param name="listener">The <see cref="RenderingListener"/> to add</param>
                    /// <returns>Whether it could be added, or it was already added</returns>
                    public bool AddListener(RenderingListener listener)
                    {
                        if (listener == null)
                        {
                            throw new InvalidListenerException("Listener to add cannot be null");
                        }

                        if (listenersSet.Contains(listener))
                        {
                            return false;
                        }

                        listenersSet.Add(listener);
                        listener.Connected();
                        // We will force the listener to be cleared, and
                        // also refresh each item. This, to decouple from
                        // the inventory itself.
                        fullStartCallback(listener);
                        return true;
                    }

                    /// <summary>
                    ///   Removes a rendering listener from this rendering broadcaster. The listener will
                    ///     be cleared and removed.
                    /// </summary>
					/// <param name="listener">The <see cref="RenderingListener"/> to remove</param>
                    /// <returns>Whether it could be removed, or it wasn't connected here on first place</returns>
                    public bool RemoveListener(RenderingListener listener)
                    {
                        if (!listenersSet.Contains(listener))
                        {
                            return false;
                        }

                        listenersSet.Remove(listener);
                        listener.Disconnected();
                        return true;
                    }

                    /// <summary>
                    ///   Disconnects all the registered listeners.
                    /// </summary>
                    public void DisconnectAll()
                    {
                        HashSet<RenderingListener> cloned = new HashSet<RenderingListener>(listenersSet);
                        listenersSet.Clear();
                        foreach (RenderingListener listener in cloned)
                        {
                            listener.Disconnected();
                        }
                    }

                    /**************************************
                     * Methods to delegate the rendering on the listener
                     **************************************/

                    /// <summary>
                    ///   This method is invoked to broadcast a standard inventory clear
                    ///     and will delegate everything in the registered listeners:
                    ///     all their contents will be cleared.
                    /// </summary>
                    public void Clear()
                    {
                        foreach (RenderingListener listener in listenersSet)
                        {
                            listener.Clear();
                        }
                    }

                    /// <summary>
                    ///   This method is invoked to broadcast a standard inventory update
                    ///     of a stack and will delegate everything in the registered listeners:
                    ///     a single stack will be updated.
                    /// </summary>
                    public void Update(int stackPosition, Item item, object quantity)
                    {
                        foreach (RenderingListener listener in listenersSet)
                        {
                            listener.UpdateStack(stackPosition, item, quantity);
                        }
                    }

                    /// <summary>
                    ///   This method is invoked to broadcast a standard inventory removal
                    ///     of a stack and will delegate everything in the registered listeners:
                    ///     a single stack will be removed.
                    /// </summary>
                    public void Remove(int stackPosition)
                    {
                        foreach (RenderingListener listener in listenersSet)
                        {
                            listener.RemoveStack(stackPosition);
                        }
                    }
                }
            }
        }
    }
}
