using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            [RequireComponent(typeof(UnityEngine.UI.Mask))]
            [RequireComponent(typeof(UnityEngine.UI.Image))]
            public class InteractiveMessage : UnityEngine.UI.ScrollRect
            {
                private UnityEngine.UI.Mask mask;
                private InteractiveMessageContent messageContent;

                void Start()
                {
                    mask = GetComponent<UnityEngine.UI.Mask>();
                    messageContent = Utils.Layout.RequireComponentInChildren<InteractiveMessageContent>(this.gameObject);
                    RectTransform me = GetComponent<RectTransform>();
                    content = messageContent.GetComponent<RectTransform>();
                    float myWidth = me.sizeDelta.x;
                    content.localPosition = Vector3.zero;
                    content.sizeDelta = new Vector2(myWidth, content.sizeDelta.y);
                }

                // Update is called once per frame
                void Update()
                {
                    mask.showMaskGraphic = false;
                    horizontal = false;
                    vertical = true;
                    if (content)
                    {
                        verticalNormalizedPosition = 0;
                    }
                }
            }
        }
    }
}