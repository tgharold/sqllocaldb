﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbInstance.cs" company="http://sqllocaldb.codeplex.com">
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
//   SqlLocalDbInstance.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing an instance of SQL Server LocalDB.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    [Serializable]
    public class SqlLocalDbInstance : ISqlLocalDbInstance
    {
        #region Fields

        /// <summary>
        /// The SQL Server LocalDB instance name.
        /// </summary>
        private readonly string _instanceName;

        /// <summary>
        /// The named pipe to the SQL Server LocalDB instance.
        /// </summary>
        private string _namedPipe;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> does not exist.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> could not be obtained.
        /// </exception>
        public SqlLocalDbInstance(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

            if (!info.Exists)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_InstanceNotFoundFormat,
                    instanceName);

                throw new InvalidOperationException(message);
            }

            _instanceName = instanceName;
            _namedPipe = info.NamedPipe;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the LocalDB instance is running.
        /// </summary>
        public bool IsRunning
        {
            get { return !string.IsNullOrEmpty(_namedPipe); }
        }

        /// <summary>
        /// Gets the name of the LocalDB instance.
        /// </summary>
        public string Name
        {
            get { return _instanceName; }
        }

        /// <summary>
        /// Gets the named pipe that should be used
        /// to connect to the LocalDB instance.
        /// </summary>
        public string NamedPipe
        {
            get { return _namedPipe; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes the specified <see cref="ISqlLocalDbInstance"/> instance.
        /// </summary>
        /// <param name="instance">The LocalDB instance to delete.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instance"/> could not be deleted.
        /// </exception>
        public static void Delete(ISqlLocalDbInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            try
            {
                SqlLocalDbApi.DeleteInstance(instance.Name);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_DeleteFailedFormat,
                    instance.Name);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Creates a <see cref="SqlConnection"/> instance to communicate
        /// with the SQL Server LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnection"/> that can be used
        /// to communicate with the SQL Server Local DB instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsRunning"/> is <see langword="false"/>.
        /// </exception>
        public virtual SqlConnection CreateConnection()
        {
            SqlConnectionStringBuilder builder = CreateConnectionStringBuilder();

            if (builder == null)
            {
                return new SqlConnection();
            }
            else
            {
                return new SqlConnection(builder.ConnectionString);
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="IsRunning"/> is <see langword="false"/>.
        /// </exception>
        public virtual SqlConnectionStringBuilder CreateConnectionStringBuilder()
        {
            if (!this.IsRunning)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_NotRunningFormat,
                    _instanceName);

                throw new InvalidOperationException(message);
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = _namedPipe;
            return builder;
        }

        /// <summary>
        /// Returns information about the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo"/>
        /// containing information about the LocalDB instance.
        /// </returns>
        public virtual ISqlLocalDbInstanceInfo GetInstanceInfo()
        {
            return SqlLocalDbApi.GetInstanceInfo(_instanceName);
        }

        /// <summary>
        /// Shares the LocalDB instance using the specified name.
        /// </summary>
        /// <param name="sharedName">The name to use to share the instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sharedName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be shared.
        /// </exception>
        public virtual void Share(string sharedName)
        {
            if (sharedName == null)
            {
                throw new ArgumentNullException("sharedName");
            }

            try
            {
                SqlLocalDbApi.ShareInstance(_instanceName, sharedName);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_ShareFailedFormat,
                    _instanceName);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Starts the SQL Server LocalDB instance.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be started.
        /// </exception>
        public void Start()
        {
            try
            {
                // The pipe name changes per instance lifetime
                _namedPipe = SqlLocalDbApi.StartInstance(_instanceName);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_StartFailedFormat,
                    _instanceName);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Stops the SQL Server LocalDB instance.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be stopped.
        /// </exception>
        public void Stop()
        {
            try
            {
                SqlLocalDbApi.StopInstance(_instanceName);
                _namedPipe = string.Empty;
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_StopFailedFormat,
                    _instanceName);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Stops sharing the LocalDB instance.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be unshared.
        /// </exception>
        public virtual void Unshare()
        {
            try
            {
                SqlLocalDbApi.UnshareInstance(_instanceName);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_UnshareFailedFormat,
                    _instanceName);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        #endregion
    }
}