using System;
using System.ComponentModel.DataAnnotations;

namespace MINIMAL_API.Dominio.DTOs
{
    public class VeiculoDTO
    {
        [Required(ErrorMessage = "O nome do veículo é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome não pode ter mais de 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A marca do veículo é obrigatória.")]
        [StringLength(100, ErrorMessage = "A marca não pode ter mais de 100 caracteres.")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "A data do veículo é obrigatória.")]
        public string Data { get; set; } // Data do veículo (apenas data, sem hora)
    }
}
