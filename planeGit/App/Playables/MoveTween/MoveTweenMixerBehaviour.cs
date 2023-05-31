using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class MoveTweenMixerBehaviour : PlayableBehaviour
{
    bool m_FirstFrameHappened;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;

        if (trackBinding == null)
            return;

        Vector3 defaultPosition = trackBinding.position;
        Quaternion defaultRotation = trackBinding.rotation;

        int inputCount = playable.GetInputCount();

        float positionTotalWeight = 0f;
        float rotationTotalWeight = 0f;

        Vector3 blendedPosition = Vector3.zero;
        Quaternion blendedRotation = new(0f, 0f, 0f, 0f);

        for (int i = 0; i < inputCount; i++)
        {
            ScriptPlayable<MoveTweenBehaviour> playableInput = (ScriptPlayable<MoveTweenBehaviour>)playable.GetInput(i);
            MoveTweenBehaviour input = playableInput.GetBehaviour();

            if (input.model == null || input.model.Path == null || input.model.Path.Length == 0)
                continue;

            Transform[] path = input.model.Path;
            if (input.isReverse) path = path.Reverse().ToArray();

            float inputWeight = playable.GetInputWeight(i);

            if (!m_FirstFrameHappened && !path.First())
            {
                input.startingPosition = defaultPosition;
            }

            float normalisedTime = (float)(playableInput.GetTime() / playableInput.GetDuration());

            if (input.tweenPosition)
            {
                positionTotalWeight += inputWeight;

                blendedPosition += MoveTweenHelper.GetPosition(path, normalisedTime) * inputWeight;
            }

            if (input.tweenRotation)
            {
                rotationTotalWeight += inputWeight;

                Quaternion desiredRotation = Quaternion.LookRotation(MoveTweenHelper.GetDirection(path, normalisedTime));

                if (Quaternion.Dot(blendedRotation, desiredRotation) < 0f) desiredRotation = ScaleQuaternion(desiredRotation, -1f);

                desiredRotation = NormalizeQuaternion(desiredRotation);
                desiredRotation = ScaleQuaternion(desiredRotation, inputWeight);

                blendedRotation = AddQuaternions(blendedRotation, desiredRotation);
            }
        }

        MoveTweenBehaviour lastInput = null;
        double endTime = 0;
        for (int j = 0; j < inputCount; j++)
        {
            ScriptPlayable<MoveTweenBehaviour> tempPlayableInput = (ScriptPlayable<MoveTweenBehaviour>)playable.GetInput(j);
            MoveTweenBehaviour tempInput = tempPlayableInput.GetBehaviour();
            if (tempInput.clip.end <= playable.GetTime() && endTime < tempInput.clip.end)
            {
                endTime = tempInput.clip.end;
                lastInput = tempInput;
            }
        }
        
        blendedPosition += defaultPosition * (1f - positionTotalWeight);
        if (lastInput is { LookAtLastDirection: true } && positionTotalWeight == 0f)
        {
            var path = lastInput.model.Path;
            if(lastInput.isReverse) path = path.Reverse().ToArray();
            if (path.Any())
            {
                blendedRotation = path.Last().rotation;
                blendedRotation = NormalizeQuaternion(blendedRotation);
            }
        }
        else
        {
            Quaternion weightedDefaultRotation = ScaleQuaternion(defaultRotation, 1f - rotationTotalWeight);
            blendedRotation = AddQuaternions(blendedRotation, weightedDefaultRotation);
        }
        
        trackBinding.position = blendedPosition;
        trackBinding.rotation = blendedRotation;

        m_FirstFrameHappened = true;
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        m_FirstFrameHappened = false;
    }

    static Quaternion AddQuaternions(Quaternion first, Quaternion second)
    {
        first.w += second.w;
        first.x += second.x;
        first.y += second.y;
        first.z += second.z;
        return first;
    }

    static Quaternion ScaleQuaternion(Quaternion rotation, float multiplier)
    {
        rotation.w *= multiplier;
        rotation.x *= multiplier;
        rotation.y *= multiplier;
        rotation.z *= multiplier;
        return rotation;
    }

    static float QuaternionMagnitude(Quaternion rotation)
    {
        return Mathf.Sqrt((Quaternion.Dot(rotation, rotation)));
    }

    static Quaternion NormalizeQuaternion(Quaternion rotation)
    {
        float magnitude = QuaternionMagnitude(rotation);

        if (magnitude > 0f)
            return ScaleQuaternion(rotation, 1f / magnitude);

        Debug.LogWarning("Cannot normalize a quaternion with zero magnitude.");
        return Quaternion.identity;
    }
}