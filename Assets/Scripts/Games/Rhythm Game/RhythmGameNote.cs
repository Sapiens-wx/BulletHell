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
            }
            
        }

        private void OnValidate()
        {
            if (type == NoteType.Left)
            {
                visualRepresentation.flipX = true;
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0.25f);
            }
            else if (type == NoteType.Right)
            {
                visualRepresentation.flipX = false;
                GetComponent<BoxCollider2D>().offset = new Vector2(0.5f, 0.25f);
            }


        }
    }
}
