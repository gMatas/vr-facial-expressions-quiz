using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurprisedCard : BaseCard
{
    public static readonly int EMOTION_CODE = FaceEmotionsClassifier.EMOTION_SURPRISED;

    public override int GetEmotionCode()
    {
        return EMOTION_CODE;
    }
}
