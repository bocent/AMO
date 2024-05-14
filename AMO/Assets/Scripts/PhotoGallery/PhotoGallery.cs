using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoGallery : MonoBehaviour
{
    public GameObject container;

    private void Start()
    {
        
    }

    public void Show()
    {
        container.SetActive(true);
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    private void LoadGallery()
    {
        
    }
}
