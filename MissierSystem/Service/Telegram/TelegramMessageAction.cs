using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.Telegram
{
    public enum TelegramMessageAction
    {
        aNumberHasBeenReserved = 1,
        aReceiptHasBeenSended = 2,
        aNumberHasBeenApproved = 3,
        aNumberHasBeenRefused = 4,
        anumberHasBeenAutomaicalyOutReserved = 5,
        aNumberHasBeenManualOutReserved = 6,
        NewRaffleCreated = 7,
        RaffleWinners = 8,
        RaffleStartedManually = 9,
        RaffleEndedManually = 10,

    }
}
