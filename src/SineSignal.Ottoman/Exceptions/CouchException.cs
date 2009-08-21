#region License

// <copyright file="CouchException.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Proxy;

namespace SineSignal.Ottoman.Exceptions
{
	/// <summary>
	/// A custom exception type to use for handling when things go boom in the night while communicating with CouchDB.
	/// </summary>
	public class CouchException : Exception
	{
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
		/// Initializes a new instance of the <see cref="CouchException"/> class.
		/// </summary>
		/// <param name="message">Message to use for the exception.</param>
		/// <param name="couchError">The error that CouchDB gave.</param>
		/// <param name="rawResponse">The raw response from the CouchDB server.</param>
		public CouchException(string message, ICouchError couchError, IHttpResponse rawResponse) 
			: base(message)
		{
			CouchError = couchError;
			RawResponse = rawResponse;
		}
	}
}