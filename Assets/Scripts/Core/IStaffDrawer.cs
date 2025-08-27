using UnityEngine;

namespace Doulingo.Core
{
    /// <summary>
    /// Interface for drawing musical staff elements (beat lines, staff lines, etc.)
    /// </summary>
    public interface IStaffDrawer
    {
        /// <summary>
        /// Initialize the staff drawer with required parameters
        /// </summary>
        void Initialize(float unitsPerBeat, float hitLineX0, float lineHeight, float lineThickness);
        
        /// <summary>
        /// Draw the staff elements
        /// </summary>
        void DrawStaff();
        
        /// <summary>
        /// Clear all drawn elements
        /// </summary>
        void ClearStaff();
        
        /// <summary>
        /// Update staff elements (called each frame)
        /// </summary>
        void UpdateStaff();
    }
}
