// <copyright file="IUFQueueableAction.cs" company="Ultra Force Development">
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

using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// The interface an action must implement so it can be placed in a queue.
  /// </summary>
  public interface IUFQueueableAction
  {
    /// <summary>
    /// Runs the action. 
    /// </summary>
    /// <remarks>
    /// If action is part of a queue and the method returns <c>false</c>, the queue will stop executing.
    /// </remarks>
    /// <param name="aToken">Cancellation token</param>
    /// <returns>
    /// <c>True</c> if everything ran ok or <c>false</c> otherwise
    /// </returns>
    Task<bool> RunAsync(CancellationToken aToken);
  }
}