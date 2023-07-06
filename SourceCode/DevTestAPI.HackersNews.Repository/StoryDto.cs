using System.Text.Json.Serialization;

namespace DevTestAPI.HackerNews.Repository
{
    public class HackerNewsItemDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("url")]
        public string Uri { get; set; }
        [JsonPropertyName("by")]
        public string PostedBy { get; set; }
        [JsonPropertyName("time")]
        public long Time { get; set; }
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("Descendants")]
        public int CommentCount { get; set; }
    }
}