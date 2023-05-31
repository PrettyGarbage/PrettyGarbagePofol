using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class XMLCreater
{
    public string FilePath = "";
#if UNITY_EDITOR
    ///<summary>
    ///네트워크용 세팅 파일 제작
    ///</summary>
    [MenuItem("XML/CreateXML/NetworkFile")]
    public static void CreateNetworkXMLFile()
    {
        //=================================
        //      XML 파일 구조
        //=================================
        //．Root
        //  ㄴNetworkSetting
        //    ㄴ NetworkName : 네트워크 그룹명
        //    ㄴ IP : IP주소
        //    ㄴ PORT : 포트
        //
        //=================================
        XmlDocument networkXMLFile = new XmlDocument();
        networkXMLFile.AppendChild(networkXMLFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        XmlNode root = networkXMLFile.CreateNode(XmlNodeType.Element, "root", string.Empty);
        networkXMLFile.AppendChild(root);

        //============LIM : 로컬 IP추가=====
        var addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        var localIPStr = "";
        for(int addressIndex = 0; addressIndex < addressList.Length; addressIndex++)
        {
            if (addressList[addressIndex].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                localIPStr = addressList[addressIndex].ToString();
        }

        //===========
        //===========
        XmlElement networkSetting = networkXMLFile.CreateElement("networkSetting");
        root.AppendChild(networkSetting);

        XmlElement networkName = networkXMLFile.CreateElement("NetworkName");
        networkName.InnerText = Common.Define.Role.TCS.ToString();
        networkSetting.AppendChild(networkName);

        XmlElement networkIP = networkXMLFile.CreateElement("IP");
        networkIP.InnerText = "225.0.1.0";
        networkSetting.AppendChild(networkIP);

        XmlElement networkPort = networkXMLFile.CreateElement("PORT");
        networkPort.InnerText = "20211";
        networkSetting.AppendChild(networkPort);
        //TCS : Training Control System ( 훈련통제 시스템 )

        XmlElement localIP = networkXMLFile.CreateElement("LOCALIP");
        localIP.InnerText = localIPStr;
        networkSetting.AppendChild(localIP);
        //===========
        //cabin crew Secretary //옵저버
        //===========
        networkSetting = networkXMLFile.CreateElement("networkSetting");
        root.AppendChild(networkSetting);

        networkName = networkXMLFile.CreateElement("NetworkName");
        networkName.InnerText = Common.Define.Role.CCS.ToString();
        networkSetting.AppendChild(networkName);

        networkIP = networkXMLFile.CreateElement("IP");
        networkIP.InnerText = "227.0.1.0";
        networkSetting.AppendChild(networkIP);

        networkPort = networkXMLFile.CreateElement("PORT");
        networkPort.InnerText = "20231";
        networkSetting.AppendChild(networkPort);

        localIP = networkXMLFile.CreateElement("LOCALIP");
        localIP.InnerText = localIPStr;
        networkSetting.AppendChild(localIP);

        //===========
        //CC1 
        //===========
        networkSetting = networkXMLFile.CreateElement("networkSetting");
        root.AppendChild(networkSetting);

        networkName = networkXMLFile.CreateElement("NetworkName");
        networkName.InnerText = Common.Define.Role.CC1.ToString();
        networkSetting.AppendChild(networkName);

        networkIP = networkXMLFile.CreateElement("IP");
        networkIP.InnerText = "227.0.1.1";
        networkSetting.AppendChild(networkIP);

        networkPort = networkXMLFile.CreateElement("PORT");
        networkPort.InnerText = "20232";
        networkSetting.AppendChild(networkPort);

        localIP = networkXMLFile.CreateElement("LOCALIP");
        localIP.InnerText = localIPStr;
        networkSetting.AppendChild(localIP);

        //===========
        //CC2
        //===========
        networkSetting = networkXMLFile.CreateElement("networkSetting");
        root.AppendChild(networkSetting);

        networkName = networkXMLFile.CreateElement("NetworkName");
        networkName.InnerText = Common.Define.Role.CC2.ToString();
        networkSetting.AppendChild(networkName);

        networkIP = networkXMLFile.CreateElement("IP");
        networkIP.InnerText = "227.0.1.2";
        networkSetting.AppendChild(networkIP);

        networkPort = networkXMLFile.CreateElement("PORT");
        networkPort.InnerText = "20233";
        networkSetting.AppendChild(networkPort);

        localIP = networkXMLFile.CreateElement("LOCALIP");
        localIP.InnerText = localIPStr;
        networkSetting.AppendChild(localIP);

        //===========
        //CC3
        //===========
        networkSetting = networkXMLFile.CreateElement("networkSetting");
        root.AppendChild(networkSetting);

        networkName = networkXMLFile.CreateElement("NetworkName");
        networkName.InnerText = Common.Define.Role.CC3.ToString();
        networkSetting.AppendChild(networkName);

        networkIP = networkXMLFile.CreateElement("IP");
        networkIP.InnerText = "227.0.1.3";
        networkSetting.AppendChild(networkIP);

        networkPort = networkXMLFile.CreateElement("PORT");
        networkPort.InnerText = "20234";
        networkSetting.AppendChild(networkPort);

        localIP = networkXMLFile.CreateElement("LOCALIP");
        localIP.InnerText = localIPStr;
        networkSetting.AppendChild(localIP);

        //===========
        //CC4
        //===========
        networkSetting = networkXMLFile.CreateElement("networkSetting");
        root.AppendChild(networkSetting);

        networkName = networkXMLFile.CreateElement("NetworkName");
        networkName.InnerText = Common.Define.Role.CC4.ToString();
        networkSetting.AppendChild(networkName);

        networkIP = networkXMLFile.CreateElement("IP");
        networkIP.InnerText = "227.0.1.4";
        networkSetting.AppendChild(networkIP);

        networkPort = networkXMLFile.CreateElement("PORT");
        networkPort.InnerText = "20235";
        networkSetting.AppendChild(networkPort);

        localIP = networkXMLFile.CreateElement("LOCALIP");
        localIP.InnerText = localIPStr;
        networkSetting.AppendChild(localIP);
        networkXMLFile.Save(Application.streamingAssetsPath + "/XML/" + "Network.xml");
    }

    ///<summary>
    ///비상착수 질문지
    ///</summary>
    [MenuItem("XML/CreateXML/EmergencyWaterAnswer")]
    public static void CreateEmergencyWaterAnswerFile()
    {
        //=================================================
        //      XML 파일 구조
        //=================================================
        //．Root
        //  ㄴPlayerNo : 플레이어 No
        //    ㄴ AnswerItemList : 답변 No
        //      ㄴ No : 샤우팅 No
        //      ㄴ index : 샤우팅 Index
        //      ㄴ Body : 샤우팅 Body
        //      ㄴ Type : 0 - 샤우팅 1 - 대사
        //
        //=================================================
        XmlDocument emergencyWaterAnswer = new XmlDocument();
        emergencyWaterAnswer.AppendChild(emergencyWaterAnswer.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        XmlNode root = emergencyWaterAnswer.CreateNode(XmlNodeType.Element, "root", string.Empty);
        emergencyWaterAnswer.AppendChild(root);

        #region Player1
        XmlElement player1 = emergencyWaterAnswer.CreateElement("Player1");
        root.AppendChild(player1);

        #region 1 샤우팅 "괜찮습니다. 자리에 앉아주세요"
        XmlElement AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        XmlElement AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "1";
        AnswerItemList.AppendChild(AnswerNo);

        XmlElement AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        XmlElement AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다. 자리에 앉아주세요";
        AnswerItemList.AppendChild(AnswerBody);

        XmlElement AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 2 대사자막  엔진정지로 인해 앞으로 약 15분 후 제주 앞 바다에 비상착수할 예정입니다.

        //==================================================================
        //엔진정지로 인해 앞으로 약 15분 후 제주 앞 바다에 비상착수할 예정입니다.   
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "2";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = " 엔진정지로 인해 앞으로 약 15분 후 제주 앞 바다에 비상착수할 예정입니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //착수 1분전에는 벨트사인 4회를 주고, 비상탈출시에는 곧바로 방송하겠습니다.   
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "2";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "1";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = " 착수 1분전에는 벨트사인 4회를 주고, 비상탈출시에는 곧바로 방송하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);


        //==================================================================
        //각 담당구역별로 방송에 따라 비상착수 준비해 주시기 바랍니다.   
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "2";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "2";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = " 각 담당구역별로 방송에 따라 비상착수 준비해 주시기 바랍니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 3 대사자막 손님 여러분, 주목해 주십시오. 긴급사태가 발생했습니다.
        //==================================================================
        //손님 여러분, 주목해 주십시오. 긴급사태가 발생했습니다.  
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "손님 여러분, 주목해 주십시오. 긴급사태가 발생했습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);


        //==================================================================
        //손님 여러분, 주목해 주십시오. 긴급사태가 발생했습니다.  
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "1";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "이 비행기는 약 15분 후에 비상착수하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //저희 승무원들은 이러한 상황에 대비해 충분히 훈련을 받았습니다.  
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "2";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "저희 승무원들은 이러한 상황에 대비해 충분히 훈련을 받았습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //저희 승무원들은 이러한 상황에 대비해 충분히 훈련을 받았습니다.  
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "3";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "침착해 주시고 지금부터 객실승무원들의 지시에 따라주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 4 손님 여러분, 좌석벨트를 매시고 좌석등받이, 테이블, 모니터, 발 받침대를 모두 원위치 시켜주십시오.
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "4";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "손님 여러분, 좌석벨트를 매시고 좌석등받이, 테이블, 모니터, 발 받침대를 모두 원위치 시켜주십시오";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 5 지금부터 탈출차림을 점검하겠습니다.
        //==================================================================
        //지금부터 탈출차림을 점검하겠습니다.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "지금부터 탈출차림을 점검하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //징이 박힌 운동화, 뾰족한 구두, 필기구, 뾰족한 물건 등은 승무원에게 주십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "1";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "징이 박힌 운동화, 뾰족한 구두, 필기구, 뾰족한 물건 등은 승무원에게 주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //셔츠나 몸에 꼭 끼는 의류는 느슨하게 해주십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "2";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "셔츠나 몸에 꼭 끼는 의류는 느슨하게 해주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);


        //==================================================================
        //넥타이와 스카프는 풀어서 선반에 넣어주십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "3";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "넥타이와 스카프는 풀어서 선반에 넣어주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //안경은 벗어서 상의주머니나 양말에 넣어주십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "4";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안경은 벗어서 상의주머니나 양말에 넣어주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //좌석 앞 주머니에는 아무것도 넣지 마십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "5";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "좌석 앞 주머니에는 아무것도 넣지 마십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        #endregion

        #region 6 휴대수하물은 모두 선반속에 보관해 주십시오.
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "6";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "휴대수하물은 모두 선반속에 보관해 주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 7 여러분의 좌석 아래에 있는 구명복을 착용하십시오.
        //==================================================================
        //여러분의 좌석 아래에 있는 구명복을 착용하십시오.  
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "7";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "여러분의 좌석 아래에 있는 구명복을 착용하십시오. 부풀리지 마십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //구명복은 절대로 기내에서 부풀리지 마시고 탈출직전 부풀려 주십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "7";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "1";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "구명복은 절대로 기내에서 부풀리지 마시고 탈출직전 부풀려 주십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 8 지금부터 충격에 대비한 자세를 설명하겠습니다.
        //==================================================================
        //구명복은 절대로 기내에서 부풀리지 마시고 탈출직전 부풀려 주십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "지금부터 충격에 대비한 자세를 설명하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //양팔을 엇갈리게 하여 앞좌석 등받이 상단을 잡으세요.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "1";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "양팔을 엇갈리게 하여 앞좌석 등받이 상단을 잡으세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //엇갈린 양팔 위에 이마를 대세요.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "2";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "엇갈린 양팔 위에 이마를 대세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //양 발을 어깨너비로 벌려 약간 앞으로 내밀어 발바닥을 바닥에 힘껏 밀착시키십시오.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "3";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "양 발을 어깨너비로 벌려 약간 앞으로 내밀어 발바닥을 바닥에 힘껏 밀착시키십시오.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);

        //==================================================================
        //항공기가 완전히 멈출때까지 이 자세를 유지하세요.
        //==================================================================
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "4";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "항공기가 완전히 멈출때까지 이 자세를 유지하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 9 착수 약 1분 전에 발신될 충격방지 자세 실시 신호를 알리겠습니다.
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "9";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "착수 약 1분 전에 발신될 충격방지 자세 실시 신호를 알리겠습니다. ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 10 여러분이 탈출할 가까운 비상구 위치를 확인하십시오.
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "10";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "여러분이 탈출할 가까운 비상구 위치를 확인하십시오";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 11 여러분 좌석주머니의 안내지를 참고해주십시오
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "11";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "여러분 좌석주머니의 안내지를 참고해주십시오";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 12 승무원은 협조자를 선정해 주세요
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "12";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "승무원은 협조자를 선정해 주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 13 승무원은 협조자를 선정해 주세요
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "13";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "기장님, 사무장입니다. 비상탈출 준비 완료 되었습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 14 "충격방지자세! Brace"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "14";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "충격방지자세! Brace";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 15 "기장님, 객실뒷편에 물이 차고 있습니다.."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "15";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "기장님, 객실뒷편에 물이 차고 있습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 16 "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "16";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려! Release Seatbelts!Get your lifevest!Get Up! Get Out!Leave Everything!)";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 17 "Raft를 앞쪽 탈출구로 이동시켜주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "17";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "Raft를 앞쪽 탈출구로 이동시켜주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 18 "운반 중 실수로 팽창이  되지 않도록 주의하세요!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "18";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "운반 중 실수로 팽창이  되지 않도록 주의하세요!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 19 "Raft의 연결 끈을  항공기의 Door 주변 단단한 시설물에 묶어주세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "19";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "Raft의 연결 끈을  항공기의 Door 주변 단단한 시설물에 묶어주세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 20 "외부상황 확인! 바람,화재,지면높이 정상!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "20";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "외부상황 확인! 바람,화재,지면높이 정상!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 21 "도어모드 Armed 확인"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "21";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "도어모드 Armed 확인";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 22 "도어오픈"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "22";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "도어오픈";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 23 "Manual inflation handle 당겨"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "23";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "Manual inflation handle 당겨";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 24 "슬라이드 팽창 확인"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "24";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "슬라이드 팽창 확인";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 25 "Raft를 물위로 던져"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "25";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "Raft를 물위로 던져";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 26 "연결선 당겨"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "26";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "연결선 당겨";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 27 "탈출구정상!! 짐버려!! 이쪽으로!!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "27";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "탈출구정상!! 짐버려!! 이쪽으로!! Good Exit!Leave Everything! Come This Way!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 28 "구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "28";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 29 "잔류승객 없습니다. 모두 탈출했습니다"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "29";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "잔류승객 없습니다. 모두 탈출했습니다";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 30 "객실승무원 모두 탈출하세요!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player1.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "30";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "객실승무원 모두 탈출하세요!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #endregion

        #region Player2
        XmlElement player2 = emergencyWaterAnswer.CreateElement("Player2");
        root.AppendChild(player2);

        #region 1 샤우팅 "괜찮습니다. 자리에 앉아주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "1";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다. 자리에 앉아주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 2 샤우팅 "자리에 앉으시고 안전벨트를 착용하세요. "
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "2";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "자리에 앉으시고 안전벨트를 착용하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 3 대사자막 "네 알겠습니다."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "네 알겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 4 대사자막 "괜찮습니다"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "4";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 5 대사자막 "안전벨트 메세요. 등받이,테이블,모니터,발 받침대 모두 원위치 시키세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안전벨트 메세요. 등받이,테이블,모니터,발 받침대 모두 원위치 시키세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 6 대사자막 "위험한 물건들은 수거하겠습니다."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "6";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "위험한 물건들은 수거하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 7 샤우팅 모든 수하물은 선반속에 보관해주세요
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "7";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "모든 수하물은 선반속에 보관해주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 8 샤우팅 "좌석 아래에 있는 구명복을 착용하세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "좌석 아래에 있는 구명복을 착용하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 9 샤우팅 "발목을 잡거나 허벅지를 감싸 안으세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "9";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "발목을 잡거나 허벅지를 감싸 안으세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 10 샤우팅 "항공기가 멈출때까지 이 자세를 유지하세요.  "
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "10";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "항공기가 멈출때까지 이 자세를 유지하세요.  ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 11 대사자막 "승무원들을 도와 승객들의 탈출을 도와주세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "11";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "승무원들을 도와 승객들의 탈출을 도와주세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 12 샤우팅 "충격방지자세! Brace(반복)"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "12";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "충격방지자세! Brace(반복)";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 13 샤우팅 "기다리세요! Wait! "
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "13";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "기다리세요! Wait! ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 14 샤우팅 "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "14";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려! Release Seatbelts!Get your lifevest!Get Up! Get Out!Leave Everything!)  ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 15 "외부상황 확인! 바람,화재,지면높이 정상!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "15";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "외부상황 확인! 바람,화재,지면높이 정상!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 16 "도어모드 Armed 확인"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "16";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "도어모드 Armed 확인";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 17 "도어오픈"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "17";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "도어오픈";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 18 "Manual inflation handle 당겨"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "18";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "Manual inflation handle 당겨";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 19 "슬라이드 팽창 확인"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "19";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "슬라이드 팽창 확인";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 20 "Raft를 물위로 던져"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "20";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "Raft를 물위로 던져";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 21 "연결선 당겨"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "21";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "연결선 당겨";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 22 "탈출구정상!! 짐버려!! 이쪽으로!!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "22";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "탈출구정상!! 짐버려!! 이쪽으로!! Good Exit!Leave Everything! Come This Way!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 23 "구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "23";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 24 "잔류승객 없습니다. 모두 탈출했습니다"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player2.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "24";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerIndex = emergencyWaterAnswer.CreateElement("Index");
        AnswerIndex.InnerText = "0";
        AnswerItemList.AppendChild(AnswerIndex);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "잔류승객 없습니다. 모두 탈출했습니다";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #endregion

        #region Player3
        XmlElement player3 = emergencyWaterAnswer.CreateElement("Player3");
        root.AppendChild(player3);

        #region 1 샤우팅 "괜찮습니다. 자리에 앉아주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "1";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다. 자리에 앉아주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 2 샤우팅 "안전벨트를 착용하세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "2";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안전벨트를 착용하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 3 대사자막 "네 알겠습니다."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "네 알겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 4 대사자막 "괜찮습니다"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "4";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 5 샤우팅 "안전벨트 메세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안전벨트 메세요. 등받이,테이블,모니터,발 받침대 모두 원위치 시키세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 6 샤우팅 "위험한 물건들은 수거하겠습니다."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "6";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "위험한 물건들은 수거하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 7 샤우팅 "모든 수하물은 선반속에 보관해주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "7";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "모든 수하물은 선반속에 보관해주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 8 샤우팅 "좌석 아래에 있는 구명복을 착용해 주세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "좌석 아래에 있는 구명복을 착용해 주세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 9 샤우팅 "어린이 등뒤에 쿠션을 넣어서 벨트를 단단하게 줘여 주세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "9";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "어린이 등뒤에 쿠션을 넣어서 벨트를 단단하게 줘여 주세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 10 샤우팅 "안고 있는 유아는 보호자만 벨트 매시고 한 손으로 안아주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "10";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안고 있는 유아는 보호자만 벨트 매시고 한 손으로 안아주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 11 샤우팅 "항공기가 멈출때까지 이 자세를 유지하세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "11";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "항공기가 멈출때까지 이 자세를 유지하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 12 대사자막 "승무원들의 지시에 따라 창문탈출구를 개방하시고 승객들의 탈출을 도와주십시요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "12";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "승무원들의 지시에 따라 창문탈출구를 개방하시고 승객들의 탈출을 도와주십시요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 13 샤우팅  "충격방지자세! Brace"  (반복)
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "13";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "충격방지자세! Brace(반복)";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 14 샤우팅  "기다리세요! Wait!"  (반복)
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "14";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "기다리세요! Wait!(반복)";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 15 샤우팅  "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려! "
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "15";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려! Release Seatbelts!Get your lifevest!Get Up! Get Out!Leave Everything!)  ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 16 샤우팅 "R2 탈출구 불량"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "16";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "R2 탈출구 불량";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 17 샤우팅 "창가에 앉은 손님, 창밖을 보세요. 안전합니까?"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "17";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "창가에 앉은 손님, 창밖을 보세요. 안전합니까? 손잡이를 당기세요.Hey You!Look outside! Is it safe? Pull the handle!!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 18 샤우팅 "창가에 앉은 손님, 창밖을 보세요. 안전합니까?"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "18";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "바깥으로! 날개 앞으로 내려! 헤엄쳐 가서 잡아! Go Off The front Of The Wing / Swim To The Slide And Hold On!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 19 샤우팅 "창가에 앉은 손님, 창밖을 보세요. 안전합니까?"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "19";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아! Inflate Your Life VestJump Into The Water Swim To The Slide And Hold On";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 20 대사자막 "승무원들의 지시에 따라 창문탈출구를 개방하시고 승객들의 탈출을 도와주십시요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player3.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "20";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "잔류승객 없습니다.모두 탈출했습니다";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #endregion

        #region Player4
        XmlElement player4 = emergencyWaterAnswer.CreateElement("Player4");
        root.AppendChild(player4);

        #region 1 샤우팅 "괜찮습니다. 자리에 앉아주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "1";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다. 자리에 앉아주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 2 샤우팅 "안전벨트를 착용하세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "2";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안전벨트를 착용하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 3 대사자막 "안전벨트를 착용하세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "3";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "네 알겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 4 샤우팅 "괜찮습니다"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "4";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "괜찮습니다";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 5 샤우팅 "안전벨트 메세요. 등받이,테이블,모니터,발 받침대 모두 원위치 시키세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "5";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "안전벨트 메세요. 등받이,테이블,모니터,발 받침대 모두 원위치 시키세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 6 샤우팅 "위험한 물건들은 수거하겠습니다."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "6";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "위험한 물건들은 수거하겠습니다.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 7 샤우팅 "모든 수하물은 선반속에 보관해주세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "7";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "모든 수하물은 선반속에 보관해주세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 8 샤우팅 "보호자 먼저 구명복을 착용한후 아이에게 구명복을 착용시키세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "8";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "보호자 먼저 구명복을 착용한후 아이에게 구명복을 착용시키세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 9 샤우팅 "양팔을 엇갈리게 하여 앞좌석 등받이 상단을 잡으세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "9";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "양팔을 엇갈리게 하여 앞좌석 등받이 상단을 잡으세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 10 샤우팅 "엇갈린 양팔위에 이마를 대세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "10";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "엇갈린 양팔위에 이마를 대세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 11 샤우팅 "양발을 어깨너비로 벌리고 앞으로 내밀어 발다닥을 바닥에 힘껏 밀착시키세요"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "11";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "양발을 어깨너비로 벌리고 앞으로 내밀어 발다닥을 바닥에 힘껏 밀착시키세요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 12 샤우팅 "항공기가 멈출때까지 이 자세를 유지하세요."
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "12";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "항공기가 멈출때까지 이 자세를 유지하세요.";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 13 대사자막 "창문탈출구의 overheadbin에 있는 Life Raft을 두분씩 협력해서 L1, R1탈출구로 이동시켜주시고"
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "13";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "창문탈출구의 overheadbin에 있는 Life Raft을 두분씩 협력해서 L1, R1탈출구로 이동시켜주시고 승무원들의 지시에 따라 이동시켜주십시요";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 14 샤우팅  "충격방지자세! Brace(반복)"  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "14";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "충격방지자세! Brace(반복)";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 15 샤우팅  "기다리세요! Wait!"  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "15";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "기다리세요! Wait!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 16 샤우팅  "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려!"  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "16";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "벨트 풀어! 구명복입어! 일어나! 나와! 짐버려! Release Seatbelts!Get your lifevest!Get Up! Get Out!Leave Everything!)  ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 17 샤우팅  "L2 탈출구 불량 "  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "17";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "L2 탈출구 불량";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 18 샤우팅  "창가에 앉은 손님, 창밖을 보세요. 안전합니까? "  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "18";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "창가에 앉은 손님, 창밖을 보세요. 안전합니까? 손잡이를 당기세요! Hey You!Look outside! Is it safe? Pull the handle!!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 19 샤우팅  "바깥으로! 날개 앞으로 내려! 헤엄쳐 가서 잡아!"  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "19";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "바깥으로! 날개 앞으로 내려! 헤엄쳐 가서 잡아! Go Off The front Of The Wing / Swim To The Slide And Hold On!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 20 샤우팅  "바깥으로! 날개 앞으로 내려! 헤엄쳐 가서 잡아!"  
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "20";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "구명복 부풀려! 물로 뛰어들어! 헤엄쳐 가서 잡아!Inflate Your Life Vest!Jump Into The Water!Swim To The Slide And Hold On!";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "0";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #region 21 대사자막 "잔류승객 없습니다. 모두 탈출했습니다  "
        AnswerItemList = emergencyWaterAnswer.CreateElement("ShoutingList");
        player4.AppendChild(AnswerItemList);

        AnswerNo = emergencyWaterAnswer.CreateElement("No");
        AnswerNo.InnerText = "21";
        AnswerItemList.AppendChild(AnswerNo);

        AnswerBody = emergencyWaterAnswer.CreateElement("Body");
        AnswerBody.InnerText = "잔류승객 없습니다. 모두 탈출했습니다  ";
        AnswerItemList.AppendChild(AnswerBody);

        AnswerType = emergencyWaterAnswer.CreateElement("Type");
        AnswerType.InnerText = "1";
        AnswerItemList.AppendChild(AnswerType);
        #endregion

        #endregion

        emergencyWaterAnswer.Save(Application.streamingAssetsPath + "/XML/" + "EmergencyAnswer.xml");
    }

    [MenuItem("XML/CreateXML/Config")]
    public static void CreateGameConfigXMLFile()
    {
        //=================================
        //      XML 파일 구조
        //=================================
        //．Root
        //  PlayerSetting
        //    ㄴ ROLE : 플레이어 역할
        //    ㄴ ISHOST : 호스트or클라이언트
        //    ㄴ PLAYERTYPE : 플레이어or옵저버
        //
        //=================================
        XmlDocument networkXMLFile = new XmlDocument();
        networkXMLFile.AppendChild(networkXMLFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        XmlNode root = networkXMLFile.CreateNode(XmlNodeType.Element, "root", string.Empty);
        networkXMLFile.AppendChild(root);

        XmlElement networkSetting = networkXMLFile.CreateElement("PlayerSetting");
        root.AppendChild(networkSetting);

        XmlElement networkName = networkXMLFile.CreateElement("ROLE");
        networkName.InnerText = "0";
        networkSetting.AppendChild(networkName);

        XmlElement networkIP = networkXMLFile.CreateElement("ISHOST");
        networkIP.InnerText = "true";
        networkSetting.AppendChild(networkIP);

        XmlElement networkPort = networkXMLFile.CreateElement("ISVRON");
        networkPort.InnerText = "true";
        networkSetting.AppendChild(networkPort);

        networkXMLFile.Save(Application.streamingAssetsPath + "/XML/" + "config.xml");
    }

    [MenuItem("XML/CreateXML/Position")]
    public static void CreatePositionXMLFile()
    {
        //=================================
        //      XML 파일 구조
        //=================================
        //．Root
        //  <<SceneCode>>
        //    ㄴ IsSetEachPosition : 각각 포지션 세팅
        //    ㄴ IsDebug : 디버그 모드 ON
        //    ㄴ Position : 포지션
        //
        //=================================
        XmlDocument positionFile = new XmlDocument();
        positionFile.AppendChild(positionFile.CreateXmlDeclaration("1.0", "utf-8", "yes"));
        XmlNode root = positionFile.CreateNode(XmlNodeType.Element, "root", string.Empty);
        positionFile.AppendChild(root);

        XmlElement Position = positionFile.CreateElement("Position");
        root.AppendChild(Position);

        XmlElement SceneCode = positionFile.CreateElement("SceneCode");
        SceneCode.InnerText = "SCN004";
        Position.AppendChild(SceneCode);

        XmlElement IsSetEachPosition = positionFile.CreateElement("IsSetEachPosition");
        IsSetEachPosition.InnerText = "true";
        Position.AppendChild(IsSetEachPosition);

        XmlElement IsDebug = positionFile.CreateElement("IsDebug");
        IsDebug.InnerText = "false";
        Position.AppendChild(IsDebug);

        XmlElement PositionX = positionFile.CreateElement("PositionX");
        PositionX.InnerText = "0.0";
        Position.AppendChild(PositionX);

        XmlElement PositionY = positionFile.CreateElement("PositionY");
        PositionY.InnerText = "0.0";
        Position.AppendChild(PositionY);

        XmlElement PositionZ = positionFile.CreateElement("PositionZ");
        PositionZ.InnerText = "0.0";
        Position.AppendChild(PositionZ);

        positionFile.Save(Application.streamingAssetsPath + "/XML/" + "position.xml");
    }
#endif
}
