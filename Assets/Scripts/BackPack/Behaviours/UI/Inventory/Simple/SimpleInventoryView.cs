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
				namespace Simple
				{
					[RequireComponent(typeof(Image))]
					public class SimpleInventoryView : SingleInventoryView {
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

						private SimpleInventoryViewPageLabel pageLabel;
						private SimpleInventoryViewSelectedItemLabel selectedItemLabel;
						public int? SelectedPosition { get; private set; }
						public BackPack.Types.Inventory.Stacks.Stack SelectedItem {
							get {
								return (SelectedPosition != null) ? SourceSingleInventory.Find(SelectedPosition.Value) : null;
							}
						}

						protected override void Awake()
						{
							base.Awake();
							pageLabel = Layout.RequireComponentInChildren<SimpleInventoryViewPageLabel>(this);
							selectedItemLabel = Layout.RequireComponentInChildren<SimpleInventoryViewSelectedItemLabel>(this);
							Layout.RequireComponentInChildren<SimpleInventoryViewNextButton>(this).GetComponent<Button>().onClick.AddListener(delegate() { Next(); });
							Layout.RequireComponentInChildren<SimpleInventoryViewPrevButton>(this).GetComponent<Button>().onClick.AddListener(delegate () { Prev(); });
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
									((SimpleInventoryViewItem)items[iSlot]).SetSelection(iSlot == slot);
								}
							}
							else
							{
								for (int iSlot = 0; iSlot < items.Length; iSlot++)
								{
									((SimpleInventoryViewItem)items[iSlot]).SetSelection(false);
								}
							}
						}
					}
				}
			}
		}
	}
}
