using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveToEscapeDungeon : GoapAction
{
    MainCharacterDungeon mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;

    public MoveToEscapeDungeon()
    {
        addPrecondition("isWerewolfNearby", false);

        addEffect("canEscape", true);
    }

    void Start()
    {
        mainChar = GetComponent<MainCharacterDungeon>();
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

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), mainChar.gridLayer.GetTile(mainChar.gridLayer.EscapeLocation));
        if (path.Count > 0)
        {
            nextTile = path[path.Count - 1];
        }
        else
        {
            nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
        }

        if (nextTile != null)
            target = nextTile.VisualTile.gameObject;

        return nextTile != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (nextTile.Passable)
            mainChar.MoveToLocation(nextTile.Location);
        else
            return false;

        hasMoved = true;

        return false;
    }
}
