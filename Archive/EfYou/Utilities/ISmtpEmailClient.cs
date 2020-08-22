// // -----------------------------------------------------------------------
// // <copyright file="ISmtpEmailClient.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EfYou.Utilities
{
    public interface ISmtpEmailClient : IDisposable
    {
        string Host { get; set; }
        int Port { get; set; }
        bool UseDefaultCredentials { get; set; }
        ICredentialsByHost Credentials { get; set; }
        int Timeout { get; set; }
        ServicePoint ServicePoint { get; }
        SmtpDeliveryMethod DeliveryMethod { get; set; }
        SmtpDeliveryFormat DeliveryFormat { get; set; }
        string PickupDirectoryLocation { get; set; }
        bool EnableSsl { get; set; }
        X509CertificateCollection ClientCertificates { get; }
        string TargetName { get; set; }
        void Send(string from, string recipients, string subject, string body);
        void Send(MailMessage message);
        void SendAsync(string from, string recipients, string subject, string body, object userToken);
        void SendAsync(MailMessage message, object userToken);
        void SendAsyncCancel();
        Task SendMailAsync(string from, string recipients, string subject, string body);
        Task SendMailAsync(MailMessage message);
        event SendCompletedEventHandler SendCompleted;
    }
}