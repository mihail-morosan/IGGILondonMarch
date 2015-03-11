using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class CharacterBasicBehaviourDungeon : CommonBehaviour
{
    public bool hasEscaped = false;
    public bool isBad = false;

    public int MaxHealth = 100;
    public int Health = 100;
    public int Attack = 40;
    public int AttackTime = 1;

    public CharacterBasicBehaviourDungeon closestEnemy = null;

    public override HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        MainCharacterDungeon mainChar = null;
        //DruidBehaviour druidChar = FindObjectOfType<DruidBehaviour>();
        WerewolfDungeonBehaviour wereChar = null;

        float dist = 10000;
        foreach (WerewolfDungeonBehaviour w in FindObjectsOfType<WerewolfDungeonBehaviour>())
        {
            if((Location - w.Location).magnitude < dist)
            {
                wereChar = w;
                dist = (Location - w.Location).magnitude;
            }
        }

        dist = 10000;
        foreach (MainCharacterDungeon w in FindObjectsOfType<MainCharacterDungeon>())
        {
            if ((Location - w.Location).magnitude < dist)
            {
                mainChar = w;
                dist = (Location - w.Location).magnitude;
            }
        }


        worldData.Add(new KeyValuePair<string, object>("canEscape", false));

        bool isHumanNearby = mainChar != null && (mainChar.Location - Location).magnitude < 2.5f;
        bool isWerewolfNearby = wereChar != null && (wereChar.Location - Location).magnitude < 2.5f;

        if (isBad)
            closestEnemy = mainChar;
        else
            closestEnemy = wereChar;

        //worldData.Add(new KeyValuePair<string, object>("isHumanNearby", isBad && (mainChar.Location - Location).magnitude < 2.5f));
        worldData.Add(new KeyValuePair<string, object>("isHumanNearby", isHumanNearby));

        worldData.Add(new KeyValuePair<string, object>("isHumanDead", false));

        //worldData.Add(new KeyValuePair<string, object>("isInCombat", isHumanNearby || isWerewolfNearby));

        worldData.Add(new KeyValuePair<string, object>("isLowHealth", Health < 0.8f * MaxHealth));

        worldData.Add(new KeyValuePair<string, object>("isWerewolfNearby", isWerewolfNearby));

        

        return worldData;
    }

    void Update()
    {
        if (currentActions == null || currentActions.Peek() == null || currentActions.Peek().target == null)
            return;
        Vector3 l1, l2;


        l1 = currentActions.Peek().target.transform.position + new Vector3(0, 0, -5);
        l2 = transform.position + new Vector3(0, 0, -5);
        Debug.DrawLine(l1, l2, Color.red);
    }

    //public abstract HashSet<KeyValuePair<string, object>> createGoalState();
}
