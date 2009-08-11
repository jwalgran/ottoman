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

namespace SineSignal.Ottoman
{
	public class CouchInstance : ICouchInstance
	{
		public IProxy Proxy { get; private set; }

		public CouchInstance(IProxy proxy)
		{
			if (proxy == null)
				throw new ArgumentNullException("proxy", "The value cannot be null.");

			Proxy = proxy;
		}

		public void CreateDatabase(string name)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name", "The value cannot be null or an empty string.");

			string result = Proxy.Put(name);
		}
	}
}