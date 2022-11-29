using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

[Flags]
public enum Genre
{
    [Display(Name="�������")]
    Thriller = 1,

    [Display(Name="�����")]
    Drama = 2,

    [Display(Name="��������")]
    Crime = 3,

    [Display(Name="���������")]
    Melodrama = 4,

    [Display(Name="��������")]
    Detective = 5,

    [Display(Name="����������")]
    Fiction = 6,

    [Display(Name="�����������")]
    Adventure = 7,

    [Display(Name="���������")]
    Biography = 8,

    [Display(Name="�����-����")]
    Filmnoir = 9,

    [Display(Name="�������")]
    Western = 10,
}