using System.Collections;
using System.Collections.Generic;
using Support.Behaviours;
using UnityEngine;


namespace WindRose
{
	namespace Behaviours
	{
		namespace UI
		{
			namespace Inventories
			{
				namespace SimpleBag
				{
					using BackPack.Behaviours.UI.Inventory.Simple;

					[RequireComponent(typeof(BasicSingleSimpleInventoryView))]
					[RequireComponent(typeof(Throttler))]
					public class SimpleBagControl : MonoBehaviour {
						/**
						 * This is an implementation of SimpleInventoryView to be straight used
						 *   by a WindRose-BackPack Bag object (this is an inventory existing as
						 *   tied to an object, and thus living inside a map). Since it belongs
						 *   to a map, it will be able to drop / grab items in / from the floor
						 *   (drop layer) of the map.
						 */

						[SerializeField]
						private bool useKeyInteraction = true;

						[SerializeField]
						private KeyCode dropKey = KeyCode.D;

						[SerializeField]
						private KeyCode pickKey = KeyCode.A;

						private Throttler throttler;
						private BasicSingleSimpleInventoryView inventoryView;

						void Awake()
						{
							throttler = GetComponent<Throttler>();
							inventoryView = GetComponent<BasicSingleSimpleInventoryView> ();
						}

						void Update()
						{
							if (Input.GetKey(dropKey))
							{
								throttler.Throttled(DropSelected);
							}
							else if (Input.GetKey(pickKey))
							{
								throttler.Throttled(Pick);
							}
						}

						void DropSelected()
						{
							if (inventoryView.SelectedPosition != null)
							{
								int position = inventoryView.SelectedPosition.Value;
								inventoryView.Unselect();
								inventoryView.SourceSingleInventory.GetComponent<WindRose.Behaviours.Entities.Objects.Bags.SimpleBag>().Drop(position);
								inventoryView.Refresh();
							}
						}

						void Pick()
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
