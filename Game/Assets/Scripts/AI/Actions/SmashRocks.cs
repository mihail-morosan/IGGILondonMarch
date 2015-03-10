using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmashRocks : GoapAction {
    CharacterBasicBehaviour mainChar = null;

    GridTile nextTile = null;
    GridTile finalTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public SmashRocks()
    {
        addPrecondition("isWerewolfNearby", false);

        addEffect("isTreasureProtected", false);
        addEffect("isEscapeRouteClear", true);
	}
	
    void Start()
    {

        mainChar = GetComponent<CharacterBasicBehaviour>();
    }
	
	public override void reset ()
	{
        nextTile = null;
        hasMoved = false;
        startTime = 0;
        finalTile = null;
	}
	
	public override bool isDone ()
	{
        return hasMoved;
	}
	
	public override bool requiresInRange ()
	{
		return true;
	}
	
	public override bool checkProceduralPrecondition (GameObject agent)
	{
        mainChar.isMoving = true;

        GridTile destination = mainChar.hasTreasure ? mainChar.gridLayer.GetTile(mainChar.gridLayer.EscapeLocation) : mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation);

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(destination,mainChar.gridLayer.GetTile(mainChar.Location));

        foreach (var p in path)
        {
            if (p.Passable == false)
            {
                finalTile = p;
                break;
            }
        }

        if(path.Count == 1)
        {
            //finalTile = destination;
        }

        if(finalTile == null)
        {
            //finalTile = mainChar.gridLayer.GetTile(mainChar.Location);
            //nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
            return false;
        }

        path = mainChar.gridLayer.GetBestPathToTile(finalTile, mainChar.gridLayer.GetTile(mainChar.Location));
        if (path.Count > 1)
        {
            nextTile = path[1];
            
        }
        else
        {
            nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
        }


        if (nextTile == null || !nextTile.Passable)
            nextTile = null;

        if(nextTile!=null)
            target = nextTile.VisualTile.gameObject;

		return nextTile != null;
	}
	
	public override bool perform (GameObject agent)
	{
        if (startTime == 0)
			startTime = Time.time;

        mainChar.MoveToLocation(nextTile.Location);

        if ((nextTile.Location - finalTile.Location).magnitude <= 1)
        {
            if (Time.time - startTime > workDuration)
            {
                if (finalTile.TileType.Equals("rock"))
                {
                    mainChar.gridLayer.MakeGrassGrid(finalTile);
                    
                }
                hasMoved = true;

                mainChar.gridLayer.MakeEffect(finalTile, "fire");

                return true;
            }
            return true;
        }
        else
        {
            hasMoved = true;
            return false;
        }
	}
}
