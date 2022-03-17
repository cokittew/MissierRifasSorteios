using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle
{
    [Table("platform_guest_reserved_number")]
    public class PlatformGuestReservedNumber
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("id_raffle")]
        [Column("id_raffle")]
        [Display(Name = "Id do Sorteio/Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdRaffle { get; set; }

        [Column("id_identity")]
        [Display(Name = "Código Único")]
        [MaxLength(10, ErrorMessage = "Identity ultrapassou o limite de 10 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string Identity { get; set; }

        [Column("full_name")]
        [Display(Name = "Código Único")]
        [MaxLength(200, ErrorMessage = "Limite máximo de 200 caracteres")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string FullName { get; set; }

        [Column("number")]
        [Display(Name = "Nº")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int Number { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

        [Column("beginning_date")]
        [Display(Name = "Primeiro Acesso Registrado")]
        public DateTime BeginningDate { get; set; }

    }
}
