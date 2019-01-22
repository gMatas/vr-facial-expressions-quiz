using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyCard : BaseCard
{
    public static readonly int EMOTION_CODE = FaceEmotionsClassifier.EMOTION_HAPPY;

    public override int GetEmotionCode()
    {
        return EMOTION_CODE;
    }
}
