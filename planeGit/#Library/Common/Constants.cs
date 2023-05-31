using System;
using UnityEngine;

namespace Common
{
    ///<summary>
    ///String 값 모아두는 클래스
    ///</summary>
    public static class Constants
    {
        #region Resource Path
        
        private const string PrefabUiPath = "UI/{0}";
        private const string PrefabCharPath = "Character/{0}";
        private const string PrefabModulePath = "Module/{0}";
        private const string PrefabPropPath = "Prop/{0}";

        #endregion

        #region Animation Parameter

        public static readonly int IsWalk = Animator.StringToHash("IsWalk");
        public static readonly int IsSit = Animator.StringToHash("IsSit");
        public static readonly int IsEquipBelt = Animator.StringToHash("IsEquipBelt");
        public static readonly int IsShock = Animator.StringToHash("IsShock");
        public static readonly int IsTable = Animator.StringToHash("IsTable");
        public static readonly int BeltOn = Animator.StringToHash("BeltOn");
        public static readonly int IsFoot = Animator.StringToHash("IsFoot");
        public static readonly int IsBack = Animator.StringToHash("IsBack");
        public static readonly int IsManTalk = Animator.StringToHash("IsManTalk");
        public static readonly int IsManTalkSecond = Animator.StringToHash("IsManTalkSecond");
        public static readonly int IsWomanTalk = Animator.StringToHash("IsWomanTalk");
        public static readonly int IsWomanTalkSecond = Animator.StringToHash("IsWomanTalkSecond");
        public static readonly int IsLookAround = Animator.StringToHash("IsLookAround");
        public static readonly int IsJumpCabinOpen = Animator.StringToHash("IsJumpCabinOpen");
        public static readonly int IsCarrierWalk = Animator.StringToHash("IsCarrierWalk");
        public static readonly int IsManSlide = Animator.StringToHash("IsManSlide");
        public static readonly int IsWomanSlide = Animator.StringToHash("IsWomanSlide");
        public static readonly int IsManLeftHelp = Animator.StringToHash("IsManLeftHelp");
        public static readonly int IsWomanLeftHelp = Animator.StringToHash("IsWomanLeftHelp");
        public static readonly int IsWomanRightHelp = Animator.StringToHash("IsWomanRightHelp");
        public static readonly int OpenoverwingWindow = Animator.StringToHash("OpenoverwingWindow");

        public static readonly int IsManLeftWindowView = Animator.StringToHash("IsManLeftWindowView");
        public static readonly int IsManRightWindowView = Animator.StringToHash("IsManRightWindowView");

        public static readonly int EquipBelt = Animator.StringToHash("EquipBelt");
        public static readonly int EquipOffBelt = Animator.StringToHash("EquipOffBelt");
        public static readonly int JumpSeat_Belt = Animator.StringToHash("JumpSeat_Belt");
        public static readonly int EquipTable = Animator.StringToHash("EquipTable");
        public static readonly int PutOffScarf = Animator.StringToHash("PutOffScarf");
        public static readonly int EquipSpikeBracelet = Animator.StringToHash("EquipSpikeBracelet");
        public static readonly int PutOffGlasses = Animator.StringToHash("PutOffGlasses");
        public static readonly int IdleState = Animator.StringToHash("IdleState");
        public static readonly int CloseFootrest = Animator.StringToHash("CloseFootrest");
        public static readonly int EquipLifeJacket = Animator.StringToHash("EquipLifeJacket");
        public static readonly int PutInOverheadbin = Animator.StringToHash("PutInOverheadbin");
        public static readonly int PointR1 = Animator.StringToHash("PointR1");
        public static readonly int PointR2 = Animator.StringToHash("PointR2");
        public static readonly int PointL1 = Animator.StringToHash("PointL1");
        public static readonly int PointL2 = Animator.StringToHash("PointL2");
        public static readonly int PointLeftFront = Animator.StringToHash("PointLeftFront");
        public static readonly int PointRightFront = Animator.StringToHash("PointRightFront");
        public static readonly int PointLeftBack = Animator.StringToHash("PointLeftBack");
        public static readonly int PointRightBack = Animator.StringToHash("PointRightBack");
        
        public static readonly int WomanChildLifeJacket = Animator.StringToHash("WomanChildLifeJacket");
        public static readonly int WomanChildSeatBelt = Animator.StringToHash("WomanChildSeatBelt");
        
        public static readonly int BreathOnRaftFront = Animator.StringToHash("BreathOnRaftFront");
        public static readonly int BreathOnRaftBack = Animator.StringToHash("BreathOnRaftBack");
        public static readonly int IsWalkOnRaftBack = Animator.StringToHash("IsWalkOnRaftBack");
        public static readonly int IsWalkOnRaftFront = Animator.StringToHash("IsWalkOnRaftFront");
        public static readonly int ConnectAssistHandle = Animator.StringToHash("ConnectAssistHandle");
        public static readonly int ConnectAssistHandleV2 = Animator.StringToHash("ConnectAssistHandleV2");
        public static readonly int ManThrowRaftOnSea = Animator.StringToHash("ManThrowRaftOnSea");
        public static readonly int ManThrowRaftOnSeaV2 = Animator.StringToHash("ManThrowRaftOnSeaV2");
        public static readonly int HangRaft = Animator.StringToHash("HangRaft");
        public static readonly int ThrowRaft = Animator.StringToHash("ThrowRaft");
        
        public static readonly int JumpOcean = Animator.StringToHash("JumpOcean");
        public static readonly int ManSwim = Animator.StringToHash("ManSwim");
        public static readonly int ManRaftClimb = Animator.StringToHash("ManRaftClimb");
        public static readonly int ManRaftSit = Animator.StringToHash("ManRaftSit");
        
        
        public static readonly int jumpSeatOpen = Animator.StringToHash("jumpSeatOpen");
        public static readonly int jumpSeatClose = Animator.StringToHash("jumpSeatClose");
        
        public static readonly int IsEquipFABelt = Animator.StringToHash("IsEquipFABelt");
        public static readonly int DoorCoverOpen = Animator.StringToHash("DoorCoverOpen");
        public static readonly int DoorMoveManual = Animator.StringToHash("DoorMoveManual");
        public static readonly int DoorCoverClose = Animator.StringToHash("DoorCoverClose");
        public static readonly int DoorOpen = Animator.StringToHash("DoorOpen");
        public static readonly int OverwingOpen = Animator.StringToHash("OverwingOpen");
        
        public static readonly int SlideOpen = Animator.StringToHash("SlideOpen");
        public static readonly int InflationHandlePull = Animator.StringToHash("InflationHandlePull");
        
        public static readonly int OpenCabinetL = Animator.StringToHash("OpenCabinetL");
        public static readonly int OpenCabinetR = Animator.StringToHash("OpenCabinetR");
        
        

        #endregion

        #region Animation Name
        
        public static readonly string WomanPutInOverheadbin = "Woman_Put_In_Overheadbin";
        public static readonly string ManPutInOverheadbin = "Man_Put_In_Overheadbin";
        public static readonly string ManCarryRaftWalkFront = "Man_Carry_Raft_Walk_Front";
        public static readonly string ManCarryRaftWalkBack = "Man_Carry_Raft_Walk_Back";
        public static readonly string Door_Operation_Handle_Door_Open = "Door_Operation_Handle_Door_Open";
        public static readonly string Slide_Spread_it_out = "Slide_Spread_it_out";
        public static readonly string Door_Mode_Cover_Open = "Door_Mode_Cover_Open";
        public static readonly string Door_Mode_Move_Manual = "Door_Mode_Move_Manual";
        public static readonly string Door_Mode_Cover_Close = "Door_Mode_Cover_Close";
        public static readonly string InflationHandle = "InflationHandle";
        public static readonly string Point_Left_Front = "Point_Left_Front";
        public static readonly string Point_Right_Front = "Point_Right_Front";
        public static readonly string Point_Left_Back = "Point_Left_Back";
        public static readonly string Point_Right_Back = "Point_Right_Back";
        public static readonly string Man_Slide = "Man_Slide";
        public static readonly string Woman_Slide = "Woman_Slide";
        public static readonly string overwingWindowExitAll_02 = "overwingWindowExitAll_02";
        public static readonly string LookPamphlet = "LookPamphlet";
        public static readonly string Woman_WindowExit = "Woman_WindowExit";
        public static readonly string Man_Carry_Raft_Hang_v02 = "Man_Carry_Raft_Hang_v02";
        public static readonly string Man_Carry_Raft_Hang_v01 = "Man_Carry_Raft_Hang_v01";
        public static readonly string Man_JumpOcean = "Man_JumpOcean";
        
        public static readonly string SpikeBracelet = "SpikeBracelet";

        #endregion

        #region String Format
        public static string PrefabUI(string path) => string.Format(PrefabUiPath, path);
        public static string PrefabChar(string path) => string.Format(PrefabCharPath, path);
        public static string PrefabProp(string path) => string.Format(PrefabPropPath, path);
        public static string PrefabModule(string path) => string.Format(PrefabModulePath, path);
        
        #endregion

        #region ScenarioDictionary Data

        public const string DataModelPath = "/DataModel/";
        public const string FileScenarioData = "ScenarioData.csv";
        
        public const string ScnB737 = "SCN011";
        public const string ScnB777 = "SCN012";
        public const string ScnA330 = "SCN013";
        public const string ScnOverwing = "SCN014";
        public const string Scn1XR = "SCN016";

        public const string ScnEmergencyWaterCode = "SCN004";

        #endregion

        #region Resource Name

        public const string VrPluginPath = "Player";

        #endregion

        #region Layer Name

        public const string RayInteractionLayer = "RaycastInteraction";

        #endregion
    }
}