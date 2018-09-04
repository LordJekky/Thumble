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
    CountDown,
    ThumbActive,
    ThumbLifted,
    Paused
}

enum TileStates
{
    Unset,          //default value, not yet determined
    Gap,            //this tile has no obstacle
    Block,          //this tile has an obstacle
    PathGap         //this tile has no obstacle - and is part of the guaranteed open path
}

class RowObstacleData
{
    public TileStates[] States;
    public bool HasObstacles;

    public RowObstacleData()
    {
        States = new TileStates[6];
        HasObstacles = false;
    }
}

public class GameBehaviour : MonoBehaviour 
{
    public GameObject ThumbSprite;

    GameStates State = GameStates.NotRunning;
    GameStates PrePausedState;

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
    float MinScrollSpeed = 0.05f;
    float MaxScrollSpeed = 0.20f;
    float TransitionScrollSpeed = 0.75f;

    //how much faster should the game scroll for each line that passes
    float ScrollSpeedIncrease = 0.0005f;

    //how many blocks in a row should be obstacles
    int MinObstacleCountH;
    int MaxObstacleCountH;

    //how much horizontal space should there be between obstacles
    int MinObstacleGapH;

    //how many blocks tall should obstavles be (how many lines in a row can have obstacles)
    int MinObstacleSizeV;
    int MaxObstacleSizeV;

    //how many lines should be left clear between 
    int MinObstacleGapV;
    int MaxObstacleGapV;

    bool CreatingObstacle;
    int LinesToGo;

    int Score;

    //rotating list, used to track what is on each row - used for density checks
    //and to ensure that gaps are left for the player
    List<RowObstacleData> Rows;

    float ThumbUpTimer;

    int BGTileIndex = 0;
    int FGTileIndex = 0;
    int DemoLinesCleared;

    int CurrentLives;
    int MaximumLives;

    bool EvenRow;
    int RowCounter;

    int Difficulty;

    Random RNG;

    UIBehaviour UI;

    public GameObject TextCountDown;
    Animator TextCountDownAnimator;
    int CountDownValue = 4;

	// Use this for initialization
	void Start () 
    {
        RNG = new Random();

        Input.multiTouchEnabled = false;

        ThumbSprite = GameObject.Find("ThumbSprite");
        ThumbSprite.SetActive(false);

        UI = GameObject.Find("UICanvas").GetComponent<UIBehaviour>();

        //TextCountDown = GameObject.Find("TextCountDown"); // CountDown
        TextCountDownAnimator = TextCountDown.GetComponent<Animator>();

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

        Rows = new List<RowObstacleData>();

        TopLine = FirstTileY;
        while (TopLine < TopTileY) AddTileRow();
        State = GameStates.DemoRunning;
	}

    public void StartNewGame()
    {
        ThumbSprite.SetActive(true);
        ThumbSprite.transform.position = new Vector3(0, FirstTileY + 2, -2);

        ThumbUpTimer = 10;

        RowCounter = 0;

        Rows = new List<RowObstacleData>();
        Rows.Add(new RowObstacleData());

        CurrentLives = 2;
        MaximumLives = 2;

        UI.UpdateHearts(CurrentLives, MaximumLives);
        UI.UpdateTimer(ThumbUpTimer);

        Difficulty = 0;
        IncreaseDifficulty();

        CreatingObstacle = false;
        LinesToGo = Random.Range(MinObstacleGapV, MaxObstacleGapV+1);

        CurrentScrollSpeed = TransitionScrollSpeed;
        State = GameStates.DemoEnding;

        Score = 0;
    }

    private void IncreaseDifficulty()
    {
        Difficulty++;
        if (Difficulty > 7) Difficulty = 7;

        switch (Difficulty)
        {
            case 1:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 2;

                    MinObstacleGapH = 4;

                    MinObstacleSizeV = 1;
                    MaxObstacleSizeV = 2;

                    MinObstacleGapV = 2;
                    MaxObstacleGapV = 8;

                    break;
                }
            case 2:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 3;

                    MinObstacleGapH = 3;

                    MinObstacleSizeV = 1;
                    MaxObstacleSizeV = 2;

                    MinObstacleGapV = 2;
                    MaxObstacleGapV = 8;

                    break;
                }
            case 3:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 3;

                    MinObstacleGapH = 3;

                    MinObstacleSizeV = 2;
                    MaxObstacleSizeV = 3;

                    MinObstacleGapV = 2;
                    MaxObstacleGapV = 6;

                    break;
                }
            case 4:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 3;

                    MinObstacleGapH = 3;

                    MinObstacleSizeV = 2;
                    MaxObstacleSizeV = 4;

                    MinObstacleGapV = 2;
                    MaxObstacleGapV = 4;

                    break;
                }
            case 5:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 4;

                    MinObstacleGapH = 2;

                    MinObstacleSizeV = 2;
                    MaxObstacleSizeV = 4;

                    MinObstacleGapV = 2;
                    MaxObstacleGapV = 4;

                    break;
                }
            case 6:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 4;

                    MinObstacleGapH = 2;

                    MinObstacleSizeV = 2;
                    MaxObstacleSizeV = 5;

                    MinObstacleGapV = 1;
                    MaxObstacleGapV = 3;

                    break;
                }
            case 7:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 4;

                    MinObstacleGapH = 2;

                    MinObstacleSizeV = 3;
                    MaxObstacleSizeV = 6;

                    MinObstacleGapV = 1;
                    MaxObstacleGapV = 2;

                    break;
                }
            case 8:
                {
                    MinObstacleCountH = 2;
                    MaxObstacleCountH = 5;

                    MinObstacleGapH = 1;

                    MinObstacleSizeV = 3;
                    MaxObstacleSizeV = 6;

                    MinObstacleGapV = 1;
                    MaxObstacleGapV = 2;

                    break;
                }
            case 9:
                {
                    MinObstacleCountH = 3;
                    MaxObstacleCountH = 5;

                    MinObstacleGapH = 1;

                    MinObstacleSizeV = 4;
                    MaxObstacleSizeV = 6;

                    MinObstacleGapV = 1;
                    MaxObstacleGapV = 2;

                    break;
                }
        }

        
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
        UI.CloseUIPanel("GameSettingsPanel");
        UI.OpenUIPanel("GameOverPanel");
    }

    void AddTileRow()
    {
        if (State == GameStates.ThumbActive || State == GameStates.ThumbLifted) Score++;

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

        if (RowCounter > 6)
        {
            if (RowCounter % 60 == 0) IncreaseDifficulty();

            if (CreatingObstacle)
            {
                CreateObstacleRow();

                LinesToGo--;
                if (LinesToGo == 0)
                {
                    CreatingObstacle = false;
                    LinesToGo = Random.Range(MinObstacleGapV, MaxObstacleGapV + 1);
                    print("" + LinesToGo);
                }
            }
            else
            {
                RowObstacleData Data = new RowObstacleData();
                Rows.Insert(0, Data);
                if (Rows.Count > 5) Rows.RemoveAt(Rows.Count - 1);

                LinesToGo--;
                if (LinesToGo == 0)
                {
                    CreatingObstacle = true;
                    LinesToGo = Random.Range(MinObstacleSizeV, MaxObstacleSizeV + 1);
                }
            }
        }

        
        
        DemoLinesCleared++;
        RowCounter++;
        TopLine++;

        if (State == GameStates.ThumbActive || State == GameStates.ThumbLifted)
        {
            CurrentScrollSpeed += ScrollSpeedIncrease;
            if (CurrentScrollSpeed > MaxScrollSpeed) CurrentScrollSpeed = MaxScrollSpeed;
        }
    }

    private void CreateObstacleRow()
    {
        TileStates[] RowLayout = new TileStates[TilesX];

        RowObstacleData Data = new RowObstacleData();
        Data.HasObstacles = true;

        RowObstacleData PrevData = Rows[0];

        int ObstacleCount = 0;
        int ObstacleTarget = Random.Range(MinObstacleCountH, MaxObstacleCountH);

        //at least one gap has to line up with the row below this one
        List<int> GapIndices = new List<int>();
        for (int i = 0; i < TilesX; i++)
        {
            if (PrevData.HasObstacles == false || PrevData.States[i] == TileStates.PathGap) GapIndices.Add(i);
        }

        int StartIndex = GapIndices[Random.Range(0, GapIndices.Count)];
        int EndIndex = StartIndex;

        RowLayout[StartIndex] = TileStates.PathGap;

        //the gap should be forced to the min gap width, randomly add gap tiles left and right
        int GapWidth = 1;
        while (GapWidth < MinObstacleGapH)
        {
            int Dir = Random.Range(0, 2);

            if (Dir == 0)
            {
                if (StartIndex == 0) continue;
                
                StartIndex--;

                RowLayout[StartIndex] = TileStates.PathGap;
                GapWidth++;
            }
            else
            {
                if (EndIndex == TilesX-1) continue;

                EndIndex++;

                RowLayout[EndIndex] = TileStates.PathGap;
                GapWidth++;
            }
        }

        for (int i = 0; i < TilesX; i++)
        {
            if (RowLayout[i] == TileStates.Unset) RowLayout[i] = TileStates.Gap;
        }

        //add obstacles at random until the obstacle count is met
        while (ObstacleCount < ObstacleTarget)
        {
            int a = Random.Range(0, TilesX);
            if (RowLayout[a] == TileStates.Gap)
            {
                RowLayout[a] = TileStates.Block;
                ObstacleCount++;
            }
        }

        for (int x = 0; x < TilesX; x++)
        {
            Data.States[x] = RowLayout[x];

            if (RowLayout[x] != TileStates.Block) continue;

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

            //if (RowLayout[x] == TileStates.PathGap) Renderer.color = new Color(0, 1, 0, 0.5f);
            //else Renderer.color = new Color(1, 1, 1, 1);

            FGTiles.Add(GridTile);
        }

        Rows.Insert(0, Data);
        if (Rows.Count > 5) Rows.RemoveAt(Rows.Count - 1);
    }

    public void PauseGame()
    {
        PrePausedState = State;
        State = GameStates.Paused;
    }

    public void ResumeGame()
    {
        State = PrePausedState;
    }

	// Update is called once per frame
	void Update ()        
    {
        switch (State)
        {
            case GameStates.NotRunning: return;
            case GameStates.Paused: return;
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
                    if (UI.AnimatingPanel()) return;

                    CheckForThumbDown();
                    break;
                }
            case GameStates.CountDown:
                {
                    CheckCountDown();

                    break;
                }
            case GameStates.ThumbActive:
                {
                    if (UI.AnimatingPanel()) return;

                    UpdateScrolling();
                    CheckForCollision();
                    AddEnemies();
                    UpdateEnemies();

                    if (CurrentLives == 0) EndGame();

                    if (ThumbUpTimer < 3)
                    {
                        ThumbUpTimer += Time.deltaTime;
                        if (ThumbUpTimer > 3) ThumbUpTimer = 3;
                        UI.UpdateTimer(ThumbUpTimer);
                    }

                    break;
                }
            case GameStates.ThumbLifted:
                {
                    if (UI.AnimatingPanel()) return;

                    UpdateScrolling();
                    AddEnemies();
                    UpdateEnemies();

                    ThumbUpTimer -= Time.deltaTime;
                    if (ThumbUpTimer <= 0) EndGame();

                    UI.UpdateTimer(ThumbUpTimer);

                    CheckForThumbDown();
                    break;
                }
        }
	}

    public void CheckCountDown()
    {
        if (Application.isMobilePlatform)//is an Android
        {
            if (Input.touchCount == 1)
            {
                if (!TextCountDown.activeInHierarchy) TextCountDown.SetActive(true);

                if (!TextCountDownAnimator.GetCurrentAnimatorStateInfo(0).IsName("NumberFades"))
                {
                    //se l'animazione non sta venendo eseguita, eseguila ed avanza nel conto alla rovescia
                    CountDownValue--;
                    
                    if (CountDownValue == 0)
                    {
                        TextCountDown.SetActive(false);
                        CountDownValue = 4;
                        State = GameStates.ThumbActive;
                        return;
                    }

                    TextCountDown.GetComponent<UnityEngine.UI.Text>().text = "" + CountDownValue;
                    TextCountDownAnimator.Play("NumberFades", 0);
                }
            }
            else
            if (Input.touchCount == 0)
            {
                TextCountDown.SetActive(false);

                State = GameStates.WaitingForThumb;
                CountDownValue = 4;
            }


        }
        else//is a PC
        {
            if (Input.GetMouseButton(0))
            {
                if (!TextCountDown.activeInHierarchy) TextCountDown.SetActive(true);

                if (!TextCountDownAnimator.GetCurrentAnimatorStateInfo(0).IsName("NumberFades"))
                {
                    //se l'animazione non sta venendo eseguita, eseguila ed avanza nel conto alla rovescia
                    CountDownValue--;

                    if (CountDownValue == 0)
                    {
                        TextCountDown.SetActive(false);
                        CountDownValue = 4;
                        State = GameStates.ThumbActive;
                        return;
                    }

                    TextCountDown.GetComponent<UnityEngine.UI.Text>().text = "" + CountDownValue;
                    TextCountDownAnimator.Play("NumberFades", 0);
                }
            }
            else
            {
                TextCountDown.SetActive(false);

                State = GameStates.WaitingForThumb;
                CountDownValue = 4;
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
        if (EnemySprites.Count > 1) return;
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
                    State = GameStates.CountDown;
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
                    State = GameStates.CountDown;
                    ThumbSprite.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2);
                }
            }
        }
    }

    public static GameObject PrefabLoader(string prefabName)
    {
        return Resources.Load<GameObject>("Prefabs/" + prefabName);
    }
}
