using System;
using System.Collections;
using System.Web;
using Defensio.Api;
using Defensio.Api.Rest;
using NUnit.Framework;

namespace Defensio.Tests {
  [TestFixture]
  public class DefensioFixture {

    [Test]
    public void TestGetUser() {
      _defensio.Payload = @"<?xml version='1.0' encoding='UTF-8'?>
        <defensio-result>
          <owner-url>example.org</owner-url>
          <api-version>2.0</api-version>
          <status>success</status>
          <message>Sample message</message>
        </defensio-result>";
      var res = _defensio.GetUser();
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
      Assert.AreEqual("2.0", res["api-version"]);
      Assert.AreEqual("Sample message", res["message"]);
      Assert.AreEqual("example.org", res["owner-url"]);
    }

    [Test]
    public void TestGetBasicStats() {
      _defensio.Payload = @"<?xml version='1.0' encoding='UTF-8'?>
        <defensio-result>
          <unwanted>
            <malicious type='integer'>42</malicious>
            <total type='integer'>43</total>
            <spam type='integer'>44</spam>
          </unwanted>
          <legitimate>
            <total type='integer'>10</total>
          </legitimate>
          <accuracy type='float'>0.95</accuracy>
          <learning-status>Accuracy will remain volatile until your filter is more mature.</learning-status>
          <api-version>2.0</api-version>
          <learning type='boolean'>true</learning>
          <false-positives type='integer'>32</false-positives>
          <false-negatives type='integer'>33</false-negatives>
          <message>Sample message</message>
          <status>success</status>
        </defensio-result>";
      var res = _defensio.GetBasicStats();
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
      Assert.AreEqual("2.0", res["api-version"]);
      Assert.AreEqual("Sample message", res["message"]);
      Assert.AreEqual(10, ((Hashtable)res["legitimate"])["total"]);
      Assert.AreEqual(42, ((Hashtable)res["unwanted"])["malicious"]);
      Assert.AreEqual(43, ((Hashtable)res["unwanted"])["total"]);
      Assert.AreEqual(44, ((Hashtable)res["unwanted"])["spam"]);
      Assert.AreEqual(.95f, (float)res["accuracy"], .01f);
      Assert.AreEqual("Accuracy will remain volatile until your filter is more mature.", res["learning-status"]);
      Assert.AreEqual(true, res["learning"]);
      Assert.AreEqual(32, res["false-positives"]);
      Assert.AreEqual(33, res["false-negatives"]);
    }

    [Test]
    public void TestGetExtendedStats() {
      _defensio.Payload = @"<?xml version='1.0' encoding='UTF-8'?>
        <defensio-result>
          <chart-urls>
            <recent-accuracy>http://example.org/chart/123456</recent-accuracy>
            <total-unwanted>http://example.org/chart/abcdef</total-unwanted>
            <total-legitimate nil='true'></total-legitimate>
          </chart-urls>
          <data type='array'>
            <datum>
              <legitimate type='integer'>100</legitimate>
              <unwanted type='integer'>500</unwanted>
              <accuracy type='float'>0.9975</accuracy>
              <date>2009-09-01</date>
              <false-negatives type='integer'>1</false-negatives>
              <false-positives type='integer'>2</false-positives>
            </datum>
            <datum>
              <legitimate type='integer'>90</legitimate>
              <unwanted type='integer'>20</unwanted>
              <accuracy type='float'>0.9876</accuracy>
              <date>2010-10-11</date>
              <false-negatives type='integer'>1</false-negatives>
              <false-positives type='integer'>3</false-positives>
            </datum>
          </data>
          <api-version>2.0</api-version>
          <message>Sample message</message>
          <status>success</status>
        </defensio-result>";
      var res = _defensio.GetExtendedStats(new Hashtable());
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
      Assert.AreEqual("2.0", res["api-version"]);
      Assert.AreEqual("Sample message", res["message"]);
      Assert.AreEqual("http://example.org/chart/123456", ((Hashtable)res["chart-urls"])["recent-accuracy"]);
      Assert.AreEqual("http://example.org/chart/abcdef", ((Hashtable)res["chart-urls"])["total-unwanted"]);
      Assert.IsNull(((Hashtable)res["chart-urls"])["total-legitimate"]);
      var data = (ArrayList)res["data"];
      Assert.AreEqual(2, data.Count);
      {
        var datum = (Hashtable)data[0];
        Assert.AreEqual(100, datum["legitimate"]);
        Assert.AreEqual(500, datum["unwanted"]);
        Assert.AreEqual(.9975f, (float)datum["accuracy"], .01f);
        Assert.AreEqual(new DateTime(2009, 09, 01), datum["date"]);
        Assert.AreEqual(1, datum["false-negatives"]);
        Assert.AreEqual(2, datum["false-positives"]);
      }
      {
        var datum = (Hashtable)data[1];
        Assert.AreEqual(90, datum["legitimate"]);
        Assert.AreEqual(20, datum["unwanted"]);
        Assert.AreEqual(.9876f, (float)datum["accuracy"], .01f);
        Assert.AreEqual(new DateTime(2010, 10, 11), datum["date"]);
        Assert.AreEqual(1, datum["false-negatives"]);
        Assert.AreEqual(3, datum["false-positives"]);
      }
    }

    [Test]
    public void TestGetExtendedStats_Fail() {
      _defensio.Payload = @"<?xml version='1.0' encoding='UTF-8'?>
        <defensio-result>
          <api-version type='float'>2.0</api-version>
          <message>The following fields are missing but required: from, to</message>
          <status>fail</status>
        </defensio-result>";
      var res = _defensio.GetExtendedStats(new Hashtable());
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("fail", res["status"]);
    }

    [Test]
    public void TestPostProfanityFilter() {
      _defensio.Payload = @"<?xml version='1.0' encoding='UTF-8'?>
        <defensio-result>
          <filtered>
            <field1>Hello World</field1>
            <other-field>Hello again</other-field>
          </filtered>
          <api-version>2.0</api-version>
          <message>Sample message</message>
          <status>success</status>
        </defensio-result>";
      var res = _defensio.PostProfanityFilter(new Hashtable {
        { "field1", "Hello World" },
        { "other_field", "Hello again" }
      });
      Assert.AreEqual("field1=Hello+World&other_field=Hello+again", _defensio.PostData);
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
      Assert.AreEqual("Sample message", res["message"]);
      var filtered = (Hashtable)res["filtered"];
      Assert.AreEqual(2, filtered.Count);
      Assert.That(filtered.ContainsKey("field1"));
      Assert.That(filtered.ContainsKey("other-field"));
    }

    string GetSampleDocumentXml() {
      return @"<?xml version='1.0' encoding='UTF-8'?>
        <defensio-result>
          <allow type='boolean'>true</allow>
          <profanity-match type='boolean'>false</profanity-match>
          <signature>eef52bf867b8ca167f64eb</signature>
          <spaminess type='float'>0.05</spaminess>
          <api-version>2.0</api-version>
          <classification>legitimate</classification>
          <message>Sample message</message>
          <status>success</status>
        </defensio-result>";
    }

    void PrepareDocumentTest() {
      _defensio.Payload = GetSampleDocumentXml();
    }

    void AssertDocumentTest(DefensioResult res) {
      Assert.AreEqual(200, res.HttpStatusCode);
      Assert.AreEqual("success", res["status"]);
      Assert.AreEqual("Sample message", res["message"]);
      Assert.AreEqual("legitimate", res["classification"]);
      Assert.AreEqual(.05f, (float)res["spaminess"], .01f);
      Assert.AreEqual("eef52bf867b8ca167f64eb", res["signature"]);
      Assert.AreEqual(false, res["profanity-match"]);
      Assert.AreEqual(true, res["allow"]);
    }

    [Test]
    public void TestPostDocument() {
      PrepareDocumentTest();
      var req = new Hashtable {
        { "content", "This is a simple test" },
        { "platform", "my_awesome_app" },
        { "type", "comment" }
      };
      var res = _defensio.PostDocument(req);
      AssertDocumentTest(res);
    }

    [Test]
    public void TestGetDocument() {
      PrepareDocumentTest();
      var res = _defensio.GetDocument("whatever");
      AssertDocumentTest(res);
    }

    [Test]
    public void TestPutDocument() {
      PrepareDocumentTest();
      var res = _defensio.PutDocument("whatever", new Hashtable { { "allow", true } });
      AssertDocumentTest(res);
    }

    [Test]
    public void TestHandlePostDocumentAsyncCallback_String() {
      var xml = GetSampleDocumentXml();
      var res = DefensioClient.HandlePostDocumentAsyncCallback(xml);
      res[DefensioResult.HttpStatusCodeKey] = 200;
      AssertDocumentTest(res);
    }

    [Test]
    public void TestHandlePostDocumentAsyncCallback_HttpRequest() {
      var xml = GetSampleDocumentXml();
      var worker = new TestingWorkerRequest(xml);
      var context = new HttpContext(worker);
      var res = DefensioClient.HandlePostDocumentAsyncCallback(context.Request);
      res[DefensioResult.HttpStatusCodeKey] = 200;
      AssertDocumentTest(res);
    }

    TestingDefensio _defensio = new TestingDefensio() { HttpStatusCode = 200 };
  }

  class TestingDefensio : DefensioClient {
    public string Payload { get; set; }
    public int HttpStatusCode { get; set; }
    public string Path { get; set; }
    public string QueryString { get; set; }
    public string PostData { get; set; }
    public TestingDefensio() : base("whatever") { }
    protected override RestResponse DoGet(string path, string queryString = null) {
      Path = path;
      QueryString = queryString;
      return new RestResponse {
        HttpStatusCode = HttpStatusCode,
        Content = Payload
      };
    }
    protected override RestResponse DoPost(string path, string postData) {
      PostData = postData;
      return DoGet(path);
    }
    protected override RestResponse DoPut(string path, string putData) {
      PostData = putData;
      return DoGet(path);
    }
  }

}

