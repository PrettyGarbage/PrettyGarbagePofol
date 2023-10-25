#if ADDRESSABLE_SUPPORT
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Mine.Code.Framework.Helper
{
    public enum AddressableState
    {
        None,
        Idle,
        Initialized,
        HasUpdate,
        NotingToUpdate,
        BundleDownloaded,
        SizeDownloaded,
        Downloading,
    }

    public struct AddressableStateData
    {
        public AddressableState State;
        public long TotalSize;
        public long RemainSize;

        public AddressableStateData(AddressableState state, long totalSize, long remainSize)
        {
            State = state;
            TotalSize = totalSize;
            RemainSize = remainSize;
        }
    }
    
    public class AddressableHelper
    {
        #region Static

        public static string DownloadURL = "192.168.0.92:80";

        #endregion

        #region Fields

        private readonly Subject<AddressableStateData> _onAddressableStateSubject = new();
        private bool _isDownloading;

        #endregion

        #region IObservable Properties
        
        public IObservable<AddressableStateData> OnAddressableState => _onAddressableStateSubject;

        #endregion

        public async UniTask Initialize() => await Addressables.InitializeAsync().ToUniTask()
            .ContinueWith(OnInitializeCompleted);
        
        
        
        public async UniTask CheckForCatalogUpdates() => await Addressables.CheckForCatalogUpdates().ToUniTask()
            .ContinueWith(OnCheckForCatalogUpdated);


        public async UniTask DownloadSize(string label) => await Addressables.GetDownloadSizeAsync(label).ToUniTask()
            .ContinueWith(OnSizeCheckCompleted);

        /// <summary>
        /// DownloadDependenciesAsync는 파라미터가 object라서 타입을 추론할 수 없어서 묶을 수가 없음.
        /// </summary>
        /// <param name="label"></param>
        public async UniTask StartDownload(string label)
        {
            if(_isDownloading) return;

            _isDownloading = true;

            var handle = Addressables.DownloadDependenciesAsync(label);
            handle.Completed -= OnDependenciesDownloaded;
            handle.Completed += OnDependenciesDownloaded;

            while (!handle.IsDone)
            {
                //다운로드 상태
                var status = handle.GetDownloadStatus();
                var totalSize = status.TotalBytes;
                var remainSize = status.TotalBytes - status.DownloadedBytes;

                var data = new AddressableStateData(AddressableState.Downloading, totalSize, remainSize);

                _onAddressableStateSubject.OnNext(data);

                await UniTask.Yield();
            }

            _isDownloading = false;
        }

        public void Dispose() => _onAddressableStateSubject.Dispose();
        

        #region private methods

        //초기화 완료시 Subject에 OnNext
        private async UniTask OnInitializeCompleted(IResourceLocator operation)
        {
            await UniTask.CompletedTask;
            
            _onAddressableStateSubject.OnNext(new AddressableStateData(AddressableState.Initialized, 
                -1, 
                -1));
        }

        //카탈로그 유효성 체크
        private async UniTask OnCheckForCatalogUpdated(List<string> result)
        {
            if (result.Count > 0)
            {
                await Addressables.UpdateCatalogs(result).ToUniTask().ContinueWith(OnUpdateCatalog);
            }
            else
            {
                _onAddressableStateSubject.OnNext(new AddressableStateData(AddressableState.NotingToUpdate, 
                    -1,
                    -1));
            }
            
            await UniTask.CompletedTask;
        }
        
        //카탈로그 업데이트시 Subject에 OnNext
        private async UniTask OnUpdateCatalog(List<IResourceLocator> result)
        {                
            _onAddressableStateSubject.OnNext(new AddressableStateData(AddressableState.HasUpdate
                , -1
                , -1));
            
            await UniTask.CompletedTask;
        }

        //사이즈 다운로드시 Subject에 OnNext
        private async UniTask OnSizeCheckCompleted(long totalSize)
        {
            _onAddressableStateSubject.OnNext(new AddressableStateData(AddressableState.SizeDownloaded
                ,totalSize 
                ,totalSize));

            await UniTask.CompletedTask;
        }

        //번들 다운로드 완료시 Subject에 OnNext
        private void OnDependenciesDownloaded(AsyncOperationHandle result)
        {
            _onAddressableStateSubject.OnNext(new AddressableStateData(AddressableState.BundleDownloaded
                , -1
                , -1));
        }
        
        //예외 발생시 호출
        private void OnException(AsyncOperationHandle handle, Exception exception)
        {
            _onAddressableStateSubject.OnError(exception);
        }
        #endregion
    }
}
#endif