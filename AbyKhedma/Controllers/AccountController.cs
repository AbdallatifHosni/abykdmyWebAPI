using AbyKhedma.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using AbyKhedma.Services;
using System.Security.Claims;
using System.Text;
using AbyKhedma.Entities;
using Core.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Core.Common;
using AbyKhedma.Core.Models;
using AbyKhedma.Core.Common;
using Newtonsoft.Json.Linq;
using AbyKhedma.Pagination;
using Core.Models;
using System;
using AbyKhedma.Interfaces;
using AbyKhedma.SignalRHubs;
using Microsoft.AspNetCore.SignalR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Numerics;
using System.Collections.Generic;
using Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace AbyKhedma.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly AccountService _accountService;
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IUriService _uriService;
        private readonly IHubContext<NotificationUserHub> _notificationUserHubContext;
        private readonly ISMSService smsService;
        private readonly IWhatsAppService whatsApp;

        public AccountController(AccountService accountService, IUserConnectionManager userConnectionManager,
            IMapper mapper, IConfiguration configuration, ILogger<AccountController> logger, IUriService uriService,
            IHubContext<NotificationUserHub> notificationUserHubContext,
            ISMSService smsService,
            IWhatsAppService whatsApp)
        {
            _accountService = accountService;
            this._userConnectionManager = userConnectionManager;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _uriService = uriService;
            _notificationUserHubContext = notificationUserHubContext;
            this.smsService = smsService;
            this.whatsApp = whatsApp;
        }
        //[HttpPost]
        //public IActionResult Refresh(string token, string refreshToken)
        //{
        //    var principal = GetPrincipalFromExpiredToken(token);
        //    var username = principal.Identity.Name;
        //    var savedRefreshToken = GetRefreshToken(username); //retrieve the refresh token from a data store
        //    if (savedRefreshToken != refreshToken)
        //        throw new SecurityTokenException("Invalid refresh token");

        //    var newJwtToken = GenerateToken(principal.Claims);
        //    var newRefreshToken = GenerateRefreshToken();
        //    DeleteRefreshToken(username, refreshToken);
        //    SaveRefreshToken(username, newRefreshToken);

        //    return new ObjectResult(new
        //    {
        //        token = newJwtToken,
        //        refreshToken = newRefreshToken
        //    });
        //}

        [HttpPost("SendToSpecificUser")]
        public async Task<ActionResult> SendToSpecificUser()
        {
            var identityClaim = User.Claims.FirstOrDefault(el => el.Type == "identityId");//.FindFirst(ClaimTypes.Role).Value;
            if (identityClaim != null)
            {
                var identityId = identityClaim.Value.ToString();

                // var clients = _notificationUserHubContext. < Type of your hub here> ();

            }
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserModel>> Login(RequesterUserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
               kvp => kvp.Key,
               kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
               );

                string[] strError = new string[errorList.Keys.Count()];
                int i = 0;
                foreach (var keyValuePair in errorList)
                {
                    strError[i] = keyValuePair.Key + ": " + keyValuePair.Value + Environment.NewLine;
                    i++;
                }
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Credentials", Errors = strError });
            }
            var user = await _accountService.Login(loginDto);
            if (user == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Credentials", Errors = new string[] { } });
            }
            user.DeviceToken = loginDto.DeviceToken;
            _accountService.UpdateLastLogin(user);//update last login time
            var claims = new[]
            {
               new Claim( "identityId",user.Id.ToString()),
               new Claim( "firstName",user.FirstName),
               new Claim( "fullName",user.FullName),
               new Claim( "email",user.Email??""),
               new Claim( "phone",user.PhoneNumber??""),
               new Claim( "lastLoginTime",user.LastLoginTime.ToString()??""),
               new Claim( "photoUrl",user.PhotoUrl ?? ""),
               new Claim( "_role",user.Role)
           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:JWTTokenSecret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userModel = _mapper.Map<UserDto>(user);
            return Ok(new { Succeeded = true, Data = new { Token = tokenHandler.WriteToken(token), user = userModel }, Message = string.Empty, Errors = new string[] { } });

        }

        //public string GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        return Convert.ToBase64String(randomNumber);
        //    }
        //}
        //private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        //{
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
        //        ValidateIssuer = false,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:JWTTokenSecret").Value)),
        //        ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    SecurityToken securityToken;
        //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        //    var jwtSecurityToken = securityToken as JwtSecurityToken;
        //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        throw new SecurityTokenException("Invalid token");

        //    return principal;
        //}

        [HttpPost("weblogin")]
        public async Task<ActionResult<UserModel>> WebLogin(EmployeeUserLoginDto loginDto)
        {
            var user = await _accountService.WebLogin(loginDto);
            if (user == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Invalid Credentials", Errors = new string[] { } });
            }
            _accountService.UpdateLastLogin(user);//update last login time
            var claims = new[]
            {
                new Claim( "identityId",user.Id.ToString()),
               new Claim( "firstName",user.FirstName),
               new Claim( "fullName",user.FullName),
               new Claim( "email",user.Email??""),
               new Claim( "phone",user.PhoneNumber??""),
               new Claim( "lastLoginTime",user.LastLoginTime.ToString()??""),
               new Claim( "photoUrl",user.PhotoUrl ?? ""),
               new Claim( "_role",user.Role)
           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:JWTTokenSecret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var userdto = _mapper.Map<UserDto>(user);
            return Ok(new { Succeeded = true, Data = new { Token = tokenHandler.WriteToken(token), user = userdto }, Message = string.Empty, Errors = new string[] { } });

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RequesterUserForRegisterDto userObj)
        {
            //if (User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized();
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (_accountService.UserExistByPhone(userObj.PhoneNumber))
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "This Phone Number is already existed, you could use forgot passowrd", Errors = new string[] { } });
            }
            userObj.Role = Constants.RequesterRole;
            var user = _mapper.Map<RequesterUserForRegisterDto, User>(userObj);
            var dbUser = await _accountService.Register(user, userObj.Password);
            await smsService.SendAsync($"رمز التحقق: {dbUser.PassCode}", user.PhoneNumber);
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("empRegister")]
        public async Task<IActionResult> EmployeeRegister(EmployeeUserForRegisterDto userObj)
        {
            //if (User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized();
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (_accountService.UserExistByEmail(userObj.Email))
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "This Email is already existed, you could use forgot passowrd", Errors = new string[] { } });
            }
            userObj.Role = Constants.EmployeeRole;
            if (userObj.AutogeneratePassword == 1)
            {
                userObj.Password = GeneratePassword(new PasswordProperties(length: 10, includeSpecialChars: true, includeNumbers: true, includeLowerCase: true, includeUpperCase: true));
            }
            var user = _mapper.Map<EmployeeUserForRegisterDto, User>(userObj);
            var dbUser = await _accountService.RegisterEmp(user, userObj.Password);
            if (userObj.SendPasswordByEmail == 1)
            {
                sendEmail(userObj.Email, userObj.Password);
            }
            return Ok(new { Succeeded = true, Data = new {   }, Message = string.Empty, Errors = new string[] { } });
        }

        private void sendEmail(string email, string password)
        {
            _logger.LogInformation($"Email: {email}, Password: {password}");
        }

        private string GeneratePassword(PasswordProperties passwordProperties)
        {
            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            const string numericChars = "0123456789";

            var allowedChars = "";
            if (passwordProperties.IncludeLowerCase) allowedChars += lowerCaseChars;
            if (passwordProperties.IncludeUpperCase) allowedChars += upperCaseChars;
            if (passwordProperties.IncludeSpecialChars) allowedChars += specialChars;
            if (passwordProperties.IncludeNumbers) allowedChars += numericChars;

            var randomBytes = new byte[passwordProperties.Length];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }

            var result = new char[passwordProperties.Length];
            for (int i = 0; i < passwordProperties.Length; i++)
            {
                var index = randomBytes[i] % allowedChars.Length;
                result[i] = allowedChars[index];
            }

            return new string(result);
        }

        [HttpPost("edit")]
        public IActionResult EditUser(UserForEditDto userObj)
        {
            if (User == null)
            {
                return Unauthorized(new { Succeeded = false, Errors = new string[] { } });
            }
            var userIdClaim = User.Claims.FirstOrDefault(e => e.Type == "identityId");
            var userRoleClaim = User.Claims.FirstOrDefault(e => e.Type == "_role");
            if (int.Parse(userIdClaim.Value) != userObj.Id)
            {
                if (!Constants.GetSystemRoles().Contains(userRoleClaim.Value))
                {
                    return Unauthorized(new { Succeeded = false, Errors = new string[] { } });
                }
            }

            var user = _accountService.GetUserById(userObj.Id);
            if (user == null)
            {
                return Unauthorized(new { Succeeded = false, Message = "Invalid user data", Errors = new string[] { } });
            }
            _mapper.Map(userObj, user);
            user.UpdatedDate = DateTime.Now;
            user.UpdatedBy = Int32.Parse(userIdClaim.Value);
            int res = 0;
            if (!string.IsNullOrEmpty(userObj.Password))
            {
                res = _accountService.Update(user, userObj.Password);
            }
            else
            {
                res = _accountService.Update(user);
            }
            if (res == 0)
            {
                return BadRequest(new { Succeeded = false, Message = string.Empty, Errors = new string[] { } });
            }
            return Ok(new { Succeeded = true, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("requesters")]
        public ActionResult<UserDto> GetRequesters([FromQuery] FilterDto filterDto)
        {
            //if (User == null || User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var users = _accountService.GetUsers().Where(u => u.Role == Constants.RequesterRole);

            var filteredList = users
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = users.Count();

            var userDtoList = _mapper.Map<List<UserDto>>(filteredList);
            return Ok(PaginationHelper.CreatePagedReponse<UserDto>(userDtoList, validFilter, totalRecords, _uriService, route));
        }

        [HttpPost("requesters")]
        public ActionResult<UserDto> GetRequestersByCriteria([FromQuery] FilterDto filterDto,UserSearchDto userSearch)
        {
            //if (User == null || User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var users = _accountService.GetUsers().Where(u => u.Role == Constants.RequesterRole
           && (userSearch.FullName == null || u.FullName.Replace(" ", "").Trim().Contains(userSearch.FullName.Replace(" ", "").Trim()))
            && (userSearch.PhoneNumber == null || u.PhoneNumber.Replace(" ", "").Trim().Contains(userSearch.PhoneNumber.Replace(" ", "").Trim()))
            );

            var filteredList = users
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = users.Count();

            var userDtoList = _mapper.Map<List<UserDto>>(filteredList);
            return Ok(PaginationHelper.CreatePagedReponse<UserDto>(userDtoList, validFilter, totalRecords, _uriService, route));
        }
        [HttpGet("users")]
        public ActionResult<UserDto> GetEmployees([FromQuery] FilterDto filterDto)
        {
            //if (User == null || User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var users = _accountService.GetUsers().Where(u => u.Role == Constants.EmployeeRole && u.IsActive == true);

            var filteredList = users
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = users.Count();

            var userDtoList = _mapper.Map<List<UserDto>>(filteredList);
            return Ok(PaginationHelper.CreatePagedReponse<UserDto>(userDtoList, validFilter, totalRecords, _uriService, route));
        }
        [HttpPost("users")]
        public ActionResult<UserDto> GetEmployeeListByCriteria([FromQuery] FilterDto filterDto, UserSearchDto userSearch)
        {
            //if (User == null || User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var users = _accountService.GetUsers().Where(u => u.Role == Constants.EmployeeRole && u.IsActive == true 
            && (userSearch.FullName==null || u.FullName.Replace(" ","").Trim().Contains(userSearch.FullName.Replace(" ", "").Trim()))
             && (userSearch.PhoneNumber == null || u.PhoneNumber.Replace(" ", "").Trim().Contains(userSearch.PhoneNumber.Replace(" ", "").Trim()))
            );

            var filteredList = users
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = users.Count();

            var userDtoList = _mapper.Map<List<UserDto>>(filteredList);
            return Ok(PaginationHelper.CreatePagedReponse<UserDto>(userDtoList, validFilter, totalRecords, _uriService, route));
        }
        [HttpGet("onlineEmployee")]
        public ActionResult<UserDto> GetOnlineEmployee([FromQuery] FilterDto filterDto)
        {

            var route = Request.Path.Value;
            var validFilter = new FilterDto(filterDto.PageNumber, filterDto.PageSize);
            var users = _accountService.GetUsers().Where(u => u.Role == Constants.EmployeeRole && u.LastLoginTime > DateTime.Now.AddDays(-1));

            var filteredList = users
                 .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                 .Take(validFilter.PageSize).ToList();
            var totalRecords = users.Count();

            var userDtoList = _mapper.Map<List<UserDto>>(filteredList);
            return Ok(PaginationHelper.CreatePagedReponse<UserDto>(userDtoList, validFilter, totalRecords, _uriService, route));
        }

        [HttpGet("users/{id}")]
        public ActionResult<UserDto> GetUserProfile([FromQuery] FilterDto filterDto, int id)
        {
            //if (User == null || User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}


            var user = _accountService.GetUsers().FirstOrDefault(u => u.Id == id);//&& u.Role == Constants.EmployeeRole 
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(new { Succeeded = true, Data = userDto, Message = string.Empty, Errors = new string[] { } });
        }

        [HttpGet("UserByPhone/{phone}")]
        public ActionResult<UserDto> GetUserByPhone([FromQuery] FilterDto filterDto, string phone)
        {
            //if (User == null || User.FindFirst(ClaimTypes.Role).Value != Constants.AdminRole)
            //{
            //    return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            //}
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}


            var user = _accountService.GetUsers().FirstOrDefault(u => u.Role == Constants.RequesterRole && u.PhoneNumber == phone);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(new { Succeeded = true, Data = userDto, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("deactivate")]
        public ActionResult<UserDto> Deactivate()
        {
            if (User == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            }
            var userIdClaim = User.Claims.FirstOrDefault(e => e.Type == "identityId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { Succeeded = false, Data = new { }, Message = "Unauthorized access", Errors = new string[] { } });
            }

            var user = _accountService.GetUsers().FirstOrDefault(u => u.Id == Int32.Parse(userIdClaim.Value));
            if (user == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Bad Request", Errors = new string[] { } });
            }
            _accountService.Deactivate(user);


            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("verify/{passcode}")]
        public ActionResult<UserDto> Verify(string passcode)
        {
            var verifiedModel = _accountService.VerifyPassCode(passcode);
            if (verifiedModel != null && verifiedModel.Result == true)
            {
                Entities.User user = verifiedModel?.User;
                _accountService.UpdateLastLogin(user);//update last login time
                var claims = new[]
                {
               new Claim( "identityId",user.Id.ToString()),
               new Claim( "firstName",user.FirstName),
               new Claim( "fullName",user.FullName),
               new Claim( "email",user.Email??""),
               new Claim( "phone",user.PhoneNumber??""),
               new Claim( "lastLoginTime",user.LastLoginTime.ToString()??""),
               new Claim( "photoUrl",user.PhotoUrl ?? ""),
               new Claim( "_role",user.Role)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:JWTTokenSecret").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(7),
                    SigningCredentials = creds
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var userModel = _mapper.Map<UserDto>(user);
                return Ok(new { Succeeded = true, Data = new { Token = tokenHandler.WriteToken(token), user = userModel }, Message = string.Empty, Errors = new string[] { } });
            }
            return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Passcode", Errors = new string[] { } });
        }

        [HttpGet("resendPasscode/{phone}")]
        public ActionResult<UserDto> ResendPasscode(string phone)
        {
            var user = _accountService.ResendPasscode(phone);
            if (user == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Phone Number or Invalid Operation", Errors = new string[] { } });
            }
            smsService.SendAsync($"رمز التحقق: {user.PassCode}", phone);
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpGet("whatsApp/{phone}")]
        public ActionResult<UserDto> SMSwhatsApp(string phone)
        {
            Random Random = new Random();
            var passcode=Random.Next(1111, 9999).ToString();
            whatsApp.SendAsync($"رمز التحقق: {passcode}", phone);
            return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
        }
        [HttpPost("forgotPassword")]
        public ActionResult<UserDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = _accountService.GetUserByPhone(forgotPasswordDto.PhoneNumber);
            if (user == null)
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Phone Number", Errors = new string[] { } });
            }
            var res = _accountService.Update(user, forgotPasswordDto.Password);
            if(res ==1)
            {
                return Ok(new { Succeeded = true, Data = new { }, Message = string.Empty, Errors = new string[] { } });
            }
            else
            {
                return BadRequest(new { Succeeded = false, Data = new { }, Message = "Invalid Operation", Errors = new string[] { } });
            }
        }
    }
}