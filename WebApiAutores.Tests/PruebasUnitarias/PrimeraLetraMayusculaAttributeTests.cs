using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            // Preparaci�n
            var primerLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "felipe";
            var valContext = new ValidationContext(new { Nombre = valor });


            // Ejecuci�n

            var resultado = primerLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificaci�n

            Assert.AreEqual("la primera letra debe ser may�scula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void PrimeraLetraMayuscula_DevuelveSuccess()
        {
            // Preparaci�n
            var primerLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "Felipe";
            var valContext = new ValidationContext(new { Nombre = valor });


            // Ejecuci�n

            var resultado = primerLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificaci�n

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorNull_DevuelveSuccess()
        {
            // Preparaci�n
            var primerLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string? valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });


            // Ejecuci�n

            var resultado = primerLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificaci�n

            Assert.IsNull(resultado);   
        }
    }
}