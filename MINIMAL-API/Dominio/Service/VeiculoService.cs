using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Infraestrutura.Db;
using MINIMAL_API.Validator;

namespace MINIMAL_API.Dominio.Service
{
    public class VeiculoService : IVeiculo
    {
        private readonly DbContexto _contexto;
        private readonly VeiculoValidador _validador;
        public VeiculoService(DbContexto contexto, VeiculoValidador validador)
        {
            _contexto = contexto;
            _validador = validador;
        }

        public void AtualizarVeiculo(Veiculo veiculo)
        {
            _validador.ValidarVeiculo(veiculo);
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }
        public Veiculo BuscaPorID(int id)
        {
            var veiculo = _contexto.Veiculos.FirstOrDefault(v => v.Id == id);
            if (veiculo == null)
            {
                throw new KeyNotFoundException($"Veículo com ID {id} não foi encontrado.");
            }
            return veiculo;
        }

        public void DeletarVeiculo(int id)
        {
            var veiculo = BuscaPorID(id);
            if (veiculo == null)
                throw new ArgumentException("Veículo não encontrado.");
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void SalvarVeiculo(Veiculo veiculo)
        {
            _validador.ValidarVeiculo(veiculo);
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }


        public List<Veiculo> Todos(int pagina, string? nome = null, string? marca = null)
        {
            int tamanhoPagina = 10;

            var query = _contexto.Veiculos.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => v.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(marca))
            {
                query = query.Where(v => v.Marca.Contains(marca));
            }

            return query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToList();
        }
    }
}
