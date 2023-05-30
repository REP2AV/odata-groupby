using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.ComponentModel.DataAnnotations;

namespace odata_groupby
{
    [ApiVersion(2.0)]
    public class WeatherForecastsController : ODataController
    {
        private readonly ILogger<WeatherForecastsController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public WeatherForecastsController(
            ILogger<WeatherForecastsController> logger,
            ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        [EnableQuery(PageSize = 1000000)]
        public IActionResult Get(
            ODataQueryOptions<WeatherForecast>? options = null,
            [FromQuery(Name = "pagesize"), Range(10, 100000)] int pagesize = 1000)
        {
            if (HttpContext != null)
            {
                if (!Request.Headers.ContainsKey("Prefer"))
                {
                    Request.Headers.Add("Prefer", $"odata.maxpagesize={pagesize}");
                    _logger.LogDebug("Using max page size {size}", pagesize);
                }
            }

            var result = _applicationDbContext.WeatherForecast.AsQueryable();

            return Ok(result);
        }
    }
}
