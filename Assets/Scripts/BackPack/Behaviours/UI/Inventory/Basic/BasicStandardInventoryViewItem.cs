﻿using GMM.Utils;
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
                    /// <summary>
                    ///   A single-simple inventory item that accounts for being selected, as well. Used in conjunction to
                    ///     <see cref="BasicStandardInventoryView" />.
                    /// </summary>
					[RequireComponent(typeof(Button))]
					public class BasicStandardInventoryViewItem : StandardInventoryView.StandardInventoryViewItem {
						/**
					     * This component will have three parts:
					     * - The selection glow: It will become visible on SetSelection().
					     * - The image: It will display the icon.
					     * - The label: It will display the quantity (if such is int or float - bools are not represented).
					     * 
					     * This component will be a button as well.
					     */

						private BasicStandardInventoryViewItemIcon iconHolder;
						private BasicStandardInventoryViewItemSelectionGlow glow;
						private BasicStandardInventoryViewItemQuantityLabel quantityLabel;
						private int? targetPosition;

						void Awake()
						{
							quantityLabel = GetComponentInChildren<BasicStandardInventoryViewItemQuantityLabel>();
							iconHolder = GetComponentInChildren<BasicStandardInventoryViewItemIcon>();
							glow = GetComponentInChildren<BasicStandardInventoryViewItemSelectionGlow>();
							BasicStandardInventoryView parent = Layout.RequireComponentInParent<BasicStandardInventoryView>(Layout.RequireComponentInParent<GridLayoutGroup>(this).gameObject);
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
