using Support.Utils;
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
					[RequireComponent(typeof(Button))]
					public class SimpleInventoryViewItem : SingleInventoryView.SingleInventoryViewItem {
						/**
					     * This component will have three parts:
					     * - The selection glow: It will become visible on SetSelection().
					     * - The image: It will display the icon.
					     * - The label: It will display the quantity (if such is int or float - bools are not represented).
					     * 
					     * This component will be a button as well.
					     */

						private SimpleInventoryViewItemIcon iconHolder;
						private SimpleInventoryViewItemSelectionGlow glow;
						private SimpleInventoryViewItemQuantityLabel quantityLabel;
						private int? targetPosition;

						void Awake()
						{
							quantityLabel = GetComponentInChildren<SimpleInventoryViewItemQuantityLabel>();
							iconHolder = GetComponentInChildren<SimpleInventoryViewItemIcon>();
							glow = GetComponentInChildren<SimpleInventoryViewItemSelectionGlow>();
							SimpleInventoryView parent = Layout.RequireComponentInParent<SimpleInventoryView>(Layout.RequireComponentInParent<GridLayoutGroup>(this).gameObject);
							GetComponent<Button>().onClick.AddListener(delegate ()
								{
									if (targetPosition != null)
									{
										parent.Select(targetPosition.Value);
									}
								});
						}

						void Start()
						{
							Clear();
							SetSelection(false);
						}

						public override void Clear()
						{
							targetPosition = null;
							iconHolder.SetIcon(null);
							quantityLabel.SetQuantity(null);
						}

						public override void Set(int position, Sprite icon, string caption, object quantity)
						{
							// Caption will be ignored in this example.        
							targetPosition = position;
							iconHolder.SetIcon(icon);
							quantityLabel.SetQuantity(quantity);
						}

						public void SetSelection(bool selected)
						{
							glow.gameObject.SetActive(selected);
						}
					}
				}
			}
		}
	}
}
