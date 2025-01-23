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

/////////////////////////////// Buffs / Effects /////////////////////////////





/////////////////////////////// BehaviourAction /////////////////////////////

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

/////////////////////////////// BehaviourAction /////////////////////////////





/////////////////////////////// Dice Passives ///////////////////////////////

public class DiceCardAbility_DiceAbility : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[]";
}

/////////////////////////////// Dice Passives ///////////////////////////////





/////////////////////////////// Page Passives ///////////////////////////////

public class DiceCardSelfAbility_DiceCardPassive : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[]";
}

/////////////////////////////// Page Passives ///////////////////////////////





///////////////////////////// Key Page Passives /////////////////////////////

public class PassiveAbility_Passive : PassiveAbilityBase
{
	//
}

///////////////////////////// Key Page Passives /////////////////////////////