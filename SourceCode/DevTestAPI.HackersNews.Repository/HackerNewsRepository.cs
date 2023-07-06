using DevTestAPI.Contracts.Repositories;
using DevTestAPI.Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace DevTestAPI.HackerNews.Repository
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<IHackerNewsRepository> _logger;
        private const string _bestStoriesCacheKey = "BestStories";
        private const string _hackerStoryItemUrl = @"https://hacker-news.firebaseio.com/v0/item/";
        private const string _bestHackerStoriesIdsUrl = @"https://hacker-news.firebaseio.com/v0/beststories.json";

        public HackerNewsRepository(
            HttpClient httpClient,
            IMemoryCache memoryCache,
            ILogger<IHackerNewsRepository> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<HackerNewsItem>> GetBestStoriesAsync(int numberOfStories, CancellationToken cancellationToken)
        {
            try
            {
                var stories = await GetAllBestStoriesAsync(cancellationToken);
                return stories.Take(numberOfStories).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading best HackerNews Stories");
                throw;
            }
        }

        private async Task<IEnumerable<HackerNewsItem>> GetAllBestStoriesAsync(CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<HackerNewsItem> stories;
                if (!_memoryCache.TryGetValue(_bestStoriesCacheKey, out stories))
                {
                    _logger.LogInformation("No data in Cache");
                    var storiesList = await FetchHackerNewsFromExternalSource(cancellationToken);
                    stories = storiesList.OrderByDescending(x => x.Score).ToList();
                    _memoryCache.Set(_bestStoriesCacheKey, stories, GetCacheExpirationDateTimeOffset());
                }

                return stories;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<HackerNewsItem>> FetchHackerNewsFromExternalSource(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching HackerNews Stories");
            var storiesIds = await _httpClient.GetFromJsonAsync<IEnumerable<int>>(_bestHackerStoriesIdsUrl, cancellationToken);
            var storiesList = new List<HackerNewsItem>();
            foreach (var id in storiesIds)
            {
                var item = await TryGetHackerNewsItemAsync(id, cancellationToken);
                if (item != null)
                {
                    storiesList.Add(item);
                }
            }
            _logger.LogInformation("HackerNews Stories fetched");
            return storiesList;
        }

        private DateTimeOffset GetDateTimeOffset(long epochTimestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(epochTimestamp);
        }

        private async Task<HackerNewsItem> TryGetHackerNewsItemAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var dto = await _httpClient.GetFromJsonAsync<HackerNewsItemDto>($"{_hackerStoryItemUrl}{id}.json", cancellationToken);
                return new HackerNewsItem
                {
                    CommentCount = dto.CommentCount,
                    PostedBy = dto.PostedBy,
                    Score = dto.Score,
                    Time = GetDateTimeOffset(dto.Time),
                    Title = dto.Title,
                    Uri = dto.Uri
                };
            }
            catch (Exception)
            {
                _logger.LogError("HackerNews item with id {itemId} does not exist", id);
                return null;
            }
        }

        private DateTimeOffset GetCacheExpirationDateTimeOffset()
        {
            var now = DateTime.UtcNow;
            var expirationDateTime = new DateTimeOffset(now.Year, now.Month, now.Day, 6,0,0, TimeSpan.Zero);
            
            if(now.Hour>6)
            {
                expirationDateTime = expirationDateTime.AddDays(1);
            }

            return expirationDateTime;
        }
    }
}