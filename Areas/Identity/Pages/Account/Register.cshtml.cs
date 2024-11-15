// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using HastaneTakipsistemi.Models;

namespace HastaneTakipsistemi.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Lütfen e-posta adresinizi giriniz.")]
            [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Lütfen adınızı giriniz.")]
            [Display(Name = "Ad")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Lütfen soyadınızı giriniz.")]
            [Display(Name = "Soyad")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
            [StringLength(50, ErrorMessage = "Şifreniz en az 6 karakterden oluşmalı", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Şifre Tekrar")]
            [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Lütfen rolünüzü seçiniz")]
            [Display(Name = "Rol")]
            public string Role { get; set; }

            // Hastaya özel alanlar
            [Required(ErrorMessage = "Lütfen yaşınızı giriniz.")]
            [Display(Name = "Yaş")]
            [Range(0, 120, ErrorMessage = "Lütfen geçerli bir yaş giriniz.")]
            public int? Age { get; set; }
            [Required(ErrorMessage = "Lütfen cinsiyetinizi seçiniz.")]
            [Display(Name = "Cinsiyet")]
            public string Gender { get; set; }

            [Display(Name = "Sağlık Geçmişi")]
            public string MedicalHistory { get; set; }
            [Display(Name = "Kan Grubu")]
            public string BloodType { get; set; }

            [Display(Name = "Alerjiler")]
            public string Allergies { get; set; }

            [Display(Name = "Kronik Hastalıklar")]
            public string ChronicDiseases { get; set; }

            //Doktora özel alan
            [Required(ErrorMessage = "Lütfen uzmanlık alanınızı seçiniz.")]
            [Display(Name = "Uzmanlık Alanı")]
            public string Specialization { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName

                };

                if (Input.Role == "Patient")
                {
                    user.Age = Input.Age;
                    user.Gender = Input.Gender;
                    user.MedicalHistory = Input.MedicalHistory;
                    user.BloodType = Input.BloodType;
                    user.Allergies = Input.Allergies;
                    user.ChronicDiseases = Input.ChronicDiseases;
                }
                else if (Input.Role == "Doctor")
                {
                    user.Specialization = Input.Specialization;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                    _logger.LogInformation("User created a new account with password.");

                    // Rol ataması
                    if (await _roleManager.RoleExistsAsync(Input.Role))
                    {
                        await _userManager.AddToRoleAsync(user, Input.Role);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}