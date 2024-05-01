using System;

namespace weather.effects
{
    [System.Serializable]
    public class AttributeEffects
    {
        public float HealthEffect;
        public float StaminaEffect;
        public float HungerEffect;
        public float ThirstEffect;
        public float AgilityEffect;
        public float AmountEffect;
        public float HungerDecayRateEffect;
        public float ThirstDecayRateEffect;

        public AttributeEffects(float health, float stamina, float hunger, float thirst, float agility, float amountEffect, float hungerDecayRateEffect, float thirstDecayRateEffect)
        {
            HealthEffect = health;
            StaminaEffect = stamina;
            HungerEffect = hunger;
            ThirstEffect = thirst;
            AgilityEffect = agility;
            AmountEffect = amountEffect;
            HungerDecayRateEffect = hungerDecayRateEffect;
            ThirstDecayRateEffect = thirstDecayRateEffect;
        }
    }
}