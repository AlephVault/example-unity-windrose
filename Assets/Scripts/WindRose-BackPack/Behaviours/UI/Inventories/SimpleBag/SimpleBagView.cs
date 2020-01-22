using Support.Utils;
using Support.Behaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackPack.Behaviours.UI.Inventory;
using WindRose.Behaviours.Entities.Objects.Bags;

[RequireComponent(typeof(Throttler))]
[RequireComponent(typeof(Image))]
public class SimpleBagView : SingleInventoryView {
    /**
     * Instances of this class will have children objects.
     * 
     * 1. A Back button.
     * 2. A Next button.
     * 3. Several (6, 8 or 10) SimpleBagViewItem objects.
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
     * - Drop() : Invoke Drop(selected) on the SimpleBag, if there
     *     is a selected item. The inventory WILL refresh.
     */

    private int? selectedItem = null;
    private SimpleBagViewPageLabel pageLabel;
    private SimpleBagViewSelectedItemLabel selectedItemLabel;
    private Throttler throttler;

    [SerializeField]
    private KeyCode dropKey = KeyCode.D;

    [SerializeField]
    private KeyCode pickKey = KeyCode.A;

    protected override void Awake()
    {
        base.Awake();
        pageLabel = Layout.RequireComponentInChildren<SimpleBagViewPageLabel>(this);
        selectedItemLabel = Layout.RequireComponentInChildren<SimpleBagViewSelectedItemLabel>(this);
        Layout.RequireComponentInChildren<SimpleBagViewNextButton>(this).GetComponent<Button>().onClick.AddListener(delegate() { Next(); });
        Layout.RequireComponentInChildren<SimpleBagViewPrevButton>(this).GetComponent<Button>().onClick.AddListener(delegate () { Prev(); });
        throttler = GetComponent<Throttler>();
    }

	protected void Start()
	{
	}

    public void Select(int position)
    {
		if (SourceSingleInventory.Find(position) != null)
        {
            if (position == selectedItem) return;

            int? positionToUnselect = selectedItem;
            selectedItem = position;
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
        if (selectedItem != null)
        {
            int positionToUnselect = selectedItem.Value;
            selectedItem = null;
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
        selectedItemLabel.SetCaption(selectedItem != null ? elements[selectedItem.Value].Second : "");
        // display the selection square over the appropriate slot, if the page is matched.
        if (selectedItem != null)
        {
            int slot = SlotFor(selectedItem.Value);
            for(int iSlot = 0; iSlot < items.Length; iSlot++)
            {
                ((SimpleBagViewItem)items[iSlot]).SetSelection(iSlot == slot);
            }
        }
        else
        {
            for (int iSlot = 0; iSlot < items.Length; iSlot++)
            {
                ((SimpleBagViewItem)items[iSlot]).SetSelection(false);
            }
        }
    }

    /***********************************************************************
     * This is also the UI - some methods and key capturing will occur here.
     ***********************************************************************/

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
        if (selectedItem != null)
        {
            int position = selectedItem.Value;
            Unselect();
			SourceSingleInventory.GetComponent<SimpleBag>().Drop(position);
            AfterRefresh();
        }
    }

    void Pick()
    {
        int? finalPosition;
		SourceSingleInventory.GetComponent<SimpleBag>().Pick(out finalPosition);
        if (finalPosition != null && selectedItem == null)
        {
            Select(finalPosition.Value);
            AfterRefresh();
        }
    }
}
