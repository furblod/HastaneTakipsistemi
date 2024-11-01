using Microsoft.AspNetCore.Mvc.Rendering;

public class EditUserViewModel
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? MedicalHistory { get; set; }
    public string? BloodType { get; set; }
    public string? Allergies { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? Specialization { get; set; }
    public List<string>? Roles { get; set; }
    public List<string>? AllRoles { get; set; }

    // Dropdown listeleri için
    public static List<SelectListItem> GenderOptions => new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seçiniz" },
        new SelectListItem { Value = "Erkek", Text = "Erkek" },
        new SelectListItem { Value = "Kadın", Text = "Kadın" }
    };

    public static List<SelectListItem> BloodTypeOptions => new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seçiniz" },
        new SelectListItem { Value = "A+", Text = "A+" },
        new SelectListItem { Value = "A-", Text = "A-" },
        new SelectListItem { Value = "B+", Text = "B+" },
        new SelectListItem { Value = "B-", Text = "B-" },
        new SelectListItem { Value = "AB+", Text = "AB+" },
        new SelectListItem { Value = "AB-", Text = "AB-" },
        new SelectListItem { Value = "0+", Text = "0+" },
        new SelectListItem { Value = "0-", Text = "0-" }
    };

    public static List<SelectListItem> SpecializationOptions => new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "Seçiniz" },
        new SelectListItem { Value = "Dahiliye", Text = "Dahiliye" },
        new SelectListItem { Value = "Kardiyoloji", Text = "Kardiyoloji" },
        new SelectListItem { Value = "Nöroloji", Text = "Nöroloji" },
        new SelectListItem { Value = "Ortopedi", Text = "Ortopedi" },
        new SelectListItem { Value = "Pediatri", Text = "Pediatri" },
        new SelectListItem { Value = "Psikiyatri", Text = "Psikiyatri" }
    };
}