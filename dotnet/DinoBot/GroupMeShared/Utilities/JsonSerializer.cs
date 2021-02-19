// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GroupMeShared.Utilities
{
    /// <summary>
    /// Handles serialization / deserialization of JSON objects
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// Deserializes an HTTP result formatted in JSON
        /// </summary>
        /// <typeparam name="T">Type of to which the data should be deserialized</typeparam>
        /// <param name="response">HTTP response</param>
        /// <returns>The JSON data deserialized as the specified object</returns>
        public static async Task<T> DeserializeJsonAsync<T>(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            return DeserializeJson<T>(responseContent);
        }

        /// <summary>
        /// Deserializes a json message from a string
        /// </summary>
        /// <typeparam name="T">Type of data to deserialize the JSON into</typeparam>
        /// <param name="result">JSON content.</param>
        /// <returns>
        /// The JSON deserialized as the specified object
        /// </returns>
        public static T DeserializeJson<T>(string result)
        {
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// Serializes an object into JSON data that can be passed to a web service
        /// </summary>
        /// <typeparam name="T">Type of data to serialize</typeparam>
        /// <param name="parameter">Data to serialize</param>
        /// <returns>JSON-formatted version of <paramref name="parameter"/></returns>
        public static StringContent SerializeToJson<T>(T parameter)
        {
            string content = JsonConvert.SerializeObject(parameter);
            StringContent result = new StringContent(content, Encoding.UTF8, "application/json");
            return result;
        }
    }
}