using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;

namespace MINIMAL_API.Dominio.Interfaces
{
    public interface IAdministrador
    {
       Administrador? login(LoginDTO loginDTO);
    }
}
