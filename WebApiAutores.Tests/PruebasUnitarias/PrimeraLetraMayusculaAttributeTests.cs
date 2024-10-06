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
            // Preparación
            var primerLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "felipe";
            var valContext = new ValidationContext(new { Nombre = valor });


            // Ejecución

            var resultado = primerLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificación

            Assert.AreEqual("la primera letra debe ser mayúscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void PrimeraLetraMayuscula_DevuelveSuccess()
        {
            // Preparación
            var primerLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "Felipe";
            var valContext = new ValidationContext(new { Nombre = valor });


            // Ejecución

            var resultado = primerLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificación

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorNull_DevuelveSuccess()
        {
            // Preparación
            var primerLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string? valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });


            // Ejecución

            var resultado = primerLetraMayuscula.GetValidationResult(valor, valContext);

            // Verificación

            Assert.IsNull(resultado);   
        }
    }
}