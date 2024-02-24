using UnityEngine;
using RoR2;
using RoR2.Skills;

[CreateAssetMenu(fileName = "rsd", menuName = "ScriptableObjects/RavagerSkinDef", order = 1)]
public class RavagerSkinDef : ScriptableObject
{
    public string nameToken = "";

    public GameObject basicSwingEffectPrefab;
    public GameObject bigSwingEffectPrefab;
    public GameObject leapEffectPrefab;

    public GameObject slashEffectPrefab;

    public GameObject bloodOrbEffectPrefab;
    public GameObject bloodBombEffectPrefab;
    public GameObject bloodRushActivationEffectPrefab;
    public Material bloodOrbOverlayMaterial;
    public Material bloodRushOverlayMaterial;

    public string consumeSoundString = "";
    public string healSoundString = "";

    public Material electricityMat;
    public Material swordElectricityMat;
    public Color glowColor = Color.red;
}