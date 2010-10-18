using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Defensio.Api.Helpers {

  static class XmlExtensions {

    public static Hashtable ToHash(string xml) {
      if (xml == null) return new Hashtable();
      var document = new XmlDocument();
      document.LoadXml(xml);
      return ProcessChildren(document.DocumentElement);
    }

    static void ProcessNode(XmlNode node, Hashtable container) {
      if (node.NodeType == XmlNodeType.Element) {
        container.Add(node.Name, ParseNode(node));
      }
    }

    static object ParseNode(XmlNode node) {
      if (IsNull(node)) {
        return null;
      }
      if (HasType(node)) {
        return ParseType(node);
      }
      if (node.Name.ToLower() == "date") {
        return ParseDate(node);
      }
      if (MustProcessChildren(node)) {
        return ProcessChildren(node);
      }
      return node.InnerText;
    }

    static bool MustProcessChildren(XmlNode node) {
      return
        node.HasChildNodes &&
        !(node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType == XmlNodeType.Text);
    }

    static Hashtable ProcessChildren(XmlNode node) {
      var result = new Hashtable();
      foreach (XmlNode child in node.ChildNodes) {
        ProcessNode(child, result);
      }
      return result;
    }

    static bool IsNull(XmlNode node) {
      if (node.Attributes["nil"] != null) {
        return bool.Parse(node.Attributes["nil"].InnerText.ToLower());
      }
      return false;
    }

    static bool HasType(XmlNode node) {
      return node.Attributes["type"] != null;
    }

    static object ParseType(XmlNode node) {
      switch (node.Attributes["type"].InnerText.ToLower()) {
        case "float":
        case "single": return float.Parse(node.InnerText);
        case "double": return double.Parse(node.InnerText);
        case "integer":
        case "int": return int.Parse(node.InnerText);
        case "long": return long.Parse(node.InnerText);
        case "boolean":
        case "bool": return bool.Parse(node.InnerText);
        case "date": return ParseDate(node);
        case "array": return ParseArray(node);
        default: throw new NotSupportedException("Unknown node type");
      }
    }

    static DateTime ParseDate(XmlNode node) {
      return DateTime.ParseExact(node.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    static ArrayList ParseArray(XmlNode node) {
      var result = new ArrayList();
      foreach (XmlNode child in node.ChildNodes) {
        result.Add(ProcessChildren(child)); // the grandchildren are the array elements
      }
      return result;
    }

  }

}
