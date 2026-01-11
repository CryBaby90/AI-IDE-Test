using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    /// <summary>
    /// 运行时卡牌实例类
    /// 包含卡牌的当前状态信息
    /// </summary>
    public class CardInstance
    {
        // 基础数据引用
        public CardData cardData { get; private set; }

        // 当前状态属性
        public int currentCost { get; set; }
        public int currentAttack { get; set; }
        public int currentHealth { get; set; }
        public int maxHealth { get; set; }

        // 状态效果列表
        private List<string> statusEffects = new List<string>();

        // 是否已使用（用于某些卡牌效果）
        public bool isUsed { get; set; }

        // 是否已死亡
        public bool isDead => currentHealth <= 0;

        /// <summary>
        /// 构造函数：从CardData创建卡牌实例
        /// </summary>
        public CardInstance(CardData data)
        {
            if (data == null)
            {
                Debug.LogError("CardInstance: CardData不能为空！");
                return;
            }

            cardData = data;
            currentCost = data.cost;
            currentAttack = data.attack;
            currentHealth = data.health;
            maxHealth = data.health;
            isUsed = false;
        }

        /// <summary>
        /// 克隆卡牌实例（深拷贝）
        /// </summary>
        public CardInstance Clone()
        {
            CardInstance clone = new CardInstance(cardData)
            {
                currentCost = this.currentCost,
                currentAttack = this.currentAttack,
                currentHealth = this.currentHealth,
                maxHealth = this.maxHealth,
                isUsed = this.isUsed
            };

            // 复制状态效果
            foreach (var effect in statusEffects)
            {
                clone.AddStatusEffect(effect);
            }

            return clone;
        }

        /// <summary>
        /// 添加状态效果
        /// </summary>
        public void AddStatusEffect(string effectName)
        {
            if (!statusEffects.Contains(effectName))
            {
                statusEffects.Add(effectName);
            }
        }

        /// <summary>
        /// 移除状态效果
        /// </summary>
        public void RemoveStatusEffect(string effectName)
        {
            statusEffects.Remove(effectName);
        }

        /// <summary>
        /// 检查是否有指定状态效果
        /// </summary>
        public bool HasStatusEffect(string effectName)
        {
            return statusEffects.Contains(effectName);
        }

        /// <summary>
        /// 获取所有状态效果
        /// </summary>
        public List<string> GetStatusEffects()
        {
            return new List<string>(statusEffects);
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
        }

        /// <summary>
        /// 恢复生命值
        /// </summary>
        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }

        /// <summary>
        /// 修改攻击力
        /// </summary>
        public void ModifyAttack(int amount)
        {
            currentAttack = Mathf.Max(0, currentAttack + amount);
        }

        /// <summary>
        /// 修改生命值上限
        /// </summary>
        public void ModifyMaxHealth(int amount)
        {
            maxHealth = Mathf.Max(1, maxHealth + amount);
            currentHealth = Mathf.Min(maxHealth, currentHealth);
        }

        /// <summary>
        /// 重置卡牌状态（用于回合重置等）
        /// </summary>
        public void ResetStatus()
        {
            currentCost = cardData.cost;
            isUsed = false;
            // 注意：不重置生命值和攻击力，因为这些可能是永久性的修改
        }
    }
}
