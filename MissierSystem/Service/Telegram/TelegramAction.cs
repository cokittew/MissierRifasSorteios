using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MissierSystem.Service.Telegram
{
    public class TelegramAction
    {
        private string BotToken { get; }
        //private readonly string DefaultIdTelegram = "1285175398";

        public TelegramAction()
        {
            this.BotToken = "5065398956:AAE7N1huh4M7M3u_mwrG-MZn9d0elUQSuP8";
        }

        public async Task<bool> SendDefaultMessageStructure(string idTelegram, ToUserInformationsTelegram info, TelegramMessageAction action)
        {
            //idTelegram = DefaultIdTelegram;

            if (String.IsNullOrEmpty(idTelegram))
                return false;

            var message = BuildDefaultCaseMessage(info, action);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();

            var defaultUrl = "https://api.telegram.org/bot" + BotToken;
            var urlFirst = "/sendMessage?chat_id=" + idTelegram;
            var urlSecond = "&text=" + message;
            var urlThird = "&parse_mode=" + "markdown";

            //var url = $"https://api.telegram.org/bot1897190789:AAGHBnrfsGl9ddfoxUeEJmzDeMlf_8fVYy8/sendMessage?chat_id={idTelegram}&text={message}&parse_mode=markdown";
            var url = defaultUrl + urlFirst + urlSecond + urlThird;

            HttpResponseMessage response = await client.GetAsync(url);
            await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }

        private string BuildDefaultCaseMessage(ToUserInformationsTelegram info, TelegramMessageAction action)
        {
            var message = "";
            switch (action)
            {
                case TelegramMessageAction.NewRaffleCreated:
                    {
                        message = $"Ação: *{info.Action}*" +
                            $"\nFeita: {info.OcurrData}" +
                            $"\n*Nome do Sorteio:* {info.RaffleName}" +
                            $"\nPrêmio: {info.RewardName}" +
                            $"\nTotal de Números: *{info.NumberQuantity}*" +
                            $"\n*Valor por Número:* {info.RaffleNumberValue}" +
                            $"\n*Link do Sorteio*: {info.RaffleLink}" +
                            $"\nCódigo Único: *{info.RaffleCode}*";

                    };break;
                case TelegramMessageAction.aNumberHasBeenReserved : 
                    {
                        message = $"Ação: *{info.Action}*" +
                            $"\nFeita: {info.OcurrData}" +
                            $"\nNome do Sorteio: *{info.RaffleName}*" +
                            $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                            $"\nNúmero(s): *{info.NumberReference}*" +
                            $"\nTotal a Pagar: *{info.ValueOperation}*";

                    }; break;
                case TelegramMessageAction.aReceiptHasBeenSended : 
                    {
                        message = $"Ação: *{info.Action}*" +
                                $"\nFeita: {info.OcurrData}" +
                                $"\nNome do Sorteio: *{info.RaffleName}*" +
                                $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                                $"\nSeu(s) Número(s): *{info.NumberReference}*" +
                                $"\nTotal Pago: *{info.ValueOperation}*";

                    }; break;

                case TelegramMessageAction.aNumberHasBeenRefused: 
                    {
                        message = $"Ação: *{info.Action}*" +
                                    $"\nFeita: {info.OcurrData}" +
                                    $"\nNome do Sorteio: *{info.RaffleName}*" +
                                    $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                                    $"\nSeu(s) Número(s): *{info.NumberReference}*" +
                                    $"\nTotal Pago: *{info.ValueOperation}*" +
                                    $"\n\nEm caso de erro, entre em contato com o criador do sorteio.";


                    };break;
                case TelegramMessageAction.aNumberHasBeenApproved: 
                    {
                        message = $"Ação: *{info.Action}*" +
                                    $"\nFeita: {info.OcurrData}" +
                                    $"\nNome do Sorteio: *{info.RaffleName}*" +
                                    $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                                    $"\nSeu(s) Número(s): *{info.NumberReference}*" +
                                    $"\nTotal Pago: *{info.ValueOperation}*";

                    };break;
                case TelegramMessageAction.aNumberHasBeenManualOutReserved: 
                    {
                        message = $"Ação: *{info.Action}*" +
                                    $"\nFeita: {info.OcurrData}" +
                                    $"\nNome do Sorteio: *{info.RaffleName}*" +
                                    $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                                    $"\nSeu(s) Número(s): *{info.NumberReference}*" +
                                    $"\nTotal Pago: *{info.ValueOperation}*" +
                                    $"\n\nEm caso de erro, entre em contato com o criador do sorteio.";

                    };break;

                case TelegramMessageAction.anumberHasBeenAutomaicalyOutReserved:
                    {
                        message = $"Ação: *{info.Action}*" +
                                    $"\nFeita: {info.OcurrData}" +
                                    $"\nNome do Sorteio: *{info.RaffleName}*" +
                                    $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                                    $"\nSeu(s) Número(s): *{info.NumberReference}*";
                    }; break;

                case TelegramMessageAction.RaffleWinners:
                    { 
                        message =   $"*{info.Action}*" +
                                    $"\nNome do Sorteio: *{info.RaffleName}*" +
                                    $"\nCódigo do Sorteio: *{info.RaffleCode}*" +
                                    $"\nPrêmio: {info.RewardName}" +
                                    $"\n*Número Sorteado: {info.NumberReference}*" +
                                    $"\nDia do Sorteio: {info.OcurrData}" +
                                    $"\n\nEntre em contato com o criador do sorteio.";

                    }; break;

            }

            return message;
        }
    }
}
