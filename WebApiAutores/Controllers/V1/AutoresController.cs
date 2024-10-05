using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Utilidades;
using System.Net;

namespace WebApiAutores.Controllers.V1
{
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version", "1")]
    //[Route("api/v1/autores")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerAutoresV1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDto>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<AutorDto>>(autores);
        }

        [HttpGet("{nombre}", Name = "ObtenerAutorPorNombreV1")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDTOConLibros>>> GetByName([FromRoute] string nombre)
        {

            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTOConLibros>>(autores);

        }

        [HttpGet("{id:int}", Name = "ObtenerAutorV1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDto>> GetById(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutorLibro)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDto>(autor);

            return dto;
        }

        [HttpPost(Name = "CrearAutorV1")]
        public async Task<ActionResult> Post([FromBody] CreateAutorDTO createAutorDTO)
        {
            var exiteAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == createAutorDTO.Nombre);

            if (exiteAutorConElMismoNombre)
            {
                return BadRequest($"Ya exite un autor con el nombre {createAutorDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(createAutorDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDto = mapper.Map<AutorDto>(autor);

            return CreatedAtRoute("ObtenerAutorV1", new { id = autor.Id }, autorDto);
        }

        [HttpPut("{id:int}", Name = "ActualizarAutorV1")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateAutorDTO createAutorDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existeAutor)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(createAutorDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Borrar un autor
        /// </summary>
        /// <param name="id">Id del autor a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "BorrarAutorV1")]
        public async Task<ActionResult> Delete(int id)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existeAutor)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
