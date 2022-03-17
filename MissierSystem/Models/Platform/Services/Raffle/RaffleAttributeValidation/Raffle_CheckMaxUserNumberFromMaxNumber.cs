using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MissierSystem.Models.Platform.Services.Raffle.RaffleAttributeValidation
{
    public class Raffle_CheckMaxUserNumberFromMaxNumber : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext fromClassAttributes)
        {
            var raffle = fromClassAttributes.ObjectInstance as PlatformServiceRaffle;

            if(raffle != null && raffle.RaffleUserMaxNumbersLimited > 0)
            {
                var result = raffle.RaffleMaxNumberLimited / 2;

                if(raffle.RaffleMaxNumberLimited <= 10)
                {
                    return new ValidationResult("Você só pode escolher 1 como opção.");
                }

                if (raffle.RaffleUserMaxNumbersLimited > result)
                {
                    return new ValidationResult($"Escolha um número entre 1 e {result}");
                }
                
            }


            return ValidationResult.Success;
        }
    }
}
