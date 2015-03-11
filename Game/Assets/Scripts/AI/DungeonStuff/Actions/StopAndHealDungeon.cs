using UnityEngine;
using System.Collections;

public class StopAndHealDungeon : GoapAction {
    CharacterBasicBehaviourDungeon mainChar;

    bool hasMoved = false;

    private float startTime = 0;
    public float workDuration = 1;

    public StopAndHealDungeon()
    {
		addPrecondition ("isLowHealth", true);

		addEffect ("isLowHealth", false);
	}
	
    void Start()
    {
        mainChar = GetComponent<CharacterBasicBehaviourDungeon>();
    }
	
	public override void reset ()
	{
        hasMoved = false;
        startTime = 0;
	}
	
	public override bool isDone ()
	{
        return hasMoved;
	}
	
	public override bool requiresInRange ()
	{
		return false;
	}
	
	public override bool checkProceduralPrecondition (GameObject agent)
	{
        return true;
	}
	
	public override bool perform (GameObject agent)
	{
        if (startTime == 0)
			startTime = Time.time;

        Animator anim = mainChar.GetComponent<Animator>();
        if(anim!=null)
        {
            anim.SetInteger("Direction", 4);
        }

        if (Time.time - startTime > workDuration)
        {
            hasMoved = true;

            mainChar.Health = mainChar.MaxHealth;

            return true;
        }

		return true;
	}
}
