using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class SelfieCamera : MonoBehaviour
{
    public GameObject container;
    public RawImage rawImage;
    public Button backButton;
    public Button saveButton;
    public Button retakeButton;
    public GameObject footer;

    private Texture2D cachedPhoto;

    private void Start()
    {
        backButton.onClick.AddListener(Back);
        saveButton.onClick.AddListener(SaveToGallery);
        retakeButton.onClick.AddListener(OpenCamera);
    }

    public void OpenCamera()
    {
        footer.SetActive(false);
        container.SetActive(true);
        rawImage.color = Color.black;
        if (NativeCamera.DeviceHasCamera())
        {
            if (NativeCamera.IsCameraBusy())
            {
                return;
            }
            TakePicture(1024);
        }
    }

    public void HideCamera()
    {
        container.SetActive(false);
    }

    private void TakePicture(int maxSize)
    {
        NativeCamera.Permission permission = NativeCamera.TakePicture((path) => 
        {
            cachedPhoto = NativeCamera.LoadImageAtPath(path, maxSize, false);
            if (cachedPhoto == null)
            {
                return;
            }
            Debug.LogWarning("take picture : " + path);
            footer.SetActive(true);
            rawImage.color = Color.white;
            rawImage.texture = cachedPhoto;

        }, maxSize, true, NativeCamera.PreferredCamera.Front);
    }



    private void SaveToGallery()
    {
        string fileName = DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".png";
        byte[] bytes = cachedPhoto.EncodeToPNG();
        string path = Consts.GALLERY_PATH + fileName;
        if (!Directory.Exists(Consts.GALLERY_PATH))
        {
            Directory.CreateDirectory(Consts.GALLERY_PATH);
        }
        File.WriteAllBytes(path, bytes);
        Back();
        Debug.Log("photo saved");
    }


    private void Back()
    {
        rawImage.texture = cachedPhoto = null;
        container.SetActive(false);
    }
}
