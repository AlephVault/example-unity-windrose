using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        namespace Sessions
        {
            /// <summary>
            ///   A global canvas to display the messages.
            /// </summary>
            [RequireComponent(typeof(Canvas))]
            public class SessionNotificationRenderer : MonoBehaviour
            {
                private float remainingTime = 0;
                private Image messageHolder;
                private TextMeshProUGUI text;
                private SampleMessageForwarder forwarder;

                void Awake()
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, -10);
                    messageHolder = transform.GetChild(0).GetComponent<Image>();
                    text = messageHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    forwarder = NetworkManager.singleton.GetComponent<SampleMessageForwarder>();

                    GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        remainingTime = 0;
                    });

                    forwarder.onMessage.AddListener(OnMessage);

                    DontDestroyOnLoad(gameObject);
                }

                void OnDestroy()
                {
                    forwarder.onMessage.RemoveListener(OnMessage);
                }

                void OnMessage(string message)
                {
                    text.text = message;
                    remainingTime = 3.0f;
                }

                void Update()
                {
                    if (remainingTime > 0)
                    {
                        messageHolder.gameObject.SetActive(true);
                        remainingTime -= Time.deltaTime;
                    }
                    if (remainingTime <= 0)
                    {
                        remainingTime = 0;
                        messageHolder.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
