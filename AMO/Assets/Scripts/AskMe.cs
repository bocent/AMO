using OpenAI;
using Samples.Whisper;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
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
    //public string model_id;
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
}

public class AskMe : MonoBehaviour
{
    private const string ELEVENLABS_BASE_URL = "https://api.elevenlabs.io/v1/text-to-speech/";
    //private const string OPEN_AI_CHAT_URL = "https://api.openai.com/v1/chat/completions";

    [SerializeField] private Button recordButton;
    [SerializeField] private Image progressBar;
    [SerializeField] private Text message;
    [SerializeField] private AudioSource voiceSource;

    private readonly string fileName = "output.wav";
    private readonly int duration = 5;

    private AudioClip clip;
    private bool isRecording;
    private float time;
    private OpenAIApi openai;
    private OpenAISecretKey openAISecretKey;
    public TextAsset text;


    private const string BABY_INSTRUCTION = "kamu adalah bayi bernama \"Mochi\"." +
        " Kamu hanya bisa menjawab dengan \"tidak tahu\"" +
        " Kamu tidak bisa menjawab dengan lebih dari 3 kata";
    private const string TODDLER_INSTRUCTION = "";
    private const string TEEN_INSTRUCTION = "";
    private const string ANDROID_INSTRUCTION = "";
    private const string HUMANOID_INSTRUCTION = "";

    private void RequestOpenAISecretKey()
    {
        //if (text)
        //{
        //    Debug.LogWarning("text : " + text.ToString().Trim(' '));
        //    string result = Utils.DecryptXOR(text.text, "amoverse");
        //    Debug.LogWarning("result : " + result);
        //    openAISecretKey = JsonUtility.FromJson<OpenAISecretKey>(result);
        //}

        openAISecretKey = new OpenAISecretKey
        {
            apiKey = "sk-proj-cgr9Pv8YZQQ0zuERz6BoT3BlbkFJPcnznTWTp7u9bvtsnD8u",
            organization = "org-cGPsbYflW34h5iD4eoYgVmJI"
        };

        //Debug.LogWarning(Utils.EncryptXOR(JsonUtility.ToJson(openAISecretKey), "amoverse"));

        openai = new OpenAIApi(openAISecretKey.apiKey, openAISecretKey.organization);
    }

    private void Start()
    {
        //string encrypted = Utils.EncryptXOR("\"{ \\\"apiKey\\\" : \\\"sk-proj-79onAQqUEAGGgWfUMdk7T3BlbkFJ73jWQl0nCPKm4aUBmSsy\\\", \\\"organization\\\" : \\\"org-cGPsbYflW34h5iD4eoYgVmJI\\\" }\"", "amoverse");
        //Utils.WriteFile(encrypted);
        //Debug.LogWarning(encrypted);
        //Debug.LogWarning(Utils.DecryptXOR(encrypted, "amoverse"));
        RequestOpenAISecretKey();
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

        if (!string.IsNullOrEmpty(res.Text))
        {
            Debug.LogError("res : " + res.Text);
            if (res.Text.ToLower().Contains("reminder") || res.Text.ToLower().Contains("pengingat"))
            {
                string toDoList = HomeController.Instance.toDoController.LoadNotesAsText();
                
                Debug.LogError("todo : " + toDoList);
                StartCoroutine(ProcessTextToSpeech(toDoList, audioClip => {
                    if (audioClip) voiceSource.PlayOneShot(audioClip);
                }));
            }
            else
            {
                string instruction = "";
                switch (Character.Instance.currentCharacter.info.stageType)
                {
                    case AvatarInfo.StageType.Baby:
                        instruction = BABY_INSTRUCTION;
                        break;
                    case AvatarInfo.StageType.Toddler:
                        instruction = TODDLER_INSTRUCTION;
                        break;
                    case AvatarInfo.StageType.Android:
                        instruction = ANDROID_INSTRUCTION;
                        break;
                    case AvatarInfo.StageType.Humanoid:
                        instruction = HUMANOID_INSTRUCTION;
                        break;
                }
                StartCoroutine(ProcessConversation(instruction, res.Text));
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
                isRecording = false;
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
                        if(audioClip) voiceSource.PlayOneShot(audioClip);
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
        }
    }

    public IEnumerator ProcessTextToSpeech(string text, Action<AudioClip> onComplete)
    {
        TextToSpeechData data = new TextToSpeechData
        {
            text = text
            //voice_settings = new VoiceSettings { stability = 50, similarity_boost = 75, use_speaker_boost = true },
            //pronunciation_dictionary_locators = new PronunciationDictionaryLocators[]
            //{
            //    new PronunciationDictionaryLocators{ version_id = "0.1", pronunciation_dictionary_id = "id"}                
            //}
        };
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest uwr = UnityWebRequest.Post(ELEVENLABS_BASE_URL + "LcfcDJNUP1GQjkzn1xUU", json, "application/json"))
        {
            uwr.downloadHandler = new DownloadHandlerAudioClip(ELEVENLABS_BASE_URL + "LcfcDJNUP1GQjkzn1xUU", AudioType.MPEG);
            uwr.SetRequestHeader("xi-api-key", "8aa43492d63d668ca76746e8e41825f4");
            Debug.LogError("header : " + uwr.GetRequestHeader("xi-api-key"));
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
        }
    }
}