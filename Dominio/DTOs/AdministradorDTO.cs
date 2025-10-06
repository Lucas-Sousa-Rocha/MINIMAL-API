using MINIMAL_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MINIMAL_API.Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Nome { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public string Perfil { get; set; } = default!;
    }
}
