#region License

// <copyright file="ShrinkTests.cs" company="SineSignal">
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
using MbUnit.Framework;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	public class CouchProxyTests
	{
		/*
		 *
		 *	Let's psycho analyze the objects on the CouchDB.
		 *		First thing we need to have to talk to a CouchDB instance is a valid URI.
		 *
		 */
		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_url_is_null_or_empty_string()
		{
			CouchProxy couchProxy = new CouchProxy(null);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_a_random_string()
		{
			CouchProxy couchProxy = new CouchProxy("some string value");
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_relative()
		{
			CouchProxy couchProxy = new CouchProxy("../somepath");
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_not_using_http_or_https_scheme()
		{
			string url = "ftp://127.0.0.1/somepath";

			CouchProxy couchProxy = new CouchProxy(url);

			Assert.IsNotNull(couchProxy.Uri);
			Assert.AreEqual(url, couchProxy.Uri.ToString());
		}

		[Test]
		public void Should_set_uri_for_couch_location_when_given_a_valid_http_uri()
		{
			string url = "http://127.0.0.1:5984/";

			CouchProxy couchProxy = new CouchProxy(url);

			Assert.AreEqual(url, couchProxy.Uri.ToString());
			Assert.AreEqual("http", couchProxy.Uri.Scheme);
		}

		[Test]
		public void Should_set_uri_for_couch_location_when_given_a_valid_https_uri()
		{
			string url = "https://domain.com:5984/";

			CouchProxy couchProxy = new CouchProxy(url);

			Assert.AreEqual(url, couchProxy.Uri.ToString());
			Assert.AreEqual("https", couchProxy.Uri.Scheme);
		}
	}

	public class CouchProxy
	{
		private readonly Uri _uri;

		public Uri Uri { get { return _uri; } }

		public CouchProxy(string couchUrl)
		{
			// Validate input
			if (String.IsNullOrEmpty(couchUrl))
				throw new ArgumentNullException("couchUrl", "The value cannot be null or an empty string.");

			bool isValidUri = Uri.TryCreate(couchUrl, UriKind.Absolute, out _uri);
			if (!isValidUri)
				throw new ArgumentException("The value is invalid, please pass a valid Uri.", "couchUrl");

			if (!_uri.Scheme.Equals("http") && !_uri.Scheme.Equals("https"))
				throw new ArgumentException("The value is not using the http or https protocol.  This is not allowed since CouchDB uses REST and Http for communication.", "couchUrl");
		}
	}
}