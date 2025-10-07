using MINIMAL_API.Dominio.Entidades;
using System;
using System.Collections.Generic;
using Xunit;

namespace MINIMAL_API_TESTE
{
    public class VeiculosMockTest
    {
        [Fact]
        public void Deve_Adicionar_Veiculo_Em_Lista_Mock()
        {
            // Arrange
            var veiculos = new List<Veiculo>();
            var dataCadastro = DateOnly.FromDateTime(new DateTime(2025, 10, 7));

            var veiculo = new Veiculo
            {
                Id = 1,
                Nome = "Gol 1.6",
                Marca = "Volkswagen",
                Data = dataCadastro
            };

            // Act
            veiculos.Add(veiculo);

            // Assert
            Assert.Single(veiculos);
            Assert.Equal("Gol 1.6", veiculos[0].Nome);
            Assert.Equal("Volkswagen", veiculos[0].Marca);
            Assert.Equal(dataCadastro, veiculos[0].Data);
        }
    }
}
