#region License

// <copyright file="ServerInfo.cs" company="SineSignal, LLC.">
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

using Newtonsoft.Json;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// Models the response given by CouchDB when retrieving info about the server.
	/// </summary>
	/// <remarks>
	/// {\"couchdb\":\"Welcome\",\"version\":\"0.10.0a800465\"}
	/// </remarks>
	public class ServerInfo : IServerInfo
	{
		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		[JsonProperty("couchdb")]
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		/// <value>The version.</value>
		public string Version { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServerInfo"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="version">The version.</param>
		public ServerInfo(string message, string version)
		{
			Message = message;
			Version = version;
		}
	}
}