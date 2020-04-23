using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FLib.FType
{
    public class FKeyValueStringList : IEnumerable
    {
        int index = 0;
        List<FKeyValueString> list = new List<FKeyValueString>();
        FKeyValueString _last = null;

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public int Count
        {
            get
            {
                if (list == null) return 0;
                return list.Count;
            }
        }


        public FKeyValueString Add(string key, string value)
        {
            if (list == null) list = new List<FKeyValueString>();
            _last = new FKeyValueString { Key = key, Value = value };
            list.Add(_last);
            return _last;
        }

        public void Clear()
        {
            if (list == null) list = new List<FKeyValueString>();
            list.Clear();
        }

        public string toJson()
        {
            string _result = "";
            _result = JsonConvert.SerializeObject(list);
            return _result;
        }

        public void fromJson(string value)
        {
            list = JsonConvert.DeserializeObject<List<FKeyValueString>>(value);
        }
    }

    public class FKeyValueString
    {
        public string Key { get; set; }
        public string Value { get; set; }

        
        

    }
}

