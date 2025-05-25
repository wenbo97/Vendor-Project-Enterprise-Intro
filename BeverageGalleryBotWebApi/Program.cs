using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

string botToken = "";
var httpClient = new HttpClient();

// telegram Webhook
app.MapPost("/webhook", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();

    if (string.IsNullOrWhiteSpace(body))
        return Results.BadRequest();

    try
    {
        var update = JsonConvert.DeserializeObject<TelegramUpdate>(body);
        var message = update?.Message;

        if (message?.Text != null)
        {
            var chat = message.Chat;

            Console.WriteLine("📥 Received message：");
            Console.WriteLine($"🔹 Text: {message.Text}");
            Console.WriteLine($"🔹 From Chat ID: {chat.Id}");
            Console.WriteLine($"🔹 Chat Type: {chat.Type}");
            Console.WriteLine($"🔹 Chat Title/Name: {chat.Title ?? chat.Username ?? ""}");

            // /getid command
            if (message.Text.Trim().StartsWith("/getid"))
            {
                var responseText = $"Chat ID is：`{chat.Id}`\n Type：{chat.Type}";
                var payload = new
                {
                    chat_id = chat.Id,
                    text = responseText,
                    parse_mode = "Markdown"
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await httpClient.PostAsync(
                    $"https://api.telegram.org/bot{botToken}/sendMessage",
                    content);
            }
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed：" + ex.Message);
        return Results.StatusCode(500);
    }
});

app.Run();


public class TelegramUpdate
{
    [JsonProperty("message")]
    public TelegramMessage? Message { get; set; }
}

public class TelegramMessage
{
    [JsonProperty("message_id")]
    public long MessageId { get; set; }

    [JsonProperty("chat")]
    public TelegramChat Chat { get; set; } = new();

    [JsonProperty("text")]
    public string? Text { get; set; }
}

public class TelegramChat
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string? Title { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }
}
