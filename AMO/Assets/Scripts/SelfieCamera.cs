using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfieCamera : MonoBehaviour
{
    public GameObject container;
    public RawImage rawImage;

    private void Start()
    {
        OpenCamera();
    }

    public void OpenCamera()
    {
        container.SetActive(true);
        if (NativeCamera.DeviceHasCamera())
        {
           
            if (NativeCamera.IsCameraBusy())
            {
                return;
            }
            TakePicture(512);
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
            Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
            if (texture == null)
            {
                return;
            }

            rawImage.texture = texture;

        }, maxSize, true, NativeCamera.PreferredCamera.Front);
    }
}
