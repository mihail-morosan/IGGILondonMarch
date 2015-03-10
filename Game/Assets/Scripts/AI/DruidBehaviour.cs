using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DruidBehaviour : CharacterBasicBehaviour
{
    void Start()
    {
        this.isBad = false;
    }

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("isTreasureProtected", true));
        goal.Add(new KeyValuePair<string, object>("isHumanNearby", true));
        //goal.Add(new KeyValuePair<string, object>("isWerewolfNearby", false));

        return goal;
    }
}
