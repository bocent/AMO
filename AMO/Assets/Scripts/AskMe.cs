using OpenAI;
using Samples.Whisper;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class ChatRequest
{
    public string message;
}

[Serializable]
public class ChatResponse
{
    public string response;   
}

#region OPEN_AI_MODEL
[Serializable]
public class DialogPromt
{
    public string model;
    public OpenAIMessage[] messages;
}

[Serializable]
public class OpenAIMessage
{
    public string role;
    public string content;
}

[Serializable]
public class ResultMessage
{
    public string id;
    public int created;
    public string model;
    public AnswerChoice[] choices;
    public Usage usage;
    public string system_fingerprint;
}

[Serializable]
public class AnswerChoice
{
    public int index;
    public OpenAIMessage message;
    public string longprobs;
    public string finish_reason;
}

[Serializable]
public class Usage
{
    public int promt_tokens;
    public int completion_tokens;
    public int total_tokens;
}

#endregion

#region ELEVEN_LABS_MODEL
[Serializable]
public class TextToSpeechData
{
    public string text;
    public string model_id;
    //public VoiceSettings voice_settings;
    //public PronunciationDictionaryLocators[] pronunciation_dictionary_locators;
    //public int seed;
    //public string previous_text;
    //public string next_text;
    //public string[] previous_request_ids;
    //public string[] next_request_ids;
}

[Serializable]
public class VoiceSettings
{
    public int stability;
    public int similarity_boost;
    public int style;
    public bool use_speaker_boost;
}

[Serializable]
public class PronunciationDictionaryLocators
{
    public string pronunciation_dictionary_id;
    public string version_id;
}
#endregion

[Serializable]
public class OpenAISecretKey
{
    public string apiKey;
    public string organization;
    public string elevenLabs;
}

public class AskMe : MonoBehaviour
{
    private const string ELEVENLABS_BASE_URL = "https://api.elevenlabs.io/v1/text-to-speech/";
    //private const string OPEN_AI_CHAT_URL = "https://api.openai.com/v1/chat/completions";

    [SerializeField] private EventTrigger eventTrigger;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text message;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioClip recordSFX;
    [SerializeField] private AudioClip stopRecordSFX;

    private readonly string fileName = "output.wav";
    private readonly float duration = 10;

    private AudioClip clip;
    private bool canRecord = true;
    private bool isRecording;
    private float time;
    private OpenAIApi openai;
    private OpenAISecretKey openAISecretKey;
    public TextAsset text;

    private const string MOCHI_VOICE_KEY = "jdtEogghM74T0WObHkIa";
    private const string AROHA_VOICE_KEY = "";
    private const string GILMO_VOICE_KEY = "";
    private const string LORRY_VOICE_KEY = "";
    private const string OLGA_VOICE_KEY = "";

    private const string BABY_INSTRUCTION = "Sekarang kamu adalah [NAME]. Kamu harus memperkenalkan diri sebagai AMO. Kamu berusia 2 tahun, sehingga apa yang kamu harus jawab sesuai dengan usia mu. Kamu tidak perlu menyebut umurmu jika tidak ditanya umur";
    private const string TODDLER_INSTRUCTION = "Sekarang kamu adalah [NAME]. Kamu harus memperkenalkan diri sebagai AMO. Kamu berusia 2 tahun, sehingga apa yang kamu harus jawab sesuai dengan usia mu. Kamu tidak perlu menyebut umurmu jika tidak ditanya umur";
    private const string TEEN_INSTRUCTION = "Sekarang kamu adalah [NAME]. Kamu harus memperkenalkan diri sebagai AMO. Kamu berusia 11 tahun, sehingga apa yang kamu harus jawab sesuai dengan usia mu. Kamu adalah Android";
    private const string ANDROID_INSTRUCTION = "Sekarang kamu adalah [NAME]. Kamu harus memperkenalkan diri sebagai AMO. Kamu adalah Humanoid";
    private const string HUMANOID_INSTRUCTION = "Sekarang kamu adalah [NAME]. Kamu harus memperkenalkan diri sebagai AMO. Kamu adalah Android";

    private IEnumerator RequestOpenAISecretKey()
    {
        //if (text)
        //{
        //    Debug.LogWarning("text : " + text.ToString().Trim(' '));
        //    string result = Utils.DecryptXOR(text.text, "amoverse");
        //    Debug.LogWarning("result : " + result);
        //    openAISecretKey = JsonUtility.FromJson<OpenAISecretKey>(result);
        //}

        using (UnityWebRequest uwr = new UnityWebRequest("https://www.dropbox.com/scl/fi/jbcf5g8dd4gc0s9l3q7de/openAI.json?rlkey=7pjtak0azf87bfu0dodalzoeg&st=394nd3ys&dl=1"))
        {
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string json = uwr.downloadHandler.text;
                openAISecretKey = JsonUtility.FromJson<OpenAISecretKey>(json);
                Debug.LogWarning("key : " + json);
                openai = new OpenAIApi(openAISecretKey.apiKey, openAISecretKey.organization);
            }
            else
            {
                Debug.LogError("failed to get keys : " + uwr.error);
            }
        }

        //openAISecretKey = new OpenAISecretKey
        //{
        //    apiKey = "sk-proj-cgr9Pv8YZQQ0zuERz6BoT3BlbkFJPcnznTWTp7u9bvtsnD8u",
        //    organization = "org-cGPsbYflW34h5iD4eoYgVmJI"
        //};

        //Debug.LogWarning(Utils.EncryptXOR(JsonUtility.ToJson(openAISecretKey), "amoverse"));

    }

    private void Start()
    {
        //string encrypted = Utils.EncryptXOR("\"{ \\\"apiKey\\\" : \\\"sk-proj-79onAQqUEAGGgWfUMdk7T3BlbkFJ73jWQl0nCPKm4aUBmSsy\\\", \\\"organization\\\" : \\\"org-cGPsbYflW34h5iD4eoYgVmJI\\\" }\"", "amoverse");
        
        StartCoroutine(RequestOpenAISecretKey());
        //recordButton.onClick.AddListener(StartRecording);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(OnRecordPressed);
        eventTrigger.triggers.Add(entry);

         entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener(OnRecordReleased);
        eventTrigger.triggers.Add(entry);

        var index = PlayerPrefs.HasKey("user-mic-device-index") ? PlayerPrefs.GetInt("user-mic-device-index") : 0;
        Debug.LogWarning("index : " + index);
    }

    private void OnRecordPressed(BaseEventData eventData)
    {
        if (canRecord)
        {
            Debug.LogWarning("OnRecordPressed");
            StartRecording();
        }
    }

    private void OnRecordReleased(BaseEventData eventData)
    {
        Debug.LogWarning("OnRecordReleased");
        EndRecording();
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
    }

    private void StartRecording()
    {
        time = 0;
        canRecord = false;
        SoundManager.instance.PlaySFX(recordSFX);
        isRecording = true;

        var index = PlayerPrefs.GetInt("user-mic-device-index");
        Debug.LogWarning("index : " + index);
#if !UNITY_WEBGL
        clip = Microphone.Start(Microphone.devices[index].ToString(), false, Mathf.CeilToInt(duration), 44100);
#endif
    }

    private async void EndRecording()
    {
        if (isRecording)
        {
            SoundManager.instance.PlaySFX(stopRecordSFX);
            isRecording = false;
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
            if (res.Error != null)
            {
                if (!string.IsNullOrEmpty(res.Error.Message))
                {
                    Debug.LogError("err : " + res.Error.Message);
                    PopupManager.Instance.ShowPopupMessage("err", "Terjadi Kesalahan", "Silahkan hubungi tim kami di kontak Customer Service", new ButtonInfo { content = "Tutup" });
                }
            }
            
            progressBar.fillAmount = 0;
            message.text = res.Text;

            if (!string.IsNullOrEmpty(res.Text))
            {
                Debug.LogError("res : " + res.Text);
                if (res.Text.ToLower().Contains("reminder") || res.Text.ToLower().Contains("pengingat"))
                {
                    string toDoList = HomeController.Instance.toDoController.LoadNotesAsText();

                    Debug.LogError("todo : " + toDoList);
                    StartCoroutine(ProcessTextToSpeech(toDoList, audioClip =>
                    {
                        if (audioClip) Character.Instance.currentCharacter.PlayVoice(audioClip);
                        canRecord = true;
                    }));
                }
                else
                {
                    string instruction = "";
                    //Character.Instance.currentCharacter.info.stageType = AvatarInfo.StageType.Humanoid;
                    switch (Character.Instance.currentCharacter.info.stageType)
                    {
                        case AvatarInfo.StageType.Baby:
                            instruction = BABY_INSTRUCTION;
                            instruction = instruction.Replace("[NAME]", Character.Instance.GetCurrentAvatarInfo().avatarName);
                            break;
                        case AvatarInfo.StageType.Toddler:
                            instruction = TODDLER_INSTRUCTION;
                            instruction = instruction.Replace("[NAME]", Character.Instance.GetCurrentAvatarInfo().avatarName);
                            break;
                        case AvatarInfo.StageType.Teen:
                            instruction = TEEN_INSTRUCTION;
                            instruction = instruction.Replace("[NAME]", Character.Instance.GetCurrentAvatarInfo().avatarName);
                            break;
                        case AvatarInfo.StageType.Android:
                            instruction = ANDROID_INSTRUCTION;
                            instruction = instruction.Replace("[NAME]", Character.Instance.GetCurrentAvatarInfo().avatarName);
                            break;
                        case AvatarInfo.StageType.Humanoid:
                            instruction = HUMANOID_INSTRUCTION;
                            instruction = instruction.Replace("[NAME]", Character.Instance.GetCurrentAvatarInfo().avatarName);
                            break;
                    }
                    StartCoroutine(ProcessConversation(instruction, res.Text));
                }
            }
            else
            {
                Debug.LogError("err : " + res.Error);
                canRecord = true;
            }
        }
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
                EndRecording();
            }
        }
    }

    public IEnumerator ProcessConversation(string instruction, string text)
    {
        //DialogPromt data = new DialogPromt
        //{
        //    model = "gpt-3.5-turbo",
        //    messages = new OpenAIMessage[]
        //    {
        //       new OpenAIMessage {
        //           role = "system",
        //           content = "When I ask for help to write something, you are a baby. when I ask your name you answer you are \"AMO\". when I ask you with \"Do you know\", you answer with \"Yes, No or I don't know\". You can start with \"as I know as baby\" and your answer cannot be more than 10 words. your name is AMO"
        //       },
        //       new OpenAIMessage {
        //            role = "user",
        //            content = text
        //       }
        //    }
        //};
        ChatRequest data = new ChatRequest
        {
            message = instruction + ". " + text
        };

        string json = JsonUtility.ToJson(data);

        //using (UnityWebRequest uwr = UnityWebRequest.Post(OPEN_AI_CHAT_URL, json, "application/json"))
        //using (UnityWebRequest uwr = UnityWebRequest.Post("89.116.134.18:5050/chat", "{ \"message:\" : \"" + text + "\" }", "application/json"))
        using (UnityWebRequest uwr = UnityWebRequest.Post("http://89.116.134.18:5050/chat", json, "application/json"))
        {
            //uwr.SetRequestHeader("Authorization", "Bearer sk-proj-rcHexPB9URkrbLCulKFGT3BlbkFJTdHjzoDFChwn6NTl73rZ");
            uwr.downloadHandler = new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            Debug.LogWarning("result : " + uwr.result.ToString() + " " + uwr.downloadHandler.text);
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = uwr.downloadHandler.text;
                ChatResponse result = JsonUtility.FromJson<ChatResponse>(jsonResult);
                if (result != null)
                {
                    yield return ProcessTextToSpeech(result.response, audioClip => {
                        if (audioClip) Character.Instance.currentCharacter.PlayVoice(audioClip);//voiceSource.PlayOneShot(audioClip);
                    });
                }
                //ResultMessage result = JsonUtility.FromJson<ResultMessage>(jsonResult);
                //if (result.choices.Length > 0)
                //{
                //    if (result.choices[0].message != null)
                //    {
                //        string resultText = result.choices[0].message.content;
                //        yield return ProcessTextToSpeech(resultText, (audioClip) => {
                //            if(audioClip) voiceSource.PlayOneShot(audioClip);
                //        });
                //    }
                //}
            }
            else
            {
                Debug.LogError("err : " + uwr.error);
            }
            canRecord = true;
        }
    }

    public IEnumerator ProcessTextToSpeech(string text, Action<AudioClip> onComplete)
    {
        TextToSpeechData data = new TextToSpeechData
        {
            text = text,
            model_id = "eleven_multilingual_v2"
            //voice_settings = new VoiceSettings { stability = 50, similarity_boost = 75, use_speaker_boost = true },
            //pronunciation_dictionary_locators = new PronunciationDictionaryLocators[]
            //{
            //    new PronunciationDictionaryLocators{ version_id = "0.1", pronunciation_dictionary_id = "id"}                
            //}
        };
        string json = JsonUtility.ToJson(data);

        string voiceCharacter = "";
        switch (Character.Instance.currentCharacter.info.avatarName)
        {
            case Consts.MOCHI:
                voiceCharacter = MOCHI_VOICE_KEY;
                break;
            case Consts.AROHA:
                voiceCharacter = AROHA_VOICE_KEY;
                break;
            case Consts.GILMO:
                voiceCharacter = GILMO_VOICE_KEY;
                break;
            case Consts.LORRY:
                voiceCharacter = LORRY_VOICE_KEY;
                break;
            case Consts.OLGA:
                voiceCharacter = OLGA_VOICE_KEY;
                break;
        }

        using (UnityWebRequest uwr = UnityWebRequest.Post(ELEVENLABS_BASE_URL + voiceCharacter, json, "application/json"))
        {
            uwr.downloadHandler = new DownloadHandlerAudioClip(ELEVENLABS_BASE_URL + voiceCharacter, AudioType.MPEG);
            uwr.SetRequestHeader("xi-api-key", openAISecretKey.elevenLabs);
            yield return uwr.SendWebRequest();
            Debug.LogError("result : " + uwr.result.ToString() + " " + uwr.downloadHandler.ToString());
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                //byte[] results = uwr.downloadHandler.data;
                //float[] samples = new float[results.Length / 4];

                //Buffer.BlockCopy(results, 0, samples, 0, samples.Length);

                //int channels = 1; //Assuming audio is mono because microphone input usually is
                //int sampleRate = 44100; //Assuming your samplerate is 44100 or change to 48000 or whatever is appropriate


                // ((DownloadHandlerAudioClip)uwr.downloadHandler).audioClip;
                //AudioClip clip = AudioClip.Create("AMO_Speech", samples.Length, channels, sampleRate, false);

                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                Debug.LogWarning("clip length : " + clip.length);
                onComplete?.Invoke(clip);
            }
            else
            {
                Debug.LogError("err : " + uwr.error);
            }
            canRecord = true;
        }
    }
}