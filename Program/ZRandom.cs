using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZJYC
{
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
}
