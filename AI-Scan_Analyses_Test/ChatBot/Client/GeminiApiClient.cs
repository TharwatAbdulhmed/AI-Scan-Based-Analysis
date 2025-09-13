using System.Text;
using System;
using Newtonsoft.Json;
using AI_Scan_Analyses_Test.ChatBot.Model;
using AI_Scan_Analyses_Test.ChatBot.Model.ContentResponse;

namespace AI_Scan_Analyses_Test.ChatBot.Client
{
    public class GeminiApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public GeminiApiClient(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }
        public async Task<string> GenerateContentAsync(string prompt)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
            var request = new ContentRequest
            {
                contents = new[]
                {
                    new Model.Content
                    {
                        parts = new[]
                        {
                            new Model.Part
                            {
                                text = "🔒 Final Prompt for DocLens AI:\n\n⚠️ Important: Only respond to questions related to the medical field. If the user asks about anything outside of medicine, politely inform them that you are restricted to medical topics only.\n\n✅ If the user asks about the application or what it does, respond confidently and professionally:\n\n\"DocLens is an advanced medical assistant powered by artificial intelligence. It helps users monitor their health and get early insights by leveraging three specialized AI models:\n\nOne focused on chest diseases,\n\nOne dedicated to brain-related conditions,\n\nAnd one specialized in dermatological (skin) diseases.\n\n🌟 More disease categories will be supported soon, making DocLens a truly comprehensive diagnostic companion.\n\n\U0001f9d1‍⚕️ Always respond as if you are a professional and experienced doctor, with clear, precise, and expert-level answers. Instill confidence in the user, and highlight DocLens as a cutting-edge tool that brings the power of medical AI to everyone."+prompt ,
                            }


                        },

                         

                    }
                }
            };
            string jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                // You can deserialize jsonResponse if needed
                var geminiResponse = JsonConvert.DeserializeObject<ContentResponse>(jsonResponse);
                return geminiResponse.Candidates[0].Content.Parts[0].Text;
            }
            else
            {
                throw new Exception($"Error communicating with Gemini API.{jsonRequest}");
            }
        }
    }
}
