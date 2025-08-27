using UnityEngine;
using Doulingo.Core;

namespace Doulingo.Core
{
    /// <summary>
    /// Utility class for calculating note positions on the musical staff
    /// </summary>
    public static class StaffMapCalculator
    {
        /// <summary>
        /// Get Y position for a piano note
        /// </summary>
        public static float GetYPosition(PianoNote note)
        {
            int keyIndex = (int)note;
            
            // Calculate Y position based on staff settings
            float baseY = GlobalSetting.BaseYPosition;
            float spacing = GlobalSetting.VerticalSpacing;
            
            // Piano key 0 (C) is at baseY, each key above adds spacing
            return baseY + (keyIndex * spacing);
        }
    }
}
