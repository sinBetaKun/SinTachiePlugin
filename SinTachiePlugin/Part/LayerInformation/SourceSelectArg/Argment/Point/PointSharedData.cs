using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Point
{
    internal class PointSharedData
    {
        public string Point { get; set; } = string.Empty;

        public PointSharedData()
        {
        }

        public PointSharedData(IPointParameter parameter)
        {
            Point = parameter.Point;
        }

        public void CopyTo(IPointParameter parameter)
        {
            parameter.Point = Point;
        }
    }
}
