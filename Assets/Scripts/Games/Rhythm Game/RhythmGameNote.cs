using System;
using UnityEngine;

namespace Games.Rhythm_Game
{
    public class RhythmGameNote : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer visualRepresentation;
        public SpriteRenderer VisualRepresentation => visualRepresentation;
        [SerializeField] private NoteType type;

        [SerializeField] private int pointWorth = 100;
        public int PointWorth => pointWorth;
        
        public NoteType Type => type;
        private bool _isFulfilled;

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
        

        private void Update()
        {
            if (_isFulfilled)
            {
                var cur = visualRepresentation.color;
                cur.a -= 1 / RhythmGameManager.Instance.NoteFadeDuration * Time.deltaTime;
                if (cur.a <= 0)
                    Destroy(gameObject);
                transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x,RhythmGameManager.Instance.NoteFadeZoomRatio,0.5f);
                visualRepresentation.color = cur;
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
        
        
        public void OnInputFulfilled()
        {
            transform.parent = transform.parent.parent; // unparent from the notes
            _isFulfilled = true;
        }
    }
}
