namespace Doulingo.Core
{
    public interface IJudgeService
    {
        void SetHitWindow(float window);
        HitResult JudgeNote(NoteData note, float hitTime);
        bool IsNoteInHitWindow(NoteData note, float currentBeat);
        float GetHitAccuracy(NoteData note, float hitTime);
        void SetJudgmentWindows(float perfect, float great, float good);
        float GetPerfectWindow();
        float GetGreatWindow();
        float GetGoodWindow();
        float GetHitWindow();
    }

    public enum HitResult
    {
        Perfect,
        Great,
        Good,
        Miss
    }
}
