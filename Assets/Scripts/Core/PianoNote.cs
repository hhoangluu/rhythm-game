namespace Doulingo.Core
{
    /// <summary>
    /// Piano note names for converting key indices
    /// </summary>
    public enum PianoNote
    {
        C = 0,   // C (Do)
        D = 1,   // D (Re)
        E = 2,   // E (Mi)
        F = 3,   // F (Fa)
        G = 4,   // G (Sol)
        A = 5,   // A (La)
        B = 6,   // B (Si)
        C2 = 7   // C2 (Do - octave higher)
    }

    /// <summary>
    /// Helper methods for piano notes
    /// </summary>
    public static class PianoNoteHelper
    {
        /// <summary>
        /// Convert piano key index to note name
        /// </summary>
        public static string GetNoteName(int keyIndex)
        {
            if (keyIndex >= 0 && keyIndex <= 7)
            {
                return ((PianoNote)keyIndex).ToString();
            }
            return "?";
        }
    }
}
