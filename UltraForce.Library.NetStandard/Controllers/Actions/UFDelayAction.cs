// <copyright file="UFDelayAction.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2018 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
// Copyright (C) 2018 Ultra Force Development
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to 
// deal in the Software without restriction, including without limitation the 
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
// IN THE SOFTWARE.
// </license>

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFDelayAction" /> is a subclass of <see cref="UFQueueableAction" />. It will wait a certain time.
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class UFDelayAction : UFQueueableAction
  {
    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFDelayAction" />.
    /// </summary>
    /// <param name="aDelay">
    /// Delay in milliseconds to wait
    /// </param>
    public UFDelayAction(int aDelay)
    {
      this.Delay = aDelay;
    }

    #endregion

    #region ufqueueableaction methods

    /// <inheritdoc />
    public override async Task<bool> RunAsync(CancellationToken aToken)
    {
      await Task.Delay(this.Delay, aToken);
      return true;
    }

    #endregion

    #region public properties

    /// <summary>
    /// The delay in milliseconds
    /// </summary>
    public int Delay { get; }

    #endregion
  }
}