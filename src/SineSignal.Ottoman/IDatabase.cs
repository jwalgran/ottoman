#region License

// <copyright file="IDatabase.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman
{
	public interface IDatabase
	{
		string Name { get; set; }
		int DocCount { get; set; }
		int DocDelCount { get; set; }
		int UpdateSequence { get; set; }
		int PurgeSequence { get; set; }
		bool CompactRunning { get; set; }
		double DiskSize { get; set; }
		string InstanceStartTime { get; set; }
		int DiskFormatVersion { get; set; }
	}
}