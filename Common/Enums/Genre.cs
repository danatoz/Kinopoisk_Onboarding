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
    Crime = 4,

    [Display(Name="���������")]
    Melodrama = 8,

    [Display(Name="��������")]
    Detective = 16,

    [Display(Name="����������")]
    Fiction = 32,

    [Display(Name="�����������")]
    Adventure = 64,

    [Display(Name="���������")]
    Biography = 128,

    [Display(Name="�����-����")]
    Filmnoir = 256,

    [Display(Name="�������")]
    Western = 512,

    [Display(Name = "������")]
    Action = 1024,

    [Display(Name = "�������")]
    Fantasy = 2048,

    [Display(Name = "�������")]
    Comedy = 4096,
}