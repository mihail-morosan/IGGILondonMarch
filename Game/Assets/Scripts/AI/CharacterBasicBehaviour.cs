using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharacterBasicBehaviour : MonoBehaviour, IGoap {

    public GridLayer gridLayer;

    public Vector2 Location;

    public int MoveSpeed = 100;

    public bool isMoving = false;

    public bool hasTreasure = false;

    public bool hasRocks = false;

    public bool isBad = false;

    /**
     * Key-Value data that will feed the GOAP actions and system while planning.
     */
    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        //worldData.Add(new KeyValuePair<string, object>("hasOre", (backpack.numOre > 0)));

        MainCharacter mainChar = FindObjectOfType<MainCharacter>();
        DruidBehaviour druidChar = FindObjectOfType<DruidBehaviour>();
        WerewolfBehaviour wereChar = FindObjectOfType<WerewolfBehaviour>();

        worldData.Add(new KeyValuePair<string, object>("hasTreasure", hasTreasure));
        worldData.Add(new KeyValuePair<string, object>("canEscape", hasTreasure && Location == gridLayer.EscapeLocation));

        //And some other stuff
        bool isTreasureProtected = !gridLayer.TreasureStolen;
        worldData.Add(new KeyValuePair<string, object>("isTreasureProtected", isTreasureProtected));

        //Has rocks
        worldData.Add(new KeyValuePair<string, object>("hasRocks", hasRocks));

        worldData.Add(new KeyValuePair<string, object>("canBuildRockBlockade", hasRocks && !isTreasureProtected));

        worldData.Add(new KeyValuePair<string, object>("isHumanNearby", isBad && (mainChar.Location - Location).magnitude < 3));

        worldData.Add(new KeyValuePair<string, object>("isWerewolfNearby", !isBad && (wereChar.Location - Location).magnitude < 3));

        

        return worldData;
    }

    public void MoveToLocation(Vector2 Loc)
    {
        //TODO check if target is ok
        Location = Loc;
        //Debug.Log(gridLayer.GetGrid()[(int)Location.x, (int)Location.y]);
        transform.position = gridLayer.GetGrid()[(int)Location.x, (int)Location.y].VisualTile.transform.position;
    }

    /**
     * Implement in subclasses
     */
    public abstract HashSet<KeyValuePair<string, object>> createGoalState();


    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target
        float step = MoveSpeed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);

        if ((gameObject.transform.position - nextAction.target.transform.position).magnitude < 0.1f)
        {
            // we are at the target location, we are done
            gameObject.transform.position = nextAction.target.transform.position;
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }

	// Use this for initialization
	void Start () {
        gridLayer = GetComponent<GridLayer>();
        //MoveToLocation(Location);
	}
	
	// Update is called once per frame
	void Update () {
        if(!isMoving)
        {
            MoveToLocation(Location);
        }
        else
        {

        }
	}
}
