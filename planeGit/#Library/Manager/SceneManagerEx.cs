using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class SceneManagerEx
    {
        #region variable

        private List<UniTask> _loadingTasks = new List<UniTask>();

        #endregion
        
        #region property

        public Define.SceneStatus CurrentStatus = Define.SceneStatus.Unknown;

        #endregion
        
        #region life cycle

        ///<summary>
        ///초기화 시킬 이벤트들 등록
        ///</summary>
        public void Initialize()
        {
            
        }

        #endregion

        #region method
        
        ///<summary>
        ///순차적일 필요가 있을 때 씬 로드 방식
        ///</summary>
        ///<param name="sceneName"></param>
        public async UniTask LoadSceneAsyncSequentially(string sceneName)
        {
            //현재 씬 로딩상태로 변경
            CurrentStatus = Define.SceneStatus.Loading;
            //씬 로드
            await OnSceneAsync(sceneName);
            //기타 로드
            
            //씬 로드 끝!
            CurrentStatus = Define.SceneStatus.Done;
        }
        //오버로딩
        public async UniTask LoadSceneAsyncSequentially(int index)
        {
            CurrentStatus = Define.SceneStatus.Loading;
            await OnSceneAsync(index);
            CurrentStatus = Define.SceneStatus.Done;
        }

        public async UniTask LoadSceneAsync(string name)
        {
            //불러와야할게 더 있으면 , 추가
            await UniTask.WhenAll(OnSceneAsync(name));
        }
        
        public async UniTask LoadSceneAsync(int index)
        {
            await UniTask.WhenAll(OnSceneAsync(index));
        }

        private async UniTask OnSceneAsync(string sceneName) =>
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        private async UniTask OnSceneAsync(int index) =>
            await SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        #endregion

        #region reference

        //참고용 코드 프로그레스바를 구현하고 싶으면 참고해서 ㄱ
        //private async UniTask StartLoading ()
        //{
        //    List<UniTask> loadingTasks = new List<UniTask>();
        //    loadingTasks.Add(UniTask.Defer(OnSceneAsync));
        //
        //
        //    foreach (var task in _loadingTasks)
        //    {
        //        await task;
        //        //Task 로딩 완료
        //    } 
        //}

        #endregion
    }
}