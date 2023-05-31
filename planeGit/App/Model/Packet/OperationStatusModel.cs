using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class OperationStatusModel : PacketModel
{
    #region Inner Class

    [Serializable]
    public class PacketData
    {
        public string status;
        public string scenario;
        public string trainingCode;
        public string trainingTime;
        public string trainingName;
        public float missionSpeed;
        public string pilotUserID;
        public string pilotRole;
        public string towingCarUserID;
        public string towingCarRole;
        public string crew1UserID;
        public string crew1Role;
        public string crew2UserID;
        public string crew2Role;
        public string crew3UserID;
        public string crew3Role;
        public string crew4UserID;
        public string crew4Role;
    }

    #endregion

    public StringReactiveProperty Status { get; private set; } = new();
    public StringReactiveProperty Scenario { get; private set; } = new();
    public StringReactiveProperty TrainingCode { get; private set; } = new();
    public StringReactiveProperty TrainingTime { get; private set; } = new();
    public StringReactiveProperty TrainingName { get; private set; } = new();
    public FloatReactiveProperty MissionSpeed { get; private set; } = new();
    public StringReactiveProperty PilotUserID { get; private set; } = new();
    public StringReactiveProperty PilotRole { get; private set; } = new();
    public StringReactiveProperty TowingCarUserID { get; private set; } = new();
    public StringReactiveProperty TowingCarRole { get; private set; } = new();
    public StringReactiveProperty Crew1UserID { get; private set; } = new();
    public StringReactiveProperty Crew1Role { get; private set; } = new();
    public StringReactiveProperty Crew2UserID { get; private set; } = new();
    public StringReactiveProperty Crew2Role { get; private set; } = new();
    public StringReactiveProperty Crew3UserID { get; private set; } = new();
    public StringReactiveProperty Crew3Role { get; private set; } = new();
    public StringReactiveProperty Crew4UserID { get; private set; } = new();
    public StringReactiveProperty Crew4Role { get; private set; } = new();
    
    public override CommonPacketData CommonPacketData => new() { name = Define.PacketNameType.OPS.ToString() };

    public override object Data
    {
        get =>
            new PacketData()
            {
                status = Status.Value,
                scenario = Scenario.Value,
                trainingCode = TrainingCode.Value,
                trainingTime = TrainingTime.Value,
                trainingName = TrainingName.Value,
                missionSpeed = MissionSpeed.Value,
                pilotUserID = PilotUserID.Value,
                pilotRole = PilotRole.Value,
                towingCarUserID = TowingCarUserID.Value,
                towingCarRole = TowingCarRole.Value,
                crew1UserID = Crew1UserID.Value,
                crew1Role = Crew1Role.Value,
                crew2UserID = Crew2UserID.Value,
                crew2Role = Crew2Role.Value,
                crew3UserID = Crew3UserID.Value,
                crew3Role = Crew3Role.Value,
                crew4UserID = Crew4UserID.Value,
                crew4Role = Crew4Role.Value,
            };
        set
        {
            PacketData data = (PacketData)value;
            Status.Value = data.status;
            Scenario.Value = data.scenario;
            TrainingCode.Value = data.trainingCode;
            TrainingTime.Value = data.trainingTime;
            TrainingName.Value = data.trainingName;
            MissionSpeed.Value = data.missionSpeed;
            PilotUserID.Value = data.pilotUserID;
            PilotRole.Value = data.pilotRole;
            TowingCarUserID.Value = data.towingCarUserID;
            TowingCarRole.Value = data.towingCarRole;
            Crew1UserID.Value = data.crew1UserID;
            Crew1Role.Value = data.crew1Role;
            Crew2UserID.Value = data.crew2UserID;
            Crew2Role.Value = data.crew2Role;
            Crew3UserID.Value = data.crew3UserID;
            Crew3Role.Value = data.crew3Role;
            Crew4UserID.Value = data.crew4UserID;
            Crew4Role.Value = data.crew4Role;
        }
    }

    public override void Deserialize(JObject packet)
    {
        Data = packet["data"].ToObject<PacketData>();
    }
}