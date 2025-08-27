using UnityEngine;
using Doulingo.Core;
using Doulingo.Gameplay;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating judge services based on type
    /// </summary>
    public class JudgeServiceFactory
    {
        /// <summary>
        /// Gets a judge service instance based on the specified type
        /// </summary>
        /// <param name="type">Type of judge service to create</param>
        /// <returns>Judge service instance</returns>
        public IJudgeService GetJudgeService(JudgeServiceType type)
        {
            return type switch
            {
                JudgeServiceType.JudgeService => new JudgeService(),
                _ => new JudgeService()
            };
        }
    }
}
