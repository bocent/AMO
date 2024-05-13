using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static Main Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    public void UnlockCharacter(string avatarId)
    {
        StartCoroutine(Character.Instance.UnlockCharacter(avatarId));
    }
}
