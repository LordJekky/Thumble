using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameStates
{
    NotRunning,
    DemoStarting,
    DemoRunning,
    DemoEnding,
    WaitingForThumb,
    ThumbActive,
    ThumbLifted
}

public class GameBehaviour : MonoBehaviour 
{
    public GameObject ThumbSprite;

    GameStates State = GameStates.NotRunning;

    float FieldWidthTiles = 6;
    float FieldHeightTiles;

    int TilesX;
    int TilesY;

    List<GameObject> BGTiles;
    Queue<GameObject> RemovedBGTiles;

    List<GameObject> FGTiles;
    Queue<GameObject> RemovedFGTiles;

    List<EnemyData> EnemySprites;

    float TopLine = 0;

    float FirstTileX, FirstTileY;
    float TopTileY;

    float CurrentScrollSpeed;
    float DemoScrollSpeed = 0.025f;
    float MinScrollSpeed = 0.01f;
    float MaxScrollSpeed = 0.1f;
    float TransitionScrollSpeed = 0.5f;
    float ScrollSpeedIncrease = 0.001f;

    int BGTileIndex = 0;
    int FGTileIndex = 0;
    int DemoLinesCleared;

    int CurrentLives;
    int MaximumLives;

    bool EvenRow;
    int RowCounter;

    Random RNG;

    UIBehaviour UI;

	// Use this for initialization
	void Start () 
    {
        RNG = new Random();

        Input.multiTouchEnabled = false;

        ThumbSprite = GameObject.Find("ThumbSprite");
        ThumbSprite.SetActive(false);

        UI = GameObject.Find("UICanvas").GetComponent<UIBehaviour>();

        float Aspect = (float)Screen.height / (float)Screen.width;
        FieldHeightTiles = Aspect * FieldWidthTiles;

        Camera.main.orthographicSize = FieldHeightTiles * 0.5f;

        TilesX = (int)FieldWidthTiles;
        TilesY = (int)Mathf.Ceil(FieldHeightTiles);
        TilesY++;

        BGTiles = new List<GameObject>();
        RemovedBGTiles = new Queue<GameObject>();

        FGTiles = new List<GameObject>();
        RemovedFGTiles = new Queue<GameObject>();

        EnemySprites = new List<EnemyData>();

        FirstTileX = (-FieldWidthTiles / 2) + 0.5f;
        FirstTileY = (-FieldHeightTiles / 2) + 0.5f;

        RowCounter = 50;
        CurrentScrollSpeed = DemoScrollSpeed;

        TopTileY = Mathf.Ceil(TilesY / 2);
        TopTileY++;

        TopLine = FirstTileY;
        while (TopLine < TopTileY) AddTileRow();
        State = GameStates.DemoRunning;
	}

    public void StartNewGame()
    {
        ThumbSprite.SetActive(true);
        ThumbSprite.transform.position = new Vector3(0, FirstTileY + 2, -2);

        RowCounter = 0;

        CurrentLives = 2;
        MaximumLives = 2;

        UI.UpdateHearts(CurrentLives, MaximumLives);

        CurrentScrollSpeed = TransitionScrollSpeed;
        State = GameStates.DemoEnding;
    }

    private void GenerateHitMap()
    {
        for (float x = FirstTileX - 1; x < FirstTileX + FieldWidthTiles; x += 0.25f)
        {
            for (float y = FirstTileY - 1; y < FirstTileY + FieldHeightTiles; y += 0.25f)
            {
                if (EnemySprites[0].IsInterecting(x, y, 1))
                {
                    GameObject GridTile = new GameObject("Hit Tile " + BGTileIndex++);
                    SpriteRenderer Renderer = GridTile.AddComponent<SpriteRenderer>();

                    Renderer.sprite = Resources.Load<Sprite>("Square");
                    Renderer.color = Color.red;

                    GridTile.transform.localScale = new Vector3(0.20f, 0.20f, 1);
                    GridTile.transform.position = new Vector3(x, y, -4);
                }
            }
        }
    }

    public void EndGame()
    {
        State = GameStates.DemoStarting;
        DemoLinesCleared = 0;
        CurrentScrollSpeed = TransitionScrollSpeed;

        foreach (EnemyData Enemy in EnemySprites)
        {
            Enemy.Dispose();
        }
        EnemySprites.Clear();

        ThumbSprite.SetActive(false);

        UI.CloseUIPanel("GamePanel");
        UI.OpenUIPanel("GameOverPanel");
    }

    void AddTileRow()
    {
        bool EvenTile = EvenRow;
        EvenRow = !EvenRow;

        //add the background tiles
        for (int x = 0; x < TilesX; x++)
        {
            GameObject GridTile;
            SpriteRenderer Renderer;

            if (RemovedBGTiles.Count > 0)
            {
                GridTile = RemovedBGTiles.Dequeue();
                Renderer = GridTile.GetComponent<SpriteRenderer>();
            }
            else
            {
                GridTile = new GameObject("BG Tile" + BGTileIndex++);
                Renderer = GridTile.AddComponent<SpriteRenderer>();
            }

            string TileName;
            if (EvenTile) TileName = "WhiteTile";
            else TileName = "BlackTile";

            if (Random.Range(1, 100) < 10)
            {
                TileName = "Cracked" + TileName;

                if (Random.Range(1, 100) < 50) TileName += "1";
                else TileName += "2";
            }
            else
            {
                TileName = "Clean" + TileName;
            }

            Renderer.sprite = Resources.Load<Sprite>("Tiles/" + TileName);

            GridTile.transform.position = new Vector3(FirstTileX + x, TopLine, 0);

            EvenTile = !EvenTile;

            BGTiles.Add(GridTile);
        }

        //add the foreground tiles
        for (int x = 0; x < TilesX; x++)
        {
            if (Random.Range(1, 100) < 75 || RowCounter < 6) continue;

            GameObject GridTile;
            SpriteRenderer Renderer;

            if (RemovedFGTiles.Count > 0)
            {
                GridTile = RemovedFGTiles.Dequeue();
                Renderer = GridTile.GetComponent<SpriteRenderer>();
            }
            else
            {
                GridTile = new GameObject("FG Tile" + FGTileIndex++);
                Renderer = GridTile.AddComponent<SpriteRenderer>();
            }

            Renderer.sprite = Resources.Load<Sprite>("Wall");

            GridTile.transform.position = new Vector3(FirstTileX + x, TopLine, -1);

            FGTiles.Add(GridTile);
        }

        DemoLinesCleared++;
        RowCounter++;
        TopLine++;
    }

	// Update is called once per frame
	void Update () 
    {
        switch (State)
        {
            case GameStates.NotRunning: return;
            case GameStates.DemoStarting:
                {
                    //whatever is there is cleared to the bottom rapidly
                    //and replaced with the start of the demo

                    UpdateScrolling();

                    if (DemoLinesCleared == TilesY)
                    {
                        CurrentScrollSpeed = DemoScrollSpeed;
                        State = GameStates.DemoRunning;
                    }

                    break;
                }
            case GameStates.DemoRunning:
                {
                    //similar to normal running, with no lives, timers, or collisions
                    UpdateScrolling();
                    break;
                }
            case GameStates.DemoEnding:
                {
                    //demo is cleared to bottom rapidly, and replaced with
                    //first screen of new game
                    UpdateScrolling();

                    if (RowCounter > TilesY)
                    {
                        State = GameStates.WaitingForThumb;
                        CurrentScrollSpeed = MinScrollSpeed;
                    }
                    break;
                }
            case GameStates.WaitingForThumb:
                {
                    CheckForThumbDown();
                    break;
                }
            case GameStates.ThumbActive:
                {
                    UpdateScrolling();
                    CheckForCollision();
                    AddEnemies();
                    UpdateEnemies();

                    if (CurrentLives == 0) EndGame();

                    break;
                }
            case GameStates.ThumbLifted:
                {
                    UpdateScrolling();
                    AddEnemies();
                    UpdateEnemies();

                    CheckForThumbDown();
                    break;
                }
        }
	}

    private void UpdateEnemies()
    {
        foreach (EnemyData Enemy in new List<EnemyData>(EnemySprites))
        {
            Enemy.Update();

            if (Enemy.Disposed)
            {
                EnemySprites.Remove(Enemy);
                return;
            }
        }
    }

    private void AddEnemies()
    {
        if (EnemySprites.Count > 4) return;
        if (Random.Range(1, 100) != 25) return;

        EnemySprites.Add(new EnemyData(FieldWidthTiles, FieldHeightTiles));
    }

    private void CheckForCollision()
    {
        Vector2 p1 = new Vector2(ThumbSprite.transform.position.x, ThumbSprite.transform.position.y);

        foreach (GameObject Tile in new List<GameObject>(FGTiles))
        {
            Vector2 p2 = new Vector2(Tile.transform.position.x, Tile.transform.position.y);

            if ((p2 - p1).magnitude < 0.9)
            {
                Handheld.Vibrate();

                Tile.transform.Translate(-100, -100, 0);
                FGTiles.Remove(Tile);
                RemovedFGTiles.Enqueue(Tile);

                LoseLife();
            }
        }

        foreach (EnemyData Enemy in new List<EnemyData>(EnemySprites))
        {
            if (Enemy.IsInterecting(ThumbSprite.transform.position.x, ThumbSprite.transform.position.y, 0.9f))
            {
                Handheld.Vibrate();

                Enemy.Dispose();
                EnemySprites.Remove(Enemy);

                LoseLife();
            }
        }
    }

    private void LoseLife()
    {
        CurrentLives--;
        if (CurrentLives < 0) CurrentLives = 0;

        UI.UpdateHearts(CurrentLives, MaximumLives);
    }

    private void UpdateScrolling()
    {
        if (State == GameStates.ThumbActive)
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount == 1)
                {
                    Vector3 MousePosition = Input.touches[0].position;
                    MousePosition = Camera.main.ScreenToWorldPoint(MousePosition);

                    ThumbSprite.transform.position = new Vector3(MousePosition.x, MousePosition.y, -2);
                }
                else
                {
                    State = GameStates.ThumbLifted;
                    return;
                }
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                    ThumbSprite.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2);
                }
                else
                {
                    State = GameStates.ThumbLifted;
                    return;
                }
            }
        }

        while (TopLine < TopTileY) AddTileRow();

        TopLine -= CurrentScrollSpeed;

        foreach (GameObject Tile in new List<GameObject>(BGTiles))
        {
            Tile.transform.Translate(0, -CurrentScrollSpeed, 0);

            if (Tile.transform.position.y <= (-FieldHeightTiles / 2) - 0.5f)
            {
                BGTiles.Remove(Tile);
                RemovedBGTiles.Enqueue(Tile);
            }
        }

        foreach (GameObject Tile in new List<GameObject>(FGTiles))
        {
            Tile.transform.Translate(0, -CurrentScrollSpeed, 0);

            if (Tile.transform.position.y <= (-FieldHeightTiles / 2) - 0.5f)
            {
                FGTiles.Remove(Tile);
                RemovedFGTiles.Enqueue(Tile);
            }
        }
    }

    private void CheckForThumbDown()
    {
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                Vector3 mousePosition = Input.touches[0].position;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                Vector2 p1 = new Vector2(mousePosition.x, mousePosition.y);
                Vector2 p2 = new Vector2(ThumbSprite.transform.position.x, ThumbSprite.transform.position.y);

                float Distance = (p2 - p1).magnitude;

                if (Distance < 1)
                {
                    State = GameStates.ThumbActive;
                    ThumbSprite.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2);
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                Vector2 p1 = new Vector2(mousePosition.x, mousePosition.y);
                Vector2 p2 = new Vector2(ThumbSprite.transform.position.x, ThumbSprite.transform.position.y);

                float Distance = (p2 - p1).magnitude;

                if (Distance < 1)
                {
                    State = GameStates.ThumbActive;
                    ThumbSprite.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2);
                }
            }
        }
    }
}
