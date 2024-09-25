using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Dto
{
    public class LibroDTOConAutores : LibroDTO
    {
        public List<AutorDto> Autores { get; set; }
    }
}