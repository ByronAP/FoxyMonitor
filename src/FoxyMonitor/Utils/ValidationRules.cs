using System.Globalization;
using System.Windows.Controls;

namespace FoxyMonitor.Utils
{
    internal class ValidationRules
    {
        internal class DisplayNameValidationRule : ValidationRule
        {
            public int MinLength { get; set; } = 4;
            public int MaxLength { get; set; } = 16;

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                if (value == null || value.ToString().Trim() == string.Empty) return new ValidationResult(false, "Display Name can not be empty.");

                var displayNameLength = value.ToString().Trim().Length;
                 
                if ((displayNameLength < MinLength) || (displayNameLength > MaxLength))
                {
                    return new ValidationResult(false, $"Display Name should be between {MinLength}-{MaxLength} characters.");
                }
                return ValidationResult.ValidResult;
            }
        }

        internal class LauncherIDValidationRule : ValidationRule
        {
            // (just a guess based on calculated average length of 98
            public int MinLength { get; set; } = 60;
            public int MaxLength { get; set; } = 120;

            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                if (value == null || value.ToString().Trim() == string.Empty) return new ValidationResult(false, "Launcher Id can not be empty.");

                var launcherId = value.ToString().Trim();
                var launcherIdLength = launcherId.Length;

                if ((launcherIdLength < MinLength) || (launcherIdLength > MaxLength))
                {
                    return new ValidationResult(false, "Invalid Launcher Id.");
                }
                return ValidationResult.ValidResult;
            }
        }
    }
}
