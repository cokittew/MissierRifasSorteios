using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MissierSystem.DataContext;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using MissierSystem.Service.EmailServices;
using MissierSystem.Service.TokenServices;

namespace MissierSystem.Controllers.User
{
    public class UserBasicController : Controller
    {
        private readonly UserClienteDataContext _context;

        public UserBasicController(UserClienteDataContext context)
        {
            _context = context;
        }

        //Methods

        [HttpGet]
        public async Task<IActionResult> VerifyEmailAutencityConfirmation(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                try
                {
                    //Não precisa trazer tudo
                    var findCodeConfirm = await _context.EmailConfirmation.FirstOrDefaultAsync(e => e.CodeAccess.Equals(code)) ;

                    if(findCodeConfirm != null)
                    {
                        var user = await _context.UserBasic.FirstOrDefaultAsync(e => e.Id == findCodeConfirm.UserId);

                        if(user != null)
                        {
                            user.EmailVerify = true;

                            _context.UserBasic.Update(user);
                            await _context.SaveChangesAsync();

                            _context.EmailConfirmation.Remove(findCodeConfirm);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        return Json($"Infelizmente esse código de confirmação é inválido!");
                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    return Json($"Houve um erro inesperado ao confirma seu email!");
                    // _context.Database.RollbackTransaction();
                }
            }
            else
            {
                return Json($"Página Iválida!");
            }

            return Json($"Email confirmado com sucesso!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordChangeAuto([Bind("Email")] LoginModel userLogin)
        {
            if (!String.IsNullOrEmpty(userLogin.Email))
            {
                try
                {
                    var userFind = await _context.UserBasicInfo.FirstOrDefaultAsync(e => e.Email.Equals(userLogin.Email));

                    if(userFind != null)
                    {
                        _context.Database.BeginTransaction();
                        userFind.UserPassword = "!" + RandomToken.RandomString(10) + "#";
                        _context.Update(userFind);
                        await _context.SaveChangesAsync();

                        if(!SenEmailTo.Send(userFind.Email, "Missier Bot Services - Nova Senha de Login", "Sua nova senha: "+ userFind.UserPassword))
                        {
                            _context.Database.RollbackTransaction();
                            return Json($"Houve um erro ao tentar alterar sua senha!");
                        }

                        _context.Database.CommitTransaction();

                    }
                    else
                    {
                        return Json($"Desculpe, esse email não é válido!");
                    }

                }
                catch(Exception e)
                {
                    _context.Database.RollbackTransaction();
                    Console.WriteLine(e);
                    return Json($"Houve um erro inesperado durante a mudança de sua senha, tente novamente.");
                }
            }
            else
            {
                return Json($"Email inválido");
            }

            return Json($"Sua senha foi alterada com sucesso, verifique seu email para visualizar a nova senha!");
        }


        //Validation Methods


        //EntityFramework Method
        private bool UserBasicExists(int id)
        {
            return _context.UserBasic.Any(e => e.Id == id);
        }
    }
}
