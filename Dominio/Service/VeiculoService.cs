using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Infraestrutura.Db;

namespace MINIMAL_API.Dominio.Service
{
    public class VeiculoService : IVeiculo
    {
        private readonly DbContexto _contexto;
        public VeiculoService(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void AtualizarVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }

        public Veiculo? BuscaPorID(int id)
        {
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public void DeletarVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void SalvarVeiculo(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }


        public List<Veiculo> Todos(int pagina, string? nome = null, string? marca = null)
        {
            int tamanhoPagina = 10; // ou um parâmetro se quiser flexível

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
