#region License

// <copyright file="DatabaseTests.cs" company="SineSignal, LLC.">
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
	public class DatabaseTests
	{
		/* 
		 * Now it's time for the interesting functionality of Ottoman.  Making it as seamless 
		 * as possible to create, retrieve, update, and delete documents.
		 *		Let's start with creating, since we can't do the others without creating first.
		 *			Tasks for creating a document:
		 *				Automatically generate ID and assign it to the object.
		 *				Assigns a doc_type to the document, based on the type of the object passed in.
		 *				Serializes the object to JSON.
		 *				POSTS the JSON to the CouchDB server.
		 *				Verifies the POST and returns the object, otherwise throw exception.
		 */
		 [Test]
		 public void Should_be_able_to_create_document_when_createdocument_is_passed_an_object()
		 {
			 IDatabase couchDatabase = new Database("test", 0, 0, 0, 0, false, 79, "1250175373642458", 4);
		 }
	}
}