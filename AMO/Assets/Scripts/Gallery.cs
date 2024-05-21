using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenGallery);
    }

    private void OpenGallery()
    {
        HomeController.Instance.ShowPhotoGallery(true);
    }
}
