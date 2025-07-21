using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using LSL;
using ResearchUtilities.DataStructure;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ResearchUtilities.Muse
{
    [RequireComponent(typeof(MuseEEGFileRecorder))]
    public class MuseLslManager : MonoBehaviour
    {
        public static MuseLslManager Instance { get; private set; }

        public static void GetOrCreateInstance(out MuseLslManager instance)
        {
            instance = Instance;
            if (instance)
                return;
            var go = new GameObject("MuseLslManager");
            instance = go.AddComponent<MuseLslManager>();
            go.AddComponent<MuseEEGFileRecorder>();
            return;
        }
        
        private StreamInlet _inlet;
        public bool IsConnected => _inlet != null;
        
        private ShadowWrappedBuffer<MuseEEGSample> _samplesBuffer;
        private const int SAMPLE_RATE = 256;
        [SerializeField] private int bufferTimeSeconds = 2;
        
        private MuseEEGFileRecorder _fileRecorder;

        private void Awake()
        {
            if (Instance)
                Destroy(Instance);
            Instance = this;
            _samplesBuffer = new ShadowWrappedBuffer<MuseEEGSample>(SAMPLE_RATE * bufferTimeSeconds);
            EstablishConnection();
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _fileRecorder = GetComponent<MuseEEGFileRecorder>();
            if (!_fileRecorder)
                gameObject.AddComponent<MuseEEGFileRecorder>();
        }

        private void Update()
        {
            if (_inlet == null) return;
            
            PullAndBufferSamples();
        }

        public bool EstablishConnection()
        {
            try
            {
                var results = LSL.LSL.resolve_stream("type", "EEG", 1, 5.0);
                if (results.Length > 0)
                {
                    _inlet = new StreamInlet(results[0]);
                    Debug.Log("已连接到 Muse LSL 流");
                    return true;
                }
                Debug.LogWarning("未找到 Muse LSL 流");
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"连接 Muse LSL 时发生错误: {e.Message}");
                _inlet = null;
                return false;
            }
        }
        

        private void OnDestroy()
        {
            if (_inlet != null)
            {
                _inlet.close_stream();
                _inlet = null;
            }
        }
        
        #region Buffer Management
        private void PullAndBufferSamples()
        {
            try
            {
                _samplesBuffer.UnmarkDirty();
                float[] sample = new float[5]; // TP9, AF7, AF8, TP10, Right AUX
                var corrections = _inlet.time_correction(); // 获取时间校正值

                while (_inlet.pull_sample(sample, corrections) > 0) // 第二个参数是timeout，返回的是sample的时间戳
                {
                    var museSample = new MuseEEGSample
                    {
                        TimeStamp = LSL.LSL.local_clock() + corrections,
                        TP9 = sample[0],
                        AF7 = sample[1],
                        AF8 = sample[2],
                        TP10 = sample[3],
                        RightAux = sample[4]
                    };
                    _samplesBuffer.Write(museSample);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"读取 Muse 数据时发生错误: {e.Message}");
                _inlet = null;
            }
        }
        
        public ReadOnlySpan<MuseEEGSample> GetSamplesSinceLastPull() => _samplesBuffer.PeekDirty();

        #endregion
    }
}