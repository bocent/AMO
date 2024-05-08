using OpenAI;
using Samples.Whisper;
using UnityEngine;
using UnityEngine.UI;

public class Amoverse : MonoBehaviour
{
    [SerializeField] private Button recordButton;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text message;

    private readonly string fileName = "output.wav";
    private readonly int duration = 5;

    private AudioClip clip;
    private bool isRecording;
    private float time;
    private OpenAIApi openai = new OpenAIApi();

    private void Start()
    {
        recordButton.onClick.AddListener(StartRecording);

        var index = PlayerPrefs.HasKey("user-mic-device-index") ? PlayerPrefs.GetInt("user-mic-device-index") : 0;
        Debug.LogWarning("index : " + index);
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording()
    {
        isRecording = true;
        recordButton.enabled = false;

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        Debug.LogWarning("index : " + index);
#if !UNITY_WEBGL
        clip = Microphone.Start(Microphone.devices[index].ToString(), false, duration, 44100);
#endif
    }

    private async void EndRecording()
    {
        message.text = "Transcripting...";

#if !UNITY_WEBGL
        Microphone.End(null);
#endif

        byte[] data = SaveWav.Save(fileName, clip);

        var req = new CreateAudioTranscriptionsRequest
        {
            FileData = new FileData() { Data = data, Name = "audio.wav" },
            // File = Application.persistentDataPath + "/" + fileName,
            Model = "whisper-1",
            Language = "id"
        };
        var res = await openai.CreateAudioTranscription(req);

        progressBar.fillAmount = 0;
        message.text = res.Text;
        recordButton.enabled = true;
    }

    private void Update()
    {
        if (isRecording)
        {
            time += Time.deltaTime;
            progressBar.fillAmount = 1 - time / duration;

            if (time >= duration)
            {
                time = 0;
                isRecording = false;
                EndRecording();
            }
        }
    }
}
