using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using NUnit.Framework;

namespace Defensio.Tests {

  class TestingWorkerRequest : SimpleWorkerRequest {
    readonly string _xml;
    public TestingWorkerRequest(string xml) : base("/", "", "", "", null) {
      _xml = xml;
    }
    public override string GetHttpVerbName() {
      return "POST";
    }
    public override string GetKnownRequestHeader(int index) {
      if (index == 11) {
        // content length
        return _xml.Length.ToString();
      }
      if (index == 12) {
        // content type
        return "text/xml";
      }
      return null;
    }
    public override byte[] GetPreloadedEntityBody() {
      return Encoding.UTF8.GetBytes(_xml);
    }
    public override bool IsEntireEntityBodyIsPreloaded() { // interesting name [!]
      return true;
    }
  }

  [TestFixture]
  public class TestingWorkerRequestFixture {

    [Test]
    public void TestingWorkerRequest_ReturnsProperPayload() {
      var content = "PAYLOAD";
      var worker = new TestingWorkerRequest(content);
      var context = new HttpContext(worker);
      var result = new StreamReader(context.Request.InputStream).ReadToEnd();
      Assert.AreEqual(content, result);
    } 
  
  }

}
