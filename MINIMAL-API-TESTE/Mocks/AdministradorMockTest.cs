using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Enums;
using Moq;
using Xunit;

namespace MINIMAL_API_TESTE.Mocks
{
    public class AdministradorMockTest
    {
        private readonly Mock<IAdministrador> _administradorMock;

        public AdministradorMockTest()
        {
            _administradorMock = new Mock<IAdministrador>();
        }

        [Fact]
        public void Deve_Chamar_SalvarAdministrador_Uma_Vez()
        {
            // Arrange
            var admin = new Administrador
            {
                Nome = "Teste",
                Email = "teste@mail.com",
                Senha = "123456",
                Perfil = Perfil.ADMIN
            };

            _administradorMock.Setup(s => s.SalvarAdministrador(admin));

            // Act
            _administradorMock.Object.SalvarAdministrador(admin);

            // Assert
            _administradorMock.Verify(s => s.SalvarAdministrador(admin), Times.Once);
        }

        [Fact]
        public void Deve_Retornar_Administrador_Pelo_Email()
        {
            // Arrange
            var admin = new Administrador
            {
                Id = 1,
                Nome = "Admin",
                Email = "admin@mail.com",
                Senha = "123456",
                Perfil = Perfil.ADMIN
            };

            _administradorMock.Setup(s => s.BuscaPorEmail("admin@mail.com"))
                              .Returns(admin);

            // Act
            var resultado = _administradorMock.Object.BuscaPorEmail("admin@mail.com");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Admin", resultado.Nome);
            Assert.Equal(Perfil.ADMIN, resultado.Perfil);
        }

        [Fact]
        public void Deve_Chamar_AtualizarAdministrador_Uma_Vez()
        {
            // Arrange
            var admin = new Administrador
            {
                Id = 1,
                Nome = "Admin Atualizado",
                Email = "admin@mail.com",
                Senha = "123456",
                Perfil = Perfil.ADMIN
            };

            _administradorMock.Setup(s => s.AtualizarAdministrador(admin));

            // Act
            _administradorMock.Object.AtualizarAdministrador(admin);

            // Assert
            _administradorMock.Verify(s => s.AtualizarAdministrador(admin), Times.Once);
        }

        [Fact]
        public void Deve_Listar_Todos_Administradores()
        {
            // Arrange
            var lista = new List<Administrador>
            {
                new Administrador { Id = 1, Nome = "Admin 1", Email = "a1@mail.com", Perfil = Perfil.ADMIN },
                new Administrador { Id = 2, Nome = "Admin 2", Email = "a2@mail.com", Perfil = Perfil.USER }
            };

            _administradorMock.Setup(s => s.Todos(1, null, null)).Returns(lista);

            // Act
            var resultado = _administradorMock.Object.Todos(1, null, null);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }
    }
}
