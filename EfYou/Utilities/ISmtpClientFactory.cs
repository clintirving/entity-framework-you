// // -----------------------------------------------------------------------
// // <copyright file="ISmtpClientFactory.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

namespace EfYou.Utilities
{
    public interface ISmtpClientFactory
    {
        ISmtpEmailClient Create();
    }
}