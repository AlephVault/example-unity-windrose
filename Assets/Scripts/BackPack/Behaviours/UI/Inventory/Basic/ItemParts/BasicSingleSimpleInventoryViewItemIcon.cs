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
					[RequireComponent(typeof(Image))]
					public class BasicSingleSimpleInventoryViewItemIcon : MonoBehaviour {
						/**
					     * This class is the icon of a SampleSimpleInventoryViewItemIton.
					     */

						private Image image;

						void Awake()
						{
							image = GetComponent<Image>();
						}

						public void SetIcon(Sprite icon)
						{
							image.sprite = icon;
							image.enabled = icon != null;
						}
					}
				}
			}
		}
	}
}
