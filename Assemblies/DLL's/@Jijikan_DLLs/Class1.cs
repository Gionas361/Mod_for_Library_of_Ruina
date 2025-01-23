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


/// <summary>
/// 6 base pages:
/// 
/// Cost 1 | 2 Dice | Quick Draw                | dice gain 1 power if axis is -0. Draw 2 pages.
/// Cost 1 | 3 Dice | Solid Base                | Restore 3 light.
/// Cost 2 | 1 Dice | Time Manipulation Barrage | reuse big dice twise.
/// Cost 2 | 2 Dice | MOXIE                     | dice gain 1 power if axis is 5.
/// Cost 3 | 2 Dice | Quick Suppression!        | If speed 8+ Draw 2 pages and Restore 2 light. Otherwise gain 2 haste.
/// Cost 5 | 4 Dice | Axis Break                | only when 5+ axis, if 5+ gain 5 power if 10+ axis 20+ power. On clash win break all of opponent's dice.
/// 
/// 
/// 1 EGO page:
/// 
/// Cost 3 | 3 Dice | Complete Domination       | If all dice land damage activate effect.
/// Cost 4 | 2 Dice | Axis Corrosion            | inflict half the current stack of 时间轴 to target. Set 时间轴 on self to 0.
///
/// 
/// Pages that need Action Scripts:
/// 
/// Done | Time Manipulation Barrage | Teleport in front, backpedal turn dash strikke, turn around enemy, do it again.
/// Nope | Quick Suppression!        | Dash through enemy, Teleport above and crash down, like hong lu tremor id.
/// Nope | Axis Break                | First dice hits, then dash, delay hits till animation done and land all attack immidietly after, in quick succession.
/// Nope | Axis Corrosion            | Upon landing first attack pierce, spiral effect on enemy, clock appears, breaks in half, second hit slashes, effect bursts.
/// </summary>


/////////////////////////////// Buffs / Effects /////////////////////////////

public class BattleUnitBuf_TimeAxis时间轴 : BattleUnitBuf
{
	//Declare public variables
	public bool teamMurdered = false;

	protected override string keywordId => "时间轴";

	protected override string keywordIconId => "时间轴";

	public BattleUnitBuf_TimeAxis时间轴(BattleUnitModel model)
	{
		this._owner = model;
		/*
		typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
		typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
		*/
		this.stack = 0;
		this.teamMurdered = false;
	}


    /////////////////// Effects ///////////////////

    // One-Sided attacks gain 1 时间轴 if teamMurdered = true
    public override void OnStartOneSideAction(BattlePlayingCardDataInUnitModel card)
    {
		if (teamMurdered) Add(1);
    }

    // Haste Related.
    public override int GetSpeedDiceAdder(int speedDiceResult)
	{
		if (this._owner.IsImmune(bufType))
		{
			return base.GetSpeedDiceAdder(speedDiceResult);
		}
		return this.stack / 3;
	}

	// Apply Haste.
	public override void OnRoundStart()
	{
		// Max roll up Effect.
		BattleUnitBuf_MaxRollUpTopStar.AddBuf(this._owner, this.stack / 2);
		
		// Haste.
		if (!this._owner.IsImmune(bufType))
		{
			SingletonBehavior<DiceEffectManager>.Instance.CreateBufEffect("BufEffect_Quickness", this._owner.view);
		}
	}

	// Power Effect.
	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			power = this.stack / 5
		});
	}

	/////////////////// Effects ///////////////////


	/////////// Addition & Substraction ///////////
	public static void AddBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_TimeAxis时间轴 BattleUnitBuf_TimeAxis时间轴 = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;
		if (BattleUnitBuf_TimeAxis时间轴 == null)
		{
			BattleUnitBuf_TimeAxis时间轴 BattleUnitBuf_TimeAxis时间轴2 = new BattleUnitBuf_TimeAxis时间轴(unit);
			BattleUnitBuf_TimeAxis时间轴2.Add(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_TimeAxis时间轴2);
		}
		else
		{
			BattleUnitBuf_TimeAxis时间轴.Add(stack);
		}
	}

	public void Add(int add)
	{
		this.stack += add;

		if (this.stack > 10 && !this.teamMurdered) this.stack = 10;
		else if (this.stack > 20 && this.teamMurdered) this.stack = 20;
	}

	public static void SubBuf(BattleUnitModel unit, int amount)
	{
		BattleUnitBuf_TimeAxis时间轴 bloodofIrahuniquebuf = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;
		bloodofIrahuniquebuf.IntegerDeleto(amount);
	}

	public void IntegerDeleto(int amount)
	{
		this.stack -= amount;

		if (this.stack < -10 && !this.teamMurdered) this.stack = -10;
		else if (this.stack < -20 && this.teamMurdered) this.stack = -20;
	}

	public static void SetBuf(BattleUnitModel unit, int stack)
	{
		BattleUnitBuf_TimeAxis时间轴 BattleUnitBuf_TimeAxis时间轴 = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;
		if (BattleUnitBuf_TimeAxis时间轴 == null)
		{
			BattleUnitBuf_TimeAxis时间轴 BattleUnitBuf_TimeAxis时间轴2 = new BattleUnitBuf_TimeAxis时间轴(unit);
			BattleUnitBuf_TimeAxis时间轴2.BuffSetter(stack);
			unit.bufListDetail.AddBuf(BattleUnitBuf_TimeAxis时间轴2);
			BattleUnitBuf_TimeAxis时间轴2.BuffSetter(stack);
		}
		else
		{
			BattleUnitBuf_TimeAxis时间轴.BuffSetter(stack);
		}
	}

	public void BuffSetter(int amount)
	{
		this.stack = amount;

		if (this.stack > 10 && !this.teamMurdered) this.stack = 10;
		else if (this.stack > 20 && this.teamMurdered) this.stack = 20;
		else if (this.stack < -10 && !this.teamMurdered) this.stack = -10;
		else if (this.stack < -20 && this.teamMurdered) this.stack = -20;
	}
}

public class BattleUnitBuf_MaxRollUpTopStar : BattleUnitBuf
{
	protected override string keywordId => "MaxRollUpTopStar";
	protected override string keywordIconId => "MaxRollUpTopStar";

	public BattleUnitBuf_MaxRollUpTopStar(BattleUnitModel model)
	{
		this._owner = model;
		/*
		typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
		typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
		*/
		this.stack = 0;
	}


	////////////////////////////////////////////// Effects of Buff ///

	// Max Roll
	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			max = this.stack
		});
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

//Time Manipulation Barrage
public class BehaviourAction_TimeManipulationBarrageAction : BehaviourActionBase
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
				movingAction.customEffectRes = "jijikanPierce";
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
//Axis Break
public class BehaviourAction_AxisBreakAction1 : BehaviourActionBase
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
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)1, (CharMoveState)1, 4f, true, 0.07f, 1f);
				opponent.infoList.Add(item);

				// Actually Slashing
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)5, (CharMoveState)1, 25f, false, 0.07f, 2f);
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
public class BehaviourAction_AxisBreakAction2 : BehaviourActionBase
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
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)1, (CharMoveState)1, 4f, true, 0.07f, 1f);
				opponent.infoList.Add(item);

				// Actually Slashing
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)4, (CharMoveState)1, 25f, false, 0.07f, 2f);
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
public class BehaviourAction_AxisBreakAction3 : BehaviourActionBase
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
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)1, (CharMoveState)1, 4f, true, 0.07f, 1f);
				opponent.infoList.Add(item);

				// Actually Slashing
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)6, (CharMoveState)1, 25f, false, 0.07f, 2f);
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
public class BehaviourAction_AxisBreakAction4 : BehaviourActionBase
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
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)1, (CharMoveState)1, 4f, true, 0.9f, 1f);
				opponent.infoList.Add(item);

				// Actually Slashing
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)5, (CharMoveState)2, 25f, false, 0.9f, 1.2f);
				list.Add(movingAction);

				// Controlling opponent
				item = new RencounterManager.MovingAction((ActionDetail)3, (CharMoveState)1, 4f, false, 0.125f, 1f);
				opponent.infoList.Add(item);

				// Effect & DMG
				movingAction = new RencounterManager.MovingAction((ActionDetail)0, (CharMoveState)1, 25f, false, 0.1f, 5f);
				movingAction.customEffectRes = "BoomEffect";
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

//Time Manipulation Barrage
public class BehaviourAction_QuickSuppression1 : BehaviourActionBase
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
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)3, (CharMoveState)1, 4f, false, 0.125f, 1f);
				opponent.infoList.Add(item);


				// Gio, if u forgot what each variable in this string means, just hover over Moving Action
				// And click the field ur confused on, u will see the numbers and what they represent gl :)
				// Attack Sprite   // If it moves & the direction   // Moving Distance   // If looks at Opponent   // Delay   // Speed
				// Actually Slashing
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)5, (CharMoveState)2, 25f, false, 0.3f, 2f);
				movingAction.customEffectRes = "jijikanPierce";
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
public class BehaviourAction_QuickSuppression2 : BehaviourActionBase
{
	// Not sure why its here but ok.
	public override bool IsMovable()
	{
		return false;
	}
	public override bool IsOpponentMovable()
	{
		return false;
	}

	private BattleUnitModel _opponent;
	private bool _MovedTP = false;
	private bool _MovedLand = false;
	private BattleUnitModel _owner;

	// Actually handles the moving and shit.
	public override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
	{
		// Assign unit values
		this._owner = self.view.model;
		this._opponent = opponent.view.model;

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
				RencounterManager.MovingAction item = new RencounterManager.MovingAction((ActionDetail)3, (CharMoveState)1, 4f, false, 0.125f, 1f);
				opponent.infoList.Add(item);

				self.view.EnableDiceActionUI(false);
				opponent.view.EnableDiceActionUI(false);
				self.view.unitBottomStatUI.EnableCanvas(false);
				self.view.LockPosY(false);
				// Gio, if u forgot what each variable in this string means, just hover over Moving Action
				// And click the field ur confused on, u will see the numbers and what they represent gl :)
				// Attack Sprite   // If it moves & the direction   // Moving Distance   // If looks at Opponent   // Delay   // Speed
				// Teleport
				RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)13, (CharMoveState)17, 1f, false, 0.3f, 1f);
				movingAction.SetEffectTiming(EffectTiming.NONE, EffectTiming.NONE, EffectTiming.NONE);
				movingAction.SetCustomMoving(new RencounterManager.MovingAction.MoveCustomEvent(this.TPAboveEnemy));
				list.Add(movingAction);

				// Go down
				RencounterManager.MovingAction movingAction2 = new RencounterManager.MovingAction((ActionDetail)13, (CharMoveState)17, 1f, false, 1f, 2f);
				movingAction2.SetEffectTiming(EffectTiming.NONE, EffectTiming.NONE, EffectTiming.NONE);
				movingAction2.SetCustomMoving(new RencounterManager.MovingAction.MoveCustomEvent(this.Land));
				movingAction2.customEffectRes = "jijikanPierce";
				list.Add(movingAction2);

				// Ka-boom
				//Self
				RencounterManager.MovingAction movingAction3 = new RencounterManager.MovingAction((ActionDetail)6, (CharMoveState)0, 1f, true, 0f, 1f);
				movingAction3.SetEffectTiming(0, 0, 0);
				movingAction3.customEffectRes = "jijikanPierce";
				// Enemy
				RencounterManager.MovingAction item2 = new RencounterManager.MovingAction((ActionDetail)3, (CharMoveState)3, 20f, true, 0.125f, 2f);
				//Apply
				opponent.infoList.Add(item2);
				list.Add(movingAction3);
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

	private bool TPAboveEnemy(float deltaTime)
	{
		if (this._opponent == null)
		{
			return true;
		}
		bool result = false;
		if (!this._MovedTP)
		{
			Vector3 worldPosition = this._opponent.view.WorldPosition;
			float x = this._opponent.view.transform.localScale.x;
			float tileSize = SingletonBehavior<HexagonalMapManager>.Instance.tileSize;
			worldPosition.y += 10f;
			worldPosition.z += 20f;
			Vector3 position = worldPosition;
			this._self.view.transform.position = position;

			this._MovedTP = true;
		}
		if (this._self.moveDetail.isArrived)
		{
			result = true;
			this._MovedTP = false;
		}
		return result;
	}

	private bool Land(float deltaTime)
	{
		if (this._opponent == null)
		{
			return true;
		}
		bool result = false;
		if (!this._MovedLand)
		{
			Vector3 worldPosition = this._opponent.view.WorldPosition;
			float x = this._opponent.view.transform.localScale.x;
			float tileSize = SingletonBehavior<HexagonalMapManager>.Instance.tileSize;
			worldPosition.y -= 5f;
			worldPosition.z -= 20f;
			Vector3 vector = worldPosition;
			this._self.moveDetail.Move(vector, 400f, false, false);
			this._MovedLand = true;
		}
		else if (this._self.moveDetail.isArrived)
		{
			result = true;
			this._MovedLand = false;
		}
		return result;
	}
}

/////////////////////////////// BehaviourAction /////////////////////////////





/////////////////////////////// Dice Passives ///////////////////////////////

// Time Manipulation Barrage
public class DiceCardAbility_ReuseTwiseDiceWhiteForDice : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[]";

	int twiseCheck = 0;

    public override void OnLoseParrying()
	{
		if (twiseCheck < 2) ActivateBonusAttackDice();
		twiseCheck += 1;
	}
    public override void OnWinParrying()
	{
		if (twiseCheck < 2) ActivateBonusAttackDice();
		twiseCheck += 1;
	}
}
// MOXIE
public class DiceCardAbility_TimeAxisAdvantage : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "If [时间轴] is 5+ gain 3 Power. If [时间轴] is 5- gain 5 Power. If [时间轴] max value is 20, dice max value +10.";

    public override void BeforeRollDice()
    {
		// Time Axis Power
		BattleUnitBuf_TimeAxis时间轴 timeAxis = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;

		if (timeAxis.stack >= 5) card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { power = 3 });
		if (timeAxis.stack <= -5) card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { power = 5 });

		if (timeAxis.teamMurdered)
		{
			// Max roll
			behavior.ApplyDiceStatBonus(new DiceStatBonus
			{
				max = 10
			});
		}
	}
}
// Axis Break
public class DiceCardAbility_DestroyAllDiceAxis : DiceCardAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "[On Clash Win] Destroy all dice on the opponent's Page. [On Clash Lose] Destroy all dice on this page.";

	public bool sumDone = false;

	public override void OnSucceedAttack()
	{
		// Sum Final
		DiceCardSelfAbility_For时间轴GainPower DiceCardSelfAbility_For时间轴GainPower = base.card.cardAbility as DiceCardSelfAbility_For时间轴GainPower;

		if (DiceCardSelfAbility_For时间轴GainPower != null)
		{
			DiceCardSelfAbility_For时间轴GainPower.sum += behavior.DiceResultValue;
		}
		sumDone = true;
	}


	public override void OnWinParrying()
	{
		// Destroy Dice
		base.card?.target?.currentDiceAction?.DestroyDice(DiceMatch.AllDice);

		// Sum Final
		DiceCardSelfAbility_For时间轴GainPower DiceCardSelfAbility_For时间轴GainPower = base.card.cardAbility as DiceCardSelfAbility_For时间轴GainPower;

		if (DiceCardSelfAbility_For时间轴GainPower != null)
		{
			DiceCardSelfAbility_For时间轴GainPower.sum += behavior.DiceResultValue;
		}
		sumDone = true;
	}

    public override void OnLoseParrying()
    {
		base.card?.DestroyDice(DiceMatch.AllDice);
	}
}
public class DiceCardAbility_SumAllDiceForFinal : DiceCardAbilityBase
{
	private bool _beforeRoll;

	public override void BeforeRollDice()
	{
		_beforeRoll = true;
	}

	public override void OnSucceedAttack()
	{
		if (_beforeRoll)
		{
			// Sum Final
			DiceCardSelfAbility_For时间轴GainPower DiceCardSelfAbility_For时间轴GainPower = base.card.cardAbility as DiceCardSelfAbility_For时间轴GainPower;

			if (DiceCardSelfAbility_For时间轴GainPower != null)
			{
				DiceCardSelfAbility_For时间轴GainPower.sum += behavior.DiceResultValue;
			}
			_beforeRoll = false;
		}
	}

	public override void OnWinParrying()
	{
		if (_beforeRoll)
		{
			// Sum Final
			DiceCardSelfAbility_For时间轴GainPower DiceCardSelfAbility_For时间轴GainPower = base.card.cardAbility as DiceCardSelfAbility_For时间轴GainPower;

			if (DiceCardSelfAbility_For时间轴GainPower != null)
			{
				DiceCardSelfAbility_For时间轴GainPower.sum += behavior.DiceResultValue;
			}
			_beforeRoll = false;
		}
	}
}
public class DiceCardAbility_FinalDiceDealAllDMG : DiceCardAbilityBase
{
	public override void OnSucceedAttack(BattleUnitModel target)
	{
		// Sum Final
		DiceCardSelfAbility_For时间轴GainPower DiceCardSelfAbility_For时间轴GainPower = base.card.cardAbility as DiceCardSelfAbility_For时间轴GainPower;
		

		if (DiceCardSelfAbility_For时间轴GainPower != null)
        {
			DiceCardSelfAbility_For时间轴GainPower.sum += behavior.DiceResultValue;
        }

		target.TakeDamage(DiceCardSelfAbility_For时间轴GainPower.sum, DamageType.Attack, base.owner);
		target.TakeBreakDamage(DiceCardSelfAbility_For时间轴GainPower.sum, DamageType.Attack, base.owner);

		// Reset Sum
		DiceCardSelfAbility_For时间轴GainPower.sum = 0;
	}
}

/////////////////////////////// Dice Passives ///////////////////////////////





/////////////////////////////// Page Passives ///////////////////////////////

// Solid Base
public class DiceCardSelfAbility_Restore3Light : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "[On Use] Restore 3 Light. If 时间轴 is exacly 0, Restore and additional 2 Light and increase 时间轴 by 2.";

	public override void OnUseCard()
	{
		BattleUnitBuf_TimeAxis时间轴 timeAxis = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;
		owner.cardSlotDetail.RecoverPlayPoint(3);

		if (timeAxis.stack == 0)
        {
			owner.cardSlotDetail.RecoverPlayPoint(2);
			BattleUnitBuf_TimeAxis时间轴.AddBuf(owner, 2);
		}
	}
}
// Quick Suppression!
public class DiceCardSelfAbility_SpeedyEightAxis : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Use] If the Speed dice this page is used on has 8+ speed, Draw 2 Pages and Restore 2 Light. Otherwise gain 2 Haste.";

	public override void OnUseCard()
	{
		if (card.speedDiceResultValue >= 8)
		{
			owner.allyCardDetail.DrawCards(2);
			owner.cardSlotDetail.RecoverPlayPoint(2);
		}
		else
        {
			owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Quickness, 2, owner);
        }
	}
}
// Time Manipulation Barrage
public class DiceCardSelfAbility_ReuseDiceTwiseWhite : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "" };
	public static string Desc = "[On Use] Every dice on this Page will gain the passive: '[On Clash Lose] or [On Clash Win] Reuse Dice.'";

    public override void OnUseCard()
    {
		card.ApplyDiceAbility(DiceMatch.AllDice, new DiceCardAbility_ReuseTwiseDiceWhiteForDice());
	}
}
// Complete Domination
public class DiceCardSelfAbility_AllDiceWinTimeAxis3More : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "[After All Dice are Used] If 3+ attack Dices landed a hit. At a 75% chanse, increase 时间轴 by 3. And by 25% decrease 时间轴 by 5.";

	int clashesWon = 0;
	List<BattleDiceBehavior> list = new List<BattleDiceBehavior>();

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
		// Increase clashesWon by 1.
		clashesWon += 1;
    }

	public override void OnEndBattle()
    {
		// After all dices are used calculate what will occur.
		// If amount of dices equal clashes won do so.
		if (3 <= clashesWon)
        {
			int randoCalc = RandomUtil.Range(1, 4);

			if (randoCalc < 4) BattleUnitBuf_TimeAxis时间轴.AddBuf(owner, 3);
			else BattleUnitBuf_TimeAxis时间轴.SubBuf(owner, 5);
		}

	}
}
// Quick Draw
public class DiceCardSelfAbility_IfLessThanZeroMoxieDraw2 : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "[On Use] If 时间轴 less than 0, increase all Dices power by 1. Draw 2 Pages";

    public override void OnUseCard()
    {
		owner.allyCardDetail.DrawCards(2);
		BattleUnitBuf_TimeAxis时间轴 timeAxis = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;

		if (timeAxis.stack < 0) card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { power = 1 });
	}
}
// Axis Corrosion
public class DiceCardSelfAbility_InflictHalf时间轴After : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "[On Hit] Inflict the reverse of half 时间轴 on user, to the target. Reset 时间轴 on self to 0.";

	public bool Inflicted时间轴 = false;


	public override void AfterGiveDamage(int damage, BattleUnitModel target)
    {
		if (!Inflicted时间轴)
		{
			BattleUnitBuf_TimeAxis时间轴 timeAxis = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;

			BattleUnitBuf_TimeAxis时间轴.SetBuf(target, (timeAxis.stack / 2) * -1);
			BattleUnitBuf_TimeAxis时间轴.SetBuf(owner, 0);

			Inflicted时间轴 = true;
		}
	}
}
// Axis Break
public class DiceCardSelfAbility_For时间轴GainPower : DiceCardSelfAbilityBase
{
	public override string[] Keywords => new string[] { "时间轴" };
	public static string Desc = "Only Usable at 5+ 时间轴. [On Use] The Dice on this Page gain +5 max roll. If 时间轴 is at 10, Dice will get an additional +10 max roll and if 时间轴 is at 20, Dice get an extra +5 max roll; additionally if target is defeated or Staggered, use this page again on a random enemy.";


	///////////////////////// Dice final Sum DMG /////////////////////////
	public int sum = 0;

	public override bool OnChooseCard(BattleUnitModel owner)
	{
		BattleUnitBuf_TimeAxis时间轴 timeAxis = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;
		if (timeAxis != null && timeAxis.stack >= 5)
		{
			return true;
		}
		return false;
	}

	public override void OnUseCard()
	{
		// OnRush
		_isBreakedStart = false;
		if (card.target != null && card.target.IsBreakLifeZero())
		{
			_isBreakedStart = true;
		}

		// Sum
		BattleUnitBuf_TimeAxis时间轴 timeAxis = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;

		// Max roll
		if (timeAxis.stack >= 20) card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { max = 20 });
		else if (timeAxis.stack >= 10) card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { max = 15 });
		else if (timeAxis.stack >= 5) card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus { max = 5 });
	}

	public override void BeforeRollDice(BattleDiceBehavior behavior)
	{
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			dmgRate = -9999,
			breakRate = -9999
		});
	}
	///////////////////////// Dice final Sum DMG /////////////////////////

	///////////////////////// On Rush Clone /////////////////////////
	private bool _isBreakedStart;

	public override void OnEndBattle()
	{
		if (card.target != null && (card.target.IsDead() || (!_isBreakedStart && card.target.IsBreakLifeZero())))
		{
			List<BattleUnitModel> aliveList = BattleObjectManager.instance.GetAliveList((base.owner.faction != Faction.Player) ? Faction.Player : Faction.Enemy);
			if (aliveList.Count > 0)
			{
				BattleUnitModel target = RandomUtil.SelectOne(aliveList);
				Singleton<StageController>.Instance.AddAllCardListInBattle(card, target);
			}
		}
	}
	///////////////////////// OnRush Clone /////////////////////////
}
// 0.5% dmg lmao
public class DiceCardSelfAbility_Dicedmgtest : DiceCardSelfAbilityBase
{
    public override void BeforeGiveDamage(BattleDiceBehavior behavior)
    {
		// Get hp values.
		int maxhp = this.owner.MaxHp;
		float currhp = this.owner.hp;

		// Calculate percentage of HP lost.
		float hpLostPercent = ((float)(maxhp - currhp) / maxhp) * 100;

		// Calculate the damage bonus as an integer.
		int damageBonus = (int)Math.Round(hpLostPercent * 0.5f);

		// Apply stat bonus.
		behavior.ApplyDiceStatBonus(new DiceStatBonus
		{
			dmg = damageBonus
		});
	}

    public override void AfterGiveDamage(int damage, BattleUnitModel target)
    {
		// Get hp values.
		int maxhp = this.owner.MaxHp;
		float currhp = this.owner.hp;

		// Calculate percentage of HP lost.
		float hpLostPercent = ((float)(maxhp - currhp) / maxhp) * 100;

		// Calculate additional damage
		int additionalDMG = (int)((hpLostPercent * 0.5f * damage) / 100);

		target.TakeDamage(additionalDMG, DamageType.Attack, owner);
	}
}

/////////////////////////////// Page Passives ///////////////////////////////





///////////////////////////// Key Page Passives /////////////////////////////

// Time Axis Control
public class PassiveAbility_TimeAxisOfTheWhite : PassiveAbilityBase
{
	// Upon winning a clash using an Attack dice, increase Time Axis.
	// On Losing a clash against a Attack dice decrease Time Axis by 1.
	// On Losing a clash against a Defence/Evasion dice decrease Time Axis by 2.
	// At 10 时间轴 gain the 'Time Axis Manipulation' Page.

	public override void OnUnitCreated()
    {
		BattleUnitBuf_TimeAxis时间轴.AddBuf(base.owner, 0);
	}

    public override void OnWinParrying(BattleDiceBehavior behavior)
    {
        if (behavior.Detail == BehaviourDetail.Hit || behavior.Detail == BehaviourDetail.Penetrate || behavior.Detail == BehaviourDetail.Slash)
        {
			BattleUnitBuf_TimeAxis时间轴.AddBuf(base.owner, 1);
		}
    }

    public override void OnLoseParrying(BattleDiceBehavior behavior)
    {
		if (behavior.TargetDice.Detail == BehaviourDetail.Hit || behavior.TargetDice.Detail == BehaviourDetail.Penetrate || behavior.TargetDice.Detail == BehaviourDetail.Slash)
		{
			BattleUnitBuf_TimeAxis时间轴.SubBuf(base.owner, 1);
		}

		if (behavior.TargetDice.Detail == BehaviourDetail.Guard || behavior.TargetDice.Detail == BehaviourDetail.Evasion)
		{
			BattleUnitBuf_TimeAxis时间轴.SubBuf(base.owner, 2);
		}
	}
}
// Master of TIme
public class PassiveAbility_MasterOfTheTimeAxis : PassiveAbilityBase
{
	// First off Gain 2 EGO pages.
	// If any other teammates exist, kill them and gain 1 permanent power for every 4 allies killed in this manner.
	// Additionally gain 1 permanent DMG Up per ally killed in this manner.
	// Finally if 4 allies were killed in this manner. Increase the max an minimum of 时间轴 by 10.
	// And for every 2 teammates killed this way gain 1 Speed Dice and MAX Light.


	public override int MaxPlayPointAdder() { return 2; }
	public override int SpeedDiceNumAdder() { return 1; }

	public override void OnUnitCreated()
    {
		base.owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 79));
		base.owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 80));
	}

    // Declare variables
    int teammatesAlive;
	bool onceTwo = false;
	bool onceFour = false;

	public override void OnRoundStart()
    {
		// Find all teammates.
		List<BattleUnitModel> teamUnits = BattleObjectManager.instance.GetAliveList(owner.faction).FindAll((BattleUnitModel x) => x != owner);
		teammatesAlive = teamUnits.Count;
		// Get buff from Owner.
		BattleUnitBuf_TimeAxis时间轴 BattleUnitBuf_TimeAxis时间轴 = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_TimeAxis时间轴) as BattleUnitBuf_TimeAxis时间轴;




		// Increase cap of 时间轴 from 10 to 20. And gain power
		if (teammatesAlive == 0) {
			BattleUnitBuf_TimeAxis时间轴.teamMurdered = true;
			if (BattleUnitBuf_TimeAxis时间轴.teamMurdered) owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.AllPowerUp, 1, owner);
		}

		// Check on teammates when only 2.
		if (teammatesAlive <= 2 && !onceFour)
        {
			onceFour = true;
			// Increases Light and Speed Dice for every 2 teammates killed this way.
			owner.bufListDetail.MaxPlayPointAdder();
			owner.bufListDetail.SpeedDiceNumAdder();
		}
		// Check on teammates when none.
		if (teammatesAlive <= 0 && !onceTwo)
		{
			onceTwo = true;
			// Increases Light and Speed Dice for every 2 teammates killed this way.
			owner.bufListDetail.MaxPlayPointAdder();
			owner.bufListDetail.SpeedDiceNumAdder();
		}

		owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.DmgUp, 4 - teammatesAlive, owner);
	}
}
// Time Re-Allocation
public class PassiveAbility_TimeAllocationOfWhite : PassiveAbilityBase
{
    // When killing an Enemy recover half the stagger bar and 1 quarter of the health bar.
    public override void OnKill(BattleUnitModel target)
    {
		owner.breakDetail.RecoverBreak(base.owner.breakDetail.GetDefaultBreakGauge() / 2);
		owner.RecoverHP(owner.MaxHp / 4);
		BattleUnitBuf_TimeAxis时间轴.AddBuf(owner, 3);
    }
}
// Boss Jijikan
public class PassiveAbility_TimeAlterationOfWhite : PassiveAbilityBase
{
	// Upon winning a clash using an Attack dice, increase Time Axis by 2.
	// On Losing a clash decrease Time Axis by 1.
	// At 10 时间轴 gain the 'Time Axis Manipulation' Page.
	// On Kill recover 10% of Max HP and 20% of Max Stagger, additionally gain 6 时间轴.

	public override void OnUnitCreated()
	{
		BattleUnitBuf_TimeAxis时间轴.AddBuf(base.owner, 0);
	}

	public override void OnWinParrying(BattleDiceBehavior behavior)
	{
		if (behavior.Detail == BehaviourDetail.Hit || behavior.Detail == BehaviourDetail.Penetrate || behavior.Detail == BehaviourDetail.Slash)
		{
			BattleUnitBuf_TimeAxis时间轴.AddBuf(base.owner, 2);
		}
	}

	public override void OnLoseParrying(BattleDiceBehavior behavior)
	{
		BattleUnitBuf_TimeAxis时间轴.SubBuf(base.owner, 1);
	}

	public override void OnKill(BattleUnitModel target)
	{
		owner.breakDetail.RecoverBreakLife(owner.breakDetail.breakLife / 5);
		owner.RecoverHP(owner.MaxHp / 10);
		BattleUnitBuf_TimeAxis时间轴.AddBuf(owner, 6);
	}
}

///////////////////////////// Key Page Passives /////////////////////////////
