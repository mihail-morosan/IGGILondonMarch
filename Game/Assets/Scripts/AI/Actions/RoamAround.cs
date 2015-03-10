using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoamAround : GoapAction {
    CharacterBasicBehaviour mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public RoamAround()
    {
		//addPrecondition ("isWerewolfNearby", false); // we need a tool to do this

		addEffect ("isHumanNearby", true);

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
		//TODO
        mainChar.isMoving = true;

        nextTile = null; //nextTile = mainChar.gridLayer.GetGrid()[(int)mainChar.Location.x + 1, (int)mainChar.Location.y];
        System.Random rand = new System.Random();
        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.Location, rand.Next(4))));
        if (path.Count > 0)
        {
            nextTile = path[path.Count - 1];
            if (nextTile.Passable == false)
                nextTile = null;
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
            mainChar.isMoving = false;
        }
		return true;
	}
}
