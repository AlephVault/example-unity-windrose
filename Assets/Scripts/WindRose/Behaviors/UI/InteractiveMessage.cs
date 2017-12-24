using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            /**
             * This component handles the message being displayed to the user. It requires two components
             *   in the same object, and one component in a child. The components go like this:
             *   1. Image (in object): Is the background of the message.
             *      Recommended settings:
             *        > Image Type: Slice
             *          > Fill Center: True
             *   2. Mask (in object): Is used to clip the content of the message, which will grow very long.
             *      Recommended settings:
             *        > Show Mask Graphic: True (otherwise the background image in the Image component
             *          will not be seen)
             *   3. This component has also recommended settings since it inherits from ScrollRect:
             *      > Viewport: None
             *      > Horizontal Scrollbar: None
             *      > Vertical Scrollbar: None
             *   4. InteractiveMessageContent (in child). It is already documented.
             */
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