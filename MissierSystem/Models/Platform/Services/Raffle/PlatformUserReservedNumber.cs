using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle
{
    [Table("platform_user_reserved_number")]
    public class PlatformUserReservedNumber
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("id_basic_user")]
        [Column("id_basic_user")]
        [Display(Name = "Id do Usuário")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdBasicUser { get; set; }

        [ForeignKey("id_raffle")]
        [Column("id_raffle")]
        [Display(Name = "Id do Sorteio/Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdRaffle { get; set; }

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
