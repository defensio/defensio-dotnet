using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Defensio.Api.Rest {

  /// <summary>
  /// Response from a REST HTTP request.
  /// </summary>
  public class RestResponse {
    /// <summary>
    /// HTTP status code of the response.
    /// </summary>
    public int HttpStatusCode { get; set; }
    /// <summary>
    /// Contents of the response.
    /// </summary>
    public string Content { get; set; }
  }

  /// <summary>
  /// A simple REST client.
  /// </summary>
  public class RestClient {

    static RestClient() {
      ServicePointManager.Expect100Continue = false;
    }

    /// <summary>
    /// The user agent to use.
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// Makes a GET request.
    /// </summary>
    /// <param name="path">Path for the request.</param>
    /// <param name="queryString">Query string for the request.</param>
    /// <returns>A rest response.</returns>
    public RestResponse Get(string path, string queryString = null) {
      return MakeRequest(path, queryString: queryString, method: "GET");
    }

    /// <summary>
    /// Makes a PUT request.
    /// </summary>
    /// <param name="path">Path for the request.</param>
    /// <param name="putData">Data for the request.</param>
    /// <returns>A rest response.</returns>
    public RestResponse Put(string path, string putData) {
      return MakeRequest(path, postData: putData, method: "PUT");
    }

    /// <summary>
    /// Makes a POST request.
    /// </summary>
    /// <param name="path">Path for the request.</param>
    /// <param name="postData">Data for the request.</param>
    /// <returns>A rest response.</returns>
    public RestResponse Post(string path, string postData) {
      return MakeRequest(path, postData: postData, method: "POST");
    }

    RestResponse MakeRequest(string path, string method, string queryString = null, string postData = null) {
      var fullPath = path + (queryString != null ? "?" + queryString : "");
      Trace.WriteLine(method + " " + fullPath);
      var req = (HttpWebRequest)WebRequest.Create(fullPath);
      req.KeepAlive = false;
      req.UserAgent = UserAgent;
      req.Method = method;
      if (postData != null) {
        req.ContentType = "application/x-www-form-urlencoded";
        InputPostData(postData, req);
      }
      return MakeRequest(req);
    }

    void InputPostData(string postData, HttpWebRequest req) {
      byte[] data = Encoding.ASCII.GetBytes(postData);
      req.ContentLength = data.Length;
      using (var input = req.GetRequestStream()) {
        input.Write(data, 0, data.Length);
      }
    }

    RestResponse MakeRequest(HttpWebRequest req) {
      try {
        using (var response = (HttpWebResponse)req.GetResponse())
        using (var reader = new StreamReader(response.GetResponseStream())) {
          var content = reader.ReadToEnd();
          Trace.WriteLine(content);
          return new RestResponse {
            HttpStatusCode = (int)response.StatusCode,
            Content = content
          };
        }
      }
      catch (WebException ex) {
        if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError) throw;
        var response = (HttpWebResponse)ex.Response;
        return new RestResponse { HttpStatusCode = (int)response.StatusCode };
      }
    }
  
  }

}
