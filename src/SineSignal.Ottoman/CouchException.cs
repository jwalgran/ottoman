using System;
using SineSignal.Ottoman.Proxies;

namespace SineSignal.Ottoman
{
	/// <summary>
	/// A custom exception type to use for handling when things go boom in the night while communicating with CouchDB.
	/// </summary>
	public class CouchException : Exception
	{
		/// <summary>
		/// The error that CouchDB gave.
		/// </summary>
		/// <value>The error.</value>
		public ICouchError CouchError { get; private set; }

		/// <summary>
		/// Gets or sets the raw response from the CouchDB server.
		/// </summary>
		/// <value>The raw response.</value>
		public IHttpResponse RawResponse { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CouchException"/> class.
		/// </summary>
		/// <param name="message">Message to use for the exception.</param>
		/// <param name="couchError">The error that CouchDB gave.</param>
		/// <param name="rawResponse">The raw response from the CouchDB server.</param>
		public CouchException(string message, ICouchError couchError, IHttpResponse rawResponse) 
			: base(message)
		{
			CouchError = couchError;
			RawResponse = rawResponse;
		}
	}
}