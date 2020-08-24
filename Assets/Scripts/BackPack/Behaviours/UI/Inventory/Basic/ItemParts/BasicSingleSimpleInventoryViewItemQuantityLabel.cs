using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
                    ///   This is a marker behaviour so the <see cref="BasicSingleSimpleInventoryViewItem" />
                    ///     ancestor can identify the label to put the item's quantity into.
                    /// </summary>
					[RequireComponent(typeof(Text))]
					public class BasicSingleSimpleInventoryViewItemQuantityLabel : MonoBehaviour {
						/**
					     * This class represents the quantity on its label.
					     */

						private Text text;

						void Awake()
						{
							text = GetComponent<Text>();
						}

						public void SetQuantity(object quantity)
						{
							if (quantity == null || quantity is bool)
							{
								text.text = "";
							}
							else
							{
								text.text = quantity.ToString();
							}
						}
					}
				}
			}
		}
	}
}
