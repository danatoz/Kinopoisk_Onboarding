using System.Text;
using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.Models;
using Newtonsoft;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    /// <summary>
    /// Контроллер
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FilmsController : ControllerBase
    {
        private readonly ILogger<FilmsController> _logger;
        private static HttpClient _httpClient = new HttpClient();
        public FilmsController(ILogger<FilmsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// получить список фильмов по различным фильтрам
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Возвращает список фильмов с пагинацией. Каждая страница содержит не более чем 20 фильмов. Данный эндпоинт не возращает более 400 фильмов. Используй /api/v1/films/filters чтобы получить id стран и жанров.</returns>
        [HttpGet(Name = "premieres")]
        public async Task<IActionResult> Premieres([FromQuery]FilmFilterModel filters)
        {
            var currentMonth = (Months)DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var requestUri = new StringBuilder();

            requestUri.Append("https://kinopoiskapiunofficial.tech/api/v2.2/films/premieres?");
            requestUri.Append($"year={currentYear}");
            requestUri.Append($"&month={currentMonth}");

            var request = HttpRequestMessage(requestUri);

            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResult = JsonConvert.DeserializeObject<ResponseMoviesModel>(content);
                return Ok(jsonResult);
            }

            return BadRequest();
        }

        private static HttpRequestMessage HttpRequestMessage(StringBuilder requestUri)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri.ToString());
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("X-API-KEY", "cfa51d0b-0a1e-4ee3-9999-c471fa878b52");
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[HttpGet(Name = "filters")]
        //public async Task<IActionResult> Filters()
        //{
        //    return Ok();
        //}
    }
}