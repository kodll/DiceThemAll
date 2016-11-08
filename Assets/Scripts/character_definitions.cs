using UnityEngine;
using System.Collections;

public class character_definitions:MonoBehaviour
{
    public GameObject[] prefab_enemy; //enemy visual prefab

	public struct actual_character
	{
        public bool living;
        public int enemyTemplateIndex;
		public int level;
		public float actual_lives;
        public float max_lives;
		public float actual_damage;
		public float actual_attspeed;
        public float actual_atttime;
        public GameObject battleicon;
        public Vector2 battleposition;
	}
	public struct def_character
	{
		public float init_damage; //REPAIR!!! - change to attribute combination
		public float init_attspeed; //REPAIR!!! - change to attribute combination

        public int visualTypeIndex;
        
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
	public def_character[] def_enemy_list; // enemy template definitions
    public def_character[] def_hero_list; // hero template definition

	public actual_character[] actual_enemy_list; // index of enemy in level
	public actual_character[] actual_hero_list; // index of hero

	public int ActiveHeroIndex;
    public int EnemyBattlesCount;
    public GameObject attackicon;
    static map_manager map_manager_local;

    public void character_definitions_init (bool firstinit)
	{
        if (firstinit)
        {
            def_enemy_list = new def_character[200]; //enemy template definitions

            SetupEnemyTemplates();

            def_hero_list = new def_character[10];

            SetupHeroTemplates();

            //actual enemies
            actual_enemy_list = new actual_character[200];

            //actual heroes
            actual_hero_list = new actual_character[5];

            map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;

            ActiveHeroIndex = 0;

            SetupNewHero(ActiveHeroIndex,0);
        }

        
        EnemyBattlesCount = 0;
    }

	public float GetMaxLives(int type,int level)
	{
		float lives;
        //Debug.Log ("Enemy Set");
        // Basic stats affect
        
        lives = 
			def_enemy_list[type].strength_to_lives_multi*def_enemy_list[type].strength+
			def_enemy_list[type].dexterity_to_lives_multi*def_enemy_list[type].dexterity+
			def_enemy_list[type].wisdom_to_lives_multi*def_enemy_list[type].wisdom+
			def_enemy_list[type].vitality_to_lives_multi*def_enemy_list[type].vitality;
            
       
		return lives;

	}

	public float GetMaxLives(int hero)
	{
		float lives;
		//Debug.Log ("Hero Set");
		lives = 
			def_hero_list[hero].strength_to_lives_multi*def_hero_list[hero].strength+
			def_hero_list[hero].dexterity_to_lives_multi*def_hero_list[hero].dexterity+
			def_hero_list[hero].wisdom_to_lives_multi*def_hero_list[hero].wisdom+
			def_hero_list[hero].vitality_to_lives_multi*def_hero_list[hero].vitality;

		return lives;
	}

    public void AddBattle(int x, int y, int enemyTemplateIndex, int enemyLevel)
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

        actual_enemy_list[EnemyBattlesCount].enemyTemplateIndex = enemyTemplateIndex;

        actual_enemy_list[EnemyBattlesCount].actual_damage = def_enemy_list[enemyTemplateIndex].init_damage;
        actual_enemy_list[EnemyBattlesCount].actual_attspeed = def_enemy_list[enemyTemplateIndex].init_attspeed;
        actual_enemy_list[EnemyBattlesCount].max_lives = GetMaxLives(enemyTemplateIndex, enemyLevel);
        actual_enemy_list[EnemyBattlesCount].actual_lives = actual_enemy_list[EnemyBattlesCount].max_lives;

        Debug.Log("Max Enemy[" + EnemyBattlesCount + "] Lives: " + actual_enemy_list[EnemyBattlesCount].max_lives + ", BattleSpeed:" + def_hero_list[enemyTemplateIndex].init_attspeed);
        Debug.Log("EnemyTemplate Index: " + enemyTemplateIndex);

        //Debug.Log("Adding Battle: " + (int)battlepos.x + ", " + (int)battlepos.y);



        EnemyBattlesCount = EnemyBattlesCount + 1;
    }

    public void RemoveBattle(int index)
    {
        actual_enemy_list[index].living = false;
        actual_enemy_list[index].battleicon.SetActive(false);
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
                //Debug.Log("Battle Found: " + foundindex);
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

    public void SetupNewHero(int actualIndex, int templateIndex)
    {
        actual_hero_list[actualIndex].actual_damage = def_hero_list[templateIndex].init_damage;
        actual_hero_list[actualIndex].actual_attspeed = def_hero_list[templateIndex].init_attspeed;
        actual_hero_list[actualIndex].max_lives = GetMaxLives(templateIndex);
        actual_hero_list[actualIndex].actual_lives = actual_hero_list[actualIndex].max_lives;

        Debug.Log("Max Hero Lives: " + actual_hero_list[actualIndex].max_lives);
    }

    public void SetupHeroTemplates()
    {
        def_hero_list[0].init_damage = 10;
        def_hero_list[0].init_attspeed = 4f;
        def_hero_list[0].strength = 25;
        def_hero_list[0].dexterity = 10;
        def_hero_list[0].wisdom = 5;
        def_hero_list[0].vitality = 40;
        def_hero_list[0].defense = 20; //**
        def_hero_list[0].resistance = 5; //**
        def_hero_list[0].strength_to_lives_multi = 1;
        def_hero_list[0].dexterity_to_lives_multi = 0;
        def_hero_list[0].wisdom_to_lives_multi = 0;
        def_hero_list[0].vitality_to_lives_multi = 1;


}

    public void SetupEnemyTemplates()
    {
        def_enemy_list[0].init_damage = 1;
        def_enemy_list[0].init_attspeed = 3f;
        def_enemy_list[0].visualTypeIndex = 0;
        def_enemy_list[0].strength = 25;
        def_enemy_list[0].dexterity = 10;
        def_enemy_list[0].wisdom = 5;
        def_enemy_list[0].vitality = 40;
        def_enemy_list[0].defense = 20; //**
        def_enemy_list[0].resistance = 5; //**
        def_enemy_list[0].strength_to_lives_multi = 1;
        def_enemy_list[0].dexterity_to_lives_multi = 0;
        def_enemy_list[0].wisdom_to_lives_multi = 0;
        def_enemy_list[0].vitality_to_lives_multi = 1;

        def_enemy_list[1].init_damage = 1;
        def_enemy_list[1].init_attspeed = 3f;
        def_enemy_list[1].visualTypeIndex = 1;
        def_enemy_list[1].strength = 35;
        def_enemy_list[1].dexterity = 10;
        def_enemy_list[1].wisdom = 5;
        def_enemy_list[1].vitality = 40;
        def_enemy_list[1].defense = 20; //**
        def_enemy_list[1].resistance = 5; //**
        def_enemy_list[1].strength_to_lives_multi = 0;
        def_enemy_list[1].dexterity_to_lives_multi = 1;
        def_enemy_list[1].wisdom_to_lives_multi = 0;
        def_enemy_list[1].vitality_to_lives_multi = 1;
    }

    
}

