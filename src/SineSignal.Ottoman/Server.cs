#region License

// <copyright file="Server.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Exceptions;
using SineSignal.Ottoman.Model;
using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// A class for managing databases on a CouchDB server.  Use this class for creating, deleting, retrieving, and for listing databases on your CouchDB server.
	/// </summary>
	/// <remarks>Please use the ServerFactory.Create factory method for creating instances of this class.</remarks>
	public sealed class Server : IServer
	{
		/// <summary>
		/// Gets the Address of the CouchDB server being used by the API.
		/// </summary>
		/// <value>The Address of the CouchDB server being used by the API.</value>
		public Uri Address { get; private set; }

		/// <summary>
		/// Gets or sets the proxy to use for talking to the CouchDB server.
		/// </summary>
		/// <value>The rest proxy.</value>
		public IRestClient RestClient { get; private set; }
		
		/// <summary>
		/// Gets or sets the serializer to use for deserializing responses from the proxy.
		/// </summary>
		/// <value>The serializer.</value>
		public ISerializer Serializer { get; private set; }

		internal Server(Uri address, IRestClient restClient, ISerializer serializer)
		{
			Address = address;
			RestClient = restClient;
			Serializer = serializer;
		}

		/// <summary>
		/// A factory method for connecting a new <see cref="Server"/> instance to the default address.
		/// </summary>
		/// <returns><see cref="Server"/></returns>
		public static Server Connect()
		{
			return Connect("http://127.0.0.1:5984/");
		}

		/// <summary>
		/// A factory method for connecting a new <see cref="Server"/> instance to the address given.
		/// </summary>
		/// <param name="address">The Address of the CouchDB server to be used by the API.</param>
		/// <exception cref="ArgumentNullException">Throws an exception if the url parameter is null or empty string.</exception>
		/// <exception cref="UriFormatException">Throws an exception if the url parameter is not a valid URI.</exception>
		/// <exception cref="ArgumentException">Throws an exception if the url parameter doesn't use the HTTP or HTTPS protocol.</exception>
		/// <returns><see cref="Server"/></returns>
		public static Server Connect(string address)
		{
			if (String.IsNullOrEmpty(address))
			    throw new ArgumentNullException("address", "The value cannot be null or an empty string.");

			Uri parsedAddress;
			bool isValidUrl = Uri.TryCreate(address, UriKind.Absolute, out parsedAddress);
			if (!isValidUrl)
			    throw new UriFormatException("The value is invalid, please pass a valid Uri.");

			if (!parsedAddress.Scheme.Equals("http") && !parsedAddress.Scheme.Equals("https"))
			    throw new ArgumentException("The value is not using the http or https protocol.  This is not allowed since CouchDB uses REST and Http for communication.", "url");

			IRestClient restClient = new RestClient();
			ISerializer serializer = new JsonSerializer();

			return new Server(parsedAddress, restClient, serializer);
		}

		/// <summary>
		/// Creates a database in CouchDB with the given name.
		/// </summary>
		/// <param name="name">The name of the database to create.</param>
		/// <exception cref="ArgumentNullException">Throws an exception if the name parameter is null or empty string.</exception>
		/// <exception cref="CannotCreateDatabaseException">Throws an exception if the database cannot be created.</exception>
		public void CreateDatabase(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "The value cannot be null or an empty string.");

			Uri requestUrl = BuildUrlFor(name);

			IHttpResponse response = RestClient.Put(requestUrl);
			
			if (response.StatusCode != HttpStatusCode.Created)
			{
				IErrorInfo error = Serializer.Deserialize<ErrorInfo>(response.Body);
				throw new CannotCreateDatabaseException(name, error, response);
			}
		}

		/// <summary>
		/// Deletes a database in CouchDB with the given name.
		/// </summary>
		/// <param name="name">The name of the database to delete.</param>
		/// <exception cref="ArgumentNullException">Throws an exception if the name parameter is null or empty string.</exception>
		/// <exception cref="CannotDeleteDatabaseException">Throws an exception if the database cannot be deleted.</exception>
		public void DeleteDatabase(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			Uri requestUrl = BuildUrlFor(name);

			IHttpResponse response = RestClient.Delete(requestUrl);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				IErrorInfo error = Serializer.Deserialize<ErrorInfo>(response.Body);
				throw new CannotDeleteDatabaseException(name, error, response);
			}
		}

		/// <summary>
		/// Gets a database in CouchDB with the given name.
		/// </summary>
		/// <param name="name">The name of the database to get.</param>
		/// <exception cref="ArgumentNullException">Throws an exception if the name parameter is null or empty string.</exception>
		/// <exception cref="CannotGetDatabaseException">Throws an exception if the database cannot be retrieved.</exception>
		public IDatabase GetDatabase(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			Uri requestUrl = BuildUrlFor(name);

			IHttpResponse response = RestClient.Get(requestUrl);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				IErrorInfo error = Serializer.Deserialize<ErrorInfo>(response.Body);
				throw new CannotGetDatabaseException(name, error, response);
			}

			IDatabaseInfo databaseInfo = Serializer.Deserialize<DatabaseInfo>(response.Body);
			
			return new Database(this, databaseInfo, RestClient, Serializer);
		}

		/// <summary>
		/// Gets a list of databases on the CouchDB server.
		/// </summary>
		/// <returns>string[] of database names.</returns>
		public string[] GetDatabases()
		{
			Uri requestUrl = BuildUrlFor("_all_dbs");

			IHttpResponse response = RestClient.Get(requestUrl);

			return Serializer.Deserialize<string[]>(response.Body);
		}

		/// <summary>
		/// Gets info about the CouchDB server.
		/// </summary>
		/// <returns><see cref="ServerInfo" /></returns>
		public IServerInfo GetInfo()
		{
			IHttpResponse response = RestClient.Get(Address);

			return Serializer.Deserialize<ServerInfo>(response.Body);
		}

		/// <summary>
		/// Creates a get request to the CouchDB server to generate a specified number of UUIDS.
		/// </summary>
		/// <param name="count">The number of UUIDS you want generated.</param>
		/// <returns>An array of <see cref="Guid" /></returns>
		public Guid[] GetUuids(int count)
		{
			Uri requestUrl = BuildUrlFor("_uuids", "count=" + count);
			
			IHttpResponse response = RestClient.Get(requestUrl);
			
			IUuidInfo wrapper = Serializer.Deserialize<UuidInfo>(response.Body);

			return wrapper.Uuids;
		}
		
		// TODO:  Move these to a more reusable spot.  Not sure where that will be at the moment.
		private Uri BuildUrlFor(string path)
		{
			return BuildUrlFor(path, null);
		}

		private Uri BuildUrlFor(string path, string query)
		{
			// TODO:  We need to UrlEncode the path, to take care of special character /.
			// We just introduced a limitation with this API.  System.Uri, does not handle the encoding correctly of /.  Hold off on this for now.
			// Until we can figure out a way to do this, we need a regex to check for this characters and throw an Exception
			UriBuilder requestUrl = new UriBuilder(Address);
			requestUrl.Path = path;
			
			if (!String.IsNullOrEmpty(query))
			{
				requestUrl.Query = query;
			}

			return requestUrl.Uri;
		}
	}
}