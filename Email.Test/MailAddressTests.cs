using CompositeC1Contrib.Email;

using NUnit.Framework;

namespace Email.Test
{
    public class MailAddressTests
    {
        [TestCase("david.jones@proseware.com")]
        [TestCase("d.j@server1.proseware.com")]
        [TestCase("jones@ms1.proseware.com")]
        [TestCase("j@proseware.com9")]
        [TestCase("js#internal@proseware.com")]
        [TestCase("j_9@[129.126.118.1]")]
        [TestCase("js@proseware.com9")]
        [TestCase("j.s@server1.proseware.com")]
        [TestCase("\"j\\\"s\\\"\"@proseware.com")]
        [TestCase("js@contoso.中国")]
        [TestCase("pauli@østerø.dk")]
        public void Valid(string address)
        {
            var isValid = MailAddressValidator.IsValid(address);

            Assert.That(isValid, Is.True);
        }

        [TestCase("j.@server1.proseware.com")]
        [TestCase("j..s@proseware.com")]
        [TestCase("js*@proseware.com")]
        [TestCase("js@proseware..com")]
        [TestCase("østerø@pauli.dk")]
        [TestCase("øster@pauli.dk")]
        [TestCase("sterø@pauli.dk")]
        [TestCase("indkøb@flyingseafood.dk")]
        public void Invalid(string address)
        {
            var isValid = MailAddressValidator.IsValid(address);

            Assert.That(isValid, Is.False);
        }
    }
}
