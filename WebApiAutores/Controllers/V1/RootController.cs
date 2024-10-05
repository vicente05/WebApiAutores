using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Dto;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatosHATEOAS>>> Get()
        {
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            var datosHateoas = new List<DatosHATEOAS>
            {
                new(enlace: Url.Link("ObtenerRoot", new { }), descripcion: "self", metodo: "Get"),
                new(enlace: Url.Link("ObtenerAutores", new { }), descripcion: "autores", metodo: "Get"),
            };

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new(enlace: Url.Link("CrearAutor", new { }), descripcion: "autor-crear", metodo: "Post"));
                datosHateoas.Add(new(enlace: Url.Link("CrearLibro", new { }), descripcion: "libro-crear", metodo: "Post"));
            }

            return datosHateoas;
        }

    }
}