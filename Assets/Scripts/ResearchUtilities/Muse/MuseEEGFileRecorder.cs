using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace ResearchUtilities.Muse
{
    public class MuseEEGFileRecorder : MonoBehaviour
    {
        [SerializeField] private float writeInterval = 5f;
        private bool _isRecording;
        private float _recordingDuration;
        private string _filePath;
        private float _lastWriteTime;
        private List<MuseEEGSample> _dataBuffer = new List<MuseEEGSample>();
        private List<MuseEEGSample> _writingBuffer = new List<MuseEEGSample>();
        private bool _isWriting;

        public void Update()
        {
            if (_isRecording)
            {
                var samples = MuseLSLEEGManager.Instance.GetSamplesSinceLastPull();
                if (samples != null && samples.Length > 0)
                {
                    _dataBuffer.AddRange(samples.ToArray());
                }

                if (Time.time - _lastWriteTime >= writeInterval)
                {
                    _lastWriteTime = Time.time;
                    WriteBufferToFileAsync();
                }
            }
        }

        private async Task WriteBufferToFileAsync()
        {
            if (_dataBuffer.Count == 0) return;

            if (_isWriting)
            {
                Debug.LogError(
                    "Previous write operation hasn't finished yet. Your game might be too busy. Consider optimizing your game or increasing the write interval! Or perhaps you should get a better device.");
                return;
            }

            _isWriting = true;
            (_dataBuffer, _writingBuffer) = (_writingBuffer, _dataBuffer); // Swap buffers to avoid blocking the main thread and avoid gc cost
            _dataBuffer.Clear();

            try
            {
                await using (StreamWriter writer = new StreamWriter(_filePath, true))
                {
                    foreach (var sample in _writingBuffer)
                    {
                        // Csv format: Timestamp,TP9,AF7,AF8,TP10,RightAux
                        string line = $"{sample.TimeStamp},{sample.TP9},{sample.AF7},{sample.AF8},{sample.TP10},{sample.RightAux}";
                        await writer.WriteLineAsync(line);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error writing to file: {e.Message}");
            }
            finally
            {
                _isWriting = false;
            }
        }

        public void StartRecording(float recordingDuration, string directory, string fileName)
        {
            if (!MuseLSLEEGManager.Instance.IsConnected)
            {
                Debug.LogError("Unable to start recording: Muse LSL EEG Manager is not connected to Muse LSL.");
                return;
            }
            if (_isRecording)
            {
                Debug.LogWarning("Recording is already in progress.");
                return;
            }
            
            _dataBuffer.Clear();
            _writingBuffer.Clear();

            try
            {
                // Make sure the directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Debug.Log($"Created directory: {directory}");
                }

                string filePath = Path.Combine(directory, fileName);
                _filePath = filePath;
                
                // Create file and write header
                File.WriteAllText(filePath, "Timestamp,TP9,AF7,AF8,TP10,RightAux\n");
                
                _isRecording = true;
                _recordingDuration = recordingDuration;
                _lastWriteTime = Time.time;
                _dataBuffer.Clear();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating directory or file: {e.Message}");
                _isRecording = false;
                _filePath = null;
            }
        }

        public void StopRecording()
        {
            if (!_isRecording) return;
            
            _isRecording = false;
            _filePath = null;
            // Final write to ensure all data is saved
            WriteBufferToFileAsync().ConfigureAwait(false);
        }
    }
}