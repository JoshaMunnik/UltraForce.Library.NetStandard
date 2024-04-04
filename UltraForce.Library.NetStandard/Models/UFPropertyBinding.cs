// <copyright file="UFPropertyBinding.cs" company="Ultra Force Development">
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

using UltraForce.Library.NetStandard.Events;

namespace UltraForce.Library.NetStandard.Models
{
  /// <summary>
  /// This class keeps two properties synchronized, whenever one of the
  /// properties changes value <see cref="UFPropertyBinding"/> 
  /// will update the other property.
  /// </summary>
  public class UFPropertyBinding
  {
    #region private variables

    /// <summary>
    /// See <see cref="FirstData"/>
    /// </summary>
    private IUFModel? m_firstData;

    /// <summary>
    /// See <see cref="SecondData"/>
    /// </summary>
    private IUFModel? m_secondData;

    /// <summary>
    /// When <c>true</c> ignore events
    /// </summary>
    private bool m_ignoreEvents;

    #endregion

    #region constructors

    /// <summary>
    /// Constructs an instance of <see cref="UFPropertyBinding"/>.
    /// </summary>
    /// <param name="aFirstData">First data</param>
    /// <param name="aFirstPropertyName">Name of property</param>
    /// <param name="aSecondData">Second data</param>
    /// <param name="aSecondPropertyName">Name of property</param>
    public UFPropertyBinding(
      IUFModel aFirstData,
      string aFirstPropertyName,
      IUFModel aSecondData,
      string aSecondPropertyName
    )
    {
      this.FirstPropertyName = aFirstPropertyName;
      this.SecondPropertyName = aSecondPropertyName;
      this.FirstData = aFirstData;
      this.SecondData = aSecondData;
    }

    #endregion

    #region public methods

    /// <summary>
    /// Unbinds the binding by clearing reference to both <see cref="UFModel"/>
    /// instances.
    /// </summary>
    public void Unbind()
    {
      this.FirstData = null;
      this.SecondData = null;
    }

    #endregion

    #region public properties

    /// <summary>
    /// First object that implements <see cref="IUFModel"/>
    /// </summary>
    public IUFModel? FirstData {
      get => this.m_firstData;
      set {
        if (this.m_firstData != null)
        {
          this.m_firstData.DataChanged -= this.HandleFirstDataChanged;
        }
        this.m_firstData = value;
        if (this.m_firstData != null)
        {
          this.m_firstData.DataChanged += this.HandleFirstDataChanged;
        }
      }
    }

    /// <summary>
    /// Name of property in <see cref="FirstData"/>
    /// </summary>
    public string FirstPropertyName { get; set; }

    /// <summary>
    /// Second object that implements <see cref="IUFModel"/>
    /// </summary>
    public IUFModel? SecondData {
      get => this.m_secondData;
      set {
        if (this.m_secondData != null)
        {
          this.m_secondData.DataChanged -= this.HandleSecondDataChanged;
        }
        this.m_secondData = value;
        if (this.m_secondData != null)
        {
          this.m_secondData.DataChanged += this.HandleSecondDataChanged;
        }
      }
    }

    /// <summary>
    /// Name of property in <see cref="SecondData"/>
    /// </summary>
    public string SecondPropertyName { get; set; }

    #endregion

    #region event handlers

    /// <summary>
    /// Handles changes to data of the first data.
    /// </summary>
    /// <param name="aSender">Sender of event</param>
    /// <param name="anEvent">Event object</param>
    private void HandleFirstDataChanged(
      object aSender,
      UFDataChangedEventArgs anEvent
    )
    {
      if (this.m_ignoreEvents)
      {
        return;
      }
      if (anEvent.HasChanged(this.FirstPropertyName) && (this.m_firstData != null))
      {
        this.m_ignoreEvents = true;
        this.SecondData?.SetPropertyValue(
          this.SecondPropertyName,
          this.m_firstData.GetPropertyValue(this.FirstPropertyName)
        );
        this.m_ignoreEvents = false;
      }
    }

    /// <summary>
    /// Handles changes to data of the second data.
    /// </summary>
    /// <param name="aSender">Sender of event</param>
    /// <param name="anEvent">Event object</param>
    private void HandleSecondDataChanged(
      object aSender,
      UFDataChangedEventArgs anEvent
    )
    {
      if (this.m_ignoreEvents)
      {
        return;
      }
      if (anEvent.HasChanged(this.SecondPropertyName))
      {
        this.m_ignoreEvents = true;
        this.FirstData?.SetPropertyValue(
          this.FirstPropertyName,
          this.SecondData?.GetPropertyValue(this.SecondPropertyName)
        );
        this.m_ignoreEvents = false;
      }
    }

    #endregion
  }
}