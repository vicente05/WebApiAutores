using AutoMapper;
using WebApiAutores.Dto;
using WebApiAutores.Entitys;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles: Profile
    {

        public AutoMapperProfiles() 
        {
            CreateMap<CreateAutorDTO, Autor>();
            CreateMap<Autor, AutorDto>();
        }
      
    }
}
