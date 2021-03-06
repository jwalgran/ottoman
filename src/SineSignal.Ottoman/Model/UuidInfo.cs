#region License

// <copyright file="UuidInfo.cs" company="SineSignal, LLC.">
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

namespace SineSignal.Ottoman.Model
{
	/// <summary>
	/// Models the response given by CouchDB when requesting Uuids.
	/// </summary>
	public class UuidInfo : IUuidInfo
	{
		/// <summary>
		/// Gets or sets the uuids.
		/// </summary>
		/// <value>The uuids.</value>
		public Guid[] Uuids { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UuidInfo"/> class.
		/// </summary>
		/// <param name="uuids">The uuids.</param>
		public UuidInfo(Guid[] uuids)
		{
			Uuids = uuids;
		}
	}
}