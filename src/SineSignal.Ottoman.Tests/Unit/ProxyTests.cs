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

using MbUnit.Framework;

namespace SineSignal.Ottoman.Tests.Unit
{
	[TestFixture]
	public class ProxyTests
	{
		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_argument_null_exception_when_url_is_null_or_empty_string()
		{
			Proxy proxy = new Proxy(null);
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_a_random_string()
		{
			Proxy proxy = new Proxy("some string value");
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_relative()
		{
			Proxy proxy = new Proxy("../somepath");
		}

		[Test]
		[ExpectedArgumentException]
		public void Should_throw_argument_exception_when_url_is_not_using_http_or_https_scheme()
		{
			string url = "ftp://127.0.0.1/somepath";

			Proxy proxy = new Proxy(url);

			Assert.IsNotNull(proxy.Uri);
			Assert.AreEqual(url, proxy.Uri.ToString());
		}

		[Test]
		public void Should_set_uri_for_couch_location_when_given_a_valid_http_uri()
		{
			string url = "http://127.0.0.1:5984/";

			Proxy proxy = new Proxy(url);

			Assert.AreEqual(url, proxy.Uri.ToString());
			Assert.AreEqual("http", proxy.Uri.Scheme);
		}

		[Test]
		public void Should_set_uri_for_couch_location_when_given_a_valid_https_uri()
		{
			string url = "https://domain.com:5984/";

			Proxy proxy = new Proxy(url);

			Assert.AreEqual(url, proxy.Uri.ToString());
			Assert.AreEqual("https", proxy.Uri.Scheme);
		}
	}
}