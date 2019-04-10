using System;
using System.Collections.Generic;
using System.Linq;

namespace LogExpert
{
    public class AzureColumnizer : ILogLineColumnizer
    {
        private const string EndOfLogMessage = "\",";
        private const string StatsAndMessageSeperator = ",\"";
        public string GetName()
        {
            return "Azure Logs Columnizer"; //Some great name so everyone knows from just that whats on
        }

        public string GetDescription()
        {
            return "Columnizer for Azure logs"; 
        }

        public int GetColumnCount()
        {
            return 9; 
        }

        public string[] GetColumnNames()
        {
            return new string[]
            {
                "Date",
                "Level",
                "Application Name",
                "Instance Id",
                "Message",
                "Event Tick Count",
                "Event Id",
                "PID",
                "TID"                
            };
        }

        public IColumnizedLogLine SplitLine(ILogLineColumnizerCallback callback, ILogLine line)
        {
            if (line.LineNumber == 0 || line.FullLine == EndOfLogMessage)
                return null;

            var columns = new List<IColumn>();
            ColumnizedLogLine columnizedLogLine = new ColumnizedLogLine();
            columnizedLogLine.LogLine = line;

            var logParts = line.FullLine.Split(new string[] { StatsAndMessageSeperator }, StringSplitOptions.RemoveEmptyEntries);
            if (logParts.Length == 2)
            {
                var informationParts = logParts[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var messagePart = logParts[1];

                columns.AddRange(informationParts.ToList().Take(4).Select(p => GetColumn(columnizedLogLine, p)).ToList());
                columns.Add(GetColumn(columnizedLogLine, messagePart));
                columns.AddRange(informationParts.ToList().Skip(4).Take(3).Select(p => GetColumn(columnizedLogLine, p)).ToList());                
            }
            else
            {                
                for (int i = 0; i < 4; i++)
                    columns.Add(GetColumn(columnizedLogLine, string.Empty));
                columns.Add(GetColumn(columnizedLogLine, line.FullLine));
            }

            columnizedLogLine.ColumnValues = columns.ToArray();
            return columnizedLogLine;
        }

        private static Column GetColumn(ColumnizedLogLine columnizedLogLine, string value)
        {
            return new Column
            {
                FullValue = value,
                Parent = columnizedLogLine //Just add the reference to the columnizedLogLine
            };
        }

        public bool IsTimeshiftImplemented()
        {
            return false; //This is an example and we don't support timeshift
        }

        public void SetTimeOffset(int msecOffset)
        {
            throw new NotImplementedException(); //Nothing to do here
        }

        public int GetTimeOffset()
        {
            throw new NotImplementedException();  //Nothing to do here
        }

        public DateTime GetTimestamp(ILogLineColumnizerCallback callback, ILogLine line)
        {
            throw new NotImplementedException(); //Nothing to do here
        }

        public void PushValue(ILogLineColumnizerCallback callback, int column, string value, string oldValue)
        {
            //Nothing to do here
        }
    }
}
