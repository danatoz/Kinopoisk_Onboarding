using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using BL.Concrete;
using BL.Constants;
using Core.Configurations;
using Core.Entities.Concrete;
using Core.Entities.Enums;
using Core.Entities.Models;
using Core.Entities.Models.ViewModel;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Dal.Concrete.Context;
using Microsoft.AspNetCore.Http.Features;
using Core.Utilities.Results;

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

        private readonly AppDbContext _dbContext;

        private readonly IDistributedCache _cache;

        public FilmsController(ILogger<FilmsController> logger, AppDbContext dbContext, IDistributedCache cache)
        {
            _logger = logger;
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
        //[Authorize]
        public async Task<ResponseWrapper<MovieViewModel>> Premieres([FromQuery] MovieFilterViewModel filters)
        {
            var result = await new MoviePageNavigation(_dbContext).Pagination(filters);

            return new ResponseWrapper<MovieViewModel>(OperationStatus.Success)
            {
                ResponseData = result
            };
        }

        /// <summary>
        /// получить список фильтров
        /// </summary>
        /// <returns>Возвращает фильтры для поиска в эндпоинте premieres</returns>
        [HttpGet(Name = "filters")]
        //[Authorize]
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