using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Board : MonoBehaviour
{
    public const int ColumnLength = 6, RowLength = 6;
    public const float Delay = 1f;
    public Candy[,] Candies = new Candy[ColumnLength + 5, RowLength + 5];
    Candy[] TempCandies = new Candy[RowLength + 5];

    const float ButtonLength = 2500f / 2595f * 1.6f;
    [SerializeField] GameObject ButtonPrefab, MyCanvas, CandyChosenHandler;
    [SerializeField] Sprite[] CandyImages;

    Tuple<float, float> GetButtonPosition(int a)
    {
        int i = Find(a).Item1, j = Find(a).Item2;
        int mc = (ColumnLength - 1) / 2 + 1, mr = (RowLength - 1) / 2 + 1;

        float x = 0, y = 0;
        if (ColumnLength % 2 == 0)
        {
            if (i <= mc) x = ButtonLength / 2 + ButtonLength * (mc - i);
            else x = -ButtonLength / 2 - ButtonLength * (i - mc - 1);
        }
        else x = ButtonLength * (mc - i);
        if (RowLength % 2 == 0)
        {
            if (j <= mr) y = ButtonLength / 2 + ButtonLength * (mr - j);
            else y = -ButtonLength / 2 - ButtonLength * (j - mr - 1);
        }
        else y = ButtonLength * (mr - j);

        return Tuple.Create(x, y);
    }
    void Awake()
    {
        int[,] SpritesIndex = new int[RowLength + 5, ColumnLength + 5];
        for (int i = 1; i <= ColumnLength; i++)
            for (int j = 1; j <= RowLength; j++)
            {
                float x = GetButtonPosition(Compress(i, j)).Item2,
                      y = GetButtonPosition(Compress(i, j)).Item1;

                // Debug.Log(i.ToString() + " " + j.ToString() + " " + x.ToString() + " " + y.ToString());
                // Debug.Log(TempButton.transform.position);
                GameObject TempButton = UnityEngine.Object.Instantiate(ButtonPrefab, new Vector3(x, y, 0f), new Quaternion(0, 0, 0, 0), MyCanvas.transform) as GameObject;
                TempButton.name = ((i - 1) * RowLength + j).ToString();
                TempButton.GetComponent<Button>().onClick.AddListener(() => { CandyChosenHandler.GetComponent<CandyChosenHandler>().SquareChoseHandler(); });

                int bl1 = -1, bl2 = -1;
                if (i > 2)
                {
                    if (SpritesIndex[i - 1, j] == SpritesIndex[i - 2, j]) bl1 = SpritesIndex[i - 1, j];
                }
                if (j > 2)
                {
                    if (SpritesIndex[i, j - 1] == SpritesIndex[i, j - 2]) bl2 = SpritesIndex[i, j - 1];
                }

                while (true)
                {
                    int xx = UnityEngine.Random.Range(0, 3);
                    if (xx != bl1 && xx != bl2)
                    {
                        SpritesIndex[i, j] = xx;
                        // Debug.Log(i.ToString() + " " + j.ToString());
                        Candies[i, j] = new Candy(TempButton.GetComponent<Button>(), (CandyType)xx);
                        break;
                    }
                }
            }
    }

    Tuple<int, int> Find(int a)
    {
        return Tuple.Create((a - 1) / RowLength + 1, (a - 1) % RowLength + 1);
    }

    int Compress(int a, int b)
    {
        return (a - 1) * RowLength + b;
    }

    bool AdjacentCheck(int a, int b)
    {
        if (a > b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }
        if (a + 6 == b) return true;
        if (a + 1 == b && a % 6 != 0) return true;
        return false;
    }

    void SwapCandyOnly(int a, int b)
    {
        var p = Find(a);
        var q = Find(b);

        var tmp = Candies[p.Item1, p.Item2];
        Candies[p.Item1, p.Item2] = Candies[q.Item1, q.Item2];
        Candies[q.Item1, q.Item2] = tmp;
    }
    void ClearCandy(int x, int y)
    {
        Candies[x, y].UpdateType(CandyType.EMPTY);
    }

    IEnumerator CreateCandy(int create_x1, int create_y1, int create_x2, int create_y2, float f)
    {
        yield return new WaitForSeconds(f);
        for (int i = create_x1; i <= create_x2; i++)
            for (int j = create_y1; j <= create_y2; j++) {
            Candies[i, j].CreateType();
        }

        for (int i = 1; i <= ColumnLength; i++)
            for (int j = 1; j <= RowLength; j++) Debug.Log(i.ToString() + " " + j.ToString() + " " + Candies[i, j].button.name);
    }

    IEnumerator Swapper(int a, int b, float f)
    {
        yield return new WaitForSeconds(f);
        SwapCandy(a, b);
    }

    IEnumerator DelaySetButton(int i, int j, int a, bool TweenOrNot, float f)
    {
        yield return new WaitForSeconds(f);
        Candies[i, j].SetButton(a, GetButtonPosition(a), TweenOrNot);
    }

    IEnumerator DelaySwapCandyOnly(int a, int b, float f)
    {
        yield return new WaitForSeconds(f);
        var p = Find(a);
        var q = Find(b);

        var tmp = Candies[p.Item1, p.Item2];
        Candies[p.Item1, p.Item2] = Candies[q.Item1, q.Item2];
        Candies[q.Item1, q.Item2] = tmp;
    }

    IEnumerator CalibrateHorizontalMatch(int x, int y1, int y2, float f)
    {
        yield return new WaitForSeconds(f);
        for (int i = x; i >= 2; i--)
            for (int j = y1; j <= y2; j++)
                SwapCandyOnly(Compress(i, j), Compress(i - 1, j));
    }

    void HorizontalMatch(int x, int y1, int y2)
    {
        for (int j = y1; j <= y2; j++) ClearCandy(x, j);

        for (int i = 1; i < x; i++)
            for (int j = y1; j <= y2; j++)
                StartCoroutine(DelaySetButton(i, j, Compress(i + 1, j), true, Delay * 0.5f));
        for (int j = y1; j <= y2; j++)
            StartCoroutine(DelaySetButton(x, j, Compress(1, j), false, Delay * 1f));

        StartCoroutine(CalibrateHorizontalMatch(x, y1, y2, Delay * 1.25f));
        StartCoroutine(CreateCandy(1, y1, 1, y2, Delay * 1.5f));
    }

    IEnumerator CalibrateVerticalMatch(int y, int x1, int x2, float f)
    {
        yield return new WaitForSeconds(f);
        for (int i = x1 - 1; i >= 1; i--)
            SwapCandyOnly(Compress(i, y), Compress(i + (x2 - x1 + 1), y));
    }

    void VerticalMatch(int y, int x1, int x2)
    {
        for (int i = x1; i <= x2; i++) ClearCandy(i, y);

        for (int i = x1 - 1; i >= 1; i--)
            StartCoroutine(DelaySetButton(i, y, Compress(i + (x2 - x1 + 1), y), true, Delay * 0.5f));
        for (int i = x1; i <= x2; i++)
            StartCoroutine(DelaySetButton(i, y, Compress(i - x1 + 1, y), false, Delay * 1.1f));

        StartCoroutine(CalibrateVerticalMatch(y, x1, x2, Delay * 1.25f));
        StartCoroutine(CreateCandy(1, y, x2 - x1 + 1, y, Delay * 1.5f));
    }

    bool InSwap = false;
    bool MatchThree()
    {
        InSwap = true;
        int match_max = -1, match_type = -1, match_i = 0, match_j = 0;
        for (int i = 1; i <= ColumnLength; i++)
        {
            int cnt = 1;
            for (int j = 2; j <= RowLength; j++)
            {
                if (Candies[i, j].GetCandyType() == Candies[i, j - 1].GetCandyType()) cnt++;
                else
                {
                    if (cnt >= 3 && cnt > match_max)
                    {
                        match_max = cnt;
                        match_type = 0;
                        match_i = i;
                        match_j = j;
                        //HorizontalMatch(i, j - cnt, j - 1);
                        //Invoke("MatchThree", Delay * (i - 1) + Delay * 2);
                        //return true;
                    }
                    cnt = 1;
                }
            }

            if (cnt >= 3 && cnt > match_max)
            {
                match_max = cnt;
                match_type = 0;
                match_i = i;
                match_j = RowLength + 1;
                //HorizontalMatch(i, RowLength + 1 - cnt, RowLength);
                //Invoke("MatchThree", Delay * (i - 1) + Delay * 2);
                //return true;
            }
        }

        for (int j = 1; j <= RowLength; j++)
        {
            int cnt = 1;
            for (int i = 2; i <= ColumnLength; i++)
            {
                if (Candies[i, j].GetCandyType() == Candies[i - 1, j].GetCandyType()) cnt++;
                else
                {
                    if (cnt >= 3 && cnt > match_max)
                    {
                        match_max = cnt;
                        match_type = 1;
                        match_i = i;
                        match_j = j;
                        //VerticalMatch(j, i - cnt, i - 1);
                        //Invoke("MatchThree", Delay * (i - cnt - 1) + Delay * 2);
                        //return true;
                    }
                    else cnt = 1;
                }
            }

            if (cnt >= 3 && cnt > match_max)
            {
                match_max = cnt;
                match_type = 1;
                match_i = ColumnLength + 1;
                match_j = j;
                //VerticalMatch(j, ColumnLength + 1 - cnt, ColumnLength);
                //Invoke("MatchThree", Delay * (ColumnLength - cnt) + Delay * 2);
                //return true;
            }
        }

        if (match_type == -1)
        {
            InSwap = false;
            return false;
        }
        else if (match_type == 0)
        {
            HorizontalMatch(match_i, match_j - match_max, match_j - 1);
            Invoke("MatchThree", Delay * 200f);
            return true;
        }
        else
        {
            VerticalMatch(match_j, match_i - match_max, match_i - 1);
            Invoke("MatchThree", Delay * 200f);
            return true;
        }
    }

    void SwapCandy(int a, int b)
    {
        SwapCandyOnly(a, b);

        var p = Find(a);
        var q = Find(b);
        Candies[p.Item1, p.Item2].SetButton(a, GetButtonPosition(a), true);
        Candies[q.Item1, q.Item2].SetButton(b, GetButtonPosition(b), true);
    }

    IEnumerator StartMatchThree(int a, int b)
    {
        yield return new WaitForSeconds(Delay / 2);
        if (!MatchThree()) SwapCandy(a, b);
    }

    public void SwapCandyCaller(int a, int b)
    {
        if (AdjacentCheck(a, b) == false || InSwap) return;
        SwapCandy(a, b);
        StartCoroutine(StartMatchThree(a, b));
    }
}