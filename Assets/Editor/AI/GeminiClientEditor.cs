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

    private List<string> safetyCategories = new List<string>();
    private List<string> safetyThresholds = new List<string>();

    private SerializedProperty _geminiClient;

    private void OnEnable()
    {
        _geminiClient = serializedObject.FindProperty("_options");
        safetyThresholds = EnumUtils.GetEnumFields<SafetyThreshold>();
        safetyCategories = EnumUtils.GetEnumFields<SafetyCategory>();
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

        // Draw fields for AIGenerationConfig
        EditorGUILayout.LabelField("Generation Config", EditorStyles.boldLabel);
        myScript.GenerationConfig.MaxOutputTokens = EditorGUILayout.IntField("Max Output Tokens", myScript.GenerationConfig.MaxOutputTokens);
        myScript.GenerationConfig.Temperature = EditorGUILayout.FloatField("Temperature", myScript.GenerationConfig.Temperature);
        myScript.GenerationConfig.TopP = EditorGUILayout.FloatField("Top P", myScript.GenerationConfig.TopP);
        myScript.GenerationConfig.TopK = EditorGUILayout.FloatField("Top K", myScript.GenerationConfig.TopK);

        // Fields for StopSequences
        EditorGUILayout.LabelField("Stop Sequences", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Stop Sequence", GUILayout.Width(100));

        EditorGUILayout.BeginVertical();

        if(myScript.GenerationConfig.StopSequences == null)
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

        // Fields for SafetySettings
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



        // Draw field with prompt and button
        EditorGUILayout.LabelField("Generate Content", EditorStyles.boldLabel);
        prompt = EditorGUILayout.TextField("Prompt", prompt);


        if (GUILayout.Button("Generate"))
        {
            Task.Run(async () => await GeneratePrompt(myScript, prompt));

            // Deactivate the button
            GUI.enabled = false;
        }

        // Display the response
        if(!string.IsNullOrEmpty(response))
        {
            EditorGUILayout.LabelField("Response", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(response);
        }
    }

    private async Task GeneratePrompt(GeminiClient client, string prompt)
    {
        Debug.Log("Generating content...");
        Task<string> task = client.GenerateContentAsync(prompt, new CancellationTokenSource().Token);
        await task;

        Debug.Log("Content generated.");

        response = task.Result;
        GUI.enabled = true;
    }
}

