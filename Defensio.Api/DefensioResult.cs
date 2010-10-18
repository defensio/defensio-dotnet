using System.Collections;

namespace Defensio.Api {

  /// <summary>
  /// Represents the result of a Defensio API call.
  /// </summary>
  public class DefensioResult {

    /// <summary>
    /// Key to the HTTP Status code stored.
    /// </summary>
    public const string HttpStatusCodeKey = "http-status-code";

    readonly Hashtable _results;
    public DefensioResult(Hashtable results) {
      _results = results;
    }

    /// <summary>
    /// HTTP Status code of the call.
    /// </summary>
    public int HttpStatusCode {
      get { return (int)_results[HttpStatusCodeKey]; }
    }

    /// <summary>
    /// Access a specific result object.
    /// </summary>
    /// <param name="key">Key to the result object.</param>
    /// <returns>A result object. Can be a <see cref="Hashtable"/>, an <see cref="ArrayList"/>, or a value.</returns>
    public object this[string key] {
      get { return _results[key]; }
    }

    public static implicit operator DefensioResult(Hashtable results) {
      return new DefensioResult(results);
    }

  }

}
