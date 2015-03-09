﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveToTreasure : GoapAction {
    MainCharacter mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public MoveToTreasure()
    {
        addPrecondition("isTreasureProtected", false);
		addPrecondition ("isWerewolfNearby", false);

		addEffect ("hasTreasure", true);

	}
	
    void Start()
    {

        mainChar = GetComponent<MainCharacter>();
    }
	
	public override void reset ()
	{
        nextTile = null;
        hasMoved = false;
        startTime = 0;
	}
	
	public override bool isDone ()
	{
        return mainChar.hasTreasure || hasMoved;
	}
	
	public override bool requiresInRange ()
	{
		return true;
	}
	
	public override bool checkProceduralPrecondition (GameObject agent)
	{
		//TODO
        mainChar.isMoving = true;

        nextTile = null; //nextTile = mainChar.gridLayer.GetGrid()[(int)mainChar.Location.x + 1, (int)mainChar.Location.y];

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation));
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

        if (Time.time - startTime > workDuration)
        {
            mainChar.MoveToLocation(nextTile.Location);
            hasMoved = true;
            //mainChar.isMoving = false;

            if(mainChar.Location.Equals(mainChar.gridLayer.TreasureLocation))
            {
                mainChar.hasTreasure = true;
            }
            return false;
        }
		return true;
	}
}
