using UnityEngine;
using Doulingo.Core;
using Doulingo.Gameplay;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating scoring services based on type
    /// </summary>
    public class ScoringServiceFactory
    {
        /// <summary>
        /// Gets a scoring service instance based on the specified type
        /// </summary>
        /// <param name="type">Type of scoring service to create</param>
        /// <returns>Scoring service instance</returns>
        public IScoringService GetScoringService(ScoringServiceType type)
        {
            return type switch
            {
                ScoringServiceType.ScoringService => new ScoringService(),
                _ => new ScoringService()
            };
        }
    }
}
