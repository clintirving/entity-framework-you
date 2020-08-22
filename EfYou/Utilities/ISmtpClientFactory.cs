// // -----------------------------------------------------------------------
// // <copyright file="ISmtpClientFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

namespace EfYouCore.Utilities
{
    public interface ISmtpClientFactory
    {
        ISmtpEmailClient Create();
    }
}