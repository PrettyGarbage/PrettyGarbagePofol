namespace Common
{
    public static class Define
    {
        public enum ScenarioMode
        {
            VR,
            XR,
            Single
        }

        ///<summary>
        ///AppFlow의 상태 값
        ///</summary>
        public enum AppFlowState
        {
            ParseXML,
            Setting,
            Main
        }
        
        ///<summary>
        ///Scene Enum 값
        ///</summary>
        public enum Scene
        {
            Unknown = 0,
            Lobby = 1,
        }

        ///<summary>
        ///씬 상태 값
        ///</summary>
        public enum SceneStatus
        {
            Unknown = 0,
            Loading,
            Done,
        }
        
        public enum Role
        {
            Unknown = -1,
            CCS = 0,
            CC1,
            CC2,
            CC3,
            CC4,
            TCS
        }
        
        public enum Status
        {
            Unknown,
            Initial,
            TraineeReady,
            Started,
            Playing,
            Pause,
            Stop,
            ScenarioEnd,
            TrainingFinish,
        }
        
        public enum EventStatus
        {
            Unknown,
            Playing,
            End
        }
        
        public enum Action
        {
            Unknown,
        }
        
        public enum ScenarioState
        {
            None,
            Mission,
            Complete
        }

        public enum SubtitleType
        {
            Guide, //지시
            Shouting, //자막
            Description, //상황
        }
        
        /// <summary>
        /// 플레이어 역할 
        /// </summary>
        public enum CrewType
        {
            CrewA,
            CrewB,
            CrewC,
            CrewD
        }

        #region 네트워크
        

       //패킷 명
       public enum PacketNameType
       {
           UNKNOWN,
           OPS,
           ADS,
           AOS,
           US,
           SEC,
           CTR,
           ATR,
           UPS,
       }

       public enum OPSStatus
       {
           Unknown,
           SystemTest,
           SystemReady,
           TrainingReady,
           TrainingStart,
           TrainingPlay,
           TrainingPause,
           TrainingStop,
           TrainingFinish,
           SystemStop,
           ReviewStart,
           ReviewPause,
           ReviewStop
       }

        #endregion
    }
}