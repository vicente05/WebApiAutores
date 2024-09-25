using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : Controller
    {
        private readonly ILogger<ComentariosController> logger;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ComentariosController(ILogger<ComentariosController> logger, ApplicationDbContext dbContext, IMapper mapper)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {

            var existeLibro = await dbContext.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await dbContext.Comentarios.Where(cometarioDB => cometarioDB.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }


        [HttpGet("{id:int}", Name = "ObtenerCometario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {

            var comentario = await dbContext.Libros.FirstOrDefaultAsync(libroDB => libroDB.Id == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }


        [HttpPost]
        public async Task<ActionResult> Post(int libroId, [FromBody] CreateComentarioDTO createComentarioDTO)
        {
            var existeLibro = await dbContext.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(createComentarioDTO);
            comentario.LibroId = libroId;
            dbContext.Add(comentario);
            await dbContext.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerCometario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId, int id, CreateComentarioDTO createComentarioDTO)
        {
            var existeLibro = await dbContext.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var existeComentario = await dbContext.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);

            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(createComentarioDTO);


            comentario.Id = id;
            comentario.LibroId = libroId;

            dbContext.Update(comentario);
            await dbContext.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerCometario", new { id = comentario.Id, libroId }, comentarioDTO);
        }

    }
}