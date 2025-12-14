namespace SinTachiePlugin.Enums
{
    [Obsolete]
    public enum BlendSTP
    {
        SourceOver,         // 普通
        Dissolve,           // ディザ合成
        Darken,             // 比較（暗）
        Multiply,           // 乗算
        ColorBurn,          // 焼きこみカラー
        LinearBurn,         // 焼きこみ（リニア）
        Lighten,            // 比較（明）
        Screen,             // スクリーン
        ColorDodge,         // 覆い焼きカラー
        LinearDodge,        // 覆い焼き（リニア）カラー
        Plus,               // 加算
        Overlay,            // オーバーレイ
        SoftLight,          // ソフトライト
        HardLight,          // ハードライト
        VividLight,         // ビビッドライト
        LinearLight,        // リニアライト
        PinLight,           // ピンライト
        HardMix,            // ハードミックス
        Difference,         // 差分
        Exclusion,          // 除外
        Subtract,           // 減算
        Division,           // 除算
        Hue,                // 色相
        Saturation,         // 彩度
        Color,              // カラー
        Luminosity,         // 輝度
        LighterColor,       // カラー比較（明）
        DestinationOver,    // 背景
        DarkerColor,        // カラー比較（暗）
        DestinationOut,     // 削除
        SourceAtop,         // 背景でクリッピング
        XOR,                // 重ならない部分のみ
        MaskInverseErt,     // 色反転マスク
    }
}
