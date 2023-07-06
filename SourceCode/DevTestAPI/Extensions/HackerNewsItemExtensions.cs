using DevTestAPI.DTOs;
using DevTestAPI.Entities.Models;

namespace DevTestAPI.Extensions
{
    public static class HackerNewsItemExtensions
    {
        public static HackerNewsStoryDto MapToStoryDto(this HackerNewsItem item)
        {
            return new HackerNewsStoryDto
            {
                CommentCount = item.CommentCount,
                PostedBy = item.PostedBy,
                Score = item.Score,
                Time = item.Time,
                Title = item.Title,
                Uri = item.Uri
            };
        }
    }
}
