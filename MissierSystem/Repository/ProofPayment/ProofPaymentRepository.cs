using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Repository.ProofPayment
{
    public class ProofPaymentRepository
    {
        List<ProofPayment> listProofPayment = new List<ProofPayment>();

        public ProofPaymentRepository()
        {
            ProofPayment ProofPayment1 = new ProofPayment(1, 2, 3009, 1, "2", "", "Referente ao número x", 15, DateTime.Now, false);
            ProofPayment ProofPayment2 = new ProofPayment(1, 2, 3009, 1, "3", "", "Referente ao número x", 15, DateTime.Now, false);
            ProofPayment ProofPayment3 = new ProofPayment(1, 2, 3009, 1, "2,3", "", "Pagamento total de todos os números", 30, DateTime.Now);

            listProofPayment.Add(ProofPayment1);
            listProofPayment.Add(ProofPayment2);
            listProofPayment.Add(ProofPayment3);

        }

    }
}
