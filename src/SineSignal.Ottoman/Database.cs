#region License

// <copyright file="Database.cs" company="SineSignal, LLC.">
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
using System.Net;

using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// 
	/// </summary>
	public class Database : IDatabase
	{
		/// <summary>
		/// Gets or sets the server the database resides on.
		/// </summary>
		/// <value>The server.</value>
		public IServer Server { get; private set; }

		/// <summary>
		/// Gets or sets the info for the database.
		/// </summary>
		/// <value>The info.</value>
		public IDatabaseInfo Info { get; private set; }
		
		private Uri Root
		{
			get
			{
				UriBuilder uriBuilder = new UriBuilder(Server.Url);
				uriBuilder.Path = Info.Name;
				return uriBuilder.Uri;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Database"/> class.
		/// </summary>
		/// <param name="server">The server the database resides on.</param>
		/// <param name="info">The info about the database.</param>
		public Database(IServer server, IDatabaseInfo info)
		{
			Server = server;
			Info = info;
		}

		/// <summary>
		/// Updates the info about the database from the server.
		/// </summary>
		public void UpdateInfo()
		{
			IHttpResponse response = Server.RestProxy.Get(Root);
			Info = Server.Serializer.Deserialize<DatabaseInfo>(response.Body);
		}

		/// <summary>
		/// Saves the document to the server.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objectToPersist">The object to persist.</param>
		public void SaveDocument<T>(T objectToPersist)
		{
			Type type = typeof(T);
			
			// TODO:  Add id generators, to generate the Id
			Guid id = Guid.NewGuid();
			type.GetProperty("Id").SetValue(objectToPersist, id, null);

			ISerializer serializer = Server.Serializer;
			string json = serializer.Serialize(objectToPersist);
			string docType = type.Name;
			json = serializer.Remove(json, "Id");
			json = serializer.Add(json, "doc_type", docType);

			IRestProxy restProxy = Server.RestProxy;
			string contentType = serializer.ContentType;
			UriBuilder uriBuilder = new UriBuilder(Root);
			uriBuilder.Path = uriBuilder.Path + "/" + id;
			IHttpResponse response = restProxy.Put(uriBuilder.Uri, contentType, json);
			
			if (response.StatusCode != HttpStatusCode.Created)
			{
				// TODO:  Throw exception based on body of response
			}

			IDocument document = serializer.Deserialize<Document>(response.Body);
		}
	}
}