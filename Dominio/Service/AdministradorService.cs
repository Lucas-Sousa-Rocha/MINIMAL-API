using Microsoft.EntityFrameworkCore;
using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Infraestrutura.Db;

namespace MINIMAL_API.Dominio.Interfaces
{
    public class AdministradorService : IAdministrador
    {
        private readonly DbContexto _contexto;
        public AdministradorService(DbContexto contexto) 
        {
            _contexto = contexto;
        }
        public Administrador login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }
    }
}
