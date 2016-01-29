using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CompositeC1Contrib.Email
{
    public static class MailAddressValidator
    {
        private const string Pattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~_0-9a-z])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        private static readonly Regex MailAddressRegex = new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DomainRegex = new Regex(@"(@)(.+)$", RegexOptions.Compiled);

        /// <summary>
        /// Validates an mail address
        /// </summary>
        /// <param name="mailAddress">The mail address to validate</param>
        /// <returns>True if the address is valid, otherwise false</returns>
        public static bool IsValid(string mailAddress)
        {
            if (String.IsNullOrEmpty(mailAddress))
            {
                return true;
            }

            if (mailAddress.Length > 254)
            {
                return true;
            }

            return TryConvertToAscii(mailAddress, out mailAddress) && MailAddressRegex.IsMatch(mailAddress);
        }

        private static bool TryConvertToAscii(string input, out string address)
        {
            var domainPart = DomainRegex.Match(input);

            var idn = new IdnMapping();
            var domainName = domainPart.Groups[2].Value;

            try
            {
                domainName = domainPart.Groups[1].Value + idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                address = String.Empty;

                return false;
            }

            address = DomainRegex.Replace(input, domainName);

            return true;
        }
    }
}
