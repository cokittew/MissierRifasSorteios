using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses.RaffleParticipant
{
    public class RaffleToPayPackage
    {
        public int RaffleId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantType { get; set; }
        public List<int> RaffleParticipantNumbers { get; set; }
        public decimal RaffleTotalValueToPay { get; set; }
    }
}
