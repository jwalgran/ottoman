#region License

// <copyright file="CannotGetDatabaseException.cs" company="SineSignal, LLC.">
//   Copyright 2007-2009 SineSignal, LLC.
//       Licensed under the Apache License, Version 2.0 (the "License");
//       you may not use this file except in compliance with the License.
//       A copy of the License can be found in the LICENSE file or you may 
//       obtain a copy of the License at
//
//           http://www.apache.org/licenses/LICENSE-2.0
//
//       Unless required by applicable law or agreed to in writing, software
//       distributed under the License is distributed on an "AS IS" BASIS,
//       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//       See the License for the specific language governing permissions and
//       limitations under the License.
// </copyright>

#endregion

using System;
using SineSignal.Ottoman.Proxies;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// A custom exception type to use for handling when a database cannot be retrieved from CouchDB.
	/// </summary>
	public class CannotGetDatabaseException : Exception
	{
		public const string ExceptionMessageFormat = "Failed to get database '{0}'";

		/// <summary>
		/// The error that CouchDB gave.
		/// </summary>
		/// <value>The error.</value>
		public ICouchError CouchError { get; private set; }

		/// <summary>
		/// Gets or sets the raw response from the CouchDB server.
		/// </summary>
		/// <value>The raw response.</value>
		public IHttpResponse RawResponse { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CannotDeleteDatabaseException"/> class.
		/// </summary>
		/// <param name="databaseName">Name of the database.</param>
		/// <param name="couchError">The error that CouchDB gave.</param>
		/// <param name="rawResponse">The raw response from the CouchDB server.</param>
		public CannotGetDatabaseException(string databaseName, ICouchError couchError, IHttpResponse rawResponse)
			: base(String.Format(ExceptionMessageFormat, databaseName))
		{
			CouchError = couchError;
			RawResponse = rawResponse;
		}
	}
}