// // -----------------------------------------------------------------------
// // <copyright file="AnonymousClassServiceTests.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EfYou.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EfYouTests.Utilities
{
    [TestClass]
    public class AnonymousClassServiceTests
    {
        private readonly AnonymousClassService _anonymousClassService = new AnonymousClassService();

        [ExpectedException(typeof(ApplicationException))]
        [TestMethod]
        public void CreateAnonymousType_NoPropertiesProvided_ThrowsApplicationException()
        {
            //Arrange
            var propertiesList = new List<PropertyInfo>();

            //Act
            var result = _anonymousClassService.CreateAnonymousType(propertiesList);

            //Assert
            //By Exception
        }

        [TestMethod]
        public void CreateAnonymousType_OnePropertyProvided_ReturnsAnonymousTypeWithOneStringPropertyCalledName()
        {
            //Arrange
            var propertiesList = new List<PropertyInfo> {typeof(DummyEntity).GetProperties().FirstOrDefault(x => x.Name == "Name")};

            //Act
            var result = _anonymousClassService.CreateAnonymousType(propertiesList);

            //Assert
            Assert.AreEqual(1, result.GetProperties().Length);
            Assert.AreEqual(typeof(string), result.GetProperties().FirstOrDefault().PropertyType);
            Assert.AreEqual("Name", result.GetProperties().FirstOrDefault().Name);
        }

        [TestMethod]
        public void
            CreateAnonymousType_TwoPropertiesProvided_ReturnsAnonymousTypeWithOneStringPropertyCalledNameAndOneDateTimePropertyCalledDefaultingDate()
        {
            //Arrange
            var propertiesList = typeof(DummyEntity).GetProperties().Where(x => x.Name == "Name" || x.Name == "DefaultingDate").ToList();

            //Act
            var result = _anonymousClassService.CreateAnonymousType(propertiesList);

            //Assert
            Assert.AreEqual(2, result.GetProperties().Length);
            Assert.AreEqual(typeof(string), result.GetProperties().FirstOrDefault().PropertyType);
            Assert.AreEqual("Name", result.GetProperties().FirstOrDefault().Name);
            Assert.AreEqual(typeof(DateTime?), result.GetProperties().LastOrDefault().PropertyType);
            Assert.AreEqual("DefaultingDate", result.GetProperties().LastOrDefault().Name);
        }
    }
}