﻿using UnityEngine;
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
        //addPrecondition("isTreasureProtected", true);
        addPrecondition("isWerewolfNearby", false);
        //addPrecondition("hasTreasure", false);

        //addEffect("hasTreasure", true);
        addEffect("isTreasureProtected", false);
        addEffect("isEscapeRouteClear", true);
        //addEffect("canEscape", true);
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

        //float dist = 1000;

        GridTile destination = mainChar.hasTreasure ? mainChar.gridLayer.GetTile(mainChar.gridLayer.EscapeLocation) : mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation);

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), destination);

        foreach (var p in path)
        {
            if (p.Passable == false)
            {
                finalTile = p;
                break;
            }
            //nextTile = p;
        }

        if(finalTile == null)
        {
            finalTile = mainChar.gridLayer.GetTile(mainChar.Location);
            nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
        }

        //System.Random rand = new System.Random();
        /*for (int i = 0; i < 4; i++)
        {
            GridTile treasureN = mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.gridLayer.TreasureLocation, i));
            if (treasureN != null)
            {
                if (!treasureN.Passable && (treasureN.Location - mainChar.Location).magnitude < dist)
                {
                    finalTile = treasureN;
                    dist = (treasureN.Location - mainChar.Location).magnitude;
                    //nextTile = treasureN;
                }
            }
        }

        Debug.Log("Dist is " + dist);*/

        path = mainChar.gridLayer.GetBestPathToTile(finalTile, mainChar.gridLayer.GetTile(mainChar.Location));
        if (path.Count > 1)
        {
            nextTile = path[1];
        }
        else
        {
            nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
        }



        //Debug.Log(nextTile.Location);

        if(nextTile!=null)
            target = nextTile.VisualTile.gameObject;

		return nextTile != null;
	}
	
	public override bool perform (GameObject agent)
	{
        if (startTime == 0)
			startTime = Time.time;

        //mainChar.isMoving = true;

        mainChar.MoveToLocation(nextTile.Location);

        //Debug.Log((nextTile.Location - finalTile.Location).magnitude);

        if ((nextTile.Location - finalTile.Location).magnitude <= 1)
        {
        }
        else
        {
            hasMoved = true;
            return false;
        }

        if (Time.time - startTime > workDuration)
        {
            if ((nextTile.Location - finalTile.Location).magnitude <= 1)
            {
                //Debug.Log("Got there");
                mainChar.gridLayer.MakeGrassGrid(finalTile);
                hasMoved = true;
            }
        }
		return true;
	}
}
