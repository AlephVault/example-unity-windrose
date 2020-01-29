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

					[RequireComponent(typeof(BasicSingleSimpleInventoryView))]
					public class BasicSimpleBagView : MonoBehaviour {
                        /**
						 * This is an implementation of SimpleInventoryView to be straight used
						 *   by a WindRose-BackPack Bag object (this is an inventory existing as
						 *   tied to an object, and thus living inside a map). Since it belongs
						 *   to a map, it will be able to drop / grab items in / from the floor
						 *   (drop layer) of the map.
						 */

                        private BasicSingleSimpleInventoryView inventoryView;

                        void Awake()
                        {
                            inventoryView = GetComponent<BasicSingleSimpleInventoryView>();
                        }

                        public void DropSelected()
						{
							if (inventoryView.SelectedPosition != null)
							{
								int position = inventoryView.SelectedPosition.Value;
								inventoryView.Unselect();
								inventoryView.SourceSingleInventory.GetComponent<WindRose.Behaviours.Entities.Objects.Bags.SimpleBag>().Drop(position);
								inventoryView.Refresh();
							}
						}

						public void Pick()
						{
							int? finalPosition;
							inventoryView.SourceSingleInventory.GetComponent<WindRose.Behaviours.Entities.Objects.Bags.SimpleBag>().Pick(out finalPosition);
							if (finalPosition != null && inventoryView.SelectedPosition == null)
							{
								inventoryView.Select(finalPosition.Value);
								inventoryView.Refresh();
							}
						}
					}
				}
			}
		}
	}
}
