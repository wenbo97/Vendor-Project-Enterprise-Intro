using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BeverageGalleryBotWebApi.Model;

public class BeverageQueryParam
{
    [JsonProperty("condition")]
    public Dictionary<string, string> Conditions { get; set; } = new();
    [JsonProperty("skip")]
    public int Skip { get; set; } = 0;
    [JsonProperty("limit")]
    public int Limit { get; set; } = 10;
}