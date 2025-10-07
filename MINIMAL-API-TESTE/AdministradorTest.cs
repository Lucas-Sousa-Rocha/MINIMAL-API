using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Enums;
using Xunit;

namespace MINIMAL_API_TESTE
{
    public class AdministradorTest
    {
        [Fact]
        public void TestarGetSetPropriedades()
        {
            // Arrange
            var administrador = new Administrador();

            // Act
            administrador.Id = 1;
            administrador.Nome = "Admin Test";
            administrador.Email = "lucas.sousa@outlook.com";
            administrador.Senha = "password123";
            administrador.Perfil = Perfil.ADMIN;

            // Assert
            Assert.Equal(1, administrador.Id);
            Assert.Equal("Admin Test", administrador.Nome);
            Assert.Equal("lucas.sousa@outlook.com", administrador.Email);
            Assert.Equal("password123", administrador.Senha);
            Assert.Equal(Perfil.ADMIN, administrador.Perfil);
        }
    }
}
