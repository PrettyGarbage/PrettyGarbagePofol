using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public static class MoveTweenHelper
{
    // path와 progress를 넘기면 해당 위치를 반환
    public static Vector3 GetPosition (Transform[] path, float progress)
    {
        if (path == null || path.Length == 0)
            return Vector3.zero;

        if (path.Length == 1)
            return path[0].position;

        float totalLength = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            totalLength += Vector3.Distance (path[i].position, path[i + 1].position);
        }

        float currentLength = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance (path[i].position, path[i + 1].position);
            float normalizedLength = segmentLength / totalLength;
            if (progress <= currentLength + normalizedLength)
            {
                float segmentProgress = (progress - currentLength) / normalizedLength;
                return Vector3.Lerp (path[i].position, path[i + 1].position, segmentProgress);
            }
            currentLength += normalizedLength;
        }

        return path[path.Length - 1].position;
    }
    
    // path와 progress를 넘기면 이동 방향 반환
    public static Vector3 GetDirection (Transform[] path, float progress)
    {
        if (path == null || path.Length < 2)
            return Vector3.zero;

        float totalLength = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            totalLength += Vector3.Distance (path[i].position, path[i + 1].position);
        }

        float currentLength = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance (path[i].position, path[i + 1].position);
            float normalizedLength = segmentLength / totalLength;
            if (progress < currentLength + normalizedLength)
            {
                return path[i + 1].position - path[i].position;
            }
            currentLength += normalizedLength;
        }

        return path[^1].position - path[^2].position;
    }
}
