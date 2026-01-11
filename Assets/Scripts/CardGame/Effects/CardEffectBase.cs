using UnityEngine;
using CardGame;

namespace CardGame.Effects
{
    /// <summary>
    /// 卡牌效果基类（MonoBehaviour）
    /// 提供通用效果处理逻辑
    /// </summary>
    public abstract class CardEffectBase : MonoBehaviour, ICardEffect
    {
        [Header("效果设置")]
        [Tooltip("效果名称")]
        public string effectName = "未命名效果";

        [Tooltip("效果描述")]
        [TextArea(2, 4)]
        public string effectDescription = "";

        [Tooltip("效果优先级（数字越大优先级越高）")]
        public int priority = 0;

        /// <summary>
        /// 卡牌打出时触发
        /// </summary>
        public virtual void OnPlay(CardInstance card, GameObject target = null)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 卡牌入场时触发
        /// </summary>
        public virtual void OnEnter(CardInstance card)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 卡牌死亡时触发
        /// </summary>
        public virtual void OnDeath(CardInstance card)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 回合开始时触发
        /// </summary>
        public virtual void OnTurnStart(CardInstance card)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 回合结束时触发
        /// </summary>
        public virtual void OnTurnEnd(CardInstance card)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 攻击时触发
        /// </summary>
        public virtual void OnAttack(CardInstance attacker, CardInstance target)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 受到伤害时触发
        /// </summary>
        public virtual void OnTakeDamage(CardInstance card, int damage)
        {
            // 子类重写此方法实现具体效果
        }

        /// <summary>
        /// 获取效果信息
        /// </summary>
        public virtual string GetEffectInfo()
        {
            return $"{effectName}: {effectDescription}";
        }
    }
}
