using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Utility class for calculating note positions based on piano key indices
    /// Follows Single Responsibility Principle by only handling position calculations
    /// </summary>
    public static class NotePositionUtility
    {
        /// <summary>
        /// Converts piano key index to lane position for visual purposes
        /// Arranges notes like a musical staff: low notes lower, high notes higher
        /// </summary>
        /// <param name="pianoKeyIndex">Piano key index (0-7, where 0 = lowest note, 7 = highest note)</param>
        /// <param name="verticalSpacing">Vertical spacing between note lanes</param>
        /// <param name="baseYPosition">Base Y position for the lowest note</param>
        /// <param name="yOffset">Additional Y position offset</param>
        /// <returns>Calculated 2D position</returns>
        public static Vector2 GetLanePosition(int pianoKeyIndex, float verticalSpacing, float baseYPosition, float yOffset = 0.0f)
        {
            // Piano key 0 = lowest note (bottom of staff), Piano key 7 = highest note (top of staff)
            float xPos = (pianoKeyIndex - 3.5f) * 1.0f; // Center around 0, spread lanes horizontally
            
            // Y position: lower piano key indices = lower Y positions (like musical staff)
            float yPos = baseYPosition + (pianoKeyIndex * verticalSpacing) + yOffset;
            
            return new Vector2(xPos, yPos);
        }
        
        /// <summary>
        /// Gets the piano key name for a given index
        /// </summary>
        /// <param name="pianoKeyIndex">Piano key index (0-7)</param>
        /// <returns>Piano key name (C, D, E, F, G, A, B, C)</returns>
        public static string GetPianoKeyName(int pianoKeyIndex)
        {
            string[] keyNames = { "C", "D", "E", "F", "G", "A", "B", "C" };
            return pianoKeyIndex >= 0 && pianoKeyIndex < keyNames.Length ? keyNames[pianoKeyIndex] : "Unknown";
        }
        
        /// <summary>
        /// Gets the musical octave for a given piano key index
        /// </summary>
        /// <param name="pianoKeyIndex">Piano key index (0-7)</param>
        /// <returns>Octave number (lower indices = lower octaves)</returns>
        public static int GetOctave(int pianoKeyIndex)
        {
            // Piano key 0-3 = lower octave, 4-7 = higher octave
            return pianoKeyIndex < 4 ? 3 : 4;
        }
        
        /// <summary>
        /// Gets the staff line position for a given piano key index
        /// </summary>
        /// <param name="pianoKeyIndex">Piano key index (0-7)</param>
        /// <returns>Staff line number (0 = bottom line, 7 = top line)</returns>
        public static int GetStaffLine(int pianoKeyIndex)
        {
            // Maps piano key index to staff line position
            // 0 = bottom line, 7 = top line (like traditional musical staff)
            return pianoKeyIndex;
        }
    }
}
