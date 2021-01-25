using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    //Variables:
    public bool FindDistance = false;
    public int rows = 27;
    public int cols = 36;
    public int scale = 1;
    public GameObject GridPrefab;
    public Vector3 LeftBottomLocation = new Vector3(0, 0, 0);
    public GameObject[,] GridArray;
    public int StartX = 0;
    public int StartY = 0;
    public int EndX = 2;
    public int EndY = 2;
    public List<GameObject> Path = new List<GameObject>();

    //Awake
    void Awake()
    {
        GridArray = new GameObject[cols, rows];

        if (GridPrefab)
            GenerateGrid();
        else
            print("Missing GridPrefab, Please Assign.");
    }

    //Update
    void Update()
    {
        if (FindDistance)
        {
            SetDistance();
            SetPath();
            FindDistance = false;
        }
    }

    //--FUNCTIONS---

    //Generate Grid Function
    void GenerateGrid()
    {
        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject obj = Instantiate(GridPrefab, new Vector3(LeftBottomLocation.x + scale * i, LeftBottomLocation.y, LeftBottomLocation.z + scale * j), Quaternion.identity);
                obj.transform.SetParent(gameObject.transform);
                obj.GetComponent<GridStat>().x = i;
                obj.GetComponent<GridStat>().y = j;
                //print("hi");
                GridArray[i, j] = obj;
            }
        }
    }
    //Sets the Distance

    void SetDistance()
    {
        InitialSetup();
        int x = StartX;
        int y = StartY;
        int[] testArray = new int[rows * cols];

        for (int step = 1; step < rows * cols; step++)
        {
            foreach (GameObject obj in GridArray)
            {
                if (obj && obj.GetComponent<GridStat>().visited == step - 1)
                    TestFourDirections(obj.GetComponent<GridStat>().x, obj.GetComponent<GridStat>().y, step);
            }
        }
    }

    //Sets the Paths
    void SetPath()
    {
        
        int Step;
        int x = EndX;
        int y = EndY;
        List<GameObject> TempList = new List<GameObject>();

        Path.Clear();

        if (GridArray[EndX, EndY] && GridArray[EndX, EndY].GetComponent<GridStat>().visited > 0)
        {
            Path.Add(GridArray[x, y]);
            Step = GridArray[x, y].GetComponent<GridStat>().visited - 1;
        }
        else
        {
            print("Can't reach the desired location.");
            return;
        }

        for (int i = Step; Step > -1; Step--)
        {
            try {
                if (TestDirection(x, y, Step, 1))
                    TempList.Add(GridArray[x, y + 1]);
                if (TestDirection(x, y, Step, 2))
                    TempList.Add(GridArray[x + 1, y]);
                if (TestDirection(x, y, Step, 3))
                    TempList.Add(GridArray[x, y - 1]);
                if (TestDirection(x, y, Step, 4))
                    TempList.Add(GridArray[x - 1, y]);


                GameObject tempObj = FindClosest(GridArray[EndX, EndY].transform, TempList);
                tempObj.GetComponent<Renderer>().material.color = new Color(25f, 25f, 25f);
                Path.Add(tempObj);
                x = tempObj.GetComponent<GridStat>().x;
                y = tempObj.GetComponent<GridStat>().y;
                TempList.Clear();
            }
            catch
            {
                print("Overflow");
                FindDistance = false;

            }
            

        }


    }

    //Setup
    void InitialSetup()
    {
        foreach (GameObject obj in GridArray)
        {
            obj.GetComponent<GridStat>().visited = -1;
        }
        GridArray[StartX, StartY].GetComponent<GridStat>().visited = 0;
    }

    //Tests for a tile in one direction
    bool TestDirection(int x, int y, int step, int direction)
    {
        //Int Direction tells which case to use: 1 is up, 2 is right, 3 is down, 4 is left.
        switch (direction)
        {
            case 1:
                if (y + 1 < rows && GridArray[x, y + 1] && GridArray[x, y + 1].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 2:
                if (x + 1 < cols && GridArray[x + 1, y] && GridArray[x + 1, y].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 3:
                if (y - 1 < -1 && GridArray[x, y - 1] && GridArray[x, y - 1].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
            case 4:
                if (x - 1 > -1 && GridArray[x - 1, y] && GridArray[x - 1, y].GetComponent<GridStat>().visited == step)
                    return true;
                else
                    return false;
        }
        return false;
    }

    //Tests for a tile in all four directions
    void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, 1))
            SetVisited(x, y + 1, step);
        if (TestDirection(x, y, -1, 2))
            SetVisited(x + 1, y, step);
        if (TestDirection(x, y, -1, 3))
            SetVisited(x, y - 1, step);
        if (TestDirection(x, y, -1, 4))
            SetVisited(x - 1, y, step);
    }

    //Sets if a tile has been visited
    void SetVisited(int x, int y, int step)
    {
        if (GridArray[x, y])
        {
            GridArray[x, y].GetComponent<GridStat>().visited = step;
        }
    }

    //Finds the most optimal path
    GameObject FindClosest(Transform targetLocation, List<GameObject> list)
    {
        float CurrentDistance = scale * rows * cols;
        int IndexNumber = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < CurrentDistance)
            {
                CurrentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                IndexNumber = i;
            }
        }
        return list[IndexNumber];
    }

}

//https://www.youtube.com/watch?v=MAbei7eMlXg
