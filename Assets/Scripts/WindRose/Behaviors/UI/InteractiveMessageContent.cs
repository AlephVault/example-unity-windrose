using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            [RequireComponent(typeof(UnityEngine.UI.Text))]
            [RequireComponent(typeof(UnityEngine.UI.ContentSizeFitter))]
            public class InteractiveMessageContent : MonoBehaviour
            {
                [SerializeField]
                private float slowTimeBetweenLetters = 0.05f;

                [SerializeField]
                private float quickTimeBetweenLetters = 0.005f;

                private UnityEngine.UI.Text textComponent;
                private Coroutine currentTextMessageCoroutine;

                public bool quickTextMovement = false;

                void Start()
                {
                    textComponent = GetComponent<UnityEngine.UI.Text>();
                }

                public Coroutine StartTextMessage(string text)
                {
                    if (currentTextMessageCoroutine == null)
                    {
                        currentTextMessageCoroutine = StartCoroutine(TextMessageCoroutine(text));
                    }
                    return currentTextMessageCoroutine;
                }

                private IEnumerator TextMessageCoroutine(string text)
                {
                    text = text ?? "";
                    float slowInterval = Utils.Values.Max(0.0001f, slowTimeBetweenLetters);
                    float quickInterval = Utils.Values.Max(0.00001f, quickTimeBetweenLetters);
                    StringBuilder builder = new StringBuilder("");

                    foreach (char current in text)
                    {
                        builder.Append(current);
                        textComponent.text = builder.ToString();
                        yield return new WaitForSeconds(quickTextMovement ? quickInterval : slowInterval);
                    }
                    currentTextMessageCoroutine = null;
                }
            }
        }
    }
}