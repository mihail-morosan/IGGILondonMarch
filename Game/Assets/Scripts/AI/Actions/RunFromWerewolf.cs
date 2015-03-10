using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunFromWerewolf : GoapAction {
    CharacterBasicBehaviour mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;

    public RunFromWerewolf()
    {
		addPrecondition ("isWerewolfNearby", true);

        addEffect("isWerewolfNearby", false);

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
        WerewolfBehaviour wereChar = null;
        float dist = 10000;
        foreach (WerewolfBehaviour w in FindObjectsOfType<WerewolfBehaviour>())
        {
            if ((mainChar.Location - w.Location).magnitude < dist)
            {
                wereChar = w;
                dist = (mainChar.Location - w.Location).magnitude;
            }
        }

        if (wereChar == null)
        {
            target = mainChar.gameObject;
            return true;
        }

        mainChar.isMoving = true;
 
        float CurrentDistance = dist;

        System.Random rand = new System.Random();

        //List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.Location, rand.Next(4))));
        //if (path.Count > 1)
        {
            //nextTile = path[path.Count - 1];
            nextTile = mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.Location, rand.Next(4)));
            if (nextTile == null)
                return false;
            float NewDistance = (wereChar.Location - nextTile.Location).magnitude;

            if (NewDistance <= CurrentDistance || !nextTile.Passable)
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

        if (nextTile.Passable)
            mainChar.MoveToLocation(nextTile.Location);

        hasMoved = true;

		return false;
	}
}
