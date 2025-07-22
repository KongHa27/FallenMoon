using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/SkillData", fileName = "SkillData")]
public class SkillData : ScriptableObject
{
    [Header("----- 기본 정보 -----")]
    public string skillName;        //스킬 이름
    public string description;      //스킬 설명
    public Sprite icon;             //스킬 아이콘

    [Header("----- 스탯 -----")]
    public float cooldown;          //쿨타임 (초)
    public float damage;            //대미지 (%) ex.230% -> 2.3f
    public float range;             //사거리
    public float duration;          //지속 시간

    [Header("----- 이펙트 -----")]
    public GameObject effectPrefab; //이펙트
    public AudioClip soundClip;     //효과음
}
