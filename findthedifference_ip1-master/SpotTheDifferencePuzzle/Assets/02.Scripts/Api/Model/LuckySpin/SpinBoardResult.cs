using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class SpinBoardResult
{
    public string spinBoardId;
    public List<ProductItemInfo> productItemList;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < productItemList.Count; i++)
        {
            sb.AppendLine(productItemList[i].ToString());
        }
        sb.AppendLine("spinBoardId : " + spinBoardId);
            
        return sb.ToString();
    }

}
