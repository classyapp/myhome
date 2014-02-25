using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Mvc.Attributes
{
    public class EveryItemIsAttribute : ValidationAttribute
    {
        public Type[] Validators { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Validators.Count() == 0)
                return ValidationResult.Success;

            var enumerable = value as IEnumerable;
            if (enumerable == null)
                return ValidationResult.Success;

            var attributes = new List<ValidationAttribute>(Validators.Count());
            foreach (var s in Validators)
            {
                attributes.Add((ValidationAttribute)Activator.CreateInstance(s));
            }
            foreach (var obj in enumerable)
            {
                foreach (var att in attributes)
                {
                    if (!att.IsValid(obj))
                        return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
