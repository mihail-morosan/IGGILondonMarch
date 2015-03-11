using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WerewolfDungeonBehaviour : CharacterBasicBehaviourDungeon
{
    void Start()
    {
        this.isBad = true;
    }

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        //goal.Add(new KeyValuePair<string, object>("isHumanNearby", true));

        goal.Add(new KeyValuePair<string, object>("isHumanDead", true));

        return goal;
    }
}
