using System;
using ResearchUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Games.Rhythm_Game
{
    public class RhythmGameManager : MonoBehaviour
    {
        public static RhythmGameManager Instance { get; private set; }
        public RhythmGameTrack CurrentTrack { get; private set; }
        
        [SerializeField] private float correctNoteThreshold = 0.5f;
        public float CorrectNoteThreshold => correctNoteThreshold;
        public float CurrentMoveStartTime;


        private void Start()
        {
            CurrentTrack = FindObjectOfType<RhythmGameTrack>();
            Instance = this;

            EventCollector.Instance.SetEventLogHeader("RhythmGame","Input Result", "Note Type", "Left Accumulated Time",
                "Right Accumulated Time", "Rest Time");
        }

        private void Update()
        {
            //if (Input.GetMouseButton((int)MouseButton.LeftMouse))
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                CurrentTrack.AccumulateControlInput(NoteType.Left, Time.deltaTime);
                
                print("left");
            }

            //if (Input.GetMouseButton((int)MouseButton.RightMouse))
            if (Input.GetKey(KeyCode.RightArrow))
            {
                CurrentTrack.AccumulateControlInput(NoteType.Right, Time.deltaTime);
                print("right");
            }

        }

        public void LoadTrack(string trackName)
        {
            var trackPrefab = Resources.Load<RhythmGameTrack>($"RhythmGame/Tracks/{trackName}");
        }

        public void OnNoteFulfilled(NoteType noteType)
        {
            var leftTime = CurrentTrack.leftDetector.timeAccumulation;
            var rightTime = CurrentTrack.rightDetector.timeAccumulation;
            var restTime = Time.time - CurrentMoveStartTime - leftTime - rightTime;

            EventCollector.Instance.RecordEvent(
                "RhythmGame",
                "True",
                noteType.ToString(),
                leftTime.ToString(),
                rightTime.ToString(),
                restTime.ToString()
            );
        }
        
        public void OnNoteNotFulfilled(NoteType noteType)
        {
            var leftTime = CurrentTrack.leftDetector.timeAccumulation;
            var rightTime = CurrentTrack.rightDetector.timeAccumulation;
            var restTime = CurrentTrack.SecondPerMove - leftTime - rightTime;

            EventCollector.Instance.RecordEvent(
                "RhythmGame",
                "False",
                noteType.ToString(),
                leftTime.ToString(),
                rightTime.ToString(),
                restTime.ToString()
            );
        }

        public void TrackEnded()
        {
            EventCollector.Instance.CollectEventLogsToFile("RhythmGame");
            EditorApplication.isPlaying = false;
        }
    }
}