using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum CandyType
{
    GREEN,
    RED,
    BLUE,
    EMPTY
}

public class Candy
{
    const int ColumnLength = Board.ColumnLength, RowLength = Board.RowLength;
    const float Delay = Board.Delay;
    CandyType type;
    public Button button;

    public Candy(Button button, CandyType type)
    {
        this.type = type;
        this.button = button;
        UpdateType(type);
        // Debug.Log(this.button.transform.position);
    }

    public void UpdateType(CandyType type)
    {
        this.type = type;
        int id = Array.IndexOf(Enum.GetValues(type.GetType()), type);

        string s;
        if (id == 0) s = "green";
        else if (id == 1) s = "red";
        else if (id == 2) s = "blue";
        else s = "blank";
        s = "candy_" + s;

        if (type == CandyType.EMPTY) this.button.transform.position = new Vector3(this.button.transform.position.x, this.button.transform.position.y, -100);
        else this.button.transform.position = new Vector3(this.button.transform.position.x, this.button.transform.position.y, 100);
        // Debug.Log((temp == null).ToString() + " " + s);
        this.button.GetComponent<Image>().sprite = (Sprite)Resources.Load(s, typeof(Sprite));
    }

    public void CreateType()
    {
        int x = UnityEngine.Random.Range(0, 3);
        this.UpdateType((CandyType)x);
    }

    public void SetButton(int a, Tuple<float, float> pos, bool TweenOrNot)
    {
        this.button.name = a.ToString();
        float y = pos.Item1, x = pos.Item2;
        if (TweenOrNot == false) this.button.transform.position = new Vector3(x, y, this.button.transform.position.z);
        else LeanTween.move(this.button.gameObject, new Vector3(x, y, this.button.transform.position.z), Delay / 2);
    }

    public CandyType GetCandyType()
    {
        return this.type;
    }
}
