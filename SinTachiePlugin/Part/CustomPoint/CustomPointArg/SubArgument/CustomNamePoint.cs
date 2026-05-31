using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument
{
    internal class CustomNamePoint
    {
        [Display(Name = nameof(TextResource.CustomPointParam_Name), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string Name { get; set; }

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_X), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F1", "px", -500, 500)]
        [Range(-10000, 10000)]
        public double X { get; set; }

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Y), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F1", "px", -500, 500)]
        [Range(-10000, 10000)]
        public double Y { get; set; }

        public CustomNamePoint(string name, double x, double y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public CustomNamePoint(CustomNamePoint origin)
        {
            Name = origin.Name;
            X = origin.X;
            Y = origin.Y;
        }
    }
}
