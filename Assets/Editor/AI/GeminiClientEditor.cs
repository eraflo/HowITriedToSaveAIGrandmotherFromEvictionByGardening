using Assets.Scripts.GeminiAI;
using Assets.Scripts.GeminiAI.Enums;
using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Request.Bidi;
using Assets.Scripts.GeminiAI.Parts;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

[CustomEditor(typeof(GeminiClient)), CanEditMultipleObjects]
public class GeminiClientEditor : Editor
{
    private string response = "";
    private string prompt = "";

    private List<string> safetyCategories = new List<string>();
    private List<string> safetyThresholds = new List<string>();
    public List<string> modalities = new List<string>();
    public List<string> prebuiltVoices = new List<string>();
    public List<string> mimeTypes = new List<string>();
    public List<string> responseModalities = new List<string>();
    public List<string> mediaResolutions = new List<string>();

    private bool showPresencePenalty = false;
    private bool showFrequencyPenalty = false;
    private bool activateLogprob = false;

    private SerializedProperty _geminiClient;

    private Vector2 scrollPosition;

    private void OnEnable()
    {
        _geminiClient = serializedObject.FindProperty("_options");
        safetyThresholds = EnumUtils.GetEnumFields<HarmThreshold>();
        safetyCategories = EnumUtils.GetEnumFields<HarmCategory>();
        modalities = EnumUtils.GetEnumFields<Modality>();
        responseModalities = EnumUtils.GetEnumFields<ResponseModality>();
        prebuiltVoices = EnumUtils.GetEnumFields<PrebuiltVoice>();
        mediaResolutions = EnumUtils.GetEnumFields<MediaResolution>();
        mimeTypes = MimeType.MimeTypes;
    }

    private void OnValidate()
    {
        response = "";
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        GeminiClient myScript = (GeminiClient)target;

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Connexion Type", EditorStyles.boldLabel);

        myScript.ConnexionType = (ConnexionType)EditorGUILayout.EnumPopup("Connexion Type", myScript.ConnexionType);

        EditorGUILayout.Space();

        switch (myScript.ConnexionType)
        {
            case ConnexionType.Http:
                //myScript.Options.Url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent";
                myScript.ChangeConnexionType(ConnexionType.Http, myScript.Options.Interaction);
                DisplayHttpEditor(myScript);
                break;
            case ConnexionType.WebSocket:

                // Default URL for the WebSocket
                //myScript.Options.Url = "wss://generativelanguage.googleapis.com/ws/google.ai.generativelanguage.v1alpha.GenerativeService.BidiGenerateContent";
                myScript.ChangeConnexionType(ConnexionType.WebSocket, myScript.Options.Interaction);
                DisplayWebSocketEditor(myScript);
                break;
        }


    }

    private void DisplayHttpEditor(GeminiClient myScript)
    {
        AIGenerationConfigMimeType(myScript);

        AIGenerationConfigDisplay(myScript);

        AIGenerationConfigStopSequencesDisplay(myScript);

        AIGenerationConfigSpeechConfig(myScript);

        SafetySettingsDisplay(myScript);

        AIGenerationConfigResponseModalitiesDisplay(myScript);


        // Draw field with prompt and button
        EditorGUILayout.LabelField("Generate Content", EditorStyles.boldLabel);
        prompt = EditorGUILayout.TextField("Prompt", prompt);


        if (GUILayout.Button("Generate"))
        {
            Task.Run(async () => await GenerateTextPrompt(myScript, prompt));

            // Deactivate the button
            GUI.enabled = false;
        }

        // Display the response
        if (!string.IsNullOrEmpty(response))
        {
            EditorGUILayout.LabelField("Response", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(response);
        }
    }

    private async void DisplayWebSocketEditor(GeminiClient myScript)
    {
        EditorGUILayout.Space();

        AIGenerationConfigMimeType(myScript);

        AIGenerationConfigDisplay(myScript);

        AIGenerationConfigStopSequencesDisplay(myScript);

        AIGenerationConfigSpeechConfig(myScript);

        SafetySettingsDisplay(myScript);

        AIGenerationConfigResponseModalitiesDisplay(myScript);

        if (GUILayout.Button("Connect") && !string.IsNullOrEmpty(myScript.Options.Model))
        {
            Task.Run(async () => await myScript.Connect(myScript.Options.Model));

            // Deactivate the button
            GUI.enabled = false;
        }

        if (GUILayout.Button("Disconnect"))
        {
            Task.Run(async () => await myScript.Disconnect());
        }


        // Draw field with prompt and button
        EditorGUILayout.LabelField("Generate Content", EditorStyles.boldLabel);
        prompt = EditorGUILayout.TextField("Prompt", prompt);

        if (GUILayout.Button("Generate"))
        {

            var receivedResponse = false;

            this.response = "";

            // Needed to get the response from the server
            var onContentReceived = new System.Action<string>((response) =>
            {
                var receiveMessageObj = myScript.GetResponseObject<ReceiveMessage>(response);

                if(receiveMessageObj.ServerContent.TurnComplete == true)
                {
                    receivedResponse = true;
                    Debug.Log("Response received");
                }
                else
                    this.response += receiveMessageObj.ServerContent.ModelTurn.Parts[0].Text;
            });


            myScript.ListenToContentReceived(onContentReceived);

            await myScript.GenerateMultiModalRequestAsync(prompt);

            // Make sure we stop listening to the content received event
            Task.Run(async () =>
            {
                while (!receivedResponse)
                {
                    await Task.Delay(100);
                }

                myScript.RemoveContentReceivedListener(onContentReceived);
                Debug.Log("Content generated.");
                GUI.enabled = true;
            });
            

            // Deactivate the button
            GUI.enabled = false;
        }

        // Display the response
        if (!string.IsNullOrEmpty(response))
        {
            EditorGUILayout.LabelField("Response", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            // Read-only style
            GUIStyle readOnlyStyle = new GUIStyle(EditorStyles.textArea);
            readOnlyStyle.normal.textColor = GUI.skin.label.normal.textColor;
            readOnlyStyle.wordWrap = true;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(response ?? "", readOnlyStyle);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();
        }
    }

    

    #region Display Functions

    private void AIGenerationConfigDisplay(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Generation Config", EditorStyles.boldLabel);

        myScript.GenerationConfig.CandidateCount = EditorGUILayout.IntField("Candidate Count", myScript.GenerationConfig.CandidateCount);
        myScript.GenerationConfig.MaxOutputTokens = EditorGUILayout.IntField("Max Output Tokens", myScript.GenerationConfig.MaxOutputTokens);
        myScript.GenerationConfig.Temperature = EditorGUILayout.FloatField("Temperature", myScript.GenerationConfig.Temperature);
        myScript.GenerationConfig.TopP = EditorGUILayout.FloatField("Top P", myScript.GenerationConfig.TopP);
        myScript.GenerationConfig.TopK = EditorGUILayout.IntField("Top K", myScript.GenerationConfig.TopK);
        myScript.GenerationConfig.Seed = EditorGUILayout.IntField("Seed", myScript.GenerationConfig.Seed);

        // Toggle for the presence penalty
        showPresencePenalty = EditorGUILayout.Toggle("Show Presence Penalty", showPresencePenalty);

        if (showPresencePenalty)
            myScript.GenerationConfig.PresencePenalty = EditorGUILayout.FloatField("Presence Penalty", (float)myScript.GenerationConfig.PresencePenalty);
        else
            myScript.GenerationConfig.PresencePenalty = null;


        // Toggle for the frequency penalty
        showFrequencyPenalty = EditorGUILayout.Toggle("Show Frequency Penalty", showFrequencyPenalty);

        if (showFrequencyPenalty)
            myScript.GenerationConfig.FrequencyPenalty = EditorGUILayout.FloatField("Frequency Penalty", (float)myScript.GenerationConfig.FrequencyPenalty);
        else
            myScript.GenerationConfig.FrequencyPenalty = null;

        // Toggle for the logprob
        activateLogprob = EditorGUILayout.Toggle("Activate Logprob", activateLogprob);

        if (activateLogprob)
        {
            if(myScript.GenerationConfig.ResponseLogprobs == null)
            {
                myScript.GenerationConfig.ResponseLogprobs = false;
            }

            if (myScript.GenerationConfig.Logprobs == null)
            {
                myScript.GenerationConfig.Logprobs = 0;
            }

            myScript.GenerationConfig.ResponseLogprobs = EditorGUILayout.Toggle("Response Logprobs", (bool)myScript.GenerationConfig.ResponseLogprobs);
            myScript.GenerationConfig.Logprobs = EditorGUILayout.IntField("Logprobs", (int)myScript.GenerationConfig.Logprobs);
        }
        else
        {
            myScript.GenerationConfig.ResponseLogprobs = null;
            myScript.GenerationConfig.Logprobs = null;
        }

        myScript.GenerationConfig.EnableEnhancedCivicAnswers = EditorGUILayout.Toggle("Enable Enhanced Civic Answers", myScript.GenerationConfig.EnableEnhancedCivicAnswers);

        EditorGUILayout.Space();
    }

    private void AIGenerationConfigResponseModalitiesDisplay(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Response Modalities", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Response Modality", GUILayout.Width(150));

        EditorGUILayout.BeginVertical();

        if (myScript.GenerationConfig.ResponseModalities == null)
        {
            myScript.GenerationConfig.ResponseModalities = new ResponseModality[0];
        }

        for (int i = 0; i < myScript.GenerationConfig.ResponseModalities.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            var selected = responseModalities.IndexOf(EnumUtils.GetEnumFieldToString(myScript.GenerationConfig.ResponseModalities[i]));

            if (selected == -1)
            {
                selected = 0;
            }

            selected = EditorGUILayout.Popup(selected, responseModalities.ToArray());
            myScript.GenerationConfig.ResponseModalities[i] = EnumUtils.GetEnumFieldFromString<ResponseModality>(responseModalities[selected]);


            if (GUILayout.Button("Remove"))
            {
                myScript.GenerationConfig.ResponseModalities = ArrayUtils.RemoveAt(myScript.GenerationConfig.ResponseModalities, i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add"))
        {
            myScript.GenerationConfig.ResponseModalities = ArrayUtils.Add(myScript.GenerationConfig.ResponseModalities, ResponseModality.TEXT);
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    private void AIGenerationConfigSpeechConfig(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Speech Config", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Prebuilt Voice", GUILayout.Width(150));

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Voice Name", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        if (myScript.GenerationConfig.SpeechConfig == null)
        {
            myScript.GenerationConfig.SpeechConfig = new SpeechConfig();
            myScript.GenerationConfig.SpeechConfig.VoiceConfig = new VoiceConfig();
            myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig = new PrebuiltVoiceConfig();
            myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName = PrebuiltVoice.None;
        }

        var selected = prebuiltVoices.IndexOf(EnumUtils.GetEnumFieldToString(myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName));

        selected = EditorGUILayout.Popup(selected, prebuiltVoices.ToArray());

        if(selected != 0)
        {
            myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName = EnumUtils.GetEnumFieldFromString<PrebuiltVoice>(prebuiltVoices[selected]);
        }
        else
        {
            myScript.GenerationConfig.SpeechConfig = null;
        }
        

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    private void AIGenerationConfigStopSequencesDisplay(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Stop Sequences", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Stop Sequence", GUILayout.Width(100));

        EditorGUILayout.BeginVertical();

        if (myScript.GenerationConfig.StopSequences == null)
        {
            myScript.GenerationConfig.StopSequences = new string[0];
        }

        for (int i = 0; i < myScript.GenerationConfig.StopSequences.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            myScript.GenerationConfig.StopSequences[i] = EditorGUILayout.TextField(myScript.GenerationConfig.StopSequences[i].ToString());
            if (GUILayout.Button("Remove"))
            {
                myScript.GenerationConfig.StopSequences = ArrayUtils.RemoveAt(myScript.GenerationConfig.StopSequences, i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add"))
        {
            myScript.GenerationConfig.StopSequences = ArrayUtils.Add(myScript.GenerationConfig.StopSequences, "");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    private void AIGenerationConfigMediaResolution(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Media Resolution", EditorStyles.boldLabel);
        var selected = mediaResolutions.IndexOf(myScript.GenerationConfig.MediaResolution.ToString());

        if (selected == -1)
        {
            selected = 0;
        }

        selected = EditorGUILayout.Popup(selected, mediaResolutions.ToArray());
        myScript.GenerationConfig.MediaResolution = EnumUtils.GetEnumFieldFromString<MediaResolution>(mediaResolutions[selected]);

        EditorGUILayout.Space();
    }

    private void AIGenerationConfigMimeType(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Response Mime Type", EditorStyles.boldLabel);
        var selected = mimeTypes.IndexOf(myScript.GenerationConfig.ResponseMimeType);

        if (selected == -1)
        {
            selected = 0;
        }

        selected = EditorGUILayout.Popup(selected, mimeTypes.ToArray());
        myScript.GenerationConfig.ResponseMimeType = mimeTypes[selected];

        EditorGUILayout.Space();
    }

    private void SafetySettingsDisplay(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Safety Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Safety Setting", GUILayout.Width(100));
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Category", GUILayout.Width(100));
        EditorGUILayout.LabelField("Threshold", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        if (myScript.SafetySettings == null)
        {
            myScript.SafetySettings = new List<SafetySettings>();
        }

        for (int i = 0; i < myScript.SafetySettings.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // Display all safety categories in a dropdown. The user can select one. Initially, the selected value is the first one.
            var selectedCategory = safetyCategories.IndexOf(EnumUtils.GetEnumFieldToString(myScript.SafetySettings[i].Category));

            if (selectedCategory == -1) 
            {
                selectedCategory = 0;
            }

            selectedCategory = EditorGUILayout.Popup(selectedCategory, safetyCategories.ToArray());
            myScript.SafetySettings[i].Category = EnumUtils.GetEnumFieldFromString<HarmCategory>(safetyCategories[selectedCategory]);


            // Display all safety thresholds in a dropdown. The user can select one. Initially, the selected value is the first one.
            var selected = safetyThresholds.IndexOf(EnumUtils.GetEnumFieldToString(myScript.SafetySettings[i].Threshold));

            if (selected == -1)
            {
                selected = 0;
            }

            selected = EditorGUILayout.Popup(selected, safetyThresholds.ToArray());
            myScript.SafetySettings[i].Threshold = EnumUtils.GetEnumFieldFromString<HarmThreshold>(safetyThresholds[selected]);


            if (GUILayout.Button("Remove"))
            {
                myScript.SafetySettings.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add"))
        {
            myScript.SafetySettings.Add(new SafetySettings());
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    #endregion

    private async Task GenerateTextPrompt(GeminiClient client, string prompt)
    {
        Debug.Log("Generating content...");
        Task<string> task = client.GenerateTextContentAsync(prompt, new CancellationTokenSource().Token);
        await task;

        Debug.Log("Content generated.");

        response = task.Result;
        GUI.enabled = true;
    }

    private async Task GeneratePrompt(GeminiClient client, string prompt)
    {
        Debug.Log("Generating content...");
        Task<string> task = client.GenerateTextContentAsync(prompt, new CancellationTokenSource().Token);
        await task;

        Debug.Log("Content generated.");

        response = task.Result;
        GUI.enabled = true;
    }
}

