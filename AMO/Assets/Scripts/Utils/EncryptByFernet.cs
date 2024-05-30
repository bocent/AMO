using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EncryptByFernet : MonoBehaviour
{
    // URL to send the POST request to
    private string url = "https://dev.amoevo.my.id/enc";

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to post data
        StartCoroutine(PostDataCoroutine("Halo Nama Saya Dicky!"));
    }

    // Coroutine to post data and get the response
    IEnumerator PostDataCoroutine(string data)
    {
        // Create a form and add the data to it
        WWWForm form = new WWWForm();
        form.AddField("data", data);

        // Create a UnityWebRequest with the form data
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for network errors
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // Get the response text
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("Response: " + responseText);
            }
        }
    }
}
