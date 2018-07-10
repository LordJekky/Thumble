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

class MovingUIObject
{
    GameObject UIObject;
    float TargetY;
    float Speed;

    Vector3 OriginalPosition;

    public bool Completed;

    public MovingUIObject(GameObject UIObject, float TargetY, float Speed)
    {
        this.UIObject = UIObject;
        this.TargetY = TargetY;
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

        if (Pos.y < TargetY)
        {
            Pos.y += Speed;
            if (Pos.y > TargetY) Pos.y = TargetY;
        }
        else if (Pos.y > TargetY)
        {
            Pos.y -= Speed;
            if (Pos.y < TargetY) Pos.y = TargetY;
        }

        UIObject.transform.localPosition = Pos;

        if (Pos.y == TargetY) Completed = true;
    }
}

public class UIBehaviour : MonoBehaviour 
{
    List<MovingUIObject> MovingObjects;
    List<MovingUIObject> FinishedObjects;

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

        State = UIAnimationStates.Default;
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

                    if (MovingObjects.Count == 0)
                    {
                        foreach (MovingUIObject O in FinishedObjects) O.Dispose();
                        FinishedObjects.Clear();

                        GameObject Panel = UIPanels[PendingNewPanel];
                        foreach (Transform child in Panel.transform)
                        {
                            GameObject ChildObject = child.gameObject;

                            float Y = ChildObject.transform.localPosition.y;
                            float NewY;

                            if (Y <= 0)
                            {
                                NewY = -(Screen.height / 2) - (ChildObject.transform as RectTransform).rect.height;
                            }
                            else
                            {
                                NewY = (Screen.height / 2) + (ChildObject.transform as RectTransform).rect.height;
                            }

                            Vector3 Pos = ChildObject.transform.localPosition;
                            Pos.y = NewY;
                            ChildObject.transform.localPosition = Pos;

                            MovingObjects.Add(new MovingUIObject(ChildObject, Y, 30f));
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

                    if (MovingObjects.Count == 0)
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

            float Y = ChildObject.transform.localPosition.y;
            if (Y <= 0)
            {
                Y = -(Screen.height/2) - (ChildObject.transform as RectTransform).rect.height;
            }
            else
            {
                Y = (Screen.height/2) + (ChildObject.transform as RectTransform).rect.height;
            }

            MovingObjects.Add(new MovingUIObject(ChildObject, Y, 30f));
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
