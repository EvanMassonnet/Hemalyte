using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{

    [Tooltip("vie max du perso")]
    public int maxLife;

    [Tooltip("en int par secondes")]
    public int lifeRegeneration;

    [Tooltip("l'endurance")]
    public int maxStamina;

    [Tooltip("en int par secondes")]
    public int staminaRegeneration;

    [Tooltip("coef de multiplication du heal recu, < 1 si debuff")]
    public int boostHeal = 1;

    [Tooltip("coef de multiplication de la vitesse du perso, < 1 si ralentie")]
    public int characterSpeed = 1;

    [Tooltip("coef de multiplication des action de type chargement, > 1 si ralentie")]
    public int actionSpeed = 1;

    [Tooltip("coef de multiplication du temps pour revive, > 1 si ralentie")]
    public int reviveSpeed = 1;

    [Tooltip("coef de multiplication des degats, < 1 si debuff")]
    public int boostDamage = 1;

    [Tooltip("coef de multiplication de resistence au degat, < 1 si debuff")]
    public int resistanceDamage = 1;

    [Tooltip("coef de multiplication des degats des aliées")]
    public int resistanceFriendlyFire = 1;

    [Tooltip("coef de multiplication des degats élémentaires")]
    public int resistanceElemental = 1;
}
