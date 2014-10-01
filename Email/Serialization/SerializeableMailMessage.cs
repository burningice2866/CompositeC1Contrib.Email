using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    public class SerializeableMailMessage
    {
        private readonly bool _isBodyHtml;
        private readonly string _body;
        private readonly SerializeableMailAddress _from;
        private readonly SerializeableMailAddress _sender;
        private readonly string _subject;
        private readonly Encoding _bodyEncoding;
        private readonly Encoding _subjectEncoding;
        private readonly DeliveryNotificationOptions _deliveryNotificationOptions;
        private readonly SerializeableCollection _headers;
        private readonly MailPriority _priority;

        private readonly IList<SerializeableMailAddress> _to;
        private readonly IList<SerializeableMailAddress> _cc;
        private readonly IList<SerializeableMailAddress> _bcc;
        private readonly IList<SerializeableMailAddress> _replyToList;

        private readonly IList<SerializeableAlternateView> _alternateViews;

        private readonly IList<SerializeableAttachment> _attachments;
       
        public SerializeableMailMessage(MailMessage mailMessage)
        {
            _to = new List<SerializeableMailAddress>();
            _cc = new List<SerializeableMailAddress>();
            _bcc = new List<SerializeableMailAddress>();
            _replyToList = new List<SerializeableMailAddress>();

            _alternateViews = new List<SerializeableAlternateView>();

            _attachments = new List<SerializeableAttachment>();

            _isBodyHtml = mailMessage.IsBodyHtml;
            _body = mailMessage.Body;
            _subject = mailMessage.Subject;
            _from = new SerializeableMailAddress(mailMessage.From);

            foreach (MailAddress ma in mailMessage.To)
            {
                _to.Add(new SerializeableMailAddress(ma));
            }

            foreach (MailAddress ma in mailMessage.CC)
            {
                _cc.Add(new SerializeableMailAddress(ma));
            }

            foreach (MailAddress ma in mailMessage.Bcc)
            {
                _bcc.Add(new SerializeableMailAddress(ma));
            }

            _attachments = new List<SerializeableAttachment>();
            foreach (Attachment att in mailMessage.Attachments)
            {
                _attachments.Add(new SerializeableAttachment(att));
            }

            _bodyEncoding = mailMessage.BodyEncoding;

            _deliveryNotificationOptions = mailMessage.DeliveryNotificationOptions;
            _headers = new SerializeableCollection(mailMessage.Headers);
            _priority = mailMessage.Priority;

            foreach (MailAddress ma in mailMessage.ReplyToList)
            {
                _replyToList.Add(new SerializeableMailAddress(ma));
            }

            if (mailMessage.Sender != null)
            {
                _sender = new SerializeableMailAddress(mailMessage.Sender);
            }

            _subjectEncoding = mailMessage.SubjectEncoding;

            foreach (AlternateView av in mailMessage.AlternateViews)
            {
                _alternateViews.Add(new SerializeableAlternateView(av));
            }
        }

        public MailMessage GetMailMessage()
        {
            var mailMessage = new MailMessage
            {
                IsBodyHtml = _isBodyHtml,
                Body = _body,
                Subject = _subject,
                BodyEncoding = _bodyEncoding,
                DeliveryNotificationOptions = _deliveryNotificationOptions,
                Priority = _priority,
                SubjectEncoding = _subjectEncoding,
            };
            
            if (_from != null)
            {
                mailMessage.From = _from.GetMailAddress();
            }

            foreach (var mailAddress in _to)
            {
                mailMessage.To.Add(mailAddress.GetMailAddress());
            }

            foreach (var mailAddress in _cc)
            {
                mailMessage.CC.Add(mailAddress.GetMailAddress());
            }

            foreach (var mailAddress in _bcc)
            {
                mailMessage.Bcc.Add(mailAddress.GetMailAddress());
            }

            foreach (var attachment in _attachments)
            {
                mailMessage.Attachments.Add(attachment.GetAttachment());
            }
            
            _headers.CopyTo(mailMessage.Headers);

            foreach (var mailAddress in _replyToList)
            {
                mailMessage.ReplyToList.Add(mailAddress.GetMailAddress());
            }

            if (_sender != null)
            {
                mailMessage.Sender = _sender.GetMailAddress();
            }

            foreach (var alternateView in _alternateViews)
            {
                mailMessage.AlternateViews.Add(alternateView.GetAlternateView());
            }

            return mailMessage;
        }
    }
}
