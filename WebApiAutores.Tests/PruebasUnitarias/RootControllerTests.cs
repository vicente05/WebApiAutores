﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using WebApiAutores.Controllers.V1;
using WebApiAutores.Tests.Mocks;

namespace WebApiAutores.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos4Links()
        {
            // Preparacón
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Ejecución

            var resultado = await rootController.Get();

            // Verificación

            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links()
        {
            // Preparacón
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Ejecución

            var resultado = await rootController.Get();

            // Verificación

            Assert.AreEqual(2, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2LinksUsandoMoq()
        {
            // Preparacón
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(), 
                It.IsAny<object>(), 
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )
            ).Returns(Task.FromResult(AuthorizationResult.Failed()));


            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
               It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(),
               It.IsAny<string>()
               )
           ).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();

            mockURLHelper.Setup(x => 
                x.Link(It.IsAny<string>(), It.IsAny<object>())
            ).Returns(string.Empty);

            var rootController = new RootController(mockAuthorizationService.Object)
            {
                Url = mockURLHelper.Object
            };

            // Ejecución

            var resultado = await rootController.Get();

            // Verificación

            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}
