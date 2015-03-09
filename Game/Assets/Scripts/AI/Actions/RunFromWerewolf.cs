using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunFromWerewolf : GoapAction {
    CharacterBasicBehaviour mainChar;

    WerewolfBehaviour wereChar;
    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;
    public float workDuration = 1;

    public RunFromWerewolf()
    {
		addPrecondition ("isWerewolfNearby", true);

        addEffect("isWerewolfNearby", false);

	}
	
    void Start()
    {
        wereChar = FindObjectOfType<WerewolfBehaviour>();
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

        if (wereChar == null)
            return true;

        mainChar.isMoving = true;
 
                float CurrentDistance = (wereChar.Location - mainChar.Location).magnitude;

                System.Random rand = new System.Random();

                List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), mainChar.gridLayer.GetTile(GridLayer.GetNeighbour(mainChar.Location, rand.Next(4))));
                if (path.Count > 0)
                {
                    nextTile = path[path.Count - 1];
                    float NewDistance = (wereChar.Location - nextTile.Location).magnitude;

                    if (NewDistance <= CurrentDistance)
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
            nextTile = null;

            return false;
        }
		return true;
	}
}
