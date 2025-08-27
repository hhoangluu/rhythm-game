using UnityEngine;
using Doulingo.Core;
using Doulingo.Input;

namespace Doulingo.Factory
{
    /// <summary>
    /// Factory for creating input readers based on type using prefabs
    /// </summary>
    public class InputReaderFactory : MonoBehaviour
    {
        [Header("Input Reader Prefabs")]
        [SerializeField] private InputReader inputReaderPrefab;
        
        /// <summary>
        /// Gets an input reader instance based on the specified type
        /// </summary>
        /// <param name="type">Type of input reader to create</param>
        /// <returns>Input reader instance</returns>
        public IInputReader GetInputReader(InputReaderType type)
        {
            return type switch
            {
                InputReaderType.InputReader => Instantiate(inputReaderPrefab),
                _ => Instantiate(inputReaderPrefab)
            };
        }
    }
}
