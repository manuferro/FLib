using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using FLib.FUtility;

namespace FLib.FUtility
{
    /// <summary>
    /// group of parameters
    /// </summary>
    public class FParameters : IEnumerable, IEnumerator
    {
        private char[] FROM_TEXT_DIVIDER = { ';', ',' };
        private char[] FROM_TEXT_ASSIGNER = { ':', '=' };
        private string _title = "";
        public string Name { get; set; }
        public string Title { get => _title; set => _title = value; }
        public string Help { get; set; }
        public int Count {
            get => _paramList == null ? 0 : _paramList.Count;
                }

        int index = 0;
        private List<FParameter> _paramList = null;
        public List<FParameter> Parameters{ 
            get{return _paramList;}
            set{ _paramList = value; }
        }
        public FParameter LastPar { 
            get { 
                if (_paramList == null) return null;
                return _paramList[_paramList.Count - 1];
            } 
        }
        public FParameters() { }
        public FParameters(string name) { this.Name = name; }

        /*********************************** 
         * add/update / remove 
         */
        public void Add(FParameter param)
        {
            if (_paramList == null) _paramList = new List<FParameter>();
            _paramList.Add(param);
        }

        public void Add(string name, string value, string label = "", bool display = false, bool optional = false)
        {
            this.Add(new FParameter { Name = name, Value = value, PType = FParameter.TYPE_TEXT, UserInput = display, Optional = optional , Label=label});
        }

        public void Add(string name, int value, string label = "", bool display = false, bool optional = false)
        {
            this.Add(new FParameter { Name = name, Value = value, PType = FParameter.TYPE_INT, UserInput=display, Optional=optional, Label = label });
        }

        public void Add(string name, string value, string options, string secondaryType, string label = "", bool display = false, bool optional = false)
        {
            FParameter parm = new FParameter
            {
                Name = name,
                Value = value,
                PType = FParameter.TYPE_TEXT,
                UserInput = display,
                Optional = optional,
                Label = label,
                Options = options,
                SType = secondaryType

            };

            
            this.Add(parm);
        }

        public void Add(string name, int value, string options, string secondaryType, string label = "", bool display = false, bool optional = false)
        {
            FParameter parm = new FParameter
            {
                Name = name,
                Value = value,
                PType = FParameter.TYPE_INT,
                UserInput = display,
                Optional = optional,
                Label = label,
                Options = options,
                SType = secondaryType

            };


            this.Add(parm);
        }


        public FParameter getValue(string paramName)
        {
            if (_paramList == null) return null;
            foreach( FParameter par in _paramList)
            {
                if (par.Name == paramName)
                    return par;
            }
            return null;
        }

        /// <summary>
        /// get the string value of parameter
        /// </summary>
        /// <param name="paramName">name of parameter</param>
        /// <returns></returns>
        public string getStrValue(string paramName)
        {
            FParameter par = getValue(paramName);
            if (par == null) return "";
            if (par.PType == FParameter.TYPE_TEXT) return @"""" + par.Value.ToString() + @"""";
            if (par.PType == FParameter.TYPE_INT) return  par.Value.ToString();
            return par.Value.ToString();



        }

        public bool setValue(string paramName, object value)
        {
            FParameter par = getValue(paramName);
            if (par == null) return false;
            par.Value = value;
            return true;
        }

        

        /*********************************** 
        * text management
        */

        public string replaceInText(string text)
        {
            string result = text;
            foreach(FParameter par in _paramList)
            {
                result = result.Replace("$$" + par.Name.ToUpper() + "$", par.Value.ToString());
            }

            return result;

        }

        public FParameters fromSpecialText(string input, string paramType = FParameter.TYPE_TEXT)
        {
            string[] listParam = input.Split(FROM_TEXT_DIVIDER);

            foreach(string keyValueStr in listParam)
            {
                string[] listKeyValue = keyValueStr.Split(FROM_TEXT_ASSIGNER);
                if (listKeyValue.Length >= 2)
                {
                    string key = listKeyValue[0];
                    object value = null;
                    
                    if (paramType == FParameter.TYPE_TEXT) value =  listKeyValue[1].ToString();
                    if (paramType == FParameter.TYPE_INT)
                    {
                        int val = 0;
                        try
                        {
                            val = Convert.ToInt32(listKeyValue[1]);
                        }
                        catch (Exception ex) { key = "DATA ERROR!"; }
                        value = val;
                    }


                    if (_paramList == null) _paramList = new List<FParameter>();
                    _paramList.Add(new FParameter { Name = key, Value = value });
                }


            }
            return this;
        }

        /*********************************** 
         * json management
         */

        public void fromJText(string text)
        {
            string newText = FStrUtility.getNoCmtRegex(text);
            _paramList =  JsonConvert.DeserializeObject<List<FParameter>>(newText);
            //return JsonConvert.DeserializeObject<FParameters>(newText);

        }

        public  string toJText()
        {
            string newText = JsonConvert.SerializeObject(_paramList);
            return newText;

        }

        /*********************************** 
         * file managemnet
        */

        public bool save(string fileName = "")
        {
            FConfig fc = new FConfig();
            bool result = fc.saveParam(this.Name, toJText());
            return result;
        }

        public bool load(string fileName = "")
        {
            bool result = false;
            FConfig fc = new FConfig();
            string txt = "";
            if (fc.getParam(this.Name, out txt))
            {
                //FParameters fp = FParameters.fromJText(txt);
                fromJText(txt);
                //this.Name = fp.Name;
                result = true;
            }
            return result;
        }


        /*************************************
         * IENUMERABLE
         */
        object IEnumerator.Current
        {
            get {
                if (_paramList == null) return null;
                if (_paramList.Count < index) return null;
                return _paramList[index]; 
            }
        }

        bool IEnumerator.MoveNext()
        {
            index++;
            if (_paramList == null)
            {
                index = 0;
                return false;
            }

            if (index > _paramList.Count)
            {
                index = _paramList.Count;
                return false;
            }

            return true;
        }

        void IEnumerator.Reset()
        {
            index = 0;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Parameters).GetEnumerator();
        }
    }

    /// <summary>
    /// base parameter class
    /// </summary>
    public class FParameter
    {
        public const string ATTRIBUTE_HELP = "MULTILINE";
        public const string TYPE_HELP = "TEXT,BOOL,INT,DECIMAL";
        public const string TYPE_TEXT = "TEXT";
        public const string TYPE_BOOL = "BOOL";
        public const string TYPE_INT = "INT";

        private string _label = "";
        

        public string Name { get; set; }
        public string Label { get { if (_label.Length > 0) return _label; return Name; } set { _label = value; } }
        public string Help { get; set; }
        public string Attributes { get; set; }
        public string PType { get; set; }
        public Object Value { get; set; }
        public bool UserInput { get; set; }
        public bool Optional { get; set; }
        public string Options { get; set; }
        //sub type
        public string SType { get; set; }   

        public FParameter()
        {
            Name = Label = Help = Attributes = Options = SType = "";
            PType = TYPE_TEXT;
            
        }

        public FParameters getOptionsList()
        {
            if ((Options == null) || (Options.Length < 3)) return null;
            FParameters result = new FParameters(Name);
            return result.fromSpecialText(Options, PType);
            return null;
        }

    }
}

