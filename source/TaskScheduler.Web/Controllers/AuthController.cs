using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using TaskScheduler.Model;
using TaskScheduler.Application;
using TaskScheduler.Domain;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TaskScheduler.Web
{
    [Route(CONTROLLER_ROUTE)]
    public class AuthController:Controller
    {
        public const string CONTROLLER_ROUTE = "auth";

        public const string SIGN_IN_ROUTE = "sign-in";
        public const string SIGN_OUT_ROUTE = "sign-out";
        public const string REGISTER_ROUTE = "register";
        public const string IS_LOGIN_EXIST_ROUTE = "is-login-exists";

        readonly ILoginAuthService _loginAuthService;
        readonly ILogger<AuthController> _logger;
        public AuthController(ILoginAuthService loginAuthService, ILogger<AuthController> logger)
        {
            _loginAuthService = loginAuthService;
            _logger = logger;
        }

        [HttpGet]
        [Route(SIGN_IN_ROUTE)]
        public async Task<IActionResult> SignInAsync()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync(WebCommon.CookieName);
            
            return View(new LoginAuthSignInModel());
        }

        [HttpPost]
        [Route(SIGN_IN_ROUTE)]
        public async Task<IActionResult> SignInAsync(LoginAuthSignInModel signInModel)
        {
            var authResult = await _loginAuthService.SingInAsync(signInModel);

            var badStatusCodeHandle = this.HandleBadStatusCode(authResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;
            
            if (authResult.IsFail)
            {
                if(authResult.FailCode != ResultFailCode.BadLoginOrPassword)
                    return this.GetUnexpectedFailResult(_logger, authResult);

                TempData[WebCommon.WarningMessageName] = authResult.FailCodeMessages.GetValueOrDefault(ResultFailCode.BadLoginOrPassword);
                return View(signInModel);
            }

            string login = authResult.SuccessResult.Login;

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, WebCommon.CookieName));

            await HttpContext.SignInAsync(WebCommon.CookieName, claimsPrincipal);

            TempData[WebCommon.SuccessMessageName] = new string[] { "You successfully signed in." };

            return RedirectToAction(HomeController.INDEX_ROUTE, HomeController.CONTROLLER_ROUTE);
        }

        [Route(SIGN_OUT_ROUTE)]
        public async Task<IActionResult> SignOutAsync()
        {
            if(HttpContext.User.Identity.IsAuthenticated)
                await HttpContext.SignOutAsync(WebCommon.CookieName);

            TempData[WebCommon.SuccessMessageName] = new string[] { "You successfully signed out." };

            return RedirectToAction(HomeController.INDEX_ROUTE, HomeController.CONTROLLER_ROUTE);
        }

        [HttpGet]
        [Route(REGISTER_ROUTE)]
        public IActionResult Register() => View(new LoginAuthRegisterModel());

        [HttpPost]
        [Route(REGISTER_ROUTE)]
        public async Task<IActionResult> RegisterAsync(LoginAuthRegisterModel registerModel)
        {
            var authResult = await _loginAuthService.RegisterAsync(registerModel);

            var badStatusCodeHandle = this.HandleBadStatusCode(authResult);
            if (badStatusCodeHandle is not null)
                return badStatusCodeHandle;

            if (authResult.IsFail)
            {
                ViewData[WebCommon.ErrorBag] = authResult.FailCodeMessages;
                return View(registerModel);
            }


            TempData[WebCommon.SuccessMessageName] = new string[] { "Registration was successful. You can sign in now."};

            return RedirectToAction(AuthController.SIGN_IN_ROUTE, AuthController.CONTROLLER_ROUTE);
        }

        [HttpPost]
        [Route(IS_LOGIN_EXIST_ROUTE)]
        public async Task<IActionResult> IsLoginExistsAsync(string login)
        {
            var loginExistsResult = await _loginAuthService.IsLoginExistsAsync(new LoginModel(login));

            return Json(!loginExistsResult.SuccessResult);
        }
    }
}
