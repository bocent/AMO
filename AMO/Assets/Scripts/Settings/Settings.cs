using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject container;
    //public Toggle sfxOn;
    //public Toggle bgmOn;
    public Slider sfxSlider;
    public Slider bgmSlider;
    public Slider voiceSlider;

    public Button closeButton;
    public Button changeNameButton;
    public Button saveNameButton;
    public Button bindAccountButton;
    public Button logoutButton;
    public TMP_InputField nameInputField;
    public Login login;

    public GameObject changeNameContainer;
    public TMP_Text nameText;

    private void Start()
    {
        //sfxOn.onValueChanged.AddListener(ChangeSfxToggle);
        //bgmOn.onValueChanged.AddListener(ChangeBgmToggle);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
        bgmSlider.onValueChanged.AddListener(ChangeBgmVolume);
        voiceSlider.onValueChanged.AddListener(ChangeVoiceVolume);

        closeButton.onClick.AddListener(Close);
        changeNameButton.onClick.AddListener(ChangeName);
        saveNameButton.onClick.AddListener(SaveName);
        if (!PlayerPrefs.HasKey("email"))
        {
            bindAccountButton.onClick.AddListener(BindAccount);
        }
        else
        {
            bindAccountButton.gameObject.SetActive(false);
        }
        logoutButton.onClick.AddListener(Logout);
        //nameInputField.onSubmit.AddListener(SubmitName);

        //sfxOn.isOn = SoundManager.instance.GetSFXOn();
        //bgmOn.isOn = SoundManager.instance.GetBGMOn();

        sfxSlider.value = SoundManager.instance.GetSfxVolume();
        bgmSlider.value = SoundManager.instance.GetBgmVolume();
        voiceSlider.value = SoundManager.instance.GetVoiceVolume();

        GetComponent<Button>().onClick.AddListener(Show);
    }

    public void Show()
    {
        Debug.LogWarning("Show setting");
        container.SetActive(true);
        nameText.text = string.IsNullOrEmpty(UserData.username) ? "Guest" : UserData.username;
    }

    private void ChangeSfxToggle(bool isOn)
    {
        SoundManager.instance.SFXOn(isOn);
    }

    private void ChangeBgmToggle(bool isOn)
    {
        SoundManager.instance.BGMOn(isOn);
    }

    private void ChangeSfxVolume(float value)
    {
        SoundManager.instance.SetSfxVolume(value);

    }

    private void ChangeBgmVolume(float value)
    {
        SoundManager.instance.SetBgmVolume(value);
    }

    private void ChangeVoiceVolume(float value)
    {
        SoundManager.instance.SetVoiceVolume(value);
    }

    public void Close()
    {
        container.SetActive(false);
    }

    public void ChangeName()
    {
        changeNameContainer.SetActive(true);
    }

    public void SaveName()
    {
        changeNameContainer.SetActive(false);
        SubmitName(nameInputField.text);
    }
    public void BindAccount()
    {
        login.ShowRegistrationPage();
    }

    public void Logout()
    {
        if (PlayerPrefs.HasKey("guest_email"))
        {
            PopupManager.Instance.ShowPopupMessage("remind", "Peringatan", "Akun guestmu akan hilang jika kamu logout", new ButtonInfo
            {
                content = "Logout",
                onButtonClicked = () =>
                {
                    PlayerPrefs.DeleteKey("email");
                    PlayerPrefs.DeleteKey("password");
                    PlayerPrefs.DeleteKey("guest_email");
                    PlayerPrefs.DeleteKey("guest_password");
                    CustomSceneManager.Instance.LoadScene(Consts.LOGIN_SCENE, null);
                }
            }, new ButtonInfo { content = "Batal" });
        }
        else
        {
            PlayerPrefs.DeleteKey("email");
            PlayerPrefs.DeleteKey("password");
            CustomSceneManager.Instance.LoadScene(Consts.LOGIN_SCENE, null);
        }
    }

    public void SubmitName(string text)
    {
        StartCoroutine(RequestSubmitName(text));
    }


    private IEnumerator RequestSubmitName(string text)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", "{\"name\" : \"" + text + "\"}");

        using (UnityWebRequest uwr = UnityWebRequest.Post(Consts.BASE_URL + "update_profile", form))
        {
            uwr.SetRequestHeader("Authorization", "Bearer " + UserData.token);
            yield return uwr.SendWebRequest();
            try
            {
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Response response = JsonUtility.FromJson<Response>(uwr.downloadHandler.text);
                    if (response.status.ToLower() != "ok")
                    {
                        throw new Exception(response.msg);
                    }
                    else
                    {
                        nameText.text = UserData.username = text;
                    }
                }
                else
                {
                    throw new Exception("Gagal mengubah nama");
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
        }
    }
}
