using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using Common;
using Common.Enums;
using Dal;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Models;
using Newtonsoft.Json;
using Entities;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Premieres([FromQuery] FilmFilterModel filters, [FromQuery] int page = 1)
        {
            const int pageSize = 10;

            var movies = _dbContext.Movies.Include(item => item.Countries);
            var total = await movies.CountAsync();
            if (movies != null)
            {
                IQueryable<Movie> result = movies;

                if (filters.SearchQuery != null)
                {
                    result = result.Where(item => 
                        item.NameRu != null && 
                        Regex.IsMatch(item.NameRu, Regex.Escape(filters.SearchQuery), RegexOptions.IgnoreCase));
                }
                if (filters.Countries != null)
                {
                    result = filters.Countries.Aggregate(result, (current, countryId) => 
                        current.Where(item => item.Countries != null && item.Countries.Any(a => a != null && a.Id == countryId)));
                }

                if (filters.Genres != null)
                {
                    var genresFilter = filters.Genres.Sum();
                    result = result.Where(item => (item.Genres & genresFilter) == genresFilter);
                }

                result = filters.Order switch
                {
                    Order.Name => result.OrderBy(item => item.NameRu),
                    Order.Duration => result.OrderBy(item => item.Duration),
                    _ => result.OrderBy(item => item.Id)
                };

                result = result.Skip((page - 1) * pageSize).Take(pageSize);

                if (result != null)
                {
                    var viewModel = new IndexViewModel<MovieModel>
                    {
                        PageViewModel = new PageViewModel(total, page, pageSize),
                        Items = MovieModel.ConvertToModels(result.ToList())
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
        public IActionResult Filters()
        {
            var countriesBytes = _cache.Get("countries");
            if (countriesBytes != null)
            {
                var countriesString = Encoding.UTF8.GetString(countriesBytes);
                var countries = JsonConvert.DeserializeObject<List<Country>>(countriesString)
                    ?.Select(item => new { item.Id, item.Name });
                var genres = Enum.GetValues(typeof(Genre)).Cast<Genre>();

                var aGenres = genres.Select(item => new { Id = (int)item, country = item.GetAttribute<DisplayAttribute>()?.Name });


                return Ok(new { genres = aGenres, countries });
            }

            return BadRequest();

        }
    }
}