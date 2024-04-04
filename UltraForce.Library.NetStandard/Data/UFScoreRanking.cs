// <copyright file="UFScoreRanking.cs" company="Ultra Force Development">
// Ultra Force Library - Copyright (C) 2024 Ultra Force Development
// </copyright>
// <author>Josha Munnik</author>
// <email>josha@ultraforce.com</email>
// <license>
// The MIT License (MIT)
//
// Copyright (C) 2024 Ultra Force Development
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
using System.Collections.Generic;
using System.Linq;

namespace UltraForce.Library.NetStandard.Data
{
  /// <summary>
  /// A class to help with the ranking of scores. It handles the case where there are multiple
  /// entries with the same score.
  /// <para>
  /// The scores are ranked from highest to lowest. The ranking starts at 1.
  /// </para>
  /// <example>
  /// <code>
  ///   // using c# v12 
  ///   UFScoreRanking&lt;int&gt; ranking = new([300, 200, 100, 200]);
  ///   // gives 1
  ///   int rank = ranking.GetRanking(100);
  ///   // gives 2
  ///   int rank = ranking.GetRanking(200);
  ///   // gives 4
  ///   int rank = ranking.GetRanking(300);
  /// </code>
  /// </example>
  /// </summary>
  /// <typeparam name="TScore">A type that supports <see cref="IComparable"/></typeparam>
  public class UFScoreRanking<TScore> where TScore : IComparable<TScore>
  {
    #region private class
    
    /// <summary>
    /// Helper class to store the score, the rank and the count of the score.
    /// </summary>
    private class ScoreRank
    {
      public TScore Score { get;  }
      public int Rank { get; set; }
      public int Count { get; private set; }

      public ScoreRank(TScore aScore)
      {
        this.Count = 1;
        this.Score = aScore;
      }

      public void IncreaseCount()
      {
        this.Count += 1;
      }
    }
    
    #endregion
    
    #region private variables
    
    /// <summary>
    /// List of scores, will be kept sorted from highest to lowest.
    /// </summary>
    private readonly List<ScoreRank> m_scoreRanks = new List<ScoreRank>();

    /// <summary>
    /// When true calculate the ranks.
    /// </summary>
    private bool m_dirty = true;
    
    #endregion
    
    #region constructors
    
    /// <summary>
    /// Constructs an empty instance.
    /// </summary>
    public UFScoreRanking()
    {
    }

    /// <summary>
    /// Creates an empty instance and then calls <see cref="AddScores"/>.
    /// </summary>
    /// <param name="aScores">Scores to add</param>
    public UFScoreRanking(IEnumerable<TScore> aScores)
    {
      this.AddScores(aScores);
    }
    
    #endregion
    
    #region public methods
    
    /// <summary>
    /// Adds a new score, this will invalidate the ranking cache.
    /// </summary>
    /// <param name="aScore"></param>
    public void AddScore(TScore aScore)
    {
      this.m_dirty = true;
      for (int index = 0; index < this.m_scoreRanks.Count; index++)
      {
        if (aScore.CompareTo(this.m_scoreRanks[index].Score) > 0)
        {
          this.m_scoreRanks.Insert(index, new ScoreRank(aScore));
          return;
        }
        if (aScore.CompareTo(this.m_scoreRanks[index].Score) == 0)
        {
          this.m_scoreRanks[index].IncreaseCount();
          return;
        }
      }
      this.m_scoreRanks.Add(new ScoreRank(aScore));
    }

    /// <summary>
    /// Adds multiple score entries.
    /// </summary>
    /// <param name="aScores">Scores to add</param>
    public void AddScores(IEnumerable<TScore> aScores)
    {
      foreach (TScore score in aScores)
      {
        this.AddScore(score);
      }
    }
    
    /// <summary>
    /// Gets a ranking for a certain score.
    /// </summary>
    /// <param name="aScore">Score to get the ranking for</param>
    /// <returns>Ranking or -1 if the score was never added</returns>
    public int GetRanking(TScore aScore)
    {
      if (this.m_dirty)
      {
        this.UpdateRanks();
      }
      int? rank = this.m_scoreRanks
        .FirstOrDefault(scoreRank => aScore.CompareTo(scoreRank.Score) == 0)
        ?.Rank;
      return rank ?? -1;
    }
    
    #endregion
    
    #region private methods
    
    /// <summary>
    /// Updates the <see cref="ScoreRank.Rank"/> value of every entry.
    /// </summary>
    private void UpdateRanks()
    {
      int rank = 1;
      foreach (ScoreRank scoreRank in this.m_scoreRanks)
      {
        scoreRank.Rank = rank;
        rank += scoreRank.Count;
      }
      this.m_dirty = false;
    }
    
    #endregion
  }
}