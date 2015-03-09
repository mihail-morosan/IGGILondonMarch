using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetRocks : GoapAction
{
    CharacterBasicBehaviour mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public GetRocks()
    {
        addPrecondition("hasRocks", false);

        addEffect("hasRocks", true);

    }

    void Start()
    {

        mainChar = GetComponent<CharacterBasicBehaviour>();
    }

    public override void reset()
    {
        nextTile = null;
        hasMoved = false;
        startTime = 0;
    }

    public override bool isDone()
    {
        return hasMoved;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        mainChar.isMoving = true;

        nextTile = null;

        GridTile treasure = mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation);
        float distmin = 1000;
        for(int i = 0;i<mainChar.gridLayer.GetGrid().GetUpperBound(0);i++)
        {
            for (int y = 0; y < mainChar.gridLayer.GetGrid().GetUpperBound(1);i++ )
            {
                GridTile tile = mainChar.gridLayer.GetTile(i, y);
                if (tile != null && tile.TileType.Equals("rock") && (mainChar.Location - tile.Location).magnitude < distmin &&
                    (tile.Location - treasure.Location).magnitude > 1)
                {
                    distmin = (mainChar.Location - tile.Location).magnitude;

                    nextTile = tile;
                }
            }
        }

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), nextTile);
        if (path.Count > 0)
        {
            //foreach(var x in path)
            //{
            //    Debug.Log(x.Location);
            //}
            //Debug.Log(path);
            nextTile = path[path.Count - 1];
        }

        if (nextTile != null)
            target = nextTile.VisualTile.gameObject;

        return nextTile != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        //mainChar.isMoving = true;

        if (Time.time - startTime > workDuration)
        {
            mainChar.MoveToLocation(nextTile.Location);
            hasMoved = true;
            mainChar.isMoving = false;

            //Build blockade here
            if (nextTile.TileType.Equals("rock"))
            {
                mainChar.hasRocks = true;
            }
        }
        return true;
    }
}
