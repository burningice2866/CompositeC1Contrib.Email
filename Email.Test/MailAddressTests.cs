using System.Collections.Generic;

using CompositeC1Contrib.Email;

using NUnit.Framework;

namespace Email.Test
{
    public class MailAddressTests
    {
        [TestCaseSource("ValidMailAddress")]
        public void Valid(string address)
        {
            var isValid = MailAddressValidator.IsValid(address);

            Assert.That(isValid, Is.True);
        }

        [TestCaseSource("InvalidMailAddress")]
        public void Invalid(string address)
        {
            var isValid = MailAddressValidator.IsValid(address);

            Assert.That(isValid, Is.False);
        }

        private static IEnumerable<string> ValidMailAddress()
        {
            return new List<string>
            {
                "david.jones@proseware.com",
                "d.j@server1.proseware.com",
                "jones@ms1.proseware.com",
                "j@proseware.com9",
                "js#internal@proseware.com",
                "j_9@[129.126.118.1]",
                "js@proseware.com9",
                "js@contoso.中国",
                "pauli@østerø.dk",
                "mohamed2500-@hotmail.com",
                "js*@proseware.com",
                "j.s@server1.proseware.com",
                "user@[IPv6:2001:db8::1]",
                "other.email-with-dash@example.com",
                "disposable.style.email.with+symbol@example.com",
                "very.common@example.com"
            };
        }

        private static IEnumerable<string> InvalidMailAddress()
        {
            return new List<string>
            {
                "j.@server1.proseware.com",
                "j..s@proseware.com",
                "js@proseware..com",
                "østerø@pauli.dk",
                "øster@pauli.dk",
                "sterø@pauli.dk",
                "indkøb@flyingseafood.dk",
                "Abc.example.com",
                "A@b@c@example.com",
                "a\"b(c)d,e:f;g<h>i[j\\k]l@example.com",
                "just\"not\"right@example.com"
            };
        }
    }
}