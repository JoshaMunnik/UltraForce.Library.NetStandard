// <copyright file="UFSerialQueueAction.cs" company="Ultra Force Development">
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

using System.Linq;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFSerialQueueAction" /> can be used to run one or more actions sequentially.
  /// </summary>
  /// <remarks>
  /// The class is a subclass of <see cref="UFParallelQueueAction"/> using a concurrent count of 1 (so only one action
  /// is ran at the time).
  /// </remarks>
  public class UFSerialQueueAction : UFParallelQueueAction
  {
    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFSerialQueueAction" />.
    /// </summary>
    /// <param name="anActions">Actions to run in sequence</param>
    public UFSerialQueueAction(params IUFQueueableAction[] anActions)
      : base(1, anActions)
    {
    }

    #endregion

    #region public properties

    /// <summary>
    /// The current action that is running or <c>null</c> if no action is running.
    /// </summary>
    public IUFQueueableAction? CurrentAction => this.GetRunningActions().FirstOrDefault();

    #endregion
  }
}