using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Infraestrutura.Db;

namespace MINIMAL_API.Validator
{
    public class VeiculoValidador 
    {
        private readonly DbContexto _contexto;
        public VeiculoValidador(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void ValidarVeiculo(Veiculo veiculo)
        {
            if (veiculo == null)
                throw new ArgumentException("Veículo inválido.");

            if (string.IsNullOrWhiteSpace(veiculo.Nome))
                throw new ArgumentException("O nome do veículo é obrigatório.");

            if (string.IsNullOrWhiteSpace(veiculo.Marca))
                throw new ArgumentException("A marca do veículo é obrigatória.");

            if (ExisteVeiculo(veiculo))
                throw new DuplicateWaitObjectException("Veículo já cadastrado.");
        }

        public bool ExisteVeiculo(Veiculo veiculo) 
        {
            return _contexto.Veiculos.Any(v => v.Nome == veiculo.Nome && v.Marca == veiculo.Marca);
        }

    }
}
