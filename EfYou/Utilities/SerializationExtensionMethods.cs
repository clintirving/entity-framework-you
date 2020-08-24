// // -----------------------------------------------------------------------
// // <copyright file="SerializationExtensionMethods.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EfYou.Utilities
{
    public static class SerializationExtensionMethods
    {
        public static string SerializeToXml(this object toSerialize)
        {
            var xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (var stringWriter = new StringWriter())
            {
                var xmlWriter = XmlWriter.Create(stringWriter);
                xmlSerializer.Serialize(xmlWriter, toSerialize);
                return stringWriter.ToString();
            }
        }
    }
}