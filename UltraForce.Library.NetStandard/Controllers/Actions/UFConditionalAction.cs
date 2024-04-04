// <copyright file="UFConditionalAction.cs" company="Ultra Force Development">
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace UltraForce.Library.NetStandard.Controllers.Actions
{
  /// <summary>
  /// <see cref="UFConditionalAction" /> can be used to run some action only when a certain condition is met.
  /// </summary>
  /// <remarks>
  /// The condition can either be a function or another action. This class can be used for example to perform a
  /// certain action if another action fails.
  /// </remarks>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public class UFConditionalAction : UFQueueableAction
  {
    #region private variables

    /// <summary>
    /// Test function or null to use the action
    /// </summary>
    private readonly Func<bool>? m_testFunction;

    /// <summary>
    /// Test action
    /// </summary>
    private readonly IUFQueueableAction? m_testAction;

    /// <summary>
    /// Result the test function or action must match
    /// </summary>
    private readonly bool m_testResult;

    /// <summary>
    /// Action that is run if values match
    /// </summary>
    private readonly IUFQueueableAction? m_successAction;

    /// <summary>
    /// Action that is run if values do not match
    /// </summary>
    private readonly IUFQueueableAction? m_failureAction;

    /// <summary>
    /// Result to return if values do not match
    /// </summary>
    private readonly bool m_actionResult;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// <para>
    /// The action will return the result from aFunction
    /// </para>
    /// </summary>
    /// <param name="aFunction">
    /// Function that should return a boolean.
    /// </param>
    public UFConditionalAction(Func<bool> aFunction)
    {
      this.m_testFunction = aFunction;
      this.m_testResult = true;
      this.m_successAction = null;
      this.m_failureAction = null;
      this.m_actionResult = true;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// <para>
    /// The action will return the result from either the aSuccessAction or aFailureAction.
    /// </para>
    /// </summary>
    /// <param name="aFunction">
    /// Function that should return a boolean.
    /// </param>
    /// <param name="aSuccessAction">
    /// Action to run if aFunction returns true
    /// </param>
    /// <param name="aFailureAction">
    /// Action to run if aFunction returns false
    /// </param>
    public UFConditionalAction(
      Func<bool> aFunction,
      IUFQueueableAction aSuccessAction,
      IUFQueueableAction aFailureAction
    )
    {
      this.m_testFunction = aFunction;
      this.m_testResult = true;
      this.m_successAction = aSuccessAction;
      this.m_failureAction = aFailureAction;
      this.m_actionResult = true;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// </summary>
    /// <param name="aFunction">
    /// Function that should return a boolean.
    /// </param>
    /// <param name="aTestResult">
    /// Value to compare result of aFunction with
    /// </param>
    /// <param name="aSuccessAction">
    /// Action to run if aTestResult matches the result from aFunction
    /// </param>
    /// <param name="anActionResult">
    /// Value to return if aConditionalAction is not run.
    /// </param>
    public UFConditionalAction(
      Func<bool> aFunction,
      bool aTestResult,
      IUFQueueableAction aSuccessAction,
      bool anActionResult = true
    )
    {
      this.m_testFunction = aFunction;
      this.m_testResult = aTestResult;
      this.m_successAction = aSuccessAction;
      this.m_actionResult = anActionResult;
      this.m_failureAction = null;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// </summary>
    /// <param name="aFunction">
    /// Function that should return a boolean.
    /// </param>
    /// <param name="aSuccessAction">
    /// Action to run if aFunction returns <c>true</c>.
    /// </param>
    /// <param name="anActionResult">
    /// Value to return if aFunction returns <c>false</c>
    /// </param>
    public UFConditionalAction(
      Func<bool> aFunction,
      IUFQueueableAction aSuccessAction,
      bool anActionResult = true
    ) : this(aFunction, true, aSuccessAction, anActionResult)
    {
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// <para>
    /// The action will return the result from either the aSuccessAction or
    /// aFailureAction.
    /// </para>
    /// </summary>
    /// <param name="anAction">
    /// Action to run and use its result for the test.
    /// </param>
    /// <param name="aSuccessAction">
    /// Action to run if anAction run successfully and returned true.
    /// </param>
    /// <param name="aFailureAction">
    /// Action to run if anAction failed and returned false.
    /// </param>
    public UFConditionalAction(
      IUFQueueableAction anAction,
      IUFQueueableAction aSuccessAction,
      IUFQueueableAction aFailureAction
    )
    {
      this.m_testAction = anAction;
      this.m_testResult = true;
      this.m_successAction = aSuccessAction;
      this.m_successAction = aFailureAction;
      this.m_actionResult = true;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// </summary>
    /// <param name="anAction">
    /// Action to run and use its result for the test.
    /// </param>
    /// <param name="aTestResult">
    /// Value to compare result of anAction with
    /// </param>
    /// <param name="aSuccessAction">
    /// Action to run if aTestResult matches the result from anAction
    /// </param>
    /// <param name="anActionResult">
    /// Value to return if aConditionalAction is not run.
    /// </param>
    public UFConditionalAction(
      IUFQueueableAction anAction,
      bool aTestResult,
      IUFQueueableAction aSuccessAction,
      bool anActionResult = true
    )
    {
      this.m_testAction = anAction;
      this.m_testResult = aTestResult;
      this.m_successAction = aSuccessAction;
      this.m_actionResult = anActionResult;
    }

    /// <summary>
    /// Constructs an instance of <see cref="UFConditionalAction"/>.
    /// </summary>
    /// <param name="anAction">
    /// Action to run and use its result for the test.
    /// </param>
    /// <param name="aSuccessAction">
    /// Action to run if aTestResult matches anAction run successfully and
    /// returned true.
    /// </param>
    /// <param name="anActionResult">
    /// Value to return if aConditionalAction is not run.
    /// </param>
    public UFConditionalAction(
      IUFQueueableAction anAction,
      IUFQueueableAction aSuccessAction,
      bool anActionResult = true
    ) : this(anAction, true, aSuccessAction, anActionResult)
    {
    }

    #endregion

    #region public methods

    /// <summary>
    /// Runs the action. First the test result is determines by either executing the function or running the action.
    /// <para>
    /// If no success action has been set, the method just returns the result.
    /// </para>
    /// <para>
    /// If the result matches the required value, the success action is run and the its result is returned. 
    /// </para>
    /// <para>
    /// Else the method checks if a failure action has been set. If there is a failure action it is run and its result
    /// is returned. If there is no failure action the default action result is returned.
    /// </para>
    /// </summary>
    /// <param name="aToken">Cancellation token</param>
    /// <returns>
    /// The result of the test function/action or the result of the success or failure action or the value as passed
    /// in the constructor. If aToken indicates a cancellation the method returns <c>false</c>.
    /// </returns>
    public override async Task<bool> RunAsync(CancellationToken aToken)
    {
      bool result;
      if (this.m_testFunction != null)
      {
        result = this.m_testFunction();
      }
      else
      {
        result = await this.m_testAction!.RunAsync(aToken);
      }
      if (aToken.IsCancellationRequested)
      {
        return this.m_actionResult && !aToken.IsCancellationRequested;
      }
      if (this.m_successAction == null)
      {
        return result;
      }
      if (result == this.m_testResult)
      {
        return await this.m_successAction.RunAsync(aToken);
      }
      if (this.m_failureAction != null)
      {
        return await this.m_failureAction.RunAsync(aToken);
      }
      return this.m_actionResult && !aToken.IsCancellationRequested;
    }

    #endregion
  }
}