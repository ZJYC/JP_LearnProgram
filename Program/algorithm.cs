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
using ZJYC_JSON;

namespace Algorithm
{
    class algorithm
    {
        class SimRes
        {
            public string Inf;
            public double Sim;
        }
        public double Sim(string txt1, string txt2)
        {
            List<char> sl1 = txt1.ToCharArray().ToList();
            List<char> sl2 = txt2.ToCharArray().ToList();
            //去重
            List<char> sl = sl1.Union(sl2).ToList<char>();

            //获取重复次数
            List<int> arrA = new List<int>();
            List<int> arrB = new List<int>();
            foreach (var str in sl)
            {
                arrA.Add(sl1.Where(x => x == str).Count());
                arrB.Add(sl2.Where(x => x == str).Count());
            }
            //计算商
            double num = 0;
            //被除数
            double numA = 0;
            double numB = 0;
            for (int i = 0; i < sl.Count; i++)
            {
                num += arrA[i] * arrB[i];
                numA += Math.Pow(arrA[i], 2);
                numB += Math.Pow(arrB[i], 2);
            }
            double cos = num / (Math.Sqrt(numA) * Math.Sqrt(numB));
            return cos;
        }

        public int Random(int begin,int end)
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(begin, end + 1);
        }

        private int SortList(SimRes a, SimRes b)
        {
            if (a.Sim > b.Sim)
            {
                return -1;
            }
            else if (a.Sim < b.Sim)
            {
                return 1;
            }
            return 0;
        }

        public List<string> FindSimiliarTopCHX(List<Entry> Entries,string Basic,int X)
        {
            List<SimRes> Res = new List<SimRes>();

            foreach(Entry entry in Entries)
            {
                Res.Add(new SimRes { Inf = entry.CH, Sim = Sim(Basic, entry.CH) });
            }

            Res.Sort(SortList);

            List<string> Ret = new List<string>();
            int i = 0;

            foreach(SimRes res in Res)
            {
                if (Ret.Contains(res.Inf) == false)
                {
                    Ret.Add(res.Inf);
                    if (++i >= X) break;
                }
            }

            return Ret;
        }

        public List<string> FindSimiliarTopJPX(List<Entry> Entries, string Basic, int X)
        {
            List<SimRes> Res = new List<SimRes>();

            foreach (Entry entry in Entries)
            {
                Res.Add(new SimRes { Inf = entry.JP, Sim = Sim(Basic, entry.JP) });
            }

            Res.Sort(SortList);

            List<string> Ret = new List<string>();
            int i = 0;

            foreach (SimRes res in Res)
            {
                if (Ret.Contains(res.Inf) == false)
                {
                    Ret.Add(res.Inf);
                    if (++i >= X) break;
                }
            }

            return Ret;
        }

        public bool IsSelectRight(ref List<Entry> Entries, string JP,string CH)
        {
            foreach(Entry entry in Entries)
            {
                if(entry.CH == CH && entry.JP == JP)
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> RandomLoad(List<string> Src)
        {
            List<string> DST = new List<string>();
            List<string> SRC = new List<string>();
            SRC.AddRange(Src);
            DST.AddRange(Src);
            for (int i = 0; i < DST.Count; i++)
            {
                int Index = Random(0, SRC.Count - 1);
                DST[i] = SRC[Index];
                SRC.Remove(SRC[Index]);
            }
            return DST;
        }

    }
}
