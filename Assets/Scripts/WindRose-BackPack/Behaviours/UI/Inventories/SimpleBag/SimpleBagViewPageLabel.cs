﻿using System.Collections;
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
					public class SimpleBagViewPageLabel : MonoBehaviour {
						/**
					     * Updates the content of the paging into its text as "page / total-pages"
					     */

						private Text textComponent;

						void Awake() {
							textComponent = GetComponent<Text>();
						}

						public void SetPaginationLabel(uint page, uint maxPage)
						{
							textComponent.text = string.Format("{0} / {1}", page + 1, maxPage + 1);
						}
					}
				}
			}
		}
	}
}
