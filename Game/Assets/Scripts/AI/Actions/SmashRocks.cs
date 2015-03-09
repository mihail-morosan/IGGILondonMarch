using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmashRocks : GoapAction {
    CharacterBasicBehaviour mainChar;

    GridTile nextTile = null;
    GridTile finalTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public SmashRocks()
    {
        //addPrecondition("isTreasureProtected", true);

        addEffect("isTreasureProtected", false);

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
        //mainChar.isMoving = true;

        nextTile = null;

        System.Random rand = new System.Random();
        //for (int i = 0; i < 4; i++)
        {
            GridTile treasureN = mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.gridLayer.TreasureLocation, rand.Next(4)));
            if (treasureN != null)
            {
                //if (!treasureN.Passable)
                {
                    finalTile = treasureN;
                    //nextTile = treasureN;
                }
            }
        }

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(finalTile, mainChar.gridLayer.GetTile(mainChar.Location));
        if (path.Count > 1)
        {
            //foreach(var x in path)
            //{
            //    Debug.Log(x.Location);
            //}
            //Debug.Log(path);
            nextTile = path[1];
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

        if (Time.time - startTime > workDuration)
        {
            mainChar.MoveToLocation(nextTile.Location);
            hasMoved = true;

            //Build blockade here
            if (nextTile.Equals(finalTile))
                mainChar.gridLayer.MakeGrassGrid(nextTile);
            else
                return false;
        }
		return true;
	}
}
