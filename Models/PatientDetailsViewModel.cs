using System.ComponentModel.DataAnnotations;

public class PatientDetailsViewModel
{
    public string? PatientId { get; set; }

    [Display(Name = "Ad")]
    public string? FirstName { get; set; }

    [Display(Name = "Soyad")]
    public string? LastName { get; set; }

    [Display(Name = "Yaş")]
    public int? Age { get; set; }

    [Display(Name = "Cinsiyet")]
    public string? Gender { get; set; }

    [Display(Name = "Kan Grubu")]
    public string? BloodType { get; set; }

    [Display(Name = "Alerjiler")]
    public string? Allergies { get; set; }

    [Display(Name = "Kronik Hastalıklar")]
    public string? ChronicDiseases { get; set; }

    [Display(Name = "Tıbbi Geçmiş")]
    public string? MedicalHistory { get; set; }
}