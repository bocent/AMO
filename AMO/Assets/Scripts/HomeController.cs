using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class HomeController : MonoBehaviour
{
    public Avatar avatar;
    public CharacterSelection characterSelection;
    public MoodController moodController;
    public EnergyController energyController;
    public ItemLibrary itemLibrary;
    public AlarmController alarmController;
    public ToDoController toDoController;
    public NotificationController notificationController;
    public InboxController inboxController;
    public SelfieCamera selfieCamera;
    public Level level;
    public Coins coins;
    public Character character;
    public AskMe askMe;

    [HideInInspector] public SelectedCharacter selectedCharacter;
    public PlaygroundActivity playgroundActivity;
    public PhotoGallery photoGallery;
    public Inventory inventory;
    public IAP iapPage;

    public GameObject homeHUD;
    public GameObject homeRoom;
    public GameObject fittingRoom;
    public GameObject characterSelectionRoom;
    public GameObject chargeRoom;

    public GameObject chargingStation;

    public GameObject glitchParticle;
    public ParticleSystem eatBatteryEffect;
    public GameObject eatBatteryBlitzObj;

    private float elapsedTime = 0;
    private DateTime dateTimeStart;
    public int startTimeInSecond;
    public int elapsedTimeInSecond;
    public float energyToSecond = 60f;
    public float inGameEnergyConsumed;
    
    [HideInInspector]
    public Limitation currentLimitation;

    private bool isCharging = false;


    public static HomeController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Debug.LogWarning("result : " + Utils.EncryptXOR("hello world", "1234567890"));
    }

    private void Start()
    {
        UserData.Init();
        SoundManager.instance.PlayBGM("home", true);
        UserData.SetEnergy(100);
        DateTime dateTime = DateTime.Now;
        startTimeInSecond = dateTime.Second + dateTime.Minute * 60 + dateTime.Hour * 3600 + dateTime.DayOfYear * 86400;
        characterSelection.Init();
        coins.SetCoin(UserData.Coins.ToString());
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        elapsedTimeInSecond = Mathf.FloorToInt(elapsedTime);

        inGameEnergyConsumed = energyToSecond * Time.deltaTime;

        AvatarInfo info = character.GetCurrentAvatarInfo();
        UserData.UseEnergy(inGameEnergyConsumed);
        currentLimitation = Main.Instance.GetFeatureLimitation(Mathf.RoundToInt(UserData.Energy));
        if ((int)UserData.Mood != currentLimitation.mood)
        {
            UserData.SetMood((Main.MoodStage)currentLimitation.mood);
            UserData.SetRequirementList(currentLimitation.requirementList);
            StartCoroutine(NeedsController.Instance.Init());
            glitchParticle.SetActive(UserData.Mood == Main.MoodStage.BROKEN);
            if (UserData.Mood == Main.MoodStage.BROKEN)
            {
                SoundManager.instance.PlayBGM("glitch", true);
            }
            else
            {
                SoundManager.instance.PlayBGM("home", true);
            }
        }

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    selectedCharacter.Evolution();
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    LoadScanScene();
        //}

        if (Input.GetKeyDown(KeyCode.U))
        {
            int addExp = Random.Range(400, 10000);
            int[] levels = UserData.GetLevel(1, info.exp);
            Debug.LogWarning("level start : " + levels[0]);
            //level.SetLevel(levels[0], levels[1], levels[2]);
            level.UpdateLevel(levels[0], levels[1], levels[2], addExp);
            selectedCharacter.AddExp(addExp);
            //RefreshLevel(selectedCharacter.Info);
        }

        if (SystemInfo.batteryStatus == BatteryStatus.Charging)
        {
            if (!isCharging)
            {
                Charging();
            }
        }
        else if (SystemInfo.batteryStatus == BatteryStatus.Discharging || SystemInfo.batteryStatus == BatteryStatus.NotCharging)
        {
            if (isCharging)
            {
                Discharging();
            }
        }
    }

    private void Charging()
    {
        isCharging = true;
        chargingStation.SetActive(true);
    }

    private void Discharging()
    {
        isCharging = false;
        chargingStation.GetComponent<ChargingStation>().Hide();
    }

    public void SetEnergy(int value)
    {
        energyController.SetEnergy(value);
    }

    public void ShowCharacterSelection(bool value)
    {
        characterSelection.Show(value);
    }

    public void ShowInventory(bool value)
    {
        inventory.Show(value);
    }

    public void SelectCharacter(AvatarInfo info)
    {
        selectedCharacter = character.SwitchCharacter(info.avatarId);
        selectedCharacter.Init(info);
        selectedCharacter.PlayChoosenAnimation();

        RefreshLevel(info);

        avatar.SetAvatar(info);
    }

    public void RefreshCoins()
    {
        coins.SetCoin(UserData.Coins.ToString());
    }

    public void RefreshLevel(AvatarInfo info)
    {
        int[] levels = UserData.GetLevel(1, info.exp);
        level.SetLevel(levels[0], levels[1], levels[2]);
    }

    public void ShowHUD(bool value)
    {
        homeHUD.SetActive(value);
    }

    public void ShowHome(bool value)
    {
        homeRoom.SetActive(value);
    }

    public void ShowFittingRoom(bool value)
    {
        fittingRoom.SetActive(value);
    }

    public void ShowCharacterSelectionRoom(bool value)
    {
        characterSelectionRoom.SetActive(value);
    }

    public void ShowChargingRoom(bool value)
    {
        chargeRoom.SetActive(value);
    }

    public void ShowPlaygroundList(bool value)
    {
        playgroundActivity.Show(value);
    }

    public void ShowPhotoGallery(bool value)
    {
        photoGallery.Show(value);
    }

    public void ShowIAP(bool value)
    {
        iapPage.Show(value);
    }

    public void OpenCamera()
    {
        selfieCamera.OpenCamera();
    }

    public void LoadScanScene()
    {
        StartCoroutine(WaitForLoadScene());
        //SceneManager.LoadSceneAsync("CodeReader", LoadSceneMode.Single);
    }

    private IEnumerator WaitForLoadScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneStackManager.Instance.LoadScene("Home", "CodeReader");
    }

    public void ShowEatBatteryEffect()
    {
        eatBatteryEffect.gameObject.SetActive(true);
        eatBatteryEffect.Play();
        eatBatteryBlitzObj.SetActive(true);
    }
}
