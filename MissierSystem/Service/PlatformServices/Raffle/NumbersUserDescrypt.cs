using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Service.PlatformServices.Raffle
{
    public class NumbersUserDescrypt
    {
        public static List<NumberStatus> GetRaffleCurrentNumberSituation(string encode)
        {
            var number = "";
            int elementeStart = 0;
            int idUserStart = 0;
            var idUsuario = "";
            int numberStart = 0;
            int status = 3;
            List<NumberStatus> CurrentRaffleSituation = new List<NumberStatus>();
            List<UserBasic> UserParticipating = new List<UserBasic>();
            List<UserNumbers> ParticipatingRaffleData = new List<UserNumbers>();
            List<int> JustNumbers = new List<int>();

            NumberStatus numberStatus = new NumberStatus();
            UserBasic user = new UserBasic();
            UserNumbers userNumber = new UserNumbers();

            if (!String.IsNullOrEmpty(encode) && !encode.Equals("none"))
            {
                for (int i=0; i < encode.Length; i++)
                {
                    if (encode.ElementAt(i).Equals('|'))
                    {
                        if(elementeStart < 1)
                            elementeStart += 1;
                        else
                            elementeStart = 0;
                    }
                    else
                    {
                        if (elementeStart == 1)
                        {
                            if (encode.ElementAt(i).Equals('*'))
                            {
                                if(idUserStart < 1)
                                    idUserStart += 1;
                                else
                                {
                                    user = new UserBasic { Id = Convert.ToInt32(idUsuario) };
                                    UserParticipating.Add(user);
                                    idUserStart = 2;
                                }
                            }
                            else
                            {
                                if(idUserStart == 1)
                                    idUsuario += encode.ElementAt(i);

                                if (idUserStart == 2)
                                {
                                    if (encode.ElementAt(i).Equals('(') || encode.ElementAt(i).Equals(')'))
                                    {
                                        if (encode.ElementAt(i).Equals(')'))
                                        {
                                            numberStatus = new NumberStatus(Convert.ToInt32(number), status);
                                            CurrentRaffleSituation.Add(numberStatus);
                                            JustNumbers.Add(Convert.ToInt32(number));
                                            status = 3;
                                            number = "";
                                        }

                                        if (numberStart < 1)
                                            numberStart += 1;
                                        else
                                            numberStart = 2;
                                    }
                                    else
                                    {
                                        if (numberStart == 1)
                                        {
                                            if (encode.ElementAt(i).Equals(','))
                                            {
                                                numberStatus = new NumberStatus(Convert.ToInt32(number), status);
                                                CurrentRaffleSituation.Add(numberStatus);
                                                JustNumbers.Add(Convert.ToInt32(number));
                                                status = 3;
                                                number = "";
                                            }
                                            else
                                            {
                                                if (encode.ElementAt(i).Equals('@'))
                                                    status = 2;
                                                else
                                                    number += encode.ElementAt(i);
                                            }
                                        }

                                        if(numberStart == 2)
                                        {
                                            if (encode.ElementAt(i).Equals('$'))
                                            {
                                                userNumber.User = user;
                                                userNumber.Numbers = JustNumbers;
                                                userNumber.NumberStatus = CurrentRaffleSituation;

                                                ParticipatingRaffleData.Add(userNumber);

                                                //Limpando dados para os proximos;
                                                user = new UserBasic();
                                                JustNumbers = new List<int>();
                                                //CurrentRaffleSituation = new List<NumberStatus>();
                                                userNumber = new UserNumbers();
                                                numberStatus = new NumberStatus();

                                                numberStart = 0;
                                                idUserStart = 0;
                                                idUsuario = "";
                                                number = "";
                                                elementeStart = 0;
                                                status = 3;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return CurrentRaffleSituation;
        }

        public static List<UserNumbers> GetRaffleCurrentRaffleSituationAllData(string encode)
        {
            var number = "";
            int elementeStart = 0;
            int idUserStart = 0;
            var idUsuario = "";
            int numberStart = 0;
            int status = 3;
            List<NumberStatus> CurrentRaffleSituation = new List<NumberStatus>();
            List<UserBasic> UserParticipating = new List<UserBasic>();
            List<UserNumbers> ParticipatingRaffleData = new List<UserNumbers>();
            List<int> JustNumbers = new List<int>();

            NumberStatus numberStatus = new NumberStatus();
            UserBasic user = new UserBasic();
            UserNumbers userNumber = new UserNumbers();


            if (!String.IsNullOrEmpty(encode) && !encode.Equals("none"))
            {
                for (int i = 0; i < encode.Length; i++)
                {
                    if (encode.ElementAt(i).Equals('|'))
                    {
                        if (elementeStart < 1)
                            elementeStart += 1;
                        else
                            elementeStart = 0;
                    }
                    else
                    {
                        if (elementeStart == 1)
                        {
                            if (encode.ElementAt(i).Equals('*'))
                            {
                                if (idUserStart < 1)
                                    idUserStart += 1;
                                else
                                {
                                    user = new UserBasic { Id = Convert.ToInt32(idUsuario) };
                                    UserParticipating.Add(user);
                                    idUserStart = 2;
                                }
                            }
                            else
                            {
                                if (idUserStart == 1)
                                    idUsuario += encode.ElementAt(i);

                                if (idUserStart == 2)
                                {
                                    if (encode.ElementAt(i).Equals('(') || encode.ElementAt(i).Equals(')'))
                                    {
                                        if (encode.ElementAt(i).Equals(')'))
                                        {
                                            numberStatus = new NumberStatus(Convert.ToInt32(number), status);
                                            CurrentRaffleSituation.Add(numberStatus);
                                            JustNumbers.Add(Convert.ToInt32(number));
                                            status = 3;
                                            number = "";
                                        }

                                        if (numberStart < 1)
                                            numberStart += 1;
                                        else
                                            numberStart = 2;
                                    }
                                    else
                                    {
                                        if (numberStart == 1)
                                        {
                                            if (encode.ElementAt(i).Equals(','))
                                            {
                                                numberStatus = new NumberStatus(Convert.ToInt32(number), status);
                                                CurrentRaffleSituation.Add(numberStatus);
                                                JustNumbers.Add(Convert.ToInt32(number));
                                                status = 3;
                                                number = "";
                                            }
                                            else
                                            {
                                                if (encode.ElementAt(i).Equals('@'))
                                                    status = 2;
                                                else
                                                    number += encode.ElementAt(i);
                                            }
                                        }

                                        if (numberStart == 2)
                                        {
                                            if (encode.ElementAt(i).Equals('$'))
                                            {
                                                userNumber.User = user;
                                                userNumber.Numbers = JustNumbers;
                                                userNumber.NumberStatus = CurrentRaffleSituation;

                                                ParticipatingRaffleData.Add(userNumber);

                                                //Limpando dados para os proximos;
                                                user = new UserBasic();
                                                JustNumbers = new List<int>();
                                                CurrentRaffleSituation = new List<NumberStatus>();
                                                userNumber = new UserNumbers();
                                                numberStatus = new NumberStatus();

                                                numberStart = 0;
                                                idUserStart = 0;
                                                idUsuario = "";
                                                number = "";
                                                elementeStart = 0;
                                                status = 3;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ParticipatingRaffleData;
        }

        public static List<UserNumbers> GetRaffleCurrentRaffleUserData(string encode)
        {

            //Variaveis pré salvamento
            var CreateNumber = "";
            var CreateUserId = "";
            int Status = 3;

            //Controladores
            int DecriptStartElement = 0;
            int DecriptGetUserId = 0;
            int DecriptGetRaffleNumbers = 0;
            
            List<NumberStatus> CurrentRaffleSituation = new List<NumberStatus>();
            List<UserBasic> UserParticipating = new List<UserBasic>();
            List<UserNumbers> ParticipatingRaffleData = new List<UserNumbers>();
            List<int> JustNumbers = new List<int>();

            NumberStatus numberStatus = new NumberStatus();
            UserBasic user = new UserBasic();
            UserNumbers userNumber = new UserNumbers();

            if (!String.IsNullOrEmpty(encode) && !encode.Equals("none"))
            {
                for (int i = 0; i < encode.Length; i++)
                {
                    if (encode.ElementAt(i).Equals('|'))
                    {
                        if (DecriptStartElement < 1)
                            DecriptStartElement += 1;
                        else
                            DecriptStartElement = 0;
                    }
                    else
                    {
                        if (DecriptStartElement == 1)
                        {
                            if (encode.ElementAt(i).Equals('*'))
                            {
                                if (DecriptGetUserId < 1)
                                    DecriptGetUserId += 1;
                                else
                                {
                                    //Encontra o segundo * e salva o id encontrado
                                    user = new UserBasic { Id = Convert.ToInt32(CreateUserId) };
                                    UserParticipating.Add(user);
                                    DecriptGetUserId = 2;
                                }
                            }
                            else
                            {
                                if (DecriptGetUserId == 1)
                                    CreateUserId += encode.ElementAt(i); // Montando o Id do usuário

                                if (DecriptGetUserId == 2)
                                {

                                    //Já encontrou e salvou o id do usuário e começa a ver seus números e status dos mesmos

                                    if (encode.ElementAt(i).Equals('(') || encode.ElementAt(i).Equals(')'))
                                    {


                                        if (encode.ElementAt(i).Equals(')'))
                                        {
                                            //Identificou que todos os números já foram selecionados e salvos.
                                            numberStatus = new NumberStatus(Convert.ToInt32(CreateNumber), Status);
                                            CurrentRaffleSituation.Add(numberStatus);
                                            JustNumbers.Add(Convert.ToInt32(CreateNumber));
                                            Status = 3;
                                            CreateNumber = "";
                                        }

                                        if (DecriptGetRaffleNumbers < 1)
                                            DecriptGetRaffleNumbers += 1;
                                        else
                                            DecriptGetRaffleNumbers = 2;
                                    }
                                    else
                                    {
                                        if (DecriptGetRaffleNumbers == 1)
                                        {
                                            // Identificou que é pra verificar os números até achar a virgula de separação.

                                            if (encode.ElementAt(i).Equals(','))
                                            {
                                                //Se achou a virgula ele salva o número pre-salvo em uma variavel em uma lista

                                                numberStatus = new NumberStatus(Convert.ToInt32(CreateNumber), Status);
                                                CurrentRaffleSituation.Add(numberStatus);
                                                JustNumbers.Add(Convert.ToInt32(CreateNumber));

                                                //Repadroniza para o próximo numero se houver

                                                Status = 3;
                                                CreateNumber = "";
                                            }
                                            else
                                            {
                                                //Se exitir um @ após o número o seta como em reserva. e continua o fluxo

                                                if (encode.ElementAt(i).Equals('@'))
                                                    Status = 2;
                                                else
                                                    CreateNumber += encode.ElementAt(i); // Salva os caracteres que formam o número.
                                            }
                                        }

                                        if (DecriptGetRaffleNumbers == 2)
                                        {
                                            //O sistema encontrou a ')' entendendo que finalizou o registro dos número;


                                            //O $ indica o fim de um conjunto de dados ligados a um usuario;
                                            if (encode.ElementAt(i).Equals('$'))
                                            {
                                                userNumber.User = user;
                                                userNumber.Numbers = JustNumbers;
                                                userNumber.NumberStatus = CurrentRaffleSituation;

                                                ParticipatingRaffleData.Add(userNumber);

                                                //Limpando dados para os proximos;
                                                user = new UserBasic();
                                                JustNumbers = new List<int>();
                                                CurrentRaffleSituation = new List<NumberStatus>();
                                                userNumber = new UserNumbers();
                                                numberStatus = new NumberStatus();

                                                //Variaveis que guardas os valores pré salvos;
                                                CreateUserId = "";
                                                CreateNumber = "";
                                                Status = 3;

                                                //Zerando os controladores para o proxímo conjunto de dados;
                                                DecriptGetUserId = 0;
                                                DecriptGetRaffleNumbers = 0;
                                                DecriptStartElement = 0;
                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ParticipatingRaffleData;
        }

        public static RaffleDecryptObject CreateRuffleDecryptObjectByEncode(string encode, int raffleId)
        {
            //Variaveis pré salvamento
            var CreateNumber = "";
            var CreateUserId = "";
            int Status = 3;

            //Controladores
            int DecriptStartElement = 0;
            int DecriptGetUserId = 0;
            int DecriptGetRaffleNumbers = 0;

            //Controle de elemento

            //Objetos de descriptação
            RaffleNumberDataBlock _RaffleNumberDataBlock = new RaffleNumberDataBlock();
            RaffleDataSingleBlock _RaffleDataSingleBlock = new RaffleDataSingleBlock(0, new List<RaffleNumberDataBlock>(), 0, 0);
            RaffleDecryptObject _RaffleDecryptObject = new RaffleDecryptObject(0, new List<RaffleDataSingleBlock>());

            //Talvez por tudo dentro de um Try-Catch

            if ((!String.IsNullOrEmpty(encode) && !encode.ToLower().Equals("none")) || raffleId != 0)
            {
                _RaffleDecryptObject.RaffleId = Convert.ToInt32(raffleId);

                for (int i = 0; i < encode.Length; i++)
                {
                    if (encode.ElementAt(i).Equals('|'))
                    {
                        _RaffleDataSingleBlock.SingleElementStartPosition = i;

                        if (DecriptStartElement < 1)
                            DecriptStartElement += 1;
                        else
                            DecriptStartElement = 0;
                    }
                    else
                    {
                        if (DecriptStartElement == 1)
                        {
                            if (encode.ElementAt(i).Equals('*'))
                            {
                                if (DecriptGetUserId < 1)
                                    DecriptGetUserId += 1;
                                else
                                {
                                    _RaffleDataSingleBlock.ParticipantId = Convert.ToInt32(CreateUserId);
                                    DecriptGetUserId = 2;
                                }
                            }
                            else
                            {
                                if (DecriptGetUserId == 1)
                                    CreateUserId += encode.ElementAt(i); // Montando o Id do usuário

                                if (DecriptGetUserId == 2)
                                {
                                    //Já encontrou e salvou o id do usuário e começa a ver seus números e status dos mesmos

                                    if (encode.ElementAt(i).Equals('(') || encode.ElementAt(i).Equals(')'))
                                    {
                                        if (encode.ElementAt(i).Equals(')'))
                                        {
                                            //Identificou que todos os números já foram selecionados e salvos.
                                            _RaffleNumberDataBlock.Number = Convert.ToInt32(CreateNumber);
                                            _RaffleNumberDataBlock.Status =  Status;
                                            _RaffleDataSingleBlock.Numbers.Add(_RaffleNumberDataBlock);
                                            _RaffleNumberDataBlock = new RaffleNumberDataBlock();

                                            Status = 3;
                                            CreateNumber = "";
                                        }

                                        if (DecriptGetRaffleNumbers < 1)
                                            DecriptGetRaffleNumbers += 1;
                                        else
                                            DecriptGetRaffleNumbers = 2;
                                    }
                                    else
                                    {
                                        if (DecriptGetRaffleNumbers == 1)
                                        {
                                            // Identificou que é pra verificar os números até achar a virgula de separação.

                                            if (encode.ElementAt(i).Equals(','))
                                            {
                                                //Se achou a virgula ele salva o número pre-salvo em uma variavel em uma lista

                                                _RaffleNumberDataBlock.Number = Convert.ToInt32(CreateNumber);
                                                _RaffleNumberDataBlock.Status = Status;
                                                _RaffleDataSingleBlock.Numbers.Add(_RaffleNumberDataBlock);
                                                _RaffleNumberDataBlock = new RaffleNumberDataBlock();

                                                //Repadroniza para o próximo numero se houver

                                                Status = 3;
                                                CreateNumber = "";
                                            }
                                            else
                                            {
                                                //Se exitir um @ após o número o seta como em reserva. e continua o fluxo

                                                if (encode.ElementAt(i).Equals('@'))
                                                    Status = 2;
                                                else
                                                    CreateNumber += encode.ElementAt(i); // Salva os caracteres que formam o número.
                                            }
                                        }

                                        if (DecriptGetRaffleNumbers == 2)
                                        {
                                            //O sistema encontrou a ')' entendendo que finalizou o registro dos número;

                                            //O $ indica o fim de um conjunto de dados ligados a um usuario;
                                            if (encode.ElementAt(i).Equals('$'))
                                            {
                                                _RaffleDataSingleBlock.SingleElementEndPosition = i;

                                                _RaffleDecryptObject.RaffleDataComponentFromEncode.Add(_RaffleDataSingleBlock);

                                                _RaffleDataSingleBlock = new RaffleDataSingleBlock(0, new List<RaffleNumberDataBlock>(), 0, 0);

                                                _RaffleNumberDataBlock = new RaffleNumberDataBlock();

                                                //Variaveis que guardas os valores pré salvos;

                                                CreateUserId = "";
                                                CreateNumber = "";
                                                Status = 3;

                                                //Zerando os controladores para o proxímo conjunto de dados;
                                                DecriptGetUserId = 0;
                                                DecriptGetRaffleNumbers = 0;
                                                DecriptStartElement = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return _RaffleDecryptObject;
        }

        public static RaffleDecryptObject CreateRuffleDecryptObjectByEncodeByUserId(string encode, int raffleId, int userId)
        {
            //Variaveis pré salvamento
            var CreateNumber = "";
            var CreateUserId = "";
            int Status = 3;

            //Controladores
            int DecriptStartElement = 0;
            int DecriptGetUserId = 0;
            int DecriptGetRaffleNumbers = 0;

            //Controle de elemento

            //Objetos de descriptação
            RaffleNumberDataBlock _RaffleNumberDataBlock = new RaffleNumberDataBlock();
            RaffleDataSingleBlock _RaffleDataSingleBlock = new RaffleDataSingleBlock(0, new List<RaffleNumberDataBlock>(), 0, 0);
            RaffleDecryptObject _RaffleDecryptObject = new RaffleDecryptObject(0, new List<RaffleDataSingleBlock>());

            //Talvez por tudo dentro de um Try-Catch

            if ((!String.IsNullOrEmpty(encode) && !encode.Equals("none")) || raffleId != 0)
            {
                _RaffleDecryptObject.RaffleId = Convert.ToInt32(raffleId);

                for (int i = 0; i < encode.Length; i++)
                {
                    if (encode.ElementAt(i).Equals('|'))
                    {
                        _RaffleDataSingleBlock.SingleElementStartPosition = i;

                        if (DecriptStartElement < 1)
                            DecriptStartElement += 1;
                        else
                            DecriptStartElement = 0;
                    }
                    else
                    {
                        if (DecriptStartElement == 1)
                        {
                            if (encode.ElementAt(i).Equals('*'))
                            {
                                if (DecriptGetUserId < 1)
                                    DecriptGetUserId += 1;
                                else
                                {
                                    _RaffleDataSingleBlock.ParticipantId = Convert.ToInt32(CreateUserId);
                                    DecriptGetUserId = 2;
                                }
                            }
                            else
                            {
                                if (DecriptGetUserId == 1)
                                    CreateUserId += encode.ElementAt(i); // Montando o Id do usuário

                                if (DecriptGetUserId == 2)
                                {
                                    //Já encontrou e salvou o id do usuário e começa a ver seus números e status dos mesmos

                                    if (encode.ElementAt(i).Equals('(') || encode.ElementAt(i).Equals(')'))
                                    {
                                        if (encode.ElementAt(i).Equals(')'))
                                        {
                                            //Identificou que todos os números já foram selecionados e salvos.
                                            _RaffleNumberDataBlock.Number = Convert.ToInt32(CreateNumber);
                                            _RaffleNumberDataBlock.Status = Status;
                                            _RaffleDataSingleBlock.Numbers.Add(_RaffleNumberDataBlock);
                                            _RaffleNumberDataBlock = new RaffleNumberDataBlock();

                                            Status = 3;
                                            CreateNumber = "";
                                        }

                                        if (DecriptGetRaffleNumbers < 1)
                                            DecriptGetRaffleNumbers += 1;
                                        else
                                            DecriptGetRaffleNumbers = 2;
                                    }
                                    else
                                    {
                                        if (DecriptGetRaffleNumbers == 1)
                                        {
                                            // Identificou que é pra verificar os números até achar a virgula de separação.

                                            if (encode.ElementAt(i).Equals(','))
                                            {
                                                //Se achou a virgula ele salva o número pre-salvo em uma variavel em uma lista

                                                _RaffleNumberDataBlock.Number = Convert.ToInt32(CreateNumber);
                                                _RaffleNumberDataBlock.Status = Status;
                                                _RaffleDataSingleBlock.Numbers.Add(_RaffleNumberDataBlock);
                                                _RaffleNumberDataBlock = new RaffleNumberDataBlock();

                                                //Repadroniza para o próximo numero se houver

                                                Status = 3;
                                                CreateNumber = "";
                                            }
                                            else
                                            {
                                                //Se exitir um @ após o número o seta como em reserva. e continua o fluxo

                                                if (encode.ElementAt(i).Equals('@'))
                                                    Status = 2;
                                                else
                                                    CreateNumber += encode.ElementAt(i); // Salva os caracteres que formam o número.
                                            }
                                        }

                                        if (DecriptGetRaffleNumbers == 2)
                                        {
                                            //O sistema encontrou a ')' entendendo que finalizou o registro dos número;

                                            //O $ indica o fim de um conjunto de dados ligados a um usuario;
                                            if (encode.ElementAt(i).Equals('$'))
                                            {
                                                _RaffleDataSingleBlock.SingleElementEndPosition = i;

                                                if (_RaffleDataSingleBlock.ParticipantId == userId)
                                                {
                                                    _RaffleDecryptObject.RaffleDataComponentFromEncode.Add(_RaffleDataSingleBlock);
                                                }

                                                _RaffleDataSingleBlock = new RaffleDataSingleBlock(0, new List<RaffleNumberDataBlock>(), 0, 0);

                                                _RaffleNumberDataBlock = new RaffleNumberDataBlock();

                                                //Variaveis que guardas os valores pré salvos;

                                                CreateUserId = "";
                                                CreateNumber = "";
                                                Status = 3;

                                                //Zerando os controladores para o proxímo conjunto de dados;
                                                DecriptGetUserId = 0;
                                                DecriptGetRaffleNumbers = 0;
                                                DecriptStartElement = 0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return _RaffleDecryptObject;
        }

        public static string CreateRaffleDecodeStringFromDecryptObject(RaffleDecryptObject decryptObject)
        {
            var newEncode = "";
            if(decryptObject.RaffleDataComponentFromEncode.Count() > 0)
            {
                var block = "";
                foreach(var participantPersonalData in decryptObject.RaffleDataComponentFromEncode)
                {
                    block += "|*";
                    block += participantPersonalData.ParticipantId.ToString() + "*(";
                    int count = 1;
                    foreach (var participantNumberData in participantPersonalData.Numbers)
                    {
                        var MAX = participantPersonalData.Numbers.Count();
                        
                        if (participantNumberData.Status == 2)
                        {
                            block += participantNumberData.Number + "@";

                            if (MAX > 1 && count < MAX)
                                block += ",";
                        }
                        else
                        {
                            block += participantNumberData.Number;

                            if (MAX > 1 && count < MAX)
                                block += ",";
                        }

                        count += 1;
                    }

                    block += ")$";
                }

                newEncode += block;
            }

            return newEncode;
        }
    }
}
