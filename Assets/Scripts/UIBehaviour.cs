using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum UIAnimationStates
{
    Default,
    ClearingPanel,
    OpeningPanel
}

class FadingUIObject
{
    GameObject UIObject;
    Image ImgComp;

    public bool Completed;
    public bool FadeIn;
    public float Speed;

    Color OriginalColour;

    public FadingUIObject(GameObject UIObject, bool FadeIn, float Speed)
    {
        this.UIObject = UIObject;
        this.FadeIn = FadeIn;
        this.Speed = Speed;

        ImgComp = UIObject.GetComponent<Image>();
        OriginalColour = ImgComp.color;

        MonoBehaviour.print(OriginalColour);

        if (FadeIn)
        {
            Color c = ImgComp.color;
            c.a = 0;
            ImgComp.color = c;
        }
    }

    public void Dispose()
    {
        ImgComp.color = OriginalColour;
    }

    public void Update()
    {
        Color c = ImgComp.color;

        if (FadeIn)
        {
            c.a += Speed;
            if (c.a > 1) c.a = 1;
            if (c.a == OriginalColour.a) Completed = true;
        }
        else
        {
            c.a -= Speed;
            if (c.a <= 0) Completed = true;
        }

        ImgComp.color = c;
    }
}

class MovingUIObject
{
    GameObject UIObject;
    Vector2 Target;
    float Speed;

    Vector3 OriginalPosition;

    public bool Completed;

    public MovingUIObject(GameObject UIObject, Vector2 Target, float Speed)
    {
        this.UIObject = UIObject;
        this.Target = Target;
        this.Speed = Speed;

        OriginalPosition = UIObject.transform.localPosition;
    }

    public void Dispose()
    {
        UIObject.transform.localPosition = OriginalPosition;
    }

    public void Update()
    {
        Vector3 Pos = UIObject.transform.localPosition;

        //moving each aspect individually for now - If objects need to move at a diagonal later
        //change this to use vectors for better appearance.

        if (Pos.y < Target.y)
        {
            Pos.y += Speed;
            if (Pos.y > Target.y) Pos.y = Target.y;
        }
        else if (Pos.y > Target.y)
        {
            Pos.y -= Speed;
            if (Pos.y < Target.y) Pos.y = Target.y;
        }

        if (Pos.x < Target.x)
        {
            Pos.x += Speed;
            if (Pos.x > Target.x) Pos.x = Target.x;
        }
        else if (Pos.x > Target.x)
        {
            Pos.x -= Speed;
            if (Pos.x < Target.x) Pos.x = Target.x;
        }

        UIObject.transform.localPosition = Pos;

        if (Pos.y == Target.y && Pos.x == Target.x) Completed = true;
    }
}

public class UIBehaviour : MonoBehaviour 
{
    List<MovingUIObject> MovingObjects;
    List<MovingUIObject> FinishedObjects;

    List<FadingUIObject> FadingObjects;
    List<FadingUIObject> FinishedFadingObjects;

    Dictionary<string, GameObject> UIPanels;

    string PendingNewPanel;
    string PendingOldPanel;

    UIAnimationStates State;

	// Use this for initialization
	void Start () 
    {
        UIPanels = new Dictionary<string, GameObject>();

        foreach (GameObject UIPanel in GameObject.FindGameObjectsWithTag("UIPanel"))
        {
            UIPanels.Add(UIPanel.name, UIPanel);

            if (UIPanel.name != "MainMenuPanel") UIPanel.SetActive(false);
        }

        MovingObjects = new List<MovingUIObject>();
        FinishedObjects = new List<MovingUIObject>();

        FadingObjects = new List<FadingUIObject>();
        FinishedFadingObjects = new List<FadingUIObject>();

        State = UIAnimationStates.Default;
	}

    //returns the location which this object should slide to or from
    //when animating screen transitions
    public Vector2 GetObjectSlideTarget(GameObject O)
    {
        float X = O.transform.localPosition.x;
        float Y = O.transform.localPosition.y;

        
        if (Mathf.Abs(X) <= 15)
        {
            //object is horizontally centered, move it vertically
            float NewY;

            if (Y <= 0)
            {
                NewY = -(Screen.height / 2) - (O.transform as RectTransform).rect.height;
            }
            else
            {
                NewY = (Screen.height / 2) + (O.transform as RectTransform).rect.height;
            }

            Y = NewY;
        }
        else
        {
            //object should move horizontally to the side it is closest to
            float NewX;

            if (X <= 0)
            {
                NewX = -(Screen.width / 2) - (O.transform as RectTransform).rect.width;
            }
            else
            {
                NewX = (Screen.width / 2) + (O.transform as RectTransform).rect.width;
            }

            X = NewX;
        }

        return new Vector2(X, Y);
    }

	// Update is called once per frame
	void Update () 
    {
        switch (State)
        {
            case UIAnimationStates.Default: return;
            case UIAnimationStates.ClearingPanel:
                {
                    foreach (MovingUIObject O in new List<MovingUIObject>(MovingObjects))
                    {
                        O.Update();
                        if (O.Completed)
                        {
                            MovingObjects.Remove(O);
                            FinishedObjects.Add(O);
                        }
                    }

                    foreach (FadingUIObject O in new List<FadingUIObject>(FadingObjects))
                    {
                        O.Update();
                        if (O.Completed)
                        {
                            FadingObjects.Remove(O);
                            FinishedFadingObjects.Add(O);
                        }
                    }

                    if (MovingObjects.Count == 0 && FadingObjects.Count == 0)
                    {
                        foreach (MovingUIObject O in FinishedObjects) O.Dispose();
                        FinishedObjects.Clear();

                        foreach (FadingUIObject O in FinishedFadingObjects) O.Dispose();
                        FinishedFadingObjects.Clear();

                        GameObject Panel = UIPanels[PendingNewPanel];
                        foreach (Transform child in Panel.transform)
                        {
                            GameObject ChildObject = child.gameObject;

                            if (ChildObject.tag == "Fade")
                            {
                                FadingObjects.Add(new FadingUIObject(ChildObject, true, 0.1f));
                            }
                            else
                            {
                                Vector2 NewPos = GetObjectSlideTarget(ChildObject);
                                Vector2 Target = new Vector2(ChildObject.transform.localPosition.x, ChildObject.transform.localPosition.y);

                                ChildObject.transform.localPosition = new Vector3(NewPos.x, NewPos.y, 0);
                                MovingObjects.Add(new MovingUIObject(ChildObject, Target, 30f));
                            }
                        }

                        UIPanels[PendingOldPanel].SetActive(false);
                        UIPanels[PendingNewPanel].SetActive(true);

                        State = UIAnimationStates.OpeningPanel;
                    }

                    break;
                }
            case UIAnimationStates.OpeningPanel:
                {
                    foreach (MovingUIObject O in new List<MovingUIObject>(MovingObjects))
                    {
                        O.Update();
                        if (O.Completed)
                        {
                            MovingObjects.Remove(O);
                        }
                    }

                    foreach (FadingUIObject O in new List<FadingUIObject>(FadingObjects))
                    {
                        O.Update();
                        if (O.Completed)
                        {
                            O.Dispose();
                            FadingObjects.Remove(O);
                        }
                    }

                    if (MovingObjects.Count == 0 && FadingObjects.Count == 0)
                    {
                        State = UIAnimationStates.Default;
                    }

                    break;
                }
        }
	}

    public void CloseUIPanel(string PanelName)
    {
        PendingOldPanel = PanelName;

        MovingObjects.Clear();

        GameObject Panel = UIPanels[PanelName];
        foreach (Transform child in Panel.transform)
        {
            GameObject ChildObject = child.gameObject;

            if (ChildObject.tag == "Fade")
            {
                FadingObjects.Add(new FadingUIObject(ChildObject, false, 0.1f));
            }
            else
            {
                Vector2 Target = GetObjectSlideTarget(ChildObject);
                MovingObjects.Add(new MovingUIObject(ChildObject, Target, 30f));
            }
        }

        State = UIAnimationStates.ClearingPanel;
    }

    public void OpenUIPanel(string PanelName)
    {
        PendingNewPanel = PanelName;
    }


    public void OpenFacebook()
    {
         Application.OpenURL("http://illustrious-software.com/thumble_fb.php");
    }

    public void OpenTwitter()
    {
        Application.OpenURL("http://illustrious-software.com/thumble_twitter.php");
    }
}
