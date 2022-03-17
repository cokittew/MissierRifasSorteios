using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Home.Services.Raffle
{
    public class RaffleSearch
    {
        [MaxLength(200, ErrorMessage ="Limite máximo de 200 caracteres.")]
        public string NameRaffleNickName { get; set; }

        [MaxLength(10, ErrorMessage = "Limite máximo de 10 caracteres.")]
        public string Code { get; set; }
    }
}
