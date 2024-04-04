// <copyright file="UFTextFileLog.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2021 Ultra Force Development
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
using System.IO;
using System.Threading.Tasks;
using UltraForce.Library.NetStandard.Tools;

namespace UltraForce.Library.NetStandard.Development
{
  /// <summary>
  /// <see cref="UFTextFileLog" /> implements a log that is written to a text file. New messages get added to the end
  /// of the log file. The messages are prefixed with a time stamp.
  /// <para>
  /// The class will create a a new log file when the day changes. The log filename includes a date.
  /// </para>
  /// <para>
  /// If <c>UFDEBUG</c> is defined, the log messages are also outputted to the general debugger log via
  /// <see cref="Debug.WriteLine(string)" />.
  /// </para>
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UFTextFileLog
  {
    #region private variables

    /// <summary>
    /// Path to create log files in
    /// </summary>
    private readonly string m_path;

    /// <summary>
    /// Filename part of the log file (without extension)
    /// </summary>
    private readonly string m_fileName;

    /// <summary>
    /// File extension (without the '.')
    /// </summary>
    private readonly string m_fileExtension;

    #endregion

    #region constructors & destructors

    /// <summary>
    /// Constructs an instance of <see cref="UFTextFileLog"/>
    /// </summary>
    /// <param name="aPath">Path to create log files in</param>
    /// <param name="aFileName">Filename for log file (without extension)</param>
    /// <param name="aFileExtension">File extension to use (without '.')</param>
    public UFTextFileLog(string aPath, string aFileName, string aFileExtension = "log")
    {
      this.m_path = aPath;
      this.m_fileName = aFileName;
      this.m_fileExtension = aFileExtension;
    }

    #endregion

    #region public methods

    /// <summary>
    /// Adds an entry to log.
    /// </summary>
    /// <param name="anObject">Object which type name will be used as tag</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    public async Task AddAsync(object anObject, string aMessage, params object[] anArguments)
    {
      await this.AddAsync(anObject.GetType().Name, aMessage, anArguments);
    }

    /// <summary>
    /// Adds an error entry to the log.
    /// </summary>
    /// <param name="anObject">Object which type name will be used as tag</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    public async Task ErrorAsync(object anObject, string aMessage, params object[] anArguments)
    {
      await this.ErrorAsync(anObject.GetType().Name, aMessage, anArguments);
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// </summary>
    /// <param name="anObject">Tag to use</param>
    /// <param name="anError">Error to add</param>
    public async Task ErrorAsync(object anObject, Exception anError)
    {
      await this.ErrorAsync(anObject.GetType().Name, anError);
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// <param name="anObject">Object which type name will be used as tag</param>
    /// <param name="anError">Error to add</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    /// </summary>
    public async Task ErrorAsync(object anObject, Exception anError, string aMessage, params object[] anArguments)
    {
      await this.ErrorAsync(anObject.GetType().Name, anError, aMessage, anArguments);
    }

    /// <summary>
    /// Adds an entry to log.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    public async Task AddAsync(string aTag, string aMessage, params object[] anArguments)
    {
      await this.AddLogLineAsync(aTag, aMessage, anArguments);
    }

    /// <summary>
    /// Adds an error entry to the log.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments"></param>
    public async Task ErrorAsync(string aTag, string aMessage, params object[] anArguments)
    {
      await this.AddLogLineAsync(aTag + "][ERROR", aMessage, anArguments);
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="anError">Error to add</param>
    public async Task ErrorAsync(string aTag, Exception anError)
    {
      await this.ErrorAsync(
        aTag,
        "{0}\n{1}",
        anError.Message + (UFExceptionTools.GetInnerExceptionMessages(anError)),
        anError.StackTrace
      );
    }

    /// <summary>
    /// Adds an error entry for an exception.
    /// <param name="aTag">Tag to use</param>
    /// <param name="anError">Error to add</param>
    /// <param name="aMessage">Log entry to add</param>
    /// <param name="anArguments">Formatting arguments</param>
    /// </summary>
    public async Task ErrorAsync(string aTag, Exception anError, string aMessage, params object[] anArguments)
    {
      await this.ErrorAsync(
        aTag,
        "{0}\n{1}\n{2}",
        anError.Message + (UFExceptionTools.GetInnerExceptionMessages(anError)),
        string.Format(aMessage, anArguments),
        anError.StackTrace
      );
    }

    #endregion

    #region private methods

    /// <summary>
    /// Adds an entry to log, adding font color tag if aColor is set.
    /// </summary>
    /// <param name="aTag">Tag to use</param>
    /// <param name="aMessage">Message to add</param>
    /// <param name="anArguments">Arguments to format message</param>
    private async Task AddLogLineAsync(
      string aTag,
      string aMessage,
      params object[] anArguments
    )
    {
      string message = string.IsNullOrEmpty(aMessage)
        ? ""
        : anArguments.Length == 0
          ? aMessage
          : string.Format(aMessage, anArguments);
      message = DateTime.Now.ToString("[HH:mm:ss]") + "[" + aTag + "] " + message;
      using (StreamWriter writer = new StreamWriter(this.BuildFullFileName(DateTime.Now), true))
      {
        await writer.WriteLineAsync(message);
      }
#if UFDEBUG
      Debug.WriteLine(message);
#endif
    }

    /// <summary>
    /// Builds a full path and file name for a log file for a specific date (time part is ignored).
    /// </summary>
    /// <param name="aDate">Date to build file name for</param>
    /// <returns>Filename including full path</returns>
    private string BuildFullFileName(DateTime aDate)
    {
      return Path.Combine(
        this.m_path,
        $"{this.m_fileName}_{aDate.Year}-{aDate.Month,0:D2}-{aDate.Day,0:D2}.{this.m_fileExtension}"
      );
    }

    #endregion
  }
}