#region License

// <copyright file="JsonSerializer.cs" company="SineSignal, LLC.">
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SineSignal.Ottoman.Serializers
{
	/// <summary>
	/// An adapter around the JSON serialization library being used by Ottoman.
	/// </summary>
	public class JsonSerializer : ISerializer
	{
		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>The settings.</value>
		public JsonSerializerSettings Settings { get; private set; }

		/// <summary>
		/// Gets the type of the content.
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentType
		{
			get { return "application/json"; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonSerializer"/> class.
		/// </summary>
		public JsonSerializer()
		{
			Settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
		}

		/// <summary>
		/// Serializes the specified object to JSON.
		/// </summary>
		/// <typeparam name="T">Type of object</typeparam>
		/// <param name="objectToSerialize">The object to serialize.</param>
		/// <returns>A JSON formatted string</returns>
		public string Serialize<T>(T objectToSerialize)
		{
			return JsonConvert.SerializeObject(objectToSerialize, Formatting.None, Settings);
		}

		/// <summary>
		/// Deserializes the specified JSON.
		/// </summary>
		/// <typeparam name="T">The type to map the JSON to.</typeparam>
		/// <param name="json">The JSON to deserialize.</param>
		/// <returns>T</returns>
		public T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		/// <summary>
		/// Removes the specified key from the serialized JSON using Json.Net LINQ.
		/// </summary>
		/// <param name="json">The json to remove key from.</param>
		/// <param name="key">The key to remove.</param>
		/// <returns>JSON string without the key.</returns>
		public string RemoveKeyFrom(string json, string key)
		{
			JObject jObject = JObject.Parse(json);
			jObject.Remove(key);
			return jObject.ToString();
		}

		/// <summary>
		/// Adds the specified key to the serialized JSON using Json.Net LINQ.
		/// </summary>
		/// <param name="json">The json to add key to.</param>
		/// <param name="key">The key to add.</param>
		/// <param name="value">The value of the key.</param>
		/// <returns>JSON string with the added key.</returns>
		public string AddKeyTo(string json, string key, string value)
		{
			JObject jObject = JObject.Parse(json);
			jObject.Add(key, JToken.FromObject(value));
			return jObject.ToString();
		}
	}
}