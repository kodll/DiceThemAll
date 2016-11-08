using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gui_battle_popup : MonoBehaviour
{
    static bool UpdateTime;

    public GameObject sliderLivesHero;
    public GameObject sliderLivesEnemy;
    public GameObject sliderAttackHero;
    public GameObject sliderAttackEnemy;

    static float actualSliderLivesHero;
    static float actualSliderLivesEnemy;
    static float actualSliderAttackHero;
    static float actualSliderAttackEnemy;

    static map_manager map_manager_local;
    static avatarstatemachine avatarobject_local;
    static camera_lowfps camera_lowfps_local;
    static GameObject enemyobject;
    int battleindex;

    static float battleshift = 25f;

    private IEnumerator setdetailcoroutine;

    // Use this for initialization
    void Start()
    {
        map_manager_local = GameObject.FindObjectOfType(typeof(map_manager)) as map_manager;
        avatarobject_local = GameObject.FindObjectOfType(typeof(avatarstatemachine)) as avatarstatemachine;
        camera_lowfps_local = GameObject.FindObjectOfType(typeof(camera_lowfps)) as camera_lowfps;
    }

    // Update is called once per frame
    void Update()
    {
        float timeDelta;
        timeDelta = Time.deltaTime;
        if (UpdateTime)
        {
            map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_atttime = map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_atttime + timeDelta;
            map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_atttime = map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_atttime + timeDelta;

            if (map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_atttime > map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_attspeed)
            {
                // hero attacks

                map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives = map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives - map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_damage;

                map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_atttime = 0;
                actualSliderAttackHero = 0;
                //Debug.Log("Hero Attacks");
            }
            else if (map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_atttime > map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_attspeed)
            {
                // enemy attacks

                map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_lives = map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_lives - map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_damage;

                map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_atttime = 0;
                actualSliderAttackEnemy = 0;
                Debug.Log("Enemy Attacks");
            }
            if (map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives < 0)
            {

                //WIN!!!!
                UpdateTime = false;
                SurrenderButton(false);
            }


        }

        SetSliders(timeDelta);

    }

    void SetSliders(float TimeDelta)
    {
        /*actualSliderLivesHero = map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_lives / map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].max_lives;
        actualSliderLivesEnemy = map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives / map_manager_local.character_definitions_local.actual_enemy_list[battleindex].max_lives;
        actualSliderAttackHero = 0;
        actualSliderAttackEnemy = 0;*/


        actualSliderAttackHero = map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_atttime / map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_attspeed;
         
        actualSliderLivesHero = actualSliderLivesHero - TimeDelta / 5;
        if (actualSliderLivesHero<(map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_lives / map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].max_lives))
        {
            actualSliderLivesHero = map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_lives / map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].max_lives;
        }


        actualSliderAttackEnemy = map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_atttime / map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_attspeed;

        actualSliderLivesEnemy = actualSliderLivesEnemy - TimeDelta / 5;
        if (actualSliderLivesEnemy < (map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives / map_manager_local.character_definitions_local.actual_enemy_list[battleindex].max_lives))
        {
            actualSliderLivesEnemy = map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives / map_manager_local.character_definitions_local.actual_enemy_list[battleindex].max_lives;
        }

        sliderAttackHero.GetComponent<Slider>().value = actualSliderAttackHero;
        sliderLivesHero.GetComponent<Slider>().value = actualSliderLivesHero;
        sliderAttackEnemy.GetComponent<Slider>().value = actualSliderAttackEnemy;
        sliderLivesEnemy.GetComponent<Slider>().value = actualSliderLivesEnemy;

    }

    public void ShowGUI(bool show, int index)
    {
        if (show) //OPEN GUI
        {
            UpdateTime = false;
            battleindex = index;
            map_manager_local.GUIDungeonMovement.GetComponent<Animator>().SetTrigger("PanelHide");

            map_manager_local.scrollmultiplier = 1;
            map_manager_local.avatarstatictime = 0.6f;
            map_manager_local.camerafadeouttime = 0.3f;
            map_manager_local.cameraspeed = 20;

            map_manager_local.canscrollmanually = false;

            //avatarobject_local.FindPath(Xcoor, Ycoor);


            map_manager_local.GUIBattlePopup.GetComponent<Animator>().SetTrigger("PanelShow");

            setdetailcoroutine = SetDetailAnim();
            StartCoroutine(setdetailcoroutine);

        }
        else //CLOSE GUI
        {

        }
    }

    public void SurrenderButton(bool surrender)
    {
        Vector3 avatarpos;

        StopCoroutine(setdetailcoroutine);

        UpdateTime = false;

        this.GetComponent<Animator>().SetTrigger("PanelHide");
        map_manager_local.mapcamera.GetComponent<Animator>().SetTrigger("smalldetail_out");
        avatarobject_local.avatarcamera.GetComponent<Animator>().SetTrigger("zoomout");
        
        avatarobject_local.avatardetail = false;
        camera_lowfps_local.fpstime = 100;
        Debug.Log("AvatarHiFPS");
        avatarobject_local.SetHiMaterial(false);
        map_manager_local.GUIDungeonMovement.GetComponent<Animator>().SetTrigger("PanelShow");
        map_manager_local.TriggerScrolling(true);
        avatarobject_local.camerafolowobject.transform.localPosition = Vector3.zero;

        DestroyEnemy();

        if (surrender)
        {
            Debug.Log("Surrender, path index: " + avatarobject_local.avatarwhereinpath);
            avatarobject_local.FindPath((int)avatarobject_local.finalpath[avatarobject_local.avatarwhereinpath -1].x, (int)avatarobject_local.finalpath[avatarobject_local.avatarwhereinpath-1].y);
            map_manager_local.character_definitions_local.actual_enemy_list[battleindex].battleicon.GetComponent<attack_icon>().attackicon_animobject.GetComponent<Animator>().SetTrigger("battlesurrender");

        }
        else //DEBUG WIN
        {
            Debug.Log("Battle Won.");
            avatarobject_local.avatarobject.GetComponent<Animator>().SetTrigger("idle");
            map_manager_local.character_definitions_local.RemoveBattle(battleindex);
            avatarobject_local.FogUpdate();

            avatarpos = Vector3.zero;
            avatarpos.x = (avatarobject_local.finalpath[avatarobject_local.avatarwhereinpath].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarobject_local.avatarshift;
            avatarpos.y = (avatarobject_local.finalpath[avatarobject_local.avatarwhereinpath].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize;
            avatarpos.z = map_manager_local.floorZ;

            avatarobject_local.transform.localPosition = avatarpos;
        }

    }

    IEnumerator SetDetailAnim()
    {
        Vector3 avatarpos;
        Vector3 enemypos;
        Vector3 ActiveElementZero;
        Vector3 center;

        map_manager_local.character_definitions_local.actual_enemy_list[battleindex].battleicon.GetComponent<attack_icon>().attackicon_animobject.GetComponent<Animator>().SetTrigger("battlestart");

        yield return new WaitForSeconds(1.25f);
        
        avatarobject_local.SetHiMaterial(true);
        map_manager_local.mapcamera.GetComponent<Animator>().SetTrigger("smalldetail_in");
        avatarobject_local.avatarcamera.GetComponent<Animator>().SetTrigger("zoomin");

        

        avatarpos = Vector3.zero;
        avatarpos.x = (avatarobject_local.finalpath[avatarobject_local.avatarwhereinpath].x - map_manager_local.mapoffset) * map_manager_local.mappiecesize + avatarobject_local.avatarshift;
        avatarpos.y = (avatarobject_local.finalpath[avatarobject_local.avatarwhereinpath].y - map_manager_local.mapoffset) * map_manager_local.mappiecesize - battleshift;
		avatarpos.z = map_manager_local.floorZ;

        avatarobject_local.transform.localPosition = avatarpos;

        ActiveElementZero = avatarobject_local.transform.position;
        center = ActiveElementZero;
        ActiveElementZero.z = ActiveElementZero.z + battleshift;
        avatarobject_local.camerafolowobject.transform.position = ActiveElementZero;

        avatarobject_local.RotateAvatarToWaypoint(center, -1);

        enemypos = avatarpos;
        enemypos.y = enemypos.y + 2.5f * battleshift;
        enemypos.z = -10.7f;

        ShowEnemy(enemypos);

        yield return new WaitForSeconds(0.5f);

        avatarobject_local.avatardetail = true;
        avatarobject_local.avatarobject.GetComponent<Animator>().SetTrigger("chestwaiting");

        //BattleStart

        map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_atttime = 0;
        map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_atttime = 0;

        actualSliderLivesHero = map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].actual_lives/ map_manager_local.character_definitions_local.actual_hero_list[map_manager_local.character_definitions_local.ActiveHeroIndex].max_lives;
        actualSliderLivesEnemy = map_manager_local.character_definitions_local.actual_enemy_list[battleindex].actual_lives / map_manager_local.character_definitions_local.actual_enemy_list[battleindex].max_lives;
        actualSliderAttackHero = 0;
        actualSliderAttackEnemy = 0;

        UpdateTime = true;

    }

    public void ShowEnemy(Vector3 pos)
    {
        Vector3 finalrotation;
        int enemyVisualIndex;

        enemyVisualIndex = map_manager_local.character_definitions_local.def_enemy_list[map_manager_local.character_definitions_local.actual_enemy_list[battleindex].enemyTemplateIndex].visualTypeIndex;

        enemyobject = Instantiate(map_manager_local.character_definitions_local.prefab_enemy[enemyVisualIndex], Vector3.zero, Quaternion.identity) as GameObject; //TODO ENEMY SETUP
        enemyobject.transform.SetParent(map_manager_local.mapcontainer.transform);
        enemyobject.transform.localPosition = pos;

        Debug.Log("Statring Battle ID: " + battleindex + ", Enemy template: " + enemyVisualIndex);

        finalrotation.x = 90;
        finalrotation.y = 90;
        finalrotation.z = 270;

        enemyobject.transform.localEulerAngles = finalrotation;
    }

    public void DestroyEnemy()
    {
        Destroy(enemyobject);
    }
}