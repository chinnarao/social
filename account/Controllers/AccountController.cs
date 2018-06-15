using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace account.Controllers
{
    [Route("api/Account")]
    [Produces("application/json")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IConfiguration Configuration;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.Configuration = configuration;
        }

        public DateTime ExpiresAt
        {
            get
            {
                return DateTime.UtcNow.AddDays(Convert.ToDouble(Configuration["JWT_TOKEN:ExpiryDays"]));
            }
        }

        //{ "chinnarao@live.com", "1111"}, { "revati@live.com", "1111"}, { "riya@live.com", "1111"}, { "kavin@live.com", "1111"}
        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody]SignUpVM model)
        {
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
            try
            {
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    TokenVM vm = new TokenVM() { ExpiresAt = ExpiresAt, BearerToken = CreateToken(model.Email, model.Email), TokenType = JwtBearerDefaults.AuthenticationScheme, Email = model.Email };
                    return Ok(vm);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginVM model)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    TokenVM vm = new TokenVM() { ExpiresAt = ExpiresAt, BearerToken = CreateToken(model.Email, model.Email), TokenType = JwtBearerDefaults.AuthenticationScheme, Email = model.Email };
                    return Ok(vm);
                }
            }
            return Unauthorized();
        }

        [HttpGet("logoff")]
        public async Task<IActionResult> LogOff()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }

        private string CreateToken(string username, string email)
        {
            List<Claim> claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(
                            issuer: Configuration["JWT_TOKEN:Issuer"],
                            audience: Configuration["JWT_TOKEN:Audience"],
                            subject: new ClaimsIdentity(new GenericIdentity(username, "Token"), claims),
                            notBefore: DateTime.UtcNow,
                            expires: ExpiresAt,
                            signingCredentials: account.utility.Jwt.SigningCredentials);
            return tokenHandler.WriteToken(token);
        }
    }
}