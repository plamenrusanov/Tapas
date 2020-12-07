namespace Tapas.Web.Areas.Identity.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using Tapas.Common;
    using Tapas.Data.Models;
    using Tapas.Services.Messaging;
    using Tapas.Web.ViewModels;

    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (this.ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = this.Input.UserName, Email = this.Input.Email, PhoneNumber = this.Input.Phone };
                var result = await this.userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    this.logger.LogInformation($"{user.UserName} създаде нов акаунт с парола.");

                    var resultRole = await this.userManager.AddToRoleAsync(user, "User");
                    if (resultRole.Succeeded)
                    {
                        this.logger.LogInformation($"{user.UserName} seed to role User.");
                    }
                    else
                    {
                        this.logger.LogInformation($"{user.UserName} can't seed to role User.");
                    }

                    this.Response.Cookies.Append(GlobalConstants.UserIdCookieKey, user.Id, new CookieOptions()
                    {
                        Domain = GlobalConstants.Domain,
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddYears(2),
                        Path = GlobalConstants.IndexRoute,
                    });
                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        GlobalConstants.TapasEmail,
                        GlobalConstants.TapasEmailSender,
                        this.Input.Email,
                        "Потвърдете имейла си",
                        $"Моля, потвърдете акаунта си, като <a href = '{HtmlEncoder.Default.Encode(callbackUrl)}'> кликнете тук </a>.");
                    if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return this.RedirectToPage("RegisterConfirmation", new { email = this.Input.Email });
                    }
                    else
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }

        public class InputModel
        {
            [RequiredBg]
            [StringLength(20, ErrorMessage = "Името трябва да дълго между 3 и 20 символа!", MinimumLength = 3)]
            [Display(Name = "Име")]
            public string UserName { get; set; }

            [RequiredBg]
            [Phone]
            [Display(Name = "Телефон")]
            public string Phone { get; set; }

            [RequiredBg]
            [EmailAddress]
            [Display(Name = "Имейл")]
            public string Email { get; set; }

            [RequiredBg]
            [StringLength(100, ErrorMessage = "{0} трябва да бъде най-малко {2} и максимум {1} символа.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Потвърди паролата")]
            [Compare("Password", ErrorMessage = "Паролата и паролата за потвърждение не съвпадат.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
