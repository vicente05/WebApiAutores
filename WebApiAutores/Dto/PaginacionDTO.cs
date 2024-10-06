using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Dto
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; }
        private int recordsPorPagina = 10;
        private readonly int cantidadMaxPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > cantidadMaxPorPagina) ? cantidadMaxPorPagina : value;
            }
        }
    }
}