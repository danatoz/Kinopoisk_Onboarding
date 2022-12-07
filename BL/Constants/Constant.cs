using Core.Entities.Enums;

namespace BL.Constants;

public static class Constant
{
    public static string PremieresUri =
        $"https://kinopoiskapiunofficial.tech/api/v2.2/films/premieres?year={DateTime.Now.Year}&month={(Months)DateTime.Now.Month}";

    public static string FiltersUri = "https://kinopoiskapiunofficial.tech/api/v2.2/films/filters";
}