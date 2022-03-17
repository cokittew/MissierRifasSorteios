using Microsoft.AspNetCore.Mvc;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.TonModality;
using MissierSystem.Service.MercadoPago;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MissierSystem.Controllers.MercadoPago
{
    public class PaymentSpaceController : Controller
    {
        private readonly UserClienteDataContext _context;
        public PaymentSpaceController(UserClienteDataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveNotification(string topic, string id)
        {         
            if (topic == "payment" && !String.IsNullOrEmpty(id))
            {
                var result = await VerifyAndUpdatePayment(id);
                return Ok();
            }

            return StatusCode(404);
        }

        private async Task<bool> VerifyAndUpdatePayment(string id)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "APP_USR-8592250525638227-050915-ec010d46d50bdc9e657cbbd18faa425d-325628631");
            var url = $"https://api.mercadopago.com/v1/payments/" + id;

            HttpResponseMessage response = await client.GetAsync(url);
            var jsonString = await response.Content.ReadAsStringAsync();

            var pay = JsonConvert.DeserializeObject<PaymentResult>(jsonString);

            var paymentSystemResult = "failed";
            if (pay != null)
            {
                var register = _context.UserPaymentRegister.Where(e => e.Reference == pay.external_reference && e.Removed == false)
                    .Select(e=> new UserPaymentRegister()
                    {
                        Id = e.Id,
                        IdBasicUser = e.IdBasicUser,
                        FinalStatus = e.FinalStatus,
                        NumberQuantity = e.NumberQuantity,
                        TransactionType = e.TransactionType,
                        TotalValue = e.TotalValue
                    }).FirstOrDefault();

                if (register != null)
                {
                    if (pay.status == "approved")
                    {
                        if(register.TransactionType == 1)
                        {
                            var user = _context.UserBasic.Where(e => e.Id == register.IdBasicUser && !e.Removed)
                            .Select(e => new UserBasic()
                            {
                                Id = e.Id,
                                UserNumberBag = e.UserNumberBag,
                            }).FirstOrDefault();

                            if (user != null)
                            {
                                user.UserNumberBag += register.NumberQuantity;
                                register.FinalStatus = "approved";
                                register.Removed = true;

                                try
                                {
                                    _context.Database.BeginTransaction();

                                    _context.UserPaymentRegister.Update(register);
                                    _context.Attach(register).Property(e => e.FinalStatus).IsModified = true;
                                    await _context.SaveChangesAsync();
                                    _context.UserPaymentRegister.Update(register);
                                    _context.Attach(register).Property(e => e.Removed).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    _context.UserBasic.Update(user);
                                    _context.Attach(user).Property(e => e.UserNumberBag).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    _context.Database.CommitTransaction();
                                    paymentSystemResult = pay.status;

                                    return true;
                                }
                                catch (Exception) { _context.Database.RollbackTransaction(); }
                            }
                        }
                        else if(register.TransactionType == 2)
                        {
                            var user = _context.RaffleBusinessParticipant.Where(e => e.Id == register.IdBasicUser && !e.Removed)
                            .Select(e => new RaffleBusinessParticipant()
                            {
                                Id = e.Id,
                                RaffleStatus = 3,
                                CollaboratorCode = e.CollaboratorCode
                                
                            }).FirstOrDefault();

                            if (user != null)
                            {
                                register.FinalStatus = "approved";
                                register.Removed = true;
                                try
                                {
                                    _context.Database.BeginTransaction();

                                    if (!String.IsNullOrEmpty(user.CollaboratorCode))
                                    {
                                        var collaborator = _context.RaffleBusinessCollaborator
                                            .Where(e => e.PersonalCode == user.CollaboratorCode.ToLower() && !e.Removed)
                                            .Select(e => new RaffleBusinessCollaborator()
                                            {
                                                Id = e.Id,
                                                YourCash = e.YourCash,
                                                YourCashPercentage = e.YourCashPercentage,

                                            }).FirstOrDefault();

                                        if (collaborator != null)
                                        {
                                            var cash = (collaborator.YourCashPercentage * (register.TotalValue * register.NumberQuantity)) / 100;
                                            collaborator.YourCash += cash;

                                            _context.RaffleBusinessCollaborator.Update(collaborator);
                                            _context.Attach(collaborator).Property(e => e.YourCash).IsModified = true;
                                            await _context.SaveChangesAsync();
                                        }
                                    }

                                    _context.UserPaymentRegister.Update(register);
                                    _context.Attach(register).Property(e => e.FinalStatus).IsModified = true;
                                    await _context.SaveChangesAsync();
                                    _context.UserPaymentRegister.Update(register);
                                    _context.Attach(register).Property(e => e.Removed).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    _context.RaffleBusinessParticipant.Update(user);
                                    _context.Attach(user).Property(e => e.RaffleStatus).IsModified = true;
                                    await _context.SaveChangesAsync();

                                    _context.Database.CommitTransaction();
                                    paymentSystemResult = pay.status;

                                    return true;
                                }
                                catch (Exception) { _context.Database.RollbackTransaction(); }
                            }
                        }

                        
                    }
                    else if (pay.status == "pending")
                    {
                        if (register.TransactionType == 1)
                        {
                            paymentSystemResult = pay.status;
                            register.FinalStatus = "pending";
                            _context.Attach(register).Property(e => e.FinalStatus).IsModified = true;
                            register.FinalStatus = pay.status;

                            try
                            {
                                _context.UserPaymentRegister.Update(register);
                                await _context.SaveChangesAsync();

                                paymentSystemResult = pay.status;

                                return true;
                            }
                            catch (Exception) { }
                        }
                        else if (register.TransactionType == 2)
                        {
                            paymentSystemResult = pay.status;
                            register.FinalStatus = "pending";
                            _context.Attach(register).Property(e => e.FinalStatus).IsModified = true;
                            register.FinalStatus = pay.status;

                            try
                            {
                                _context.UserPaymentRegister.Update(register);
                                await _context.SaveChangesAsync();

                                paymentSystemResult = pay.status;

                                return true;
                            }
                            catch (Exception) { }
                        }
                            
                    }
                }
            }

            return false;
        }

    }
}
