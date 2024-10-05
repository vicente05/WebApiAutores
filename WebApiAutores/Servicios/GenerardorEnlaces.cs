using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAutores.Dto;

namespace WebApiAutores.Servicios
{
    public class GenerardorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GenerardorEnlaces(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor
        )
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);

        }

        private async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return resultado.Succeeded;
        }

        public async Task GenerarEnlaces(AutorDto autorDto)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirURLHelper();

            autorDto.Enlaces.Add(new DatosHATEOAS(
                enlace: Url.Link("ObtenerAutor", new { id = autorDto.Id }),
                descripcion: "self",
                metodo: "GET"
            ));

            if (esAdmin)
            {
                autorDto.Enlaces.Add(new DatosHATEOAS(
                    enlace: Url.Link("ActualizarAutor", new { id = autorDto.Id }),
                    descripcion: "autor-actualizar",
                    metodo: "PUT"
                ));

                autorDto.Enlaces.Add(new DatosHATEOAS(
                    enlace: Url.Link("BorrarAutor", new { id = autorDto.Id }),
                    descripcion: "borrar-autor",
                    metodo: "DELETE"
                ));
            }

        }

    }
}