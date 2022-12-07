using System.Text.RegularExpressions;
using BL.Abstract;
using Core.Entities.Concrete;
using Core.Entities.Enums;
using Core.Entities.Models;
using Core.Entities.Models.ViewModel;
using Dal.Concrete.Context;
using Microsoft.EntityFrameworkCore;

namespace BL.Concrete;

public class MoviePageNavigation
{
    private readonly AppDbContext _dbContext;

    public MoviePageNavigation(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<MovieViewModel> Pagination(MovieFilterViewModel filters)
    {
        IQueryable<Movie> movies = _dbContext.Movies.Include(item => item.Countries);

        if (!string.IsNullOrEmpty(filters.SearchQuery))
        {
            movies = movies.Where(movie =>
                movie.NameRu != null && movie.NameRu.ToLower().Contains(filters.SearchQuery));
        }

        var genresFilter = filters.Genres?.Sum();

        if (genresFilter != null)
        {
            movies = movies.Where(item => (item.Genres & genresFilter) == genresFilter);
        }

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
}