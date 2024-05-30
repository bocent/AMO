using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DecryptByFernet : MonoBehaviour
{
    // URL to send the POST request to
    private string url = "https://dev.amoevo.my.id/dec";

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to post data
        StartCoroutine(PostDataCoroutine("gAAAAABmV9XICsj7Pxpnim48fKCNfyoL_T05ffTwLGhtYydZwmMUwveTZCB1-O7veGoU_mmm0CGAw-GluAQ1uApxcGytKA8LG7fqSCEcQ19xV-EH0Q-y8WY="));
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
