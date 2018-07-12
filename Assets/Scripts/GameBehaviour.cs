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

/*public class EnemyData
{
    GameObject Sprite;
    SpriteRenderer Renderer;

    Vector2 Direction;
    float MoveSpeed = 0.075f;
    float RotateSpeed = 2f;

    public bool Disposed;

    static int EnemyIndex = 0;

    float TilesX, TilesY;

    public EnemyData(float TilesX, float TilesY)
    {
        this.TilesX = TilesX;
        this.TilesY = TilesY;

        bool Left = true;
        if (Random.Range(1, 100) < 50) Left = false;

        float Xpos = 0;
        if (Left) Xpos = (TilesX / 2f) + 1f;
        else Xpos = (-TilesX / 2f) - 1f;

        float Ypos = Random.Range(0f, TilesY / 2);

        Sprite = new GameObject("Enemy" + EnemyIndex++);
        Renderer = Sprite.AddComponent<SpriteRenderer>();

        Renderer.sprite = Resources.Load<Sprite>("KnifeEnemy");

        Sprite.transform.position = new Vector3(Xpos, Ypos, -3);

        if (Left)
        {
            Direction = new Vector2(-0.5f, -0.5f);
        }
        else
        {
            Renderer.flipX = true;
            Direction = new Vector2(0.5f, -0.5f);
            RotateSpeed *= -1;
        }
    }

    public bool IsInterecting(float Circle_x, float Circle_y, float Circle_Radius)
    {
        //this is not working correctly

        float rel_x = Circle_x - Sprite.transform.position.x;
        float rel_y = Circle_y - Sprite.transform.position.y;

        float angle = -Sprite.transform.rotation.eulerAngles.z;
        angle = angle * (Mathf.PI / 180f);

        float local_x = Mathf.Cos(angle) * rel_x - Mathf.Sin(angle) * rel_y + Sprite.transform.position.x;
        float local_y = Mathf.Sin(angle) * rel_x + Mathf.Cos(angle) * rel_y + Sprite.transform.position.y;

        float minx = -0.5f;
        float maxx = 0.5f;

        float miny = -1.24f;
        float maxy = 1.24f;

        float delta_x = local_x;
        if (delta_x < minx) delta_x = minx;
        if (delta_x > maxx) delta_x = maxx;

        float delta_y = local_y;
        if (delta_y < miny) delta_y = miny;
        if (delta_y > maxy) delta_y = maxy;

        delta_x = Mathf.Abs(local_x - delta_x);
        delta_y = Mathf.Abs(local_y - delta_y);

        return delta_x * delta_x + delta_y * delta_y < Circle_Radius * Circle_Radius;
    }

    public bool IsInterectingDebug(float Circle_x, float Circle_y, float Circle_Radius)
    {
        MonoBehaviour.print("cx: " + Circle_x + ", cy: " + Circle_y + ", cr: " + Circle_Radius);

        float rel_x = Circle_x - Sprite.transform.position.x;
        float rel_y = Circle_y - Sprite.transform.position.y;

        MonoBehaviour.print("relx: " + rel_x + ", rely: " + rel_y);

        float angle = -Sprite.transform.rotation.eulerAngles.z;
        angle = angle * (Mathf.PI / 180f);

        MonoBehaviour.print("angle: " + angle);

        float local_x = Mathf.Cos(angle) * rel_x - Mathf.Sin(angle) * rel_y + Sprite.transform.position.x;
        float local_y = Mathf.Sin(angle) * rel_x + Mathf.Cos(angle) * rel_y + Sprite.transform.position.y;

        MonoBehaviour.print("localx: " + local_x + ", localy: " + local_y);

        float minx = -0.5f;
        float maxx = 0.5f;

        float miny = -1.24f;
        float maxy = 1.24f;

        float delta_x = local_x;
        if (delta_x < minx) delta_x = minx;
        if (delta_x > maxx) delta_x = maxx;

        float delta_y = local_y;
        if (delta_y < miny) delta_y = miny;
        if (delta_y > maxy) delta_y = maxy;

        MonoBehaviour.print("nearx: " + delta_x + ", neary: " + delta_y);

        delta_x = Mathf.Abs(local_x - delta_x);
        delta_y = Mathf.Abs(local_y - delta_y);

        MonoBehaviour.print("deltax: " + delta_x + ", deltay: " + delta_y);

        return delta_x * delta_x + delta_y * delta_y < Circle_Radius * Circle_Radius;
    }

    public void Dispose()
    {
        if (Disposed) return;

        Object.Destroy(Sprite);
        Disposed = true;
    }

    internal void Update()
    {
        Vector2 MoveVector = Direction;
        MoveVector.Normalize();

        MoveVector *= MoveSpeed;

        Sprite.transform.position = new Vector3(Sprite.transform.position.x + MoveVector.x, Sprite.transform.position.y + MoveVector.y, -3);
        Sprite.transform.Rotate(0, 0, RotateSpeed);

        if (Sprite.transform.position.x < (-TilesX / 2f) - 1.5f) Dispose();
        if (Sprite.transform.position.x > (TilesX / 2f) + 1.5f) Dispose();
        if (Sprite.transform.position.y < (-TilesY / 2f) - 1.5f) Dispose();
    }
}*/

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

	// Use this for initialization
	void Start () 
    {
        RNG = new Random();

        Input.multiTouchEnabled = false;

        ThumbSprite = GameObject.Find("ThumbSprite");
        ThumbSprite.SetActive(false);

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

            
            Renderer.sprite = Resources.Load<Sprite>("Square");

            GridTile.transform.position = new Vector3(FirstTileX + x, TopLine, 0);

            if (EvenTile) Renderer.color = new Color(0.2f, 0.2f, 0.2f);
            else Renderer.color = new Color(0.8f, 0.8f, 0.8f);

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
        //CurrentLives--;
        if (CurrentLives < 0) CurrentLives = 0;

        //visual update
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
