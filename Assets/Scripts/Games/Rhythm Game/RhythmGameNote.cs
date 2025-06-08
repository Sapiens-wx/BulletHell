using System;
using UnityEngine;

namespace Games.Rhythm_Game
{
    public class RhythmGameNote : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualRepresentation;
        public SpriteRenderer VisualRepresentation => visualRepresentation;
        [SerializeField] private NoteType type;
        
        public NoteType Type => type;

        private void Start()
        {
            switch (type)
            {
                case NoteType.Left:
                    visualRepresentation.flipX = true;
                    break;
                case NoteType.Right:
                    visualRepresentation.flipX = false;
                    break;
                case NoteType.Terminator:
                    visualRepresentation.color = Color.clear;
                    break;
            }
            
        }

        private void OnValidate()
        {
            if (!visualRepresentation)
                visualRepresentation = GetComponent<SpriteRenderer>();
            if (type == NoteType.Left)
            {
                visualRepresentation.color = Color.white;
                visualRepresentation.flipX = true;
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0.25f);
            }
            else if (type == NoteType.Right)
            {
                visualRepresentation.color = Color.white;
                visualRepresentation.flipX = false;
                GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0.25f);
            }
            else if (type == NoteType.Terminator)
            {
                visualRepresentation.color = Color.gray;
                visualRepresentation.flipX = false;
                GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0.25f);
            }


        }
    }
}
