using Support.Utils;
using Support.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackPack.Behaviours.UI.Inventory;


namespace BackPack
{
	namespace Behaviours
	{
		namespace UI
		{
			namespace Inventory
			{
				namespace Basic
				{
					[RequireComponent(typeof(Image))]
					public class BasicSingleSimpleInventoryView : SingleSimpleInventoryView {
						/**
					     * Instances of this class will have children objects.
					     * 
					     * 1. A Back button.
					     * 2. A Next button.
					     * 3. Several (6, 8 or 10) SimpleInventoryViewItem objects.
					     * 4. A label telling the current page number and max number.
					     * 5. A label telling the currently selected item.
					     * 
					     * This element is also the UI selecting the item and dropping
					     *   the item (right now, the entire stack by Drop(position)).
					     * 
					     * Methods:
					     * - Select(position) : If the position is among the selected
					     *     items, mark the item as the selected one and refresh the
					     *     whole inventory view.
					     * - Unselect() : Remove the index of selected one, and refresh
					     *     the whole inventory view.
					     * - Drop() : Invoke Drop(selected) on the SimpleInventory, if there
					     *     is a selected item. The inventory WILL refresh.
					     */

						private BasicSingleSimpleInventoryViewPageLabel pageLabel;
						private BasicSingleSimpleInventoryViewSelectedItemLabel selectedItemLabel;
						public int? SelectedPosition { get; private set; }
						public Types.Inventory.Stacks.Stack SelectedItem {
							get {
								return (SelectedPosition != null) ? SourceSingleInventory.Find(SelectedPosition.Value) : null;
							}
						}

						protected override void Awake()
						{
							base.Awake();
							pageLabel = Layout.RequireComponentInChildren<BasicSingleSimpleInventoryViewPageLabel>(this);
							selectedItemLabel = Layout.RequireComponentInChildren<BasicSingleSimpleInventoryViewSelectedItemLabel>(this);
							Layout.RequireComponentInChildren<BasicSingleSimpleInventoryViewNextButton>(this).GetComponent<Button>().onClick.AddListener(delegate() { Next(); });
							Layout.RequireComponentInChildren<BasicSingleSimpleInventoryViewPrevButton>(this).GetComponent<Button>().onClick.AddListener(delegate () { Prev(); });
						}

						protected void Start()
						{
						}

						public void Select(int position)
						{
							if (SourceSingleInventory.Find(position) != null)
							{
								if (position == SelectedPosition) return;

								int? positionToUnselect = SelectedPosition;
								SelectedPosition = position;
								if (positionToUnselect != null)
								{
									SourceSingleInventory.Blink(positionToUnselect.Value);
								}
								SourceSingleInventory.Blink(position);
								// Go to that page (useful if automatically selected)
								Go(PageFor(position));
								// Force refresh on general components as well
								AfterRefresh();
							}
						}

						public void Unselect()
						{
							if (SelectedPosition != null)
							{
								int positionToUnselect = SelectedPosition.Value;
								SelectedPosition = null;
								SourceSingleInventory.Blink(positionToUnselect);
								// Force refresh on general components as well
								AfterRefresh();
							}
						}

						/**
					     * This method is called on refresh (when you force to update everything).
					     */
						protected override void AfterRefresh()
						{
							pageLabel.SetPaginationLabel(Page, MaxPage());
							selectedItemLabel.SetCaption(SelectedPosition != null ? elements[SelectedPosition.Value].Second : "");
							// display the selection square over the appropriate slot, if the page is matched.
							if (SelectedPosition != null)
							{
								int slot = SlotFor(SelectedPosition.Value);
								for(int iSlot = 0; iSlot < items.Length; iSlot++)
								{
									((BasicSingleSimpleInventoryViewItem)items[iSlot]).SetSelection(iSlot == slot);
								}
							}
							else
							{
								for (int iSlot = 0; iSlot < items.Length; iSlot++)
								{
									((BasicSingleSimpleInventoryViewItem)items[iSlot]).SetSelection(false);
								}
							}
						}
					}
				}
			}
		}
	}
}
