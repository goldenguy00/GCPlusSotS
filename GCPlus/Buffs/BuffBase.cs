using System;
using R2API;
using RoR2;
using UnityEngine;

namespace GoldenCoastPlusRevived.Buffs
{
    public abstract class BuffBase<T> : BuffBase where T : BuffBase<T>
    {
        public static T instance { get; private set; }
        public static BuffIndex BuffIndex => instance?.buffDef?.buffIndex ?? BuffIndex.None;

        public BuffBase(bool enabled) : base(enabled)
        {
            if (instance != null)
                throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting BuffBase was instantiated twice");

            instance = this as T;
        }
    }

    public abstract class BuffBase
    {
        public BuffBase(bool enabled)
        {
            if (!enabled)
                return;

            AddBuff();
            AddHooks();
        }

        internal abstract string name { get; }
        internal abstract Sprite icon { get; }
        internal abstract Color color { get; }
        internal abstract bool canStack { get; }
        internal abstract bool isDebuff { get; }
        internal abstract EliteDef eliteDef { get; }

        public BuffDef buffDef;

        internal virtual void AddBuff()
        {
            buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = this.name;
            buffDef.iconSprite = this.icon;
            buffDef.buffColor = this.color;
            buffDef.canStack = this.canStack;
            buffDef.isDebuff = this.isDebuff;
            buffDef.eliteDef = this.eliteDef;
            ContentAddition.AddBuffDef(buffDef);
        }
        internal virtual void AddHooks()
        {

        }
    }
}
