using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundActivity : MonoBehaviour
{
    public GameObject container;
    public Button backButton;
    public Button photoButton;
    public Button galleryButton;

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
        photoButton.onClick.AddListener(OpenCamera);
        galleryButton.onClick.AddListener(ShowGallery);
    }

    public void Show(bool value)
    {
        container.SetActive(value);
    }

    public void Show()
    {
        container.SetActive(true);
    }

    public void Hide()
    {
        container?.SetActive(false);
    }

    private void OpenCamera()
    {
        HomeController.Instance.OpenCamera();
    }

    private void ShowGallery()
    {
        HomeController.Instance.ShowPhotoGallery(true);
    }
}
