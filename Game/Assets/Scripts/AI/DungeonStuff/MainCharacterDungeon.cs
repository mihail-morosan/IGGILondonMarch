using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCharacterDungeon : CharacterBasicBehaviourDungeon
{
    void Start()
    {
        this.isBad = false;
    }

    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("canEscape", true));

        return goal;
    }
}
