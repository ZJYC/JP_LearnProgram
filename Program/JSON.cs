using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Algorithm;
using ZJYC;

namespace ZJYC_JSON
{
    public class Entry
    {
        public class EntryParam
        {
            public int ID = 0;
            public double ErrorRate = 1.0;
            public int ErrorCount = 0;
            public int LearnCount = 1;
            public DateTime LastTime { get; set; }

            public EntryParam()
            {
                this.LastTime = DateTime.Now;
                this.ErrorRate = 0.0;
            }

            public void Process(string Mode)
            {
                if(Mode == "单词")ErrorRate = 1.0 * ErrorCount / LearnCount;
            }

        }

        public EntryParam PM;
        public string CS { get; set; }
        public string JP { get; set; }
        public string CH { get; set; }
        public string ER { get; set; }

        public Entry(string JP, string CH,string CS)
        {
            this.JP = JP;
            this.CH = CH;
            this.CS = CS;
            this.PM = new EntryParam();
        }

        public void Process()
        {
            PM.Process(this.CS);
            ER = PM.ErrorRate.ToString("F2");
        }

    }
    /// <summary>
    /// 前级过滤器
    /// </summary>
    public class FilterFormer
    {
        public string FilterMode = string.Empty;

        public string [] SupportedFilterMode = { "全部", "单词", "语法", "翻译" };

        public FilterFormer(string Mode)
        {
            this.FilterMode = Mode;
        }

        public void Filter(ref List<Entry> Entries)
        {
            try
            {
                if (this.FilterMode == string.Empty) int.Parse("");
                if (SupportedFilterMode.Contains(this.FilterMode) == false) int.Parse("");

                //全部：无事可做
                if (this.FilterMode == "全部") { return; }
                //进行过滤
                for (int i = 0; i < Entries.Count;)
                {
                    if (Entries[i].CS != this.FilterMode)
                    {
                        Entries.Remove(Entries[i]);
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }
                
            }
            catch
            {
                MessageBox.Show("FilterFormer.Filter");
            }
        }

    }
    /// <summary>
    /// 中级过滤器
    /// </summary>
    public class FilterMiddle
    {
        public string FilterMode = string.Empty;
        public string FilterParam = string.Empty;

        public string[] SupportedFilterMode = { "全部", "错误次数", "时间间隔", "学习次数", "错误率"};

        public FilterMiddle(string Mode,string Param)
        {
            this.FilterMode = Mode;
            this.FilterParam = Param;
        }

        private void ParseConfigParam(string SortConfigParam, ref double CountMin, ref double CountMax)
        {

            double Min = 0, Max = 0;

            if (SortConfigParam.Contains("-") == true)
            {
                string[] Split = SortConfigParam.Split('-');
                Min = double.Parse(Split[0]);
                Max = double.Parse(Split[1]);
            }
            else
            {
                Min = double.Parse(SortConfigParam);
                Max = double.Parse(SortConfigParam);
            }

            CountMin = (Min <= Max ? Min : Max);
            CountMax = (Min <= Max ? Max : Min);

        }

        delegate bool ShouldRemoveDelegate(Entry Entry,double Min, double Max);

        private bool ShouldRemoveTemplate(Entry Entry, double Min, double Max)
        {
            return false;
        }
        
        private bool ShouldRemoveByErrorCount(Entry Entry,double Min,double Max)
        {
            if (Min <= Entry.PM.ErrorCount && Entry.PM.ErrorCount <= Max) return false;
            return true;
        }

        private bool ShouldRemoveByDateInterval(Entry Entry, double Min, double Max)
        {
            DateTime CurTime = DateTime.Now;

            DateTime Bgn = DateTime.Now;
            DateTime End = DateTime.Now;

            Bgn = (Min <= Max ? CurTime.AddDays(Max * -1) : CurTime.AddDays(Min * -1));
            End = (Min <= Max ? CurTime.AddDays(Min * -1) : CurTime.AddDays(Max * -1));

            if (DateTime.Compare(Bgn, Entry.PM.LastTime) <= 0 && DateTime.Compare(Entry.PM.LastTime, End) <= 0)return false;
            return true;
        }

        private bool ShouldRemoveByLearnCount(Entry Entry, double Min, double Max)
        {
            if (Min <= Entry.PM.LearnCount && Entry.PM.LearnCount <= Max) return false;
            return true;
        }

        private bool ShouldRemoveByErrorRate(Entry Entry, double Min, double Max)
        {
            if (Min <= Entry.PM.ErrorRate && Entry.PM.ErrorRate <= Max) return false;
            return true;
        }

        public void Filter(ref List<Entry> Entries)
        {
            try
            {
                if (this.FilterMode == string.Empty) int.Parse("");
                if (SupportedFilterMode.Contains(this.FilterMode) == false) int.Parse("");

                //全部：无事可做
                if (this.FilterMode == "全部") { return; }
                //进行过滤
                double Min = 0, Max = 0;
                ParseConfigParam(this.FilterParam, ref Min, ref Max);
                ShouldRemoveDelegate ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveTemplate);

                if (this.FilterMode == "错误次数") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveByErrorCount);
                if (this.FilterMode == "时间间隔") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveByDateInterval);
                if (this.FilterMode == "学习次数") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveByLearnCount);
                if (this.FilterMode == "错误率") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveByErrorRate);

                for (int i = 0; i < Entries.Count;)
                {
                    if (ShouldRemove(Entries[i],Min,Max))
                    {
                        Entries.Remove(Entries[i]);
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }

            }
            catch
            {
                MessageBox.Show("FilterMiddle.Filter");
            }
        }

    }
    /// <summary>
    /// 排序器
    /// </summary>
    public class SorterFormer
    {
        public string SorterMode = string.Empty;

        public string[] SupportedSorterMode = { "学习时间", "学习次数", "错误次数", "错误率" };

        public SorterFormer(string Mode)
        {
            this.SorterMode = Mode;
        }

        private int SortByTimeLastTime(Entry A, Entry B)
        {
            return DateTime.Compare(A.PM.LastTime, B.PM.LastTime);
        }

        private int SortByTimeLearnCount(Entry A, Entry B)
        {
            if (A.PM.LearnCount > B.PM.LearnCount) return -1;
            if (A.PM.LearnCount < B.PM.LearnCount) return 1;
            return 0;
        }

        private int SortByTimeErrorCount(Entry A, Entry B)
        {
            if (A.PM.ErrorCount > B.PM.ErrorCount) return -1;
            if (A.PM.ErrorCount < B.PM.ErrorCount) return 1;
            return 0;
        }

        private int SortByTimeErrorRate(Entry A, Entry B)
        {
            if (A.PM.ErrorRate > B.PM.ErrorRate) return -1;
            if (A.PM.ErrorRate < B.PM.ErrorRate) return 1;
            return 0;
        }

        public void Sortor(ref List<Entry> Entries)
        {
            try
            {
                if (this.SorterMode == string.Empty) int.Parse("");
                if (SupportedSorterMode.Contains(this.SorterMode) == false) int.Parse("");

                if (this.SorterMode == "学习时间") Entries.Sort(SortByTimeLastTime);
                if (this.SorterMode == "学习次数") Entries.Sort(SortByTimeLearnCount);
                if (this.SorterMode == "错误次数") Entries.Sort(SortByTimeErrorCount);
                if (this.SorterMode == "错误率") Entries.Sort(SortByTimeErrorRate);
            }
            catch
            {
                MessageBox.Show("FilterMiddle.Filter");
            }
        }

    }

    public class FilterRear
    {
        public string FilterMode = string.Empty;
        public string FilterParam = string.Empty;

        public string[] SupportedFilterMode = { "全部","起始","前几个","后几个"};

        public FilterRear(string Mode)
        {
            this.FilterMode = Mode;
        }
        private void ParseConfigParam(string SortConfigParam, ref double CountMin, ref double CountMax)
        {

            double Min = 0, Max = 0;

            if (SortConfigParam.Contains("-") == true)
            {
                string[] Split = SortConfigParam.Split('-');
                Min = double.Parse(Split[0]);
                Max = double.Parse(Split[1]);
            }
            else
            {
                Min = double.Parse(SortConfigParam);
                Max = double.Parse(SortConfigParam);
            }

            CountMin = (Min <= Max ? Min : Max);
            CountMax = (Min <= Max ? Max : Min);

            CountMin -= 1;
            CountMax -= 1;

        }

        delegate bool ShouldRemoveDelegate(ref List<Entry> Entries,Entry Entry, double Min, double Max);

        private bool ShouldRemoveTemplate(ref List<Entry> Entries,Entry Entry, double Min, double Max)
        {
            return false;
        }

        private bool ShouldRemoveBeginToEnd(ref List<Entry> Entries, Entry Entry, double Min, double Max)
        {
            if (Min <= Entries.IndexOf(Entry) && Entries.IndexOf(Entry) <= Max) return false;
            return true;
        }

        private bool ShouldRemoveTop(ref List<Entry> Entries, Entry Entry, double Min, double Max)
        {
            if (Entries.IndexOf(Entry) <= Max) return false;
            return true;
        }

        private bool ShouldRemoveEnd(ref List<Entry> Entries, Entry Entry, double Min, double Max)
        {
            if (Entries.Count - 1 - Min <= Entries.IndexOf(Entry)) return false;
            return true;
        }

        public void Filter(ref List<Entry> Entries)
        {
            try
            {
                if (this.FilterMode == string.Empty) int.Parse("");
                if (SupportedFilterMode.Contains(this.FilterMode) == false) int.Parse("");

                //全部：无事可做
                if (this.FilterMode == "全部") { return; }
                //进行过滤
                double Min = 0, Max = 0;
                ParseConfigParam(this.FilterParam, ref Min, ref Max);
                ShouldRemoveDelegate ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveTemplate);

                if (this.FilterMode == "起始") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveBeginToEnd);
                if (this.FilterMode == "前几个") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveTop);
                if (this.FilterMode == "后几个") ShouldRemove = new ShouldRemoveDelegate(ShouldRemoveEnd);

                for (int i = 0; i < Entries.Count;)
                {
                    if (ShouldRemove(ref Entries,Entries[i], Min, Max))
                    {
                        Entries.Remove(Entries[i]);
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }

            }
            catch
            {
                MessageBox.Show("FilterRear.Filter");
            }
        }
    }

    public class FinderFormer
    {
        private algorithm Algorithm = new algorithm();
        public string Keyword = string.Empty;

        public FinderFormer(string Keyword)
        {
            this.Keyword = Keyword;
        }

        public void Finder(ref List<Entry> Entries)
        {
            if (Keyword == "") return;
            for (int i = 0; i < Entries.Count;)
            {
                if (Algorithm.Sim(Keyword, Entries[i].CH) < 0.2 &&
                    Algorithm.Sim(Keyword, Entries[i].JP) < 0.2)
                {
                    Entries.Remove(Entries[i]);
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }

    }
    public class ZJSON
    {
        public class LowLayerOpt
        {
            public void ReadStringFrJsonFile(string JsonFileName, ref string Content)
            {
                try
                {
                    using (StreamReader Reader = new StreamReader(JsonFileName))
                    {
                        Content = Reader.ReadToEnd();
                    }
                }
                catch
                {
                    MessageBox.Show("ReadStringFrJsonFile" + JsonFileName);
                }
            }
            public void WritStringToJsonFile(string JsonFileName, ref string Content)
            {
                try
                {
                    using (StreamWriter Writer = new StreamWriter(JsonFileName))
                    {
                        Writer.Write("");
                        Writer.Write(Content);
                    }
                }
                catch
                {
                    MessageBox.Show("WritJsonFile" + JsonFileName);
                }
            }
        }

        private LowLayerOpt LLO = new LowLayerOpt();
        private algorithm Algorithm = new algorithm();
        private string JsonFileName = string.Empty;
        public List<Entry> JsonCache = new List<Entry>();

        public ZJSON(string JsonFileName)
        {
            this.JsonFileName = JsonFileName;
            ReadJsonFileToCache();
        }

        public void FillJsonFileWsBlank()
        {
            List<Entry> Temp = new List<Entry>(){new Entry(JP:"XXXXXX",CH:"XXXXXX",CS:"单词" ),};
            string TempString = JsonConvert.SerializeObject(Temp);
            LLO.WritStringToJsonFile(this.JsonFileName, ref TempString);
        }

        public void ReadJsonFileToCache()
        {
            string TempString = string.Empty;
            LLO.ReadStringFrJsonFile(this.JsonFileName,ref TempString);
            JsonCache = JsonConvert.DeserializeObject<List<Entry>>(TempString);
        }
        public void WritJsonFileFrCache()
        {
            int i = 0;
            foreach(Entry entry in JsonCache){entry.PM.ID = i++;entry.Process();}
            string TempString = JsonConvert.SerializeObject(JsonCache);
            LLO.WritStringToJsonFile(this.JsonFileName, ref TempString);
        }

        private int GetIndexByID_FromJsonCache(int ID)
        {
            for (int i = 0; i < JsonCache.Count; i++)
            {
                if (JsonCache[i].PM.ID == ID) return i;
            }
            MessageBox.Show("GetIndexByID_FromJsonCache" + ID.ToString());
            return 0;
        }

        public void EntryAddTOCache(Entry entry)
        {
            try
            {
                JsonCache.Add(entry);
            }
            catch
            {
                MessageBox.Show("DeleteOneEntryFrJsonFile");
            }
        }
        public void EntryAddTOCache(List<Entry> Entries)
        {
            try
            {
                JsonCache.AddRange(Entries);
            }
            catch
            {
                MessageBox.Show("DeleteOneEntryFrJsonFile");
            }
        }
        public void EntryModifyInCache(int ID,Entry entry)
        {
            int Index = GetIndexByID_FromJsonCache(ID);
            try
            {
                JsonCache[Index].CH = entry.CH;
                JsonCache[Index].JP = entry.JP;
                JsonCache[Index].PM.LastTime = DateTime.Now;
            }
            catch
            {
                MessageBox.Show("EntryModifyInCache");
            }
        }
        public void EntryModifyInCache(int ID, int ErrorCountAdd, int LearnCountAdd)
        {
            int Index = GetIndexByID_FromJsonCache(ID);
            try
            {
                JsonCache[Index].PM.ErrorCount += ErrorCountAdd;
                JsonCache[Index].PM.LearnCount += LearnCountAdd;
                JsonCache[Index].PM.LastTime = DateTime.Now;
            }
            catch
            {
                MessageBox.Show("EntryModifyInCache");
            }
        }
        public void EntryModifyInCache(int ID, double ErrorRate)
        {
            int Index = GetIndexByID_FromJsonCache(ID);
            try
            {
                double Sum = ErrorRate;
                Sum += JsonCache[Index].PM.ErrorRate * JsonCache[Index].PM.LearnCount;
                JsonCache[Index].PM.LearnCount += 1;
                JsonCache[Index].PM.ErrorRate = Sum / JsonCache[Index].PM.LearnCount;
                JsonCache[Index].PM.LastTime = DateTime.Now;
            }
            catch
            {
                MessageBox.Show("EntryModifyInCache");
            }
        }
        public void EntryDeleteIncache(int ID)
        {
            try
            {
                int Index = GetIndexByID_FromJsonCache(ID);
                JsonCache.Remove(JsonCache[Index]);
            }
            catch
            {
                MessageBox.Show("DeleteOneEntryFrJsonFile");
            }
        }

        public List<Entry> JsonCacheClone()
        {
            List<Entry> entries = new List<Entry>();
            foreach(Entry entry in JsonCache)entries.Add(entry);
            return entries;
        }
        public void SetJsonFileName(string FileName)
        {
            WritJsonFileFrCache();
            this.JsonFileName = FileName;
            ReadJsonFileToCache();
        }

    }
    public class SingleChoiceQuestions
    {

        public class OUT
        {
            public string Title = string.Empty;
            public string SelectionA = string.Empty;
            public string SelectionB = string.Empty;
            public string SelectionC = string.Empty;
            public string SelectionD = string.Empty;
            public string RightSelet = string.Empty;
        }

        public OUT Out = new OUT();

        private ZJSON Json;
        private ZRandom zRandom = new ZRandom();
        private Regular regular = new Regular();
        private GenInterference genInterference = new GenInterference();
        private algorithm Algorithm = new algorithm();

        public string Mode = string.Empty;
        public string TstType = string.Empty;
        public string Difficulty = string.Empty;

        public string[] SupportedMode = { "顺序", "随机" };
        public string[] SupportedTstType = { "JP=>CH", "CH=>JP" };
        public string[] SupportedDifficulty = { "Easy", "JustReplace" };

        public SingleChoiceQuestions(ref ZJSON Json,string Mode)
        {
            this.Mode = Mode;
            this.Json = Json;
        }

        private List<Entry> Entries = new List<Entry>();
        private List<int> RegulrIndexList = new List<int>();
        private List<int> RandomIndexList = new List<int>();

        private Entry TstEntryThisTime;
        private void SelectOneEntryToUse()
        {
            List<int> IndexList = new List<int>();

            if (Mode == "随机") IndexList = RandomIndexList;
            if (Mode == "顺序") IndexList = RegulrIndexList;

            if (IndexList.Count == 0)
            {
                MessageBox.Show("没有了");
                return;
            }
            int Index = IndexList[0];
            TstEntryThisTime = Entries[Index];
            IndexList.Remove(Index);
        }

        public void ImportEntries(ref List<Entry> Entries,string Mode)
        {
            this.Entries.Clear();
            foreach (Entry entry in Entries) this.Entries.Add(entry);
            RandomIndexList = zRandom.List(0, Entries.Count - 1);
            RegulrIndexList = regular.GetOrderList(0, Entries.Count - 1);
        }   
        public void GenOneQuestion(List<Entry> Entries)
        {
            List<string> Res = new List<string>();

            SelectOneEntryToUse();

            if (Difficulty == "Easy" && TstType == "CH=>JP")
            {
                Res = Algorithm.FindSimiliarTopJPX(Entries, TstEntryThisTime.JP, 4);
                Out.Title = TstEntryThisTime.CH;
                Out.RightSelet = TstEntryThisTime.JP;
            }
            if (Difficulty == "Easy" && TstType == "JP=>CH")
            {
                Res = Algorithm.FindSimiliarTopCHX(Entries, TstEntryThisTime.CH, 4);
                Out.Title = TstEntryThisTime.JP;
                Out.RightSelet = TstEntryThisTime.CH;
            }
            if (Difficulty == "JustReplace" && TstType == "CH=>JP")
            {
                Res = genInterference.Gen(TstEntryThisTime.JP,4);
                Out.Title = TstEntryThisTime.CH;
                Out.RightSelet = TstEntryThisTime.JP;
            }
            if (Difficulty == "JustReplace" && TstType == "JP=>CH")
            {
                Res = genInterference.Gen(TstEntryThisTime.CH, 4);
                Out.Title = TstEntryThisTime.CH;
                Out.RightSelet = TstEntryThisTime.JP;
            }

            List<int> RandomList = zRandom.List(0, 3);

            Out.SelectionA = Res[RandomList[0]];
            Out.SelectionB = Res[RandomList[1]];
            Out.SelectionC = Res[RandomList[2]];
            Out.SelectionD = Res[RandomList[3]];

        }
        public bool Judging(string UserSelected)
        {
            bool Res = false;
            if (UserSelected == Out.RightSelet) Res = true;
            if(Res == true)
            {
                Json.EntryModifyInCache(TstEntryThisTime.PM.ID, 0, 1);
            }
            else
            {
                Json.EntryModifyInCache(TstEntryThisTime.PM.ID, 1, 1);
            }

            Json.WritJsonFileFrCache();

            return Res;
        }
        public void Exit()
        {
            Json.WritJsonFileFrCache();
        }
        public int Remain()
        {
            List<int> IndexList = new List<int>();

            if (Mode == "随机") IndexList = RandomIndexList;
            if (Mode == "顺序") IndexList = RegulrIndexList;
            return IndexList.Count;
        }

    }
    public class TranslateQuestion
    {
        public class OUT
        {
            public string Question = string.Empty;
            public string RightAnswer = string.Empty;
        }

        public OUT Out = new OUT();

        private ZJSON Json;
        private ZRandom zRandom = new ZRandom();
        private Regular regular = new Regular();
        private algorithm Algorithm = new algorithm();
        public string Mode = string.Empty;
        public string[] SupportedMode = { "顺序", "随机" };
        private List<Entry> Entries = new List<Entry>();
        private List<int> RegulrIndexList = new List<int>();
        private List<int> RandomIndexList = new List<int>();
        private Entry TstEntryThisTime;

        public TranslateQuestion(ref ZJSON Json, string Mode)
        {
            this.Mode = Mode;
            this.Json = Json;
        }

        public void ImportEntries(ref List<Entry> Entries, string Mode)
        {
            this.Entries.Clear();
            foreach (Entry entry in Entries) this.Entries.Add(entry);
            RandomIndexList = zRandom.List(0, Entries.Count - 1);
            RegulrIndexList = regular.GetOrderList(0, Entries.Count - 1);
        }

        private void SelectOneEntryToUse()
        {
            List<int> IndexList = new List<int>();

            if (Mode == "随机") IndexList = RandomIndexList;
            if (Mode == "顺序") IndexList = RegulrIndexList;

            if (IndexList.Count == 0)
            {
                MessageBox.Show("没有了");
                return;
            }
            int Index = IndexList[0];
            TstEntryThisTime = Entries[Index];
            IndexList.Remove(Index);
        }

        public void GenOneQuestion()
        {
            List<string> Res = new List<string>();
            SelectOneEntryToUse();
            Out.Question = TstEntryThisTime.JP;
            Out.RightAnswer = TstEntryThisTime.CH;
        }

        public double Judging(string UserSelected)
        {
            double Sim = Algorithm.Sim(Out.RightAnswer, UserSelected);
            Json.EntryModifyInCache(TstEntryThisTime.PM.ID, Sim);
            Json.WritJsonFileFrCache();
            return Sim;
        }

        public void Exit()
        {
            Json.WritJsonFileFrCache();
        }
    }
}
