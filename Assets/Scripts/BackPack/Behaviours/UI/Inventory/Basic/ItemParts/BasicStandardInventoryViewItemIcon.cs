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
                    ///   This is a marker behaviour so the <see cref="BasicStandardInventoryViewItem" />
                    ///     ancestor can identify the image to put the item's sprite into.
                    /// </summary>
					[RequireComponent(typeof(Image))]
					public class BasicStandardInventoryViewItemIcon : MonoBehaviour {
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
