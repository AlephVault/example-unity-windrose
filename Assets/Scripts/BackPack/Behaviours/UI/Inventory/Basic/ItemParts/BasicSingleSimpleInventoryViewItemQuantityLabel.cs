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
