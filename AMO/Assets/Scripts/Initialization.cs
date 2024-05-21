using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialization : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return null;
        CustomSceneManager.Instance.LoadSceneAsync("Home", null);
    }

}
