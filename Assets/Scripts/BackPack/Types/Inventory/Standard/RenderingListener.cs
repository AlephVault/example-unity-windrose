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
                using ScriptableObjects.Inventory.Items;

                /// <summary>
                ///   Listeners will refresh item and quantity for a stack, and will account
                ///     for a single container and an 1D-indexed intra-container positioning.
                /// </summary>
                public interface RenderingListener
                {
                    void Connected();
                    void UpdateStack(int stackPosition, Item item, object quantity);
                    void RemoveStack(int position);
                    void Clear();
                    void Disconnected();
                }
            }
        }
    }
}
