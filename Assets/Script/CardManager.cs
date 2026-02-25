using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null &  Instance!= this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 6;
    // gameobject instance
    [SerializeField]
    private GameObject prefab;
    // parent object of cards
    [SerializeField]
    private GameObject cardList;
    // sprite for card back
    [SerializeField]
    private Sprite cardBack;
    // all possible sprite for card front
    [SerializeField]
    private Sprite[] sprites;
    // list of card
    private Card[] cards;
    //we place card on this panel
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;
    [SerializeField]
    TextMeshProUGUI matches;
    [SerializeField]
    TextMeshProUGUI turns;
    // other UI
    /*    [SerializeField]
        private Text sizeLabel;
        [SerializeField]
        private Slider sizeSlider;
        [SerializeField]
        private Text timeLabel;*/
    [SerializeField]
    private GameObject menu;
    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;
    [SerializeField]
    private ToggleGroup levelToggle;
    void Start()
    {
        gameStart = false;
        panel.SetActive(false);
    }

    // Start a game
    public void SetGameLevel() {
        Toggle toggle = levelToggle.ActiveToggles().FirstOrDefault();
        SetGridSize(toggle.GetComponent<LevelState>().level); 
    }
    public void StartCardGame()
    {
        if (gameStart) return; // return if game already running
        gameStart = true;
        HideMenue((gameStart ? false : true));
        // toggle UI
        panel.SetActive(true);
        info.SetActive(false);
        // set cards, size, position
        SetGamePanel();
        // renew gameplay variables
        cardSelected = spriteSelected = -1;
        cardLeft = cards.Length;
        // allocate sprite to card
        SpriteCardAllocation();
        StartCoroutine(HideFace());
       // time = 0;
    }

    // Initialize cards, size, and position based on size of game
    private void SetGamePanel()
    {
        int totalCards = rows * columns;

        // If odd, remove 1 card (must be even for pairing)
        if (totalCards % 2 != 0)
            totalCards -= 1;

        cards = new Card[totalCards];

        // Destroy old cards
        foreach (Transform child in cardList.transform)
            Destroy(child.gameObject);

        RectTransform panelSize = panel.GetComponent<RectTransform>();

        float rowSize = panelSize.sizeDelta.x;
        float colSize = panelSize.sizeDelta.y;

        float xInc = rowSize / columns;
        float yInc = colSize / rows;

        float scaleX = 1f / columns;
        float scaleY = 1f / rows;

        float startX = -rowSize / 2 + xInc / 2;
        float startY = -colSize / 2 + yInc / 2;

        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (index >= totalCards)
                    break;

                GameObject c = Instantiate(prefab, cardList.transform);

                c.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                c.transform.localPosition = new Vector3(
                    startX + j * xInc,
                    startY + i * yInc,
                    0);

                cards[index] = c.GetComponent<Card>();
                cards[index].ID = index;

                index++;
            }
        }
    }

    // Flip all cards after a short period
    IEnumerator HideFace()
    {
        //display for a short moment before flipping
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
    // Allocate pairs of sprite to card instances
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
        // sprite selection
        for (i = 0; i < cards.Length / 2; i++)
        {
            // get a random sprite
            int value = Random.Range(0, sprites.Length - 1);
            // check previous number has not been selection
            // if the number of cards is larger than number of sprites, it will reuse some sprites
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }

        // card sprite deallocation
        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
        // card sprite pairing allocation
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
    // Slider update gameSize
    /*   public void SetGameSize()
       {
           gameSize = (int)sizeSlider.value;
           sizeLabel.text = gameSize + " X " + gameSize;
       }*/
    // Update is called once per frame
    // return Sprite based on its id
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    // return card back Sprite
    public Sprite CardBack()
    {
        return cardBack;
    }
    // check if clickable
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
    // card onclick event
    public void cardClicked(int spriteId, int cardId)
    {
        // first card selected
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
        }
        else
        { // second card selected
            Card.CardClick++;
            TurnText(Card.CardClick);
            if (spriteSelected == spriteId)
            {
                //correctly matched
                Card.CardMatch++;
                CardMatch(Card.CardMatch);
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                CheckGameWin();
            }
            else
            {
                // incorrectly matched
                cards[cardSelected].Flip();
                cards[cardId].Flip();
            }
            cardSelected = spriteSelected = -1;
        }
    }
    // check if game is completed
    private void CheckGameWin()
    {
        // win game
        if (cardLeft == 0)
        {
            EndGame();
           // AudioPlayer.Instance.PlayAudio(1);
        }
    }
    // stop game
    private void EndGame()
    {
        ResetText();
        gameStart = false;
        HideMenue((gameStart ? false : true));
        panel.SetActive(false);
    }
    public void GiveUp()
    {
        ResetText();
        EndGame();
    }
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }

    private void GridLayout(int x, int y) { 
        rows= x;
        columns = y;
    }
    private void SetGridSize(Level level)
    {
        switch ( level)
        {
            case Level.Easy:
                GridLayout(2, 2);
                break;

            case Level.Moderate:
                GridLayout(2, 3);
                break;

            case Level.Medium:
                GridLayout(4, 3);
                break;

            case Level.Tricky:
                GridLayout(3, 4);
                break;

            case Level.Hard:
                GridLayout(4, 4);
                break;

            case Level.Extreme:
                GridLayout(4, 5);
                break;
        }
    }

    /// <summary>
    /// Hide game menue when start game
    /// </summary>
    /// <param name="hide"></param>
    private void HideMenue(bool hide)
    {
        menu.SetActive(hide);
    }
    // Set game text
    private void TurnText(int turnCount) { turns.text = turnCount.ToString(); }
    private void CardMatch(int match) { matches.text = match.ToString(); }
    //Reset Text when win
    private void ResetText()
    {
        Card.CardClick = Card.CardMatch= 0;
        TurnText(0);
        CardMatch(0);
    }
}
