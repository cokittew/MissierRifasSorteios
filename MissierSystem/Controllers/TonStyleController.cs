using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.Platform.Enums;
using MissierSystem.Models.TonModality;
using MissierSystem.Service.MercadoPago;
using MissierSystem.Service.TokenServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MissierSystem.Controllers
{
    public class TonStyleController : Controller
    {

        private readonly UserClienteDataContext _context;
        private IConfiguration _configuration;

        public TonStyleController(UserClienteDataContext context, IConfiguration Configuration)
        {
            _context = context;
            _configuration = Configuration;
        }

        private int? IsAutenticated(bool admin = false)
        {
            var id = HttpContext.Session.GetInt32("UserLogId");
            if (id == 0 || id == null)
                return 0;

            var user = _context.UserBasicInfo.Where(e => e.IdBasicUser == id)
                .Select(e => new UserBasicInfo() { Id = e.Id, Email = e.Email, FullName = e.FullName }).FirstOrDefault();

            if(user != null)
            {
                bool worker;
                if (admin)
                    worker = _context.MissierWorker.Any(e => (!e.Removed && e.HasPermission && e.HasPermissionCollaborator) && (e.FullName == user.FullName && e.Email == user.Email));
                else
                     worker =_context.MissierWorker.Any(e => (!e.Removed && e.HasPermission) && (e.FullName == user.FullName && e.Email == user.Email));

                if (worker)
                    return id;
                else
                {
                    HttpContext.Session.SetInt32("UserLogId", 0);
                    return 0;
                }
            }
            else
            {
                HttpContext.Session.SetInt32("UserLogId", 0);
                return 0;
            }

        }

        public IActionResult RefreshSuccessPage()
        {
            var c = new Dictionary<string, string>(){ {"hasRegister", "payed" } };
            return RedirectToAction("InicialPage", c);
        }

        public IActionResult RefreshPage()
        {
            var c = new Dictionary<string, string>() { { "hasRegister", "registered" } };
            return RedirectToAction("InicialPage", c);
        }

        public IActionResult InicialPage(string hasRegister, [FromQuery(Name = "convite")] string collaboratorCode = "")
        {

            if (!String.IsNullOrEmpty(collaboratorCode) && collaboratorCode.Length == 8)
            {
                var exists = _context.RaffleBusinessCollaborator.Any(e => !e.Removed && e.PersonalCode.ToLower() == collaboratorCode.ToLower());
                if (!exists)
                    collaboratorCode = "";
                else
                    HttpContext.Session.SetString("collaboratorCode", collaboratorCode);
            }

            ViewBag.hasRegister = hasRegister;
            ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");
            var r = _context.RaffleBusinessRaffle.Where(e => !e.Removed).ToList();
            return View(r);
        }

        public IActionResult InicialPageAdmin()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");
            var r = _context.RaffleBusinessRaffle.Where(e => !e.Removed && e.IdBasicUser == idUser).ToList();
            return View(r);
        }

        public IActionResult SelectAndReserveIndex(int id, [FromQuery(Name = "convite")]string collaboratorCode = "")
        {
            if(id == 0) { return RedirectToAction("InicialPage"); }

            if (!String.IsNullOrEmpty(collaboratorCode) && collaboratorCode.Length == 8)
            {
                var exists = _context.RaffleBusinessCollaborator.Any(e => !e.Removed && e.PersonalCode.ToLower() == collaboratorCode.ToLower());
                if (!exists)
                    collaboratorCode = "";
                else
                    HttpContext.Session.SetString("collaboratorCode", collaboratorCode);
            }else if (!String.IsNullOrEmpty(HttpContext.Session.GetString("collaboratorCode")))
            {
                collaboratorCode = HttpContext.Session.GetString("collaboratorCode");
            }

            ViewBag.RaffleId = id;
            ViewBag.CollaboratorCode = collaboratorCode;
            ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");
            var r =  _context.RaffleBusinessRaffle.Where(e => e.Id == Convert.ToInt32(id) && !e.Removed).FirstOrDefault();
            ViewBag.Raffle = r;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddParticipantReservation(IFormCollection form)
        {
            RaffleBusinessParticipant raffleParticipant = new RaffleBusinessParticipant();

            raffleParticipant.Email = form["Email"];
            raffleParticipant.FullName = form["FullName"];
            raffleParticipant.CollaboratorCode = form["CollaboratorCode"];

            if (String.IsNullOrEmpty(raffleParticipant.CollaboratorCode))
            {
                var code = HttpContext.Session.GetString("collaboratorCode");
                raffleParticipant.CollaboratorCode = String.IsNullOrEmpty(code) ? "" : code;
                HttpContext.Session.SetString("collaboratorCode", raffleParticipant.CollaboratorCode);
            }

            if (!String.IsNullOrEmpty(raffleParticipant.CollaboratorCode))
                raffleParticipant.CollaboratorCode = raffleParticipant.CollaboratorCode.ToLower();
            var pattern = new Regex("[^0-9]");
            var number = pattern.Replace(form["PhoneNumber"], string.Empty);
            raffleParticipant.PhoneNumber = number;

            raffleParticipant.RaffleId = Convert.ToInt32(form["RaffleId"]);

            int quantity = Convert.ToInt32(form["Numbers"]);

            if (quantity < 1)
                return View("SelectAndReserveIndex", raffleParticipant.Id);

            var raffle = _context.RaffleBusinessRaffle.Where(e => e.Id == raffleParticipant.RaffleId && !e.Removed)
                .Select(e => new RaffleBusinessRaffle() { Id = e.Id, RaffleNumbersValue = e.RaffleNumbersValue }).FirstOrDefault();

            List<string> l = new List<string>();
            Random r = new Random();

            for (int i = 0; i < quantity; i++)
            {
                var randomNumber = r.Next(01000, 99999);
                var toPut = randomNumber.ToString("D5");

                if (l.Any(e => e == toPut))
                    i -= 1;
                else
                {
                    var has = _context.RaffleBusinessParticipant.Any(e => e.RaffleId == raffle.Id && e.Numbers.Contains(toPut));

                    if(!has)
                        l.Add(toPut);
                    else
                        i -= 1;
                }
            }

            var s = l.FirstOrDefault();

            for (int i = 2; i <= l.Count(); i++)
            {
                s += ", " + l[i - 1];
            }

            raffleParticipant.Numbers = s;
            raffleParticipant.Identity = RandomToken.RandomString(10);
            raffleParticipant.Removed = false; 
            raffleParticipant.ParticipantStatus = 1;


            try
            {
                await _context.Database.BeginTransactionAsync();
                var participant = _context.RaffleBusinessParticipant.Add(raffleParticipant);
                await _context.SaveChangesAsync();

                var v = await CreatePaymentDate(quantity, raffleParticipant, raffle.RaffleNumbersValue);

                var d = new Dictionary<string, string>()
                    {
                        {"preferenceId", v },
                        {"participantId",  raffleParticipant.Id.ToString()}
                    };

                return RedirectToAction("SuccessBuy", d);

            }
            catch (Exception ex)
            {
                return View(new RaffleBusinessRaffle());
            }
        }

        private async Task<string> CreatePaymentDate(int quantity, RaffleBusinessParticipant raffleParticipant, decimal price)
        {
            var token = _configuration["MercadoPago:ProductionToken"];

            MercadoPagoConfig.AccessToken = token;
            var reference = RandomToken.RandomString(25);

            var paymentMethods = new PreferencePaymentMethodsRequest
            {
                //DefaultPaymentMethodId = "bank_transfer",
                ExcludedPaymentTypes = new List<PreferencePaymentTypeRequest>
                    {
                        new PreferencePaymentTypeRequest
                        {
                            Id = "digital_currency", //GIFTCard
                        },
                        new PreferencePaymentTypeRequest
                        {
                            Id = "digital_wallet", //PayPal
                        },

                    },
                Installments = 12,
            };

            var payer = new PreferencePayerRequest()
            {
                Email = raffleParticipant.Email,
                Name = raffleParticipant.FullName,
            };

            try
            {
                var request = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                        {
                            new PreferenceItemRequest
                            {
                                Title = "Cota de Número U" + raffleParticipant.Id + "R" + raffleParticipant.RaffleId,
                                Quantity = quantity,
                                CurrencyId = "BRL",
                                UnitPrice = price,
                            },
                         },
                    ExternalReference = reference,
                    PaymentMethods = paymentMethods,
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = _configuration["URLs:DefaultUrl"] + "TonStyle/RefreshPage",
                        Failure = _configuration["URLs:DefaultUrl"] + "TonStyle/RefreshPage",
                        Pending = _configuration["URLs:DefaultUrl"] + "TonStyle/RefreshPage",
                    },
                    AutoReturn = "approved",
                    NotificationUrl = "https://missiersorteios.azurewebsites.net/paymentspace/ReceiveNotification",
                    Payer = payer
                };

                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(request);

                if (preference != null)
                {
                    UserPaymentRegister prePay = new UserPaymentRegister()
                    {
                        IdBasicUser = raffleParticipant.Id,
                        FinalStatus = "Start",
                        NumberQuantity = quantity,
                        Reference = reference,
                        TransactionType = 2,
                        IdRaffle = raffleParticipant.RaffleId,
                        ReferenceId = preference.Id,
                        TotalValue = price * quantity,
                    };

                    try
                    {
                        _context.UserPaymentRegister.Add(prePay);
                        await _context.SaveChangesAsync();
                        _context.Database.CommitTransaction();

                        return preference.Id;
                    }
                    catch (Exception es) 
                    {
                        _context.Database.RollbackTransaction();
                    }

                    _context.RaffleBusinessParticipant.Remove(raffleParticipant);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e) { _context.Database.RollbackTransaction(); }

            return "";
        }

        public IActionResult SuccessBuy(string preferenceId, string participantId)
        {
            if (String.IsNullOrEmpty(preferenceId) || participantId == "0")
            {
                ViewBag.DeuRuim = true;
                return View();
            }

            var participant = _context.RaffleBusinessParticipant.Where(e => e.Id.ToString() == participantId && !e.Removed).FirstOrDefault();
            if (participant == null)
                ViewBag.DeuRuim = true;
            else
            {
                var raffle =_context.RaffleBusinessRaffle.Where(e => e.Id == participant.RaffleId && !e.Removed).FirstOrDefault();
                if(raffle == null)
                    ViewBag.DeuRuim = true;
                else
                {
                    participant.PhoneNumber = Convert.ToInt64(participant.PhoneNumber).ToString(@"(00) 00000-0000");
                    ViewBag.DeuRuim = false;
                    ViewBag.referenceId = preferenceId;
                    ViewBag.Participant = participant;
                    ViewBag.Raffle = raffle;    
                }
            }

            ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");
            return View();
        }

        public IActionResult AddNewRaffleTonStyle()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.UserId = idUser.Value;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRaffle(IFormCollection form)
        {
            var idUser = IsAutenticated();
            var userIdRout = Convert.ToInt32(form["IdBasicUser"]);

            if (idUser == 0 || idUser == null || idUser != userIdRout)
                return RedirectToAction("GetOutFromLogin", "Home");

            RaffleBusinessRaffle raffle = new RaffleBusinessRaffle();
            raffle.IdBasicUser = idUser.Value;
            raffle.RaffleName = form["RaffleName"];
            raffle.RaffleGeneralDescription = form["RaffleGeneralDescription"];
            raffle.RafflePremiationDescription = form["RafflePremiationDescription"];
            raffle.RaffleReceiptFile = form["base64"].ToString();
            var n = form["RaffleNumbersValue"].ToString().Replace(".", ",");
            var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            var culture = CultureInfo.CreateSpecificCulture("pt-BR");

            bool canConvert = decimal.TryParse(n, style, culture, out decimal number);
            if (canConvert)
                raffle.RaffleNumbersValue = number;
            else
                return View(raffle);

            raffle.RaffleStartDate = DateTime.Now;
            raffle.Removed = false;
            raffle.Identity = RandomToken.RandomString(10);
            raffle.RaffleStatus = GetStatusFromDate(raffle.RaffleStartDate);

            _context.RaffleBusinessRaffle.Add(raffle);
            await _context.SaveChangesAsync();

            return View("AddNewRaffleTonStyle", new RaffleBusinessRaffle());
        }

        public IActionResult RaffleDatails(int id, string paymentStatus = "approved")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null && id == 0)
                return RedirectToAction("GetOutFromLogin", "Home");

            var raffle = _context.RaffleBusinessRaffle.Where(e => e.Id == id && !e.Removed)
                .Select(e=> new RaffleBusinessRaffle() 
                {
                    Id = e.Id,
                    RaffleName = e.RaffleName,
                    RaffleNumbersValue = e.RaffleNumbersValue
                }).FirstOrDefault();

            ViewBag.UserId = idUser.Value;

            if (raffle != null)
            {
                var paymentRegisterData = _context.UserPaymentRegister.Where(e => e.TransactionType == 2 && e.IdRaffle == id && e.FinalStatus == paymentStatus)
                    .Select(e=> new UserPaymentRegister() 
                    {
                        Id = e.Id,
                        IdBasicUser = e.IdBasicUser,
                        FinalStatus = e.FinalStatus,
                        NumberQuantity = e.NumberQuantity,
                        TotalValue = e.TotalValue,
                        ValueWithTax = e.ValueWithTax
                        
                    }).ToList();

                var participantRegisterData = _context.RaffleBusinessParticipant.Where(e => !e.Removed && e.RaffleId == id)
                   .Select(e => new RaffleBusinessParticipant()
                   {
                       Id = e.Id,
                       RaffleId = e.RaffleId,
                       FullName = e.FullName,
                       Numbers = e.Numbers,
                       PhoneNumber = e.PhoneNumber,
                   }).ToList();

                var statistics = new List<RaffleBusinessStatistic>();
                var currency = CultureInfo.CreateSpecificCulture("pt-BR");
                ViewBag.Culture = currency;
                foreach (var register in paymentRegisterData)
                {
                    var participant = participantRegisterData.Where(e => e.Id == register.IdBasicUser && e.RaffleId == raffle.Id).FirstOrDefault();

                    if(participant != null)
                    {
                        var statistic = new RaffleBusinessStatistic()
                        {
                            ParticipantId = participant.Id,
                            PaymentId = register.Id,
                            RaffleId = raffle.Id,
                            UserId = register.IdBasicUser,

                            RaffleName = raffle.RaffleName,
                            RaffleNumbersValueString = raffle.RaffleNumbersValue.ToString("C2", currency),
                            RaffleNumbersValue = raffle.RaffleNumbersValue,

                            ParticipantFullName = participant.FullName,
                            ParticipantNumbers = participant.Numbers,
                            ParticipantPhoneNumber = Convert.ToInt64(participant.PhoneNumber).ToString(@"(00) 00000-0000"),
                            FinalStatus = register.FinalStatus,

                            NumberQuantity = register.NumberQuantity,
                            TotalValueString = register.TotalValue.ToString("C2", currency),
                            TotalValue = register.TotalValue,
                            RaffleTatalTax = register.ValueWithTax,
                            RaffleTatalTaxString = register.ValueWithTax.ToString("C2", currency),
                        };

                        statistics.Add(statistic);
                    }

                }

                return View(statistics);
            }

            return View(new List<RaffleBusinessStatistic>());
        }

        public async Task<IActionResult> RemoveRaffle(int id)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var p = _context.RaffleBusinessParticipant.Where(e => e.RaffleId == id).Select(e => new RaffleBusinessParticipant() { Id = e.Id }).ToList();
            var v = _context.RaffleBusinessRaffle.Where(e => e.Id == id && !e.Removed)
                .Select(e => new RaffleBusinessRaffle() { Id = e.Id, RaffleStatus = e.RaffleStatus })
                .FirstOrDefault();

            if(v.RaffleStatus == 3)
            {
                v.Removed = true;
                _context.RaffleBusinessRaffle.Attach(v).Property(e => e.Removed).IsModified = true;
            }
            else
            {
                if (p.Count() > 0)
                {
                    v.RaffleStatus = 3;
                    _context.RaffleBusinessRaffle.Attach(v).Property(e => e.RaffleStatus).IsModified = true;
                }
                else
                {
                    v.Removed = true;
                    _context.RaffleBusinessRaffle.Attach(v).Property(e => e.Removed).IsModified = true;
                }

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e) { }

            return RedirectToAction("InicialPageAdmin");
        }

        private int GetStatusFromDate(DateTime initial)
        {
            if (initial.Date.CompareTo(DateTime.Now.Date) == 0)
                return (int)RaffleEnums.Status.Iniciado;

            return (int)RaffleEnums.Status.Agendado;
        }

        public IActionResult RaffleViewList()
        {
            return View(_context.RaffleBusinessRaffle.Where(e=> !e.Removed).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PhoneSearch(IFormCollection form)
        {
            var pattern = new Regex("[^0-9]");
            var number = pattern.Replace(form["PhoneSearch"], string.Empty);
            var status = form["Status"].ToString();
            var phone = number;

            if (!String.IsNullOrEmpty(phone))
            {
                if (phone.Length == 11)
                {
                    var participantList = _context.RaffleBusinessParticipant.Where(e => e.PhoneNumber == phone && !e.Removed)
                        .Select(e=> new RaffleBusinessParticipant() 
                        {
                            Id = e.Id,
                            FullName = e.FullName,
                            Numbers = e.Numbers,
                            PhoneNumber = Convert.ToInt64(e.PhoneNumber).ToString(@"(00) 00000-0000"),
                            RaffleId = e.RaffleId,
                            ParticipantStatus = e.ParticipantStatus
                        })
                        .ToList();
                    if (participantList.Count > 0)
                    {
                        //var remo = status == "approved" ? true : false;

                        var paymentList = _context.UserPaymentRegister
                            .Where(e => e.TransactionType == 2 && e.FinalStatus == status)
                            .Select(e=> new UserPaymentRegister() 
                            {
                                Id = e.Id, ReferenceId = e.ReferenceId,
                                IdBasicUser = e.IdBasicUser,
                                TotalValue = e.TotalValue,
                                NumberQuantity = e.NumberQuantity,
                                IdRaffle = e.IdRaffle

                            }).ToList();

                        var raffleList = _context.RaffleBusinessRaffle
                            .Where(e => !e.Removed/* && e.RaffleStatus != 3*/)
                            .Select(e => new RaffleBusinessRaffle()
                            {
                                Id = e.Id,
                                RaffleName = e.RaffleName,
                                RafflePremiationDescription = e.RafflePremiationDescription,
                                RaffleStatus = e.RaffleStatus
                            });

                        UserPaymentRegister objectPayment;
                        RaffleBusinessRaffle objectRaffle;

                        foreach (var participant in participantList)
                        {
                            objectPayment = paymentList.Where(e => e.IdBasicUser == participant.Id).FirstOrDefault();
                            objectRaffle = raffleList.Where(e => e.Id == participant.RaffleId).FirstOrDefault();

                            participant.RaffleName = objectRaffle != null ? objectRaffle.RaffleName : "Rifa Excluída.";
                            participant.RafflePremiation = objectRaffle != null ? objectRaffle.RafflePremiationDescription : "Rifa Excluída.";
                            participant.StatusRaffle = objectRaffle != null ? objectRaffle.RaffleStatus == 1 ? "Disponível" : "Encerrado" : "Excluída";

                            participant.ReferenceId = objectPayment == null ? "" : objectPayment.ReferenceId;
                            participant.Value = objectPayment == null ? 0 : objectPayment.TotalValue;
                            participant.NumberQuantity =  objectPayment == null ? 0 : objectPayment.NumberQuantity;
                            participant.TotalValue = objectPayment == null ? "" : (objectPayment.NumberQuantity * objectPayment.TotalValue).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                        }

                        
                        ViewBag.Participant = participantList.Where(e => e.Value > 0 && e.NumberQuantity > 0);
                        ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");
                    }

                }

                ViewBag.Status = status;
                ViewBag.Phone = form["PhoneSearch"];
            }

            return View();
        }

        public IActionResult VerifyPhoneNumber(string PhoneNumber, string PhoneNumber2)
        {
            if (!String.IsNullOrEmpty(PhoneNumber))
            {
                if (!String.IsNullOrEmpty(PhoneNumber2))
                {
                    var pattern = new Regex("[^0-9]");
                    var number = pattern.Replace(PhoneNumber, string.Empty);
                    PhoneNumber = number;

                    number = pattern.Replace(PhoneNumber2, string.Empty);
                    PhoneNumber2 = number;

                    if (PhoneNumber != PhoneNumber2)
                        return Json($"Números de telefone diferentes");
                    else
                        return Json(true);
                }
                return Json(true);

            }
            else if(!String.IsNullOrEmpty(PhoneNumber2))
            {
                if (!String.IsNullOrEmpty(PhoneNumber))
                {
                    var pattern = new Regex("[^0-9]");
                    var number = pattern.Replace(PhoneNumber, string.Empty);
                    PhoneNumber = number;

                    number = pattern.Replace(PhoneNumber2, string.Empty);
                    PhoneNumber2 = number;

                    if (PhoneNumber != PhoneNumber2)
                        return Json($"Números de telefone diferentes");
                    else
                        return Json(true);
                }
                return Json(true);

            }

            return Json(true);
        }

        public IActionResult VerifyInviteCode(string collaboratorCode)
        {
            if(!String.IsNullOrEmpty(collaboratorCode))
                if(collaboratorCode.Length == 8)
                {
                    var answer =_context.RaffleBusinessCollaborator.Any(e => e.PersonalCode == collaboratorCode.ToLower() && !e.Removed);
                    if (answer)
                        return Json(true);
                    else
                        return Json("Convite inválido.");
                }
                else
                    return Json("Um convite válido possui 8 caracteres.");


            return Json(false);
        }

    }
}
