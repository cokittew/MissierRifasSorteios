using Microsoft.EntityFrameworkCore;
using MissierSystem.Models.GeneralModels.Models;
using MissierSystem.Models.GeneralModels.Models.UserModelItens;
using MissierSystem.Models.Platform.Services;
using MissierSystem.Models.Platform.Services.Raffle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MissierSystem.Models.GeneralModels.Models.UserExtraModels;
using MissierSystem.Models.Validation;
using MissierSystem.Models.TonModality;

namespace MissierSystem.DataContext
{
    public class UserClienteDataContext : DbContext
    {
        public UserClienteDataContext(DbContextOptions<UserClienteDataContext> options) : base(options)
        {

        }

        public DbSet<UserBasic> UserBasic { get; set; }
        public DbSet<UserBasicInfo> UserBasicInfo { get; set; }
        public DbSet<EmailConfirmation> EmailConfirmation { get; set; }
        public DbSet<UserTelegramValidation> UserTelegramValidation { get; set; }

        /*Services Model*/

        public DbSet<PlataformAllServicesBasic> PlataformAllServicesBasic { get; set; }

        /*Raffle*/

        public DbSet<PlatformServiceRaffle> PlataformServiceRaffle { get; set; }
        public DbSet<PlatformServiceRaffleInformations> PlatformServiceRaffleInformations { get; set; }
        public DbSet<MissierSystem.Models.GeneralModels.Models.UserSocialMidia> UserSocialMidia { get; set; }
        public DbSet<MissierSystem.Models.GeneralModels.Models.UserExtraModels.UserPixInformation> UserPixInformation { get; set; }
        public DbSet<MissierSystem.Models.GeneralModels.Models.UserExtraModels.UserBankInformation> UserBankInformation { get; set; }
        public DbSet<MissierSystem.Models.Platform.Services.Raffle.PlatformUserReservedNumber> PlatformUserReservedNumber { get; set; }
        public DbSet<MissierSystem.Models.Platform.Services.Raffle.PlatformGuestReservedNumber> PlatformGuestReservedNumber { get; set; }
        public DbSet<MissierSystem.Service.MercadoPago.UserPaymentRegister> UserPaymentRegister { get; set; }
        public DbSet<PlatformServiceRaffleFile> PlatformServiceRaffleFile { get; set; }
        public DbSet<MissierSystem.Models.TonModality.RaffleBusinessRaffle> RaffleBusinessRaffle { get; set; }
        public DbSet<MissierSystem.Models.TonModality.RaffleBusinessParticipant> RaffleBusinessParticipant { get; set; }
        public DbSet<MissierSystem.Models.TonModality.RaffleBusinessCollaborator> RaffleBusinessCollaborator { get; set; }
        public DbSet<MissierSystem.Models.TonModality.CollaboratorPaymentRegister> CollaboratorPaymentRegister { get; set; }
        public DbSet<MissierSystem.Models.TonModality.MissierWorker> MissierWorker { get; set; }

    }
}
