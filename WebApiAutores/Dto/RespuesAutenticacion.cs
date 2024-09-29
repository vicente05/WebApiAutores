using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAutores.Dto
{
    public class RespuesAutenticacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }

    }
}