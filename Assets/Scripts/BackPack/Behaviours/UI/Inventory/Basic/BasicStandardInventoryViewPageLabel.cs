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
                    ///   This is a marker behaviour so the <see cref="BasicStandardInventoryView" />
                    ///     ancestor can identify the label to put the current page.
                    /// </summary>
					[RequireComponent(typeof(Text))]
					public class BasicStandardInventoryViewPageLabel : MonoBehaviour {
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
