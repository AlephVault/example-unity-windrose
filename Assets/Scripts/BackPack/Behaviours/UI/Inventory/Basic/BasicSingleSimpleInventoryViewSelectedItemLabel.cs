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
					public class BasicSingleSimpleInventoryViewSelectedItemLabel : MonoBehaviour {
						/**
					     * Updates the content of the item into its text.
					     */

						private Text textComponent;

						void Awake()
						{
							textComponent = GetComponent<Text>();
						}

						public void SetCaption(string caption)
						{
							textComponent.text = caption;
						}
					}
				}
			}
		}
	}
}
