using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralCard : BaseCard
{
    public static readonly int EMOTION_CODE = FaceEmotionsClassifier.EMOTION_NEUTRAL;

    public override int GetEmotionCode()
    {
        return EMOTION_CODE;
    }
}
