using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserExtraModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.WorkClasses
{
    public class NumberStatusTotal
    {
        public NumberStatusTotal(List<PlatformServiceRaffleFile> receipts = null, List<NumberStatus> numberStatus = null, int raffleTotalNumbers = 0, UserBasicInfo raffleOwner = null,
            PlatformServiceRaffle raffle = null, PlatformServiceRaffleInformations raffleExtraInfo = null, UserSocialMidia raffleOwnerSocialMidia = null,
            int avaliableNumber = 0, int schaduleNumber = 0, int unavaliableNumber = 0, UserPixInformation userPixInformation = null, List<UserBankInformation> userBankInformation = null)
        {
            NumberStatus = numberStatus;
            RaffleTotalNumbers = raffleTotalNumbers;
            RaffleOwner = raffleOwner;
            Raffle = raffle;
            RaffleExtraInfo = raffleExtraInfo;
            AvaliableNumber = avaliableNumber;
            SchaduleNumber = schaduleNumber;
            UnavaliableNumber = unavaliableNumber;
            RaffleOwnerSocialMidia = raffleOwnerSocialMidia;
            UserPixInformation = userPixInformation;
            UserBankInformation = userBankInformation;
            ParticipantReceipts = receipts;
        }

        public NumberStatusTotal(UserBasicInfo raffleOwner,
            List<PlatformServiceRaffle> raffles, UserSocialMidia raffleOwnerSocialMidia,
            UserPixInformation userPixInformation, List<UserBankInformation> userBankInformation = null, PlatformServiceRaffleInformations raffleExtraInfo = null)
        {
            RaffleOwner = raffleOwner;
            Raffles = raffles;
            RaffleExtraInfo = raffleExtraInfo;
            RaffleOwnerSocialMidia = raffleOwnerSocialMidia;
            UserPixInformation = userPixInformation;
            UserBankInformation = userBankInformation;
        }

        //Raffle Simple Data
        public List<NumberStatus> NumberStatus { get; private set; }

        public int RaffleTotalNumbers { get; private set; }

        //Raffle Owner Data
        public UserBasicInfo RaffleOwner { get; private set; }
        public UserSocialMidia RaffleOwnerSocialMidia { get; private set; }
        public UserPixInformation UserPixInformation { get; private set; }
        public List<UserBankInformation> UserBankInformation { get; private set; }


        //Raffle All Data
        public PlatformServiceRaffle Raffle { get; private set; }
        public List<PlatformServiceRaffle> Raffles { get; private set; }

        public PlatformServiceRaffleInformations RaffleExtraInfo { get; private set; }

        //Raffle Statistics

        public int AvaliableNumber { get; private set; }
        public int SchaduleNumber { get; private set; }
        public int UnavaliableNumber { get; private set; }

        public List<int> ParticipantSelectNumber { get; set; }
        public List<int> ParticipantSelectNumberReceipt { get; set; }

        //Raffle Receipts Data

        public List<PlatformServiceRaffleFile> ParticipantReceipts { get; private set; }

        [NotMapped]
        [Required(ErrorMessage = "Marque o campo para validar o pagamento." )]
        public bool ConfirmBuy { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Marque o campo para validar o pagamento.")]
        public bool RefuseBuy { get; set; }

    }

}
