using AutoMapper;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;
using WebApiAutores.Migrations;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {

            #region AUTORES

            CreateMap<CreateAutorDTO, Autor>();
            CreateMap<Autor, AutorDto>();
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(AutorDto => AutorDto.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

            #endregion AUTORES

            #region LIBROS

            CreateMap<CreateLibroDTO, Libro>()
                .ForMember(libro => libro.AutorLibro, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libro => libro.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            #endregion

            #region COMENTARIOS

            CreateMap<CreateComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            #endregion

        }

        private List<AutorLibro> MapAutoresLibros(CreateLibroDTO createLibroDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if (createLibroDTO.AutoresIds == null)
            {
                return resultado;
            }

            foreach (var autorId in createLibroDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }

            return resultado;
        }

        private List<AutorDto> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDto>();

            if (libro.AutorLibro == null)
            {
                return resultado;
            }

            foreach (var autorLibro in libro.AutorLibro)
            {
                resultado.Add(new AutorDto() { Id = autorLibro.AutorId, Nombre = autorLibro.Autor.Nombre });
            }

            return resultado;
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDto autorDto)
        {
            var resultado = new List<LibroDTO>();

            if (autor.AutorLibro == null)
            {
                return resultado;
            }

            foreach (var autorLibro in autor.AutorLibro)
            {
                resultado.Add(new LibroDTO() { Id = autorLibro.AutorId, Titulo = autorLibro.Libro.Titulo });
            }

            return resultado;
        }

    }
}
