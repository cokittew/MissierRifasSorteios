using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserExtraModels;
using MissierSystem.Service.LocalDataBase;

namespace MissierSystem.Controllers.Platform.UserServices
{
    public class UserBankInformationsController : Controller
    {
        private readonly UserClienteDataContext _context;

        public UserBankInformationsController(UserClienteDataContext context)
        {
            _context = context;
        }

        private int? IsAutenticated()
        {
            var id = HttpContext.Session.GetInt32("UserLogId");
            if (id == 0 || id == null)
                return 0;

            var user = _context.UserBasic.Where(e => e.Id == id).Select(e => new UserBasic() { Id = e.Id, SignatureActive = e.SignatureActive }).FirstOrDefault();

            if (user == null || (user != null && !user.SignatureActive))
            {
                HttpContext.Session.SetInt32("UserLogId", 0);
                return 0;
            }

            return user.Id;
        }

        // GET: UserBankInformations
        public async Task<IActionResult> Index(string actionSystem = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            if (!String.IsNullOrEmpty(actionSystem))
                ViewBag.alert = actionSystem;

            ViewBag.userId = Convert.ToInt32(idUser);

            ViewBag.banksData = LocalDataBase.GetAllBank();

            return View(await _context.UserBankInformation.Where(e => !e.Removed && e.IdBasicUser == Convert.ToInt32(idUser)).Take(10).ToListAsync());
        }

        // GET: UserBankInformations/Details/5
        private async Task<IActionResult> Details(int? id, string userId, string signature)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBankInformation = await _context.UserBankInformation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userBankInformation == null)
            {
                return NotFound();
            }

            ViewBag.userId = userId;
            ViewBag.signature = signature;

            return View(userBankInformation);
        }

        // GET: UserBankInformations/Create
        public IActionResult Create()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);

            return View();
        }

        // POST: UserBankInformations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdBasicUser,BankCode,BankAccount,AgenceAccount,AccountOwnerCpf")] UserBankInformation userBankInformation)
        {
            var apenasDigitos = new Regex(@"[^\d]");
            userBankInformation.AccountOwnerCpf = apenasDigitos.Replace(userBankInformation.AccountOwnerCpf, "");

            if(userBankInformation.AccountOwnerCpf.Length == 11)
            {
                if (ModelState.IsValid)
                {
                    userBankInformation.BeginningDate = DateTime.Now;
                    userBankInformation.AccountOwnerCpf = userBankInformation.AccountOwnerCpf.ToString();
                    _context.Add(userBankInformation);
                    await _context.SaveChangesAsync();

                    var dic = new Dictionary<string, string>()
                {
                    {"actionSystem", "successSave" }
                };

                    return RedirectToAction(nameof(Index), dic);
                }
            }

            ViewBag.alert = "fail";

            return View(userBankInformation);
        }

        // GET: UserBankInformations/Edit/5
        private async Task<IActionResult> Edit(int? id, string userId, string signature)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBankInformation = await _context.UserBankInformation.FindAsync(id);
            if (userBankInformation == null)
            {
                return NotFound();
            }
            ViewBag.userId = userId;
            ViewBag.signature = signature;

            return View(userBankInformation);
        }

        // POST: UserBankInformations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> Edit(int id, [Bind("Id,IdBasicUser,WhatServiceUse,BankCode,BankAccount,BankAccountDigt,AccountOwnerCpf,BeginningDate,Removed")] UserBankInformation userBankInformation)
        {
            if (id != userBankInformation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBankInformation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBankInformationExists(userBankInformation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userBankInformation);
        }

        // GET: UserBankInformations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            var answer = "";
            UserBankInformation userBankInformation = null;
            try
            {
                 userBankInformation = await _context.UserBankInformation.FindAsync(Convert.ToInt32(id));
            }
            catch (Exception) { }

            if(userBankInformation != null)
            {
                await _context.Database.BeginTransactionAsync();
                userBankInformation.Removed = true;
                _context.UserBankInformation.Update(userBankInformation);

                try
                {
                    await _context.SaveChangesAsync();
                    _context.Database.CommitTransaction();
                    answer = "successDelete";
                }
                catch (Exception)
                {
                    answer = "failDelete";
                    _context.Database.RollbackTransaction();
                }
                
            }

            var dic = new Dictionary<string, string>()
            {
                {"actionSystem",answer }
            };

            return RedirectToAction("Index", dic);
        }

        // POST: UserBankInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userBankInformation = await _context.UserBankInformation.FindAsync(id);
            _context.UserBankInformation.Remove(userBankInformation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserBankInformationExists(int id)
        {
            return _context.UserBankInformation.Any(e => e.Id == id);
        }
    }
}
