using System.Collections.Generic;
using System.Linq;

public class ScenarioModel : AppContext<ScenarioModel>
{
    #region variable

    Dictionary<string, string> dictionary = new();
    #endregion

    #region Unity LifeCycle

    protected override void Awake()
    {
        base.Awake();
        
        if(Instance != this) return;
        InitData();
    }

    #endregion

    #region Public Method

    public void InitData()
    {
        dictionary = CSVManager.Instance.LoadScenarioData();
    }
    
    public string GetScenarioName(string code)
    {
        return dictionary
            .Single(key => key.Key.Equals(code))
            .Value;
    }

    public string GetScenarioCode(string value)
    {
        return dictionary
            .FirstOrDefault(val => val.Value.Equals(value))
            .Key;
    } 
    
    #endregion
}