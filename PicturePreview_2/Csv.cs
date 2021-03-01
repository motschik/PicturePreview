using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicturePreview_2
{
    class Mono
    {
        public List<List<int>> Data { set; get; }

        public int Height
        {
            get { return Data.Count; }
        }
        public int Width
        {
            get { return Data.Max(x => x.Count); }
        }

        public Mono()
        {
            Data = new List<List<int>>();
        }

        public int At(int x,int y)
        {
            return Data[y][x];
        }

        public override string ToString()
        {
            string str = "";
            foreach(var row in Data)
            {
                foreach(var col in row)
                {
                    str += col.ToString() + " ";
                }
                str += "\n";
            }
            return str;
        }
    }
}
