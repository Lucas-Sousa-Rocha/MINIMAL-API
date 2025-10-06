using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;

namespace MINIMAL_API.Dominio.Interfaces
{
    public interface IAdministrador
    {
       Administrador? login(LoginDTO loginDTO);
       void SalvarAdministrador(Administrador administrador);
       void AtualizarAdministrador(Administrador administrador);
       List<Administrador> Todos(int pagina, string? nome = null, string? perfil = null);
       Administrador? BuscaPorID(int id);
    }
}
