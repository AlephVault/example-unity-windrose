using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SimpleBagViewItemIcon : MonoBehaviour {
    /**
     * This class is the icon of a SampleSimpleBagViewItemIton.
     */

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetIcon(Sprite icon)
    {
        image.sprite = icon;
        image.enabled = icon != null;
    }
}
