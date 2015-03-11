using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class CharacterBasicBehaviour : CommonBehaviour {

    public bool hasTreasure = false;

    public bool hasRocks = false;

    public bool isBad = false;

    public override HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        MainCharacter mainChar = GetComponent<MainCharacter>();
        //DruidBehaviour druidChar = FindObjectOfType<DruidBehaviour>();
        WerewolfBehaviour wereChar = null;

        float dist = 10000;
        foreach(WerewolfBehaviour w in FindObjectsOfType<WerewolfBehaviour>())
        {
            if((Location - w.Location).magnitude < dist)
            {
                wereChar = w;
                dist = (Location - w.Location).magnitude;
            }
        }


        bool isEscapeRouteClear = true;

        List<GridTile> path;
        if (mainChar != null)
        {
            path = gridLayer.GetBestPathToTile(gridLayer.GetTile(mainChar.Location), gridLayer.GetTile(gridLayer.EscapeLocation));

            foreach (var p in path)
            {
                if (p.Passable == false)
                {
                    isEscapeRouteClear = false;
                    break;
                }
            }
        }
 

        bool isTreasureProtected = false;

        if(GetComponent<DruidBehaviour>())
            foreach (MainCharacter w in FindObjectsOfType<MainCharacter>())
            {
            
                path = gridLayer.GetBestPathToTile(gridLayer.GetTile(w.Location), gridLayer.GetTile(gridLayer.TreasureLocation));

                foreach(var p in path)
                {
                    if(p.Passable == false)
                    {
                        isTreasureProtected = true;
                        break;
                    }
                }

            }

        if(mainChar!=null)
        {
            path = gridLayer.GetBestPathToTile(gridLayer.GetTile(Location), gridLayer.GetTile(gridLayer.TreasureLocation));

            foreach (var p in path)
            {
                if (p.Passable == false)
                {
                    isTreasureProtected = true;
                    break;
                }
            }
        }

        worldData.Add(new KeyValuePair<string, object>("isEscapeRouteClear", isEscapeRouteClear));
        
        worldData.Add(new KeyValuePair<string, object>("isTreasureProtected", isTreasureProtected));


        worldData.Add(new KeyValuePair<string, object>("isTreasureAvailable", !gridLayer.TreasureLocation.Equals(new Vector2(-1000,-1000))));

        

        worldData.Add(new KeyValuePair<string, object>("canEscape", (hasTreasure || gridLayer.TreasureLocation.Equals(new Vector2(-1000,-1000)) && Location == gridLayer.EscapeLocation)));

        worldData.Add(new KeyValuePair<string, object>("hasTreasure", hasTreasure || gridLayer.TreasureLocation.Equals(new Vector2(-1000, -1000))));

        worldData.Add(new KeyValuePair<string, object>("hasRocks", hasRocks));

        //worldData.Add(new KeyValuePair<string, object>("isHumanNearby", isBad && (mainChar.Location - Location).magnitude < 2.5f));
        worldData.Add(new KeyValuePair<string, object>("isHumanNearby", false));

        worldData.Add(new KeyValuePair<string, object>("isWerewolfNearby", !isBad && wereChar != null && (wereChar.Location - Location).magnitude < 2.5f));

        

        return worldData;
    }

    //public abstract HashSet<KeyValuePair<string, object>> createGoalState();
}
