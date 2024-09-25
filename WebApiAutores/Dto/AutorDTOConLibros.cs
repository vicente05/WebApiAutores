using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Dto
{
    public class AutorDTOConLibros : AutorDto
    {
        public List<LibroDTO> Libros { get; set; }
    }
}