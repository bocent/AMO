using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Utils
{
    public static IEnumerator GetSprite(string url, string path, string name, Action<Sprite> onComplete)
    {
        LoadPicture(path, name, out Sprite resultSprite);
        if (resultSprite)
        {
            onComplete?.Invoke(resultSprite);
        }
        else
        {
            Debug.Log("load from server");
            yield return GetSpriteFromServer(url, Consts.IMAGE_NAME, name, onComplete);
        }
    }

    public static IEnumerator GetTextureFromServer(string url, Action<Texture> onTextureDownloaded)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(uwr.error);
        }
        else
        {
            Debug.LogWarning("result : " + ((DownloadHandlerTexture)uwr.downloadHandler).data.Length);
            onTextureDownloaded?.Invoke(((DownloadHandlerTexture)uwr.downloadHandler).texture);
        }
    }

    public static IEnumerator GetSpriteFromServer(string url, string path, string fileName, Action<Sprite> onTextureDownloaded)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(uwr.error + " url : " + url);
        }
        else
        {
            Texture texture = ((DownloadHandlerTexture)uwr.downloadHandler).texture;
            Sprite sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            SavePicture(Consts.PICTURES_FOLDER_NAME + path, fileName, (Texture2D)texture);
            onTextureDownloaded?.Invoke(sprite);
        }
    }

    public static void SavePicture(string path, string fileName, Texture2D texture)
    {
        if (!Directory.Exists(Application.persistentDataPath + path))
        {
            Directory.CreateDirectory(Application.persistentDataPath + path);
        }
        byte[] bytes = texture.EncodeToPNG();
        string fullPath = Application.persistentDataPath + path + fileName;
        File.WriteAllBytes(fullPath, bytes);
    }

    public static void LoadPicture(string path, string fileName, out Sprite resultSprite)
    {
        Debug.Log("load : " + Application.persistentDataPath + path + fileName);
        Debug.Log("isFile " + fileName + " exists : " + File.Exists(Application.persistentDataPath + path + fileName));
        if (File.Exists(Application.persistentDataPath + path + fileName))
        {
            string fullPath = Application.persistentDataPath + path + fileName;
            byte[] bytes = File.ReadAllBytes(fullPath);
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.LoadImage(bytes);
            resultSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
        }
        else
        {
            resultSprite = null;
        }
    }

    public static string GetSizeWithSuffix(decimal value)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        while (value >= 1024 && order < sizes.Length - 1)
        {
            order++;
            value /= 1024;
        }

        // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
        // show a single decimal place, and no space.
        return string.Format("{0:0.##} {1}", value, sizes[order]);
    }
}
