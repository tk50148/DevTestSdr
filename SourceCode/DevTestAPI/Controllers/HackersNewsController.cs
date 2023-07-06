using DevTestAPI.Contracts.Repositories;
using DevTestAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DevTestAPI.Controllers
{
    [ApiController]
    [Route("api/hacker-news")]
    public class HackersNewsController : ControllerBase
    {
        private readonly IHackerNewsRepository _hackerNewsRepository;

        private readonly ILogger<HackersNewsController> _logger;

        public HackersNewsController(
            ILogger<HackersNewsController> logger,
            IHackerNewsRepository hackerNewsRepository)
        {
            _logger = logger;
            _hackerNewsRepository = hackerNewsRepository;
        }

        [HttpGet(Name = "best-stories")]
        public async Task<IActionResult> GetBestHackerNewsAsync([FromQuery] int? topStoriesCount, CancellationToken cancellationToken)
        {
            try
            {
                if (topStoriesCount == null)
                {
                    return BadRequest("Missing value for parameter 'topStoriesCount'");
                }

                if (topStoriesCount <= 0)
                {
                    return BadRequest("Value of parameter 'topStoriesCount' has to be grater than 0");
                }

                var stories = await _hackerNewsRepository.GetBestStoriesAsync(topStoriesCount.Value, cancellationToken);
                var dtos = stories.Select(x => x.MapToStoryDto()).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing request for top {itemsCount} Best Stories", topStoriesCount);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}