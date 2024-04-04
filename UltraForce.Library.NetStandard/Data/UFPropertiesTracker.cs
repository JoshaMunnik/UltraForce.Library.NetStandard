// <copyright file="UFPropertiesTracker.cs" company="Ultra Force Development">
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
using UltraForce.Library.NetStandard.Events;
using UltraForce.Library.NetStandard.Interfaces;

namespace UltraForce.Library.NetStandard.Data
{
  /// <summary>
  /// <see cref="UFPropertiesTracker" /> can be used to track various 
  /// properties in an object that implements the 
  /// <see cref="IUFNotifyDataChanged" /> and call a delegate whenever one of
  /// the property changes.
  /// </summary>
  public class UFPropertiesTracker
  {
    #region private vars

    /// <summary>
    /// The data to track.
    /// </summary>
    private IUFNotifyDataChanged? m_data;

    /// <summary>
    /// Properties to track for.
    /// </summary>
    private readonly string[] m_properties;

    /// <summary>
    /// The delegate to call when a property changes.
    /// </summary>
    private readonly Action m_callback;

    #endregion

    #region public methods

    /// <summary>
    /// Initializes a new instance of the <see cref="UFPropertiesTracker"/> 
    /// class.
    /// </summary>
    /// <param name='aData'>
    /// Object that implements <see cref="IUFNotifyDataChanged" />.
    /// </param>
    /// <param name='aCallback'>
    /// A delegate that gets called whenever one of the properties changes.
    /// </param>
    /// <param name='aProperties'>
    /// One or more property names to track; empty or <c>null</c> will track 
    /// all properties.
    /// </param>
    public UFPropertiesTracker(
      IUFNotifyDataChanged? aData,
      Action aCallback,
      params string[] aProperties
    )
    {
      this.m_data = null;
      this.m_properties = aProperties;
      this.m_callback = aCallback;
      this.Data = aData;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UFPropertiesTracker"/> 
    /// class. Assign value to <see cref="Data" /> property to start tracking
    /// the data.
    /// </summary>
    /// <param name='aCallback'>
    /// A delegate that gets called whenever one of the properties changes.
    /// </param>
    /// <param name='aProperties'>
    /// One or more property names to track; empty or <c>null</c> will track 
    /// all properties.
    /// </param>
    public UFPropertiesTracker(Action aCallback, params string[] aProperties)
      : this(null, aCallback, aProperties)
    {
    }

    #endregion

    #region public properties

    /// <summary>
    /// The object that implements <see cref="IUFNotifyDataChanged" />. When
    /// a new value gets assigned, it will clean up and old event handlers 
    /// before using the new value.
    /// </summary>
    public IUFNotifyDataChanged? Data {
      get => this.m_data;
      set
      {
        if (value == this.m_data)
        {
          return;
        }
        if (this.m_data != null)
        {
          this.m_data.DataChanged -= this.HandleDataChanged;
        }
        this.m_data = value;
        if (this.m_data == null)
        {
          return;
        }
        this.m_data.DataChanged += this.HandleDataChanged;
        this.m_callback();
      }
    }

    #endregion

    #region event handlers

    /// <summary>
    /// Data has changed.
    /// </summary>
    /// <param name="aSender">
    /// Sender of the event
    /// </param>
    /// <param name='anEvent'>
    /// The event object.
    /// </param>
    private void HandleDataChanged(
      object aSender,
      UFDataChangedEventArgs anEvent
    )
    {
      // no properties set?
      if (this.m_properties.Length == 0)
      {
        // yes, always call callback
        this.m_callback();
        return;
      }
      // check if one of the properties has changed
      if (anEvent.HasChanged(this.m_properties))
      {
        this.m_callback();
      }
    }

    #endregion
  }
}