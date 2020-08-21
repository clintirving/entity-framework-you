// // -----------------------------------------------------------------------
// // <copyright file="SmtpClientFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

namespace EfYouCore.Utilities
{
    public class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpEmailClient Create()
        {
            return new SmtpEmailClient();
        }
    }
}