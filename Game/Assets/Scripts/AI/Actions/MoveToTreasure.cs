using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveToTreasure : GoapAction {
    MainCharacter mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;

    public MoveToTreasure()
    {
        addPrecondition("hasTreasure", false);

        addPrecondition("isTreasureProtected", false);

        addPrecondition("isTreasureAvailable", true);

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
        mainChar.isMoving = true;

        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation), mainChar.gridLayer.GetTile(mainChar.Location));
        if (path.Count > 1)
        {
            nextTile = path[1];
        }
        else
        {

            nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
            if((mainChar.Location - mainChar.gridLayer.TreasureLocation).magnitude <= 1)
            {
                nextTile = mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation);
            }
        }

        if(nextTile!=null)
            target = nextTile.VisualTile.gameObject;

		return nextTile != null;
	}
	
	public override bool perform (GameObject agent)
	{
        if (startTime == 0)
			startTime = Time.time;

        mainChar.MoveToLocation(nextTile.Location);

        if (mainChar.Location.Equals(mainChar.gridLayer.TreasureLocation))
        {
            mainChar.gridLayer.MakeGrassGrid(mainChar.gridLayer.GetTile(mainChar.gridLayer.TreasureLocation));
            mainChar.hasTreasure = true;

            mainChar.gridLayer.TreasureLocation = new Vector2(-1000, -1000);
        }
        else
        {
            hasMoved = true;
        }

		return false;
	}
}
