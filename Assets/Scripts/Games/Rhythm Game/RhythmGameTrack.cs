using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Rhythm_Game
{
    public class RhythmGameTrack : MonoBehaviour
    {
        [SerializeField] private Transform notesParent;
        [SerializeField, Range(0.0167f, 1f)] private float moveDuration;
        private bool _isMoving;
        private float _moveProgress;

        [SerializeField] private float bpm;
        [SerializeField] private float beatsPerMove;
        
        [SerializeField] public RhythmGameDetector leftDetector, rightDetector;
        public float BPM => bpm;
        public float BeatsPerMove => beatsPerMove;
        private float _secondPerMove => beatsPerMove / bpm * 60f;
        private float _moveCounter;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                MoveDown();
            
            _moveCounter += Time.deltaTime;
            if (_moveCounter >= _secondPerMove)
            {
                _moveCounter -= _secondPerMove;
                MoveDown();
            }
            
            if (_isMoving)
            {
                var deltaProgress = Time.deltaTime / moveDuration;
                _moveProgress += deltaProgress;
                if (_moveProgress > 1)
                    deltaProgress -= _moveProgress - 1f;
                notesParent.position -= notesParent.up * deltaProgress;
                if (_moveProgress > 1)
                    _isMoving = false;
            }
        }


        public void MoveDown()
        {
            if (_isMoving)
                return;
            _isMoving = true;
            _moveProgress = 0f;
            leftDetector.timeAccumulation = 0;
            rightDetector.timeAccumulation = 0;
        }

        public void AccumulateControlInput(NoteType noteType, float deltaTime)
        {
            var detector = noteType switch
            {
                NoteType.Left => leftDetector,
                NoteType.Right => rightDetector,
                _ => throw new ArgumentOutOfRangeException(nameof(noteType), noteType, null)
            };
            if (!detector._currentNote)
                return;
            detector.timeAccumulation += deltaTime;
            if (detector.timeAccumulation >= _secondPerMove * RhythmGameManager.Instance.CorrectNoteThreshold)
                Destroy(detector._currentNote.gameObject);
        }
    }
}
