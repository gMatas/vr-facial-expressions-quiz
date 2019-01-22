using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public BaseCard Card { get; private set; }
    public GameController GameHub;

    public float CardVievingTimeDuration;
    public float CardClickTimerDuration;

    private bool cardIsSelected;

    private float currenttime;
    private float timestamp;


	// Use this for initialization
	void Start ()
    {
        BaseCard[] cards = GetComponents<BaseCard>();
        if (cards.Length == 0) throw new System.ArgumentNullException("Please add ONLY one BaseCard script object to this game object.");
        if (cards.Length > 1) throw new System.ArgumentException("There can be ONLY one BaseCard script object attached to this game object.");
        Card = cards[0];
    }

    // Update is called once per frame
    void Update ()
    {
        if (!cardIsSelected && !Card.IsFacingFront)
            return;

        if (GameHub.Timer.InProgress)
        {
            // TODO: Get that shit in here, right here... here there
            return;
        }

        if (!Card.IsFacingFront)
        {
            GameHub.Timer.StopTimer();

            Dictionary<int, int> emotions = GameHub.CvSystem.GetEmotions();
            GameHub.CvSystem.StopCvPipeline();
            bool match = CompareEmotions(emotions);
            if (match)
            {
                GameHub.Match();
                print("Emotion: " + Card.GetEmotionCode().ToString() + " Has matched!!");
            }

            timestamp = Time.time;
            Card.SetFaceUp();
            cardIsSelected = false;
            return;
        }

        if (Time.time >= timestamp + CardVievingTimeDuration)
        {
            Card.SetFaceDown();
            GameHub.EnableCardSelection();
        }
	}

    public void GameCardFaceUp()
    {
        Card.SetFaceUp();
    }

    public void GameCardFaceDown()
    {
        Card.SetFaceDown();
    }

    private void OnMouseDown()
    {
        if (!GameHub.CardSelectionEnabled)
            return;

        Debug.Log("Clicked on:" + Card.GetType().ToString());
        if (!GameHub.Timer.InProgress)
        {
            cardIsSelected = true;
            GameHub.Timer.StartTimer(CardClickTimerDuration);
            GameHub.DisableCardSelection();
            GameHub.CvSystem.StartCvPipline();
        }
    }

    private bool CompareEmotions(Dictionary<int, int> emotions)
    {
        if (!emotions.ContainsKey(Card.GetEmotionCode()))
            return false;

        int score = emotions[Card.GetEmotionCode()];
        foreach (var pair in emotions)
            if (pair.Key != Card.GetEmotionCode())
                if (pair.Value > score)
                    return false;
        return true;
    }
}
