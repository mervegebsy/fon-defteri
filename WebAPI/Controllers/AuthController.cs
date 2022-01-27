using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.DTOs;

namespace WebAPI.Controllers
{
    //Kullanıcı giriş işlemleri için yetkilendirme işlemlerinde token üretimi ve login işlemlerini kontrol eden sınıftır.
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        //kullanıcı girişi için kullanılan endpoint
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }
            //authentication için jwt tokeni yaratılan metoda çağrı yapılır
            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }

        [HttpPost("register")]
        //kullanıcı yeni kayıt işlemleri için kullanılır
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            //kullanıcının veritabanında olup olmadığı kontrol edilir
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)
            {
                //kullanıcının bulunduğu durumda zaten var mesajı dönülür
                return BadRequest(userExists.Message);
            }
            //kullanıcı kayıt işlemi gerçekleştirilir
            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                //başarılı işlem mesajı dönülür
                return Ok(result.Data);
            }

            return BadRequest(result.Message);
        }
    }
}
