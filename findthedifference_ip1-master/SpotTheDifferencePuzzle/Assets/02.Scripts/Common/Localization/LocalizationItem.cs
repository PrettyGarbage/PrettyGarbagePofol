using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class LocalizationData
{
    public List<LocalizationItem> textItems;

    public LocalizationData()
    {
        textItems = new List<LocalizationItem>();
    }

    public bool IsExistKey(string key)
    {
        return textItems.Exists(i => i.key.Equals(key));
    }

    public bool AddData(string createKey, string createValueKr, string createValueEn, string createValueJp)
    {
        textItems.Add(new LocalizationItem(createKey.ToUpper(), createValueKr, createValueEn, createValueJp));
        return true;
    }

    public bool RemoveData(string key)
    {
        LocalizationItem localizationItem= textItems.Find(i => i.key.Equals(key));
        if(localizationItem != null)
        {
            textItems.Remove(localizationItem);
            return true;
        }
        else
        {
            return false;
        }
    }

    public LocalizationSimpleItemData GetLocalizationItemList(LangType langType)
    {
        LocalizationSimpleItemData localizationSimpleItemData = new LocalizationSimpleItemData();
        List<LocalizationSimpleItem> localizationSimpleItemList = new List<LocalizationSimpleItem>();

        for (int i = 0; i < textItems.Count; i++)
        {
            string value = string.Empty;
            switch (langType)
            {
                case LangType.KR:
                    value = textItems[i].valueKr;
                    break;
                case LangType.JP:
                    value = textItems[i].valueJp;
                    break;
                case LangType.EN:
                    value = textItems[i].valueEn;
                    break;
                default:
                    break;
            }
            localizationSimpleItemList.Add(new LocalizationSimpleItem(textItems[i].key, value));
        }
        localizationSimpleItemData.localizationSimpleItems = localizationSimpleItemList;
        return localizationSimpleItemData;
    }

    public string GetEnumStringFromKeys()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("public enum LocalizationTextKey {");
        for (int i = 0; i < textItems.Count; i++)
        {
            sb.AppendLine("     " + textItems[i].key.ToUpper() + " = " + i);
            if(i < textItems.Count - 1)
            {
                sb.Append(",");
            }
        }
        sb.Append("}");

        return sb.ToString();
    }

}

[System.Serializable]
public class LocalizationItem {

    public string key;
    public string valueKr;
    public string valueEn;
    public string valueJp;

    public LocalizationItem(string key, string valueKr, string valueEn, string valueJp)
    {
        this.key = key;
        this.valueKr = valueKr;
        this.valueEn = valueEn;
        this.valueJp = valueJp;
    }
}

[System.Serializable]
public class LocalizationSimpleItem
{
    public string key;
    public string value;
    
    public LocalizationSimpleItem(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}

[System.Serializable]
public class LocalizationSimpleItemData
{
    public List<LocalizationSimpleItem> localizationSimpleItems;

    public LocalizationSimpleItemData()
    {
        localizationSimpleItems = new List<LocalizationSimpleItem>();
    }
}
