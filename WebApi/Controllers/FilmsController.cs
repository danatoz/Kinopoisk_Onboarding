using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using BL.Constants;
using Core.Configurations;
using Core.Entities.Concrete;
using Core.Entities.Enums;
using Core.Entities.Models;
using Core.Entities.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Dal.Concrete.Context;

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
        [Authorize]
        public async Task<IActionResult> Premieres([FromQuery] FilmFilterModel filters)
        {
            var movies = _dbContext.Movies.Include(item => item.Countries);
            var total = await movies.CountAsync();

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

            result = result.Skip(filters.Page * filters.PageSize).Take(filters.PageSize);


            var viewModel = new IndexViewModel<MovieModel>
            {
                PageViewModel = new PageViewModel(total, filters.Page, filters.PageSize),
                Items = MovieModel.ConvertToModels(result.ToList())
            };

            return Ok(viewModel);
        }

        /// <summary>
        /// получить список фильтров
        /// </summary>
        /// <returns>Возвращает фильтры для поиска в эндпоинте premieres</returns>
        [HttpGet(Name = "filters")]
        [Authorize]
        public IActionResult Filters()
        {
            var countriesBytes = _cache.Get(RedisKeyConstant.Countries);
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