namespace CardGame.Effects
{
    /// <summary>
    /// 预定义效果类型
    /// </summary>
    public static class EffectTypes
    {
        // 伤害类效果
        public const string DAMAGE = "Damage";
        public const string AOE_DAMAGE = "AoEDamage";
        public const string BURN = "Burn";

        // 治疗类效果
        public const string HEAL = "Heal";
        public const string REGENERATION = "Regeneration";

        // 抽牌类效果
        public const string DRAW_CARD = "DrawCard";
        public const string DISCARD = "Discard";

        // Buff/Debuff类效果
        public const string BUFF_ATTACK = "BuffAttack";
        public const string BUFF_HEALTH = "BuffHealth";
        public const string DEBUFF_ATTACK = "DebuffAttack";
        public const string DEBUFF_HEALTH = "DebuffHealth";

        // 状态类效果
        public const string TAUNT = "Taunt";           // 嘲讽
        public const string CHARGE = "Charge";         // 冲锋
        public const string STEALTH = "Stealth";       // 潜行
        public const string DIVINE_SHIELD = "DivineShield"; // 圣盾
        public const string POISON = "Poison";         // 剧毒
        public const string FROZEN = "Frozen";         // 冻结

        // 特殊效果
        public const string SUMMON = "Summon";
        public const string TRANSFORM = "Transform";
        public const string DESTROY = "Destroy";
        public const string RETURN_TO_HAND = "ReturnToHand";
    }
}
