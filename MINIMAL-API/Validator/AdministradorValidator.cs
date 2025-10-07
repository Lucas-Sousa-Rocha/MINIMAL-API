using MINIMAL_API.Dominio.Entidades;
using MINIMAL_API.Infraestrutura.Db;

namespace MINIMAL_API.Validator
{
    public class AdministradorValidator
    {
        private readonly DbContexto _contexto;
        public AdministradorValidator(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public void ValidarAdministrador(Administrador administrador)
        {
            if (administrador == null)
                throw new ArgumentException("Administrador inválido.");

            if (string.IsNullOrWhiteSpace(administrador.Nome))
                throw new ArgumentException("O nome do administrador é obrigatório.");

            if (string.IsNullOrWhiteSpace(administrador.Email))
                throw new ArgumentException("O e-mail do administrador é obrigatório.");

            if (string.IsNullOrWhiteSpace(administrador.Senha))
                throw new ArgumentException("A senha do administrador é obrigatória.");

            if (ExisteAdministrador(administrador))
                throw new DuplicateWaitObjectException("Já existe um administrador com o mesmo nome e e-mail.");
        }

        public bool ExisteAdministrador(Administrador administrador)
        {
            // Verifica se já existe outro administrador com o mesmo nome e e-mail, ignorando o próprio no caso de edição
            return _contexto.Administradores.Any(a =>
                a.Nome == administrador.Nome &&
                a.Email == administrador.Email &&
                a.Id != administrador.Id
            );
        }

    }
}
