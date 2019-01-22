using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCard : MonoBehaviour
{
    public bool IsFacingFront;

    public SpriteRenderer BackCover;
    public SpriteRenderer Emoji;
    public SpriteRenderer Title;

    // Use this for initialization
    void Start ()
    {
        UpdateFacingDirection();
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateFacingDirection();
    }

    public void SetFaceUp()
    {
        IsFacingFront = true;
    }

    public void SetFaceDown()
    {
        IsFacingFront = false;
    }

    private void UpdateFacingDirection()
    {
        if (IsFacingFront)
        {
            Emoji.enabled = true;
            Title.enabled = true;
            BackCover.enabled = false;
            return;
        }
        Emoji.enabled = false;
        Title.enabled = false;
        BackCover.enabled = true;
    }

    public abstract int GetEmotionCode();
}
