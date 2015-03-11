using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackEnemyDungeon : GoapAction
{
    CharacterBasicBehaviourDungeon mainChar;

    GridTile nextTile = null;

    bool hasMoved = false;
    private float startTime = 0;

    public AttackEnemyDungeon()
    {
        addPrecondition("isHumanNearby", true);
        addPrecondition("isWerewolfNearby", true);

        addEffect("isWerewolfNearby", false);
        addEffect("isHumanNearby", false);
        addEffect("isHumanDead", true);
    }

    void Start()
    {
        mainChar = GetComponent<CharacterBasicBehaviourDungeon>();
    }

    public override void reset()
    {
        nextTile = null;
        hasMoved = false;
        startTime = 0;
    }

    public override bool isDone()
    {
        return hasMoved;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        mainChar.isMoving = true;

        if (mainChar.closestEnemy == null)
            return false;
        
        List<GridTile> path = mainChar.gridLayer.GetBestPathToTile(mainChar.gridLayer.GetTile(mainChar.Location), mainChar.gridLayer.GetTile(mainChar.closestEnemy.Location));
        if (path.Count > 1)
        {
            nextTile = path[path.Count - 1];
        }
        else
        {
            nextTile = mainChar.gridLayer.GetTile(mainChar.Location);
        }

        if (nextTile != null && !nextTile.Passable)
            nextTile = null;

        if (nextTile != null)
            target = nextTile.VisualTile.gameObject;

        return nextTile != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if(mainChar.Health <= 0)
        {
            hasMoved = true;
            return false;
        }

        if ((mainChar.Location - mainChar.closestEnemy.Location).magnitude <= 1)
        {
            if (Time.time - startTime > mainChar.AttackTime)
            {
                //if ((mainChar.Location - mainChar.closestEnemy.Location).magnitude <= 1)
                if(mainChar.closestEnemy.Health > 0)
                {
                    mainChar.gridLayer.MakeEffect(mainChar.gridLayer.GetTile(mainChar.closestEnemy.Location), "fire");

                    mainChar.closestEnemy.Health -= mainChar.Attack;
                }
                hasMoved = true;

                return false;
            }
            return true;
        }
        else
        {
            if (nextTile.Passable)
                mainChar.MoveToLocation(nextTile.Location);
            else
                return false;

            hasMoved = true;
            return false;
        }
    }
}
