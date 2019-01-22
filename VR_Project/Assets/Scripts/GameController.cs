using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CircleTimerController Timer;
    public CardsGridController CardsGrid;
    public CVSystemController CvSystem;

    public bool CardSelectionEnabled { get; private set; }

    public float startingTimeDuration = 2f;
    private float timestamp = 0f;

    private bool isRunning = false;

	// Use this for initialization
	void Start ()
    {
        DisableCardSelection();
    }

    private bool unshuffled = true;

	// Update is called once per frame
	void Update ()
    {
        if (unshuffled)
        {
            unshuffled = false;
            CardsGrid.PlaceShuffledGameCards();
        }

        if (!isRunning)
        {
            CardsGrid.SetAllCardsFaceUp();
            if (Time.time > timestamp + startingTimeDuration)
            {
                isRunning = true;
                CardsGrid.SetAllCardsFaceDown();
                EnableCardSelection();
            }
        }
	}

    public void EnableCardSelection()
    {
        CardSelectionEnabled = true;
    }

    public void DisableCardSelection()
    {
        CardSelectionEnabled = false;
    }

    public void Match()
    {

    }
}
