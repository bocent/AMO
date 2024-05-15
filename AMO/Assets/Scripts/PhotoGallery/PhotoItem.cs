using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoItem : MonoBehaviour
{
    public RawImage rawImage;

    public void Init(Texture2D texture)
    {
        rawImage.texture = texture;
    }
}
