#region License

// <copyright file="Employee.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman.Tests.SampleDomain
{
	public class Employee
	{
		public Guid Id { get; protected set; }
		public string Name { get; set; }
		public string Login { get; set; }

		public Employee(Guid id, string name, string login)
		{
			Id = id;
			Name = name;
			Login = login;
		}

		public override bool Equals(object obj)
		{
			Employee compareTo = obj as Employee;

			if (compareTo != null)
			{
				if (Id == compareTo.Id && Name == compareTo.Name && Login == compareTo.Login)
				{
					return true;
				}
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}