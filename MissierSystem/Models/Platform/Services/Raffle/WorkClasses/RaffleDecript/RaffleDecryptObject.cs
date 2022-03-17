using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses
{
    public class RaffleDecryptObject
    {
        public RaffleDecryptObject(int raffleId, List<RaffleDataSingleBlock> raffleDataComponentFromEncode)
        {
            RaffleId = raffleId;
            RaffleDataComponentFromEncode = raffleDataComponentFromEncode;
        }

        public int RaffleId { get; set; }
        public List<RaffleDataSingleBlock> RaffleDataComponentFromEncode { get; set; }

    }
}
