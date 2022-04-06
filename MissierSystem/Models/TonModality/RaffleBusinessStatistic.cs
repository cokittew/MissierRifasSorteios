using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.TonModality
{
    public class RaffleBusinessStatistic
    {
        public int RaffleId { get; set; }
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public int ParticipantId { get; set; }

        public string RaffleName { get; set; }
        public string ParticipantFullName { get; set; }
        public string ParticipantPhoneNumber { get; set; }
        public string ParticipantNumbers { get; set; }

        public string FinalStatus { get; set; }
        public int NumberQuantity { get; set; }
        public string TotalValueString { get; set; }
        public decimal TotalValue { get; set; }

        public string RaffleNumbersValueString { get; set; }
        public decimal RaffleNumbersValue { get; set; }

    }
}
