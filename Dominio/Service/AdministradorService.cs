using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Infraestrutura.Db;
using MINIMAL_API.Validator;

namespace MINIMAL_API.Dominio.Interfaces
{
    public class AdministradorService : IAdministrador
    {
        private readonly DbContexto _contexto;
        private readonly AdministradorValidator _validador;
        public AdministradorService(DbContexto contexto, AdministradorValidator validador) 
        {
            _contexto = contexto;
            _validador = validador;
        }
        public Administrador login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }

        public void SalvarAdministrador(Administrador administrador)
        {
            _validador.ValidarAdministrador(administrador);
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
        }

        public void AtualizarAdministrador(Administrador administrador)
        {
            _validador.ValidarAdministrador(administrador);
            _contexto.Administradores.Update(administrador);
            _contexto.SaveChanges();
        }
    }
}
