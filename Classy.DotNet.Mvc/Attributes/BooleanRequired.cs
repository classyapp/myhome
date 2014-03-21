using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Classy.DotNet.Mvc.Attributes
{
    public class BooleanRequired : RequiredAttribute, IClientValidatable
    {
        public BooleanRequired()
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value.GetType() == typeof(System.Boolean)) return (bool)value;
            return Boolean.Parse(value.ToString()) == true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelClientValidationRule[] { new ModelClientValidationRule() { ValidationType = "brequired", ErrorMessage = this.ErrorMessage } };
        }
    }
}
