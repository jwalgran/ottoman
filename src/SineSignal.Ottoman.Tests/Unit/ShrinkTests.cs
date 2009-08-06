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
	public class ShrinkTests
	{
		/*
		 *
		 *	Let's psycho analyze the objects on the CouchDB.
		 *		First thing we need to have to talk to a CouchDB instance is a valid URI.
		 *
		 */
		[Test]
		[ExpectedArgumentNullException]
		public void Should_throw_invalid_argument_exception_if_url_is_null_or_empty_string()
		{
			// Arrange, Act, Assert
			Shrink shrink = new Shrink(null);
		}
	}
	
	public class Shrink
	{
		public Uri CouchUri { get; protected set; }
		
		public Shrink(string url)
		{
			if (String.IsNullOrEmpty(url)) throw new ArgumentNullException("url", "The value cannot be null or an empty string.");
		}
	}
}