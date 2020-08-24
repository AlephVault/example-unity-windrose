using System.Collections;
using System.Collections.Generic;
using GMM.Behaviours;
using UnityEngine;


namespace WindRose
{
	namespace Behaviours
	{
		namespace UI
		{
			namespace Inventory
			{
				namespace SimpleBag
				{
					using BackPack.Behaviours.UI.Inventory.Basic;
                    using BackPack.Behaviours.Inventory.Single;
                    using BackPack.Types.Inventory.Stacks;
                    using Entities.Objects.Bags;

                    /// <summary>
                    ///   This is a basic control for the SimpleBag component. It will have
                    ///     two involved parts: It will, to start, need a component of type
                    ///     <see cref="BasicSingleSimpleInventoryView" />, and a reference
                    ///     to a <see cref="SimpleBag"/> component (which can be changed
                    ///     any time). On start, and/or when the reference is changed, the
                    ///     control's view will be connected to the start/new value of the
                    ///     bag's underlying invetory's rendering strategy. This implies
                    ///     that both objects belong to the same scope (e.g. local games)
                    ///     and also that these components will interact.
                    /// </summary>
					[RequireComponent(typeof(BasicSingleSimpleInventoryView))]
					public class BasicSimpleBagControl : MonoBehaviour {
                        /**
						 * This is an implementation of SimpleInventoryView to be straight used
						 *   by a WindRose-BackPack Bag object (this is an inventory existing as
						 *   tied to an object, and thus living inside a map). Since it belongs
						 *   to a map, it will be able to drop / grab items in / from the floor
						 *   (drop layer) of the map.
						 */

                        private BasicSingleSimpleInventoryView inventoryView;

                        [SerializeField]
                        private SimpleBag bag;

                        private void Awake()
                        {
                            inventoryView = GetComponent<BasicSingleSimpleInventoryView>();
                        }

                        private void Start()
                        {
                            if (bag)
                            {
                                bag.GetComponent<InventorySingleSimpleRenderingManagementStrategy>().AddSubRenderer(inventoryView);
                            }
                        }

                        /// <summary>
                        ///   Drops the currently selected item. Dropping it implies that the
                        ///     underlying <see cref="World.Layers.Drop.DropLayer"/> will get
                        ///     such dropped item, if the drop layer is in use. It will also
                        ///     refresh the related view to clear the selection, but because
                        ///     of the drop.
                        /// </summary>
                        public void DropSelected()
						{
							if (inventoryView.SelectedPosition != null)
							{
								int position = inventoryView.SelectedPosition.Value;
								inventoryView.Unselect();
								bag.Drop(position);
                                //inventoryView.Refresh();
							}
						}

                        /// <summary>
                        ///   Picks an item from the ground, if the map has a layer of type
                        ///     <see cref="World.Layers.Drop.DropLayer"/>. The just-picked
                        ///     item will be the selected one in the view.
                        /// </summary>
						public void Pick()
						{
							int? finalPosition;
							bag.Pick(out finalPosition);
							if (finalPosition != null && inventoryView.SelectedPosition == null)
							{
								inventoryView.Select(finalPosition.Value);
								inventoryView.Refresh();
							}
						}

                        /// <summary>
                        ///   Gets the currently selected stack, by considering both the
                        ///     selection in the UI and the content in the inventory.
                        /// </summary>
                        public Stack SelectedItem
                        {
                            get
                            {
                                return (inventoryView.SelectedPosition != null) ? bag.Inventory.Find(inventoryView.SelectedPosition.Value) : null;
                            }
                        }
                    }
                }
			}
		}
	}
}
