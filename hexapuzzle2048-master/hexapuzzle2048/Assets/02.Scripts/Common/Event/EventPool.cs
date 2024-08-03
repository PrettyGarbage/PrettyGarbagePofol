using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;


/// <summary>
/// 글로벌 이벤트 Pool 관리.
/// </summary>
public class EventPool
{
    class EventInfo
    {
        public EventInfo(EventNames eventName, ActionParams action)
        {
            this.eventName = eventName;
            this.action = action;
        }
        public EventNames eventName { get; set; }
        public ActionParams action { get; set; }
    }

    private readonly static Dictionary<EventNames, ActionParams> listenerDic = new Dictionary<EventNames, ActionParams>();
    private readonly static Dictionary<int, List<EventInfo>> EventInfoDic = new Dictionary<int, List<EventInfo>>();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Listen(int instanceId, EventNames eventName, ActionParams action)
    {
        //Debug.Log("Listen instanceId : " + instanceId + " / eventName : " + eventName);
        ActionParams actions;
        listenerDic.TryGetValue(eventName, out actions);

        if (actions != null)
        {
            listenerDic[eventName] = actions + action;
        }
        else
        {
            listenerDic[eventName] = action;
        }

        List<EventInfo> eventInfoList;
        if (EventInfoDic.TryGetValue(instanceId, out eventInfoList) == false)
        {
            eventInfoList = new List<EventInfo>();
            eventInfoList.Add(new EventInfo(eventName, action));
            EventInfoDic[instanceId] = eventInfoList;
        }
        else
        {
            eventInfoList.Add(new EventInfo(eventName, action));
        }
        //Debug.Log("Listen EventInfo " + eventInfoList.Count);
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void Remove(int instanceId, EventNames eventName, ActionParams action)
    {
        //Debug.Log("Remove instanceId : " + instanceId + " / eventName : " + eventName);
        ActionParams actions;
        listenerDic.TryGetValue(eventName, out actions);

        if (actions != null)
        {
            listenerDic[eventName] = actions - action;
        }

        List<EventInfo> eventInfoList;
        if (EventInfoDic.TryGetValue(instanceId, out eventInfoList))
        {
            for (int i = 0; i < eventInfoList.Count; i++)
            {
                if (eventInfoList[i].eventName == eventName && eventInfoList[i].action == action)
                {
                    eventInfoList.RemoveAt(i);
                }
            }
            //Debug.Log("Remove EventInfo " + eventInfoList.Count);

        }
    }

    public static void RemoveAll(int instanceId)
    {
        List<EventInfo> eventInfoList;
        if (EventInfoDic.TryGetValue(instanceId, out eventInfoList))
        {
            for (int i = 0; i < eventInfoList.Count; i++)
            {
                EventNames eventName = eventInfoList[i].eventName;
                var actions = listenerDic[eventName] as ActionParams;
                if (actions != null)
                {
                    listenerDic[eventName] = actions - eventInfoList[i].action;
                }
            }

            EventInfoDic.Remove(instanceId);
            eventInfoList.Clear();
            eventInfoList = null;
        }
    }

    public static void Send(EventNames eventName, params object[] args)
    {
        //Debug.Log("eventName : " + eventName);

        ActionParams actions;
        listenerDic.TryGetValue(eventName, out actions);
        if (actions != null)
        {
            actions(args);
        }
    }

}