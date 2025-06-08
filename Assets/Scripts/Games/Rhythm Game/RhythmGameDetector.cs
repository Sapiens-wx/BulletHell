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
        
        public void ClearCurrentNote()
        {
            _currentNote = null;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            var note = other.GetComponent<RhythmGameNote>();

            if (note.Type == NoteType.Terminator)
            {
                RhythmGameManager.Instance.TrackEnded();
                note.gameObject.SetActive(false);
                return;
            }
            
            _currentNote = note;
            _currentNote.VisualRepresentation.color = Color.white * .8f;
        }
    }
}