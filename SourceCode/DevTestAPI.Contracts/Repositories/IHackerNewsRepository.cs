using DevTestAPI.Entities.Models;

namespace DevTestAPI.Contracts.Repositories
{
    public interface IHackerNewsRepository
    {
        Task<IReadOnlyCollection<HackerNewsItem>> GetBestStoriesAsync(int storiesId, CancellationToken cancellationToken);
    }
}
