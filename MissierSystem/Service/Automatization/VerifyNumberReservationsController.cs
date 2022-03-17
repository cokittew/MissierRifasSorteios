using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.Platform.Services.Raffle;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses;
using MissierSystem.Service.PlatformServices.Raffle;
using MissierSystem.Service.Telegram;

namespace MissierSystem.Service.Automatization
{
    public class VerifyNumberReservationsController : IVerifyNumberReservationsController
    {
        private readonly UserClienteDataContext _context;

        public VerifyNumberReservationsController(UserClienteDataContext context)
        {
            _context = context;
        }

        public void VerifyParticipantNumberReservation()
        {
            var now = DateTime.Now.ToLocalTime();

            var reservationList = _context.PlatformUserReservedNumber.Where(e => !e.Removed && e.BeginningDate.Date != now.Date)
                .Select(re => new PlatformUserReservedNumber() {Id = re.Id, IdRaffle = re.IdRaffle, Number = re.Number, BeginningDate = re.BeginningDate, IdBasicUser = re.IdBasicUser }).ToList();

            foreach (var reservation in reservationList)
            {
                var timeDiference = now - reservation.BeginningDate;

                if (timeDiference.TotalHours >= 24)
                {
                    var raffle = _context.PlatformServiceRaffleInformations.Where(e => e.IdRaffle == reservation.IdRaffle && !e.Removed)
                        /*.Select(r => new PlatformServiceRaffleInformations() {Id = r.Id, IdRaffle = r.IdRaffle, RaffleParticipant = r.RaffleParticipant,  })*/.FirstOrDefault();

                    if (raffle != null)
                    {
                        var newCode = RaffleDataSerachAndUpdate(raffle.RaffleParticipant, raffle.IdRaffle, reservation.Number);

                        if (raffle.RaffleParticipant != newCode)
                        {
                            try
                            {
                                 _context.Database.BeginTransaction();

                                _context.PlatformUserReservedNumber.Remove(reservation);
                                 _context.SaveChanges();

                                raffle.RaffleParticipant = newCode;
                                _context.PlatformServiceRaffleInformations.Update(raffle);
                                _context.Attach(raffle).Property(e => e.RaffleParticipant).IsModified = true;
                                _context.SaveChanges();

                                _context.Database.CommitTransaction();

                                var userB = _context.UserBasic.Where(e => e.Id == reservation.IdBasicUser).Select(e => new UserBasic() { Id = e.Id, IdTelegram = e.IdTelegram }).FirstOrDefault();
                                var raffleNameIdentity = _context.PlataformServiceRaffle.Where(e => e.Id == reservation.IdRaffle).Select(e => new PlatformServiceRaffle() { Id = e.Id, RaffleName = e.RaffleName, Identity = e.Identity }).FirstOrDefault();

                                if (userB != null && raffleNameIdentity != null)
                                {

                                    ToUserInformationsTelegram reservedNumber = new ToUserInformationsTelegram()
                                    {
                                        Action = "Reserva de Número(s) (EXPIRADO)",
                                        NumberReference = reservation.Number.ToString(),
                                        OcurrData = DateTime.Now.AddHours(-3).ToShortDateString(),
                                        RaffleName = raffleNameIdentity.RaffleName,
                                        RaffleCode = raffleNameIdentity.Identity,
                                    };
                                    TelegramAction bot = new TelegramAction();

                                    bot.SendDefaultMessageStructure(userB.IdTelegram, reservedNumber, TelegramMessageAction.anumberHasBeenAutomaicalyOutReserved);
                                }
                            }
                            catch (Exception){ _context.Database.RollbackTransaction(); }
                        }
                    }
                }
            }
        }

        private string RaffleDataSerachAndUpdate(string raffleParticipantCode, int raffleId, int number)
        {
            var raffleDecriptObj = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncode(raffleParticipantCode, raffleId);

            var canBrak = false;
            int? raiz = null;
            int? raiz2 = null;
            if (raffleDecriptObj.RaffleDataComponentFromEncode.Count() > 0)
            {
                foreach (var participantPersonalData in raffleDecriptObj.RaffleDataComponentFromEncode)
                {
                    foreach (var participantNumberData in participantPersonalData.Numbers)
                    {
                        if (participantNumberData.Number == number)
                        {
                            raiz = raffleDecriptObj.RaffleDataComponentFromEncode.IndexOf(participantPersonalData);
                            raiz2 = raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers.IndexOf(participantNumberData);

                            canBrak = true;
                            break;
                        }   
                    }

                    if (canBrak)
                        break;
                }

                if(raiz != null && raiz2 != null)
                {
                    var t = raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers
                        .Remove(raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers[Convert.ToInt32(raiz2)]);

                    if(raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)].Numbers.Count <= 0)
                        raffleDecriptObj.RaffleDataComponentFromEncode.Remove(raffleDecriptObj.RaffleDataComponentFromEncode[Convert.ToInt32(raiz)]);
                }

                var newDecode = NumbersUserDescrypt.CreateRaffleDecodeStringFromDecryptObject(raffleDecriptObj);
                return newDecode;
            }

            return null;
        }
    }
}
