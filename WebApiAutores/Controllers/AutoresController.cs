using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace WebApiAutores.Controllers
{
    [Route("api/autores")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AutoresController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDto>>> Get()
        {

            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDto>>(autores);

        }

        [HttpGet("primero")]
        public async Task<ActionResult<List<Autor>>> PrimerAutorV2([FromHeader] int miValor)
        {

            return await context.Autores.ToListAsync();

        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTOConLibros>>> Get([FromRoute] string nombre)
        {

            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTOConLibros>>(autores);

        }

        [HttpGet("{id:int}", Name = "obtenerAutor")]
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

            return mapper.Map<AutorDto>(autor);
        }

        [HttpPost]
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

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDto);
        }

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
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
