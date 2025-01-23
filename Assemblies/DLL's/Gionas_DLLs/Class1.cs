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
/// Attack Names:
/// - Clouded Swipe			- Slash, On use give 2 dreams & restore 1 light. gives 1 dream on hit.		- 0 cost WORKING
/// - Prepared Mind			- Counter, On Use if has 3 dream, Restore 2 light. Draw 1 card.				- 1 cost WORKING
/// - Comfy Dreams			- Defence, On Use if has 7 dream Draw 3 cards. On Hit gain 2 dream.			- 2 cost WORKING
/// - Heavy Sleeper			- Blunt, If has 5 dream, +1 power. Gain 1 dream on use, 2 on hit.			- 3 cost WORKING
/// - Blurry Strike			- Pierce, If has 5 dream, +2 power. Gain 3 dream on onesided. 1 on hit.		- 1 cost WORKING
/// - Quick Thinking		- Defence, If has 2 dream, gain 1 Haste, 1 Light, 1 Dream.					- 1 cost WORKING
/// - Deep Sleep			- On Play, Immobalized this turn, gain 7 dream.	Gain 6 Light.				- 2 cost WORKING
/// - Sharp Mind			- Slash, If 15+ dream raise minimum power by 2,
///							  if rolled 4 gain 41 power. Lose clash gain 3 dream.						- 4 cost WORKING
/// - Dreamt Assault		- Pierce, if 15+ dream, consume 10 dream and gain +6 power.
///							  If not, drecrease minimum by 3 power.										- 4 cost 
/// - Nightmare				- On hit, reuse and turn 5 dreams into nightmares.							- 2 cost WORKING
/// - Fear Dream			- On Play, gain 1 Nightmare.												- 2 cost WORKING
/// </summary>


namespace Gionas_DLLs
{
	/////////////////////////////// Buffs / Effects /////////////////////////////

	public class BattleUnitBuf_CrittableGionas : BattleUnitBuf
	{
		public BattleUnitBuf_CrittableGionas(BattleUnitModel model)
		{
			this._owner = model;
			/*
			typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
			typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
			*/
			this.stack = 0;
		}


		////////////////////////////////////////////// Effects of Buff ///
		public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
		{
			return AtkResist.Weak;
		}

		public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
		{
			return AtkResist.Weak;
		}

        public override void OnLoseHp(int dmg) { Destroy(); }

		public override void OnRoundEnd() { Destroy(); }
		////////////////////////////////////////////// Effects of Buff ///


		public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_CrittableGionas battleUnitBuf_CrittableGionas = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_CrittableGionas) as BattleUnitBuf_CrittableGionas;
			if (battleUnitBuf_CrittableGionas == null)
			{
				BattleUnitBuf_CrittableGionas battleUnitBuf_CrittableGionas2 = new BattleUnitBuf_CrittableGionas(unit);
				battleUnitBuf_CrittableGionas2.Add(stack);
				unit.bufListDetail.AddBuf(battleUnitBuf_CrittableGionas2);
			}
			else
			{
				battleUnitBuf_CrittableGionas.Add(stack);
			}
		}
	}

	public class BattleUnitBuf_DreamOfGionas : BattleUnitBuf
	{
		protected override string keywordId => "DreamOfGionas";
		protected override string keywordIconId => "DreamOfGionas";

		public BattleUnitBuf_DreamOfGionas(BattleUnitModel model)
		{
			this._owner = model;
			/*
			typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
			typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
			*/
			this.stack = 0;
		}


		////////////////////////////////////////////// Effects of Buff ///
		private List<BattleUnitModel> _attackers = new List<BattleUnitModel>();

		// Handle Crits.
        public override void OnRollDice(BattleDiceBehavior behavior)
		{
			BattleUnitModel theTarget = behavior?.card?.target;
			int chanseCalc = RandomUtil.Range(1, 4);

			if (this._owner.passiveDetail.HasPassive<PassiveAbility_DreamOfGionasManager>() && chanseCalc != 4) chanseCalc = RandomUtil.Range(3, 4);

			if (chanseCalc == 4)
			{
				BattleUnitBuf_CrittableGionas.AddBuf(theTarget, 1);
			}
		}

		// Random Buff adder.
		// More likely if Gionas.
		// Lose passive 2
        public override void OnRoundEnd()
        {
			int chanseCalc = 0;
			int effectCalc = 0;
			bool hasPassive = this._owner.passiveDetail.HasPassive<PassiveAbility_DreamOfGionasManager>();

			if (hasPassive) chanseCalc = RandomUtil.Range(3, 4);
			if (!hasPassive) chanseCalc = RandomUtil.Range(1, 4);

			// We dont want random buffs to happen when immobalized.
			BattleUnitBuf_stun immobalized = this._owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_stun) as BattleUnitBuf_stun;

			if (this.stack >= 15 && chanseCalc == 4 && immobalized == null)
            {
				while (chanseCalc == 4)
				{
					effectCalc = RandomUtil.Range(0, 32);

					if (effectCalc == 0)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Decay, 3);
					if (effectCalc == 1)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn, 5);
					if (effectCalc == 2)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Paralysis, 2);
					if (effectCalc == 3)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Bleeding, 9);
					if (effectCalc == 4)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, 1);
					if (effectCalc == 5)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable_break, 1);
					if (effectCalc == 6)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Weak, 1);
					if (effectCalc == 7)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Disarm, 1);
					if (effectCalc == 8)  this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Binding, 1);
					if (effectCalc == 10) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Protection, 3);
					if (effectCalc == 11) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BreakProtection, 1);
					if (effectCalc == 12) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1);
					if (effectCalc == 13) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 1);
					if (effectCalc == 14) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1);
					if (effectCalc == 15) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Stun, 1);
					if (effectCalc == 16) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DmgUp, 3);
					if (effectCalc == 17) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.SlashPowerUp, 2);
					if (effectCalc == 18) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.PenetratePowerUp, 2);
					if (effectCalc == 19) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HitPowerUp, 2);
					if (effectCalc == 20) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DefensePowerUp, 2);
					if (effectCalc == 21) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.WarpCharge, 5);
					if (effectCalc == 22) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Smoke, 5);
					if (effectCalc == 23) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.NullifyPower, 1);
					if (effectCalc == 24) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HalfPower, 1);
					if (effectCalc == 25) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 1);
					if (effectCalc == 26) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.BurnSpread, 3);
					if (effectCalc == 27) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Nail, 5);
					if (effectCalc == 28) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Seal, 1);
					if (effectCalc == 29) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.ForbidRecovery, 1);
					if (effectCalc == 30) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Blurry, 1);
					if (effectCalc == 31) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.DecreaseSpeedTo1, 1);
					if (effectCalc == 32) this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.HeavySmoke, 10);

					chanseCalc = 0;

					if (hasPassive) chanseCalc = RandomUtil.Range(3, 4);
					if (!hasPassive) chanseCalc = RandomUtil.Range(1, 4);
				}

				IntegerDeleto(12);
			}

			if (!hasPassive) IntegerDeleto(2);
		}
        ////////////////////////////////////////////// Effects of Buff ///


		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_DreamOfGionas battleUnitBuf_DreamOfGionas = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			BattleUnitBuf_NightmareOfGionas battleUnitBuf_NightmareOfGionas = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_NightmareOfGionas) as BattleUnitBuf_NightmareOfGionas;
			bool hasNightmarePassive = unit.passiveDetail.HasPassive<PassiveAbility_DistortedDreamer>();

			if (battleUnitBuf_DreamOfGionas == null)
			{
				BattleUnitBuf_DreamOfGionas battleUnitBuf_DreamOfGionas2 = new BattleUnitBuf_DreamOfGionas(unit);
				battleUnitBuf_DreamOfGionas2.Add(stack);
				unit.bufListDetail.AddBuf(battleUnitBuf_DreamOfGionas2);
			}
			else
			{
				battleUnitBuf_DreamOfGionas.Add(stack);
			}
		}

		public void Add(int add)
		{
			this.stack += add;
		}

		public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
		{
			BattleUnitBuf_DreamOfGionas DreamOfGionas = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			if (div0sub1 == 0)
			{
				DreamOfGionas.PercentageDeleto(percentage);
			}
			else if (div0sub1 == 1)
			{
				DreamOfGionas.IntegerDeleto(percentage);
			}
		}

		public void IntegerDeleto(int amount)
		{
			this.stack -= amount;
			if (this.stack < 0)
            {
				this.stack = 0;
            }
		}

		public void PercentageDeleto(int percentage)
		{
			int value = this.stack;

			double percentasdouble = (double)percentage / 100;
			this.stack -= value * (int)percentasdouble;
		}
	}

	public class BattleUnitBuf_NightmareOfGionas : BattleUnitBuf
	{
		protected override string keywordId => "NightmareOfGionas";
		protected override string keywordIconId => "NightmareOfGionas";

		public BattleUnitBuf_NightmareOfGionas(BattleUnitModel model)
		{
			this._owner = model;
			/*
			typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
			typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
			*/
			this.stack = 1;
		}


		////////////////////////////////////////////// Effects of Buff ///
		// Fatal to everything.
		public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail) { return AtkResist.Weak; }
		public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail) { return AtkResist.Weak; }


		private List<BattleUnitModel> _attackers = new List<BattleUnitModel>();

		// Lose 1 Nightmare if doesnt have passive.
		// Destroy if staggered or stack 0.
		// Give buff and debuffs by the stack.
		public override void OnRoundEnd()
		{
			int disStack = this.stack;
			int multForNegatives = 2;


			if (!this._owner.passiveDetail.HasPassive<PassiveAbility_DistortedDreamer>())
			{
				this._owner.TakeBreakDamage(disStack, DamageType.Buf, this._owner);
				IntegerDeleto(1);
			}
			else
			{
				Add(1);
				this._owner.TakeBreakDamage(disStack * 3, DamageType.Buf, base._owner);
			}
			
			if (this._owner.breakDetail.IsBreakLifeZero() || this.stack <= 0) { Destroy(); }

			if (this._owner.passiveDetail.HasPassive<PassiveAbility_DreamOfGionasManager>()) multForNegatives = 1;

			this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, disStack);
			this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Vulnerable, disStack * multForNegatives);
			this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Disarm, disStack * multForNegatives);
			this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 2);

			
		}

		// Destroy if 0
        public override void OnLoseHp(int dmg)
        {
			if (this.stack <= 0) Destroy();
        }

		// When getting hit by opponent lose 1 stack of Nightmare
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
		{
			if (!this._owner.passiveDetail.HasPassive<PassiveAbility_DistortedDreamer>()) IntegerDeleto(1);
		}
		////////////////////////////////////////////// Effects of Buff ///


		public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_NightmareOfGionas battleUnitBuf_NightmareOfGionas = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_NightmareOfGionas) as BattleUnitBuf_NightmareOfGionas;
			if (battleUnitBuf_NightmareOfGionas == null)
			{
				BattleUnitBuf_NightmareOfGionas battleUnitBuf_NightmareOfGionas2 = new BattleUnitBuf_NightmareOfGionas(unit);
				battleUnitBuf_NightmareOfGionas2.Add(stack);
				unit.bufListDetail.AddBuf(battleUnitBuf_NightmareOfGionas2);
			}
			else
			{
				battleUnitBuf_NightmareOfGionas.Add(stack);
			}
		}

		public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
		{
			BattleUnitBuf_NightmareOfGionas NightmareOfGionas = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_NightmareOfGionas) as BattleUnitBuf_NightmareOfGionas;
			if (div0sub1 == 0)
			{
				NightmareOfGionas.IntegerDeleto(percentage);
			}
			else if (div0sub1 == 1)
			{
				NightmareOfGionas.IntegerDeleto(percentage);
			}
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

	public class BattleUnitBuf_DreamsToRemember : BattleUnitBuf
	{
		protected override string keywordId => "DreamsToRemember";
		protected override string keywordIconId => "DreamsToRemember";

		public BattleUnitBuf_DreamsToRemember(BattleUnitModel model)
		{
			this._owner = model;
			/*
			typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all).SetValue(this, IrahInitializer.ArtWorks["Blood"]);
			typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all).SetValue(this, true);
			*/
			this.stack = 0;
		}


		////////////////////////////////////////////// Effects of Buff ///

		// Power
		public override void BeforeRollDice(BattleDiceBehavior behavior)
		{
			behavior.ApplyDiceStatBonus(new DiceStatBonus
			{
				power = this.stack
			});
		}

		////////////////////////////////////////////// Effects of Buff ///


		public void Add(int add)
		{
			if (this.stack < 3) this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_DreamsToRemember battleUnitBuf_DreamsToRemember = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamsToRemember) as BattleUnitBuf_DreamsToRemember;
			if (battleUnitBuf_DreamsToRemember == null)
			{
				BattleUnitBuf_DreamsToRemember battleUnitBuf_DreamsToRemember2 = new BattleUnitBuf_DreamsToRemember(unit);
				battleUnitBuf_DreamsToRemember2.Add(stack);
				unit.bufListDetail.AddBuf(battleUnitBuf_DreamsToRemember2);
			}
			else
			{
				battleUnitBuf_DreamsToRemember.Add(stack);
			}
		}

		public static void SubBuf(BattleUnitModel unit, int amount)
		{
			BattleUnitBuf_DreamsToRemember DreamsToRemember = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamsToRemember) as BattleUnitBuf_DreamsToRemember;
			DreamsToRemember.IntegerDeleto(amount);
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

	public class BehaviourAction_GionasSharpSword : BehaviourActionBase
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
																								   // Attack Sprite  // If it moves &
																													 // the direction
					// Preparing to Draw blade
					RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)13, (CharMoveState)1, 0f, true, 0.5f, 1f);
					list.Add(movingAction);

					// Actually Slashing
					movingAction = new RencounterManager.MovingAction((ActionDetail)14, (CharMoveState)1, 0f, false, 0.5f, 1f);
					movingAction.customEffectRes = "Apostle_H";
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

	public class BehaviourAction_GionasFuriosoCopy : BehaviourActionBase
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

					// [START]
					// Hit
					RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)13, (CharMoveState)1, 0f, true, 0.5f, 1f);
					list.Add(movingAction);

					// S4
					movingAction = new RencounterManager.MovingAction((ActionDetail)16, (CharMoveState)1, 0f, true, 0.1f, 2f);
					list.Add(movingAction);
					// S5
					movingAction = new RencounterManager.MovingAction((ActionDetail)17, (CharMoveState)1, 0f, true, 0.1f, 2f);
					list.Add(movingAction);
					// S6
					movingAction = new RencounterManager.MovingAction((ActionDetail)21, (CharMoveState)1, 0f, true, 0.1f, 2f);
					list.Add(movingAction);
					// S7
					movingAction = new RencounterManager.MovingAction((ActionDetail)22, (CharMoveState)1, 0f, true, 0.1f, 2f);
					list.Add(movingAction);
					// S8
					movingAction = new RencounterManager.MovingAction((ActionDetail)23, (CharMoveState)1, 0f, true, 0.1f, 2f);
					list.Add(movingAction);
					// S9
					movingAction = new RencounterManager.MovingAction((ActionDetail)24, (CharMoveState)1, 0f, true, 0.1f, 1f);
					movingAction.customEffectRes = "Apostle_H";
					movingAction.SetEffectTiming(0, 0, 0);
					list.Add(movingAction);
					// S10
					movingAction = new RencounterManager.MovingAction((ActionDetail)25, (CharMoveState)1, 0f, true, 0.1f, 2f);
					list.Add(movingAction);

					// [END]
					// S2
					movingAction = new RencounterManager.MovingAction((ActionDetail)14, (CharMoveState)1, 0f, false, 0.1f, 2f);
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

	// Clouded Swipe
	public class DiceCardAbility_HitGain1Dream : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Hit] Gain 1 Dream.";

		public override void OnSucceedAttack()
		{
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 1);
		}
	}

	// Comfy Dreams
	public class DiceCardAbility_WinGain2Dream : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Clash Win] Gain 2 Dream.";

        public override void OnWinParryingDefense()
		{
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 2);
		}
	}

	// Comfy Dreams
	// Heavy Sleeper
	public class DiceCardAbility_HitGain2Dream : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Hit] Gain 2 Dream.";

		public override void OnSucceedAttack()
		{
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 2);
		}
	}

	// Sharp Mind
	public class DiceCardAbility_Roll4 : DiceCardAbilityBase
	{
		public static string Desc = "[On Use] If the natural roll is Max, and is 4 or higher, add +41 power.";

        public override void OnRollDice()
		{
			if (behavior.DiceVanillaValue >= 4 && behavior.DiceVanillaValue == behavior.GetDiceMax())
			{
				behavior.ApplyDiceStatBonus(new DiceStatBonus
				{
					power = 41
				});
			}
		}
	}

	// Nightmare
	public class DiceCardAbility_5DreamTo5NightmareReuse : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas", "NightmareOfGionas" };
		public static string Desc = "[On Hit] Reuse Dice, and turn 5 Dreams into 1 Nightmare (Until not enough Dreams).";

        public override void OnSucceedAttack()
		{
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;

			if (dreamOfGionas.stack >= 5)
			{
				ActivateBonusAttackDice();

				BattleUnitBuf_DreamOfGionas.SubBuf(base.owner, 5, 1);
				BattleUnitBuf_NightmareOfGionas.AddBuf(base.owner, 1);
			}
		}
	}

	/////////////////////////////// Dice Passives ///////////////////////////////



	/////////////////////////////// Page Passives ///////////////////////////////

	// Clouded Swipe
	public class DiceCardSelfAbility_1Dream1Light : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] Gain 1 Dream and Restore 1 Light.";

		public override void OnUseCard()
		{
			base.owner.cardSlotDetail.RecoverPlayPoint(1);
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 1);
		}
	}

	// Prepared Mind
	public class DiceCardSelfAbility_2LightDraw1Page : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] If has +3 Dreams. Restore 2 Light, draw 1 Page.";

        public override void OnUseCard()
		{
			base.owner.cardSlotDetail.RecoverPlayPoint(2);
			base.owner.allyCardDetail.DrawCards(1);
		}
	}

	// Comfy Dreams
	public class DiceCardSelfAbility_Draw3Page : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] If has +7 Dream, Draw 3 Page.";

		public override void OnUseCard()
		{
			base.owner.allyCardDetail.DrawCards(3);
		}
	}

	// Heavy Sleeper
	public class DiceCardSelfAbility_UseGain1Dream : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] Gain 1 Dream. If has +5 Dream, Dice gain +1 Power and Restore 1 Light.";

		public override void OnUseCard()
		{
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			if (amountOfDreams >= 5)
            {
				base.owner.cardSlotDetail.RecoverPlayPoint(1);

				card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
				{
					power = 1
				});
			}

			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 1);
		}
	}

	// Blurry Strike
	public class DiceCardSelfAbility_3DreamOnesidedHit1Dream : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] If has +5 Dream, Dice gain +2 Power. [On Hit] Gain 1 Dream. If [One Sided] Gain 3 Dreams.";

        public override void OnUseCard()
        {
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			if (amountOfDreams >= 5)
			{
				card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
				{
					power = 2
				});
			}
		}

		public override void OnStartOneSideAction()
		{
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 3);
		}

		public override void OnSucceedAttack()
		{
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 1);
		}
	}

	// Quick Thinking
	public class DiceCardSelfAbility_Gain1HasteLightDream : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] If has +2 Dream, Restore 2 Light and Gain 1 Dream.";

		public override void OnSucceedAttack()
		{
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			if (amountOfDreams >= 2)
			{
				base.owner.cardSlotDetail.RecoverPlayPoint(2);
				BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 1);
			}
		}
	}

	// Deep Sleep
	public class DiceCardSelfAbility_Immobalized7Dreams6Light : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Play] Become Immobilized this Scene. Gain 15 Dreams. Restore 6 Light.";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
		{
			base.owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Stun, 1, base.owner);
			base.owner.cardSlotDetail.RecoverPlayPoint(6);
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 15);

			SingletonBehavior<BattleManagerUI>.Instance.ui_unitListInfoSummary.UpdateCharacterProfile(unit, unit.faction, unit.hp, unit.breakDetail.breakGauge, unit.bufListDetail.GetBufUIDataList());
			if (unit.bufListDetail.GetActivatedBuf(KeywordBuf.Stun) != null)
			{
				unit.view.speedDiceSetterUI.SetSpeedDicesBeforeRoll(unit.Book.GetSpeedDiceRule(unit).speedDiceList, unit.faction);
				unit.view.speedDiceSetterUI.DeselectAll();
			}
		}
	}

	// Sharp Mind
	public class DiceCardSelfAbility_15DreamRaiseMinimum2 : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] If +15 Dreams, add +2 base minimum roll to all ATK dice in page. [On Hit] Destroy all of opponent's dice. [On Clash Lose] Gain 3 Dream. [On Clash Win] Destroy a Combat Page set on another random Speed die of the target.";

		public override void OnUseCard()
		{
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			if (amountOfDreams >= 15)
			{
				card.ApplyDiceStatBonus(DiceMatch.AllAttackDice, new DiceStatBonus
				{
					min = 2
				});
			}
		}

        public override void OnSucceedAttack()
		{
			base.card?.target?.currentDiceAction?.DestroyDice(DiceMatch.AllDice);
		}

		public override void OnLoseParrying()
        {
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 3);
		}

        public override void OnWinParryingAtk()
        {
			if (card.target == null)
			{
				return;
			}
			BattleUnitModel target = card.target;
			int targetSlotOrder = card.targetSlotOrder;
			List<BattlePlayingCardDataInUnitModel> list = new List<BattlePlayingCardDataInUnitModel>();
			for (int i = 0; i < target.cardSlotDetail.cardAry.Count; i++)
			{
				if (i != targetSlotOrder)
				{
					BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel = target.cardSlotDetail.cardAry[i];
					if (battlePlayingCardDataInUnitModel != null && !battlePlayingCardDataInUnitModel.isDestroyed && battlePlayingCardDataInUnitModel.GetDiceBehaviorList().Count > 0)
					{
						list.Add(battlePlayingCardDataInUnitModel);
					}
				}
			}
			if (list.Count > 0)
			{
				RandomUtil.SelectOne(list).DestroyPlayingCard();
			}
		}

        public override void OnWinParryingDef()
		{
			if (card.target == null)
			{
				return;
			}
			BattleUnitModel target = card.target;
			int targetSlotOrder = card.targetSlotOrder;
			List<BattlePlayingCardDataInUnitModel> list = new List<BattlePlayingCardDataInUnitModel>();
			for (int i = 0; i < target.cardSlotDetail.cardAry.Count; i++)
			{
				if (i != targetSlotOrder)
				{
					BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel = target.cardSlotDetail.cardAry[i];
					if (battlePlayingCardDataInUnitModel != null && !battlePlayingCardDataInUnitModel.isDestroyed && battlePlayingCardDataInUnitModel.GetDiceBehaviorList().Count > 0)
					{
						list.Add(battlePlayingCardDataInUnitModel);
					}
				}
			}
			if (list.Count > 0)
			{
				RandomUtil.SelectOne(list).DestroyPlayingCard();
			}
		}
	}

	// Dreamt Assault
	public class DiceCardSelfAbility_Lose7Dream3PowerOr3LessPower : DiceCardSelfAbilityBase
    {
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] Forget 7 Dreams. If not enough Dreams, dices lose 6 Power. If has 10+, forget 10 Dreams and dices gain 3 Power.";

		public override void OnUseCard()
		{
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			// Less than [7] lose power
			if (amountOfDreams < 7)
			{
				card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
				{
					power = -6
				});
			}
			// At [7] and less than [10] consume dreams and stay base.
            else if (amountOfDreams <= 7 && amountOfDreams < 10)
            {
				BattleUnitBuf_DreamOfGionas.SubBuf(base.owner, 7, 1);
			}
			// At or more than [10] gain power
			else if (amountOfDreams <= 10)
            {
				BattleUnitBuf_DreamOfGionas.SubBuf(base.owner, 10, 1);

				card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
				{
					power = 3
				});
			}
		}
	}

	// Nightmare
	public class DiceCardSelfAbility_UseGain5Dream : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DreamOfGionas" };
		public static string Desc = "[On Use] Gain 5 Dream.";

		public override void OnUseCard()
		{
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 5);
		}
	}

	// Fear Dream
	public class DiceCardSelfAbility_UseGain1Nightmare : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "NightmareOfGionas" };
		public static string Desc = "[On Play] Gain 1 Nightmare.";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
		{
			BattleUnitBuf_NightmareOfGionas.AddBuf(base.owner, 1);
		}
	}

	/////////////////////////////// Page Passives ///////////////////////////////



	///////////////////////////// Key Page Passives /////////////////////////////

	//General for Dreamers
	public class PassiveAbility_DreamOfGionasManager : PassiveAbilityBase
	{
		// Doesn't Lose 2 Dream on round start.
		// If on Round start have 50 dreams, gain 1 permanent Power (Up to 3).
		// Crits are now dealt at a 50% chance. Random Buff application from Dreams are now also at a 50% chance.
		// Nightmares only give 1 Fragile and 1 Disarm instead of 2.
		// Obtain the EGO Page Nightmare.

		public string dreamStorage;
		public string ownerStorage;

		public override void OnUnitCreated()
		{
			base.owner.personalEgoDetail.AddCard(new LorId("MyTopStar", 52));

			/*
			// Get value of ownerForDreams, if it is then proceed. --------------Saving buff through acts----
			var stage = StageController.Instance.GetStageModel();
			if (stage.GetStageStorageData(ownerStorage, out HashSet<UnitBattleDataModel> ownerForDreams))
			{
				if (ownerForDreams.Contains(base.owner.UnitData))
				{
					// If dreamAmount higher than 0 apply Dreams to the prev check Owner.
					if (stage.GetStageStorageData(dreamStorage, out int dreamAmount))
					{
						if (dreamAmount > 0)
						{
							BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, dreamAmount);
						}
					}
				}
			}
			*/
		}

        // Keep updating on round end. --------------Saving buff through acts----
        public override void OnRoundEnd()
        {
			/*
			// retrieve amount of dreams
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			// Dream Amount
				// prepare storage
				var stage = StageController.Instance.GetStageModel();
				stage.GetStageStorageData(dreamStorage, out int dreamAmount);

				// assign the amount to store
				dreamAmount = amountOfDreams;
				stage.SetStageStorgeData(dreamStorage, dreamAmount);

			// Owner of Dreams
				stage.GetStageStorageData(ownerStorage, out HashSet<UnitBattleDataModel> ownerForDreams);

				ownerForDreams = new HashSet<UnitBattleDataModel>();
				stage.SetStageStorgeData(ownerStorage, ownerForDreams);

				ownerForDreams.Add(base.owner.UnitData);
			*/
		}

        public override void OnRoundStart()
		{
			BattleUnitBuf_DreamOfGionas dreamOfGionas = base.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;
			int amountOfDreams = dreamOfGionas.stack;

			if (amountOfDreams >= 50) BattleUnitBuf_DreamsToRemember.AddBuf(base.owner, 1);
		}

		
	}

	// Unique for Gionas
	public class PassiveAbility_DistortedDreamer : PassiveAbilityBase
	{
		// Gain unique page Fear Dream
		// Doestn lose 1 Nightmare at end of Scene.
		// Obtain 2 Nightmare when Turning into Distortion
		// Draw Pages and Restore Light equal to half Nightmare (Rounded Down).

		bool bChangeSkin = false;

        public override void OnUnitCreated() { this.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 53), false); }

        public override void OnRoundStart()
        {
			BattleUnitBuf_NightmareOfGionas battleUnitBuf_NightmareOfGionas = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_NightmareOfGionas) as BattleUnitBuf_NightmareOfGionas;
			BattleUnitBuf_DreamOfGionas battleUnitBuf_DreamOfGionas = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DreamOfGionas) as BattleUnitBuf_DreamOfGionas;

			if (!bChangeSkin && battleUnitBuf_NightmareOfGionas != null)
            {
                this.owner.view.ChangeWorkShopSkin("MyTopStar", "GionasEgoFull");
				battleUnitBuf_DreamOfGionas.Destroy();

				BattleUnitBuf_NightmareOfGionas.AddBuf(base.owner, 2);
				bChangeSkin = true;
			}
		}

        public override void OnRoundEnd()
        {
			BattleUnitBuf_NightmareOfGionas battleUnitBuf_NightmareOfGionas = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_NightmareOfGionas) as BattleUnitBuf_NightmareOfGionas;
			int nightmares = battleUnitBuf_NightmareOfGionas.stack;

			if (nightmares > 0)
            {
				BattleUnitBuf_DreamOfGionas.SubBuf(base.owner, 999, 1);

				base.owner.cardSlotDetail.RecoverPlayPoint(nightmares / 2);
				base.owner.allyCardDetail.DrawCards(nightmares / 2);
			}
		}

		public override void OnBreakState()
        {
			this.owner.view.ChangeWorkShopSkin("MyTopStar", "GionasFull");
			BattleUnitBuf_DreamOfGionas.AddBuf(base.owner, 0);
			BattleUnitBuf_NightmareOfGionas.SubBuf(base.owner, 999, 1);
			bChangeSkin = false;

			BattleUnitBuf_disarm disarm = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_disarm) as BattleUnitBuf_disarm;
			BattleUnitBuf_vulnerable vuln = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_vulnerable) as BattleUnitBuf_vulnerable;
			BattleUnitBuf_strength str = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_strength) as BattleUnitBuf_strength;
			BattleUnitBuf_quickness quick = this.owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_quickness) as BattleUnitBuf_quickness;

			disarm.Destroy();
			vuln.Destroy();
			str.Destroy();
			quick.Destroy();
		}
	}

	///////////////////////////// Key Page Passives /////////////////////////////
}
