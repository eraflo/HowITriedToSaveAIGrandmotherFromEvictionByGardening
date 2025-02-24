using Assets.Scripts.GeminiAI;
using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeminiClient)), CanEditMultipleObjects]
public class GeminiClientEditor : Editor
{
    private string response = "";
    private string prompt = "";
    private string model = "";

    private List<string> safetyCategories = new List<string>();
    private List<string> safetyThresholds = new List<string>();
    public List<string> responseModalities = new List<string>();
    public List<string> prebuiltVoices = new List<string>();

    private SerializedProperty _geminiClient;

    private void OnEnable()
    {
        _geminiClient = serializedObject.FindProperty("_options");
        safetyThresholds = EnumUtils.GetEnumFields<SafetyThreshold>();
        safetyCategories = EnumUtils.GetEnumFields<SafetyCategory>();
        responseModalities = EnumUtils.GetEnumFields<Modality>();
        prebuiltVoices = EnumUtils.GetEnumFields<PrebuiltVoice>();
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
                DisplayHttpEditor(myScript);
                break;
            case ConnexionType.WebSocket:
                DisplayWebSocketEditor(myScript);
                break;
        }


    }

    private void DisplayHttpEditor(GeminiClient myScript)
    {
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

    private void DisplayWebSocketEditor(GeminiClient myScript)
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Model", EditorStyles.boldLabel);
        model = EditorGUILayout.TextField("Model", model);

        EditorGUILayout.Space();

        if (GUILayout.Button("Connect") && !string.IsNullOrEmpty(model))
        {
            Task.Run(async () => await myScript.Connect(myScript.Options.Url, myScript.Options.ApiKey, model));

            // Deactivate the button
            GUI.enabled = false;
        }

        if(GUILayout.Button("Disconnect"))
        {
            Task.Run(async () => await myScript.Disconnect());
        }
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
            myScript.GenerateMultiModalRequestAsync(prompt);

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

    private async Task GenerateTextPrompt(GeminiClient client, string prompt)
    {
        Debug.Log("Generating content...");
        Task<string> task = client.GenerateTextContentAsync(prompt, new CancellationTokenSource().Token);
        await task;

        Debug.Log("Content generated.");

        response = task.Result;
        GUI.enabled = true;
    }

    #region Display Functions

    private void AIGenerationConfigDisplay(GeminiClient myScript)
    {
        EditorGUILayout.LabelField("Generation Config", EditorStyles.boldLabel);
        myScript.GenerationConfig.MaxOutputTokens = EditorGUILayout.IntField("Max Output Tokens", myScript.GenerationConfig.MaxOutputTokens);
        myScript.GenerationConfig.Temperature = EditorGUILayout.FloatField("Temperature", myScript.GenerationConfig.Temperature);
        myScript.GenerationConfig.TopP = EditorGUILayout.FloatField("Top P", myScript.GenerationConfig.TopP);
        myScript.GenerationConfig.TopK = EditorGUILayout.FloatField("Top K", myScript.GenerationConfig.TopK);

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
            myScript.GenerationConfig.ResponseModalities = new string[0];
        }

        for (int i = 0; i < myScript.GenerationConfig.ResponseModalities.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            var selected = responseModalities.IndexOf(myScript.GenerationConfig.ResponseModalities[i]);

            if (selected == -1)
            {
                selected = 0;
            }

            selected = EditorGUILayout.Popup(selected, responseModalities.ToArray());
            myScript.GenerationConfig.ResponseModalities[i] = responseModalities[selected];


            if (GUILayout.Button("Remove"))
            {
                myScript.GenerationConfig.ResponseModalities = ArrayUtils.RemoveAt(myScript.GenerationConfig.ResponseModalities, i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add"))
        {
            myScript.GenerationConfig.ResponseModalities = ArrayUtils.Add(myScript.GenerationConfig.ResponseModalities, "");
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
            myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName = EnumUtils.GetEnumFieldToString(PrebuiltVoice.None);
        }

        var selected = prebuiltVoices.IndexOf(myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName);

        selected = EditorGUILayout.Popup(selected, prebuiltVoices.ToArray());

        if(selected != 0)
        {
            myScript.GenerationConfig.SpeechConfig.VoiceConfig.PrebuiltVoiceConfig.VoiceName = prebuiltVoices[selected];
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
            myScript.GenerationConfig.StopSequences = new List<object>();
        }

        for (int i = 0; i < myScript.GenerationConfig.StopSequences.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            myScript.GenerationConfig.StopSequences[i] = EditorGUILayout.TextField(myScript.GenerationConfig.StopSequences[i].ToString());
            if (GUILayout.Button("Remove"))
            {
                myScript.GenerationConfig.StopSequences.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add"))
        {
            myScript.GenerationConfig.StopSequences.Add("");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

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
            var selectedCategory = safetyCategories.IndexOf(myScript.SafetySettings[i].Category);

            if (selectedCategory == -1) 
            {
                selectedCategory = 0;
            }

            selectedCategory = EditorGUILayout.Popup(selectedCategory, safetyCategories.ToArray());
            myScript.SafetySettings[i].Category = safetyCategories[selectedCategory];


            // Display all safety thresholds in a dropdown. The user can select one. Initially, the selected value is the first one.
            var selected = safetyThresholds.IndexOf(myScript.SafetySettings[i].Threshold);

            if (selected == -1)
            {
                selected = 0;
            }

            selected = EditorGUILayout.Popup(selected, safetyThresholds.ToArray());
            myScript.SafetySettings[i].Threshold = safetyThresholds[selected];


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

