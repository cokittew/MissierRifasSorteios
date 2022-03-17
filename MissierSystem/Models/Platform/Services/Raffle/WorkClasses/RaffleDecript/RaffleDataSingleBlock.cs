using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses
{
    public class RaffleDataSingleBlock
    {
        public RaffleDataSingleBlock(int participantId, List<RaffleNumberDataBlock> numbers, int singleElementStartPosition,
            int singleElementEndPosition)
        {
            ParticipantId = participantId;
            Numbers = numbers;
            SingleElementStartPosition = singleElementStartPosition;
            SingleElementEndPosition = singleElementEndPosition;
        }

        public RaffleDataSingleBlock()
        {

        }

        public int ParticipantId { get; set; }
        public List<RaffleNumberDataBlock> Numbers { get; set; }
        public int SingleElementStartPosition { get; set; }
        public int SingleElementEndPosition { get; set; }

    }
}
