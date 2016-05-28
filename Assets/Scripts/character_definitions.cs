using UnityEngine;
using System.Collections;

public class character_definitions:MonoBehaviour
{
	public struct actual_character
	{
		public int enemytype;
		public int level;
		public float actual_lives;
		public float actual_damage;
		public float actual_attspeed;
	}
	public struct def_character
	{
		
		public float init_lives;
		public float init_damage;
		public float init_attspeed;
		public float strength;
		public float dexterity;
		public float wisdom;
		public float vitality;

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

	public void character_definitions_init ()
	{
		def_enemy_list = new def_character[100,200];
		def_hero_list = new def_character[3];
		actual_enemy_list = new actual_character[200];

		//actual heroes
		ActiveHero=0;
		actual_hero_list = new actual_character[3];

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
}

