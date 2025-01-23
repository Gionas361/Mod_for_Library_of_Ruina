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
using ExtendedLoader;
using BaseMod;
using Battle.BufEffect;
using fxy_VFXAdder;




/////////////////////////////// Buffs / Effects /////////////////////////////

public class BattleUnitBuf_BoilingPointYaren : BattleUnitBuf
{
	protected override string keywordId => "BoilingPointYaren";
	protected override string keywordIconId => "BoilingPointYaren";


	public int burnInflicted;
	public bool isDistorted;
	public int roundsPassed;

	public BattleUnitBuf_BoilingPointYaren(BattleUnitModel model)
	{
		this._owner = model;
		this.stack = 0;
		burnInflicted = 0;
		isDistorted = false;
		roundsPassed = 0;
	}


    ////////////////////////////////////////////// Effects of Buff ///

    public override void OnRoundEnd()
    {
		// If above 50 get burn, plus keep track of burn applied.
		// So that distortion can occur.
		if (this.stack > 100)
		{
			this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, this.stack - 100, this._owner);
			burnInflicted += this.stack - 100;
			IntegerDeleto(this.stack - 100);
		}
    }

    public override void OnRoundStart()
    {
		// Keep track of rounds passed
		if (isDistorted) roundsPassed += 1;

		// If burn reaches 30, distort yaren.
		if (burnInflicted >= 30)
        {
			burnInflicted = 0;
			// Write code to distort.
			// Make sure it only works with the passive that says distort.
			if (!isDistorted && this._owner.passiveDetail.HasPassive<PassiveAbility_EgoPageAndDistortYaren>())
            {
				this._owner.view.ChangeWorkShopSkin("MyTopStar", "YarenEGOFull"); // Change once youve made the skin
				isDistorted = true;
			}
        }
    }

    ////////////////////////////////////////////// Effects of Buff ///


    public void Add(int add)
	{
		this.stack += add;
	}

	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_BoilingPointYaren BattleUnitBuf_BoilingPointYaren = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;
		if (BattleUnitBuf_BoilingPointYaren == null)
		{
			BattleUnitBuf_BoilingPointYaren BattleUnitBuf_BoilingPointYaren2 = new BattleUnitBuf_BoilingPointYaren(unit);
			BattleUnitBuf_BoilingPointYaren2.Add(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_BoilingPointYaren2);
		}
		else
		{
			BattleUnitBuf_BoilingPointYaren.Add(stack);
		}
	}

	public static void SubBuf(BattleUnitModel unit, int amount)
	{
		BattleUnitBuf_BoilingPointYaren BattleUnitBuf_BoilingPointYaren = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;
		BattleUnitBuf_BoilingPointYaren.IntegerDeleto(amount);
	}

	public void IntegerDeleto(int amount)
	{
		this.stack -= amount;
		if (this.stack < 0)
		{
			this.stack = 0;
		}
	}
}

public class BattleUnitBuf_ChainReactionYaren : BattleUnitBuf
{
	protected override string keywordId => "ChainReactionYaren";
	protected override string keywordIconId => "ChainReactionYaren";


	public BattleUnitBuf_ChainReactionYaren(BattleUnitModel model)
	{
		this._owner = model;
		this.stack = 0;
	}


    ////////////////////////////////////////////// Effects of Buff ///

    public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
    {
		int random = RandomUtil.Range(1, 3);

		// If lucky or if has the passive that guarantees triggering this effect.
		if (random == 2 || this._owner.passiveDetail.HasPassive<PassiveAbility_ChainReactionManager>())
		{
			this.Explode(atkDice, dmg);

		}

		// Get Team
		List<BattleUnitModel> team = BattleObjectManager.instance.GetAliveList(this._owner.faction).FindAll((BattleUnitModel x) => x != this._owner);

		// For each eammate
		foreach (BattleUnitModel teammate in team)
        {
			int chance25Percent = RandomUtil.Range(1, 4);

			if (chance25Percent == 3)
            {
				BattleUnitBuf_ChainReactionYaren BattleUnitBuf_ChainReactionYaren = teammate.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ChainReactionYaren) as BattleUnitBuf_ChainReactionYaren;
				BattleUnitBuf_ChainReactionYaren.TriggerExplosion(teammate, atkDice, dmg/2);
			}
		}

	}

	////////////////////////////////////////////// Effects of Buff ///

	public static void TriggerExplosion(BattleUnitModel unit, BattleDiceBehavior atkDice, int amount)
	{
		BattleUnitBuf_ChainReactionYaren BattleUnitBuf_ChainReactionYaren = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ChainReactionYaren) as BattleUnitBuf_ChainReactionYaren;
		BattleUnitBuf_ChainReactionYaren.Explode(atkDice, amount);
	}

	public void Explode(BattleDiceBehavior atkDice, int dmg)
	{
		IntegerDeleto(1);
		this._owner.TakeDamage(dmg, DamageType.Buf, atkDice.owner);
	}

    public void Add(int add)
	{
		this.stack += add;
	}

	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_ChainReactionYaren BattleUnitBuf_ChainReactionYaren = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ChainReactionYaren) as BattleUnitBuf_ChainReactionYaren;
		if (BattleUnitBuf_ChainReactionYaren == null)
		{
			BattleUnitBuf_ChainReactionYaren BattleUnitBuf_ChainReactionYaren2 = new BattleUnitBuf_ChainReactionYaren(unit);
			BattleUnitBuf_ChainReactionYaren2.Add(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_ChainReactionYaren2);
		}
		else
		{
			BattleUnitBuf_ChainReactionYaren.Add(stack);
		}
	}

	public static void SubBuf(BattleUnitModel unit, int amount)
	{
		BattleUnitBuf_ChainReactionYaren BattleUnitBuf_ChainReactionYaren = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_ChainReactionYaren) as BattleUnitBuf_ChainReactionYaren;
		BattleUnitBuf_ChainReactionYaren.IntegerDeleto(amount);
	}

	public void IntegerDeleto(int amount)
	{
		this.stack -= amount;
		if (this.stack <= 0)
		{
			this.Destroy();
		}
	}
}

public class BattleUnitBuf_YarensAlliesAreUntargetable : BattleUnitBuf
{
	protected override string keywordId => "YarensAlliesAreUntargetable";
	protected override string keywordIconId => "YarensAlliesAreUntargetable";


	public BattleUnitBuf_YarensAlliesAreUntargetable(BattleUnitModel model)
	{
		this._owner = model;
		this.stack = 0;
	}


	////////////////////////////////////////////// Effects of Buff ///

	public override bool IsTargetable()
	{
		return false;
	}

    public override void OnRoundEnd()
    {
		this.Destroy();
    }

    ////////////////////////////////////////////// Effects of Buff ///


    public void Add(int add)
	{
		this.stack += add;
	}

	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_YarensAlliesAreUntargetable BattleUnitBuf_YarensAlliesAreUntargetable = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_YarensAlliesAreUntargetable) as BattleUnitBuf_YarensAlliesAreUntargetable;
		if (BattleUnitBuf_YarensAlliesAreUntargetable == null)
		{
			BattleUnitBuf_YarensAlliesAreUntargetable BattleUnitBuf_YarensAlliesAreUntargetable2 = new BattleUnitBuf_YarensAlliesAreUntargetable(unit);
			BattleUnitBuf_YarensAlliesAreUntargetable2.Add(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_YarensAlliesAreUntargetable2);
		}
		else
		{
			BattleUnitBuf_YarensAlliesAreUntargetable.Add(stack);
		}
	}

	public static void SubBuf(BattleUnitModel unit, int amount)
	{
		BattleUnitBuf_YarensAlliesAreUntargetable BattleUnitBuf_YarensAlliesAreUntargetable = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_YarensAlliesAreUntargetable) as BattleUnitBuf_YarensAlliesAreUntargetable;
		BattleUnitBuf_YarensAlliesAreUntargetable.IntegerDeleto(amount);
	}

	public void IntegerDeleto(int amount)
	{
		this.stack -= amount;
		if (this.stack < 0)
		{
			this.stack = 0;
		}
	}
}

/////////////////////////////// Buffs / Effects /////////////////////////////





/////////////////////////////// BehaviourAction /////////////////////////////

/*
public class BehaviourAction_BehaviourAction : BehaviourActionBase
{
	// Not sure why its here but ok.
	public override bool IsMovable()
	{
		return false;
	}

	// Actually handles the moving and shit.
	public override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
	{
		// Makes it so that it doesnt happen when receiving a Mass Attack.
		bool flag = false;
		bool flag2 = opponent.behaviourResultData != null;
		bool flag3 = flag2;
		if (flag3)
		{
			flag = opponent.behaviourResultData.IsFarAtk();
		}
		bool flag4 = self.result <= 0 && !flag;
		bool flag5 = self.result > 0 || flag;
		bool flag6 = flag5;

		List<RencounterManager.MovingAction> result = new List<RencounterManager.MovingAction>();

		if (flag6)
		{
			result = base.GetMovingAction(ref self, ref opponent);
		}

		// When its not a mass attack do this:
		if (!flag6)
		{
			// Basically clears the actions/animations of the opponent.
			List<RencounterManager.MovingAction> list = new List<RencounterManager.MovingAction>();
			List<RencounterManager.MovingAction> infoList = opponent.infoList;
			bool flag7 = infoList != null && infoList.Count > 0;
			bool flag8 = flag7;
			if (flag8)
			{
				opponent.infoList.Clear();
			}
			bool flag9 = flag4;
			bool flag10 = flag9;

			// If its not responding to a mass attack:
			if (flag10)
			{
				// Controlling opponent
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)3, (CharMoveState)1, 4f, true, 0.125f, 1f);
				opponent.infoList.Add(item);


				// Gio, if u forgot what each variable in this string means, just hover over Moving Action
				// And click the field ur confused on, u will see the numbers and what they represent gl :)
				// Attack Sprite   // If it moves & the direction   // Moving Distance   // If looks at Opponent   // Delay   // Speed
				// Preparing to Pierce
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)2, (CharMoveState)11, 4f, true, 0.001f, 1.5f);
				list.Add(movingAction);
				movingAction = new RencounterManager.MovingAction((ActionDetail)2, (CharMoveState)11, 4f, true, 0.001f, 1.5f);
				list.Add(movingAction);


				movingAction = new RencounterManager.MovingAction((ActionDetail)2, (CharMoveState)3, 4f, true, 0.3f, 2.5f);
				list.Add(movingAction);

				// Actually Slashing
				movingAction = new RencounterManager.MovingAction((ActionDetail)5, (CharMoveState)2, 25f, false, 0.9f, 2f);
				movingAction.customEffectRes = "";
				movingAction.SetEffectTiming(0, 0, 0);
				list.Add(movingAction);
			}

			// If it is responding to mass attack set the return variable into what originally is meant to be animated.
			if (!flag10)
			{
				list = base.GetMovingAction(ref self, ref opponent);
			}
			// Sets the movingActions into the return variable.
			result = list;
		}

		return result;
	}
}
*/

/////////////////////////////// BehaviourAction /////////////////////////////





/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_GearShiftDice1Yaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Clash Win] Gain 1 Power Up this turn and next turn.";

    public override void OnWinParrying()
    {
		owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.AllPowerUp, 1, owner);
		owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.AllPowerUp, 1, owner);
	}
}

public class DiceCardAbility_GearShiftDice2Yaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "[On Hit] Increase Boiling Point by 1.";

    public override void OnSucceedAttack()
    {
		BattleUnitBuf_BoilingPointYaren.AddBuf(owner, 1);
    }
}

public class DiceCardAbility_PedalToTheMetalDiceYaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Hit] Gain 1 Haste.";

    public override void OnSucceedAttack()
    {
		owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 1, owner);
    }
}

public class DiceCardAbility_BackingUpDiceYaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "[On Clash Win] Increase Boiling Point by 1.";

    public override void OnWinParrying()
    {
		BattleUnitBuf_BoilingPointYaren.AddBuf(owner, 1);
    }
}

public class DiceCardAbility_EnginePowerUpDice2Yaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Clash Win] Restore 2 Light.";

    public override void OnWinParrying()
    {
		owner.cardSlotDetail.RecoverPlayPoint(2);
    }
}

public class DiceCardAbility_OnHit1PowerOnClashLose3Power : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Hit] Next dice gains 1 Power. [On Clash Lose] Gain 3 instead.";

	public override void OnSucceedAttack()
	{
		behavior.card.ApplyDiceStatBonus(DiceMatch.NextDice, new DiceStatBonus
		{
			power = 1
		});
	}

    public override void OnLoseParrying()
	{
		behavior.card.ApplyDiceStatBonus(DiceMatch.NextDice, new DiceStatBonus
		{
			power = 3
		});
	}
}

public class DiceCardAbility_FumeRetentionDice2Yaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Hit] Draw 2 Pages.";

    public override void OnSucceedAttack()
    {
		owner.allyCardDetail.DrawCards(2);
    }
}

public class DiceCardAbility_FumeRetentionDice3Yaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "[On Clash Win] Increase Boiling Point by 3.";

    public override void OnWinParrying()
    {
		BattleUnitBuf_BoilingPointYaren.AddBuf(owner, 3);
    }
}

public class DiceCardAbility_BoilingPowerDiceYaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "Roll same value as opponent Dice. If it’s a one-sided attack, roll 1.";

    public override void BeforeRollDice_Target(BattleDiceBehavior targetDice)
	{
		// Calc power needed
		targetDice.RollDice();
		behavior.RollDice();
		int targetRoll = this.behavior.TargetDice.DiceResultValue;
		int ownerRoll = this.behavior.DiceResultValue;

		int powerNeeded = targetRoll - ownerRoll;

		// If power null take it out, as it will break script
		bool powerNullBool = owner.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_nullifyPower);
		if (powerNullBool)
		{
			BattleUnitBuf_nullifyPower powerNull = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_nullifyPower) as BattleUnitBuf_nullifyPower;
			powerNull.Destroy();
		}

		// Give power to clash as tie.
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			power = powerNeeded
		});

		// Re-Apply power null if needed
		if (powerNullBool) owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.NullifyPower, 1, owner);
	}
}

public class DiceCardAbility_OverloadDiceYaren : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "ChainReactionYaren" };
	public static string Desc = "[On Hit] Inflict 0~9 stacks of Chain Reaction.";

    public override void OnSucceedAttack(BattleUnitModel target)
    {
		BattleUnitBuf_ChainReactionYaren.AddBuf(target, RandomUtil.Range(0, 9));
    }
}

/////////////////////////////// Dice Passives ///////////////////////////////





/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_StartupYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Use] Destroy all dice in this page and make 1~3 dice. They will randomly be decided to either be Slash or Block dice. [On Use] Restore 2 Light and Draw 2 Pages.";

    public override void OnUseCard()
    {
		// Restore Light and draw Pages
		this.owner.allyCardDetail.DrawCards(2);
		this.owner.cardSlotDetail.RecoverPlayPoint(2);

		// Dont use randomutil range in a for loop, game softlocks.
		int diceAmount = RandomUtil.Range(1, 3);

		// Destroy the base dice
		base.card?.DestroyDice(DiceMatch.AllDice);

		// Make 1~3 dice
		for (int i = 0; i < diceAmount; i++)
		{
			// Get min and max
			int min = RandomUtil.Range(1, 10);
			int max = RandomUtil.Range(min, 10);
			// Slash or Block
			BehaviourDetail detail = RandomUtil.SelectOne<BehaviourDetail>(BehaviourDetail.Slash, BehaviourDetail.Guard);
			// Declare and assign BehaviourDetail as necessary based on detail
			BehaviourType type = BehaviourType.Standby;
			if (detail == BehaviourDetail.Slash) type = BehaviourType.Atk;
			if (detail == BehaviourDetail.Guard) type = BehaviourType.Def;
			// Declare and assign MotionDetail as necessary based on detail
			MotionDetail mdetail = MotionDetail.N;
			if (detail == BehaviourDetail.Slash) mdetail = MotionDetail.J;
			if (detail == BehaviourDetail.Guard) mdetail = MotionDetail.G;
			// Script for dice
			string script = "";
			// ActionScript for dice
			string actionscript = "";
			// VFX for the dice
			string effectres = "";

			// Create and add dice to page
			this.card.AddDice(creatediefromtheaether(min, max, detail, type, mdetail, script, actionscript, effectres));
		}
	}

	public static BattleDiceBehavior creatediefromtheaether(int min, int max, BehaviourDetail detail, BehaviourType type, MotionDetail mdetail = MotionDetail.N, string script = "", string actionscript = "", string effectres = "")
	{
		BattleDiceBehavior thing = new BattleDiceBehavior
		{
			behaviourInCard = new DiceBehaviour
			{
				Min = min,
				Dice = max,
				ActionScript = actionscript,
				Desc = "",
				Detail = detail,
				EffectRes = effectres,
				MotionDetail = mdetail,
				MotionDetailDefault = MotionDetail.N,
				Script = script,
				Type = type
			}
		};
		return thing;
	}
}

public class DiceCardSelfAbility_GearShiftYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "MaxRollUpTopStar" };
	public static string Desc = "[On Combat Phase Begin] Turn all Haste into Max Roll Up.";

    public override void OnStartBattle()
    {
		// Find all haste
		BattleUnitBuf_quickness haste = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_quickness) as BattleUnitBuf_quickness;
		// Give Max Roll Up
		BattleUnitBuf_MaxRollUpTopStar.AddBuf(owner, haste.stack);
		// Destroy Haste
		haste.Destroy();
	}
}

public class DiceCardSelfAbility_PedaltToTheMetalYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Use] Gain 2 Haste next turn.";

    public override void OnUseCard()
    {
		owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 2, owner);
    }
}

public class DiceCardSelfAbility_BackingUpYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Combat Phase Begin] Gain 2 Endurance this and next turn.";

	public override void OnStartBattle()
	{
		owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Endurance, 2, owner);
		owner.bufListDetail.AddKeywordBufThisRoundByCard(KeywordBuf.Endurance, 2, owner);
	}
}

public class DiceCardSelfAbility_EnginePowerUp : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "[On Use] Restore 1 Light and increase Boiling Point by 3.";

	public override void OnUseCard()
	{
		owner.cardSlotDetail.RecoverPlayPoint(1);
		BattleUnitBuf_BoilingPointYaren.AddBuf(owner, 3);
	}
}

public class DiceCardSelfAbility_FumeRetentionYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "[On Use] Draw 1 Page and increase Boiling Point by 1.";

	public override void OnUseCard()
	{
		owner.allyCardDetail.DrawCards(1);
		BattleUnitBuf_BoilingPointYaren.AddBuf(owner, 1);
	}
}

public class DiceCardSelfAbility_BoilingPowerYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "[On Use] For every 12 boiling points, increase this page’s cost (max of 9). Dice on this page will always deal additional DMG equal to the user’s Boiling Point.";


	public BattleUnitBuf_BoilingPointYaren boilingPoint;

	public override void OnRoundStart_inHand(BattleUnitModel unit, BattleDiceCardModel self)
    {
		// Determine value of card
		boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;
		int value = 1 + (boilingPoint.stack / 12);
		if (value > 9) value = 9;

		self.SetCurrentCost(value);
    }

    public override void OnDrawParrying()
    {
		boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;
		this.card.target.TakeDamage(boilingPoint.stack);
    }

    public override void OnLoseParrying()
	{
		boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;
		this.card.target.TakeDamage(boilingPoint.stack);
	}

	public override void OnSucceedAttack()
	{
		boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;
		this.card.target.TakeDamage(boilingPoint.stack);
	}
}

public class DiceCardSelfAbility_OverloadYaren : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "BoilingPointYaren" };
	public static string Desc = "Dices on this page hit all other characters. Dice on this page only deal half DMG. Additionally, characters in the enemy faction take DMG equal to Boiling Point. Once the page has finished, set Boiling Point count to 0.";


	BattleUnitBuf_BoilingPointYaren boilingPoint;


	public override void OnSucceedAreaAttack(BattleUnitModel target)
    {
		boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;

		if (target.faction != owner.faction) target.TakeDamage(boilingPoint.stack);
    }

    public override void OnEndAreaAttack()
    {
		boilingPoint.stack = 0;
    }
}

/////////////////////////////// Page Passives ///////////////////////////////





///////////////////////////// Key Page Passives /////////////////////////////

// Always passive
public class PassiveAbility_TheTankIsHereYaren : PassiveAbilityBase
{
	// All teammates become untargetable, every third scene this effects is dispelled.
	// If no allies remain, this effect is only triggered every third scene but for this character.
	// Any one-sided page is responded with 1~3 Defense Dice (6-9 roll value).


	// List of teammates
	List<BattleUnitModel> team;
	// Keeps track of scenes
	int sceneCount = 0;

    // Make allies or self untargetable
    public override void OnRoundStart()
    {
		// Assign variables
		team = BattleObjectManager.instance.GetAliveList(owner.faction).FindAll((BattleUnitModel x) => x != owner);
		sceneCount += 1;

		// On scene 1 and 2 of rotation
		if (sceneCount != 3)
		{
			// Give buff to every teammate
			foreach (BattleUnitModel teammate in team) BattleUnitBuf_YarensAlliesAreUntargetable.AddBuf(teammate, 0);
		}
		// On scene 3 of rotation
		else if (sceneCount == 3)
        {
			// If alone give buff to self
			if (team.Count() == 0) BattleUnitBuf_YarensAlliesAreUntargetable.AddBuf(owner, 0);
			// Reset scene count
			sceneCount = 0;
        }
	}

    // Creates block dice
    public override void OnStartBattle()
    {
		// Amount of def dice to make.
		int defDice = RandomUtil.Range(1, 3);

		// Get the page that already had this block dice
		BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(1100022));
		if (battleDiceCardModel == null)
		{
			return;
		}
		while (defDice != 0)
		{
			foreach (BattleDiceBehavior item in battleDiceCardModel.CreateDiceCardBehaviorList())
			{
				owner.cardSlotDetail.keepCard.AddBehaviourForOnlyDefense(battleDiceCardModel, item);
			}

			defDice--;
		}
	}

    // Creates block dice
    public override void OnStartTargetedOneSide(BattlePlayingCardDataInUnitModel attackerCard)
	{
		// Amount of def dice to make.
		int defDice = RandomUtil.Range(1, 3);

		// Get the page that already had this block dice
		BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(1100022));
		if (battleDiceCardModel == null)
		{
			return;
		}
		while (defDice != 0)
		{
			foreach (BattleDiceBehavior item in battleDiceCardModel.CreateDiceCardBehaviorList())
			{
				owner.cardSlotDetail.keepCard.AddBehaviourForOnlyDefense(battleDiceCardModel, item);
			}

			defDice--;
		}
	}
}

// EGO Passives
public class PassiveAbility_BoilingPointManager : PassiveAbilityBase
{
    // Every scene the character takes DMG, increases the Boiling Point by either the roll of the enemy dice or by the damage received (whichever is higher).

    public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
    {
        if (dmg > atkDice.DiceResultValue) BattleUnitBuf_BoilingPointYaren.AddBuf(owner, dmg);
		if (dmg < atkDice.DiceResultValue) BattleUnitBuf_BoilingPointYaren.AddBuf(owner, atkDice.DiceResultValue);
		if (dmg == atkDice.DiceResultValue) BattleUnitBuf_BoilingPointYaren.AddBuf(owner, dmg);
	}
}

public class PassiveAbility_BlockDicePowerUpYaren : PassiveAbilityBase
{
	// Defensive dice gain 2 power. If a page has both Attack and Defensive dice, only gain 1.
	public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
	{
		// Variables to check.
		bool hasDefense = false;
		bool hasAttack = false;

		// Get all dice in page.
		List<BehaviourDetail> list = new List<BehaviourDetail>();
		foreach (DiceBehaviour b in curCard.GetOriginalDiceBehaviorList())
		{
			if (b.Detail != BehaviourDetail.None && !list.Exists((BehaviourDetail x) => x == b.Detail))
			{
				list.Add(b.Detail);
			}
		}

		// Find atleast 1 attack and 1 defencive dice.
		foreach (BehaviourDetail diceType in list)
        {
			if (diceType == BehaviourDetail.Guard || diceType == BehaviourDetail.Evasion) hasDefense = true;
			if (diceType == BehaviourDetail.Hit || diceType == BehaviourDetail.Slash || diceType == BehaviourDetail.Penetrate) hasAttack = true;
		}

		// If has both only 1 power.
		if (hasAttack && hasDefense)
		{
			curCard.ApplyDiceStatBonus(DiceMatch.AllDefenseDice, new DiceStatBonus
			{
				power = 1
			});
			owner.battleCardResultLog?.SetPassiveAbility(this);
		}

		// If has only defensive 2 power.
		if (!hasAttack && hasDefense)
		{
			curCard.ApplyDiceStatBonus(DiceMatch.AllDefenseDice, new DiceStatBonus
			{
				power = 2
			});
			owner.battleCardResultLog?.SetPassiveAbility(this);
		}
	}
}

public class PassiveAbility_HalfDMGBurnYaren : PassiveAbilityBase
{
	// Only takes half DMG from burn.

	private float _burnDmgFactor = 0.5f;

	public override float DmgFactor(int dmg, DamageType type = DamageType.ETC, KeywordBuf keyword = KeywordBuf.None)
	{
		if (keyword == KeywordBuf.Burn)
		{
			return _burnDmgFactor;
		}
		return base.DmgFactor(dmg, type, keyword);
	}
}

public class PassiveAbility_EgoPageAndDistortYaren : PassiveAbilityBase
{
    // Gain EGO page. Once the conditions are met, Distort.

    public override void OnUnitCreated()
    {
		base.owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 95)); // change value once card is made.
		BattleUnitBuf_BoilingPointYaren.AddBuf(owner, 0);
	}

    // Gotta write code to destroy these passives and give the distortion passives.
    public override void OnRoundEndTheLast()
	{
		BattleUnitBuf_BoilingPointYaren boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;

		if (boilingPoint.burnInflicted >= 30)
		{
			// Change Passives
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 32));
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 33));
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 34));
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 35));
			owner.passiveDetail.DestroyPassive(owner.passiveDetail.FindPassive<PassiveAbility_BlockDicePowerUpYaren>());
			owner.passiveDetail.DestroyPassive(owner.passiveDetail.FindPassive<PassiveAbility_BoilingPointManager>());

			// Restore Break
			owner.RecoverBreakLife(999);

			// Change Passives (this one's here cuss it would not trigger anything after it)
			owner.passiveDetail.DestroyPassive(this);
		}
	}
}

// Distortion Passives
public class PassiveAbility_ChainReactionManager : PassiveAbilityBase
{
    // When dealing DMG, inflict 1~2 Chain Reaction. When hit by this character always trigger Chain Reaction.

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
		BattleUnitBuf_ChainReactionYaren.AddBuf(behavior.TargetDice.owner, RandomUtil.Range(1, 2));
    }
}

public class PassiveAbility_AttackDicePowerUpYaren : PassiveAbilityBase
{
	// Attack dice gain 2 power. If a page has both Attack and Defensive dice, only gain 1.
	public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
	{
		// Variables to check.
		bool hasDefense = false;
		bool hasAttack = false;

		// Get all dice in page.
		List<BehaviourDetail> list = new List<BehaviourDetail>();
		foreach (DiceBehaviour b in curCard.GetOriginalDiceBehaviorList())
		{
			if (b.Detail != BehaviourDetail.None && !list.Exists((BehaviourDetail x) => x == b.Detail))
			{
				list.Add(b.Detail);
			}
		}

		// Find atleast 1 attack and 1 defencive dice.
		foreach (BehaviourDetail diceType in list)
		{
			if (diceType == BehaviourDetail.Guard || diceType == BehaviourDetail.Evasion) hasDefense = true;
			if (diceType == BehaviourDetail.Hit || diceType == BehaviourDetail.Slash || diceType == BehaviourDetail.Penetrate) hasAttack = true;
		}

		// If has both only 1 power.
		if (hasAttack && hasDefense)
		{
			curCard.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
			{
				power = 1
			});
			owner.battleCardResultLog?.SetPassiveAbility(this);
		}

		// If has only attack 2 power.
		if (hasAttack && !hasDefense)
		{
			curCard.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
			{
				power = 2
			});
			owner.battleCardResultLog?.SetPassiveAbility(this);
		}
	}
}

public class PassiveAbility_AllDefensiveDiceToAttackYaren : PassiveAbilityBase
{
	// All dice in pages used, turn into Attack dice. (Evasive dice turn into Slash dice and Block dice turn into Blunt dice)

	// Prepare variables to store new page created
	public class transformedPageInfo
	{
		public BattleDiceCardModel transformedPage;

		public BattleDiceCardModel equippedCard;
	}

	// List to store the cards that were changed
	public List<transformedPageInfo> transformedPageInfoList;
	
	// Tranform dice
	public void SettransformedPage(BattleDiceCardModel equippedCard)
	{
		// New list card
		transformedPageInfoList = new List<transformedPageInfo>();

		// If no page found, stop
		if (equippedCard == null)
		{
			return;
		}
		// Else continue
		// Get info of original page and store it
		transformedPageInfo transformedPageInfo = new transformedPageInfo();
		transformedPageInfo.equippedCard = equippedCard;
		BattleDiceCardModel battleDiceCardModel = BattleDiceCardModel.CreatePlayingCard(equippedCard.XmlData);
		List<DiceBehaviour> list = new List<DiceBehaviour>();
		// For each dice in equiped page.
		foreach (DiceBehaviour behaviour in battleDiceCardModel.GetBehaviourList())
		{
			// Set variable that stores original dice.
			DiceBehaviour diceBehaviour = behaviour.Copy();
			
			// We don't need this because we only want to change Block Dice.
			/*
			if (behaviour.Type == BehaviourType.Atk)
			{
				diceBehaviour.Type = BehaviourType.Def;
				diceBehaviour.Detail = RandomUtil.SelectOne(new List<BehaviourDetail>
				{
					BehaviourDetail.Guard,
					BehaviourDetail.Evasion
				});
			}
			*/

			// Change block dice to respective attacks.
			if (behaviour.Type == BehaviourType.Def)
			{
				// Set to attack
				diceBehaviour.Type = BehaviourType.Atk;

				// Change to specific
				if (diceBehaviour.Detail == BehaviourDetail.Evasion) diceBehaviour.Detail = BehaviourDetail.Slash;
				if (diceBehaviour.Detail == BehaviourDetail.Guard) diceBehaviour.Detail = BehaviourDetail.Hit;
			}
			// Counter dice thats originally block, make it atack.
			else if (behaviour.Type == BehaviourType.Standby)
			{
				if (diceBehaviour.Detail == BehaviourDetail.Evasion) diceBehaviour.Detail = BehaviourDetail.Slash;
				if (diceBehaviour.Detail == BehaviourDetail.Guard) diceBehaviour.Detail = BehaviourDetail.Hit;
			}
			list.Add(diceBehaviour);
		}
		// Basically make the card
		battleDiceCardModel.XmlData.DiceBehaviourList = list;
		// Store it in the originally made variable to storew the changed card.
		transformedPageInfo.transformedPage = battleDiceCardModel;
		// Add the class for Cards changed
		transformedPageInfoList.Add(transformedPageInfo);
	}

	public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
	{
		if (card?.card != null) SettransformedPage(card.card);

		// If no card is equiped or this card wasnt changed. Do nothing.
		if (!transformedPageInfoList.Exists((transformedPageInfo x) => x.equippedCard == card.card))
		{
			return;
		}
		else
        {
			// Get the changed card.
			transformedPageInfo getClassOfCard = transformedPageInfoList.Find((transformedPageInfo x) => x.equippedCard == card.card);
			BattleDiceCardModel theCard = getClassOfCard.transformedPage;

			// Get list of dice in transformed cards.
			List<BattleDiceBehavior> list = theCard.CreateDiceCardBehaviorList();
			// Destroy current dice.
			card.RemoveAllDice();
			// Set the changed dice.
			foreach (BattleDiceBehavior item in list)
			{
				card.AddDice(item);
			}
		}
	}

	public override void OnRoundEndTheLast()
	{
		// After the Scene ends, clear the lying cards.
		transformedPageInfoList.Clear();
	}
}

public class PassiveAbility_UnDistortYaren : PassiveAbilityBase
{
    // When 3 Scenes have gone by after Distorting, dispel Distortion. (Does not occur in some specific Story nodes). If Boiling Point is at 50 or more when this occurs, set Boiling Point to 0.

    // Gotta write code to destroy these passives and give the EGO passives.
    public override void OnRoundEndTheLast()
	{
		// Un-Distorts
		BattleUnitBuf_BoilingPointYaren boilingPoint = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_BoilingPointYaren) as BattleUnitBuf_BoilingPointYaren;

		if (boilingPoint.roundsPassed == 3)
        {
			// Change passives
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 28));
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 29));
			owner.passiveDetail.AddPassive(new LorId("MyTopStar", 31));
			owner.passiveDetail.DestroyPassive(owner.passiveDetail.FindPassive<PassiveAbility_ChainReactionManager>());
			owner.passiveDetail.DestroyPassive(owner.passiveDetail.FindPassive<PassiveAbility_AttackDicePowerUpYaren>());
			owner.passiveDetail.DestroyPassive(owner.passiveDetail.FindPassive<PassiveAbility_AllDefensiveDiceToAttackYaren>());
			
			// Reser values
			boilingPoint.roundsPassed = 0;
			boilingPoint.isDistorted = false;
			boilingPoint.burnInflicted = 0;

			if (boilingPoint.stack >= 50) boilingPoint.stack = 0;
			
			// Change Passives (this one's here cuss it would not trigger anything after it)
			owner.passiveDetail.DestroyPassive(this);
        }
	}

	public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
	{
		return AtkResist.Weak;
	}

	public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
	{
		return AtkResist.Immune;
	}
}

///////////////////////////// Key Page Passives /////////////////////////////