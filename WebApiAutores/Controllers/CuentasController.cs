using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebApiAutores.Dto;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly ILogger<CuentasController> _logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashServices hashServices;
        private readonly IDataProtector dataProtector;

        public CuentasController(
            ILogger<CuentasController> logger,
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashServices hashServices
        )
        {
            _logger = logger;
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashServices = hashServices;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_quizas_secreto");
        }


        [HttpGet("{textoPlano}")]
        public ActionResult RealizarHash(string textoPlano)
        {
            var resultado = hashServices.Hash(textoPlano);
            var resultado2 = hashServices.Hash(textoPlano);

            return Ok(new
            {
                textoPlano,
                Hash1 = resultado,
                Hash2 = resultado2
            });
        }


        [HttpGet("Encriptar")]
        public ActionResult Encriptar()
        {
            var textoPlano = "Vicente Marti";
            var textoCifrado = dataProtector.Protect(textoPlano);
            var textoDesencriptado = dataProtector.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano,
                textoCifrado,
                textoDesencriptado
            });
        }

        [HttpGet("EncriptarPorTiempo")]
        public ActionResult EncriptarPorTiempo()
        {
            var protectorLimitadoPorTiempo = dataProtector.ToTimeLimitedDataProtector();

            var textoPlano = "Vicente Marti";
            var textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano, lifetime: TimeSpan.FromSeconds(5));
            Thread.Sleep(6000);
            var textoDesencriptado = protectorLimitadoPorTiempo.Unprotect(textoCifrado);

            return Ok(new
            {
                textoPlano,
                textoCifrado,
                textoDesencriptado
            });
        }


        [HttpPost("registrar")]
        public async Task<ActionResult<RespuesAutenticacion>> Registrar(CredencialesUsuario credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(resultado.Errors);
            }

            return await ConstruirToken(credenciales);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuesAutenticacion>> Login(CredencialesUsuario credenciales)
        {
            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado == null && !resultado.Succeeded)
            {
                return BadRequest("Login incorrecto");
            }

            return await ConstruirToken(credenciales);

        }

        [HttpGet("RenoverToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuesAutenticacion>> RenovarToken()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUser = new CredencialesUsuario()
            {
                Email = email
            };

            return await ConstruirToken(credencialesUser);
        }

        private async Task<RespuesAutenticacion> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>()
            {
                new("email", credenciales.Email),
                new("lo que yo quier", "cualquier otro valor")
            };

            var usuario = await userManager.FindByEmailAsync(credenciales.Email);
            var claimDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimDB);

            var llaveConfig = configuration.GetValue<string>("llaveJwt");
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llaveConfig));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuesAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        [HttpPost("HacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

    }
}