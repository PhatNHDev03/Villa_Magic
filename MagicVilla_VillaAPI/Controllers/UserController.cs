using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UserAuth")]
    [ApiVersionNeutral]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        public UserController(IUserRepository  userRepository)
        {
            _userRepository = userRepository;
            this._response = new APIResponse();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var login = await _userRepository.Login(model);
            if (login.User == null || String.IsNullOrEmpty(login.Token)) {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucess =false;
                _response.ErrorMessages.Add("username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode=HttpStatusCode.OK;
            _response.result = login;
            _response.IsSucess=true;
            return Ok(_response);
            
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool uniqueUserName = _userRepository.IsUniqueUser(model.UserName);
            if (!uniqueUserName) {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucess = false;
                _response.ErrorMessages.Add("username must be unique");
                return BadRequest(_response);
            }
            var registationResponse = await _userRepository.Register(model);
            if (registationResponse ==null) {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSucess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSucess=true;
            return Ok(_response);
        }

    }
}
