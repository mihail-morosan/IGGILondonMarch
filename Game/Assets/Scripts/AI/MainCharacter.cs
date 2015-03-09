using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCharacter : CharacterBasicBehaviour {
    void Start()
    {
        this.isBad = false;
    }

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("hasTreasure", true));
        //goal.Add(new KeyValuePair<string, object>("canEscape", true));
        //goal.Add(new KeyValuePair<string, object>("isWerewolfNearby", false));

        return goal;
    }
}
