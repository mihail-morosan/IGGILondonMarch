using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CommonBehaviour : MonoBehaviour, IGoap
{
    public GridLayer gridLayer;

    public Vector2 Location;

    public int MoveSpeed = 1;

    public bool isMoving = false;

    int lastDirection = 0;

    public Queue<GoapAction> currentActions;

    /**
     * Key-Value data that will feed the GOAP actions and system while planning.
     */
    public abstract HashSet<KeyValuePair<string, object>> getWorldState();

    public void MoveToLocation(Vector2 Loc)
    {
        if (enabled)
        {
            Location = Loc;
            transform.position = gridLayer.GetGrid()[(int)Location.x, (int)Location.y].VisualTile.transform.position;
        }
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
        //Debug.Log("<color=green>Plan found for " + this.name + "</color> " + GoapAgent.prettyPrint(actions));
        currentActions = actions;
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        //Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        //Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
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

            if (direction == 1)
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
    void Start()
    {
        //gridLayer = GetComponent<GridLayer>();
    }

    public void SetGridLayer(GridLayer gl)
    {
        gridLayer = gl;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving && enabled)
        {
            MoveToLocation(Location);
        }
        else
        {

        }
    }
}
