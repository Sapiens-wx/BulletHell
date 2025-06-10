using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Games.Rhythm_Game
{
    public class RhythmGameClearUIHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform backgroundMovementEndAnchor;
        [SerializeField] private RectTransform backgroundMovementStartAnchor;
        
        
        
        [SerializeField] private TextMeshProUGUI songNameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI rankingText;
        [SerializeField] private TextMeshProUGUI rankTextContent;
        [SerializeField] private TextMeshProUGUI maxComboText;
        [SerializeField] private TextMeshProUGUI maxComboTextContent;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI accuracyTextContent;

        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image rankBackgroundFill;
        [SerializeField] private Image rankingBackgroundOutline;
        
        
        private Vector3 _backgroundStartPosition;
        private float _playTime;
        private bool _isPlaying;
        private float _backgroundTansparency;

        private void Start()
        {
            _backgroundStartPosition = background.anchoredPosition;
            _backgroundTansparency = backgroundImage.color.a;
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (_isPlaying)
            {
                _playTime += Time.deltaTime;
                if (_playTime >= RhythmGameManager.Instance.ClearUIMoveInDuration)
                {
                    _playTime = RhythmGameManager.Instance.ClearUIMoveInDuration;
                    _isPlaying = false;
                }
                float t = _playTime / RhythmGameManager.Instance.ClearUIMoveInDuration;
                background.anchoredPosition = Vector3.Lerp(backgroundMovementStartAnchor.anchoredPosition, backgroundMovementEndAnchor.anchoredPosition, t);
                
                songNameText.color = new Color(songNameText.color.r, songNameText.color.g, songNameText.color.b, Mathf.Lerp(0f, 1f, t));
                scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, Mathf.Lerp(0f, 1f, t));
                maxComboText.color = new Color(maxComboText.color.r, maxComboText.color.g, maxComboText.color.b, Mathf.Lerp(0f, 1f, t));
                rankingText.color = new Color(rankingText.color.r, rankingText.color.g, rankingText.color.b, Mathf.Lerp(0f, 1f, t));
                rankBackgroundFill.color = new Color(rankBackgroundFill.color.r, rankBackgroundFill.color.g, rankBackgroundFill.color.b, Mathf.Lerp(0f, 1f, t));
                rankingBackgroundOutline.color = new Color(rankingBackgroundOutline.color.r, rankingBackgroundOutline.color.g, rankingBackgroundOutline.color.b, Mathf.Lerp(0f, 1f, t));
                rankTextContent.color = new Color(rankTextContent.color.r, rankTextContent.color.g, rankTextContent.color.b, Mathf.Lerp(0f, 1f, t));
                maxComboText.color = new Color(maxComboText.color.r, maxComboText.color.g, maxComboText.color.b, Mathf.Lerp(0f, 1f, t));
                maxComboTextContent.color = new Color(maxComboTextContent.color.r, maxComboTextContent.color.g, maxComboTextContent.color.b, Mathf.Lerp(0f, 1f, t));
                accuracyText.color = new Color(accuracyText.color.r, accuracyText.color.g, accuracyText.color.b, Mathf.Lerp(0f, 1f, t));
                accuracyTextContent.color = new Color(accuracyTextContent.color.r, accuracyTextContent.color.g, accuracyTextContent.color.b, Mathf.Lerp(0f, 1f, t));
                backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, Mathf.Lerp(0f, _backgroundTansparency, t));
            }
            
            
        }
        
        public void Play(string songName, int score, int maxCombo, string ranking, float accuracy)
        {
            gameObject.SetActive(true);
            _playTime = 0f;
            _isPlaying = true;
            _backgroundStartPosition = background.anchoredPosition;
            
            songNameText.text = songName;
            scoreText.text = $"Score: {score}";
            maxComboTextContent.text = maxCombo.ToString();
            accuracyTextContent.text = $"{accuracy * 100f:0.00}%";
            maxComboTextContent.text = maxCombo.ToString();
            rankTextContent.text = ranking;

            songNameText.color = new Color(songNameText.color.r, songNameText.color.g, songNameText.color.b, 0f);
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 0f);
            maxComboText.color = new Color(maxComboText.color.r, maxComboText.color.g, maxComboText.color.b, 0f);
            rankingText.color = new Color(rankingText.color.r, rankingText.color.g, rankingText.color.b, 0f);

            Color rankColor = ranking switch
            {
                "S" => new Color(1f, 0.8f, 0.2f), // Gold
                "A" => new Color(0.2f, 0.8f, 1f), // Blue
                "B" => new Color(0.2f, 1f, 0.2f), // Green
                "C" => new Color(1f, 0.5f, 0.5f), // Red
                "D" => new Color(0.5f, 0.5f, 0.5f), // Gray
                _   => new Color(1f, 1f, 1f)       // Default white
            };
            
            rankColor.a = 0f;
            rankingText.color = rankColor;
            rankingBackgroundOutline.color = rankColor;
            rankBackgroundFill.color = new Color(rankBackgroundFill.color.r, rankBackgroundFill.color.g, rankBackgroundFill.color.b, 0f);
            


        }
    }
}