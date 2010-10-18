using System.Collections;
using System.Text;
using System.Web;

namespace Defensio.Api.Helpers {

  static class HashExtensions {

    public static string ToUrlEncoded(Hashtable hash) {
      var sb = new StringBuilder();
      var keys = new ArrayList(hash.Keys);
      keys.Sort(); // make the output deterministic
      foreach (string key in keys) {
        sb.AppendFormat("{0}={1}&",
          HttpUtility.UrlEncode(key.ToString()),
          HttpUtility.UrlEncode(hash[key].ToString()));
      }
      if (sb.Length > 1) --sb.Length; // trim last "&"
      return sb.ToString();
    }

  }

}
