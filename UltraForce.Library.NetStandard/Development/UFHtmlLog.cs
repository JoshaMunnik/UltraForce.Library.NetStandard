// <copyright file="UFHtmlLog.cs" company="Ultra Force Development">
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Development
{
  /// <summary>
  /// <see cref="UFHtmlLog" /> implements a log using HTML formatting for every  entry. New messages get added to log
  /// and are output to the general debugger log via <see cref="Debug.WriteLine(string)" />.
  /// <para>
  /// This class only has functionality if UFDEBUG has been defined else the methods will do nothing.
  /// </para>
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UFHtmlLog
  {
#if UFDEBUG

    #region private consts

    /// <summary>
    /// Colors for different type messages
    /// </summary>
    private const string ErrorColor = "#ff0000";

    /// <summary>
    /// No color
    /// </summary>
    private const string NoColor = "";

    #endregion

    #region private variables

    /// <summary>
    /// Current log.
    /// </summary>
    private readonly StringBuilder? m_log;

    /// <summary>
    /// Used for thread safe access.
    /// </summary>
    private readonly object? m_lock;

    /// <summary>
    /// Action to use to output log line
    /// </summary>
    private readonly Action<string>? m_outputLine;

    #endregion

#endif

    #region constructors & destructors

    /// <summary>
    /// Constructs instance of <see cref="UFHtmlLog" /> using
    /// <see cref="Debug.WriteLine(object)"/> to output log lines to.
    /// </summary>
    public UFHtmlLog() : this(line => Debug.WriteLine(line))
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFHtmlLog"/>
    /// </summary>
    /// <param name="anOutputLine">
    /// Action that is called to output log lines to
    /// </param>
    public UFHtmlLog(Action<string> anOutputLine)
    {
#if UFDEBUG
      this.m_outputLine = anOutputLine;
      this.m_log = new StringBuilder(8192);
      this.m_lock = new object();
#endif
    }

    #endregion

    #region public methods

    /// <summary>
    /// Adds an entry to log.
    /// </summary>
    /// <param name="anObject">Object which type name will be used as tag</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    [Conditional("UFDEBUG")]
    public void Add(object anObject, string aMessage, params object[] anArguments)
    {
#if UFDEBUG
      this.Add(anObject.GetType()
          .Name,
        aMessage,
        anArguments);
#endif
    }

    /// <summary>
    /// Adds an error entry to the log.
    /// </summary>
    /// <param name="anObject">Object which type name will be used as tag</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    [Conditional("UFDEBUG")]
    public void Error(object anObject, string aMessage, params object[] anArguments)
    {
#if UFDEBUG
      this.Error(anObject.GetType()
          .Name,
        aMessage,
        anArguments);
#endif
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// </summary>
    /// <param name="anObject">Tag to use</param>
    /// <param name="anError">Error to add</param>
    [Conditional("UFDEBUG")]
    public void Error(object anObject, Exception anError)
    {
#if UFDEBUG
      this.Error(anObject.GetType()
          .Name,
        anError);
#endif
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// <param name="anObject">Object which type name will be used as tag</param>
    /// <param name="anError">Error to add</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    /// </summary>
    [Conditional("UFDEBUG")]
    public void Error(
      object anObject,
      Exception anError,
      string aMessage,
      params object[] anArguments
    )
    {
#if UFDEBUG
      this.Error(anObject.GetType()
          .Name,
        anError,
        aMessage,
        anArguments);
#endif
    }

    /// <summary>
    /// Adds an entry to log.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    [Conditional("UFDEBUG")]
    public void Add(string aTag, string aMessage, params object[] anArguments)
    {
#if UFDEBUG
      this.AddLogColor(aTag, aMessage, NoColor, anArguments);
#endif
    }

    /// <summary>
    /// Adds an error entry to the log.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    [Conditional("UFDEBUG")]
    public void Error(string aTag, string aMessage, params object[] anArguments)
    {
#if UFDEBUG
      this.AddLogColor(aTag, aMessage, ErrorColor, anArguments);
#endif
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="anError">Error to add</param>
    [Conditional("UFDEBUG")]
    public void Error(string aTag, Exception anError)
    {
#if UFDEBUG
      this.Error(
        aTag,
        "{0}\n{1}",
        anError.Message + (UFExceptionTools.GetInnerExceptionMessages(anError)),
        anError.StackTrace
      );
#endif
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// <param name="aTag">Tag to use</param>
    /// <param name="anError">Error to add</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    /// </summary>
    [Conditional("UFDEBUG")]
    public void Error(
      string aTag,
      Exception anError,
      string aMessage,
      params object[] anArguments
    )
    {
#if UFDEBUG
      this.Error(
        aTag,
        "{0}\n{1}\n{2}",
        anError.Message + (UFExceptionTools.GetInnerExceptionMessages(anError)),
        string.Format(aMessage, anArguments),
        anError.StackTrace
      );
#endif
    }

    /// <summary>
    /// Clears the log.
    /// </summary>
    [Conditional("UFDEBUG")]
    public void Clear()
    {
#if UFDEBUG
      lock (this.m_lock!)
      {
        this.m_log!.Length = 0;
      }
      this.OnChanged();
#endif
    }

    #endregion

    #region public properties

#if UFDEBUG

    /// <summary>
    /// Returns the current html formatted log.
    /// </summary>
    public string HtmlLog {
      get {
        string result;
        lock (this.m_lock!)
        {
          result = this.m_log!.ToString();
        }
        return result;
      }
    }

    /// <summary>
    /// Returns the current log without html formatting.
    /// </summary>
    public string PlainLog {
      get { return UFHtmlTools.ToPlain(this.HtmlLog); }
    }

#else
    /// <summary>
    /// Returns the current html formatted log.
    /// </summary>
    public string HtmlLog {
      get {
        return string.Empty;
      }
    }

    /// <summary>
    /// Returns the current log without html formatting.
    /// </summary>
    public string PlainLog {
      get {
        return string.Empty;
      }
    }

#endif

    /// <summary>
    /// Changed events are invoked whenever the log changes.
    /// </summary>
#pragma warning disable 67
    public event EventHandler? Changed;
#pragma warning restore 67

    #endregion

#if UFDEBUG

    #region private methods

    /// <summary>
    /// Invokes the changed event.
    /// </summary>
    private void OnChanged()
    {
      EventHandler? copy = this.Changed;
      if (copy != null)
      {
        copy.Invoke(this, null);
      }
    }

    /// <summary>
    /// Adds an entry to log, adding font color tag if aColor is set.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="aMessage">Message to add</param>
    /// <param name="aColor">Color to use or "" to skip font tag</param>
    /// <param name="anArguments">Arguments to format message</param>
    private void AddLogColor(
      string aTag,
      string aMessage,
      string aColor,
      params object[] anArguments
    )
    {
      // don't use format if there are no arguments.
      string message = string.IsNullOrEmpty(aMessage)
        ? ""
        : anArguments.Length == 0
          ? aMessage
          : string.Format(aMessage, anArguments);
      switch (aColor)
      {
        case ErrorColor:
          this.m_outputLine?.Invoke("[" + aTag + "] [ERROR] " + message);
          break;
        default:
          this.m_outputLine?.Invoke("[" + aTag + "] " + message);
          break;
      }
      lock (this.m_lock!)
      {
        if (!aColor.Equals(NoColor))
        {
          this.m_log!
            .Append("<font color=\"")
            .Append(aColor)
            .Append("\">");
        }
        this.m_log!
          .Append("<b>")
          .Append(DateTime.Now.ToString("[MM-dd HH:mm:ss]"))
          .Append("[")
          .Append(aTag)
          .Append("] ")
          .Append("</b><br/>")
          .Append(UFHtmlTools
            .ToHtml(message)
            .Replace("\r", "")
            .Replace("\n", "<br/>")
          );
        if (!aColor.Equals(NoColor))
        {
          this.m_log.Append("</font>");
        }
        this.m_log.Append("<br/><small> </small><br/>");
      }
      this.OnChanged();
    }

    #endregion

#endif
  }
}