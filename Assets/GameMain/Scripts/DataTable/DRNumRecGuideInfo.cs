
using System.IO;
using GameFramework;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;

public class DRNumRecGuideInfo : IDataRow
{
    public int Id { get; private set; }
    public NumRecGuideInfo NumRecGuideInfoSet { get; private set; }

    public bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
    {
        string[] columnTexts = dataRowSegment.Source.Substring(dataRowSegment.Offset, dataRowSegment.Length).Split('\t');
        for(int i = 0; i < columnTexts.Length; i++)
        {
            columnTexts[i] = columnTexts[i].Trim('\t');//把前后的制表符去掉
        }

        //略过#注释列
        int index = 1;
        try
        {
            Id = int.Parse(columnTexts[index++]);

            NumRecGuideInfoSet = new NumRecGuideInfo();

            NumRecGuideInfoSet.StepId = int.Parse(columnTexts[index++]);
            NumRecGuideInfoSet.StepDescription = columnTexts[index++];
            NumRecGuideInfoSet.StepCount = int.Parse(columnTexts[index++]);
            NumRecGuideInfoSet.StepMusic = columnTexts[index++];
            NumRecGuideInfoSet.MusicTime = int.Parse(columnTexts[index++]);
            NumRecGuideInfoSet.BubbleTip = columnTexts[index++];

            return true;
        }
        catch (System.Exception ex)
        {
            Log.Error("DRNumRecGuideInfo:" + Id.ToString() + "--" + ex.Message);

            return false;
        }
    }

    public bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
    {
        return false;
    }

    public bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
