using Core.Entities.Enums;

namespace BL.Constants;

public class UriConstant
{
    public readonly string PremieresUri;

    public readonly string FiltersUri;

    public UriConstant(string filtersUri, string premieresUriWithoutParams)
    {
        FiltersUri = filtersUri;
        PremieresUri = $"{premieresUriWithoutParams}?year={DateTime.Now.Year}&month={(Months)DateTime.Now.Month}";
    }
}