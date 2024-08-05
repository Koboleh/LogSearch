using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LogSearch
{
    public class LogSearcher
    {
        private const string PATH = "";

        public string[] GetAllMatches(string query)
        {
            var result = new List<string>();
            try
            {
                var regexs = Separate(WildcardToRegex(query));

                using (StreamReader reader = new StreamReader(PATH))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if(IsMatch(regexs, line))
                        {
                            result.Add(line);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }

            return result.ToArray();
        }

        public string[] GetLogFileView(string line) {
            var previousLines = new string[100];
            var nextLines = new List<string>();
            nextLines.Add(line);
            var pointer = 0;
            try
            {
                using (StreamReader reader = new StreamReader(PATH))
                {
                    string logLine;
                    while ((logLine = reader.ReadLine()) != null)
                    {
                        if (logLine.Contains(line))
                        {
                            for(int i = 0; i < 100 && (logLine = reader.ReadLine()) != null; i++)
                            {
                                nextLines.Add(logLine);
                            }
                            break;
                        }
                        else
                        {
                            previousLines[pointer] = logLine;
                            pointer = (pointer + 1) % 100;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
            var result = previousLines.Skip(pointer).Concat(previousLines.Take(pointer).Concat(nextLines)).Where(x => x != null).ToArray();   
            return result;
        }

        private List<string> CheckLeft(string regex)
        {
            var counter = 0;
            var result = new List<string>();
            for (int i = regex.Length - 1; i >= 0; i--)
            {
                if (regex[i] == ')')
                {
                    counter++;
                }
                else if (regex[i] == '(')
                {
                    counter--;

                    if (counter == 0)
                    {
                        result.AddRange(Separate(regex.Substring(i)));
                        break;
                    }
                    else if (counter < 0)
                    {
                        result.AddRange(Separate(regex.Substring(i + 1)));
                        break;
                    }
                }
            }

            if (counter == 0)
            {
                result.AddRange(Separate(regex));
            }

            return result;
        }

        private List<string> CheckRight(string regex)
        {
            int counter = 0;
            var result = new List<string>();
            for (int i = 0; i < regex.Length; i++)
            {
                if (regex[i] == '(')
                {
                    counter++;
                }
                else if (regex[i] == ')')
                {
                    counter--;

                    if (counter == 0)
                    {
                        result.AddRange(Separate(regex.Substring(0, i + 1)));
                        break;
                    }
                    if (counter < 0)
                    {
                        result.AddRange(Separate(regex.Substring(0, i - 1)));
                        break;
                    }
                }
            }

            if (counter == 0)
            {
                result.AddRange(Separate(regex));
            }

            return result;
        }

        private List<string> Separate(string pattern)
        {
            var result = new List<string>();

            var and = " and ";
            var andIndex = pattern.IndexOf(and);

            if (andIndex != -1)
            {
                var left = pattern.Substring(0, andIndex);
                var right = pattern.Substring(andIndex + and.Length);

                result.AddRange(CheckLeft(left));
                result.AddRange(CheckRight(right));
            }
            else
            {
                result.Add(pattern);
            }

            return result;
        }

        private string WildcardToRegex(string query)
        {
            return query.Replace("*", ".*").Replace("?", ".").Replace(" or ", "|");
        }

        private bool IsMatch(List<string> regexs, string line)
        {
            foreach (var r in regexs)
            {
                if (!Regex.IsMatch(line, r))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
