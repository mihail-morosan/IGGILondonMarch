﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildBlockade : GoapAction {
    CharacterBasicBehaviour mainChar;

    GridTile nextTile = null;
    GridTile finalTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public BuildBlockade()
    {
        //addPrecondition("hasRocks", true);

        addPrecondition("hasTreasure", false);
        addPrecondition("isTreasureProtected", false);

		addEffect ("isTreasureProtected", true);

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

        nextTile = null;

        for (int i = 0; i < 4; i++)
        {
            GridTile treasureN = mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.gridLayer.TreasureLocation, i));
            if (treasureN != null)
            {
                if (treasureN.Passable)
                {
                    finalTile = treasureN;
                    //nextTile = treasureN;
                }
            }
        }

        if(finalTile == null || mainChar.Location.Equals(finalTile.Location))
        {
            nextTile = finalTile;
            if(finalTile != null)
                target = nextTile.VisualTile.gameObject;
            return finalTile != null;
        }

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), finalTile);
        if (path.Count > 0)
        {
            //foreach(var x in path)
            //{
            //    Debug.Log(x.Location);
            //}
            //Debug.Log(path);
            nextTile = path[path.Count - 1];
        }

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
        
        mainChar.isMoving = false;

        //Build blockade here
        if (nextTile.Equals(finalTile))
        {
            //mainChar.gridLayer.MakeRockGrid(nextTile);
            //mainChar.hasRocks = false;
        }
        else
            hasMoved = true;

        if (Time.time - startTime > workDuration)
        {
            if (nextTile.Equals(finalTile))
            {
                mainChar.gridLayer.MakeRockGrid(nextTile);
                mainChar.hasRocks = false;
            }
            hasMoved = true;
        }
		return true;
	}
}
