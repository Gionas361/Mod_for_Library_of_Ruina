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
using DDJJ_DLL_s;




/////////////////////////////// Buffs / Effects /////////////////////////////

/*
public class BattleUnitBuf_MaxRollUpTopStar : BattleUnitBuf
{
	protected override string keywordId => "";
	protected override string keywordIconId => "";

	public BattleUnitBuf_MaxRollUpTopStar(BattleUnitModel model)
	{
		this._owner = model;
		this.stack = 0;
	}


	////////////////////////////////////////////// Effects of Buff ///

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
		BattleUnitBuf_MaxRollUpTopStar BattleUnitBuf_MaxRollUpTopStar = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_MaxRollUpTopStar) as BattleUnitBuf_MaxRollUpTopStar;
		if (BattleUnitBuf_MaxRollUpTopStar == null)
		{
			BattleUnitBuf_MaxRollUpTopStar BattleUnitBuf_MaxRollUpTopStar2 = new BattleUnitBuf_MaxRollUpTopStar(unit);
			BattleUnitBuf_MaxRollUpTopStar2.Add(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_MaxRollUpTopStar2);
		}
		else
		{
			BattleUnitBuf_MaxRollUpTopStar.Add(stack);
		}
	}

	public static void SubBuf(BattleUnitModel unit, int amount)
	{
		BattleUnitBuf_MaxRollUpTopStar maxRollUpTopStar = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_MaxRollUpTopStar) as BattleUnitBuf_MaxRollUpTopStar;
		maxRollUpTopStar.IntegerDeleto(amount);
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
*/

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

// Armored Tackle
public class DiceCardAbility_Inflict10StardustDDJJ : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Inflict 10 Stardust.";

	public override void OnSucceedAttack()
	{
		BattleUnitBuf_DDJJsStardust.AddBuf(base.card.target, 10);
	}
}

// Double Chain Trigger
public class DiceCardAbility_TriggerStardustBurstAndReuseDiceOnce : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Trigger Stardust Burst and [Reuse this Dice] (up to 1 time.)";

	bool reused = false;

	public override void OnSucceedAttack()
	{
		BattleUnitBuf_DDJJsStardust.BurstItAll(base.card.target, owner);

		if (!reused) ActivateBonusAttackDice();
		
		reused = true;
	}
}

// Wing Swing
public class DiceCardAbility_Transfer5StarbustFromSelfToTarget : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Transfer 5 Stardust from self to target.";

	public override void OnSucceedAttack()
	{
		BattleUnitBuf_DDJJsStardust.Transfer(base.card.target, owner, 5);
	}
}

// Wing Swing
public class DiceCardAbility_Transfer5StarbustFromSelfToTargetAndStardustBurst : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Transfer 5 Stardust from self to target. And trigger Stardust Burst on target.";

    public override void OnSucceedAttack()
    {
		BattleUnitBuf_DDJJsStardust.Transfer(base.card.target, owner, 5);
		BattleUnitBuf_DDJJsStardust.BurstItAll(base.card.target, owner);
	}
}

// Focused Stardust
public class DiceCardAbility_MaxRollByStardustAndTransferStardust : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[Before Roll] Increase max roll by Stardust on self. [On Hit] Transfer all Stardust on self to target.";

    public override void BeforeRollDice()
    {
		// Get stack amount
		BattleUnitBuf_DDJJsStardust dDJJsStardustEffect = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;

		// Increase Max roll
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			max = dDJJsStardustEffect.stack
		});
	}

    public override void OnSucceedAttack()
    {
		// Get stack amount
		BattleUnitBuf_DDJJsStardust dDJJsStardustEffect = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;

		// Transfer stacks
		BattleUnitBuf_DDJJsStardust.Transfer(base.card.target, owner, dDJJsStardustEffect.stack);
    }
}

// Retraction
public class DiceCardAbility_ClashLose3StardustOnSelf : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Clash Lose] Inflict 3 Stardust on self.";

    public override void OnLoseParrying()
	{
		BattleUnitBuf_DDJJsStardust.AddBuf(owner, 3);
	}
}

// Retraction
public class DiceCardAbility_Inflict3StardustDDJJ : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Inflict 3 Stardust on target.";

	public override void OnSucceedAttack()
	{
		BattleUnitBuf_DDJJsStardust.AddBuf(base.card.target, 3);
	}
}

// Stardust Barrage
public class DiceCardAbility_Inflict1StardustDDJJ : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Inflict 1 Stardust on user and target.";

	public override void OnSucceedAttack()
	{
		BattleUnitBuf_DDJJsStardust.AddBuf(base.card.target, 1);
		BattleUnitBuf_DDJJsStardust.AddBuf(owner, 1);
	}
}

// Stardust Barrage
public class DiceCardAbility_Reuse8TimesForStardust : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[Reuse this dice] (up to 8 times.)";

	int timesLeftToReroll = 8;

    public override void OnSucceedAttack()
    {
		if (timesLeftToReroll != 0) ActivateBonusAttackDice();
		timesLeftToReroll--;
	}

    public override void OnLoseParrying()
    {
		if (timesLeftToReroll != 0) ActivateBonusAttackDice();
		timesLeftToReroll--;
	}
}

// Metamorphosis
public class DiceCardAbility_Inflict3StardustAndTriggerBurst : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Hit] Inflict 3 Stardust and trigger Stardust Burst.";

	public override void OnSucceedAttack()
	{
		BattleUnitBuf_DDJJsStardust.AddBuf(base.card.target, 3);
		BattleUnitBuf_DDJJsStardust.BurstItAll(base.card.target, owner);
	}
}

/////////////////////////////// Dice Passives ///////////////////////////////





/////////////////////////////// Page Passives ///////////////////////////////

// Wing Swing
public class DiceCardSelfAbility_Gain15StardustAndTriggerStardustBurstOnSelf : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Use] Gain 15 Stardust. [After Clash End] Trigger Stardust Burst on user.";

    public override void OnUseCard()
    {
		BattleUnitBuf_DDJJsStardust.AddBuf(owner, 15);
    }

    public override void OnEndBattle()
    {
		BattleUnitBuf_DDJJsStardust.BurstItAll(owner, owner);
    }
}

// Focused Stardust
public class DiceCardSelfAbility_Gain10StarburstAndTriggerBurstOnSelf : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Use] Gain 10 Stardust. [After Clash End] Trigger Stardust Burst on user.";

	public override void OnUseCard()
	{
		BattleUnitBuf_DDJJsStardust.AddBuf(owner, 10);
	}

	public override void OnEndBattle()
	{
		BattleUnitBuf_DDJJsStardust.BurstItAll(owner, owner);
	}
}

// Retraction
public class DiceCardSelfAbility_Restore3LightDraw2PagesAndGain10Stardust : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Use] Restore 3 Light, draw 2 Pages and Inflict 10 Stardust to the user.";

	public override void OnUseCard()
	{
		this.owner.allyCardDetail.DrawCards(2);
		this.owner.cardSlotDetail.RecoverPlayPoint(3);
		BattleUnitBuf_DDJJsStardust.AddBuf(owner, 10);
	}
}

// Stardust Barrage
public class DiceCardSelfAbility_Give1StardustPassiveToDice : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Use] Dice in this page will: \"[On Hit] Inflict 1 Stardust on user and target.\"";

	public override void OnUseCard()
	{
		card.ApplyDiceAbility(DiceMatch.AllDice, new DiceCardAbility_Inflict1StardustDDJJ());
	}
}

// Metamorphosis
public class DiceCardSelfAbility_Restore2LightDraw2PagesGain5Stardust : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "DDJJsStardust" };
	public static string Desc = "[On Use] Restore 2 Light, draw 2 Pages and inflict 5 Stardust on self.";

    public override void OnUseCard()
    {
		this.owner.allyCardDetail.DrawCards(2);
		this.owner.cardSlotDetail.RecoverPlayPoint(2);
		BattleUnitBuf_DDJJsStardust.AddBuf(owner, 5);
	}
}

/////////////////////////////// Page Passives ///////////////////////////////





///////////////////////////// Key Page Passives /////////////////////////////

// Sick Affliction
public class PassiveAbility_hitApplyRandomEffectsStardust : PassiveAbilityBase
{
	// Will inflict a pre-determined amount of stacks depending on the effect rolled. This occurs upon landing a hit.

	public bool triggeredEffects = false;

	public override void OnSucceedAttack(BattleDiceBehavior behavior)
	{
		applyEffects(behavior.TargetDice.owner);

	}

	public override void OnWinParrying(BattleDiceBehavior behavior)
	{
		applyEffects(behavior.TargetDice.owner);

	}

	public void applyEffects(BattleUnitModel target)
	{
		int effectCalc = 0;

		if (!triggeredEffects)
		{
			effectCalc = RandomUtil.Range(0, 34);

			if (effectCalc == 0) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Decay, 2);
			if (effectCalc == 1) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 4);
			if (effectCalc == 2) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Paralysis, 2);
			if (effectCalc == 3) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, 10);
			if (effectCalc == 4) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, 1);
			if (effectCalc == 5) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable_break, 1);
			if (effectCalc == 6) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Weak, 1);
			if (effectCalc == 7) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Disarm, 1);
			if (effectCalc == 8) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 1);
			if (effectCalc == 10) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 1);
			if (effectCalc == 11) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1);
			if (effectCalc == 12) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1);
			if (effectCalc == 13) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1);
			if (effectCalc == 14) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1);
			if (effectCalc == 15) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Stun, 1);
			if (effectCalc == 16) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DmgUp, 1);
			if (effectCalc == 17) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.SlashPowerUp, 1);
			if (effectCalc == 18) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.PenetratePowerUp, 1);
			if (effectCalc == 19) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HitPowerUp, 1);
			if (effectCalc == 20) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DefensePowerUp, 1);
			if (effectCalc == 21) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.WarpCharge, 3);
			if (effectCalc == 22) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Smoke, 5);
			if (effectCalc == 23) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.NullifyPower, 1);
			if (effectCalc == 24) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HalfPower, 1);
			if (effectCalc == 25) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 1);
			if (effectCalc == 26) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BurnSpread, 3);
			if (effectCalc == 27) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Nail, 5);
			if (effectCalc == 28) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Seal, 1);
			if (effectCalc == 29) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.ForbidRecovery, 1);
			if (effectCalc == 30) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Blurry, 1);
			if (effectCalc == 31) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DecreaseSpeedTo1, 1);
			if (effectCalc == 32) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HeavySmoke, 6);
			if (effectCalc == 33) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Fairy, 5);
			if (effectCalc == 34) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.NicolaiTarget, 1);
		}

		triggeredEffects = true;
	}
}

// Stardust Spreader
public class PassiveAbility_stardustSpreadByDDJJ : PassiveAbilityBase
{
    // On round start will inflict Stardust to EVERY single character in the stage. Probably around 1~5.
    // When getting hit, inflict Stardust, 1~3.

    public override void OnRoundStart()
    {
		// Inflict on onwer faction
        foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(this.owner.faction)) {
			BattleUnitBuf_DDJJsStardust.AddBuf(unit, RandomUtil.Range(1, 5));
		}

		// Get enemy faction
		List<BattleUnitModel> aliveList_opponent = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
		// Inflict on enemy faction
		foreach (BattleUnitModel unit in aliveList_opponent)
		{
			BattleUnitBuf_DDJJsStardust.AddBuf(unit, RandomUtil.Range(1, 5));
		}
	}

    public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
    {
		// When getting hit inflict.
		BattleUnitBuf_DDJJsStardust.AddBuf(atkDice.owner, RandomUtil.Range(1, 3));
    }
}

// Carrier of a Million Sickeness's
public class PassiveAbility_stardustBurstEffectApplication : PassiveAbilityBase
{
    // Check on End of Scene. If Stardust Burst was triggered at any point in the scene to a unit,
    // for every stack of Stardust lost by that unit when Stardust Burst triggered, activate the random effect application effect
	// by the amount of Stardust lost.

    public override void OnRoundEnd()
    {
		// Inflict on onwer faction
		foreach (BattleUnitModel unit in BattleObjectManager.instance.GetAliveList(this.owner.faction).FindAll((BattleUnitModel x) => x != owner))
		{
			// Check if the unit has been triggered
			bool BattleUnitBuf_stardustBurstTriggered = unit.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggered);

			// If yes...
			if (BattleUnitBuf_stardustBurstTriggered)
            {
				// Get the stack amount
				BattleUnitBuf_stardustBurstTriggered dDJJsStardustEffect = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggered) as BattleUnitBuf_stardustBurstTriggered;
				// For each stack,  Roll gacha of effects
				for (int i = 0; i < dDJJsStardustEffect.stack; i++) applyEffects(unit);

				dDJJsStardustEffect.Destroy();
			}
		}

		// Get enemy faction
		List<BattleUnitModel> aliveList_opponent = BattleObjectManager.instance.GetAliveList_opponent(owner.faction);
		// Inflict on enemy faction
		foreach (BattleUnitModel unit in aliveList_opponent)
		{
			// Check if the unit has been triggered
			bool BattleUnitBuf_stardustBurstTriggered = unit.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggered);

			// If yes...
			if (BattleUnitBuf_stardustBurstTriggered)
			{
				// Get the stack amount
				BattleUnitBuf_stardustBurstTriggered dDJJsStardustEffect = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggered) as BattleUnitBuf_stardustBurstTriggered;
				// For each stack, Roll gacha of effects
				for (int i = 0; i < dDJJsStardustEffect.stack; i++) applyEffects(unit);

				dDJJsStardustEffect.Destroy();
			}
		}
	}

	public void applyEffects(BattleUnitModel target)
	{
		int effectCalc = 0;

		effectCalc = RandomUtil.Range(0, 34);

		if (effectCalc == 0) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Decay, 2);
		if (effectCalc == 1) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 4);
		if (effectCalc == 2) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Paralysis, 2);
		if (effectCalc == 3) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, 10);
		if (effectCalc == 4) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, 1);
		if (effectCalc == 5) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable_break, 1);
		if (effectCalc == 6) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Weak, 1);
		if (effectCalc == 7) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Disarm, 1);
		if (effectCalc == 8) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 1);
		if (effectCalc == 10) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 1);
		if (effectCalc == 11) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1);
		if (effectCalc == 12) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1);
		if (effectCalc == 13) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1);
		if (effectCalc == 14) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1);
		if (effectCalc == 15) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Stun, 1);
		if (effectCalc == 16) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DmgUp, 1);
		if (effectCalc == 17) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.SlashPowerUp, 1);
		if (effectCalc == 18) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.PenetratePowerUp, 1);
		if (effectCalc == 19) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HitPowerUp, 1);
		if (effectCalc == 20) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DefensePowerUp, 1);
		if (effectCalc == 21) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.WarpCharge, 3);
		if (effectCalc == 22) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Smoke, 5);
		if (effectCalc == 23) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.NullifyPower, 1);
		if (effectCalc == 24) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HalfPower, 1);
		if (effectCalc == 25) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 1);
		if (effectCalc == 26) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BurnSpread, 3);
		if (effectCalc == 27) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Nail, 5);
		if (effectCalc == 28) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Seal, 1);
		if (effectCalc == 29) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.ForbidRecovery, 1);
		if (effectCalc == 30) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Blurry, 1);
		if (effectCalc == 31) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DecreaseSpeedTo1, 1);
		if (effectCalc == 32) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HeavySmoke, 6);
		if (effectCalc == 33) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Fairy, 5);
		if (effectCalc == 34) target.bufListDetail.AddKeywordBufByEtc(KeywordBuf.NicolaiTarget, 1);
	}
}

// No more fears.
public class PassiveAbility_drawAndRestoreByStardustBurst : PassiveAbilityBase
{
	// When triggering Stardust Burst draw 1 page and restore 1 light per 5 Stardust lost by the target when the effect triggered.

	public override void OnEndBattle(BattlePlayingCardDataInUnitModel curCard)
    {
		// Calculate amount
		BattleUnitBuf_stardustBurstTriggeredByUser dDJJsStardustEffect = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggeredByUser) as BattleUnitBuf_stardustBurstTriggeredByUser;
		int amount5 = dDJJsStardustEffect.stack / 5;

		// Draw pages and restore light
		this.owner.allyCardDetail.DrawCards(amount5);
		this.owner.cardSlotDetail.RecoverPlayPoint(amount5);
	}
}

// Plotarmor
public class PassiveAbility_dDJJsStardustPlotArmor : PassiveAbilityBase
{
	// If character reaches Emotion Level 3 or its HP is lowered below 30%. Change into Comeback Skin.
	// Additionally gain a passive 2 All Power Up and 3 stacks of Max Dice Roll Up.

	public override void OnRoundStart()
    {
		int emotionLevel = this.owner.emotionDetail.EmotionLevel;

		if (emotionLevel >= 4 || ((owner.MaxHp * 30) / 100) <= owner.hp)
        {
			BattleUnitBuf_MaxRollUpTopStar.AddBuf(this.owner, 3);
			this.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.AllPowerUp, 2);
        }
	}
}

// Friend Spawn
public class PassiveAbility_SpawnGionasAnYaren : PassiveAbilityBase
{
	// Spawn another Unit.
	public override void OnUnitCreated()
	{
		// Spawns Friends.

		// Gionas
		Singleton<StageController>.Instance.GetCurrentWaveModel().SetFormationPosition(1, new Vector2Int(7, -8));
		BattleUnitModel battleUnitModelGionas = Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId("MyTopStar", 8), 1);
		battleUnitModelGionas.passiveDetail.AddPassive(new PassiveAbility_PreventFriendsFromDyingDDJJ());

		// Yaren
		Singleton<StageController>.Instance.GetCurrentWaveModel().SetFormationPosition(2, new Vector2Int(10, -15));
		BattleUnitModel battleUnitModelYaren = Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId("MyTopStar", 8), 2);
		battleUnitModelYaren.passiveDetail.AddPassive(new PassiveAbility_PreventFriendsFromDyingDDJJ());
	}

	public override void OnRoundEnd()
	{
		owner.cardSlotDetail.RecoverPlayPoint(10);
		owner.allyCardDetail.DrawCards(10);
	}
}

// Friends will not Die Again
public class PassiveAbility_PreventFriendsFromDyingDDJJ : PassiveAbilityBase
{
    public override void OnRoundEnd()
    {
		owner.cardSlotDetail.RecoverPlayPoint(10);
		owner.allyCardDetail.DrawCards(10);
    }

    public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
	{
		if (owner.hp <= (float)dmg)
		{
			owner.RecoverHP(999);
		}
		return false;
	}
}

///////////////////////////// Key Page Passives /////////////////////////////