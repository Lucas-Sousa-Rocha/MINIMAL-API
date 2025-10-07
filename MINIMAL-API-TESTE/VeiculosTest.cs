using MINIMAL_API.Dominio.Entidades;
using System;
using Xunit;

namespace MINIMAL_API_TESTE
{
    public class VeiculosTest
    {
        [Fact]
        public void Deve_Criar_Veiculo_Com_Propriedades_Corretas()
        {
            // Arrange
            var dataEsperada = DateOnly.FromDateTime(DateTime.Now);

            // Act
            var veiculo = new Veiculo
            {
                Id = 1,
                Nome = "Carro Test",
                Marca = "Marca Test",
                Data = dataEsperada
            };

            // Assert
            Assert.Equal(1, veiculo.Id);
            Assert.Equal("Carro Test", veiculo.Nome);
            Assert.Equal("Marca Test", veiculo.Marca);
            Assert.Equal(dataEsperada, veiculo.Data);
        }
    }
}
