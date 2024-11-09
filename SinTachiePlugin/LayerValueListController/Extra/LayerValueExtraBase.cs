using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.LayerValueListController.Extra
{
    public abstract class LayerValueExtraBase : SharedParameterBase
    {
        public LayerValueExtraBase()
        {
        }

        public LayerValueExtraBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract double GetValue(long frame, long length, int fps);
        public abstract void CopyFrom(LayerValueExtraBase? origin);
    }
}
