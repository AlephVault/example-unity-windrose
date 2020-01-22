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
					public class SimpleBagViewSelectedItemLabel : MonoBehaviour {
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
