using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryCard : BaseCard
{
    public static readonly int EMOTION_CODE = FaceEmotionsClassifier.EMOTION_ANGRY;

    public override int GetEmotionCode()
    {
        return EMOTION_CODE;
    }
}
