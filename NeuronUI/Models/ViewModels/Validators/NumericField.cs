using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace NeuronUI.Models.ViewModels.Validators
{
    public class NumericField : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;

            string pattern = @"^\d+$";
            Regex regex = new(pattern, RegexOptions.Singleline, TimeSpan.FromSeconds(1));

            if (!regex.Match(str).Success)
            {
                return new ValidationResult(false, "Este campo debe ser númerico");
            }
            return new ValidationResult(true, null);
        }
    }
}
