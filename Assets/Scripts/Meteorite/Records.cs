using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meteorite{
[CreateAssetMenu(fileName ="",menuName = "inventory/records")]
public class Records : ScriptableObject
{
    public List<Record> records;
    public void AddRecord(Record record){
        int idx=records.BinarySearch(record);
        if(idx>=0)
            throw new Exception("adding duplicate record");
        //if binary search cannot find the element, it returns [~i], i is the index where the new element should be inserted
        idx=~idx;
        //insert the element
        if(idx>=records.Count) records.Add(record);
        else records.Insert(idx, record);
        //keep the records of the top 10 elements.
        while(records.Count>10)
            records.RemoveAt(records.Count-1);
    }
    [System.Serializable]
    public class Record:IComparable<Record>{
        public string name;
        public float score,accuracy;
        [SerializeField]public DateTime time;
        public Record(string name, float score, float accuracy){
            this.name=name;
            this.score=score;
            this.accuracy=accuracy;
            time=DateTime.Now;
        }
        public int CompareTo(Record other){
            if(this==other) return 0;
            if(other.score!=this.score) return -score.CompareTo(other.score);
            if(accuracy!=other.accuracy) return -accuracy.CompareTo(other.accuracy);
            return 1;
        }
    }
}
}