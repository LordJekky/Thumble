using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point2D
{
    public float X;
    public float Y;


    public Point2D(float X, float Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public Point2D Rotate(float OriginX, float OriginY, float Angle)
    {
        //assuming angle is in degrees, because unity does them that way for some reason

        Angle = Angle * (Mathf.PI / 180f);

        float px = Mathf.Cos(Angle) * (X - OriginX) - Mathf.Sin(Angle) * (Y - OriginY) + OriginX;
        float py = Mathf.Sin(Angle) * (X - OriginX) + Mathf.Cos(Angle) * (Y - OriginY) + OriginY;

        return new Point2D(px, py);
    }
}

public class EnemyData
{
    GameObject Sprite;
    SpriteRenderer Renderer;

    Vector2 Direction;
    float MoveSpeed = 0.075f;
    float RotateSpeed = 2f;

    float SpriteWidth, SpriteHeight;

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

        SpriteWidth = Renderer.bounds.size.x * 0.9f;
        SpriteHeight = Renderer.bounds.size.y * 0.9f;

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

    float sign(Point2D p1, Point2D p2, Point2D p3)
    {
        return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
    }

    bool PointInTriangle(Point2D pt, Point2D v1, Point2D v2, Point2D v3)
    {
        bool b1, b2, b3;

        b1 = sign(pt, v1, v2) < 0.0f;
        b2 = sign(pt, v2, v3) < 0.0f;
        b3 = sign(pt, v3, v1) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }

    float GetDistance(Point2D A, Point2D B)
    {
        float LenX = Mathf.Abs(A.X - B.X);
        float LenY = Mathf.Abs(A.Y - B.Y);

        return Mathf.Sqrt((LenX * LenX) + (LenY * LenY));
    }

    float GetDistanceSquared(Point2D A, Point2D B)
    {
        float LenX = Mathf.Abs(A.X - B.X);
        float LenY = Mathf.Abs(A.Y - B.Y);

        return (LenX * LenX) + (LenY * LenY);
    }

    float GetDistanceToLineSegment(Vector2 SegmentStart, Vector2 SegmentEnd, Vector2 p)
    {
        // Return minimum distance between line segment vw and point p
        float l2 = (SegmentEnd - SegmentStart).sqrMagnitude;

        if (l2 == 0.0) return (SegmentStart-p).magnitude;   // v == w case

        // Consider the line extending the segment, parameterized as v + t (w - v).
        // We find projection of point p onto the line. 
        // It falls where t = [(p-v) . (w-v)] / |w-v|^2
        // We clamp t from [0,1] to handle points outside the segment vw.

        float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(p - SegmentStart, SegmentEnd - SegmentStart) / l2));
        Vector2 projection = SegmentStart + t * (SegmentEnd - SegmentStart);  // Projection falls on the segment
        return (p - projection).magnitude;
    }

    public bool IsInterecting(float Circle_x, float Circle_y, float Circle_Radius)
    {
        //get the 4 corners of the sprite rect (unrotated)
        Point2D p1 = new Point2D(Sprite.transform.position.x - (SpriteWidth / 2f), Sprite.transform.position.y - (SpriteHeight / 2f));   //bottom-left
        Point2D p2 = new Point2D(Sprite.transform.position.x + (SpriteWidth / 2f), Sprite.transform.position.y - (SpriteHeight / 2f));  //bottom-right
        Point2D p3 = new Point2D(Sprite.transform.position.x + (SpriteWidth / 2f), Sprite.transform.position.y + (SpriteHeight / 2f));  //top-left
        Point2D p4 = new Point2D(Sprite.transform.position.x - (SpriteWidth / 2f), Sprite.transform.position.y + (SpriteHeight / 2f));  //top-right

        //MonoBehaviour.print(p1.X + ", " + p1.Y + " : " + p2.X + ", " + p2.Y + " : " + p3.X + ", " + p3.Y + " : " + p4.X + ", " + p4.Y);

        //apply rotation to the 4 corners to get the -actual- rectangle
        p1 = p1.Rotate(Sprite.transform.position.x, Sprite.transform.position.y, Sprite.transform.eulerAngles.z);
        p2 = p2.Rotate(Sprite.transform.position.x, Sprite.transform.position.y, Sprite.transform.eulerAngles.z);
        p3 = p3.Rotate(Sprite.transform.position.x, Sprite.transform.position.y, Sprite.transform.eulerAngles.z);
        p4 = p4.Rotate(Sprite.transform.position.x, Sprite.transform.position.y, Sprite.transform.eulerAngles.z);

        //MonoBehaviour.print(p1.X + ", " + p1.Y + " : " + p2.X + ", " + p2.Y + " : " + p3.X + ", " + p3.Y + " : " + p4.X + ", " + p4.Y);

        //split into 2 triangles - 1,2,3 and 3,4,1 (CCW winding)
        //test if the thumb center point is inside one of the triangles
        Point2D ThumbPoint = new Point2D(Circle_x, Circle_y);

        if (PointInTriangle(ThumbPoint, p1, p2, p3))
        {
            //MonoBehaviour.print("Inside triangle A");
            //MonoBehaviour.print(ThumbPoint.X + ", " + ThumbPoint.Y + " : " + p1.X + ", " + p1.Y + " : " + p2.X + ", " + p2.Y + " : " + p3.X + ", " + p3.Y);
            return true;
        }
        if (PointInTriangle(ThumbPoint, p3, p4, p1))
        {
            //MonoBehaviour.print("Inside triangle B");
            return true;
        }

        Vector2 v1 = new Vector2(p1.X, p1.Y);
        Vector2 v2 = new Vector2(p2.X, p2.Y);
        Vector2 v3 = new Vector2(p3.X, p3.Y);
        Vector2 v4 = new Vector2(p4.X, p4.Y);
        Vector2 Vt = new Vector2(ThumbPoint.X, ThumbPoint.Y);

        //if not, test for closest point on the edge of the rect to the thumb point
        //and check for distance <= radius

        Circle_Radius *= 0.9f;

        if (GetDistanceToLineSegment(v1, v2, Vt) <= Circle_Radius)
        {
            //MonoBehaviour.print("Line segment A");
            return true;
        }
        if (GetDistanceToLineSegment(v2, v3, Vt) <= Circle_Radius)
        {
            //MonoBehaviour.print("Line segment B");
            return true;
        }
        if (GetDistanceToLineSegment(v3, v4, Vt) <= Circle_Radius)
        {
            //MonoBehaviour.print("Line segment C");
            return true;
        }
        if (GetDistanceToLineSegment(v4, v1, Vt) <= Circle_Radius)
        {
            //MonoBehaviour.print("Line segment D");
            return true;
        }

        return false;
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
