using MINIMAL_API.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MINIMAL_API_TESTE
{
    public class VeiculosTest
    {
        [Fact]
        public void VeiculosTeste() 
        {
            // Arrange
            var veiculo = new Veiculo();
            // Act
            veiculo.Id = 1;
            veiculo.Nome = "Carro Test";
            veiculo.Marca = "Marca Test";
            veiculo.Data = DateOnly.FromDateTime(DateTime.Now);
            // Assert
            Assert.Equal(1, veiculo.Id);
            Assert.Equal("Carro Test", veiculo.Nome);
            Assert.Equal("Marca Test", veiculo.Marca);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), veiculo.Data);

        }
    }
}
