using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData
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
}
