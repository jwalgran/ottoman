#region License

// <copyright file="IServer.cs" company="SineSignal, LLC.">
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

using SineSignal.Ottoman.Proxy;
using SineSignal.Ottoman.Serializers;

namespace SineSignal.Ottoman
{
	public interface IServer
	{
		Uri Url { get; }
		IRestClient RestClient { get; }
		ISerializer Serializer { get; }
		void CreateDatabase(string name);
		void DeleteDatabase(string name);
		IDatabase GetDatabase(string name);
		string[] GetDatabases();
		IServerInfo GetInfo();
		Guid[] GetUuids(int count);
	}
}