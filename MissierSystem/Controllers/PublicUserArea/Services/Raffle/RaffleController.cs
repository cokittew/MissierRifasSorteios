using Microsoft.AspNetCore.Mvc;
using MissierSystem.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses;
using MissierSystem.Models.Platform.Services.Raffle;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.Home.Services.Raffle;
using MissierSystem.Service.PlatformServices.Raffle;
using MercadoPago.Client.Common;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Http;
using MissierSystem.Models.Platform.Services.Raffle.WorkClasses.RaffleParticipant;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using Newtonsoft.Json;
using MissierSystem.Service.TokenServices;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using MissierSystem.Service.LocalDataBase;
using MissierSystem.Service.Automatization;
using System.Globalization;
using MissierSystem.Models.GeneralModels.Models.UserExtraModels;
using MissierSystem.Service.MercadoPago;
using MissierSystem.Service.Telegram;

namespace MissierSystem.Controllers.PublicUserArea.Services.Raffle
{
    public class RaffleController : Controller
    {
        private readonly UserClienteDataContext _context;

        public RaffleController(UserClienteDataContext context)
        {
            _context = context;
        }

        private int? IsAutenticated()
        {
            var id = HttpContext.Session.GetInt32("UserLogId");
            if (id == 0 || id == null)
                return 0;

            var user = _context.UserBasic.Where(e => e.Id == id).Select(e=> new UserBasic() {Id = e.Id, SignatureActive = e.SignatureActive }).FirstOrDefault();

            if (user == null || (user != null && !user.SignatureActive))
            {
                HttpContext.Session.SetInt32("UserLogId", 0);
                return 0;
            }

            return user.Id;
        }

        public async Task<IActionResult> MainPage()
        {
            //var pay = new PaymentResult()
            //{
            //    id = "4654564564",
            //    external_reference = "GisYT68pkYLi26ThqJZvWFY7Q",
            //    status = "approved"


            //};
            //var paymentSystemResult = "failed";
            //if (pay != null)
            //{
            //    var register = _context.UserPaymentRegister.Where(e => e.Reference == pay.external_reference && e.Removed == false)
            //        .Select(e => new UserPaymentRegister()
            //        {
            //            Id = e.Id,
            //            IdBasicUser = e.IdBasicUser,
            //            FinalStatus = e.FinalStatus,
            //            NumberQuantity = e.NumberQuantity
            //        }).FirstOrDefault();

            //    if (register != null)
            //    {
            //        if (pay.status == "approved")
            //        {
            //            var user = _context.UserBasic.Where(e => e.Id == register.IdBasicUser && !e.Removed)
            //                .Select(e => new UserBasic()
            //                {
            //                    Id = e.Id,
            //                    UserNumberBag = e.UserNumberBag,
            //                }).FirstOrDefault();

            //            if (user != null)
            //            {
            //                user.UserNumberBag += register.NumberQuantity;
            //                register.FinalStatus = "approved";
            //                register.Removed = true;

            //                try
            //                {
            //                    _context.Database.BeginTransaction();

            //                    _context.UserPaymentRegister.Update(register);
            //                    _context.Attach(register).Property(e => e.FinalStatus).IsModified = true;
            //                    await _context.SaveChangesAsync();
            //                    _context.UserPaymentRegister.Update(register);
            //                    _context.Attach(register).Property(e => e.Removed).IsModified = true;
            //                    await _context.SaveChangesAsync();

            //                    _context.UserBasic.Update(user);
            //                    _context.Attach(user).Property(e => e.UserNumberBag).IsModified = true;
            //                    await _context.SaveChangesAsync();

            //                    _context.Database.CommitTransaction();
            //                    paymentSystemResult = pay.status;
            //                }
            //                catch (Exception) { _context.Database.RollbackTransaction(); }
            //            }
            //        }
            //        else if (pay.status == "pending")
            //        {
            //            paymentSystemResult = pay.status;
            //            register.FinalStatus = "pending";
            //            _context.Attach(register).Property(e => e.FinalStatus).IsModified = true;
            //            register.FinalStatus = pay.status;

            //            try
            //            {
            //                _context.UserPaymentRegister.Update(register);
            //                await _context.SaveChangesAsync();

            //                paymentSystemResult = pay.status;
            //            }
            //            catch (Exception) { }
            //        }
            //    }
            //}

            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            return View();
        }

        public IActionResult RaffleList(string code = "", string nameNick = "", string type="", string answer = "")
        {
            ViewBag.alert = answer;

            var idUser = IsAutenticated();

            List<PlatformServiceRaffle> raffleList = new List<PlatformServiceRaffle>();
            var status = 1;

            if (idUser != 0 && idUser != null)
            {
                if (!String.IsNullOrEmpty(type) && type != "0")
                    status = Convert.ToInt32(type);

                if (!String.IsNullOrEmpty(code))
                    raffleList = _context.PlataformServiceRaffle.Where(e => e.Identity.Equals(code) && e.IdBasicUser != idUser && e.RaffleStatus == status)
                        .Select(e=> new PlatformServiceRaffle() {
                            Id = e.Id,
                            RaffleStatus = e.RaffleStatus,
                            RaffleEndDate = e.RaffleEndDate,
                            RaffleStartDate = e.RaffleStartDate,
                            RaffleName = e.RaffleName,
                            RaffleNumberResult = e.RaffleNumberResult,
                            RaffleMaxNumber = e.RaffleMaxNumber,
                            RaffleNumbersValue = e.RaffleNumbersValue,
                            RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                            RaffleCloseOption = e.RaffleCloseOption,
                            Identity = e.Identity,
                            RaffleType = e.RaffleType
                            
                        }).ToList();

                else if (!String.IsNullOrEmpty(nameNick))
                {
                    UserBasicInfo userBasicInfo = new UserBasicInfo();
                    userBasicInfo = _context.UserBasicInfo.Where(e => e.NickName.Equals(nameNick) && e.IdBasicUser != idUser).FirstOrDefault();

                    if (userBasicInfo != null)
                        raffleList = _context.PlataformServiceRaffle.Where(e => e.IdBasicUser == userBasicInfo.IdBasicUser && e.IdBasicUser != idUser && e.RaffleStatus == status).Take(15)
                            .Select(e => new PlatformServiceRaffle()
                                {
                                    Id = e.Id,
                                    RaffleStatus = e.RaffleStatus,
                                    RaffleEndDate = e.RaffleEndDate,
                                    RaffleStartDate = e.RaffleStartDate,
                                    RaffleName = e.RaffleName,
                                    RaffleNumberResult = e.RaffleNumberResult,
                                    RaffleMaxNumber = e.RaffleMaxNumber,
                                    RaffleNumbersValue = e.RaffleNumbersValue,
                                    RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                                    RaffleCloseOption = e.RaffleCloseOption,
                                    Identity = e.Identity,
                                    RaffleType = e.RaffleType

                            }).ToList();
                    else
                        raffleList = _context.PlataformServiceRaffle.Where(e => e.RaffleName.Contains(nameNick) && e.IdBasicUser != idUser && e.RaffleStatus == status)
                            .Take(15)
                            .Select(e => new PlatformServiceRaffle()
                            {
                                Id = e.Id,
                                RaffleStatus = e.RaffleStatus,
                                RaffleEndDate = e.RaffleEndDate,
                                RaffleName = e.RaffleName,
                                RaffleNumberResult = e.RaffleNumberResult,
                                RaffleMaxNumber = e.RaffleMaxNumber,
                                RaffleNumbersValue = e.RaffleNumbersValue,
                                RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                                RaffleCloseOption = e.RaffleCloseOption,
                                Identity = e.Identity,
                                RaffleType = e.RaffleType

                            }).ToList();
                }
                else
                    raffleList = _context.PlataformServiceRaffle.Where(e => e.IdBasicUser != idUser && e.RaffleStatus == status)
                        .Take(15)
                        .Select(e => new PlatformServiceRaffle()
                        {
                            Id = e.Id,
                            RaffleStatus = e.RaffleStatus,
                            RaffleEndDate = e.RaffleEndDate,
                            RaffleStartDate = e.RaffleStartDate,
                            RaffleName = e.RaffleName,
                            RaffleNumberResult = e.RaffleNumberResult,
                            RaffleMaxNumber = e.RaffleMaxNumber,
                            RaffleNumbersValue = e.RaffleNumbersValue,
                            RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                            RaffleCloseOption = e.RaffleCloseOption,
                            Identity = e.Identity,
                            RaffleType = e.RaffleType

                        }).ToList();

                foreach (var raffle in raffleList)
                {
                    if (DateTime.Now.Date.CompareTo(raffle.RaffleStartDate.Date) >= 0 && raffle.RaffleStatus == 2)
                    {
                        raffle.RaffleStatus = 1;
                        _context.PlataformServiceRaffle.Update(raffle);
                        _context.Attach(raffle).Property(e => e.RaffleStatus).IsModified = true;
                        _context.SaveChangesAsync();
                    }
                    else if (raffle.RaffleEndDate != null)
                    {
                        if (raffle.RaffleEndDate.Date.CompareTo(DateTime.Parse("2016-10-01").Date) != 0)
                        {
                            if (DateTime.Now.Date.CompareTo(raffle.RaffleEndDate) >= 0 && raffle.RaffleStatus == 1)
                            {
                                raffle.RaffleStatus = 3;
                                _context.PlataformServiceRaffle.Update(raffle);
                                _context.Attach(raffle).Property(e => e.RaffleStatus).IsModified = true;
                                _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;
            return View(raffleList.Where(e=> e.RaffleStatus == status).ToList());
        }

        public IActionResult RaffleUserDirectLink(string identity)
        {
            var idUser = IsAutenticated();

            if (!String.IsNullOrEmpty(identity))
            {
                var raffle = new PlatformServiceRaffle();
                 raffle = _context.PlataformServiceRaffle.Where(e => e.Identity.Equals(identity)).Select(e => new PlatformServiceRaffle()
                 {
                     Id = e.Id,
                     IdBasicUser = e.IdBasicUser,
                     RaffleStatus = e.RaffleStatus,
                     RaffleEndDate = e.RaffleEndDate,
                     RaffleStartDate = e.RaffleStartDate,
                     RaffleName = e.RaffleName,
                     RaffleNumberResult = e.RaffleNumberResult,
                     RaffleMaxNumber = e.RaffleMaxNumber,
                     RaffleNumbersValue = e.RaffleNumbersValue,
                     RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                     RaffleCloseOption = e.RaffleCloseOption,
                     Identity = e.Identity,
                     RaffleType = e.RaffleType

                 }).FirstOrDefault();

                if(raffle != null)
                {
                    if(raffle.IdBasicUser == idUser)
                    {
                        return RedirectToAction("RaffleMainPage", "PlataformServiceRaffle");
                    }

                    var raffleSelectedParameters = new Dictionary<string, string>()
                        {
                            {"raffleId",raffle.Id.ToString() }
                        };

                    if (raffle.RaffleCloseOption || raffle.RaffleStatus == 3 || raffle.RaffleStatus == 2)
                    {
                        return RedirectToAction("SelectedRaffleParticipateVisualisation", raffleSelectedParameters);
                    }

                    if (idUser == 0 || idUser == null)
                    {
                        var raffleSelectedToLoginParameters = new Dictionary<string, string>()
                        {
                            {"localuse", "DirectRaffleAccess"},
                            {"answer", "ToSeeRaffle" },
                            {"raffleId", raffle.Id.ToString() + ":" + raffle.IdBasicUser.ToString() }
                        };

                        return RedirectToAction("Login", "UserBasicInfo", raffleSelectedToLoginParameters);
                    }
                    else
                    {
                        if(raffle.RaffleType == 3)
                            return RedirectToAction("AmorinStyle", raffleSelectedParameters);
                        else
                            return RedirectToAction("SelectedRaffleParticipate", raffleSelectedParameters);
                    }
                }                
            }

            //var raffleListParams = new Dictionary<string, string>(){{"answer","NotFound" }};

            return RedirectToAction("MainPage");
        }

        public async Task<IActionResult> SelectedRaffleParticipate(string raffleId, string actionAfter = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = idUser;
            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            if (!String.IsNullOrEmpty(raffleId) && !raffleId.Equals("0"))
            {
                try
                {
                    var raffleInfo = _context.PlatformServiceRaffleInformations.Where(e => e.IdRaffle == Convert.ToInt32(raffleId))
                        .Select(e=> new PlatformServiceRaffleInformations() {
                        Id = e.Id,
                        IdRaffle = e.IdRaffle,
                        RaffleParticipant = e.RaffleParticipant
                        }).FirstOrDefault();

                    if (raffleInfo != null)
                    {
                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleInfo.IdRaffle)
                            .Select(e=> new PlatformServiceRaffle() {
                            Id = e.Id,
                            EndRaffleDay = e.EndRaffleDay,
                            RaffleStartDate = e.RaffleStartDate,
                            IdBasicUser = e.IdBasicUser,
                            RaffleName = e.RaffleName,
                            RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                            RaffleMaxNumber = e.RaffleMaxNumber,
                            RaffleNumbersValue = e.RaffleNumbersValue,
                            RaffleGeneralDescription = e.RaffleGeneralDescription,
                            RafflePremiationDescription = e.RafflePremiationDescription
                            })
                            .FirstOrDefault();

                        var userBasicInfo = _context.UserBasicInfo
                            .Select(e=> new UserBasicInfo() {
                            Id = e.Id,
                            IdBasicUser = e.IdBasicUser,
                            NickName = e.NickName,
                            }).FirstOrDefault(e => e.IdBasicUser == raffle.IdBasicUser);

                        var userSociaMedia = _context.UserSocialMidia.Where(e => e.IdBasicUser == raffle.IdBasicUser)
                            .Select(e=> new UserSocialMidia()
                            {
                                AnotherInformations = e.AnotherInformations,
                                Facebook = e.Facebook,
                                Instagram = e.Instagram,
                                Kwai = e.Kwai,
                                Reddit = e.Reddit,
                                TikTok = e.TikTok,
                                Twitter = e.Twitter,
                                WhatsApp = e.WhatsApp,
                                Youtube = e.Youtube
                            }).FirstOrDefault();

                        var userBank = _context.UserBankInformation.Where(e => e.IdBasicUser == raffle.IdBasicUser && e.Removed == false)
                            .Select(e=> new UserBankInformation() {
                            AccountOwnerCpf = e.AccountOwnerCpf,
                            AgenceAccount = e.AgenceAccount,
                            BankAccount = e.BankAccount,
                            BankCode = e.BankCode,
                            }).ToList();

                        var userPix = _context.UserPixInformation.Where(e => e.IdBasicUser == raffle.IdBasicUser)
                            .Select(e=> new UserPixInformation()
                            {
                            PixKey = e.PixKey,
                            PixKeyType = e.PixKeyType,
                            }).FirstOrDefault();

                        var receipts = _context.PlatformServiceRaffleFile.Where(e => e.IdBasicUser == (int)idUser && !e.Removed && e.IdRaffle == Convert.ToInt32(raffleId))
                            .Select(e=>new PlatformServiceRaffleFile()
                            {
                                NumberSequence = e.NumberSequence,
                                ReceiptFile = e.ReceiptFile,
                                Aproved = e.Aproved,
                                PreAproved = e.PreAproved
                            }).ToList();

                        List<int> dataReceipts = new List<int>();

                        foreach(var number in receipts)
                        {
                            var numbers = number.NumberSequence.Split(',');

                            foreach(var i in numbers)
                            {
                                dataReceipts.Add(Convert.ToInt32(i));
                            }
                        }

                        ViewBag.banksData = LocalDataBase.GetAllBank();
                        ViewBag.pixData = LocalDataBase.GetAllPixType();

                        if (!raffleInfo.RaffleParticipant.Equals("None") && !String.IsNullOrEmpty(raffleInfo.RaffleParticipant))
                        {
                            var raffleNumber = NumbersUserDescrypt.GetRaffleCurrentNumberSituation(raffleInfo.RaffleParticipant);
                            var raffleSeeUserParticipation = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncodeByUserId(raffleInfo.RaffleParticipant, raffle.Id, Convert.ToInt32(idUser));

                            if (raffleNumber.Count > 0 && raffle != null)
                            {
                                var avaliableNumber = raffle.RaffleMaxNumber;
                                var schaduleNumber = 0;
                                var unaviableNumber = 0;

                                foreach(var number in raffleNumber)
                                {
                                    if (number.Status == 2)
                                        schaduleNumber += 1;
                                    else
                                        unaviableNumber += 1;
                                }

                                avaliableNumber -= schaduleNumber + unaviableNumber;

                                if (userBasicInfo != null)
                                {
                                    var raffleNumberSelection = new NumberStatusTotal(receipts,
                                        raffleNumber, raffle.RaffleMaxNumber, userBasicInfo, 
                                        raffle, raffleInfo, userSociaMedia,avaliableNumber, schaduleNumber,unaviableNumber,
                                        userPix, userBank);

                                    raffleNumberSelection.ParticipantSelectNumberReceipt = dataReceipts;
                                    ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;

                                    if(raffleSeeUserParticipation.RaffleDataComponentFromEncode.Count() > 0)
                                    {
                                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers - raffleSeeUserParticipation.RaffleDataComponentFromEncode.FirstOrDefault().Numbers.Count();
                                        ViewBag.YourNumbers = raffleSeeUserParticipation.RaffleDataComponentFromEncode.FirstOrDefault().Numbers;
                                    }
                                    else
                                    {
                                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;
                                        ViewBag.YourNumbers = new List<RaffleNumberDataBlock>();
                                    }

                                    ViewBag.ActionFinalResult = actionAfter;

                                    return View(raffleNumberSelection);
                                }    
                            }
                        }

                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;
                        ViewBag.YourNumbers = new List<RaffleNumberDataBlock>();

                        var dataToFront = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(), new List<NumberStatus>(), raffle.RaffleMaxNumber, userBasicInfo,
                            raffle, raffleInfo, userSociaMedia, avaliableNumber: raffle.RaffleMaxNumber,
                            userBankInformation: userBank, userPixInformation: userPix);

                        dataToFront.ParticipantSelectNumberReceipt = dataReceipts;

                        return View(dataToFront);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }   
            }

            ViewBag.ActionFinalResult = "Fail";
            //Lança um erro já que todos os campos voltam como NULL
            return View(new NumberStatusTotal());
        }

        public async Task<IActionResult> AmorinStyle(string raffleId, string actionAfter = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = idUser;
            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            if (!String.IsNullOrEmpty(raffleId) && !raffleId.Equals("0"))
            {
                try
                {
                    var raffleInfo = _context.PlatformServiceRaffleInformations.Where(e => e.IdRaffle == Convert.ToInt32(raffleId))
                        .Select(e => new PlatformServiceRaffleInformations()
                        {
                            Id = e.Id,
                            IdRaffle = e.IdRaffle,
                            RaffleParticipant = e.RaffleParticipant
                        }).FirstOrDefault();

                    if (raffleInfo != null)
                    {
                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleInfo.IdRaffle)
                            .Select(e => new PlatformServiceRaffle()
                            {
                                Id = e.Id,
                                EndRaffleDay = e.EndRaffleDay,
                                RaffleStartDate = e.RaffleStartDate,
                                IdBasicUser = e.IdBasicUser,
                                RaffleName = e.RaffleName,
                                RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                                RaffleMaxNumber = e.RaffleMaxNumber,
                                RaffleNumbersValue = e.RaffleNumbersValue,
                                RaffleGeneralDescription = e.RaffleGeneralDescription,
                                RafflePremiationDescription = e.RafflePremiationDescription
                            })
                            .FirstOrDefault();

                        var userBasicInfo = _context.UserBasicInfo
                            .Select(e => new UserBasicInfo()
                            {
                                Id = e.Id,
                                IdBasicUser = e.IdBasicUser,
                                NickName = e.NickName,
                            }).FirstOrDefault(e => e.IdBasicUser == raffle.IdBasicUser);

                        var userSociaMedia = _context.UserSocialMidia.Where(e => e.IdBasicUser == raffle.IdBasicUser)
                            .Select(e => new UserSocialMidia()
                            {
                                AnotherInformations = e.AnotherInformations,
                                Facebook = e.Facebook,
                                Instagram = e.Instagram,
                                Kwai = e.Kwai,
                                Reddit = e.Reddit,
                                TikTok = e.TikTok,
                                Twitter = e.Twitter,
                                WhatsApp = e.WhatsApp,
                                Youtube = e.Youtube
                            }).FirstOrDefault();

                        var userBank = _context.UserBankInformation.Where(e => e.IdBasicUser == raffle.IdBasicUser && e.Removed == false)
                            .Select(e => new UserBankInformation()
                            {
                                AccountOwnerCpf = e.AccountOwnerCpf,
                                AgenceAccount = e.AgenceAccount,
                                BankAccount = e.BankAccount,
                                BankCode = e.BankCode,
                            }).ToList();

                        var userPix = _context.UserPixInformation.Where(e => e.IdBasicUser == raffle.IdBasicUser)
                            .Select(e => new UserPixInformation()
                            {
                                PixKey = e.PixKey,
                                PixKeyType = e.PixKeyType,
                            }).FirstOrDefault();

                        var receipts = _context.PlatformServiceRaffleFile.Where(e => e.IdBasicUser == (int)idUser && !e.Removed && e.IdRaffle == Convert.ToInt32(raffleId))
                            .Select(e => new PlatformServiceRaffleFile()
                            {
                                NumberSequence = e.NumberSequence,
                                ReceiptFile = e.ReceiptFile,
                                Aproved = e.Aproved,
                                PreAproved = e.PreAproved
                            }).ToList();

                        List<int> dataReceipts = new List<int>();

                        foreach (var number in receipts)
                        {
                            var numbers = number.NumberSequence.Split(',');

                            foreach (var i in numbers)
                            {
                                dataReceipts.Add(Convert.ToInt32(i));
                            }
                        }

                        ViewBag.banksData = LocalDataBase.GetAllBank();
                        ViewBag.pixData = LocalDataBase.GetAllPixType();

                        if (!raffleInfo.RaffleParticipant.Equals("None") && !String.IsNullOrEmpty(raffleInfo.RaffleParticipant))
                        {
                            var raffleNumber = NumbersUserDescrypt.GetRaffleCurrentNumberSituation(raffleInfo.RaffleParticipant);
                            var raffleSeeUserParticipation = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncodeByUserId(raffleInfo.RaffleParticipant, raffle.Id, Convert.ToInt32(idUser));

                            if (raffleNumber.Count > 0 && raffle != null)
                            {
                                var avaliableNumber = raffle.RaffleMaxNumber;
                                var schaduleNumber = 0;
                                var unaviableNumber = 0;

                                foreach (var number in raffleNumber)
                                {
                                    if (number.Status == 2)
                                        schaduleNumber += 1;
                                    else
                                        unaviableNumber += 1;
                                }

                                avaliableNumber -= schaduleNumber + unaviableNumber;

                                if (userBasicInfo != null)
                                {
                                    var raffleNumberSelection = new NumberStatusTotal(receipts,
                                        raffleNumber, raffle.RaffleMaxNumber, userBasicInfo,
                                        raffle, raffleInfo, userSociaMedia, avaliableNumber, schaduleNumber, unaviableNumber,
                                        userPix, userBank);

                                    raffleNumberSelection.ParticipantSelectNumberReceipt = dataReceipts;
                                    ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;

                                    if (raffleSeeUserParticipation.RaffleDataComponentFromEncode.Count() > 0)
                                    {
                                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers - raffleSeeUserParticipation.RaffleDataComponentFromEncode.FirstOrDefault().Numbers.Count();

                                        var toSelect = raffleSeeUserParticipation.RaffleDataComponentFromEncode.FirstOrDefault().Numbers;
                                        var AmorinList = new List<int>();

                                       
                                        for (int po = 1; po < 100; po += 4)
                                        {
                                            if (toSelect.Any(e => e.Number == po))
                                                AmorinList.Add(po);
                                        }

                                        ViewBag.YourNumbers = AmorinList;
                                        ViewBag.YourNumbersComplete = toSelect;
                                    }
                                    else
                                    {
                                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;
                                        ViewBag.YourNumbers = new List<int>();
                                    }

                                    ViewBag.ActionFinalResult = actionAfter;

                                    return View(raffleNumberSelection);
                                }
                            }
                        }

                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;
                        ViewBag.YourNumbers = new List<int>();

                        var dataToFront = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(), new List<NumberStatus>(), raffle.RaffleMaxNumber, userBasicInfo,
                            raffle, raffleInfo, userSociaMedia, avaliableNumber: raffle.RaffleMaxNumber,
                            userBankInformation: userBank, userPixInformation: userPix);

                        dataToFront.ParticipantSelectNumberReceipt = dataReceipts;

                        return View(dataToFront);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            ViewBag.ActionFinalResult = "Fail";
            //Lança um erro já que todos os campos voltam como NULL
            return View(new NumberStatusTotal());
        }

        public async Task<IActionResult> SelectedRaffleParticipateVisualisation(string raffleId, string actionAfter = "")
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            ViewBag.Culture = culture;

            if (!String.IsNullOrEmpty(raffleId) && !raffleId.Equals("0"))
            {
                try
                {
                    var raffleInfo = _context.PlatformServiceRaffleInformations.FirstOrDefault(e => e.IdRaffle == Convert.ToInt32(raffleId));

                    if (raffleInfo != null)
                    {
                        var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleInfo.IdRaffle)
                            .Select(e => new PlatformServiceRaffle()
                            {
                                Id = e.Id,
                                EndRaffleDay = e.EndRaffleDay,
                                RaffleStartDate = e.RaffleStartDate,
                                IdBasicUser = e.IdBasicUser,
                                RaffleName = e.RaffleName,
                                RaffleUserMaxNumbers = e.RaffleUserMaxNumbers,
                                RaffleMaxNumber = e.RaffleMaxNumber,
                                RaffleNumbersValue = e.RaffleNumbersValue

                            }).FirstOrDefault();

                        var userBasicInfo = _context.UserBasicInfo
                            .Select(e => new UserBasicInfo()
                            {
                                Id = e.Id,
                                IdBasicUser = e.IdBasicUser,
                                NickName = e.NickName,
                            }).FirstOrDefault(e => e.IdBasicUser == raffle.IdBasicUser);

                        var userSociaMedia = _context.UserSocialMidia.Where(e => e.IdBasicUser == raffle.IdBasicUser)
                            .Select(e => new UserSocialMidia()
                            {
                                AnotherInformations = e.AnotherInformations,
                                Facebook = e.Facebook,
                                Instagram = e.Instagram,
                                Kwai = e.Kwai,
                                Reddit = e.Reddit,
                                TikTok = e.TikTok,
                                Twitter = e.Twitter,
                                WhatsApp = e.WhatsApp,
                                Youtube = e.Youtube
                            }).FirstOrDefault();

                        var userBank = _context.UserBankInformation.Where(e => e.IdBasicUser == raffle.IdBasicUser && e.Removed == false)
                            .Select(e => new UserBankInformation()
                            {
                                AccountOwnerCpf = e.AccountOwnerCpf,
                                AgenceAccount = e.AgenceAccount,
                                BankAccount = e.BankAccount,
                                BankCode = e.BankCode,
                            }).ToList();

                        var userPix = _context.UserPixInformation.Where(e => e.IdBasicUser == raffle.IdBasicUser)
                            .Select(e => new UserPixInformation()
                            {
                                PixKey = e.PixKey,
                                PixKeyType = e.PixKeyType,
                            }).FirstOrDefault();

                        ViewBag.banksData = LocalDataBase.GetAllBank();
                        ViewBag.pixData = LocalDataBase.GetAllPixType();

                        if (!raffleInfo.RaffleParticipant.Equals("None") && !String.IsNullOrEmpty(raffleInfo.RaffleParticipant))
                        {
                            if (raffleInfo.RaffleParticipant.ElementAt(0) == 'M')
                            {
                                //Melhorar desempenho
                                var list = _context.PlatformGuestReservedNumber.Where(e => e.IdRaffle == raffleInfo.IdRaffle && !e.Removed)
                                    .Select(e=> new PlatformGuestReservedNumber()
                                    {
                                        Id = e.Id,
                                        Number = e.Number
                                    })
                                    .ToList();

                                var avaliableNumber = raffle.RaffleMaxNumber;
                                var schaduleNumber = 0;
                                var unaviableNumber = list.Count();

                                avaliableNumber -= unaviableNumber;

                                var raffleNumber = new List<NumberStatus>();
                                foreach (var item in list)
                                {
                                    var itemSave = new NumberStatus(item.Number, 3);
                                    raffleNumber.Add(itemSave);
                                }

                                if (userBasicInfo != null)
                                {
                                    var raffleNumberSelection = new NumberStatusTotal(new List<PlatformServiceRaffleFile>(),
                                        raffleNumber, raffle.RaffleMaxNumber, userBasicInfo,
                                        raffle, raffleInfo, userSociaMedia, avaliableNumber, schaduleNumber, unaviableNumber,
                                        userPix, userBank);

                                    ViewBag.MaxUserCanSelect = 0;
                                    ViewBag.YourNumbers = 0;
                                    ViewBag.ActionFinalResult = actionAfter;

                                    return View(raffleNumberSelection);
                                }


                            }
                            else
                            {
                                var raffleNumber = NumbersUserDescrypt.GetRaffleCurrentNumberSituation(raffleInfo.RaffleParticipant);

                                if (raffleNumber.Count > 0 && raffle != null)
                                {
                                    var avaliableNumber = raffle.RaffleMaxNumber;
                                    var schaduleNumber = 0;
                                    var unaviableNumber = 0;

                                    foreach (var number in raffleNumber)
                                    {
                                        if (number.Status == 2)
                                            schaduleNumber += 1;
                                        else
                                            unaviableNumber += 1;
                                    }

                                    avaliableNumber -= schaduleNumber + unaviableNumber;

                                    if (userBasicInfo != null)
                                    {
                                        var raffleNumberSelection = new NumberStatusTotal( new List<PlatformServiceRaffleFile>(),
                                            raffleNumber, raffle.RaffleMaxNumber, userBasicInfo,
                                            raffle, raffleInfo, userSociaMedia, avaliableNumber, schaduleNumber, unaviableNumber,
                                            userPix, userBank);

                                        ViewBag.MaxUserCanSelect = 0;

                                        ViewBag.YourNumbers = 0;
                                        ViewBag.ActionFinalResult = actionAfter;

                                        return View(raffleNumberSelection);
                                    }
                                }
                            }       
                        }

                        ViewBag.MaxUserCanSelect = raffle.RaffleUserMaxNumbers;

                        return View(new NumberStatusTotal(new List<PlatformServiceRaffleFile>(), new List<NumberStatus>(), raffle.RaffleMaxNumber, userBasicInfo,
                            raffle, raffleInfo, userSociaMedia, avaliableNumber: raffle.RaffleMaxNumber,
                            userBankInformation: userBank, userPixInformation: userPix));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            ViewBag.ActionFinalResult = "Fail";
            return View(new NumberStatusTotal());
        }

        [HttpPost]
        public async Task<IActionResult> RaffleSendPaymentProof(IFormCollection raffleReceiptForm)
        {
            var idUser = IsAutenticated();
            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var idRaffle = Convert.ToInt32(raffleReceiptForm["raffleId"]);

            var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == idRaffle && !e.Removed)
                .Select(e => new PlatformServiceRaffle() { Id = e.Id, RaffleType = e.RaffleType }).FirstOrDefault();

            try
            {
                //Adicionar validação de valores do form pro brack
                var n = raffleReceiptForm["receiptValue"].ToString().Replace(".", ",");
                var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                var culture = CultureInfo.CreateSpecificCulture("pt-BR");
                decimal receiptValue = 0;
                bool canConvert = decimal.TryParse(n, style, culture, out decimal number);
                if (canConvert)
                    receiptValue = number;

                var numberSequence = raffleReceiptForm["receiptNumberReference"];
                var indexType = raffleReceiptForm["raffleId"];
                var file = raffleReceiptForm["base64"];

                var type = file.ToString().Split(';');
                var typeFile = type[0].ToString().Split('/');

                PlatformServiceRaffleFile newReceiptFile = new PlatformServiceRaffleFile()
                {
                    IdBasicUser = (int)idUser,
                    IdRaffle = idRaffle,
                    NumberSequence = numberSequence,
                    IndexType = indexType,
                    ReceiptFile = file,
                    ReceiptValue = receiptValue,
                    Aproved = false,
                    PreAproved = true,
                    Removed = false,
                    BeginningDate = DateTime.Now
                };

                _context.Database.BeginTransaction();

                if (typeFile[0] != "data:image")
                {
                    throw new ArgumentException("Não foi achada reserva de números nessa rifa");
                }

                var reservation = _context.PlatformUserReservedNumber.Where(e => e.IdRaffle == idRaffle && e.IdBasicUser == idUser)
                    .Select(e => new PlatformUserReservedNumber() { Id = e.Id, Number = e.Number }).ToList();
                var numbersSelected = numberSequence.ToString().Split(',');
                
                if (reservation.Count() > 0)
                {
                    _context.PlatformServiceRaffleFile.Add(newReceiptFile);
                    await _context.SaveChangesAsync();
                    var numbers = "";
                    foreach (var r in reservation)
                    {
                        var canRemove = false;
                        foreach(var s in numbersSelected)
                        {
                            if(s == r.Number.ToString())
                            {
                                numbers += s + ", ";

                                canRemove = true;
                                break;
                            }
                        }

                        if (canRemove)
                        {
                            _context.PlatformUserReservedNumber.Remove(r);
                            await _context.SaveChangesAsync();
                        }                       
                    }

                    _context.Database.CommitTransaction();

                    var OwnerRaffleId = _context.PlataformServiceRaffle.Where(e => e.Id == idRaffle).Select(e => new PlatformServiceRaffle() 
                    { IdBasicUser = e.IdBasicUser, RaffleName = e.RaffleName, Identity = e.Identity }).FirstOrDefault();

                    if(OwnerRaffleId != null)
                    {
                        var participant = _context.UserBasic.Where(e => e.Id == idUser && !e.Removed).Select(e => new UserBasic() { IdTelegram = e.IdTelegram }).FirstOrDefault();
                        var raffleOwner = _context.UserBasic.Where(e => e.Id == OwnerRaffleId.IdBasicUser && !e.Removed).Select(e => new UserBasic() { IdTelegram = e.IdTelegram }).FirstOrDefault();

                        var SendReceiptRaffle = new ToUserInformationsTelegram()
                        {
                            Action = "Envio de Comprovante",
                            NumberReference = numbers,
                            RaffleCode = OwnerRaffleId.Identity,
                            RaffleName = OwnerRaffleId.RaffleName,
                            ValueOperation = receiptValue.ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR")),
                            OcurrData = DateTime.Now.AddHours(-3).ToShortDateString(),
                        };

                        var t = new TelegramAction();
                        await t.SendDefaultMessageStructure(participant.IdTelegram, SendReceiptRaffle, TelegramMessageAction.aReceiptHasBeenSended);
                        await t.SendDefaultMessageStructure(raffleOwner.IdTelegram, SendReceiptRaffle, TelegramMessageAction.aReceiptHasBeenSended);

                    }

                    var dic = new Dictionary<string, string>()
                    {
                        {"raffleId", idRaffle.ToString() },
                        {"actionAfter","FileSaved"}
                    };

                    if (raffle.RaffleType == 3)
                        return RedirectToAction("AmorinStyle", dic);
                    else
                        return RedirectToAction("SelectedRaffleParticipate", dic);
                }

                throw new ArgumentException("Não foi achada reserva de números nessa rifa");

            }
            catch (Exception e)
            {
                _context.Database.RollbackTransaction();
                var dic = new Dictionary<string, string>()
                {
                    {"raffleId", idRaffle.ToString() },
                    {"actionAfter","FileSavedFail"}
                };

                if (raffle.RaffleType == 3)
                    return RedirectToAction("AmorinStyle", dic);
                else
                    return RedirectToAction("SelectedRaffleParticipate", dic);
            }   
        }

        [HttpPost]
        public async Task<IActionResult> RafflePrepareToBuy(IFormCollection raffleNumberForm)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var stringListNumber = raffleNumberForm["serverSendNumbers"].ToString();

            List<int> numberList = new List<int>();

            if (!String.IsNullOrEmpty(stringListNumber) && !stringListNumber.Equals("0"))
            {
                string numberFind = "";
                for (int i = 0; i < stringListNumber.Length; i++)
                {
                    if (!stringListNumber.ElementAt(i).Equals(','))
                        numberFind += stringListNumber.ElementAt(i);
                    else
                    {
                        numberList.Add(Convert.ToInt32(numberFind));
                        numberFind = "";
                    }
                }

                if(!String.IsNullOrEmpty(numberFind))
                    numberList.Add(Convert.ToInt32(numberFind));
            }

            if (numberList.Count() <= 0)
                return View(); // Erro desconhecido ao gerar a lista de numeros

            var valueTotal = numberList.Count() * Convert.ToDecimal(raffleNumberForm["numbersValue"]);

            var raffleId = Convert.ToInt32(raffleNumberForm["raffleId"]);

            var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == raffleId && !e.Removed)
                .Select(e => new PlatformServiceRaffle() { Id = e.Id, RaffleType = e.RaffleType }).FirstOrDefault();

            RaffleToPayPackage pack = new RaffleToPayPackage()
            {
                RaffleId = Convert.ToInt32(raffleNumberForm["raffleId"]),
                RaffleParticipantNumbers = numberList,
                RaffleTotalValueToPay = valueTotal,
                ParticipantType = "Guest", // Verificar/mudar isso
                ParticipantId = Convert.ToInt32(idUser)
            };

            var result = await RaffleStartUserNumberReservation(pack, idUser);

            if (result)
            {
                var dic = new Dictionary<string, string>()
                {
                    {"raffleId",raffleNumberForm["raffleId"] },
                    {"actionAfter","ReservationSuccess"}
                };

                if(raffle.RaffleType == 3)
                    return RedirectToAction("AmorinStyle", dic);
                else
                    return RedirectToAction("SelectedRaffleParticipate", dic);


            }
            else
            {
                var dic = new Dictionary<string, string>()
                {
                    {"raffleId",raffleNumberForm["raffleId"] },
                    {"actionAfter","ReservationFail"}
                };
                return RedirectToAction("SelectedRaffleParticipate", dic);
            }
        }

        private async Task<bool> RaffleStartUserNumberReservation(RaffleToPayPackage payerData, int? userId)
        {
            if(payerData != null && payerData.RaffleParticipantNumbers.Count() > 0 && payerData.ParticipantId != 0 && payerData.RaffleId != 0)
            {
                var raffleInformation = _context.PlatformServiceRaffleInformations
                    .Where(e => e.IdRaffle == payerData.RaffleId)
                    .Select(e=> new PlatformServiceRaffleInformations()
                    {
                        Id= e.Id,
                        RaffleParticipant = e.RaffleParticipant,
                    })
                    .FirstOrDefault();

                if (raffleInformation == null)
                    return false;

                var raffleNumber = NumbersUserDescrypt.CreateRuffleDecryptObjectByEncode(raffleInformation.RaffleParticipant, payerData.RaffleId);

                var blockAction = false;
                string numberStringToPutEncript = "";

                RaffleDataSingleBlock ParticipantExistentBlock = null;

                foreach (var selectNumberUser in payerData.RaffleParticipantNumbers)
                {
                    numberStringToPutEncript += "," + selectNumberUser.ToString() + "@";

                    foreach (var item in raffleNumber.RaffleDataComponentFromEncode)
                    {
                        foreach(var number in item.Numbers)
                        {
                            if (number.Number == selectNumberUser)
                            {
                                blockAction = true;
                                break;
                            }

                            if (item.ParticipantId == Convert.ToInt32(payerData.ParticipantId))
                            {
                                ParticipantExistentBlock = new RaffleDataSingleBlock();
                                ParticipantExistentBlock = item;
                            }
                        }

                        if (blockAction)
                            break;
                    }

                    if (blockAction)
                        break;
                }

                if (!blockAction)
                {
                    _context.Database.BeginTransaction();

                    if (ParticipantExistentBlock != null)
                    {
                        //Já possui numeros nesse sorteio

                        var blockSize = ParticipantExistentBlock.SingleElementEndPosition + 1 - ParticipantExistentBlock.SingleElementStartPosition;

                        var encode = raffleInformation.RaffleParticipant;
                        var myBlock = encode.Substring(ParticipantExistentBlock.SingleElementStartPosition, blockSize);

                        myBlock = myBlock.Insert(blockSize - 2, numberStringToPutEncript);

                        encode = encode.Remove(ParticipantExistentBlock.SingleElementStartPosition, blockSize);

                        encode = encode.Insert(ParticipantExistentBlock.SingleElementStartPosition, myBlock);

                        raffleInformation.RaffleParticipant = encode;
                    }
                    else
                    {
                        if (raffleInformation.RaffleParticipant.Equals("None"))
                            raffleInformation.RaffleParticipant = "";

                        var refixNumber = numberStringToPutEncript.Remove(0, 1);
                        var newBlock = "|*" + payerData.ParticipantId + "*(" + refixNumber + ")$";

                        raffleInformation.RaffleParticipant += newBlock;
                    }

                    try
                    {
                        var ok = _context.PlatformServiceRaffleInformations.Update(raffleInformation);
                        _context.Attach(raffleInformation).Property(e => e.RaffleParticipant).IsModified = true;
                        await _context.SaveChangesAsync();

                        if (ok.Entity != null)
                        {
                            foreach(var n in payerData.RaffleParticipantNumbers)
                            {
                                PlatformUserReservedNumber saveReserve = new PlatformUserReservedNumber()
                                {
                                    IdBasicUser = payerData.ParticipantId,
                                    IdRaffle = payerData.RaffleId,
                                    Number = n,
                                    BeginningDate = DateTime.Now.ToLocalTime()
                                };

                                _context.PlatformUserReservedNumber.Add(saveReserve);
                                await _context.SaveChangesAsync();
                            }

                            _context.Database.CommitTransaction();

                            var raffle = _context.PlataformServiceRaffle.Where(e => e.Id == payerData.RaffleId).Select(e => new PlatformServiceRaffle()
                            {
                                Id = e.Id,
                                RaffleName = e.RaffleName,
                                Identity = e.Identity,
                                RaffleNumbersValue = e.RaffleNumbersValue
                            }).FirstOrDefault();

                            var userB = _context.UserBasic.Where(e => e.Id == userId).Select(e => new UserBasic() { Id = e.Id, IdTelegram = e.IdTelegram }).FirstOrDefault();

                            if(raffle != null)
                            {
                                var numbers = "";
                                payerData.RaffleParticipantNumbers.ForEach(number => {
                                    numbers +=number.ToString() +  ", " ;
                                });

                                ToUserInformationsTelegram reservedNumber = new ToUserInformationsTelegram()
                                {
                                    Action = "Reserva de Número(s)",
                                    NumberReference = numbers,
                                    OcurrData = DateTime.Now.AddHours(-3).ToShortDateString(),
                                    RaffleName = raffle.RaffleName,
                                    RaffleCode = raffle.Identity,
                                    ValueOperation = (payerData.RaffleParticipantNumbers.Count() * raffle.RaffleNumbersValue).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"))


                                };
                                TelegramAction bot = new TelegramAction();
                                await bot.SendDefaultMessageStructure(userB.IdTelegram, reservedNumber, TelegramMessageAction.aNumberHasBeenReserved);

                            }

                            return true;
                        }else 
                            _context.Database.RollbackTransaction();
                    }
                    catch (Exception) { _context.Database.RollbackTransaction(); }
                }
            }

            return false; // Erro desconhecido impediu a verificação e registro da reserva dos números
        }
    }
}
