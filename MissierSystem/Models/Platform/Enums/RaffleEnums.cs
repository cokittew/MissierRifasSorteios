using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Enums
{
    public static class RaffleEnums
    {
        public enum Status
        {
            Iniciado = 1,
            Agendado = 2,
            Finalizado = 3
        }

        public enum PaymentAllowed
        {
            MercadoLivre = 1,
            PagSeguro = 2,
            EnvioComprovante = 3,
            Todos = 4,
        }

        public static string GetPixTypeName(int type)
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
