using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController : Controller
    {
        private readonly ILogger<ComentariosController> logger;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(
            ILogger<ComentariosController> logger,
            ApplicationDbContext dbContext,
            IMapper mapper,
            UserManager<IdentityUser> userManager
        )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "ObtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery] PaginacionDTO paginacion)
        {

            var existeLibro = await dbContext.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var queryable = dbContext.Comentarios.Where(cometarioDB => cometarioDB.LibroId == libroId).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var comentarios = await queryable.OrderBy(comentario => comentario.Id).Paginar(paginacion).ToListAsync();

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


        [HttpPost(Name = "CrearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, [FromBody] CreateComentarioDTO createComentarioDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var existeLibro = await dbContext.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(createComentarioDTO);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuario.Id;
            dbContext.Add(comentario);
            await dbContext.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ObtenerCometario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);

        }

        [HttpPut("{id:int}", Name = "ActualizarComentario")]
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