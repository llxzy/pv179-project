using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLayer.DataTransferObjects;
using BusinessLayer.Facades.FacadeInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Application.Models.UserModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Application.Controllers
{
    public class UserController : Controller
    {
        private IUserFacade _userFacade;

        private IMapper mapper = new Mapper(new MapperConfiguration(ApplicationMappingConfig.ConfigureMap));
        //public UserDto user;

        public UserController(IUserFacade facade)
        {
            _userFacade = facade;
        }
        // GET
        public IActionResult Index()
        { 
            return View();
        }

        public IActionResult FindUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FindUser2(string passedMail)
        {
           // var user = _userFacade.GetUserByMail(passedMail);
            /*
            user.Name = "Petko Hlineny";
            _userFacade.Update(user);
            var changeduser = _userFacade.GetUserByMail(passedMail);*/
            //return View(user);
            var x = _userFacade.DeleteLoggedUser(int.Parse(passedMail));
            return new ContentResult() { Content = "IT WORKED YAY"};
        }


        /*
         * defaultne je to HttpGet, zavola View = User.Create
         * obsahuje submit button, ktory ked stlacis tak zavola HttpPost Create (metoda pod touto)
         */
        [HttpGet("Register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserCreateModel userCreateModel)
        {
            var user = new UserRegistrationDto
            {
                Name = userCreateModel.Name,
                MailAddress = userCreateModel.MailAddress,
                Password = userCreateModel.Password
            };
            try
            {
                await _userFacade.RegisterNewUser(user);
            }
            catch (Exception)
            {
                ModelState.AddModelError("User", "Already exists");
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "User");
            //return new ContentResult() { Content = user.Name + user.MailAddress };
        }
        

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(UserLoginModel userModel)
        {
            var user = _userFacade.GetUserByMail(userModel.MailAddress);
            try
            {
                //var password = BusinessLayer.Utils.HashingUtils.Encode(userModel.Password);
                _userFacade.VerifyUserLogin(userModel.MailAddress, userModel.Password);
                await CreateClaimsAndSignInAsync(user);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("Mail", "Invalid credentials");
                return View("Login");
            }
        }
        
        private async Task CreateClaimsAndSignInAsync(UserDto user)
        {
            var claims = new List<Claim>
            {
                //Set User Identity Name to actual user Id - easier access with user connected operations
                new Claim(ClaimTypes.Name, user.Id.ToString())
            };
            claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            // TODO USERS ID IS STORED IN User.Identity.Name;
            var id = User.Identity?.Name;
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = _userFacade.GetAsync(int.Parse(id)).Result;
            return View(mapper.Map<UserModel>(user));
        }
        
    }
}