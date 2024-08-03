using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class ShopInfo
{
    public string id;
    public int seq;
    public string productId;
    public string name;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(
            "\n id : " + id
            + "\n seq : " + seq
            + "\n productId : " + productId
            + "\n name : " + name            
            );

        return sb.ToString();
    }
}