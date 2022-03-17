using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.LocalDataBase
{
    public class LocalDataBase
    {
        public class BanksInfo
        {
            public BanksInfo(string bankCode, string bankName)
            {
                BankCode = bankCode;
                BankName = bankName;
            }

            public string BankName { get; set; }
            public string BankCode { get; set; }
        }

        public class PixInfo
        {
            public PixInfo(int id, string type)
            {
                Id = id;
                PixType = type;
            }
            public int Id { get; set; }
            public string PixType { get; set; }
        }

        
        public static List<BanksInfo> GetAllBank()
        {
            List<BanksInfo> Banks = new List<BanksInfo>();
            Banks.Add(new BanksInfo("260", "Nubank"));
            Banks.Add(new BanksInfo("077", "BANCO INTER S.A"));
            Banks.Add(new BanksInfo("237", "BRADESCO S.A / NEXT"));
            Banks.Add(new BanksInfo("290", "Pagseguro Internet S.A (PagBank)"));
            Banks.Add(new BanksInfo("323", "Mercado Pago"));
            Banks.Add(new BanksInfo("104", "CAIXA ECONÔMICA FEDERAL"));

            return Banks;
        }

        public static List<PixInfo> GetAllPixType()
        {
            List<PixInfo> Pix = new List<PixInfo>();
            Pix.Add(new PixInfo(1, "CPF"));
            Pix.Add(new PixInfo(2, "Email"));
            Pix.Add(new PixInfo(3, "Telefone"));
            Pix.Add(new PixInfo(4, "Chave Aleatória"));

            return Pix;
        }

    }
}
