using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace CompositeC1Contrib.Email
{
    public static class MailAddressValidator
    {
        private static readonly Regex AllowedLocalChars = new Regex(@"[a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~\.]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DomanPartValidator = new Regex(@"^(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Validates an mail address
        /// </summary>
        /// <param name="mailAddress">The mail address to validate</param>
        /// <returns>True if the address is valid, otherwise false</returns>
        public static bool IsValid(string mailAddress)
        {
            var isValid = ValidateMailAddress(mailAddress);
            if (!isValid)
            {
                return false;
            }

            var parts = mailAddress.Split('@');
            if (parts.Length != 2)
            {
                return false;
            }

            isValid = ValidateLocalPart(parts[0]);
            if (!isValid)
            {
                return false;
            }

            isValid = ValidateDomainPart(parts[1]);
            if (!isValid)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateMailAddress(string mailAddress)
        {
            if (String.IsNullOrEmpty(mailAddress))
            {
                return false;
            }

            if (mailAddress.Length > 254)
            {
                return false;
            }

            if (mailAddress.Contains("..") || mailAddress.Contains(".@") || mailAddress.Contains("@.") || mailAddress.Contains("._."))
            {
                return false;
            }

            if (mailAddress.EndsWith("."))
            {
                return false;
            }

            return true;
        }

        private static bool ValidateLocalPart(string local)
        {
            if (local.Length == 0 || local.Length > 64)
            {
                return false;
            }

            if (!Char.IsLetter(local[0]))
            {
                return false;
            }

            if (local.Any(c => !AllowedLocalChars.IsMatch(c.ToString())))
            {
                return false;
            }

            return true;
        }

        private static bool ValidateDomainPart(string domain)
        {
            if (domain.Length < 3)
            {
                return false;
            }

            if (!TryConvertDomainToAscii(domain, out domain))
            {
                return false;
            }

            if (!DomanPartValidator.IsMatch(domain))
            {
                var sIp = domain.Substring(1, domain.Length - 2);
                if (sIp.StartsWith("IPv6:"))
                {
                    sIp = sIp.Substring(5, sIp.Length - 5);
                }

                IPAddress ip;
                if (IPAddress.TryParse(sIp, out ip))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        private static bool TryConvertDomainToAscii(string domain, out string asciiDomain)
        {
            var idn = new IdnMapping();

            try
            {
                asciiDomain = idn.GetAscii(domain);

                return true;
            }
            catch (ArgumentException)
            {
                asciiDomain = String.Empty;

                return false;
            }
        }
    }
}
