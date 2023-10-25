#if ADDRESSABLE_SUPPORT
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;
using Object = UnityEngine.Object;

namespace Mine.Framework.Manager.Addressable.Util
{
   public enum SizeUnit
   {
      Byte, KB, MB, GB
   }
   
   public static class AddressableUtility
   {
      #region public
      
      public static bool IsNetworkValid() => Application.internetReachability != NetworkReachability.NotReachable;
      public static bool IsDiskSpaceValid(long size) => size <= Caching.defaultCache.spaceFree;
      
      
      public static string GetSizeToString(long size)
      {
         var sizeString = GetSizeUnit(size) switch
         {
            SizeUnit.Byte => $"{size} Byte",
            SizeUnit.KB => $"{size / OneKB} KB",
            SizeUnit.MB => $"{size / OneMB} MB",
            SizeUnit.GB => $"{size / OneGB} GB",
            _ => string.Empty
         };
         return sizeString;
      }
      
      // asset List 에서 key 에 해당하는 asset 의 목록을 구한다.
      // List<IResourceLocator>는 Addressables.UpdateCatalogs()의 결과물이다.
      public static IList<IResourceLocation> ConvertResourceLocations(this List<IResourceLocator> source,string key,Type type)
      {
         var result = new List<IResourceLocation>();
         foreach (var locator in source)
         {
            IList<IResourceLocation> downloadList = new List<IResourceLocation>();
            locator.Locate((object)key,type,out downloadList);
            if(!downloadList.IsNullOrEmpty())result.AddRange(downloadList);
         }
         return result;   
      }
      
      // asset List 에서 key 에 해당하는 asset 의 목록을 구한다.
      public static async UniTask<IList<T>> GetAssetList<T>(string key, Action<T> action = null) where T: Object
      {
         var asyncOperation = Addressables.LoadAssetsAsync<T>((object)key, action);
         await asyncOperation;
         return asyncOperation.Result;
      }
      
      public static void SetImageSprite(this Image targetImage, Sprite sprite) => targetImage.sprite = sprite;
      public static void SetImageSprite(this Image targetImage, string key) => Addressables.LoadAssetAsync<Sprite>(key).Completed += handle => targetImage.sprite = handle.Result;

      #endregion


      #region private
      
      private static SizeUnit GetSizeUnit(long size)
      {
         if (size >= OneGB) return SizeUnit.GB;
         if (size >= OneMB) return SizeUnit.MB;
         return size >= OneKB ? SizeUnit.KB : SizeUnit.Byte;
      }
      private static long OneGB => 1073741824L;
      private static long OneMB => 1048576L;
      private static long OneKB => 1024L;

      #endregion
      
   }
   
}
#endif