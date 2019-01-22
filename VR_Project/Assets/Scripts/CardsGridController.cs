using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsGridController : MonoBehaviour
{
    public GameObject[] GameCards;
    [Space]
    public int ShufflingCount = 100;
    public int GridRows = 2;
    public int GridCols = 2;
    [Space]
    public Transform[] CardHolders;

    private CardsGrid grid;

    public bool isShufflingCards = false;
    public bool isPlacingCards = false;

    // Use this for initialization
    void Start ()
    {
        if (GameCards.Length == 0)
            throw new System.ArgumentException("Please provide unique cards.");
        if (CardHolders.Length == 0)
            throw new System.ArgumentException("Please provide a flattened representation of a card holders grid.");
        if (CardHolders.Length != (GridRows * GridCols))
            throw new System.ArgumentException("Number of card holders must match grid size (n == rows * columns).");

        grid = new CardsGrid(ref CardHolders, GridRows, GridCols);

        PlaceCards();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isShufflingCards)
        {
            ShuffleCards();
            isShufflingCards = false;
            isPlacingCards = false;
        }

        if (isPlacingCards)
        {
            PlaceCards();
            isPlacingCards = false;
        }
    }

    public void SetAllCardsFaceUp()
    {
        foreach (GameObject gameCard in GameCards)
            gameCard.GetComponent<CardController>().Card.SetFaceUp();
    }

    public void SetAllCardsFaceDown()
    {
        foreach (GameObject gameCard in GameCards)
            gameCard.GetComponent<CardController>().Card.SetFaceDown();
    }

    public void PlaceGameCards()
    {
        isPlacingCards = true;
    }

    public void PlaceShuffledGameCards()
    {
        isShufflingCards = true;
        isPlacingCards = true;
    }

    private void PlaceCards()
    {
        for (int i = 0; i < GameCards.Length; i++)
        {
            Transform cardPlaceholder = grid.GetCardPlaceholder(i);
            GameCards[i].transform.position = cardPlaceholder.position;
        }
    }

    private void ShuffleCards()
    {
        grid.Shuffle(ShufflingCount);
        PlaceCards();
    }

    private class CardsGrid
    {
        public readonly int Rows;
        public readonly int Cols;

        private Transform[,] grid;
        private Transform[] cardHolders;

        public CardsGrid(ref Transform[] cardHolders, int rows, int cols)
        {
            this.cardHolders = cardHolders;
            Rows = rows;
            Cols = cols;
            
            grid = new Transform[rows, cols];
            Reset();
        }

        public Transform GetCardPlaceholder(int index)
        {
            int rowIdx = (int) (((float) (index * Rows) / cardHolders.Length) + 1) - 1;
            int colIdx = index - Cols * rowIdx;
            return grid[rowIdx, colIdx];
        }

        public Transform GetCardPlaceholder(int row, int column)
        {
            return grid[row, column];
        }

        public void Reset()
        {
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    grid[i, j] = cardHolders[Cols * i + j];
        }

        public void Shuffle(int n)
        {
            for (int i = 0; i < n; i++)
            {
                int rowIdx1 = Random.Range(0, Rows);
                int colIdx1 = Random.Range(0, Cols);

                int rowIdx2 = Random.Range(0, Rows);
                int colIdx2 = Random.Range(0, Cols);

                var temp = grid[rowIdx1, colIdx1];
                grid[rowIdx1, colIdx1] = grid[rowIdx2, colIdx2];
                grid[rowIdx2, colIdx2] = temp;
            }
        }
    }

    //private class CardsShuffler
    //{
    //    public bool IsMoving { get; private set; }
    //    public float Speed { get; set; }

    //    private readonly CardsGrid cardsGrid;
    //    private readonly Rigidbody2D[] cardsBodies;

    //    public CardsShuffler(CardsGrid cardsGrid, ref Rigidbody2D[] cardsBodies)
    //    {
    //        this.cardsGrid = cardsGrid;
    //        this.cardsBodies = cardsBodies;
    //    }

    //    public void Shuffle()
    //    {
    //        IsMoving = true;

    //    }

    //    public void ShuffleOnce()
    //    {
    //        cardsGrid.Shuffle(10);
    //        for (int i = 0; i < cardsBodies.Length; i++)
    //        {
    //            Transform targetPosition = cardsGrid.GetCardPlaceholder(i);


    //        }
    //    }

    //    public void StopMoving()
    //    {
    //        IsMoving = false;
    //    }

    //    private bool HasReachedDestination()
    //    {
    //        return false;
    //    }
    //}
}
