using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MissierSystem.DataContext;
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

        public IActionResult InicialPage(string raffleId)
        {
            var r = _context.RaffleBusinessRaffle.Where(e => !e.Removed).ToList();
            return View(r);
        }

        public IActionResult SelectAndReserveIndex(int id)
        {
            ViewBag.RaffleId = id;
            var r=  _context.RaffleBusinessRaffle.Where(e => e.Id == Convert.ToInt32(id) && !e.Removed).FirstOrDefault();
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
            if (!String.IsNullOrEmpty(raffleParticipant.CollaboratorCode))
                raffleParticipant.CollaboratorCode = raffleParticipant.CollaboratorCode.ToLower();
            var pattern = new Regex("[^0-9]");
            var number = pattern.Replace(form["PhoneNumber"], string.Empty);
            raffleParticipant.PhoneNumber = number;

            raffleParticipant.RaffleId = Convert.ToInt32(form["RaffleId"]);

            int quantity = Convert.ToInt32(form["Numbers"]);

            if (quantity < 1)
                return View("", raffleParticipant);

            var raffle = _context.RaffleBusinessRaffle.Where(e => e.Id == raffleParticipant.RaffleId && !e.Removed)
                .Select(e => new RaffleBusinessRaffle() { Id = e.Id, RaffleNumbersValue = e.RaffleNumbersValue }).FirstOrDefault();

            List<string> l = new List<string>();
            Random r = new Random();

            for (int i = 0; i < quantity; i++)
            {
                var randomNumber = r.Next(01000, 99999);

                if (l.Any(e => e == randomNumber.ToString("D5")))
                    i -= 1;
                else
                    l.Add(randomNumber.ToString("D5"));
            }

            var s = l.FirstOrDefault();

            for (int i = 2; i <= l.Count(); i++)
            {
                s += ", " + l[i - 1];
            }

            raffleParticipant.Numbers = s;
            raffleParticipant.Identity = RandomToken.RandomString(10);
            raffleParticipant.Removed = false; 
            raffleParticipant.RaffleStatus = 1;


            try
            {
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
                        Success = _configuration["URLs:DefaultUrl"] + "TonStyle/InicialPage",
                        Failure = _configuration["URLs:DefaultUrl"] + "TonStyle/InicialPage",
                        Pending = _configuration["URLs:DefaultUrl"] + "TonStyle/InicialPage",
                    },
                    AutoReturn = "approved",
                    NotificationUrl = "https://missier.azurewebsites.net/paymentspace/ReceiveNotification",
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
                        TotalValue = price,
                    };

                    try
                    {
                        _context.Database.BeginTransaction();
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
            catch (Exception e) { }

            return "";
        }

        public IActionResult SuccessBuy(string preferenceId, string participantId)
        {
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
                    ViewBag.DeuRuim = false;
                    ViewBag.referenceId = preferenceId;
                    ViewBag.Participant = participant;
                    ViewBag.Raffle = raffle;    
                }
            }

            return View();
        }

        public IActionResult AddNewRaffleTonStyle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRaffle(IFormCollection form)
        {
            RaffleBusinessRaffle raffle = new RaffleBusinessRaffle();
            raffle.IdBasicUser = 1;
            raffle.RaffleName = form["RaffleName"];
            raffle.RaffleGeneralDescription = ""; // form["RaffleGeneralDescription"];
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
            var phone = number;

            var deuRuim = true;

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
                            RaffleStatus = e.RaffleStatus
                        })
                        .ToList();
                    if (participantList.Count > 0)
                    {
                        var paymentList = _context.UserPaymentRegister
                            .Where(e => !e.Removed && e.TransactionType == 2)
                            .Select(e=> new UserPaymentRegister() 
                            {
                                Id = e.Id, ReferenceId = e.ReferenceId,
                                IdBasicUser = e.IdBasicUser,
                                TotalValue = e.TotalValue,
                                NumberQuantity = e.NumberQuantity,
                                IdRaffle = e.IdRaffle

                            }).ToList();

                        var raffleList = _context.RaffleBusinessRaffle
                            .Where(e => !e.Removed && e.RaffleStatus == 1)
                            .Select(e => new RaffleBusinessRaffle()
                            {
                                Id = e.Id,
                                RaffleName = e.RaffleName,
                                RafflePremiationDescription = e.RafflePremiationDescription,

                            });

                        UserPaymentRegister objectPayment;
                        RaffleBusinessRaffle objectRaffle;

                        foreach (var participant in participantList)
                        {
                            objectPayment = paymentList.Where(e => e.IdBasicUser == participant.Id).FirstOrDefault();
                            objectRaffle = raffleList.Where(e => e.Id == participant.RaffleId).FirstOrDefault();

                            participant.RaffleName = objectRaffle.RaffleName;
                            participant.RafflePremiation = objectRaffle.RafflePremiationDescription;

                            participant.ReferenceId = objectPayment == null ? "" : objectPayment.ReferenceId;
                            participant.Value = objectPayment == null ? 0 : objectPayment.TotalValue;
                            participant.NumberQuantity =  objectPayment == null ? 0 : objectPayment.NumberQuantity;
                            participant.TotalValue = objectPayment == null ? "" : (objectPayment.NumberQuantity * objectPayment.TotalValue).ToString("C2", CultureInfo.CreateSpecificCulture("pt-BR"));
                        }

                        ViewBag.Participant = participantList.Where(e => e.Value > 0 && e.NumberQuantity > 0);
                        ViewBag.Currency = CultureInfo.CreateSpecificCulture("pt-BR");
                    }

                }

            }

            return View();
        }

        public IActionResult VerifyPhoneNumber(string PhoneNumber, string PhoneNumber2)
        {
            if (!String.IsNullOrEmpty(PhoneNumber) && !String.IsNullOrEmpty(PhoneNumber2))
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
            else
                return Json(false);
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
