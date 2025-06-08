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

        private readonly Dictionary<string, EventLog> EventLogs = new();

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

        public void CollectEventLogsToFile(string eventName)
        {
            if (!EventLogs.ContainsKey(eventName))
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
                writer.WriteLine(EventLogs[eventName].HeaderContent);
                string[] lines = EventLogs[eventName].ContentBuilder.ToString().Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    writer.WriteLine(line);
                }
            }
        }

        public void CollectAllEventLogsToFile()
        {
            foreach (string eventName in EventLogs.Keys)
            {
                CollectEventLogsToFile(eventName);
            }
        }
        
        
        public void SetEventLogHeader(string eventName, params string[] headers)
        {
            if (!EventLogs.ContainsKey(eventName))
                EventLogs.Add(eventName, new EventLog());
            EventLogs[eventName].HeaderContent = "Timestamp, " + string.Join(", ", headers);
        }

        public void RecordEvent(string eventName, params string[] logContents)
        {
            DateTime now = _startTime.Add(_stopwatch.Elapsed);
            string timestamp = now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (!EventLogs.ContainsKey(eventName))
                EventLogs.Add(eventName, new EventLog());

            string logContent = string.Join(", ", logContents);
            EventLogs[eventName].ContentBuilder.Append($"UTC {timestamp}, {logContent}\n");
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
