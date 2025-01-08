using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.Parts
{
    internal class FrameAndLength
    {
        public int Frame;
        public int Length;

        public FrameAndLength(int frame, int length)
        {
            Frame = frame;
            Length = length;
        }

        public void Update(int frame, int length)
        {
            Frame = frame;
            Length = length;
        }
    }
}
