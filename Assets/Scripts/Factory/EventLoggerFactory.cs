using UnityEngine;
using Doulingo.Core;
using Doulingo.Logging;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating event loggers based on type using prefabs
    /// </summary>
    public class EventLoggerFactory : MonoBehaviour
    {
        [Header("Event Logger Prefabs")]
        [SerializeField] private EventLogger eventLoggerPrefab;
        
        /// <summary>
        /// Gets an event logger instance based on the specified type
        /// </summary>
        /// <param name="type">Type of event logger to create</param>
        /// <returns>Event logger instance</returns>
        public IEventLogger GetEventLogger(EventLoggerType type)
        {
            return type switch
            {
                EventLoggerType.EventLogger => Instantiate(eventLoggerPrefab),
                _ => Instantiate(eventLoggerPrefab)
            };
        }
    }
}
