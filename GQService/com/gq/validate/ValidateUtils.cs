using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GQService.com.gq.validate
{
    /// <summary>
    /// 
    /// </summary>
    public static class ValidateUtils
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static bool TryValidateModel(object model, List<ValidationResult> results = null)
        {
            if (results == null)
            {
                results = new List<ValidationResult>();
            }
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            var isValid = Validator.TryValidateObject(model, context, results, true);
            return isValid;
        }


    }
}
