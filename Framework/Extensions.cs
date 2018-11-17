using System;
using System.Security.Cryptography;
using System.Xml;
using Newtonsoft.Json;

namespace Undone.Line.Framework
{
  public static class Extensions
  {
    #region XML-RSA
    public static void fromXmlString(this RSA rsa, string xmlString)
    {
      var parameters = new RSAParameters();
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.LoadXml(xmlString);

      if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
      {
        foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
        {
          switch (node.Name)
          {
            case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
            case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
            case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
            case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
            case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
            case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
            case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
            case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
          }
        }
      }
      else
      {
        throw new Exception("Invalid XML RSA key.");
      }

      rsa.ImportParameters(parameters);
    }

    public static string toXmlString(this RSA rsa, bool includePrivateParameters)
    {
      RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

      return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
            parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
            parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
            parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
            parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
            parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
            parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
            parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
            parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
    }
    #endregion

    #region JSON-RSA TODO: TESTING JSON-RSA
    public static void fromJsonString(this RSA rsa, string jsonString)
    {
      //Check.Argument.IsNotEmpty(jsonString, nameof(jsonString));
      try
      {
        var paramsJson = JsonConvert.DeserializeObject<RSAParametersJson>(jsonString);

        RSAParameters parameters = new RSAParameters();

        parameters.Modulus = paramsJson.n != null ? Convert.FromBase64String(paramsJson.n) : null;
        parameters.Exponent = paramsJson.e != null ? Convert.FromBase64String(paramsJson.e) : null;
        parameters.P = paramsJson.p != null ? Convert.FromBase64String(paramsJson.p) : null;
        parameters.Q = paramsJson.q != null ? Convert.FromBase64String(paramsJson.q) : null;
        parameters.DP = paramsJson.dp != null ? Convert.FromBase64String(paramsJson.dp) : null;
        parameters.DQ = paramsJson.dq != null ? Convert.FromBase64String(paramsJson.dq) : null;
        parameters.InverseQ = paramsJson.qi != null ? Convert.FromBase64String(paramsJson.qi) : null;
        parameters.D = paramsJson.d != null ? Convert.FromBase64String(paramsJson.d) : null;
        rsa.ImportParameters(parameters);
      }
      catch
      {
        throw new Exception("Invalid JSON RSA key.");
      }
    }

    public static string ToJsonString(this RSA rsa, bool includePrivateParameters)
    {
      RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

      var parasJson = new RSAParametersJson()
      {
        n = parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
        e = parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
        p = parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
        q = parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
        dp = parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
        dq = parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
        qi = parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
        d = parameters.D != null ? Convert.ToBase64String(parameters.D) : null
      };

      return JsonConvert.SerializeObject(parasJson);
    }
    #endregion

    public class RSAParametersJson
    {
      public string kty { get; set; }
      public string n { get; set; }
      public string e { get; set; }
      public string d { get; set; }
      public string p { get; set; }
      public string q { get; set; }
      public string dp { get; set; }
      public string dq { get; set; }
      public string qi { get; set; }
    }
  }
}