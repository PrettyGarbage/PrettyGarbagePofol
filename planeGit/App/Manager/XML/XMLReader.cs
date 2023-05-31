using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class XMLReader
{
    ///<summary>
    ///네트워크 설정 XML파일을 불러온다
    ///</summary>
    ///<returns></returns>
    public List<NetworkModel.NetworkSetting> ReadNetworkXML()
    {
        Logger.Log("네트워크 :: XML파일을 읽어옵니다");
        List<NetworkModel.NetworkSetting> result = new List<NetworkModel.NetworkSetting>();
        try
        {
            var networkRootNode = ReadXML("Network.xml");
            var networkSettingNodes = networkRootNode.SelectNodes("networkSetting");
            foreach (XmlNode networkSettingNode in networkSettingNodes)
            {
                var networkSetting = new NetworkModel.NetworkSetting();
                networkSetting.networkName = networkSettingNode.SelectSingleNode("NetworkName").InnerText;
                networkSetting.ip = networkSettingNode.SelectSingleNode("IP").InnerText;
                networkSetting.port = networkSettingNode.SelectSingleNode("PORT").InnerText;
                networkSetting.localIP = networkSettingNode.SelectSingleNode("LOCALIP").InnerText;
                result.Add(networkSetting);
            }
        }
        catch(Exception e)
        {
            throw ( new Exception("네트워크 XML 파일 읽는 도중에 에러가 났습니다 + " + e.Message));
        }
        return result;
    }

    ///<summary>
    ///Config파일 로드
    ///</summary>
    ///<returns></returns>
    public ConfigModel.PlayerSetting ReadConfigFile()
    {
        Logger.Log("네트워크 :: XML파일을 읽어옵니다");
        ConfigModel.PlayerSetting result = new ConfigModel.PlayerSetting();
        try
        {
            var configFile = ReadXML("config.xml");
            var configSettingNode = configFile.SelectSingleNode("PlayerSetting");
            result.role = int.Parse(configSettingNode.SelectSingleNode("ROLE").InnerText);
            result.observerIP = configSettingNode.SelectSingleNode("OBSERVERIP").InnerText;
            result.multicastIP = configSettingNode.SelectSingleNode("MULTICASTIP").InnerText;
            result.debugMode = bool.Parse(configSettingNode.SelectSingleNode("DEBUGMODE").InnerText);
        }
        catch (Exception e)
        {
            Logger.Log("네트워크 XML 파일 읽는 도중에 에러가 났습니다 + " + e.Message);
        }
        return result;
    }


    ///<summary>
    ///Positionsetting파일 로드
    ///</summary>
    ///<returns></returns>
    public List<PositionModel.PositionSetting> ReadPositionFile()
    {
        Logger.Log("포지션 세팅 :: XML파일을 읽어옵니다");
        List<PositionModel.PositionSetting> result = new List<PositionModel.PositionSetting>();
        try
        {
            var positionRootNode = ReadXML("position.xml");
            var positionkSettingNodes = positionRootNode.SelectNodes("Position");
            foreach (XmlNode node in positionkSettingNodes)
            {
                var positionSetting = new PositionModel.PositionSetting();
                positionSetting.sceneCode = node.SelectSingleNode("SceneCode").InnerText;
                positionSetting.isSetEachPosition = bool.Parse(node.SelectSingleNode("IsSetEachPosition").InnerText);
                positionSetting.isDebug = bool.Parse(node.SelectSingleNode("IsDebug").InnerText);

                var x = float.Parse(node.SelectSingleNode("PositionX").InnerText);
                var y = float.Parse(node.SelectSingleNode("PositionY").InnerText);
                var z = float.Parse(node.SelectSingleNode("PositionZ").InnerText);

                positionSetting.position = new Vector3(x, y, z);
                result.Add(positionSetting);
            }
        }
        catch (Exception e)
        {
            throw (new Exception("네트워크 XML 파일 읽는 도중에 에러가 났습니다 + " + e.Message));
        }
        return result;
    }

    ///<summary>
    ///XML파일 리드
    ///</summary>
    private XmlNode ReadXML(string fileName)
    {
        XmlDocument xMLFile = new XmlDocument();
        XmlNode root;
        string folderPath = Application.streamingAssetsPath + "/XML/";
        var filePath = Directory.GetFiles(folderPath, fileName);
        using (FileStream fileStream = File.OpenRead(filePath[0]))
        {
            xMLFile.Load(fileStream);
            root = xMLFile.SelectSingleNode("root");
        }
        Logger.Log("XML :: 비상착수 선택버튼용 XML 파일을 로드에 성공하였습니다!");
        return root;
    }
}