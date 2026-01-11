using UnityEngine;

namespace CardGame.Effects
{
    /// <summary>
    /// 卡牌效果接口
    /// 所有卡牌效果都需要实现此接口
    /// </summary>
    public interface ICardEffect
    {
        /// <summary>
        /// 卡牌打出时触发
        /// </summary>
        /// <param name="card">打出的卡牌实例</param>
        /// <param name="target">目标（可选）</param>
        void OnPlay(CardInstance card, GameObject target = null);

        /// <summary>
        /// 卡牌入场时触发（仅随从卡牌）
        /// </summary>
        /// <param name="card">入场的卡牌实例</param>
        void OnEnter(CardInstance card);

        /// <summary>
        /// 卡牌死亡时触发
        /// </summary>
        /// <param name="card">死亡的卡牌实例</param>
        void OnDeath(CardInstance card);

        /// <summary>
        /// 回合开始时触发
        /// </summary>
        /// <param name="card">拥有此效果的卡牌实例</param>
        void OnTurnStart(CardInstance card);

        /// <summary>
        /// 回合结束时触发
        /// </summary>
        /// <param name="card">拥有此效果的卡牌实例</param>
        void OnTurnEnd(CardInstance card);

        /// <summary>
        /// 攻击时触发
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">目标</param>
        void OnAttack(CardInstance attacker, CardInstance target);

        /// <summary>
        /// 受到伤害时触发
        /// </summary>
        /// <param name="card">受到伤害的卡牌</param>
        /// <param name="damage">伤害值</param>
        void OnTakeDamage(CardInstance card, int damage);
    }
}
