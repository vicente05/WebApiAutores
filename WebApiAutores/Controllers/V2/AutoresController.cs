using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V2
{
    [Route("api/autores")]
    //[Route("api/v2/autores")]
    [CabeceraEstaPresente("x-version", "2")]
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

        [HttpGet(Name = "ObtenerAutoresV2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDto>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDto>>(autores);
        }

        [HttpGet("{nombre}", Name = "ObtenerAutorPorNombreV2")]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDTOConLibros>>> GetByName([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();
            return mapper.Map<List<AutorDTOConLibros>>(autores);
        }

        [HttpGet("{id:int}", Name = "ObtenerAutorV2")]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDto>> PrimerAutor(int id)
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

        [HttpPost(Name = "CrearAutorV2")]
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

            return CreatedAtRoute("ObtenerAutorV2", new { id = autor.Id }, autorDto);
        }

        [HttpPut("{id:int}", Name = "ActualizarAutorV2")]
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

        [HttpDelete("{id:int}", Name = "BorrarAutorV2")]
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
