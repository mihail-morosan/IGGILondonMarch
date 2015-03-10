using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class CharacterBasicBehaviour : MonoBehaviour, IGoap {

    public GridLayer gridLayer;

    public Vector2 Location;

    public int MoveSpeed = 100;

    public bool isMoving = false;

    public bool hasTreasure = false;

    public bool hasRocks = false;

    public bool isBad = false;

    int lastDirection = 0;

    /**
     * Key-Value data that will feed the GOAP actions and system while planning.
     */
    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        MainCharacter mainChar = FindObjectOfType<MainCharacter>();
        DruidBehaviour druidChar = FindObjectOfType<DruidBehaviour>();
        WerewolfBehaviour wereChar = FindObjectOfType<WerewolfBehaviour>();

        worldData.Add(new KeyValuePair<string, object>("hasTreasure", mainChar.hasTreasure));

        worldData.Add(new KeyValuePair<string, object>("canEscape", hasTreasure && Location == gridLayer.EscapeLocation));

        bool isEscapeRouteClear = true;

        List<GridTile> path = gridLayer.GetBestPathToTile(gridLayer.GetTile(mainChar.Location), gridLayer.GetTile(gridLayer.EscapeLocation));

        foreach (var p in path)
        {
            if (p.Passable == false)
            {
                isEscapeRouteClear = false;
                break;
            }
        }

        worldData.Add(new KeyValuePair<string, object>("isEscapeRouteClear", isEscapeRouteClear)); 

        bool isTreasureProtected = false;

        path = gridLayer.GetBestPathToTile(gridLayer.GetTile(mainChar.Location), gridLayer.GetTile(gridLayer.TreasureLocation));

        foreach(var p in path)
        {
            if(p.Passable == false)
            {
                isTreasureProtected = true;
                break;
            }
        }

        FindObjectOfType<Text>().text = "Path is clear: " + isEscapeRouteClear;
        
        worldData.Add(new KeyValuePair<string, object>("isTreasureProtected", isTreasureProtected));

        worldData.Add(new KeyValuePair<string, object>("hasRocks", hasRocks));

        worldData.Add(new KeyValuePair<string, object>("isHumanNearby", isBad && (mainChar.Location - Location).magnitude < 2.5f));

        worldData.Add(new KeyValuePair<string, object>("isWerewolfNearby", !isBad && wereChar != null && (wereChar.Location - Location).magnitude < 2.5f));

        

        return worldData;
    }

    public void MoveToLocation(Vector2 Loc)
    {
        Location = Loc;
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

        Animator animator = GetComponent<Animator>();

        if (animator != null)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = 1;

            int direction = lastDirection;
            Vector3 dir = nextAction.target.transform.position - gameObject.transform.position;
            if (dir.x > 0)
            {
                direction = 1;
            }
            if (dir.x < 0)
                direction = 3;
            if (dir.y > 0)
                direction = 0;
            if (dir.y < 0)
                direction = 2;

            if(direction == 1)
                theScale.x *= -1; ;

            animator.SetInteger("Direction", direction);
            lastDirection = direction;

            transform.localScale = theScale;
        }

        if ((gameObject.transform.position - nextAction.target.transform.position).magnitude == 0)
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
