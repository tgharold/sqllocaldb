﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbExceptionTests.cs" company="http://sqllocaldb.codeplex.com">
//   New BSD License (BSD)
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the New BSD Licence (BSD). 
//   A copy of the license can be found in the License.txt file at the root of this
//   distribution.  By using this source code in any fashion, you are agreeing to be
//   bound by the terms of the New BSD Licence. You must not remove this notice, or
//   any other, from this software.
// </license>
// <summary>
//   SqlLocalDbExceptionTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbException"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbExceptionTests
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbExceptionTests"/> class.
        /// </summary>
        public SqlLocalDbExceptionTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests .ctor().")]
        public void Constructor_Default()
        {
            SqlLocalDbException target = new SqlLocalDbException();

            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(SR.SqlLocalDbException_DefaultMessage, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string).")]
        public void Constructor_Message()
        {
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message);

            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, Exception).")]
        public void Constructor_MessageInnerException()
        {
            InvalidOperationException innerException = new InvalidOperationException();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, innerException);

            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreSame(innerException, target.InnerException, "The InnerException property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int).")]
        public void Constructor_MessageErrorCode()
        {
            int errorCode = 337519;
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, errorCode);

            Assert.AreEqual(errorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int, string).")]
        public void Constructor_MessageErrorCodeInstanceName()
        {
            int errorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, errorCode, instanceName);

            Assert.AreEqual(errorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(instanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int, string, Exception).")]
        public void Constructor_MessageErrorCodeInstanceNameInnerException()
        {
            InvalidOperationException innerException = new InvalidOperationException();
            int errorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, errorCode, instanceName, innerException);

            Assert.AreEqual(errorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreSame(innerException, target.InnerException, "The InnerException property is incorrect.");
            Assert.AreEqual(instanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(SerializationInfo, StreamingContext).")]
        public void Constructor_Serialization()
        {
            InvalidOperationException innerException = new InvalidOperationException();
            int errorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, errorCode, instanceName, innerException);

            BinaryFormatter formatter = new BinaryFormatter();

            SqlLocalDbException deserialized;

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, target);
                stream.Seek(0L, SeekOrigin.Begin);
                deserialized = formatter.Deserialize(stream) as SqlLocalDbException;
            }

            Assert.IsNotNull(deserialized, "The exception was not deserialized.");
            Assert.AreEqual(deserialized.ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(deserialized.InstanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(deserialized.Message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests GetObjectData() if info is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetObjectData_ThrowsIfInfoIsNull()
        {
            SqlLocalDbException target = new SqlLocalDbException();

            throw ErrorAssert.Throws<ArgumentNullException>(
                () => target.GetObjectData(null, new System.Runtime.Serialization.StreamingContext()),
                "info");
        }

        #endregion
    }
}