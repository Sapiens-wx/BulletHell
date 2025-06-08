using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Rhythm_Game
{
    public class RhythmGameDetector : MonoBehaviour
    {
        [SerializeField] private NoteType noteType;
        public RhythmGameNote _currentNote;
        private float _currentControlAccumulation;
        private RhythmGameTrack _currentTrack;
        public float timeAccumulation;

        private void Start()
        {
            _currentTrack = transform.parent.parent.GetComponent<RhythmGameTrack>();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            _currentNote = other.GetComponent<RhythmGameNote>();
            _currentNote.VisualRepresentation.color = Color.white * .8f;
        }
    }
}