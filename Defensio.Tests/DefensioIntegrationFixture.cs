using System;
using System.Collections;
using System.Threading;
using Defensio.Api;
using NUnit.Framework;

namespace Defensio.Tests {

  [TestFixture]
  [Category("Integration")]
  public class DefensioIntegrationFixture {

    [Test]
    public void TestGetUser() {
      var res = _defensio.GetUser();
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
    }

    [Test]
    public void TestGetUser_404() {
      var res = new DefensioClient("should-return-404").GetUser();
      Assert.AreEqual(404, res.HttpStatusCode);
    }

    [Test]
    public void TestGetBasicStats() {
      var res = _defensio.GetBasicStats();
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
    }

    [Test]
    public void TestGetExtendedStats() {
      var res = _defensio.GetExtendedStats(new Hashtable { 
        { "from", "2010-01-01" },
        { "to", "2010-10-10" }
      });
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
    }

    [Test]
    public void TestGetExtendedStats_Fail() {
      var res = _defensio.GetExtendedStats(new Hashtable { 
        { "from", "bogus" }
      });
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("fail", res["status"]);
    }

    [Test]
    public void TestPostProfanityFilter() {
      var res = _defensio.PostProfanityFilter(new Hashtable {
        { "field1", "Hello World" },
        { "other_field", "Hello again" }
      });
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
      var filtered = (Hashtable)res["filtered"];
      Assert.AreEqual(2, filtered.Count);
      Assert.That(filtered.ContainsKey("field1"));
      Assert.That(filtered.ContainsKey("other-field"));
    }

    [Test]
    public void TestPostDocument() {
      var req = new Hashtable {
        { "content", "This is a simple test" },
        { "platform", "my_awesome_app" },
        { "type", "comment" }
      };
      var res = _defensio.PostDocument(req);
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
    }

    [Test]
    public void TestGetDocument_404() { 
      var res = _defensio.GetDocument("should-return-404");
      Assert.AreEqual(404, res.HttpStatusCode);
    }

    [Test]
    public void TestPostGetPutDocument() { 
      // post
      var data = new Hashtable { { "content", "This is a simple test" }, { "platform", "my_awesome_app" }, { "type", "comment" } };
      var body = _defensio.PostDocument(data);
      Assert.AreEqual(200, body.HttpStatusCode);
      Assert.AreEqual("success", body["status"]);

      // keep some variables around
      var originalAllowResult = (bool)body["allow"];
      var signature = (string)body["signature"];

      // give Defensio some time to process
      Thread.Sleep(500);

      // GET
      body = _defensio.GetDocument(signature);
      Assert.AreEqual(200, body.HttpStatusCode);
      Assert.AreEqual("success", body["status"]);
      Assert.AreEqual(signature, body["signature"]);

      // PUT
      body = _defensio.PutDocument(signature, new Hashtable { { "allow", !originalAllowResult } });
      Assert.AreEqual(200, body.HttpStatusCode);
      Assert.AreEqual("success", body["status"]);
      Assert.AreEqual(signature, body["signature"]);
      Assert.AreEqual(!originalAllowResult, body["allow"]);

      // PUT (back to original value)
      body = _defensio.PutDocument(signature, new Hashtable { { "allow", originalAllowResult } });
      Assert.AreEqual(200, body.HttpStatusCode);
      Assert.AreEqual("success", body["status"]);
      Assert.AreEqual(signature, body["signature"]);
      Assert.AreEqual(originalAllowResult, body["allow"]);
    }

    DefensioClient _defensio = new DefensioClient(GetApiKey());

    static string GetApiKey() {
      
      // ------------------------------------------------
      // --  Set API key here for integration testing  --
      // ------------------------------------------------
      var key = "";
      // ------------------------------------------------	

      if (key == "") {
        Assert.Fail("You must specify a Defensio API key in DefensioIntegrationFixture.GetApiKey() before running the tests.");
      }
      return key;
    }
  }

}

