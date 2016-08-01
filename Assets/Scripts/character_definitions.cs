using UnityEngine;
using System.Collections;

public class character_definitions:MonoBehaviour
{
	public struct actual_character
	{
        public bool living;
        public int enemytype;
		public int level;
		public float actual_lives;
		public float actual_damage;
		public float actual_attspeed;
        public GameObject battleicon;
        public Vector2 battleposition;
	}
	public struct def_character
	{
		
		public float init_lives;
		public float init_damage;
		public float init_attspeed;
        
        //basic attributes - must be 6
        public float strength;
		public float dexterity;
		public float wisdom;
		public float vitality;
        public float defense; //**
        public float resistance; //**

        public float strength_to_lives_multi;
		public float dexterity_to_lives_multi;
		public float wisdom_to_lives_multi;
		public float vitality_to_lives_multi;

		public float strength_to_damage_multi;
		public float dexterity_to_damage_multi;
		public float wisdom_to_damage_multi;
		public float vitality_to_damage_multi;

		public float strength_to_attspeed_multi;
		public float dexterity_to_attspeed_multi;
		public float wisdom_to_attspeed_multi;
		public float vitality_to_attspeed_multi;

		public float strength_levelup_increment;
		public float dexterity_levelup_increment;
		public float wisdom_levelup_increment;
		public float vitality_levelup_increment;

	}
	public def_character[,] def_enemy_list; // visual type, level
	public def_character[] def_hero_list; // visual type

	public actual_character[] actual_enemy_list; // index of enemy in level
	public actual_character[] actual_hero_list; // index of hero

	public int ActiveHero;
    public int EnemyBattlesCount;
    public GameObject attackicon;
    static map_manager map_manager_local;

    public void character_definitions_init (bool firstinit)
	{
        if (firstinit)
        {
            def_enemy_list = new def_character[100, 200];
            def_hero_list = new def_character[10];


            //actual enemies
            actual_enemy_list = new actual_character[200];

            //actual heroes
            actual_hero_list = new actual_character[5];

            map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
        }

        ActiveHero = 0;
        EnemyBattlesCount = 0;
    }

	public float GetMaxLives(int type,int level)
	{
		float lives;
		Debug.Log ("Enemy Set");
		// Basic stats affect
		lives = 
			def_enemy_list[type, level].strength_to_lives_multi*def_enemy_list[type, level].strength+
			def_enemy_list[type, level].dexterity_to_lives_multi*def_enemy_list[type, level].dexterity+
			def_enemy_list[type, level].wisdom_to_lives_multi*def_enemy_list[type, level].wisdom+
			def_enemy_list[type, level].vitality_to_lives_multi*def_enemy_list[type, level].vitality;

		return lives;

	}

	public float GetMaxLives(int hero)
	{
		float lives;
		Debug.Log ("Hero Set");
		lives = 
			def_hero_list[hero].strength_to_lives_multi*def_hero_list[hero].strength+
			def_hero_list[hero].dexterity_to_lives_multi*def_hero_list[hero].dexterity+
			def_hero_list[hero].wisdom_to_lives_multi*def_hero_list[hero].wisdom+
			def_hero_list[hero].vitality_to_lives_multi*def_hero_list[hero].vitality;

		return lives;
	}

    public void AddBattle(int x, int y)
    {
        Vector3 pos;
        Vector2 battlepos;

        actual_enemy_list[EnemyBattlesCount].battleicon = Instantiate(attackicon, Vector3.zero, Quaternion.identity) as GameObject;
        actual_enemy_list[EnemyBattlesCount].battleicon.transform.SetParent(map_manager_local.mapcontainer.transform);
        actual_enemy_list[EnemyBattlesCount].battleicon.transform.localScale = attackicon.transform.localScale;
        actual_enemy_list[EnemyBattlesCount].battleicon.transform.localRotation = attackicon.transform.localRotation;

        pos = Vector3.zero;
        pos.x = (x - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
        pos.y = (y - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
        actual_enemy_list[EnemyBattlesCount].battleicon.transform.localPosition = pos;

        battlepos = Vector2.zero;
        battlepos.x = x;
        battlepos.y = y;
        actual_enemy_list[EnemyBattlesCount].battleposition = battlepos;

        actual_enemy_list[EnemyBattlesCount].living = true;

        Debug.Log("Adding Battle: " + (int)battlepos.x + ", " + (int)battlepos.y);

        EnemyBattlesCount = EnemyBattlesCount + 1;
    }

    public int CheckBattle(int x, int y)
    {
        Vector2 givenpos;
        int foundindex;
        int i;
        givenpos = Vector2.zero;
        givenpos.x = x;
        givenpos.y = y;
        foundindex = -1;
        for (i = 0; i < EnemyBattlesCount; i++)
        {

            if (actual_enemy_list[i].battleposition == givenpos)
            {
                foundindex = i;
            }
        }
        if (foundindex>=0)
        {
            if (actual_enemy_list[foundindex].living)
            {
                Debug.Log("Battle Found: " + foundindex);
                return foundindex;
            }
            else
            {
                return -1;
            }
        } 
        else
        {
            return -1;
        }
        
    }
}

