using BL.Abstract;
using BL.Constants;
using Core.Configurations;
using Core.Entities.Enums;
using Core.Entities.Models;
using Core.Entities.Models.ViewModel;
using Dal.Concrete.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Core.Entities.Concrete;


namespace BL.Concrete;

public class MovieManager : IMovieService
{
    private readonly AppDbContext _dbContext;

    private readonly ILogger<MovieManager> _logger;

    private readonly SharedConfiguration _sharedConfiguration;

    private readonly UriConstant _uriConstant;

    private int _count = 0;

    private readonly IServiceProvider _serviceProvider;

    public MovieManager(AppDbContext dbContext, ILogger<MovieManager> logger, SharedConfiguration sharedConfiguration, UriConstant uriConstant, IServiceProvider serviceProvider)
    {
        _dbContext = dbContext;
        _logger = logger;
        _sharedConfiguration = sharedConfiguration;
        _uriConstant = uriConstant;
        _serviceProvider = serviceProvider;
    }

    public async Task<MovieViewModel> GetPremieres(MovieFilterViewModel filters)
    {
        IQueryable<Movie> movies = _dbContext.Movies;

        var genresFilter = filters.Genres?.Sum();

        movies = movies.Where(movie => EF.Functions.ILike(movie.NameRu, $"{filters.SearchQuery}%"))
            .Where(movie => (movie.Genres & genresFilter) == genresFilter);

        if (filters.Countries != null)
        {
            movies = filters.Countries.Aggregate(movies, (current, countryId) =>
                current.Where(movie => movie.Countries.Any(item => item.Id == countryId)));
        }

        movies = filters.SortOrder switch
        {
            MovieSortState.NameAsc => movies.OrderBy(item => item.NameRu),
            MovieSortState.NameDesc => movies.OrderByDescending(item => item.NameRu),
            MovieSortState.DurationAsc => movies.OrderBy(item => item.Duration),
            MovieSortState.DurationDesc => movies.OrderByDescending(item => item.Duration),
            _ => movies.OrderBy(item => item.Id)
        };

        var items = await movies.Skip(filters.Page * filters.PageSize).Take(filters.PageSize).ToListAsync();

        return new MovieViewModel
        {
            Total = await movies.CountAsync(),
            Items = MovieModel.ConvertToModels(items)
        };
    }

    public async Task<int> Download(CancellationToken cancel)
    {
        try
        {
            var apiService = _serviceProvider.GetService(typeof(IApiService<ResponseMoviesModel>)) as IApiService<ResponseMoviesModel>;
            var wrapperMoviesModel = await apiService.Request(new HttpRequestConfiguration
            {
                Uri = _uriConstant.PremieresUri,
                Method = HttpMethod.Get,
                Headers = new Dictionary<string, string>() { { "Accept", "application/json" }, { "X-API-KEY", _sharedConfiguration.ApiKey } }
            });

            if (wrapperMoviesModel.StatusCode != HttpStatusCode.OK)
            {
                _count++;
                if (_count == 5)
                    return 0;
                await Task.Delay(new TimeSpan(0, 0, 1), cancel);
                return await Download(cancel);
            }

            var movies = MovieModel.ConvertToEntities(wrapperMoviesModel.Data.items?.ToList());
            foreach (var movie in movies)
            {
                var tmpCountries = movie?.Countries;
                movie.Countries = new List<Country>();
                foreach (var country in tmpCountries)
                {
                    var entity = await _dbContext.Countries.FirstOrDefaultAsync(item => item.Name == country.Name, cancellationToken: cancel);

                    if (entity != null)
                    {
                        movie.Countries.Add(entity);
                    }
                }
            }
            await _dbContext.Movies.AddRangeAsync(movies!, cancel);
            await _dbContext.SaveChangesAsync(cancel);

            return movies.Count;
        }
        catch (Exception e)
        {
            _logger.LogError($"Api is not responding");
            _logger.LogError($"{e.Message}");
        }

        return 0;
    }
}