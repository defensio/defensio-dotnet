using System;
using System.Collections;
using System.IO;
using System.Web;
using Defensio.Api.Helpers;
using Defensio.Api.Rest;

namespace Defensio.Api {

  /// <summary>
  /// Official .NET library for Defensio API 2.0
  /// </summary>
  public class DefensioClient {
    const string ApiVersion = "2.0";
    const string ApiHost = "http://api.defensio.com";

    internal const string LibVersion = "0.9";
    const string Format = "xml";
    const string UserAgent = "Defensio-DotNet " + LibVersion;
    const string Client = "Defensio-DotNet | " + LibVersion + " | Websense, Inc. | info@defensio.com";

    readonly string _apiKey;
    readonly string _client;

	/// <summary>
    /// Creates an instance of a new Defensio API client
    /// </summary>
    /// <param name="apiKey">Your API key to access the Defensio service</param>
    /// <param name="client">The name of your application, in the format specified in our API documentation that can be found at http://defensio.com/api </param>
	public DefensioClient(string apiKey, string client = Client) {
      _apiKey = apiKey;
      _client = client;
    }

    /// <summary>
    /// Get information about the user and API key.
    /// </summary>
    /// <returns>User and API key information.</returns>
    public DefensioResult GetUser() {
      return Get(ApiPath());
    }

    /// <summary>
    /// Create and analyze a new document
    /// </summary>
    /// <param name="input">Input parameters for the request. See our API documentation at http://defensio.com/api </param>
    /// <returns>The result of the analysis.</returns>
    public DefensioResult PostDocument(Hashtable input) {
      input["client"] = input["client"] ?? _client;
      return Post(ApiPath("documents"), HashExtensions.ToUrlEncoded(input));
    }

    /// <summary>
    /// Gets the status of a previously submitted document.
    /// </summary>
    /// <param name="signature">The signature of the document to retrieve.</param>
    /// <returns>The result of the request.</returns>
    public DefensioResult GetDocument(string signature) {
      return Get(ApiPath("documents", signature));
    }

    /// <summary>
    /// Submits classification errors (false positives or false negatives).
    /// </summary>
    /// <param name="signature">The signature of the document to modify.</param>
    /// <param name="input">Input parameters for the request. See our API documentation at http://defensio.com/api </param>
    /// <returns>The result of the request.</returns>
    /// <remarks>
    /// Use this option within 30 days of posting a document.
    /// </remarks>
    public DefensioResult PutDocument(string signature, Hashtable input) {
      return Put(ApiPath("documents", signature), HashExtensions.ToUrlEncoded(input));
    }

    /// <summary>
    /// Gets basic statistics for the current API key.
    /// </summary>
    /// <returns>Basic statistics information.</returns>
    public DefensioResult GetBasicStats() {
      return Get(ApiPath("basic-stats"));
    }

    /// <summary>
    /// Gets extended statistics for the current API key.
    /// </summary>
    /// <param name="input">Input parameters for the request. See our API documentation at http://defensio.com/api </param>
    /// <returns>The result of the request.</returns>
    public DefensioResult GetExtendedStats(Hashtable input) {
      return Get(ApiPath("extended-stats"), HashExtensions.ToUrlEncoded(input));
    }

    /// <summary>
    /// Filter a set of values based on a pre-defined dictionary.
    /// </summary>
    /// <param name="input">Input parameters for the request. See our API documentation at http://defensio.com/api </param>
    /// <returns>The filtered input values.</returns>
    public DefensioResult PostProfanityFilter(Hashtable input) {
      return Post(ApiPath("profanity-filter"), HashExtensions.ToUrlEncoded(input));
    }

    /// <summary>
    /// Parses the HTTP callback received by Defensio after an asynchronous Document/POST.
    /// </summary>
    /// <param name="content">The XML data received in Defensio's callback.</param>
    /// <returns>The result of the analysis.</returns>
    public static Hashtable HandlePostDocumentAsyncCallback(string content) {
      return XmlExtensions.ToHash(content);
    }

    /// <summary>
    /// Parses the HTTP callback received by Defensio after an asynchronous Document/POST.
    /// </summary>
    /// <param name="request">ASP.NET request object.</param>
    /// <returns>The result of the analysis.</returns>
    public static Hashtable HandlePostDocumentAsyncCallback(HttpRequest request) {
      if (request == null) throw new ArgumentNullException("request");
      if (request.HttpMethod != "POST") throw new ArgumentException("Not a POST!");
      if (request.ContentType != "text/" + Format) throw new NotSupportedException("Unsupported format!");
      var xml = new StreamReader(request.InputStream).ReadToEnd();
      return HandlePostDocumentAsyncCallback(xml);
    }

    #region Rest and Deserialize
    Hashtable Get(string path, string queryString = null) {
      return Transform(DoGet(path, queryString));
    }
    Hashtable Post(string path, string postData) {
      return Transform(DoPost(path, postData));
    }
    Hashtable Put(string path, string putData) {
      return Transform(DoPut(path, putData));
    }
    Hashtable Transform(RestResponse restResponse) {
      var result = XmlExtensions.ToHash(restResponse.Content);
      result[DefensioResult.HttpStatusCodeKey] = restResponse.HttpStatusCode;
      return result;
    }
    protected virtual RestResponse DoGet(string path, string queryString = null) {
      return _restClient.Get(path, queryString);
    }
    protected virtual RestResponse DoPost(string path, string postData) {
      return _restClient.Post(path, postData);
    }
    protected virtual RestResponse DoPut(string path, string putData) {
      return _restClient.Put(path, putData);
    }
    readonly RestClient _restClient = new RestClient { UserAgent = UserAgent };
    #endregion

    string ApiPath(string action = null, string id = null) {
      string path = ApiHost + "/" + ApiVersion + "/users/" + _apiKey;
      if (action != null) path += "/" + action;
      if (id != null) path += "/" + id;
      path += "." + Format;
      return path;
    }
  }

}
