#region License

// <copyright file="IDatabaseInfo.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman.Model
{
	public interface IDatabaseInfo
	{
		string Name { get; }
		int DocCount { get; }
		int DocDelCount { get; }
		int UpdateSequence { get; }
		int PurgeSequence { get; }
		bool CompactRunning { get; }
		double DiskSize { get; }
		string InstanceStartTime { get; }
		int DiskFormatVersion { get; }
	}
}