using UnityEngine;

namespace Games.Rhythm_Game
{
    public class RhythmGameManager : MonoBehaviour
    {
        public RhythmGameManager Instance { get; private set; }
        public RhythmGameTrack Track { get; private set; }
        
        public void LoadTrack(string trackName)
        {
            var trackPrefab = Resources.Load<RhythmGameTrack>($"RhythmGame/Tracks/{trackName}");
        }
    }
}