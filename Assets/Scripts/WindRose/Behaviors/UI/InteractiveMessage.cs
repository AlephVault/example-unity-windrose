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
                public bool QuickTextMovement
                {
                    get { return messageContent.QuickTextMovement; }
                    set { messageContent.QuickTextMovement = value; }
                }

                protected override void Start()
                {
                    base.Start();
                    mask = GetComponent<UnityEngine.UI.Mask>();
                    messageContent = Utils.Layout.RequireComponentInChildren<InteractiveMessageContent>(this.gameObject);
                    RectTransform me = GetComponent<RectTransform>();
                    content = messageContent.GetComponent<RectTransform>();
                    float myWidth = me.sizeDelta.x;
                    float itsWidth = content.sizeDelta.x;
                    content.localPosition = new Vector2((myWidth - itsWidth) / 2, 0);
                    content.sizeDelta = new Vector2(itsWidth, content.sizeDelta.y);
                }

                public Coroutine StartTextMessage(string text)
                {
                    return messageContent.StartTextMessage(text);
                }

                // Update is called once per frame
                void Update()
                {
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