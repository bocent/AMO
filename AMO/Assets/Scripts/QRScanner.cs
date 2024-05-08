using JsonFx.Json;
using maxstAR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QRScanner : ARBehaviour
{
    public Button backButton;

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
        if (!codeScanResult.Equals("") && codeScanResult.Length > 0)
        {
            Dictionary<string, string> resultAsDicionary =
                new JsonReader(codeScanResult).Deserialize<Dictionary<string, string>>();

            //codeFormatText.text = "Format : " + resultAsDicionary["Format"];
            //codeValueText.text = "Value : " + resultAsDicionary["Value"];
            if (resultAsDicionary != null)
            {
                ScanSuccess();
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

    private void ScanSuccess()
    {
        CustomSceneManager.Instance.LoadScene("Home", () => Main.Instance.UnlockCharacter("Gilmo_1"));
    }

    private void BackToHome()
    {
        CustomSceneManager.Instance.LoadScene("Home", null);
    }
}
