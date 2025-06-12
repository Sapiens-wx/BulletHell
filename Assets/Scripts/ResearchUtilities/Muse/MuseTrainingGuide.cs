using UnityEngine;
using UnityEngine.Serialization;

namespace ResearchUtilities.Muse
{
    public class MuseTrainingGuide : MonoBehaviour
    {
        [System.Serializable]
        public class TrainingCategoryData
        {
            public string name;
            public string instruction;
            public int desiredDurationSeconds;
            public int desiredRecordingCount;
        }
        
        [SerializeField] private TrainingCategoryData[] trainingCategories = new []
        {
            // Basic categories
            new TrainingCategoryData() { name = "Left", instruction = "Think about something simple about left.", desiredDurationSeconds = 30, desiredRecordingCount = 10 },
            new TrainingCategoryData() { name = "Right", instruction = "Think about something simple about right.", desiredDurationSeconds = 30, desiredRecordingCount = 10 },
            new TrainingCategoryData() { name = "Neutral", instruction = "Think about something simple about up.", desiredDurationSeconds = 30, desiredRecordingCount = 10 },
            // Transition states
            new TrainingCategoryData() { name = "LeftToRight", instruction = "Think about Left. When recording start change to thinking about right.", desiredDurationSeconds = 1, desiredRecordingCount = 20 },
            new TrainingCategoryData() { name = "RightToLeft", instruction = "Think about Right. When recording start change to thinking about left.", desiredDurationSeconds = 1, desiredRecordingCount = 20 },
            new TrainingCategoryData() { name = "NeutralToLeft", instruction = "Think about Neutral. When recording start change to thinking about left.", desiredDurationSeconds = 1, desiredRecordingCount = 20 },
            new TrainingCategoryData() { name = "NeutralToRight", instruction = "Think about Neutral. When recording start change to thinking about right.", desiredDurationSeconds = 1, desiredRecordingCount = 20 },
            new TrainingCategoryData() { name = "LeftToNeutral", instruction = "Think about Left. When recording start change to thinking about neutral.", desiredDurationSeconds = 1, desiredRecordingCount = 20 },
            new TrainingCategoryData() { name = "RightToNeutral", instruction = "Think about Right. When recording start change to thinking about neutral.", desiredDurationSeconds = 1, desiredRecordingCount = 20 },
        };
        
    }
}