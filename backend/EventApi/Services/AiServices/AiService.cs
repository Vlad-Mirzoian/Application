using EventApi.Dtos.AiDtos;
using System.Text;
using System.Text.Json;

namespace EventApi.Services.AiServices
{
    public class AiService : IAiService
    {
        private readonly IAiDataProvider _dataProvider;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AiService(IAiDataProvider dataProvider, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _dataProvider = dataProvider;
            _httpClient = httpClientFactory.CreateClient("GroqClient");
            _config = config;
        }

        public async Task<AiResponseDto> AskAsync(string question, Guid userId)
        {
            var context = await _dataProvider.GetContextAsync(userId, question);
            var answer = await CallGroqMultiStepAsync(question, context);
            return new AiResponseDto { Response = answer ?? "No response from AI." };
        }

        private async Task<string> CallGroqMultiStepAsync(string question, AiContextDto context)
        {
            var messages = new List<object>
        {
            new { role = "system", content = "You are a helpful event assistant. First, analyze the data. Then, answer the question." }
        };

            var contextPrompt = BuildContextPrompt(context);
            messages.Add(new { role = "user", content = contextPrompt });

            var analysis = await CallGroqOnceAsync(messages);
            if (string.IsNullOrWhiteSpace(analysis))
                return "No events found.";

            messages.Add(new { role = "assistant", content = analysis });

            messages.Add(new { role = "user", content = $"Now, answer this question: {question}" });

            var finalAnswer = await CallGroqOnceAsync(messages);
            return finalAnswer ?? "No response from AI.";
        }

        private string BuildContextPrompt(AiContextDto context)
        {
            var userEvents = string.Join("\n", context.UserEvents.Select(e =>
                $"- {e.Title} | {e.Start:yyyy-MM-dd HH:mm} UTC | {e.Location} | " +
                $"User's role: {e.Role} | Tags: {string.Join(", ", e.Tags)} | " +
                $"Participants: {(e.ParticipantNames.Any() ? string.Join(", ", e.ParticipantNames) : "None")}"
            ));

            var publicEvents = string.Join("\n", context.PublicEvents.Select(e =>
                $"- {e.Title} | {e.Start:yyyy-MM-dd HH:mm} UTC | {e.Location} | " +
                $"Tags: {string.Join(", ", e.Tags)} | " +
                $"Participants: {(e.ParticipantNames.Any() ? string.Join(", ", e.ParticipantNames) : "None")}"
            ));

            return $@"
Current time (UTC): {context.CurrentTime:yyyy-MM-dd HH:mm:ss}

=== MY EVENTS (organizing or attending) ===
{(string.IsNullOrEmpty(userEvents) ? "None" : userEvents)}

=== PUBLIC EVENTS (if relevant) ===
{(string.IsNullOrEmpty(publicEvents) ? "None" : publicEvents)}

Please analyze this data and prepare to answer questions about it.
".Trim();
        }

        private async Task<string> CallGroqOnceAsync(List<object> messages)
        {
            var request = new
            {
                model = "openai/gpt-oss-20b",
                messages,
                max_tokens = 500,
                temperature = 0.5
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = false });
            Console.WriteLine("Groq Request:\n" + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Groq error: {response.StatusCode} - {error}");
            }

            var groqResponse = await response.Content.ReadFromJsonAsync<GroqResponse>();
            if (groqResponse?.choices == null || !groqResponse.choices.Any())
                return "No response from AI.";
            return groqResponse.choices[0].message.content?.Trim() ?? "No response from AI.";
        }
    }

    public class GroqResponse
    {
        public List<GroqChoice> choices { get; set; } = new();
    }

    public class GroqChoice
    {
        public GroqMessage message { get; set; } = new();
    }

    public class GroqMessage
    {
        public string content { get; set; } = string.Empty;
    }
}