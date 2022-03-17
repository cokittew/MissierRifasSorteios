using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserExtraModels;

namespace MissierSystem.Controllers.Platform.UserServices
{
    public class UserPixInformationsController : Controller
    {
        private readonly UserClienteDataContext _context;

        public UserPixInformationsController(UserClienteDataContext context)
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

        // GET: UserPixInformations
        private async Task<IActionResult> Index()
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);

            return View(await _context.UserPixInformation.ToListAsync());
        }

        // GET: UserPixInformations/Details/5
        private async Task<IActionResult> Details(int? id, string userId, string signature)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPixInformation = await _context.UserPixInformation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPixInformation == null)
            {
                return NotFound();
            }

            return View(userPixInformation);
        }

        // GET: UserPixInformations/Create
        public IActionResult Create(string haveSaved = "")
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);
            try
            {
                var userCount = _context.UserPixInformation.FirstOrDefault(e => e.IdBasicUser == Convert.ToInt32(idUser));
                if (userCount != null)
                    return View(userCount);
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
            
            return View(new UserPixInformation());
        }

        // POST: UserPixInformations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdBasicUser,PixKeyType,PixKey")] UserPixInformation userPixInformation, IFormCollection form)
        {
            var idUser = IsAutenticated();

            if (idUser == 0 || idUser == null)
                return RedirectToAction("GetOutFromLogin", "Home");

            ViewBag.userId = Convert.ToInt32(idUser);

            if (ModelState.IsValid)
            {
                userPixInformation.BeginningDate = DateTime.Now;

                var userCount = new UserPixInformation();
                if (userPixInformation.Id != 0)
                {
                    _context.UserPixInformation.Update(userPixInformation);
                } 
                else
                    userCount = _context.Add(userPixInformation).Entity;

                await _context.SaveChangesAsync();
                ViewBag.alert = "success";
                return View(userCount);
            }
            ViewBag.alert = "error";
            return View(userPixInformation);
        }

        // GET: UserPixInformations/Edit/5
        private async Task<IActionResult> Edit(int? id, string userId, string signature)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPixInformation = await _context.UserPixInformation.FindAsync(id);
            if (userPixInformation == null)
            {
                return NotFound();
            }
            return View(userPixInformation);
        }

        // POST: UserPixInformations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> Edit(int id, [Bind("Id,IdBasicUser,PixKeyType,PixKey,WhatServiceUse,BeginningDate,Removed")] UserPixInformation userPixInformation)
        {
            if (id != userPixInformation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPixInformation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPixInformationExists(userPixInformation.Id))
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
            return View(userPixInformation);
        }

        // GET: UserPixInformations/Delete/5
        private async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userPixInformation = await _context.UserPixInformation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userPixInformation == null)
            {
                return NotFound();
            }

            return View(userPixInformation);
        }

        // POST: UserPixInformations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userPixInformation = await _context.UserPixInformation.FindAsync(id);
            _context.UserPixInformation.Remove(userPixInformation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPixInformationExists(int id)
        {
            return _context.UserPixInformation.Any(e => e.Id == id);
        }
    }
}
