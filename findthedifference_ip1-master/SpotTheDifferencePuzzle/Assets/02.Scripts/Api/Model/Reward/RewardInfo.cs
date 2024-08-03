using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class RewardInfo
{

    public string id;
    public string name;
    public List<ProductItemInfo> productItemList = new List<ProductItemInfo>();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(
            "\n id : " + id
            + "\n name : " + name);

        for (int i = 0; i < productItemList.Count; i++)
        {
            sb.AppendLine(productItemList[i].ToString());
        }

        return sb.ToString();
    }

}
