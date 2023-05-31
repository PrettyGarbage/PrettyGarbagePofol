using Framework.Common.Template.SceneLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SubscribeData : IEqualityComparer<SubscribeData>
{
    public string info;
    public string message;

    public SubscribeData(string info, string message)
    {
        this.info = info;
        this.message = message;
    }

    public bool Equals(SubscribeData x, SubscribeData y)
    {
        return x.message == y.message;
    }

    public int GetHashCode(SubscribeData obj)
    {
        return obj.message.GetHashCode();
    }
}

public static class GetSubscribeData
{
    public static void Parse()
    {
        string path = $"Assets/##MyAssets/Dialogue/{SceneLoader.Instance.CurrentScene}";
        StreamWriter sw;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }


        Debug.Log($"작성 시작 : {path}");
        WriteText(path, false);
        //WriteText(path, true);
        Debug.Log($"작성 완료 : {path}");
    }

    private static void WriteText(string path, bool isFilter)
    {
        string fullPath = $"{path}/{SceneLoader.Instance.CurrentScene}";

        StreamWriter sw;
        List<SubscribeData> dialogueList = new List<SubscribeData>();

        if (!File.Exists(fullPath))
        {
            var file = File.CreateText($"{fullPath}.txt");
            file.Close();
        }
        sw = new StreamWriter(fullPath + ".txt");

        ScenarioSystem.Instance.CurrentScenario.Missions.ForEach(scenarioEvent =>
        {
            if (scenarioEvent == null) return;

            WriteProduction(scenarioEvent, isFilter, dialogueList);
            WriteMission(isFilter, scenarioEvent, dialogueList);
        });

        dialogueList.Select(x =>
        {
            x.message = x.message.Trim();
            return x;
        }).Select(x =>
        {
            x.message = x.message.Trim('\n');
            return x;
        }).Distinct().ForEach((x, index) =>
        {
            sw.WriteLine("--------------------");
            sw.WriteLine($"{x.info}_{(index + 1) :000} : {x.message}");
        });

        sw.Flush();
        sw.Close();
    }

    private static void WriteProduction(ScenarioEvent scenarioEvent, bool isFilter, List<SubscribeData> dialogueList)
    {
        if (scenarioEvent.Production == null) return;
        if (scenarioEvent.Production.Dialogues == null) return;

        scenarioEvent.Production.Dialogues.ForEach(dialogue =>
        {
            if (dialogue == null) return;
            if (dialogue.Body == null) return;

            dialogue.Body.ForEach((body, index) =>
            {
                if (isFilter && body.Clip && dialogue.Type != Common.Define.SubtitleType.Shouting) return;

                var info = scenarioEvent.EventCode.Substring(3);
                info = info.Substring(0, info.Length - 4);

                dialogueList.Add(new (info, body.Message));
            });
        });
    }

    private static void WriteMission(bool isFilter, ScenarioEvent scenarioEvent, List<SubscribeData> dialogueList)
    {
        scenarioEvent.Missions.ForEach(mission =>
        {
            if (mission == null) return;
            if (mission.Dialogues == null) return;
            mission.Dialogues.ForEach(dialogue =>
            {
                if (dialogue == null) return;
                if (dialogue.Body == null) return;

                dialogue.Body.ForEach((body, index) =>
                {
                    if (isFilter && body.Clip && dialogue.Type != Common.Define.SubtitleType.Shouting) return;

                    var info = scenarioEvent.EventCode.Substring(3);
                    info = info.Substring(0, info.Length - 4);

                    dialogueList.Add(new (info, body.Message));
                });
            });
        });
    }
}
