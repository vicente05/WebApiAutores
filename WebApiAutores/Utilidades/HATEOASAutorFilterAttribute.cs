using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.Dto;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GenerardorEnlaces generardorEnlaces;

        public HATEOASAutorFilterAttribute(GenerardorEnlaces generardorEnlaces)
        {
            this.generardorEnlaces = generardorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;

            var autorDTO = resultado.Value as AutorDto;

            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDto> ??
                    throw new ArgumentNullException("Se esperaba una instancia de AutorDTO o List<AutorDTO>");

                autoresDTO.ForEach(async autor => await generardorEnlaces.GenerarEnlaces(autor));
            }
            else
            {
                await generardorEnlaces.GenerarEnlaces(autorDTO);
            }

            await next();
        }
    }
}