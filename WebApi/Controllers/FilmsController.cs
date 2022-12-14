using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using BL.Abstract;
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
        private readonly AppDbContext _dbContext;

        private readonly IFilterService _filterManager;

        private readonly IMovieService _movieService;

        public FilmsController(AppDbContext dbContext, IFilterService filterManager, IMovieService movieService)
        {
            _dbContext = dbContext;
            _filterManager = filterManager;
            _movieService = movieService;
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
            var result = await _movieService.GetPremieres(filters);

            return result != null
                ? new ResponseWrapper<MovieViewModel>(OperationStatus.Success) { ResponseData = result }
                : new ResponseWrapper<MovieViewModel>(OperationStatus.Failed);
        }

        /// <summary>
        /// получить список фильтров
        /// </summary>
        /// <returns>Возвращает фильтры для поиска в эндпоинте premieres</returns>
        [HttpGet(Name = "filters")]
        //[Authorize]
        public ResponseWrapper<FilterWrapper> Filters()
        {
            var result = _filterManager.GetAll();

            return result != null
                ? new ResponseWrapper<FilterWrapper>(OperationStatus.Success) { ResponseData = result }
                : new ResponseWrapper<FilterWrapper>(OperationStatus.Failed);
        }
    }
}