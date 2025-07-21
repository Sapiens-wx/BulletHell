using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ResearchUtilities.Muse
{
    public class MuseEEGFileRecorder : MonoBehaviour
    {
        [SerializeField] private float writeIntervalSecond = 5f;
        private bool _isRecording;
        private float _recordingDuration;
        private string _filePath;
        private List<MuseEEGSample> _dataBuffer = new List<MuseEEGSample>();
        private List<MuseEEGSample> _writingBuffer = new List<MuseEEGSample>();
        private bool _isWriting;

        // During Recording
        private float _recordingStartTime;
        private float _lastWriteTime;
        private bool _needToWrite;
        private bool _isFirstWrite = true;
        
        private void Update()
        {
            if (!_isRecording)
            {
                if (_needToWrite && IsAbleToWriteBuffer())
                {
                    SwapDataAndWritingBuffer();
                    _isWriting = true;
                    BeginWriteBufferToFileAsync();
                    _needToWrite = false;
                }
                return;
            }

            var samples = MuseLslManager.Instance.GetSamplesSinceLastPull();
            _dataBuffer.AddRange(samples.ToArray());

            if (_needToWrite && IsAbleToWriteBuffer())
            {
                SwapDataAndWritingBuffer();
                _isWriting = true;
                BeginWriteBufferToFileAsync();
                _needToWrite = false;
            }
            
            if (_recordingDuration >= 0 && Time.time - _recordingStartTime >= _recordingDuration)
            {
                StopRecording();
                if (IsAbleToWriteBuffer())
                {
                    SwapDataAndWritingBuffer();
                    _isWriting = true;
                    BeginWriteBufferToFileAsync();
                }
                else
                {
                    _needToWrite = true;
                }
                return;
            }
            
            if (Time.time - _lastWriteTime >= writeIntervalSecond)
            {
                if (IsAbleToWriteBuffer())
                {
                    SwapDataAndWritingBuffer();
                    _isWriting = true;
                    BeginWriteBufferToFileAsync();
                    _lastWriteTime = Time.time;
                }
            }
        }

        private bool IsAbleToStartRecording()
        {
            if (_isWriting || _needToWrite)
                return false;
            return true;
        }

        public bool TryStartRecording(string participantId, string specifiedFileName = null, string specifiedDirectoryName = null, float duration = -1f)
        {
            if (!IsAbleToStartRecording())
            {
                Debug.LogError("Cannot start recording: writing operation is in progress or previous write operation is not finished yet.");
                return false;
            }

            _isRecording = true;
            _recordingDuration = duration;
            _recordingStartTime = Time.time;
            _isFirstWrite = true;
            
            if (specifiedDirectoryName == null) specifiedDirectoryName = Application.persistentDataPath;
            var directory = Path.Join(specifiedDirectoryName, participantId, "RecordedMuseEEGData");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            _filePath = Path.Join(directory,
                $"{(specifiedFileName != null ? specifiedDirectoryName : string.Join('_', "eeg", DateTime.Now.ToString("yyyyMMdd_HHmmss")))}.csv");

            if (File.Exists(_filePath))
            {
                Debug.LogError("File already exists. Unable to start recording.");
                return false;
            }
            return true;
        }


        private async void StopRecording()
        {
            _isRecording = false;
        }

        private bool IsAbleToWriteBuffer()
        {
            if (_dataBuffer.Count == 0)
            {
                print("No data to write.");
                return false;
            }

            if (_isWriting)
            {
                Debug.LogError("Write operation is already in progress. Or previous write operation is not finished yet.");
                return false;
            }

            return true;
        }
        private void SwapDataAndWritingBuffer()
        {
            (_dataBuffer, _writingBuffer) = (_writingBuffer, _dataBuffer);
        }
        private async void BeginWriteBufferToFileAsync()
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                if (_isFirstWrite)
                {
                    sb.AppendLine("timestamps,TP9,AF7,AF8,TP10,Right AUX");
                    _isFirstWrite = false;
                }

                foreach (var sample in _writingBuffer)
                    sb.AppendLine($"{sample.TimeStamp}, {sample.TP9}, {sample.AF7}, {sample.AF8}, {sample.TP10}, {sample.RightAux}");
                
                await using (StreamWriter writer = new StreamWriter(_filePath, true))
                    await writer.WriteAsync(sb.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError($"Error writing to file: {e.Message}");
            }
            finally
            {
                _isWriting = false;
                _writingBuffer.Clear();
            }
        }
    }
}