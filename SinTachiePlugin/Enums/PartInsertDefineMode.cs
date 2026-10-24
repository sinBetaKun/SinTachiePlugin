using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    internal enum PartInsertDefineMode
    {
        [Display(Name = nameof(TextResource.PartInsertDefineMode_DontInsert), ResourceType = typeof(TextResource))]
        DontInsert,
        [Display(Name = nameof(TextResource.PartInsertDefineMode_SelectTagAndPosition), ResourceType = typeof(TextResource))]
        SelectTagAndPosition,
        [Display(Name = nameof(TextResource.PartInsertDefineMode_ToParentAndPosition), ResourceType = typeof(TextResource))]
        ToParentAndPosition,
        [Display(Name = nameof(TextResource.GlobalEnumDisplay_DontOverride), ResourceType = typeof(TextResource))]
        DontOverride,
    }
}
