using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GabTab.Behaviours;
using TMPro;

[RequireComponent(typeof(Hideable))]
public class MessagePopup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Hideable hideable = GetComponent<Hideable>();
        Button button = GetComponent<Button>();
        TMP_Text text = GetComponent<TMP_Text>();
        button.onClick.AddListener(delegate ()
        {
            hideable.Hidden = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
