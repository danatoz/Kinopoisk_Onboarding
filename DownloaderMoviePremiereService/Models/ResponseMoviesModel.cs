namespace DownloaderMoviePremiereService.Models;

class ResponseMoviesModel
{
    public int total { get; set; }

    public MovieModel[]? items { get; set; }
}