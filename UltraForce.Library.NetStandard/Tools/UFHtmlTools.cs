// <copyright file="UFHtmlTools.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2018 Ultra Force Development
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace UltraForce.Library.NetStandard.Tools
{
  /// <summary>
  /// An utility class that contains HTML related methods.
  /// </summary>
  public static class UFHtmlTools
  {
    /// <summary>
    /// Generates an unique id for a DOM element. 
    /// </summary>
    /// <returns>Valid dom id</returns>
    public static string NewDomId()
    {
      return "dom-" + Guid.NewGuid();
    }
    
    /// <summary>
    /// Converts an HTML formatted string to plain string by replacing 
    /// <c>br</c> and closing <c>p</c> tags with \r\n, removing all other 
    /// html tags and replacing html entities with their unicode equivalent.
    /// </summary>
    /// <param name="aHtmlText">Html formatted string</param>
    /// <returns>String without html tags</returns>
    public static string ToPlain(string aHtmlText)
    {
      // check if aValue contains at least one html tag
      Regex regex = new Regex(
        @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>",
        RegexOptions.Singleline
      );
      if (!regex.Match(aHtmlText.ToLower()).Success)
      {
        return aHtmlText;
      }
      // remove any \n and \r
      aHtmlText = aHtmlText.Replace("\n", "").Replace("\r", "");
      // replace all multiple spaces with 1 space
      aHtmlText = Regex.Replace(aHtmlText, @"\s{2,}", " ");
      // replace </p> and <br[/]> with \n
      aHtmlText = aHtmlText
        .Replace(@"</p>", "\n")
        .Replace(@"<br>", "\n")
        .Replace(@"<br/>", "\n");
      // remove all other tags
      aHtmlText = regex.Replace(aHtmlText, "");
      // replace entities with unicode equivalents
      return EntityToUnicode(aHtmlText);
    }

    /// <summary>
    /// Adds a protocol to an url that starts with "//" or does not contain "://".
    /// </summary>
    /// <param name="anUrl">Url to check</param>
    /// <param name="aProtocol">Protocol to add</param>
    /// <returns>Url with protocol</returns>
    public static string AddProtocol(string anUrl, string aProtocol = "http")
    {
      if (string.IsNullOrEmpty(anUrl))
      {
        return "";
      }
      if (anUrl.IndexOf("//", StringComparison.Ordinal) == 0)
      {
        anUrl = aProtocol + ":" + anUrl;
      }
      else if (anUrl.IndexOf("://", StringComparison.Ordinal) < 0)
      {
        anUrl = aProtocol + "://" + anUrl;
      }
      return anUrl;
    }

    /// <summary>
    /// Convert plain text to html formatted text. It will replace special
    /// characters with their entity code and all other chars > 159 with
    /// their unicode value.
    /// <para>
    /// It will replace "\n" with "<br/>"; "\r" characters are not copied.
    /// </para>
    /// </summary>
    /// <param name="aPlainText">Plain text to convert</param>
    /// <returns>Html safe text</returns>
    public static string ToHtml(string aPlainText)
    {
      if (string.IsNullOrEmpty(aPlainText))
      {
        return string.Empty;
      }
      StringBuilder result = new StringBuilder();
      foreach (char character in UnicodeToEntity(aPlainText))
      {
        switch (character)
        {
          case '\uff1c':
            result.Append("&#65308;");
            break;
          case '\uff1e':
            result.Append("&#65310;");
            break;
          case '\r':
            break;
          case '\n':
            result.Append("<br/>");
            break;
          default:
            if (character > 159)
            {
              result.Append("&#");
              result.Append(((int)character).ToString(CultureInfo.InvariantCulture));
              result.Append(";");
            }
            else
            {
              result.Append(character);
            }
            break;
        }
      }
      return result.ToString();
    }

    /// <summary>
    /// Replace html entity definitions with their unicode equivalent
    /// <para>
    /// Entity replace code based upon code from:
    /// https://github.com/Cratesmith/RestSharp-for-unity3d/blob/master/RestSharp/Extensions/MonoHttp/HtmlEncoder.cs
    /// </para>
    /// </summary>
    /// <param name="aText">Text with entity definitions</param>
    /// <returns>String with entities replaced</returns>
    public static string EntityToUnicode(string aText)
    {
      // Not the fastest solution, but easiest.
      // See HTML 4.01 W3C recommendation.
      return aText
        .Replace("&nbsp;", "\u00A0")
        .Replace("&iexcl;", "\u00A1")
        .Replace("&cent;", "\u00A2")
        .Replace("&pound;", "\u00A3")
        .Replace("&curren;", "\u00A4")
        .Replace("&yen;", "\u00A5")
        .Replace("&brvbar;", "\u00A6")
        .Replace("&sect;", "\u00A7")
        .Replace("&uml;", "\u00A8")
        .Replace("&copy;", "\u00A9")
        .Replace("&ordf;", "\u00AA")
        .Replace("&laquo;", "\u00AB")
        .Replace("&not;", "\u00AC")
        .Replace("&shy;", "\u00AD")
        .Replace("&reg;", "\u00AE")
        .Replace("&macr;", "\u00AF")
        .Replace("&deg;", "\u00B0")
        .Replace("&plusmn;", "\u00B1")
        .Replace("&sup2;", "\u00B2")
        .Replace("&sup3;", "\u00B3")
        .Replace("&acute;", "\u00B4")
        .Replace("&micro;", "\u00B5")
        .Replace("&para;", "\u00B6")
        .Replace("&middot;", "\u00B7")
        .Replace("&cedil;", "\u00B8")
        .Replace("&sup1;", "\u00B9")
        .Replace("&ordm;", "\u00BA")
        .Replace("&raquo;", "\u00BB")
        .Replace("&frac14;", "\u00BC")
        .Replace("&frac12;", "\u00BD")
        .Replace("&frac34;", "\u00BE")
        .Replace("&iquest;", "\u00BF")
        .Replace("&Agrave;", "\u00C0")
        .Replace("&Aacute;", "\u00C1")
        .Replace("&Acirc;", "\u00C2")
        .Replace("&Atilde;", "\u00C3")
        .Replace("&Auml;", "\u00C4")
        .Replace("&Aring;", "\u00C5")
        .Replace("&AElig;", "\u00C6")
        .Replace("&Ccedil;", "\u00C7")
        .Replace("&Egrave;", "\u00C8")
        .Replace("&Eacute;", "\u00C9")
        .Replace("&Ecirc;", "\u00CA")
        .Replace("&Euml;", "\u00CB")
        .Replace("&Igrave;", "\u00CC")
        .Replace("&Iacute;", "\u00CD")
        .Replace("&Icirc;", "\u00CE")
        .Replace("&Iuml;", "\u00CF")
        .Replace("&ETH;", "\u00D0")
        .Replace("&Ntilde;", "\u00D1")
        .Replace("&Ograve;", "\u00D2")
        .Replace("&Oacute;", "\u00D3")
        .Replace("&Ocirc;", "\u00D4")
        .Replace("&Otilde;", "\u00D5")
        .Replace("&Ouml;", "\u00D6")
        .Replace("&times;", "\u00D7")
        .Replace("&Oslash;", "\u00D8")
        .Replace("&Ugrave;", "\u00D9")
        .Replace("&Uacute;", "\u00DA")
        .Replace("&Ucirc;", "\u00DB")
        .Replace("&Uuml;", "\u00DC")
        .Replace("&Yacute;", "\u00DD")
        .Replace("&THORN;", "\u00DE")
        .Replace("&szlig;", "\u00DF")
        .Replace("&agrave;", "\u00E0")
        .Replace("&aacute;", "\u00E1")
        .Replace("&acirc;", "\u00E2")
        .Replace("&atilde;", "\u00E3")
        .Replace("&auml;", "\u00E4")
        .Replace("&aring;", "\u00E5")
        .Replace("&aelig;", "\u00E6")
        .Replace("&ccedil;", "\u00E7")
        .Replace("&egrave;", "\u00E8")
        .Replace("&eacute;", "\u00E9")
        .Replace("&ecirc;", "\u00EA")
        .Replace("&euml;", "\u00EB")
        .Replace("&igrave;", "\u00EC")
        .Replace("&iacute;", "\u00ED")
        .Replace("&icirc;", "\u00EE")
        .Replace("&iuml;", "\u00EF")
        .Replace("&eth;", "\u00F0")
        .Replace("&ntilde;", "\u00F1")
        .Replace("&ograve;", "\u00F2")
        .Replace("&oacute;", "\u00F3")
        .Replace("&ocirc;", "\u00F4")
        .Replace("&otilde;", "\u00F5")
        .Replace("&ouml;", "\u00F6")
        .Replace("&divide;", "\u00F7")
        .Replace("&oslash;", "\u00F8")
        .Replace("&ugrave;", "\u00F9")
        .Replace("&uacute;", "\u00FA")
        .Replace("&ucirc;", "\u00FB")
        .Replace("&uuml;", "\u00FC")
        .Replace("&yacute;", "\u00FD")
        .Replace("&thorn;", "\u00FE")
        .Replace("&yuml;", "\u00FF")
        .Replace("&fnof;", "\u0192")
        .Replace("&Alpha;", "\u0391")
        .Replace("&Beta;", "\u0392")
        .Replace("&Gamma;", "\u0393")
        .Replace("&Delta;", "\u0394")
        .Replace("&Epsilon;", "\u0395")
        .Replace("&Zeta;", "\u0396")
        .Replace("&Eta;", "\u0397")
        .Replace("&Theta;", "\u0398")
        .Replace("&Iota;", "\u0399")
        .Replace("&Kappa;", "\u039A")
        .Replace("&Lambda;", "\u039B")
        .Replace("&Mu;", "\u039C")
        .Replace("&Nu;", "\u039D")
        .Replace("&Xi;", "\u039E")
        .Replace("&Omicron;", "\u039F")
        .Replace("&Pi;", "\u03A0")
        .Replace("&Rho;", "\u03A1")
        .Replace("&Sigma;", "\u03A3")
        .Replace("&Tau;", "\u03A4")
        .Replace("&Upsilon;", "\u03A5")
        .Replace("&Phi;", "\u03A6")
        .Replace("&Chi;", "\u03A7")
        .Replace("&Psi;", "\u03A8")
        .Replace("&Omega;", "\u03A9")
        .Replace("&alpha;", "\u03B1")
        .Replace("&beta;", "\u03B2")
        .Replace("&gamma;", "\u03B3")
        .Replace("&delta;", "\u03B4")
        .Replace("&epsilon;", "\u03B5")
        .Replace("&zeta;", "\u03B6")
        .Replace("&eta;", "\u03B7")
        .Replace("&theta;", "\u03B8")
        .Replace("&iota;", "\u03B9")
        .Replace("&kappa;", "\u03BA")
        .Replace("&lambda;", "\u03BB")
        .Replace("&mu;", "\u03BC")
        .Replace("&nu;", "\u03BD")
        .Replace("&xi;", "\u03BE")
        .Replace("&omicron;", "\u03BF")
        .Replace("&pi;", "\u03C0")
        .Replace("&rho;", "\u03C1")
        .Replace("&sigmaf;", "\u03C2")
        .Replace("&sigma;", "\u03C3")
        .Replace("&tau;", "\u03C4")
        .Replace("&upsilon;", "\u03C5")
        .Replace("&phi;", "\u03C6")
        .Replace("&chi;", "\u03C7")
        .Replace("&psi;", "\u03C8")
        .Replace("&omega;", "\u03C9")
        .Replace("&thetasym;", "\u03D1")
        .Replace("&upsih;", "\u03D2")
        .Replace("&piv;", "\u03D6")
        .Replace("&bull;", "\u2022")
        .Replace("&hellip;", "\u2026")
        .Replace("&prime;", "\u2032")
        .Replace("&Prime;", "\u2033")
        .Replace("&oline;", "\u203E")
        .Replace("&frasl;", "\u2044")
        .Replace("&weierp;", "\u2118")
        .Replace("&image;", "\u2111")
        .Replace("&real;", "\u211C")
        .Replace("&trade;", "\u2122")
        .Replace("&alefsym;", "\u2135")
        .Replace("&larr;", "\u2190")
        .Replace("&uarr;", "\u2191")
        .Replace("&rarr;", "\u2192")
        .Replace("&darr;", "\u2193")
        .Replace("&harr;", "\u2194")
        .Replace("&crarr;", "\u21B5")
        .Replace("&lArr;", "\u21D0")
        .Replace("&uArr;", "\u21D1")
        .Replace("&rArr;", "\u21D2")
        .Replace("&dArr;", "\u21D3")
        .Replace("&hArr;", "\u21D4")
        .Replace("&forall;", "\u2200")
        .Replace("&part;", "\u2202")
        .Replace("&exist;", "\u2203")
        .Replace("&empty;", "\u2205")
        .Replace("&nabla;", "\u2207")
        .Replace("&isin;", "\u2208")
        .Replace("&notin;", "\u2209")
        .Replace("&ni;", "\u220B")
        .Replace("&prod;", "\u220F")
        .Replace("&sum;", "\u2211")
        .Replace("&minus;", "\u2212")
        .Replace("&lowast;", "\u2217")
        .Replace("&radic;", "\u221A")
        .Replace("&prop;", "\u221D")
        .Replace("&infin;", "\u221E")
        .Replace("&ang;", "\u2220")
        .Replace("&and;", "\u2227")
        .Replace("&or;", "\u2228")
        .Replace("&cap;", "\u2229")
        .Replace("&cup;", "\u222A")
        .Replace("&int;", "\u222B")
        .Replace("&there4;", "\u2234")
        .Replace("&sim;", "\u223C")
        .Replace("&cong;", "\u2245")
        .Replace("&asymp;", "\u2248")
        .Replace("&ne;", "\u2260")
        .Replace("&equiv;", "\u2261")
        .Replace("&le;", "\u2264")
        .Replace("&ge;", "\u2265")
        .Replace("&sub;", "\u2282")
        .Replace("&sup;", "\u2283")
        .Replace("&nsub;", "\u2284")
        .Replace("&sube;", "\u2286")
        .Replace("&supe;", "\u2287")
        .Replace("&oplus;", "\u2295")
        .Replace("&otimes;", "\u2297")
        .Replace("&perp;", "\u22A5")
        .Replace("&sdot;", "\u22C5")
        .Replace("&lceil;", "\u2308")
        .Replace("&rceil;", "\u2309")
        .Replace("&lfloor;", "\u230A")
        .Replace("&rfloor;", "\u230B")
        .Replace("&lang;", "\u2329")
        .Replace("&rang;", "\u232A")
        .Replace("&loz;", "\u25CA")
        .Replace("&spades;", "\u2660")
        .Replace("&clubs;", "\u2663")
        .Replace("&hearts;", "\u2665")
        .Replace("&diams;", "\u2666")
        .Replace("&quot;", "\u0022")
        .Replace("&amp;", "\u0026")
        .Replace("&lt;", "\u003C")
        .Replace("&gt;", "\u003E")
        .Replace("&OElig;", "\u0152")
        .Replace("&oelig;", "\u0153")
        .Replace("&Scaron;", "\u0160")
        .Replace("&scaron;", "\u0161")
        .Replace("&Yuml;", "\u0178")
        .Replace("&circ;", "\u02C6")
        .Replace("&tilde;", "\u02DC")
        .Replace("&ensp;", "\u2002")
        .Replace("&emsp;", "\u2003")
        .Replace("&thinsp;", "\u2009")
        .Replace("&zwnj;", "\u200C")
        .Replace("&zwj;", "\u200D")
        .Replace("&lrm;", "\u200E")
        .Replace("&rlm;", "\u200F")
        .Replace("&ndash;", "\u2013")
        .Replace("&mdash;", "\u2014")
        .Replace("&lsquo;", "\u2018")
        .Replace("&rsquo;", "\u2019")
        .Replace("&sbquo;", "\u201A")
        .Replace("&ldquo;", "\u201C")
        .Replace("&rdquo;", "\u201D")
        .Replace("&bdquo;", "\u201E")
        .Replace("&dagger;", "\u2020")
        .Replace("&Dagger;", "\u2021")
        .Replace("&permil;", "\u2030")
        .Replace("&lsaquo;", "\u2039")
        .Replace("&rsaquo;", "\u203A")
        .Replace("&euro;", "\u20AC");
    }

    /// <summary>
    /// Replace certain unicode  definitions with their html entity definition
    /// </summary>
    /// <param name="aText">Text to check</param>
    /// <returns>String with entities</returns>
    public static string UnicodeToEntity(string aText)
    {
      // Not the fastest solution, but easiest.
      // See HTML 4.01 W3C recommendation.
      // make sure & is done first, so not to replace the ones in the entity
      // definitions
      return aText
        .Replace("\u0026", "&amp;")
        .Replace("\u00A0", "&nbsp;")
        .Replace("\u00A1", "&iexcl;")
        .Replace("\u00A2", "&cent;")
        .Replace("\u00A3", "&pound;")
        .Replace("\u00A4", "&curren;")
        .Replace("\u00A5", "&yen;")
        .Replace("\u00A6", "&brvbar;")
        .Replace("\u00A7", "&sect;")
        .Replace("\u00A8", "&uml;")
        .Replace("\u00A9", "&copy;")
        .Replace("\u00AA", "&ordf;")
        .Replace("\u00AB", "&laquo;")
        .Replace("\u00AC", "&not;")
        .Replace("\u00AD", "&shy;")
        .Replace("\u00AE", "&reg;")
        .Replace("\u00AF", "&macr;")
        .Replace("\u00B0", "&deg;")
        .Replace("\u00B1", "&plusmn;")
        .Replace("\u00B2", "&sup2;")
        .Replace("\u00B3", "&sup3;")
        .Replace("\u00B4", "&acute;")
        .Replace("\u00B5", "&micro;")
        .Replace("\u00B6", "&para;")
        .Replace("\u00B7", "&middot;")
        .Replace("\u00B8", "&cedil;")
        .Replace("\u00B9", "&sup1;")
        .Replace("\u00BA", "&ordm;")
        .Replace("\u00BB", "&raquo;")
        .Replace("\u00BC", "&frac14;")
        .Replace("\u00BD", "&frac12;")
        .Replace("\u00BE", "&frac34;")
        .Replace("\u00BF", "&iquest;")
        .Replace("\u00C0", "&Agrave;")
        .Replace("\u00C1", "&Aacute;")
        .Replace("\u00C2", "&Acirc;")
        .Replace("\u00C3", "&Atilde;")
        .Replace("\u00C4", "&Auml;")
        .Replace("\u00C5", "&Aring;")
        .Replace("\u00C6", "&AElig;")
        .Replace("\u00C7", "&Ccedil;")
        .Replace("\u00C8", "&Egrave;")
        .Replace("\u00C9", "&Eacute;")
        .Replace("\u00CA", "&Ecirc;")
        .Replace("\u00CB", "&Euml;")
        .Replace("\u00CC", "&Igrave;")
        .Replace("\u00CD", "&Iacute;")
        .Replace("\u00CE", "&Icirc;")
        .Replace("\u00CF", "&Iuml;")
        .Replace("\u00D0", "&ETH;")
        .Replace("\u00D1", "&Ntilde;")
        .Replace("\u00D2", "&Ograve;")
        .Replace("\u00D3", "&Oacute;")
        .Replace("\u00D4", "&Ocirc;")
        .Replace("\u00D5", "&Otilde;")
        .Replace("\u00D6", "&Ouml;")
        .Replace("\u00D7", "&times;")
        .Replace("\u00D8", "&Oslash;")
        .Replace("\u00D9", "&Ugrave;")
        .Replace("\u00DA", "&Uacute;")
        .Replace("\u00DB", "&Ucirc;")
        .Replace("\u00DC", "&Uuml;")
        .Replace("\u00DD", "&Yacute;")
        .Replace("\u00DE", "&THORN;")
        .Replace("\u00DF", "&szlig;")
        .Replace("\u00E0", "&agrave;")
        .Replace("\u00E1", "&aacute;")
        .Replace("\u00E2", "&acirc;")
        .Replace("\u00E3", "&atilde;")
        .Replace("\u00E4", "&auml;")
        .Replace("\u00E5", "&aring;")
        .Replace("\u00E6", "&aelig;")
        .Replace("\u00E7", "&ccedil;")
        .Replace("\u00E8", "&egrave;")
        .Replace("\u00E9", "&eacute;")
        .Replace("\u00EA", "&ecirc;")
        .Replace("\u00EB", "&euml;")
        .Replace("\u00EC", "&igrave;")
        .Replace("\u00ED", "&iacute;")
        .Replace("\u00EE", "&icirc;")
        .Replace("\u00EF", "&iuml;")
        .Replace("\u00F0", "&eth;")
        .Replace("\u00F1", "&ntilde;")
        .Replace("\u00F2", "&ograve;")
        .Replace("\u00F3", "&oacute;")
        .Replace("\u00F4", "&ocirc;")
        .Replace("\u00F5", "&otilde;")
        .Replace("\u00F6", "&ouml;")
        .Replace("\u00F7", "&divide;")
        .Replace("\u00F8", "&oslash;")
        .Replace("\u00F9", "&ugrave;")
        .Replace("\u00FA", "&uacute;")
        .Replace("\u00FB", "&ucirc;")
        .Replace("\u00FC", "&uuml;")
        .Replace("\u00FD", "&yacute;")
        .Replace("\u00FE", "&thorn;")
        .Replace("\u00FF", "&yuml;")
        .Replace("\u0192", "&fnof;")
        .Replace("\u0391", "&Alpha;")
        .Replace("\u0392", "&Beta;")
        .Replace("\u0393", "&Gamma;")
        .Replace("\u0394", "&Delta;")
        .Replace("\u0395", "&Epsilon;")
        .Replace("\u0396", "&Zeta;")
        .Replace("\u0397", "&Eta;")
        .Replace("\u0398", "&Theta;")
        .Replace("\u0399", "&Iota;")
        .Replace("\u039A", "&Kappa;")
        .Replace("\u039B", "&Lambda;")
        .Replace("\u039C", "&Mu;")
        .Replace("\u039D", "&Nu;")
        .Replace("\u039E", "&Xi;")
        .Replace("\u039F", "&Omicron;")
        .Replace("\u03A0", "&Pi;")
        .Replace("\u03A1", "&Rho;")
        .Replace("\u03A3", "&Sigma;")
        .Replace("\u03A4", "&Tau;")
        .Replace("\u03A5", "&Upsilon;")
        .Replace("\u03A6", "&Phi;")
        .Replace("\u03A7", "&Chi;")
        .Replace("\u03A8", "&Psi;")
        .Replace("\u03A9", "&Omega;")
        .Replace("\u03B1", "&alpha;")
        .Replace("\u03B2", "&beta;")
        .Replace("\u03B3", "&gamma;")
        .Replace("\u03B4", "&delta;")
        .Replace("\u03B5", "&epsilon;")
        .Replace("\u03B6", "&zeta;")
        .Replace("\u03B7", "&eta;")
        .Replace("\u03B8", "&theta;")
        .Replace("\u03B9", "&iota;")
        .Replace("\u03BA", "&kappa;")
        .Replace("\u03BB", "&lambda;")
        .Replace("\u03BC", "&mu;")
        .Replace("\u03BD", "&nu;")
        .Replace("\u03BE", "&xi;")
        .Replace("\u03BF", "&omicron;")
        .Replace("\u03C0", "&pi;")
        .Replace("\u03C1", "&rho;")
        .Replace("\u03C2", "&sigmaf;")
        .Replace("\u03C3", "&sigma;")
        .Replace("\u03C4", "&tau;")
        .Replace("\u03C5", "&upsilon;")
        .Replace("\u03C6", "&phi;")
        .Replace("\u03C7", "&chi;")
        .Replace("\u03C8", "&psi;")
        .Replace("\u03C9", "&omega;")
        .Replace("\u03D1", "&thetasym;")
        .Replace("\u03D2", "&upsih;")
        .Replace("\u03D6", "&piv;")
        .Replace("\u2022", "&bull;")
        .Replace("\u2026", "&hellip;")
        .Replace("\u2032", "&prime;")
        .Replace("\u2033", "&Prime;")
        .Replace("\u203E", "&oline;")
        .Replace("\u2044", "&frasl;")
        .Replace("\u2118", "&weierp;")
        .Replace("\u2111", "&image;")
        .Replace("\u211C", "&real;")
        .Replace("\u2122", "&trade;")
        .Replace("\u2135", "&alefsym;")
        .Replace("\u2190", "&larr;")
        .Replace("\u2191", "&uarr;")
        .Replace("\u2192", "&rarr;")
        .Replace("\u2193", "&darr;")
        .Replace("\u2194", "&harr;")
        .Replace("\u21B5", "&crarr;")
        .Replace("\u21D0", "&lArr;")
        .Replace("\u21D1", "&uArr;")
        .Replace("\u21D2", "&rArr;")
        .Replace("\u21D3", "&dArr;")
        .Replace("\u21D4", "&hArr;")
        .Replace("\u2200", "&forall;")
        .Replace("\u2202", "&part;")
        .Replace("\u2203", "&exist;")
        .Replace("\u2205", "&empty;")
        .Replace("\u2207", "&nabla;")
        .Replace("\u2208", "&isin;")
        .Replace("\u2209", "&notin;")
        .Replace("\u220B", "&ni;")
        .Replace("\u220F", "&prod;")
        .Replace("\u2211", "&sum;")
        .Replace("\u2212", "&minus;")
        .Replace("\u2217", "&lowast;")
        .Replace("\u221A", "&radic;")
        .Replace("\u221D", "&prop;")
        .Replace("\u221E", "&infin;")
        .Replace("\u2220", "&ang;")
        .Replace("\u2227", "&and;")
        .Replace("\u2228", "&or;")
        .Replace("\u2229", "&cap;")
        .Replace("\u222A", "&cup;")
        .Replace("\u222B", "&int;")
        .Replace("\u2234", "&there4;")
        .Replace("\u223C", "&sim;")
        .Replace("\u2245", "&cong;")
        .Replace("\u2248", "&asymp;")
        .Replace("\u2260", "&ne;")
        .Replace("\u2261", "&equiv;")
        .Replace("\u2264", "&le;")
        .Replace("\u2265", "&ge;")
        .Replace("\u2282", "&sub;")
        .Replace("\u2283", "&sup;")
        .Replace("\u2284", "&nsub;")
        .Replace("\u2286", "&sube;")
        .Replace("\u2287", "&supe;")
        .Replace("\u2295", "&oplus;")
        .Replace("\u2297", "&otimes;")
        .Replace("\u22A5", "&perp;")
        .Replace("\u22C5", "&sdot;")
        .Replace("\u2308", "&lceil;")
        .Replace("\u2309", "&rceil;")
        .Replace("\u230A", "&lfloor;")
        .Replace("\u230B", "&rfloor;")
        .Replace("\u2329", "&lang;")
        .Replace("\u232A", "&rang;")
        .Replace("\u25CA", "&loz;")
        .Replace("\u2660", "&spades;")
        .Replace("\u2663", "&clubs;")
        .Replace("\u2665", "&hearts;")
        .Replace("\u2666", "&diams;")
        .Replace("\u0022", "&quot;")
        .Replace("\u003C", "&lt;")
        .Replace("\u003E", "&gt;")
        .Replace("\u0152", "&OElig;")
        .Replace("\u0153", "&oelig;")
        .Replace("\u0160", "&Scaron;")
        .Replace("\u0161", "&scaron;")
        .Replace("\u0178", "&Yuml;")
        .Replace("\u02C6", "&circ;")
        .Replace("\u02DC", "&tilde;")
        .Replace("\u2002", "&ensp;")
        .Replace("\u2003", "&emsp;")
        .Replace("\u2009", "&thinsp;")
        .Replace("\u200C", "&zwnj;")
        .Replace("\u200D", "&zwj;")
        .Replace("\u200E", "&lrm;")
        .Replace("\u200F", "&rlm;")
        .Replace("\u2013", "&ndash;")
        .Replace("\u2014", "&mdash;")
        .Replace("\u2018", "&lsquo;")
        .Replace("\u2019", "&rsquo;")
        .Replace("\u201A", "&sbquo;")
        .Replace("\u201C", "&ldquo;")
        .Replace("\u201D", "&rdquo;")
        .Replace("\u201E", "&bdquo;")
        .Replace("\u2020", "&dagger;")
        .Replace("\u2021", "&Dagger;")
        .Replace("\u2030", "&permil;")
        .Replace("\u2039", "&lsaquo;")
        .Replace("\u203A", "&rsaquo;")
        .Replace("\u20AC", "&euro;");
    }
  }
}