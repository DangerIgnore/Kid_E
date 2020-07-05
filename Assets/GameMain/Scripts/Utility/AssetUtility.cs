using System;
using GameFramework;

public class AssetUtility
{
    public static string GetDataTableAsset(string assetName, LoadType loadType)
    {
        return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, loadType == LoadType.Text ? "txt" : "bytes");
    }

    public static string GetMusicAsset(string assetName)
    {
        return Utility.Text.Format("Assets/GameMain/Music/{0}.mp3", assetName);
    }
}
