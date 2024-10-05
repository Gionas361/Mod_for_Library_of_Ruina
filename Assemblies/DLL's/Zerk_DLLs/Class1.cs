using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Battle.DiceAttackEffect;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////// Initializer /////////////////////////////////

/**
public class MyTopStarInitializer : ModInitializer
{
    //Declarations
    public static string path;
    public static bool Init;
    public static Dictionary<string, AssetBundle> assetBundle = new Dictionary<string, AssetBundle>();
    public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();


    //Code
    public static void AddAssets(string name, string path)
    {
        AssetBundle value = AssetBundle.LoadFromFile(path);
        MyTopStarInitializer.assetBundle.Add(name, value);
    }

    public override void OnInitializeMod()
    {
        base.OnInitializeMod();
        Harmony harmony = new Harmony("MyTopStar");

        MyTopStarInitializer.path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        MyTopStarInitializer.Init = true;
        MyTopStarInitializer.AddAssets("manastack", MyTopStarInitializer.path + "/CustomEffect/manastack");
    }
}
**/

/////////////////////////////// Initializer /////////////////////////////////



/////////////////////////////// Buffs / Effects /////////////////////////////

/**
public class BattleUnitBuf_Mana : BattleUnitBuf
{
    //I just dont know
    public void PrintEffect(BattleUnitModel target)
    {
        GameObject gameObject = new GameObject();
        AssetBundle assetBundle = MyTopStarInitializer.assetBundle["manastack"];
        for (int i = 0; i < assetBundle.GetAllAssetNames().Length; i++)
        {
            string text = assetBundle.GetAllAssetNames()[i];
            gameObject = UnityEngine.Object.Instantiate<GameObject>(assetBundle.LoadAsset<GameObject>(text));
        }
        gameObject.transform.parent = target.view.transform;
        gameObject.transform.localPosition = new Vector3(0f, 2f);
        gameObject.transform.localScale = new Vector3((float)target.view.model.UnitData.unitData.customizeData.height * 0.025f, (float)target.view.model.UnitData.unitData.customizeData.height * 0.025f, (float)target.view.model.UnitData.unitData.customizeData.height * 0.025f);
        gameObject.layer = LayerMask.NameToLayer("Effect");
        gameObject.AddComponent<BattleUnitBuf_Mana.Destroyer>().Init();
        gameObject.SetActive(true);
    }

    //Declares the keyword id of the effect
    protected override string keywordId
    {
        get
        {
            return "ManaStack";
        }
    }

    public override BufPositiveType positiveType
    {
        get
        {
            return (BufPositiveType)1;
        }
    }

    //Initializes the actual buf.
    public override void Init(BattleUnitModel owner)
    {
        base.Init(owner);
        typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, MyTopStarInitializer.ArtWorks["ManaStack"]);
        typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
    }

    //Checks if the target actually has the buff.
    public static BattleUnitBuf_Mana IshaveBuf(BattleUnitModel target, bool findready = false)
    {
        if (findready)
        {
            foreach (BattleUnitBuf battleUnitBuf in target.bufListDetail.GetReadyBufList())
            {
                if (battleUnitBuf is BattleUnitBuf_Mana)
                {
                    return battleUnitBuf as BattleUnitBuf_Mana;
                }
            }
        }
        foreach (BattleUnitBuf battleUnitBuf2 in target.bufListDetail.GetActivatedBufList())
        {
            if (battleUnitBuf2 is BattleUnitBuf_Mana)
            {
                return battleUnitBuf2 as BattleUnitBuf_Mana;
            }
        }
        return null;
    }

    //Handles addding to the buff stack.
    public static void AddBuf(BattleUnitModel target, int stack, bool isImm = false)
    {
        BattleUnitBuf_Mana battleUnitBuf_Mana = BattleUnitBuf_Mana.IshaveBuf(target, true);
        if (battleUnitBuf_Mana != null)
        {
            battleUnitBuf_Mana.stack += stack;
            return;
        }
        BattleUnitBuf_Mana BattleUnitBuf_Mana2 = new BattleUnitBuf_Mana();
        BattleUnitBuf_Mana2.stack = stack;
        BattleUnitBuf_Mana2.Init(target);
        if (isImm)
        {
            target.bufListDetail.AddBuf(BattleUnitBuf_Mana2);
            return;
        }
        target.bufListDetail.GetReadyBufList().Add(BattleUnitBuf_Mana2);
    }

    //Handles destroying the buf.
    public class Destroyer : MonoBehaviour
    {
        public void Init()
        {
            this.duration = 1f;
        }

        public void Update()
        {
            this.duration -= Time.deltaTime;
            if (this.duration <= 0f)
            {
                UnityEngine.Object.DestroyImmediate(base.gameObject);
            }
        }

        public float duration;
    }
}
**/

public class BattleUnitBuf_CostdownCards : BattleUnitBuf
{
    public override int GetCardCostAdder(BattleDiceCardModel card)
    {
        return -_owner.emotionDetail.EmotionLevel;
    }
}

public class BattleUnitBuf_ManaStack : BattleUnitBuf
{
    protected override string keywordId => "ManaStack";
    protected override string keywordIconId => "ManaStack";

    public BattleUnitBuf_ManaStack(BattleUnitModel model)
	{
		this._owner = model;
		/*
		typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
		typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
		*/
		this.stack = 0;
	}

	public void Add(int add)
	{
		this.stack += add;
	}

	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_ManaStack BattleUnitBuf_ManaStack = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ManaStack) as BattleUnitBuf_ManaStack;
		if (BattleUnitBuf_ManaStack == null)
		{
			BattleUnitBuf_ManaStack BattleUnitBuf_ManaStack2 = new BattleUnitBuf_ManaStack(unit);
			BattleUnitBuf_ManaStack2.Add(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_ManaStack2);
		}
		else
		{
			BattleUnitBuf_ManaStack.Add(stack);
		}
	}

	public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
	{
		BattleUnitBuf_ManaStack manaStack = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ManaStack) as BattleUnitBuf_ManaStack;
		if (div0sub1 == 0)
		{
			manaStack.PercentageDeleto(unit, percentage);
		}
		else if (div0sub1 == 1)
		{
			manaStack.IntegerDeleto(unit, percentage);
		}
	}

	public void IntegerDeleto(BattleUnitModel unit, int amount)
	{
		this.stack -= amount;
	}

	public void PercentageDeleto(BattleUnitModel unit, int percentage)
	{
		int value = this.stack;

		double percentasdouble = (double)percentage / 100;
		this.stack -= value * (int)percentasdouble;
	}
}

/////////////////////////////// Buffs / Effects /////////////////////////////



/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_DragonRoarStagger : DiceCardAbilityBase
{
    public static string Desc = "[On Clash Win] Stagger the opponent.";

    public override void OnSucceedAttack()
    {
        int halfMana = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ManaStack).stack / 2;
        BattleUnitModel targeto = base.card?.target;


        List<BattleUnitModel> enemyAliveList = BattleObjectManager.instance.GetAliveList(targeto.faction);
        foreach (BattleUnitModel item in enemyAliveList)
        {
            item.TakeDamage(halfMana, DamageType.Card_Ability, base.owner);
        }

        List<BattleUnitModel> teamAliveList = BattleObjectManager.instance.GetAliveList(base.owner.faction);
        foreach (BattleUnitModel item in teamAliveList)
        {
            item.TakeDamage(halfMana, DamageType.Card_Ability, base.owner);
        }

        if (targeto != null)
        {
            int num = targeto.breakDetail.breakGauge;
            if (num < 1)
            {
                num = 1;
            }
            targeto.TakeBreakDamage(num, DamageType.Card_Ability, base.owner, AtkResist.None);
        }
    }
}

public class DiceCardAbility_BurnBothBy7 : DiceCardAbilityBase
{
    public override string[] Keywords => new string[] { "ManaStack" };
    public static string Desc = "[On Clash Win] Inflict 7 stacks of Mana on opponent and the Librarian.";

    public override void OnWinParrying()
    {
        BattleUnitBuf_ManaStack.AddBuf(base.owner, 7);
        BattleUnitBuf_ManaStack.AddBuf(card.target, 7);
    }
}

public class DiceCardAbility_BurnBoth2n1 : DiceCardAbilityBase
{
    public override string[] Keywords => new string[] { "ManaStack" };
    public static string Desc = "[On Clash Win] Inflict 2 stacks of Mana on opponent and 1 stack to the Librarian.";

    public override void OnWinParrying()
    {
        BattleUnitBuf_ManaStack.AddBuf(base.owner, 1);
        BattleUnitBuf_ManaStack.AddBuf(card.target, 2);
    }
}

/////////////////////////////// Dice Passives ///////////////////////////////



/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_CashoutEngineSkill : DiceCardSelfAbilityBase
{
    public override string[] Keywords => new string[] { "ManaStack" };
    public static string Desc = "[On Use] If librarian has equal or more mana than its Max Light. Restore all Light and consume that amount of Mana.";

}

public class DiceCardSelfAbility_HpForLight : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Use] Lose 10% of your HP and recover 4 Light.";

    public override void OnUseCard()
    {
        int num = base.owner.MaxHp / 10;

        base.owner.TakeDamage(num, DamageType.Passive, base.owner);
        base.owner.cardSlotDetail.RecoverPlayPoint(4);
    }
}

public class DiceCardSelfAbility_RecoverStaggerToDiceValue : DiceCardSelfAbilityBase
{
    public static string Desc = "Recover Stagger amount equal to the value of all the dice in this page.";

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
        BattleUnitModel battleUnitModel = base.card?.target;
        if (battleUnitModel != null && base.owner != null && behavior != null)
        {
            int diceResultValue = behavior.DiceResultValue;
            base.owner.breakDetail.RecoverBreak(diceResultValue);
        }
    }
}

public class DiceCardSelfAbility_ExpendableEngine : DiceCardSelfAbilityBase
{
    public static string Desc = "Dice in this page will gain 2 Power for each \"Mana Stack\".";

    public override void OnStartParrying()
    {
        card?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
        {
            power = 3
        });
    }
}

public class DiceCardSelfAbility_DragonRoarStaggerCard : DiceCardSelfAbilityBase
{
    public static string Desc = "[On Clash Win] Deal DMG equal to half the Librarian's Mana Stacks to every character (includes self).";
}

/////////////////////////////// Page Passives ///////////////////////////////



///////////////////////////// Key Page Passives /////////////////////////////

public class PassiveAbility_ZerkEveryUnique : PassiveAbilityBase
{
    //For each card used since last time "Cashout Engine" was used increase a invisible count.
    //Once the card "Cashout Engine" is used restore light equal to the count.

    //Discard hand at end of Scene and draw 5 cards.

    public int CardsUsed;
    public bool IsSingleton;
    public int intforcardsID;

    public override void OnUnitCreated()
    {
        BattleUnitBuf_ManaStack.AddBuf(base.owner, 1);
        CardsUsed = 0;
        IsSingleton = false;

        if (owner.faction == Faction.Player)
        {
            owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 40));
        }

        if (base.owner.allyCardDetail.IsHighlander())
        {
            IsSingleton = true;
        }
    }

    public override void OnRoundEnd()
    {
        owner.allyCardDetail.DiscardACardByAbility(base.owner.allyCardDetail.GetHand());
        owner.allyCardDetail.DrawCards(5);

        int cardsInHand = base.owner.allyCardDetail.GetHand().Count / 2;
        BattleUnitBuf_ManaStack.AddBuf(base.owner, cardsInHand);
    }

    public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
    {
        BattleUnitBuf_ManaStack.AddBuf(base.owner, 1);
        CardsUsed = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ManaStack).stack;
        intforcardsID = 31;

        if (curCard.card.GetID() == new LorId("MyTopStar", 33))
        {
            base.owner.cardSlotDetail.RecoverPlayPoint(CardsUsed);

            if (CardsUsed > base.owner.cardSlotDetail.GetMaxPlayPoint())
            {
                CardsUsed -= base.owner.cardSlotDetail.GetMaxPlayPoint();
            }
        }

        if (curCard.card.GetID() == new LorId("MyTopStar", 40))
        {
            owner.currentDiceAction.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 3 * CardsUsed
            });

            BattleUnitBuf_ManaStack.SubBuf(base.owner, (CardsUsed / 2) - 1, 1);
        }

        if (IsSingleton == true)
        {
            while (intforcardsID <= 40)
            {
                if (curCard.card.GetID() == new LorId("MyTopStar", intforcardsID))
                {
                    BattleUnitBuf_ManaStack.AddBuf(base.owner, 1);
                }

                intforcardsID++;
            }
        }
    }
}

public class PassiveAbility_ZerkPassiveHeal : PassiveAbilityBase
{
    public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
    {
        if (curCard.GetOriginalDiceBehaviorList().FindAll((DiceBehaviour x) => x.Type == BehaviourType.Def).Count != 0)
        {
            int healmount = RandomUtil.Range(1, 8) * curCard.GetOriginalDiceBehaviorList().FindAll((DiceBehaviour x) => x.Type == BehaviourType.Def).Count;

            owner.battleCardResultLog?.SetPassiveAbility(this);
            base.owner.RecoverHP(healmount);
        }
    }

    public override void OnRoundEnd()
    {
        if (base.owner.cardSlotDetail.PlayPoint <= 1)
        {
            owner.cardSlotDetail.RecoverPlayPoint(2);
        }
    }
}

public class PassiveAbility_OverpoweredDragon : PassiveAbilityBase
{
    //Ability for the bossfight, pretty self explanitory, the more emotional level
    //the less the cards cost and the more energy that is regenerated. 

    public override int SpeedDiceNumAdder()
    {
        BattleUnitModel battleUnitModel = owner;

        int emotionLevel = base.owner.emotionDetail.EmotionLevel;

        return (2);
    }

    public override void OnRoundEnd()
    {
        base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_CostdownCards).Destroy();
    }

    public override void OnRoundStart()
    {
        owner.cardSlotDetail.RecoverPlayPoint(base.owner.emotionDetail.EmotionLevel);

        base.owner.bufListDetail.AddBuf(new BattleUnitBuf_CostdownCards());

        /**
        foreach (BattleDiceCardModel item in base.owner.allyCardDetail.GetHand())
        {
            int MINUS = item.GetOriginCost() - base.owner.emotionDetail.EmotionLevel;
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, MINUS);

            if (MINUS < 0)
            {
                MINUS = 0;
            }

            item.SetCurrentCost(MINUS);
        }
        **/
    }
}

///////////////////////////// Key Page Passives /////////////////////////////
