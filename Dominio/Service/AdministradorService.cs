using MINIMAL_API.Dominio.DTOs;
using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Enums;
using MINIMAL_API.Infraestrutura.Db;
using MINIMAL_API.Validator;
using System.Text.RegularExpressions;

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

        public List<Administrador> Todos(int pagina, string? nome = null, string? perfil = null)
        {
            int tamanhoPagina = 10;

            var query = _contexto.Administradores.AsQueryable();

            // Filtra pelo nome, se informado
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(a => a.Nome.Contains(nome));
            }

            // Filtra pelo perfil, se informado
            if (!string.IsNullOrEmpty(perfil))
            {
                // Converte a string para enum
                if (Enum.TryParse<Perfil>(perfil, true, out var perfilEnum))
                {
                    query = query.Where(a => a.Perfil == perfilEnum);
                }
                else
                {
                    // Se a string não for válida, não retorna nenhum resultado
                    query = query.Where(a => false);
                }
            }

            return query
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToList();
        }


        public Administrador? BuscaPorID(int id)
        {
            return _contexto.Administradores.FirstOrDefault(a => a.Id == id);
        }

        public Administrador BuscaPorEmail(string email)
        {
            var admin = _contexto.Administradores.FirstOrDefault(a => a.Email == email);
            if (admin == null)
                throw new KeyNotFoundException("Administrador não encontrado.");
            return admin;
        }

    }
}
