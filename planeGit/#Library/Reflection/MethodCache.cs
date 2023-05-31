using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Library.Util.Reflection
{
    // Type을 키 값으로 MethodInfo를 캐싱하는 클래스
    public static class MethodCache
    {
        //Type을 키 값으로 MethodInfo를 캐싱하는 딕셔너리
        static Dictionary<Type, Dictionary<string, MethodInfo>> cache = new();
        
        //Type과 Method 이름을 받아서 MethodInfo를 반환하는 함수
        public static MethodInfo GetMethodInfo(Type type, string methodFullName)
        {
            CacheMethodInfo(type);

            //Method 이름을 키 값으로 캐싱된 MethodInfo를 반환
            return cache[type][methodFullName];
        }
        
        //Type과 Method 이름을 받아서 MethodInfo를 반환하는 함수
        public static MethodInfo GetMethodInfo<T>(string methodName) => GetMethodInfo(typeof(T), methodName);
        
        //Type을 받아서 Type의 모든 MethodInfo를 반환하는 메서드
        public static ReadOnlyCollection<MethodInfo> GetMethodInfos(Type type)
        {
            CacheMethodInfo(type);

            //캐싱된 딕셔너리의 모든 MethodInfo를 반환
            return new ReadOnlyCollection<MethodInfo>(cache[type].Values.ToList());
        }
        
        //Type을 받아서 Type의 모든 MethodInfo를 반환하는 메서드
        public static ReadOnlyCollection<MethodInfo> GetMethodInfos<T>() => GetMethodInfos(typeof(T));
        
        static void CacheMethodInfo(Type type)
        {
            //Type이 캐싱된 딕셔너리에 없다면
            if (!cache.ContainsKey(type))
            {
                //Type을 키 값으로 새로운 딕셔너리를 생성
                cache.Add(type, new Dictionary<string, MethodInfo>());

                //Type을 키 값으로 캐싱된 딕셔너리를 가져옴
                Dictionary<string, MethodInfo> methodInfoDic = cache[type];

                //Type의 모든 MethodInfo를 가져옴
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                //Type의 모든 MethodInfo를 캐싱된 딕셔너리에 추가
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    var parameters = String.Join(",", methodInfo.GetParameters().Select(parameter => parameter.ParameterType.Name));
                    methodInfoDic.Add($"{methodInfo.Name}({parameters})", methodInfo);
                }
            }
        }
    }
}
