using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvents : MonoBehaviour
{
    public TMP_Text buttonText;
    public Sprite selectedImage, unselectedImage;
    private Button button;

    public void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnHoverEnter()
    {
        buttonText.color =Color.yellow;
    }

    public void OnHoverExit()
    {
        buttonText.color = Color.white;
    }
    public void OnHoverEnterImage()
    {
        button.image.sprite = selectedImage;
    }

    public void OnHoverExitImage()
    {
        button.image.sprite = unselectedImage;
    }
}
