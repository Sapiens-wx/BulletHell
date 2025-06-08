using System;
using UnityEngine;

namespace Games.Rhythm_Game
{
    public class RhythmGameManager : MonoBehaviour
    {
        public static RhythmGameManager Instance { get; private set; }
        public RhythmGameTrack CurrentTrack { get; private set; }
        
        [SerializeField] private float correctNoteThreshold = 0.5f;
        public float CorrectNoteThreshold => correctNoteThreshold;


        private void Start()
        {
            CurrentTrack = FindObjectOfType<RhythmGameTrack>();
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                CurrentTrack.AccumulateControlInput(NoteType.Left, Time.deltaTime);
            if (Input.GetKey(KeyCode.RightArrow))
                CurrentTrack.AccumulateControlInput(NoteType.Right, Time.deltaTime);

        }

        public void LoadTrack(string trackName)
        {
            var trackPrefab = Resources.Load<RhythmGameTrack>($"RhythmGame/Tracks/{trackName}");
        }
    }
}