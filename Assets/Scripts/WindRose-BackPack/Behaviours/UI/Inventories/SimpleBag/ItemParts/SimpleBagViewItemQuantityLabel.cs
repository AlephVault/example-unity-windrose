using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
					[RequireComponent(typeof(Text))]
					public class SimpleBagViewItemQuantityLabel : MonoBehaviour {
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
