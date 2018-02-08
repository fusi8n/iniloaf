using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace iniloaf
{
    public class ini
    {
        public ini(string file, string commentDelimiter = "#")
        {
            CommentDelimiter = commentDelimiter;
            TheFile = file;
        }

        public ini()
        {
            CommentDelimiter = ";";
        }

        public string CommentDelimiter { get; set; }

        private string theFile = null;
        public string TheFile
        {
            get
            {
                return theFile;
            }
            set
            {
                theFile = null;
                dictionary.Clear();
                if (File.Exists(value))
                {
                    theFile = value;
                    using (StreamReader sr = new StreamReader(theFile))
                    {
                        string line, section = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.Length == 0) continue;
                            if (!String.IsNullOrEmpty(CommentDelimiter) && line.StartsWith(CommentDelimiter))
                                continue;

                            if (line.StartsWith("[") && line.Contains("]"))
                            {
                                int index = line.IndexOf(']');
                                section = line.Substring(1, index - 1).Trim();
                                continue;
                            }

                            if (line.Contains("="))
                            {
                                int index = line.IndexOf('=');
                                string key = line.Substring(0, index).Trim();
                                string val = line.Substring(index + 1).Trim();
                                string key2 = String.Format("[{0}]{1}", section, key).ToLower();

                                if (val.StartsWith("\"") && val.EndsWith("\""))
                                    val = val.Substring(1, val.Length - 2);

                                if (dictionary.ContainsKey(key2))
                                {
                                    index = 1;
                                    string key3;
                                    while (true)
                                    {
                                        key3 = String.Format("{0}~{1}", key2, ++index);
                                        if (!dictionary.ContainsKey(key3))
                                        {
                                            dictionary.Add(key3, val);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    dictionary.Add(key2, val);
                                }
                            }
                        }
                    }
                }
            }
        }
        private Dictionary<string, string> dictionary = new Dictionary<string, string>();

        private bool TryGetValue(string section, string key, out string value)
        {
            string key2;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key);
            else
                key2 = String.Format("[{0}]{1}", section, key);

            return dictionary.TryGetValue(key2.ToLower(), out value);
        }
        public string GetValue(string section, string key, string defaultValue = "")
        {
            string value;
            if (!TryGetValue(section, key, out value))
                return defaultValue;

            return value;
        }
        public string this[string section, string key]
        {
            get
            {
                return GetValue(section, key);
            }
        }
        public int GetInteger(string section, string key, int defaultValue = 0,
            int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            string stringValue;
            if (!TryGetValue(section, key, out stringValue))
                return defaultValue;

            int value;
            if (!int.TryParse(stringValue, out value))
            {
                double dvalue;
                if (!double.TryParse(stringValue, out dvalue))
                    return defaultValue;
                value = (int)dvalue;
            }

            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;
            return value;
        }
        public double GetDouble(string section, string key, double defaultValue = 0,
            double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            string stringValue;
            if (!TryGetValue(section, key, out stringValue))
                return defaultValue;

            double value;
            if (!double.TryParse(stringValue, out value))
                return defaultValue;

            if (value < minValue)
                value = minValue;
            if (value > maxValue)
                value = maxValue;
            return value;
        }
        public bool GetBoolean(string section, string key, bool defaultValue = false)
        {
            string stringValue;
            if (!TryGetValue(section, key, out stringValue))
                return defaultValue;

            return (stringValue != "0" && !stringValue.StartsWith("f", true, null));
        }
        public string[] GetAllValues(string section, string key)
        {
            string key2, key3, value;
            if (section.StartsWith("["))
                key2 = String.Format("{0}{1}", section, key).ToLower();
            else
                key2 = String.Format("[{0}]{1}", section, key).ToLower();

            if (!dictionary.TryGetValue(key2, out value))
                return null;

            List<string> values = new List<string>();
            values.Add(value);
            int index = 1;
            while (true)
            {
                key3 = String.Format("{0}~{1}", key2, ++index);
                if (!dictionary.TryGetValue(key3, out value))
                    break;
                values.Add(value);
            }

            return values.ToArray();
        }

        internal string Read(string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
}