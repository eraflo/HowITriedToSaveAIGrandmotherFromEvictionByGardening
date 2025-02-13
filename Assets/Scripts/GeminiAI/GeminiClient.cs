using Assets.Scripts.GeminiAI.Request;
using Assets.Scripts.GeminiAI.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GeminiAI
{
    [Serializable]
    public class GeminiOptions
    {
        public string ApiKey;
        public string Url;
    }

    [CreateAssetMenu(fileName = "GeminiClient", menuName = "Gemini/GeminiClient")]
    public class GeminiClient : ScriptableObject
    {
        [SerializeField] private GeminiOptions _options = new();

        private HttpClient _httpClient;
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        public AIGenerationConfig GenerationConfig { get; set; } = new();
        public List<SafetySettings> SafetySettings { get; set; } = new();

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(_options.ApiKey))
            {
                Debug.LogError("Gemini API Key is required.");
            }

            if (string.IsNullOrEmpty(_options.Url))
            {
                Debug.LogError("Gemini URL is required.");
            }

            if(_httpClient == null)
            {
                _httpClient = new HttpClient(new GeminiDelegatingHandler(_options));
            }
        }

        public async Task<string> GenerateContentAsync(string input, CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = APIRequestFactory.CreateRequest(input, GenerationConfig, SafetySettings.ToArray());
                var serializedRequestBody = JsonConvert.SerializeObject(requestBody, _serializerSettings);
                var content = new StringContent(serializedRequestBody, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_options.Url, content, cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                var geminiResponse = JsonConvert.DeserializeObject<AIResponse>(responseBody);

                var geminiResponseText = geminiResponse?.Candidates[0].Content.Parts[0].Text;

                return geminiResponseText;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error generating content: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
