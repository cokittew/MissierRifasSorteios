using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.Telegram
{
    public class ToUserInformationsTelegram
    {
        public string Action { get; set; }
        public string AboutUserName { get; set; }
        public string NumberReference { get; set; }
        public string ValueOperation { get; set; }
        public string OcurrData { get; set; }
        public string PaymentType { get; set; }
        public string PaymentValue { get; set; }
        public string NumberQuantity { get; set; }
        public string ComplementMessage { get; set; }
        public string RewardName { get; set; }
        public string RaffleName { get; set; }
        public string RaffleNumberValue { get; set; }
        public string RaffleLink { get; set; }
        public string RaffleCode { get; set; }
    }
}
