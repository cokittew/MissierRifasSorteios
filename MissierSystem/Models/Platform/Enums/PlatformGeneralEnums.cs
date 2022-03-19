using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Enums
{
    public static class PlatformGeneralEnums
    {
        public enum ServicesName
        {
            Raffle = 1, // Rifa/Sorteios
        }

        public enum PixKeyType
        {
            CPFCNPJ = 1,
            Email = 2,
            Telefone = 3,
            ChaveAleatoria = 4
        }


        static string GetName(int type)
        {
            switch (type)
            {
                case 1: return "CPF/CNPJ";
                case 2: return "Email";
                case 3: return "Telefone";
                case 4: return "Chave Aleatória";
                default: return "";
            }
        }
    }
}
