using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Repository.ProofPayment
{
    public class ProofPayment
    {
        public ProofPayment() { }
        public ProofPayment(int id, int userId, int raffleId, int referenceType,
            string numberSequence, string base64, string observation, decimal value, DateTime creation, bool allbought = true, bool aproved= false, bool removed=false) 
        {
            Id = id;
            UserId = userId;
            RaffleId = raffleId;
            ReferenceType = referenceType;
            NumberSequence = numberSequence;
            Base64File = base64;
            Observation = observation;
            Value = value;
            CreationDate = creation;
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int RaffleId { get; set; }
        public int ReferenceType { get; set; }
        public string NumberSequence { get; set; }
        public string Base64File { get; set; }
        public bool Aproved { get; set; }
        public bool AllBought { get; set; }
        public bool Removed { get; set; }
        public DateTime CreationDate { get; set; }

        #region Especifications

        public string Observation { get; set; }
        public decimal Value { get; set; }

        #endregion

        #region SystemControl
        public string FileKeySend { get; set; }
        #endregion
    }
}
