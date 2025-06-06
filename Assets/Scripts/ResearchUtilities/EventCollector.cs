using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace ResearchUtilities
{
    public class EventCollector : MonoBehaviour
    {
        private static EventCollector _instance;
        public static EventCollector Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EventCollector>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("EventCollector");
                        _instance = obj.AddComponent<EventCollector>();
                        DontDestroyOnLoad(_instance);
                    }
                }
                return _instance;
            }
        }
        
        private DateTime _startTime;
        private Stopwatch _stopwatch;
        
        public string DefaultLogFolderPath { get; private set; }
        private bool _filePathSet = false;
        private string FilePath => _filePathSet ? _logFilePath : DefaultLogFolderPath;
        private string _logFilePath;

        private static Dictionary<string, StringBuilder> _eventLogs = new();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _startTime = DateTime.UtcNow;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DefaultLogFolderPath = System.IO.Path.Combine(docPath, "ExperimentData");
            if (!System.IO.Directory.Exists(DefaultLogFolderPath))
                System.IO.Directory.CreateDirectory(DefaultLogFolderPath);
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DefaultLogFolderPath = System.IO.Path.Combine(docPath, "Documents", "ExperimentData");
            if (!System.IO.Directory.Exists(DefaultLogFolderPath))
                System.IO.Directory.CreateDirectory(DefaultLogFolderPath);
#elif UNITY_STANDALONE_LINUX
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            DefaultLogFolderPath = System.IO.Path.Combine(docPath, "ExperimentData");
            if (!System.IO.Directory.Exists(DefaultLogFolderPath))
                System.IO.Directory.CreateDirectory(DefaultLogFolderPath);
#elif UNITY_ANDROID || UNITY_IOS
            DefaultLogFolderPath = System.IO.Path.Combine(Application.persistentDataPath, "ExperimentData");
            if (!System.IO.Directory.Exists(DefaultLogFolderPath))
                System.IO.Directory.CreateDirectory(DefaultLogFolderPath);
#else
            DefaultLogFolderPath = System.IO.Path.Combine(Application.dataPath, "ExperimentData");
            if (!System.IO.Directory.Exists(DefaultLogFolderPath))
                System.IO.Directory.CreateDirectory(DefaultLogFolderPath);
#endif
        }

        private void Start()
        {
            // 测试：添加几条日志并导出
            RecordEvent("TestEvent", "log1", "value1");
            RecordEvent("TestEvent", "log2", "value2");
            RecordEvent("TestEvent", "log3", "value3");
            RecordEvent("AnotherEvent", "foo", "bar");
            RecordEvent("AnotherEvent", "baz", "qux");
            CollectAllEventLogsToFile();
        }

        public void CollectEventLogsToFile(string eventName)
        {
            if (!_eventLogs.ContainsKey(eventName))
                return;

            // 修正：直接在DefaultLogFolderPath下新建session子文件夹
            string sessionName = _startTime.ToString("yyyyMMdd_HHmmss");
            string directory = System.IO.Path.Combine(DefaultLogFolderPath, sessionName);
            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            // 文件名为事件名.csv，输出到session子文件夹
            string outFile = System.IO.Path.Combine(directory, eventName + ".csv");
            using (var writer = new System.IO.StreamWriter(outFile, false, Encoding.UTF8))
            {
                writer.WriteLine("Timestamp,LogContent");
                string[] lines = _eventLogs[eventName].ToString().Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    writer.WriteLine(line);
                }
            }
        }

        public void CollectAllEventLogsToFile()
        {
            foreach (string eventName in _eventLogs.Keys)
            {
                CollectEventLogsToFile(eventName);
            }
        }

        public void RecordEvent(string eventName, params string[] logContents)
        {
            DateTime now = _startTime.Add(_stopwatch.Elapsed);
            string timestamp = now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (!_eventLogs.ContainsKey(eventName))
                _eventLogs.Add(eventName, new StringBuilder());

            string logContent = string.Join(", ", logContents);
            _eventLogs[eventName].Append($"UTC {timestamp}, {logContent}\n");
        }

        public void SpecifyLogFilePath(string filePath)
        {
            if (_filePathSet)
                throw new InvalidOperationException("Log file path has already been set.");
            _logFilePath = filePath;
            _filePathSet = true;
        }
    }
}
