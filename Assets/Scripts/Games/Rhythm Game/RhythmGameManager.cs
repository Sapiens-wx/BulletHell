using System;
using ResearchUtilities;
using TMPro;
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

        private int _score;
        private int _comboCounter;
        private int _maxComboCounter;
        [SerializeField] private TextMeshProUGUI ComboCounterText;
        [SerializeField] private TextMeshProUGUI ComboText;
        [SerializeField] private TextMeshProUGUI ScoreText;
        
        [SerializeField] private float noteFadeDuration = 0.5f;
        public float NoteFadeDuration => noteFadeDuration;
        [SerializeField] private float noteFadeZoomRatio = 1.5f;
        public float NoteFadeZoomRatio => noteFadeZoomRatio;
        
        [Header("Track Clear UI")]
        [SerializeField] private RhythmGameClearUIHandler clearUIHandler;
        [SerializeField] private float clearUIMoveInDuration = 1f;
        public float ClearUIMoveInDuration => clearUIMoveInDuration;
        
        


        private void Start()
        {
            CurrentTrack = FindObjectOfType<RhythmGameTrack>();
            Instance = this;

            EventCollector.Instance.SetEventLogHeader("RhythmGame","Input Result", "Note Type", "Left Accumulated Time",
                "Right Accumulated Time", "Rest Time");
            
            ComboCounterText.text = "";
            ComboText.text = "";
            ScoreText.text = "0";
            
            _maxComboCounter = 0;
            
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

        /// <summary>
        /// Currently just add 100 points every time 
        /// </summary>
        /// <param name="noteType"></param>
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

            _score += 100;
            _comboCounter++;
            ComboCounterText.text = _comboCounter.ToString();
            ScoreText.text = _score.ToString();
            ComboText.text = "Combo";
            _maxComboCounter = Mathf.Max(_maxComboCounter, _comboCounter);
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

            _comboCounter = 0;
            ComboCounterText.text = "";
            ComboText.text = "";
        }

        public void TrackEnded()
        {
            EventCollector.Instance.CollectEventLogsToFile("RhythmGame");
            clearUIHandler.gameObject.SetActive(true);
            clearUIHandler.Play(CurrentTrack.TrackName, _score, _maxComboCounter, GetRanking(_score), GetAccuracy());
            CurrentTrack.enabled = false;
        }
        
        private float GetAccuracy()
        {
            return 0;
        }

        private string GetRanking(int score)
        {
            var percentage = (float)score / CurrentTrack.MaxScore;
            if (percentage >= 0.9f)
            {
                return "S";
            }
            else if (percentage >= 0.8f)
            {
                return "A";
            }
            else if (percentage >= 0.7f)
            {
                return "B";
            }
            else if (percentage >= 0.6f)
            {
                return "C";
            }
            else
            {
                return "D";
            }
        }
    }
}