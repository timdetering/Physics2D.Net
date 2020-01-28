using System;
using System.Collections.Generic;
namespace AdvanceSystem
{
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;


/// <summary>
/// A Score Entry
/// </summary>
public class PlayerScoreEntry
{
    int[] scores;
    string playerName;
    public PlayerScoreEntry() { }
    public PlayerScoreEntry(string playerName, int[] scores)
    {
        this.scores = scores;
        this.playerName = playerName;
    }
    public int[] Scores
    {
        get { return scores; }
        set
        {
            if (scores != null)
            {
                throw new InvalidOperationException("This property has already been set and cannot be modified.");
            }
            scores = value;
        }
    }
    public string PlayerName
    {
        get { return playerName; }
        set
        {
            if (playerName != null)
            {
                throw new InvalidOperationException("This property has already been set and cannot be modified.");
            }
            playerName = value;
        }
    }
    public int TotalScore
    {
        get
        {
            int total = 0;
            for (int index = 0; index < scores.Length; ++index)
            {
                total += scores[index];
            }
            return total;
        }
    }
}
/// <summary>
/// A collection of Score Entries.
/// </summary>
public class PlayerScoreCollection
{
    /// <summary>
    /// Loads the file from a XMl format
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static PlayerScoreCollection LoadFromFile(string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            return LoadFromStream(stream);
        }
    }
    public static PlayerScoreCollection LoadFromStream(Stream stream)
    {
        if (!stream.CanRead)
        {
            throw new Exception("Non readable Stream");
        }
        return (PlayerScoreCollection)new XmlSerializer(typeof(PlayerScoreCollection)).Deserialize(stream);
    }

    string[] columnNames;
    List<PlayerScoreEntry> highScoreEntries;

    public PlayerScoreCollection() { }
    public PlayerScoreCollection(string[] columnNames)
    {
        this.columnNames = columnNames;
        this.highScoreEntries = new List<PlayerScoreEntry>();
    }

    public string[] ColumnNames
    {
        get { return columnNames; }
        set {
            if (columnNames != null)
            {
                throw new InvalidOperationException("This property has already been set and cannot be modified.");
            }
            columnNames = value; }
    }
    public PlayerScoreEntry[] HighScoreEntries
    {
        get { return highScoreEntries.ToArray(); }
        set
        {
            if (highScoreEntries != null)
            {
                throw new InvalidOperationException("This property has already been set and cannot be modified.");
            }
            highScoreEntries = new List<PlayerScoreEntry>(value);
        }
    }


    /// <summary>
    /// Adds a new Entry
    /// </summary>
    /// <param name="playerName"></param>
    /// <param name="scores"></param>
    /// <returns>the a reference to the PlayerScoreEntry</returns>
    public PlayerScoreEntry AddEntry(string playerName, int[] scores)
    {
        if (scores.Length != columnNames.Length)
        {
            throw new ArgumentOutOfRangeException("scores.Length", "incorrect number of Scores");
        }
        PlayerScoreEntry entry = new PlayerScoreEntry(playerName, scores);
        highScoreEntries.Add(entry);
        return entry;
    }


    private int GetIndexofColumn(string columnName)
    {
        return Array.IndexOf<string>(columnNames, columnName);
    }

    /// <summary>
    /// Adds a column togather and returns the total.
    /// </summary>
    /// <param name="columnName"></param>
    /// <returns></returns>
    public int GetColumnTotal(string columnName)
    {
        int columnIndex = GetIndexofColumn(columnName);
        if (columnIndex == -1)
        {
            throw new ArgumentOutOfRangeException("columnName", "column does not Exist");
        }
        return GetColumnTotal(columnIndex, 0, highScoreEntries.Count);
    }
    public int GetColumnTotal(string columnName, int startIndex)
    {
        int columnIndex = GetIndexofColumn(columnName);
        if (columnIndex == -1)
        {
            throw new ArgumentOutOfRangeException("columnName", "column does not Exist");
        }
        return GetColumnTotal(columnIndex, startIndex, highScoreEntries.Count - startIndex);
    }
    public int GetColumnTotal(string columnName, int startIndex, int count)
    {
        int columnIndex = GetIndexofColumn(columnName);
        if (columnIndex == -1)
        {
            throw new ArgumentOutOfRangeException("columnName", "column does not Exist");
        }
        return GetColumnTotal(columnIndex, startIndex, count);

    }
    public int GetColumnTotal(int columnIndex)
    {
        return GetColumnTotal(columnIndex, 0, highScoreEntries.Count);
    }
    public int GetColumnTotal(int columnIndex, int startIndex)
    {
        return GetColumnTotal(columnIndex, startIndex, highScoreEntries.Count - startIndex);
    }
    public int GetColumnTotal(int columnIndex, int startIndex, int count)
    {
        if ((columnIndex < 0) || (columnIndex > columnNames.Length))
        {
            throw new ArgumentOutOfRangeException("columnIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if ((startIndex < 0) || (startIndex > highScoreEntries.Count))
        {
            throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if ((count < 0) || (startIndex > (highScoreEntries.Count - count)))
        {
            throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
        }
        int total = 0;
        int endIndex = count + startIndex;
        for (; startIndex < endIndex; ++startIndex)
        {
            total += highScoreEntries[startIndex].Scores[columnIndex];
        }
        return total;
    }


    /// <summary>
    /// Gets the combined score of multible columns 
    /// </summary>
    /// <param name="columnNames"></param>
    /// <returns></returns>
    public int[] GetCombinedColumnScores(string[] columnNames)
    {
        return GetCombinedColumnScores(columnNames, 0, highScoreEntries.Count);
    }
    public int[] GetCombinedColumnScores(string[] columnNames, int startIndex)
    {
        return GetCombinedColumnScores(columnNames, startIndex, highScoreEntries.Count - startIndex);
    }
    public int[] GetCombinedColumnScores(string[] columnNames, int startIndex, int count)
    {
        if (columnNames == null)
        {
            throw new ArgumentNullException("columnNames");
        }
        int[] columnIndexes = new int[columnNames.Length];
        for (int columnIndex = 0; columnIndex < columnIndexes.Length; ++columnIndex)
        {
            columnIndexes[columnIndex] = GetIndexofColumn(columnNames[columnIndex]);
            if (columnIndex == -1)
            {
                throw new ArgumentOutOfRangeException("columnNames", "column does not Exist");
            }
        }
        return GetCombinedColumnScores(columnIndexes, startIndex, count);
    }
    public int[] GetCombinedColumnScores(int[] columnIndexes)
    {
        return GetCombinedColumnScores(columnIndexes, 0, highScoreEntries.Count);
    }
    public int[] GetCombinedColumnScores(int[] columnIndexes, int startIndex)
    {
        return GetCombinedColumnScores(columnIndexes, startIndex, highScoreEntries.Count - startIndex);
    }
    public int[] GetCombinedColumnScores(int[] columnIndexes, int startIndex, int count)
    {
        if (columnIndexes == null)
        {
            throw new ArgumentNullException("columnIndexes");
        }
        for (int index = 0; index < columnIndexes.Length; ++index)
        {
            if ((columnIndexes[index] < 0) || (columnIndexes[index] > columnNames.Length))
            {
                throw new ArgumentOutOfRangeException("columnIndexes", "Index was out of range. Must be non-negative and less than the size of the collection.");
            }
        }
        if ((startIndex < 0) || (startIndex > highScoreEntries.Count))
        {
            throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if ((count < 0) || (startIndex > (highScoreEntries.Count - count)))
        {
            throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
        }
        int[] rv = new int[count];
        for (int index = 0; index < count; ++index)
        {
            PlayerScoreEntry entry = highScoreEntries[index + startIndex];
            for (int columnIndex = 0; columnIndex < columnIndexes.Length; ++columnIndex)
            {
                rv[index] += entry.Scores[columnIndex];
            }
        }
        return rv;
    }


    /// <summary>
    /// Adds all the scores together
    /// </summary>
    /// <returns>the total of all the scores</returns>
    public int GetOverallTotal()
    {
        return GetOverallTotal(0, highScoreEntries.Count);
    }
    public int GetOverallTotal(int startIndex)
    {
        return GetOverallTotal(startIndex, highScoreEntries.Count - startIndex);
    }
    public int GetOverallTotal(int startIndex, int count)
    {
        if ((startIndex < 0) || (startIndex > highScoreEntries.Count))
        {
            throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if ((count < 0) || (startIndex > (highScoreEntries.Count - count)))
        {
            throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
        }
        int total = 0;
        int endIndex = count + startIndex;
        for (; startIndex < endIndex; ++startIndex)
        {
            total += highScoreEntries[startIndex].TotalScore;
        }
        return total;
    }

    /// <summary>
    /// Gets The total for each player.
    /// </summary>
    /// <returns></returns>
    public int[] GetTotals()
    {
        return GetTotals(0, highScoreEntries.Count);
    }
    public int[] GetTotals(int startIndex)
    {
        return GetTotals(startIndex, highScoreEntries.Count - startIndex);
    }
    public int[] GetTotals(int startIndex, int count)
    {
        if ((startIndex < 0) || (startIndex > highScoreEntries.Count))
        {
            throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if ((count < 0) || (startIndex > (highScoreEntries.Count - count)))
        {
            throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
        }
        int[] totals = new int[count];
        int endIndex = count + startIndex;
        for (; startIndex < endIndex; ++startIndex)
        {
            totals[startIndex] = highScoreEntries[startIndex].TotalScore;
        }
        return totals;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columnName"></param>
    public void SortByScore(string columnName)
    {
        int index = GetIndexofColumn(columnName);
        if (index == -1)
        {
            throw new ArgumentOutOfRangeException("columnName", "column does not Exist");
        }
        SortByScore(index);
    }
    public void SortByScore(int columnIndex)
    {
        if (columnIndex == -1)
        {
            throw new ArgumentOutOfRangeException("columnIndex", "column does not Exist");
        }
        highScoreEntries.Sort(delegate(PlayerScoreEntry left, PlayerScoreEntry right)
        {
            return left.Scores[columnIndex].CompareTo(right.Scores[columnIndex]);
        });
    }
    /// <summary>
    /// Sorts the Collection by the Player's names
    /// </summary>
    public void SortByPlayer()
    {
        SortByPlayer(StringComparison.CurrentCulture);
    }
    public void SortByPlayer(StringComparison stringComparison)
    {
        highScoreEntries.Sort(delegate(PlayerScoreEntry left, PlayerScoreEntry right)
        {
            return string.Compare(left.PlayerName, right.PlayerName, stringComparison);
        });
    }

    /// <summary>
    /// Saves the file in XMl form
    /// </summary>
    /// <param name="path"></param>
    public void SaveToFile(string path)
    {
        using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write))
        {
            SaveToStream(stream);
        }
    }
    public void SaveToStream(Stream stream)
    {
        if (!stream.CanWrite)
        {
            throw new Exception("non Writable Stream");
        }
        new XmlSerializer(typeof(PlayerScoreCollection)).Serialize(stream, this);
    }


    /// <summary>
    /// Turns the Score list into a string.
    /// </summary>
    /// <param name="lineFormat">
    /// The format parameter where {0} is the players name.
    /// and the indexes after it represent the index 
    /// </param>
    /// <returns>A string with its first line being the name of each column instead of the actaul score</returns>
    /// <example>
    /// ToString("{0} | {1} | {2}") would return
    /// Player's Name  | Score1 | Score2
    /// Bob | 1 | 0
    /// Fredd | 2 | 4
    /// </example>
    public string ToString(string lineFormat)
    {
        return ToString(lineFormat,0, highScoreEntries.Count);
    }
    public string ToString(string lineFormat, int startIndex)
    {
        return ToString(lineFormat, startIndex, highScoreEntries.Count - startIndex);
    }
    public string ToString(string lineFormat, int startIndex, int count)
    {
        if (lineFormat == null)
        {
            throw new ArgumentNullException("lineFormat");
        }
        if ((startIndex < 0) || (startIndex > highScoreEntries.Count))
        {
            throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        if ((count < 0) || (startIndex > (highScoreEntries.Count - count)))
        {
            throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
        }


        StringBuilder builder = new StringBuilder();
        object[] objarr = new object[columnNames.Length + 1];
        
        objarr[0] = "Player's Name";
        columnNames.CopyTo(objarr, 1);
        builder.AppendFormat(lineFormat, objarr);
        builder.AppendLine();

        int endIndex = count + startIndex;
        for (; startIndex < endIndex; ++startIndex)
        {
            objarr[0] = highScoreEntries[startIndex].PlayerName;
            highScoreEntries[startIndex].Scores.CopyTo(objarr, 1);
            builder.AppendFormat(lineFormat, objarr);
            builder.AppendLine();
        }
        return builder.ToString();
    }
}


    /*public class EventedList<T> : List<T>
    {
        public event EventHandler Changed;
        public EventedList() : base() { }
        public EventedList(IEnumerable<T> collection) : base(collection) { }
        public EventedList(int capacity) : base(capacity) { }
        public new void Clear()
        {
            base.Clear();
            OnChange();
        }
        public new void Add(T item)
        {
            base.Add(item);
            OnChange();
        }
        public new bool Remove(T item)
        {
            bool rv = base.Remove(item);
            if (rv) { OnChange(); }
            return rv;
        }
        public new int RemoveAll(Predicate<T> match)
        {
            int rv = base.RemoveAll(match);
            if (rv > 0) { OnChange(); }
            return rv;
        }
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            OnChange();
        }
        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            OnChange();
        }
        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            OnChange();
        }
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnChange();
        }
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            OnChange();
        }
        public void OnChange()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }
    }*/


    public static class AdvEnviroment
    {
        static readonly string processFileName;
        static AdvEnviroment()
        {
            using (System.Diagnostics.Process thisProcess = System.Diagnostics.Process.GetCurrentProcess())
            {
                processFileName = thisProcess.MainModule.FileName;
            }
        }
        public static string ProcessFileName
        {
            get { return AdvEnviroment.processFileName; }
        } 
        public static string ProcessDirectory
        {
            get { return Path.GetDirectoryName(AdvEnviroment.processFileName); }
        }
    }

    public delegate bool Equation<T>(T left, T right);
    public static class Functions
    {
        public static class SetTheory
        {
            public static T[] Union<T>(T[] leftArray, T[] rightArray)
            {
                if (leftArray == null || leftArray.Length == 0)
                {
                    return rightArray;
                }
                if (rightArray == null || rightArray.Length == 0)
                {
                    return leftArray;
                }
                return UnionInternal<T>(new List<T>(leftArray), rightArray);
            }
            private static T[] UnionInternal<T>(List<T> rv, T[] rightArray)
            {
                for (int index = 0; index < rightArray.Length; ++index)
                {
                    T item = rightArray[index];
                    if (!rv.Contains(item))
                    {
                        rv.Add(item);
                    }
                }
                return rv.ToArray();
            }
            public static T[] Intersection<T>(T[] leftArray, T[] rightArray)
            {
                if (leftArray == null ||
                    leftArray.Length == 0 ||
                    rightArray == null ||
                    rightArray.Length == 0)
                {
                    return null;
                }
                List<T> rv = new List<T>();
                for (int index = 0; index < rightArray.Length; ++index)
                {
                    T item = rightArray[index];
                    if (Array.IndexOf<T>(leftArray, item) != -1)
                    {
                        rv.Add(item);
                    }
                }
                if (rv.Count == 0)
                {
                    return null;
                }
                return rv.ToArray();
            }
            public static T[] Intersection<T>(T[] leftArray, T[] rightArray, Equation<T> equator)
            {
                if (leftArray == null ||
                    leftArray.Length == 0 ||
                    rightArray == null ||
                    rightArray.Length == 0)
                {
                    return null;
                }
                List<T> rv = new List<T>();
                for (int index = 0; index < rightArray.Length; ++index)
                {
                    T item = rightArray[index];
                    if (Functions.Lists.Exists<T>(leftArray, item, equator))
                    {
                        rv.Add(item);
                    }
                }
                if (rv.Count == 0)
                {
                    return null;
                }
                return rv.ToArray();
            }
            public static bool CheckIntersection<T>(T[] leftArray, T[] rightArray)
            {
                if (leftArray == null ||
                    leftArray.Length == 0 ||
                    rightArray == null ||
                    rightArray.Length == 0)
                {
                    return false;
                }
                for (int index = 0; index < rightArray.Length; ++index)
                {
                    if (Array.IndexOf<T>(leftArray, rightArray[index]) != -1)
                    {
                        return true;
                    }
                }
                return false;
            }
            public static bool CheckIntersection<T>(T[] leftArray, T[] rightArray, Equation<T> equator)
            {
                if (leftArray == null ||
                    leftArray.Length == 0 ||
                    rightArray == null ||
                    rightArray.Length == 0)
                {
                    return false;
                }
                for (int leftIndex = 0; leftIndex < leftArray.Length; ++leftIndex)
                {
                    for (int rightIndex = 0; rightIndex < rightArray.Length; ++rightIndex)
                    {
                        if (equator(leftArray[leftIndex], rightArray[rightIndex]))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public static bool CheckRelativeComplement<T>(T[] leftArray, T[] rightArray)
            {
                if (leftArray == null ||
                    leftArray.Length == 0)
                {
                    return false;
                }
                if (rightArray == null ||
                    rightArray.Length == 0)
                {
                    return true;
                }
                for (int index = 0; index < leftArray.Length; ++index)
                {
                    T item = leftArray[index];
                    if (Array.IndexOf<T>(rightArray, item) == -1)
                    {
                        return true;
                    }
                }
                return false;
            }
            public static T[] RelativeComplement<T>(T[] leftArray, T[] rightArray)
            {
                if (leftArray == null ||
                    leftArray.Length == 0)
                {
                    return null;
                }
                if (rightArray == null ||
                    rightArray.Length == 0)
                {
                    return leftArray;
                }
                List<T> rv = new List<T>();
                for (int index = 0; index < leftArray.Length; ++index)
                {
                    T item = leftArray[index];
                    if (Array.IndexOf<T>(rightArray, item) == -1)
                    {
                        rv.Add(item);
                    }
                }
                if (rv.Count == 0)
                {
                    return null;
                }
                return rv.ToArray();
            }
        }
        /// <summary>
        /// A bunch of classes written for useability
        /// </summary>
        public static class Lists
        {
            private static Equation<T> GetEquation<T>(Comparison<T> comparison)
            {
                if (comparison == null)
                {
                    throw new ArgumentNullException("comparison");
                }
                return delegate(T left, T right) { return comparison(left, right) == 0; };
            }
            public static bool Exists<T>(IList<T> list, T item, Comparison<T> comparison)
            {
                return Exists<T>(list, item, GetEquation<T>(comparison));
            }
            public static bool Exists<T>(IList<T> list, T item, Equation<T> equator)
            {
                return (IndexOf<T>(list, item, equator) != -1);
            }
            public static int IndexOf<T>(IList<T> list, T item, Comparison<T> comparison)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return IndexOf<T>(list, item, 0, list.Count, GetEquation<T>(comparison));
            }
            public static int IndexOf<T>(IList<T> list, T item, Equation<T> equator)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return IndexOf<T>(list, item, 0, list.Count, equator);
            }
            public static int IndexOf<T>(IList<T> list, T item, int startIndex, Comparison<T> comparison)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return IndexOf<T>(list, item, startIndex, list.Count - startIndex, GetEquation<T>(comparison));
            }
            public static int IndexOf<T>(IList<T> list, T item, int startIndex, Equation<T> equator)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return IndexOf<T>(list, item, startIndex, list.Count - startIndex, equator);
            }
            public static int IndexOf<T>(IList<T> list, T item, int startIndex, int count, Comparison<T> comparison)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return IndexOf<T>(list, item, startIndex, count, GetEquation<T>(comparison));
            }
            public static int IndexOf<T>(IList<T> list, T item, int startIndex, int count, Equation<T> equator)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                if (equator == null)
                {
                    throw new ArgumentNullException("equator");
                }
                if ((startIndex < 0) || (startIndex > list.Count))
                {
                    throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                if ((count < 0) || (startIndex > (list.Count - count)))
                {
                    throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
                }
                int endpos = startIndex + count;
                for (int pos = startIndex; pos < endpos; pos++)
                {
                    if (equator(list[pos], item))
                    {
                        return pos;
                    }
                }
                return -1;
            }

            public static int LastIndexOf<T>(IList<T> list, T item, Comparison<T> comparison)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return LastIndexOf<T>(list, item, list.Count - 1, list.Count, GetEquation<T>(comparison));
            }
            public static int LastIndexOf<T>(IList<T> list, T item, Equation<T> equator)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return LastIndexOf<T>(list, item, list.Count - 1, list.Count, equator);
            }
            public static int LastIndexOf<T>(IList<T> list, T item, int startIndex, Comparison<T> comparison)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return LastIndexOf<T>(list, item, startIndex, startIndex + 1, GetEquation<T>(comparison));
            }
            public static int LastIndexOf<T>(IList<T> list, T item, int startIndex, Equation<T> equator)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return LastIndexOf<T>(list, item, startIndex, startIndex + 1, equator);
            }
            public static int LastIndexOf<T>(IList<T> list, T item, int startIndex, int count, Comparison<T> comparison)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                return LastIndexOf<T>(list, item, startIndex, count, GetEquation<T>(comparison));

            }
            public static int LastIndexOf<T>(IList<T> list, T item, int startIndex, int count, Equation<T> equator)
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                if (equator == null)
                {
                    throw new ArgumentNullException("equator");
                }
                if (list.Count == 0)
                {
                    if (startIndex != -1)
                    {
                        throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
                    }
                }
                else if ((startIndex < 0) || (startIndex >= list.Count))
                {
                    throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                if ((count < 0) || (((startIndex - count) + 1) < 0))
                {
                    throw new ArgumentOutOfRangeException("count", "Count must be positive and count must refer to a location within the collection.");
                }
                int endpos = startIndex - count;
                for (int pos = startIndex; pos > endpos; pos--)
                {
                    if (equator(list[pos], item))
                    {
                        return pos;
                    }
                }
                return -1;
            }
            public static bool Equals<T>(IList<T> leftList, IList<T> rightList)
            {
                if (leftList == rightList) { return true; }
                if (leftList == null ^ null == rightList) { return false; }
                if (leftList.Count != rightList.Count) { return false; }
                for (int index = 0; index < leftList.Count; ++index)
                {
                    if (!leftList[index].Equals(rightList[index])) { return false; }
                }
                return true;
            }
            public static bool Equals<T>(IList<T> leftList, IList<T> rightList, Comparison<T> comparison)
            {
                return Equals<T>(leftList, rightList, GetEquation<T>(comparison));
            }
            public static bool Equals<T>(IList<T> leftList, IList<T> rightList, Equation<T> equator)
            {
                if (equator == null)
                {
                    throw new ArgumentNullException("equator");
                }
                if (leftList == rightList) { return true; }
                if (leftList == null ^ null == rightList) { return false; }
                if (leftList.Count != rightList.Count) { return false; }
                for (int index = 0; index < leftList.Count; ++index)
                {
                    if (!equator(leftList[index],rightList[index])) { return false; }
                }
                return true;
            }
        }
        private delegate int IndexOfCallback<T>(T[] array, T item, int startIndex, int count);

        public static int RemoveDuplicates<T>(ref T[] array)
        {
            return RemoveDuplicates<T>(ref array, Array.IndexOf<T>);
        }
        public static int RemoveDuplicates<T>(ref T[] array, Equation<T> equator)
        {
            if (equator == null)
            {
                throw new ArgumentNullException("equator");
            }
            return RemoveDuplicates<T>(ref array,
                delegate(T[] array2, T item, int startIndex, int count)
                {
                    return Lists.IndexOf<T>(array2, item, startIndex, count, equator);
                });
        }
        private static int RemoveDuplicates<T>(ref T[] array, IndexOfCallback<T> IndexOf)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int index = 0;
            int size = array.Length;
            while ((index < size) && IndexOf(array, array[index], 0, index) == -1)
            {
                index++;
            }
            if (index >= size)
            {
                return 0;
            }
            int duplicateIndex = index + 1;
            for (; ; )
            {
                if (duplicateIndex >= size)
                {
                    int duplicatesLength = size - index;
                    Array.Resize<T>(ref array, index);
                    return duplicatesLength;
                }
                while ((duplicateIndex < size) &&
                   IndexOf(array, array[duplicateIndex], 0, index) != -1)
                {
                    duplicateIndex++;
                }
                if (duplicateIndex < size)
                {
                    array[index++] = array[duplicateIndex++];
                }
            }

        }
        

        public static int SeperateDuplicates<T>(ref T[] array, out T[] duplicates)
        {
            return SeperateDuplicates<T>(ref array, out duplicates,Array.IndexOf<T>);
        }
       /* public static void SeperateDuplicates<T>(ref T[] array, out T[] duplicates)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int index = 0;
            int size = array.Length;
            while ((index < size) &&
                Array.IndexOf<T>(array, array[index], 0, index) == -1)
            {
                index++;
            }
            if (index >= size)
            {
                duplicates = new T[0];
                return;
            }
            int duplicateIndex = index + 1;
            for (; ; )
            {
                if (duplicateIndex >= size)
                {
                    int duplicatesLength = size - index;
                    size = index;
                    duplicates = new T[duplicatesLength];
                    Array.Copy(array, index, duplicates, 0, duplicatesLength);
                    Array.Resize<T>(ref array, index);
                    return;
                }
                while ((duplicateIndex < size) &&
                    Array.IndexOf<T>(array, array[duplicateIndex], 0, index) != -1)
                {
                    duplicateIndex++;
                }
                if (duplicateIndex < size)
                {
                    T temp = array[index];
                    array[index++] = array[duplicateIndex];
                    array[duplicateIndex++] = temp;
                }
            }

        }*/
        public static int SeperateDuplicates<T>(ref T[] array, out T[] duplicates, Equation<T> equator)
        {
            if (equator == null)
            {
                throw new ArgumentNullException("equator");
            }
            return SeperateDuplicates<T>(ref array, out duplicates,
                delegate(T[] array2, T item, int startIndex, int count)
                {
                    return Lists.IndexOf<T>(array2, item, startIndex, count, equator);
                });
        }
        private static int SeperateDuplicates<T>(ref T[] array, out T[] duplicates, IndexOfCallback<T> IndexOf)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            int index = 0;
            int size = array.Length;
            while ((index < size) && IndexOf(array, array[index], 0, index) == -1)
            {
                index++;
            }
            if (index >= size)
            {
                duplicates = null;
                return 0;
            }
            int duplicateIndex = index + 1;
            for(;;)
            {
                if (duplicateIndex >= size)
                {
                    int duplicatesLength = size - index;
                    duplicates = new T[duplicatesLength];
                    Array.Copy(array, index, duplicates, 0, duplicatesLength);
                    Array.Resize<T>(ref array, index);
                    return duplicatesLength;
                }
                while ((duplicateIndex < size) &&
                   IndexOf(array, array[duplicateIndex], 0, index) != -1)
                {
                    duplicateIndex++;
                }
                if (duplicateIndex < size)
                {
                    T temp = array[index];
                    array[index++] = array[duplicateIndex];
                    array[duplicateIndex++] = temp;
                }
            }

        }

        public static bool ArrayEquals(Array leftArray, Array rightArray)
        {
            if (leftArray == rightArray) { return true; }
            if (leftArray == null ^ null == rightArray) { return false; }
            if (leftArray.Length != rightArray.Length) { return false; }
            for (int index = 0; index < leftArray.Length; ++index)
            {
                if (!leftArray.GetValue(index).Equals(rightArray.GetValue(index))) { return false; }
            }
            return true;
        }

        public static T[] ArrayConcat<T>(T[] leftArray, T[] rightArray)
        {
            if (leftArray == null ^ rightArray == null)
            {
                if (leftArray == null)
                {
                    return (T[])rightArray.Clone();
                }
                else
                {
                    return (T[])leftArray.Clone();
                }
            }
            else
            {
                if (leftArray == null)
                {
                    return null;
                }
                else
                {
                    T[] rv = new T[leftArray.Length + rightArray.Length];
                    leftArray.CopyTo(rv, 0);
                    rightArray.CopyTo(rv, leftArray.Length);
                    return rv;
                }
            }

        }
        public static T[] ArrayConcat<T>(params T[][] arrays)
        {
            if (arrays == null)
            {
                throw new ArgumentNullException("arrays", "The array of " + typeof(T).Name + "[] cannot be null");
            }
            int finalLength = 0;
            for (int pos = 0; pos < arrays.Length; ++pos)
            {
                if (arrays[pos] != null)
                {
                    finalLength += arrays[pos].Length;
                }
            }
            int position = 0;
            T[] rv = new T[finalLength];
            for (int pos = 0; pos < arrays.Length; ++pos)
            {
                T[] array = arrays[pos];
                if (array != null)
                {
                    array.CopyTo(rv, position);
                    position += array.Length;
                }
            }
            return rv;
        }
        public static T[] SubArray<T>(T[] array, int startIndex, int length)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array", "The array cannot be null");
            }
            int arrayLength = array.Length;
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex", "StartIndex cannot be less than zero.");
            }
            if (startIndex > arrayLength)
            {
                throw new ArgumentOutOfRangeException("startIndex", "startIndex cannot be larger than length of the array.");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "Length cannot be less than zero.");
            }
            if (startIndex > (arrayLength - length))
            {
                throw new ArgumentOutOfRangeException("length", "Index and length must refer to a location within the array.");
            }
            if (length == 0)
            {
                return new T[0];
            }
            T[] rv = new T[length];
            Array.Copy(array, startIndex, rv, 0, length);
            return rv;
        }
        public static T[] ArrayInsert<T>(T[] array, int startIndex, params T[] needle)
        {
            if (startIndex == 0)
            {
                return ArrayConcat<T>(needle, array);
            }
            if (array == null)
            {
                throw new ArgumentNullException("array", "The array cannot be null");
            }
            if (needle == null)
            {
                return (T[])array.Clone();
            }
            if (startIndex == array.Length)
            {
                return ArrayConcat<T>(array, needle);
            }
            T[] rv = new T[array.Length + needle.Length];
            Array.Copy(array, 0, rv, 0, startIndex);
            Array.Copy(needle, 0, rv, startIndex, needle.Length);
            Array.Copy(array, startIndex, rv, startIndex + array.Length, array.Length - startIndex);
            return rv;
        }

        public static void Sort<TComparable>(ref TComparable min, ref TComparable max)
            where TComparable : IComparable<TComparable>
        {
            if (max.CompareTo(min) < 0)
            {
                Swap<TComparable>(ref min, ref max);
            }
        }
        public static void Sort<TComparable, TOther>(ref TComparable min, ref TComparable max, ref TOther minObject, ref TOther maxObject)
            where TComparable : IComparable<TComparable>
        {
            if (max.CompareTo(min) < 0)
            {
                Swap<TComparable>(ref min, ref max);
                Swap<TOther>(ref minObject, ref maxObject);
            }
        }
        public static void Swap<T>(ref T left, ref T right)
        {
            T tmp = left;
            left = right;
            right = tmp;
        }


        public static T Clone<T>(T obj) where T : ICloneable
        {
            T rv = default(T);
            if (obj != null)
            {
                rv = (T)obj.Clone();
            }
            return rv;
        }

        public static T LoadXmlFile<T>(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = File.OpenRead(path))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
        public static void SaveXmlFile<T>(string path, T item)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, item);
            }
        }

    }
}
