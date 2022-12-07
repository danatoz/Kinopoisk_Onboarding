using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Enums;

public enum Order
{
    /// <summary>
    /// Duration
    /// </summary>
    [Display(Name = "Duration")]
    Duration,
    /// <summary>
    /// Name
    /// </summary>
    [Display(Name = "Name")]
    Name,
}