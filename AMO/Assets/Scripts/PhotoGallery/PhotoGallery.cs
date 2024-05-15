using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PhotoGallery : MonoBehaviour
{
    public GameObject container;
    public Transform photoParent;
    public GameObject photoPrefab;
    public Button backButton;

    private List<GameObject> photoList = new List<GameObject>();

    private void Start()
    {
        backButton.onClick.AddListener(Hide);
    }

    public void Show(bool value)
    {
        if (value) 
            Show();
        else
            Hide();
    }

    public void Show()
    {
        container.SetActive(true);
        CheckStoragePermission();
    }

    public void Hide()
    {
        container.SetActive(false);
        ClearPhotoList();
    }

    private void ClearPhotoList()
    {
        foreach (GameObject item in photoList)
        {
            item.SetActive(false);
        }
    }

    private GameObject GetItem()
    {
        for (int i = 0; i < photoList.Count; i++)
        {
            if (!photoList[i].activeSelf)
            {
                photoList[i].SetActive(true);
                return photoList[i];
            }
        }
        return AddItem();
    }

    private GameObject AddItem()
    {
        GameObject go = Instantiate(photoPrefab, photoParent, false);
        photoList.Add(go);
        return go;
    }

    private void CheckStoragePermission()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            StartCoroutine(LoadGallery());
        }
        else
        {
            PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
            permissionCallbacks.PermissionGranted += (permission) => StartCoroutine(LoadGallery());
            permissionCallbacks.PermissionDenied += ErrorMessage;
            permissionCallbacks.PermissionDeniedAndDontAskAgain += ErrorMessage;
            Permission.RequestUserPermission(Permission.ExternalStorageWrite,  permissionCallbacks);
        }
    }

    private void ErrorMessage(string message)
    {
        Debug.LogError(message);
    }

    private IEnumerator LoadGallery()
    {
        string path = Consts.GALLERY_PATH;
        Debug.LogWarning("path : " + path);
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                Debug.LogWarning("file : " + file);
                byte[] bytes = File.ReadAllBytes(file);
                Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                if (texture.LoadImage(bytes))
                {
                    GameObject item = GetItem();
                    item.GetComponent<PhotoItem>()?.Init(texture);
                    yield return null;
                }
                else
                {
                    Debug.LogError("failed");
                }
                //using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(file))
                //{
                //    yield return uwr.SendWebRequest();
                //    if (uwr.result == UnityWebRequest.Result.Success)
                //    {
                //        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                //        Debug.LogWarning("texture : " + texture);
                //        GameObject item = GetItem();
                //        item.GetComponent<PhotoItem>()?.Init(texture);
                //    }
                //    else
                //    {
                //        Debug.LogError("err : " + uwr.error);
                //    }
                //}
            }
        }
    }
}
