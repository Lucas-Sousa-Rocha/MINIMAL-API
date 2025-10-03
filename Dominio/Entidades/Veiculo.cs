using System.ComponentModel.DataAnnotations;

namespace MINIMAL_API.Dominio.Entidades
{
    public class Veiculo
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }
        [Required]
        [StringLength(100)]
        public string Marca { get; set; }
        [Required(ErrorMessage = "A data é obrigatória")]
        public DateOnly Data { get; set; }

    }
}
