using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GQService.com.gq.validate
{
    public class FunctionValidatorAttribute : ValidationAttribute
    {
        public delegate bool FunctionValidatorDelegate(object value, object ObjectInstance);

        private MethodInfo Function { get; set; }
        private string Mensaje { get; set; }

        public FunctionValidatorAttribute(Type delegateType, string delegateName, string mensaje = "")
        {
            Function = delegateType.GetMethod(delegateName);
            Mensaje = mensaje;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = (bool)Function.Invoke(validationContext.ObjectInstance, new object[] { value, validationContext.ObjectInstance });

            if (!result)
            {
                return new ValidationResult(this.FormatErrorMessage(string.IsNullOrWhiteSpace(Mensaje) ? validationContext.DisplayName : Mensaje), new string[] { validationContext.DisplayName });
            }
            return null;
        }
    }
}
