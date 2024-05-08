using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    public static CustomSceneManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void LoadScene(string sceneName, Action onLoadCompleted)
    {
        StartCoroutine(LoadSceneIEnumerator(sceneName, onLoadCompleted, 0.1f));
    }

    public void LoadSceneAsync(string sceneName, Action onLoadCompleted)
    {
        StartCoroutine(LoadSceneIEnumerator(sceneName, onLoadCompleted));
    }

    private IEnumerator LoadSceneIEnumerator(string sceneName, Action onLoadCompleted)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        yield return ao;
        onLoadCompleted?.Invoke();
    }

    private IEnumerator LoadSceneIEnumerator(string sceneName, Action onLoadCompleted, float delay)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return new WaitForSeconds(delay);
        onLoadCompleted?.Invoke();
    }
}
