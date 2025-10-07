using Microsoft.EntityFrameworkCore;
using MINIMAL_API.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MINIMAL_API.Dominio.Entidades
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = default!;
        [Required]
        [MaxLength(100)]
        public string Senha { get; set; } = default!;
        [Required]
        [MaxLength(10)]
        public Perfil Perfil { get; set; } = default!;
        [Required]
        [Column(TypeName = "date")]
        public DateTime DataCriacao { get; set; }
    }
}
