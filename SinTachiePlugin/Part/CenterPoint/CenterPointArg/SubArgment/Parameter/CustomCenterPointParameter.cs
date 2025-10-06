using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Armgent.CustomPoint;
using SinTachiePlugin.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter
{
    internal class CustomCenterPointParameter : CenterPointSubArgBase, ICustomPointParameter
    {
        [Display(Name = nameof(Texts.PartParam_CenterPoint_CustomPoint), ResourceType = typeof(Texts))]
        [TextEditor]
        public string CustomPoint { get => customPoint; set => Set(ref customPoint, value); }
        private string customPoint = string.Empty;

        public CustomCenterPointParameter()
        {
        }

        public CustomCenterPointParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CenterPointSubArgBase? origin)
        {
            if (origin is ICustomPointParameter customPointParameter)
                CustomPoint = customPointParameter.CustomPoint;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new CustomPointSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<CustomPointSharedData>() is CustomPointSharedData customPointSharedData)
                CustomPoint = customPointSharedData.CustomPoint;
        }
    }
}
