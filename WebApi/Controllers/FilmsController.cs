using System.ComponentModel.DataAnnotations;
using System.Text;
using Common;
using Common.Enums;
using Dal;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Models;
using Newtonsoft.Json;
using Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Extensions;
using WebApi.Models.ViewModel;

namespace WebApi.Controllers
{
    /// <summary>
    /// Контроллер
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]/[action]/")]
    public class FilmsController : ControllerBase
    {
        private readonly ILogger<FilmsController> _logger;

        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly SharedConfiguration _sharedConfiguration;

        private readonly AppDbContext _dbContext;

        private readonly IDistributedCache _cache;

        public FilmsController(ILogger<FilmsController> logger, SharedConfiguration sharedConfiguration, AppDbContext dbContext, IDistributedCache cache)
        {
            _logger = logger;
            _sharedConfiguration = sharedConfiguration;
            _dbContext = dbContext;
            _cache = cache;
        }

        /// <summary>
        /// получить список фильмов по различным фильтрам
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="page"></param>
        /// <returns>Возвращает список фильмов с пагинацией. Каждая страница содержит не более чем 20 фильмов. Данный эндпоинт не возращает более 400 фильмов. Используй /api/v1/films/filters чтобы получить id стран и жанров.</returns>
        [HttpGet(Name = "premieres")]
        public async Task<IActionResult> Premieres([FromQuery]FilmFilterModel filters, int page = 1)
        {
            var currentMonth = (Months)DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var requestUri = new StringBuilder();

            requestUri.Append("https://kinopoiskapiunofficial.tech/api/v2.2/films/premieres?");
            requestUri.Append($"year={currentYear}");
            requestUri.Append($"&month={currentMonth}");

            var request = HttpRequestMessage(requestUri);

            using var response = await _httpClient.SendAsync(request);

            const int pageSize = 10;
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResult = JsonConvert.DeserializeObject<ResponseMoviesModel>(content);

                if (jsonResult != null)
                {
                    var result = jsonResult.items
                        .Skip((page - 1 ) * pageSize).Take(pageSize).ToList();
                    var viewModel = new IndexViewModel<MovieModel>
                    {
                        PageViewModel = new PageViewModel(jsonResult.total, page, pageSize),
                        Items = result
                    };


                    return Ok(viewModel);
                }
            }

            return BadRequest();
        }

        private HttpRequestMessage HttpRequestMessage(StringBuilder requestUri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri.ToString());
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("X-API-KEY", _sharedConfiguration.ApiKey);
            return request;
        }

        /// <summary>
        /// получить список фильтров
        /// </summary>
        /// <returns>Возвращает фильтры для поиска в эндпоинте premieres</returns>
        [HttpGet(Name = "filters")]
        public async Task<IActionResult> Filters()
        {
            var countriesBytes = _cache.Get("countries");
            if (countriesBytes != null)
            {
                var countriesString = Encoding.UTF8.GetString(countriesBytes);
                var countries = JsonConvert.DeserializeObject<List<Country>>(countriesString);
                var genres = Enum.GetValues(typeof(Genre)).Cast<Genre>();

                var aGenres = genres.Select(item => new { Id = (int)item, country = item.GetAttribute<DisplayAttribute>()?.Name });


                return Ok(new { genres = aGenres, countries });
            }

            return BadRequest();

        }
    }
}