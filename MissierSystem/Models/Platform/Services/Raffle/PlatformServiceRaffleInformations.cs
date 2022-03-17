using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle
{
    [Table("platform_service_raffle_informations")]
    public class PlatformServiceRaffleInformations
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("id_raffle")]
        [Column("id_raffle")]
        [Display(Name = "Id do Sorteio/Rifa")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public int IdRaffle { get; set; }

        [Column("raffle_easy_link")]
        [Display(Name = "Fast Link")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string RaffleEasyLink { get; set; }

        [Column("raffle_link_result")]
        [Display(Name = "Link Para Postagem de Resultado")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string RaffleResultLink { get; set; }

        [Column("raffle_participant")]
        [Display(Name = "Participantes e Números")]
        [Required(ErrorMessage = "Este campo é obrigatório!")]
        public string RaffleParticipant { get; set; }

        [Column("removed")]
        [Display(Name = "Removido")]
        public bool Removed { get; set; }

    }
}
