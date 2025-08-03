using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PruebaTecnicaAPI.Controllers;
using PruebaTecnicaAPI.Models;

namespace PruebaTecnicaAPI.Tests
{
    public class LoginTests
    {
        // Prueba 1: Verifica que el login sea exitoso con credenciales correctas.
        [Fact]
        public void Login_WithCorrectCredentials_ReturnsOkWithToken()
        {
            // Arrange
            // Creamos un Mock para la configuración, necesaria para el controlador.
            var mockConfig = new Mock<IConfiguration>();
            // Configuramos el mock para que devuelva una clave secreta cuando se pida.
            mockConfig.SetupGet(x => x[It.IsAny<string>()]).Returns("UnaClaveSecretaParaPruebas12345678901234");
            
            var controller = new AuthController(mockConfig.Object);
            var loginRequest = new LoginRequest { Email = "usuario@prueba.com", Password = "password123" };

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            // Esperamos un resultado de tipo OkObjectResult (código 200).
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Verificamos que el objeto retornado no sea nulo.
            Assert.NotNull(okResult.Value);
        }

        // Prueba 2: Verifica que el login falle con credenciales incorrectas.
        [Fact]
        public void Login_WithIncorrectCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.SetupGet(x => x[It.IsAny<string>()]).Returns("UnaClaveSecretaParaPruebas12345678901234");

            var controller = new AuthController(mockConfig.Object);
            var loginRequest = new LoginRequest { Email = "usuario@prueba.com", Password = "passwordincorrecta" };

            // Act
            var result = controller.Login(loginRequest);

            // Assert
            // Esperamos un resultado de tipo UnauthorizedResult (código 401).
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
