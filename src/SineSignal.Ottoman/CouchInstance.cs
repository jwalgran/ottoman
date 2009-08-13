#region License

// <copyright file="CouchInstance.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Proxies;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// A class for managing CouchDB instances.  Use this class for creating, deleting, retrieving, and for listing databases on your CouchDB server.
	/// </summary>
	/// <remarks>Please use the CouchFactory.Create factory method for creating instances of this class.</remarks>
	public class CouchInstance : ICouchInstance
	{
		private readonly Uri _url;

		/// <summary>
		/// Gets the URL of the CouchDB server being used by the API.
		/// </summary>
		/// <value>The URL of the CouchDB server being used by the API.</value>
		public Uri Url { get { return _url; } }

		/// <summary>
		/// Gets or sets the proxy to use for talking to the CouchDB server.
		/// </summary>
		/// <value>The rest proxy.</value>
		public IRestProxy RestProxy { get; private set; }
		
		/// <summary>
		/// Gets or sets the serializer to use for deserializing responses from the proxy.
		/// </summary>
		/// <value>The serializer.</value>
		public ISerializer Serializer { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CouchInstance"/> class.
		/// </summary>
		/// <param name="url">The URL of the CouchDB server to be used by the API.</param>
		/// <param name="restProxy">The proxy to use for talking to the CouchDB server.</param>
		/// <param name="serializer">The serializer to use for deserializing responses from the proxy.</param>
		/// <exception cref="ArgumentNullException">Throws an exception if the url parameter is null or empty string.</exception>
		/// <exception cref="ArgumentException">Throws an exception if the url parameter is not a valid URI.</exception>
		/// <exception cref="ArgumentException">Throws an exception if the url parameter doesn't use the HTTP or HTTPS protocol.</exception>
		/// <exception cref="ArgumentNullException">Throws an exception if the restProxy parameter is null.</exception>
		/// <exception cref="ArgumentNullException">Throws an exception if the serializer parameter is null.</exception>
		public CouchInstance(string url, IRestProxy restProxy, ISerializer serializer)
		{
			// Validate input
			if (String.IsNullOrEmpty(url))
				throw new ArgumentNullException("url", "The value cannot be null or an empty string.");

			bool isValidUrl = Uri.TryCreate(url, UriKind.Absolute, out _url);
			if (!isValidUrl)
				throw new ArgumentException("The value is invalid, please pass a valid Uri.", "url");

			if (!_url.Scheme.Equals("http") && !_url.Scheme.Equals("https"))
				throw new ArgumentException("The value is not using the http or https protocol.  This is not allowed since CouchDB uses REST and Http for communication.", "url");
				
			if (restProxy == null)
			    throw new ArgumentNullException("restProxy", "The value cannot be null.");
			    
			if (serializer == null)
				throw new ArgumentNullException("serializer", "The value cannot be null.");

			RestProxy = restProxy;
			Serializer = serializer;
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
			
			// TODO:  We need to UrlEncode the name, to take care of special characters like _, $, (, ), +, -, and /.
			// We just introduced a limitation with this API.  System.Net.Uri, does not handle the encoding correctly.  Hold off on this for now.
			// Until we can figure out a way to do this, we need a regex to check for these characters and throw an ArgumentException

			UriBuilder requestUrl = new UriBuilder(Url);
			requestUrl.Path = name;

			IHttpResponse response = RestProxy.Put(requestUrl.Uri);
			
			if (response.StatusCode != HttpStatusCode.Created)
			{
				CouchError error = Serializer.Deserialize<CouchError>(response.Body);
				throw new CannotCreateDatabaseException(name, error, response);
			}
		}

		/// <summary>
		/// Deletes a database in CouchDB with the given name.
		/// </summary>
		/// <param name="name">The name of the database to delete.</param>
		/// <exception cref="ArgumentNullException">Throws an exception if the name parameter is null or empty string.</exception>
		public void DeleteDatabase(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			// TODO:  We need to UrlEncode the name, to take care of special characters like _, $, (, ), +, -, and /.
			// We just introduced a limitation with this API.  System.Net.Uri, does not handle the encoding correctly.  Hold off on this for now.
			// Until we can figure out a way to do this, we need a regex to check for these characters and throw an ArgumentException

			UriBuilder requestUrl = new UriBuilder(Url);
			requestUrl.Path = name;

			IHttpResponse response = RestProxy.Delete(requestUrl.Uri);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				CouchError error = Serializer.Deserialize<CouchError>(response.Body);
				throw new CannotDeleteDatabaseException(name, error, response);
			}
		}
	}
}