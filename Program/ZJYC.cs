using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJYC
{

    class Regular
    {
        public List<int> GetOrderList(int Min,int Max)
        {
            List<int> Res = new List<int>();
            for (int i = Min; i <= Max; i++) Res.Add(i);
            return Res;
        }
    }

    class ZRandom
    {
        public double Random(double Min,double Max)
        {
            int Tem = new Random(Guid.NewGuid().GetHashCode()).Next();
            double Res = Tem * 1.0 / int.MaxValue;
            return Res * (Max - Min) + Min;
        }
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <returns></returns>
        public int Random()
        {
            int Res = new Random(Guid.NewGuid().GetHashCode()).Next();
            return Res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Min">最小</param>
        /// <param name="Max">最大</param>
        /// <returns></returns>
        public int Random(int Min, int Max)
        {
            int Res = new Random(Guid.NewGuid().GetHashCode()).Next(Min, Max + 1);
            return Res;
        }
        /// <summary>
        /// 生成可重复序列
        /// </summary>
        /// <param name="Start">起始</param>
        /// <param name="End">结束</param>
        /// <param name="Count">总个数</param>
        /// <returns></returns>
        public List<int> List(int Start, int End, int Count)
        {
            List<int> Res = new List<int>();

            for (int i = 0; i < Count; i++)
            {
                Res.Add(Random(Start, End));
            }

            return Res;
        }
        /// <summary>
        /// 生成不可重复序列
        /// </summary>
        /// <param name="Start">起始</param>
        /// <param name="End">结束</param>
        /// <returns></returns>
        public List<int> List(int Start, int End)
        {
            List<int> Tem = new List<int>();
            List<int> Res = new List<int>();
            for (int i = 0; i < End - Start + 1; i++)
            {
                Tem.Add(Start + i);
            }

            for (int i = 0; i < End - Start + 1; i++)
            {
                int DstIndex = Random(0, Tem.Count - 1);
                Res.Insert(i, Tem[DstIndex]);
                Tem.Remove(Tem[DstIndex]);
            }


            return Res;
        }
        /// <summary>
        /// 生成不可重复序列
        /// </summary>
        /// <param name="InputList">输入序列</param>
        /// <returns></returns>
        public List<int> List(List<int> Tem)
        {
            List<int> Res = new List<int>();
            int Count = Tem.Count;
            for (int i = 0; i < Count; i++)
            {
                int DstIndex = Random(0, Tem.Count - 1);
                Res.Insert(i, Tem[DstIndex]);
                Tem.Remove(Tem[DstIndex]);
            }
            return Res;
        }

    }

    class GenInterference
    {
        class ExchangeItem
        {
            //之前
            public string Sen = string.Empty;
            //之后
            public string Rai = string.Empty;
            //概率
            public double Ritu = 0.5;

            public ExchangeItem(string Sen, string Rai, double Ritu)
            {
                this.Sen = Sen;
                this.Rai = Rai;
                this.Ritu = Ritu;
            }
        }

        List<ExchangeItem> Items = new List<ExchangeItem>()
        {
            new ExchangeItem("た","だ",0.20),
            new ExchangeItem("だ","た",0.20),
            new ExchangeItem("て","で",0.20),
            new ExchangeItem("で","て",0.20),
            new ExchangeItem("が","か",0.20),
            new ExchangeItem("か","が",0.20),
            new ExchangeItem("い",""  ,0.10),
            new ExchangeItem("う",""  ,0.25),
            new ExchangeItem("や","ゃ",0.50),
            new ExchangeItem("ゆ","ゅ",0.50),
            new ExchangeItem("よ","ょ",0.50),
            new ExchangeItem("つ","っ",0.25),
            new ExchangeItem("つ",""  ,0.20),
        };

        private string ExchangeByRate(string Input)
        {
            ZRandom random = new ZRandom();

            foreach (ExchangeItem Item in Items)
            {
                //如果包含并且随机概率符合要求就替换
                if (Input.Contains(Item.Sen) && random.Random(0.0, 1.0) < Item.Ritu)
                {
                    Input = Input.Replace(Item.Sen, Item.Rai);
                }
            }

            return Input;
        }

        public List<string> Gen(string Input, int Count)
        {
            List<string> Res = new List<string>();

            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < Count; i++)
                {
                    string Temp = Gen(Input);
                    if (Res.Contains(Temp) == false) Res.Add(Temp);
                }
                if (Res.Count >= Count) break;
            }
            //没有生成足够的项目就复制原字符串充数
            for (int i = Res.Count; i < Count; i++)
            {
                Res.Add(Input);
            }
            return Res;

        }

        public string Gen(string Input)
        {
            string Res = ExchangeByRate(Input);
            return Res;
        }
    }
}
