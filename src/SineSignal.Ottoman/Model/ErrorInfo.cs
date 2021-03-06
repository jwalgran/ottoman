#region License

// <copyright file="ErrorInfo.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman.Model
{
	/// <summary>
	/// Models the response given by CouchDB when an error occurs.
	/// </summary>
	/// <remarks>
	/// This is a sample response from CouchDB when an error occurs.
	/// {"error":"not_found","reason":"missing"}
	/// </remarks>
	public class ErrorInfo : IErrorInfo
	{
		/// <summary>
		/// Gets or sets the error given by CouchDB.
		/// </summary>
		/// <value>The error given by CouchDB.</value>
		public string Error { get; set; }

		/// <summary>
		/// Gets or sets the reason given by CouchDB.
		/// </summary>
		/// <value>The reason given by CouchDB.</value>
		public string Reason { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorInfo"/> class.
		/// </summary>
		public ErrorInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorInfo"/> class.
		/// </summary>
		/// <param name="error">The error given by CouchDB.</param>
		/// <param name="reason">The reason given by CouchDB.</param>
		public ErrorInfo(string error, string reason)
		{
			Error = error;
			Reason = reason;
		}
	}
}