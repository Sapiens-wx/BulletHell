namespace Games.Rhythm_Game
{
    public class RhythmGameNoteInfo
    {
        public readonly int Length;
        public readonly RhythmGameNoteType Type;

        public RhythmGameNoteInfo(int length, RhythmGameNoteType type)
        {
            Length = length;
            Type = type;
        }
    }

    public enum RhythmGameNoteType
    {
        Left,
        Right,
        
    }
}