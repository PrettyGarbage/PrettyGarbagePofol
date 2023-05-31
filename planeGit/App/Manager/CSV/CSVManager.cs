using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Framework.Common.Template.SceneLoader;
using UnityEngine;

public class CSVManager : AppContext<CSVManager>
{
    private CSVReader _csvReader = new CSVReader();

    public Dictionary<string,string> LoadScenarioData()
    {
        return ReadScenarioData();
    }
    
    Dictionary<string, string> ReadScenarioData()
    {
        try
        {
            var readCsvResult = _csvReader.ReadCSV(Application.streamingAssetsPath + Constants.DataModelPath, Constants.FileScenarioData);

            return readCsvResult.ToDictionary(strArr => strArr[0], strArr => strArr[1]);
        }
        catch (Exception e)
        {
            throw (new Exception("CSV :: 데이터를 로드하는 도중 에러가 발생했습니다 + " + e.Message));
        }
    }
}
