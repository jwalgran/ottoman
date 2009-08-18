#region License

// <copyright file="Manager.cs" company="SineSignal, LLC.">
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
using System.Collections.Generic;

namespace SineSignal.Ottoman.Tests.SampleDomain
{
	public class Manager : Employee
	{
		public IList<Worker> Subordinates { get; set; }

		public Manager(Guid id, string name, string login, IList<Worker> subordinates)
			: base(id, name, login)
		{
			Subordinates = subordinates;
		}

		public static Manager CreateManager()
		{
			var bobOriginal = new Worker(Guid.NewGuid(), "Bob", "bbob", new Address { Street = "123 Somewhere St.", City = "Kalamazoo", State = "MI", Zip = "12345" }, 40);
			var aliceOriginal = new Worker(Guid.NewGuid(), "Alice", "aalice", new Address { Street = "123 Somewhere St.", City = "Kalamazoo", State = "MI", Zip = "12345" }, 40);
			var eveOriginal = new Worker(Guid.NewGuid(), "Eve", "eeve", new Address { Street = "123 Somewhere St.", City = "Kalamazoo", State = "MI", Zip = "12345" }, 20);
			var chrisOriginal = new Manager(default(Guid), "Chris", "cchandler", new List<Worker> { bobOriginal, aliceOriginal, eveOriginal });

			return chrisOriginal;
		}
	}
}