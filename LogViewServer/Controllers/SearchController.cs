using LogSearch;
using Microsoft.AspNetCore.Mvc;

namespace LogView.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        [HttpGet("GetSearchResults")]
        public async Task<IActionResult> GetSearchResults(string query)
        {
            var searcher = new LogSearcher();
            var logs = searcher.GetAllMatches(query);
            return Ok(logs);
        }

        [HttpGet("ViewLogs")]
        public async Task<IActionResult> ViewLogs(string log)
        {
            var searcher = new LogSearcher();
            var logs = searcher.GetLogFileView(log);
            return Ok(logs);
        }
    }
}
