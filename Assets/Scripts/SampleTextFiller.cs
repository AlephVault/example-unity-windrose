﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(WindRose.Behaviors.UI.InteractiveMessageContent))]
public class SampleTextFiller : MonoBehaviour
{
    const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nec mattis tellus. Nulla pellentesque rutrum est eu porttitor. Phasellus dapibus blandit mauris at iaculis.";
    const string IPSUM = "Aliquam consequat, tellus non consequat sollicitudin, ligula dui pellentesque ipsum, eget lobortis urna turpis ac augue. Nullam sed faucibus eros. Fusce vitae ex sapien. Sed in massa eget tellus ultricies aliquam. Maecenas a euismod ante, vitae molestie arcu.";

    private WindRose.Behaviors.UI.InteractiveMessageContent content;

    // Use this for initialization
    void Start ()
    {
        content = GetComponent<WindRose.Behaviors.UI.InteractiveMessageContent>();
        StartCoroutine(StartMessageTwice());
	}

    IEnumerator StartMessageTwice() {
        yield return new WaitForSeconds(1);
        yield return content.StartTextMessage(LOREM);
        yield return content.StartTextMessage(IPSUM);
    }
}
