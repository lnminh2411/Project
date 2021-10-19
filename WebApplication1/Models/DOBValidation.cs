using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class DOBDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = new DateTime();
            if (value != null)
            {
                bool parse = DateTime.TryParse(value.ToString(), out date);

                if (!parse)
                    return new ValidationResult("Invalid Date");
                else
                {
                    //change below as per requirement
                    var min = DateTime.Now.AddYears(-16); //for min 16 age

                    var msg = string.Format("You must over 16 year old");
                    try
                    {
                        if (date > min)
                            return new ValidationResult(msg);
                        else
                            return ValidationResult.Success;
                    }
                    catch (Exception e)
                    {
                        return new ValidationResult(e.Message);
                    }
                }
            }
            else
                return new ValidationResult("Invalid Date");
        }
    }
}