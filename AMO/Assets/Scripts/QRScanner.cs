using JsonFx.Json;
using maxstAR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class QRScanner : ARBehaviour
{
    public Button backButton;

    private bool isRequesting = false;

    private CameraBackgroundBehaviour cameraBackgroundBehaviour = null;
    void Awake()
    {
        backButton.onClick.AddListener(BackToHome);
        Init();
        cameraBackgroundBehaviour = FindObjectOfType<CameraBackgroundBehaviour>();
        if (cameraBackgroundBehaviour == null)
        {
            Debug.LogError("Can't find CameraBackgroundBehaviour.");
            return;
        }
    }

    void Start()
    {
        isRequesting = false;
        StartCodeScan();
        StartCameraInternal();
    }

    void Update()
    {
        TrackingState state = TrackerManager.GetInstance().UpdateTrackingState();

        if (state == null)
        {
            return;
        }

        cameraBackgroundBehaviour.UpdateCameraBackgroundImage(state);

        string codeScanResult = state.GetCodeScanResult();
        Debug.LogWarning("code scan result : " + codeScanResult);
        if (!codeScanResult.Equals("") && codeScanResult.Length > 0)
        {
            Dictionary<string, string> resultAsDicionary =
                new JsonReader(codeScanResult).Deserialize<Dictionary<string, string>>();

            //codeFormatText.text = "Format : " + resultAsDicionary["Format"];
            //codeValueText.text = "Value : " + resultAsDicionary["Value"];
            if (resultAsDicionary != null)
            {
                Debug.LogWarning("result scan : " + resultAsDicionary["Value"]);
                if (!isRequesting)
                {
                    StartCoroutine(RequestGetItem(resultAsDicionary["Value"], ScanSuccess, null));
                }
            }
        }
    }

    private IEnumerator RequestGetItem(string id, Action<string> onComplete, Action<string> onFailed)
    {
        isRequesting = true;
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"qrcode\" : \"" + id + "\" }");
        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "scan_card", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    ScanCardResponse response = JsonUtility.FromJson<ScanCardResponse>(uwr.downloadHandler.text);
                    if (response.status.ToLower() == "ok")
                    {
                        string content = "";
                        for (int i = 0; i < response.user_gets.Length; i++)
                        {
                            content += "• " + response.user_gets[i];
                            if (i < response.user_gets.Length - 1)
                            {
                                content += "\n";
                            }
                        }
                        onComplete?.Invoke(content);
                        isRequesting = false;
                    }
                    else
                    {
                        throw new Exception(response.msg);
                    }
                }
                else
                {
                    throw new Exception(uwr.error);
                }
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
                Debug.LogError("err : " + e.Message);
                PopupManager.Instance.ShowPopupMessage("err", "Gagal Mendapatkan Data", e.Message,
                    new ButtonInfo
                    {
                        content = "Ulangi",
                        onButtonClicked = () => StartCoroutine(RequestGetItem(id, onComplete, onFailed))
                    },
                    new ButtonInfo
                    {
                        content = "Batal"
                    });

                isRequesting = false;
            }
        }
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            TrackerManager.GetInstance().StopTracker();
            StopCameraInternal();
        }
        else
        {
            StartCodeScan();
            StartCameraInternal();
        }
    }

    void OnDestroy()
    {
        TrackerManager.GetInstance().StopTracker();
        TrackerManager.GetInstance().DestroyTracker();
        StopCameraInternal();
    }

    public void StartCodeScan()
    {
        //codeFormatText.text = "";
        //codeValueText.text = "";
        TrackerManager.GetInstance().StartTracker(TrackerManager.TRACKER_TYPE_CODE_SCANNER);
    }

    private void StartCameraInternal()
    {
        StartCamera();
        StartCoroutine(AutoFocusCoroutine());
    }

    private void StopCameraInternal()
    {
        StopCamera();
        StopCoroutine(AutoFocusCoroutine());
    }

    IEnumerator AutoFocusCoroutine()
    {
        while (true)
        {
            CameraDevice.GetInstance().SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_AUTO);
            yield return new WaitForSeconds(3.0f);
        }
    }

    private void ScanSuccess(string content)
    {
        CustomSceneManager.Instance.LoadScene(Consts.HOME_SCENE, () => Main.Instance.UnlockItemByScan(content));
    }

    private void BackToHome()
    {
        CustomSceneManager.Instance.LoadScene(Consts.HOME_SCENE, null);
    }
}
