using MINIMAL_API.Dominio.Entidades;

namespace MINIMAL_API.Dominio.Interfaces
{
    public interface IVeiculo
    {
        List<Veiculo> Todos(int pagina, string? nome = null, string? marca = null);
        Veiculo? BuscaPorID(int id);
        void SalvarVeiculo(Veiculo veiculo);
        void AtualizarVeiculo(Veiculo veiculo);
        void DeletarVeiculo(int id);
    }
}
