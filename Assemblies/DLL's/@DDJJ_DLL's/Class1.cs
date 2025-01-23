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

namespace DDJJ_DLL_s
{
	/////////////////////////////// Buffs / Effects /////////////////////////////

	public class BattleUnitBuf_DDJJBurst : BattleUnitBuf
	{
		protected override string keywordId => "DDJJBurst";
		protected override string keywordIconId => "DDJJBurst";

		public bool linkedUp = false;
		public BattleUnitModel linkedUnit;



		////////////////// Buff Effect //////////////////
		
		public BattleUnitBuf_DDJJBurst(BattleUnitModel model, BattleUnitModel giver)
		{
			this._owner = model;
			this.stack = 0;

			if (giver == null) linkingUpUnits();
            else
            {
				linkedUp = true;
				linkedUnit = giver;
            }
		}

		public void linkingUpUnits()
        {
			// On creation check if theres any unit in same team to link up to,
			// else set the buff's stack amount to 3.
			if (!BattleObjectManager.instance.GetAliveList(this._owner.faction).Exists((BattleUnitModel x) => x != this._owner))
			{
				this.stack = 2;
			}
			else if (linkedUp == false)
			{
				// Find all teammates and select a random one.
				List<BattleUnitModel> teamUnits = BattleObjectManager.instance.GetAliveList(this._owner.faction).FindAll((BattleUnitModel x) => x != this._owner);
				int teammate = RandomUtil.Range(1, teamUnits.Count);
				int currunit = 0;

				// Will check if another unit has the buff as to
				// not have 3 units linked up.
				foreach (BattleUnitModel unit in teamUnits)
				{
					// Checks if buff exists on unit.
					bool ddjjBurst = unit.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJBurst);
					// If so theres already someone linked up. Thus save the unit as linked.
					if (ddjjBurst && !linkedUp && unit != this._owner)
					{
						linkedUp = true;
						linkedUnit = unit;
					}
				}

				// If the previous foreach didnt find a unit with the buff
				// proceed to give a random teammate the buff and save them as the linked individual.
				if (linkedUp == false)
				{
					foreach (BattleUnitModel unit in teamUnits)
					{
						currunit += 1;

						if (currunit == teammate && !linkedUp)
						{
							BattleUnitBuf_DDJJBurst.CreateWithLinking(unit, 1, this._owner);
							linkedUnit = unit;
							linkedUp = true;
						}
					}
				}
			}
		}

		/*-----------------------------------------------------------------------------*/

		// Kills the Linked Up unit.
        public override void OnDie()
        {
			int maxhp = linkedUnit.MaxHp;
			linkedUnit.TakeDamage(maxhp, DamageType.Buf, this._owner);
        }

        // Deals half damage taken to Linked Up unit.
        public override void OnTakeDamageByAttack(BattleDiceBehavior atkDice, int dmg)
        {

			linkedUnit.TakeDamage(dmg / 2, DamageType.Attack, atkDice.owner);
        }

        // Gives 1 Strenght and 3 Damage Up per stack.
        public override void OnRoundStart()
        {
			this._owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, this.stack);
			this._owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.DmgUp, 3 * this.stack);

			// Staggers the linked unit if this character gets staggered.
			if (linkedUnit.IsBreakLifeZero() && !this._owner.IsBreakLifeZero())
            {
				int maxbreak = this._owner.MaxBreakLife;
				this._owner.breakDetail.TakeBreakDamage(maxbreak, DamageType.Buf, this._owner);
            }
		}

        ////////////////// Buff Effect //////////////////



        ////////////////// Buff manipulation //////////////////

		// Custom function i need to avoid crashing, it escentially makes it so that,
		// it doesnt have to run main code to assign a partner. Im guessing it crashes bcs of
		// some bullshit with running main code whilest the code for first one with buff is still running or smt.
		public static void CreateWithLinking(BattleUnitModel reciver, int stack, BattleUnitModel giver)
        {
			BattleUnitBuf_DDJJBurst BattleUnitBuf_DDJJBurst = reciver.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJBurst) as BattleUnitBuf_DDJJBurst;
			if (BattleUnitBuf_DDJJBurst == null)
            {
				BattleUnitBuf_DDJJBurst BattleUnitBuf_DDJJBurst2 = new BattleUnitBuf_DDJJBurst(reciver, giver);
				BattleUnitBuf_DDJJBurst2.Add(stack);
				reciver.bufListDetail.AddBuf(BattleUnitBuf_DDJJBurst2);
			}
			else
			{
				BattleUnitBuf_DDJJBurst.Add(stack);
			}

		}

		/*-----------------------------------------------------------------------------*/

		// All of these are self explanitory
		public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_DDJJBurst BattleUnitBuf_DDJJBurst = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJBurst) as BattleUnitBuf_DDJJBurst;
			if (BattleUnitBuf_DDJJBurst == null)
			{
				BattleUnitBuf_DDJJBurst BattleUnitBuf_DDJJBurst2 = new BattleUnitBuf_DDJJBurst(unit, null);
				BattleUnitBuf_DDJJBurst2.Add(stack);
				unit.bufListDetail.AddBuf(BattleUnitBuf_DDJJBurst2);
			}
			else
			{
				BattleUnitBuf_DDJJBurst.Add(stack);
			}
		}

		public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
		{
			BattleUnitBuf_DDJJBurst dDJJBurst = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJBurst) as BattleUnitBuf_DDJJBurst;
			if (div0sub1 == 0)
			{
				dDJJBurst.PercentageDeleto(unit, percentage);
			}
			else if (div0sub1 == 1)
			{
				dDJJBurst.IntegerDeleto(unit, percentage);
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

		////////////////// Buff manipulation //////////////////
	}

	public class BattleUnitBuf_DDJJsStardust : BattleUnitBuf
    {
		protected override string keywordId => "DDJJsStardust";
		protected override string keywordIconId => "DDJJsStardust";



		////////////////// Buff Effect //////////////////

		public BattleUnitBuf_DDJJsStardust(BattleUnitModel model)
		{
			this._owner = model;
			this.stack = 0;
		}

		// Lose 1~2 stack on hit.
        public override void OnSuccessAttack(BattleDiceBehavior behavior)
        {
			int range = RandomUtil.Range(1, 2);

			BattleUnitBuf_DDJJsStardust.SubBuf(this._owner, range, 1);
		}

		// Gain power every 25.
		public override void BeforeRollDice(BattleDiceBehavior behavior)
		{
			if (!_owner.IsImmune(bufType))
			{
				behavior.ApplyDiceStatBonus(new DiceStatBonus
				{
					power = this.stack/20
				});
				SingletonBehavior<DiceEffectManager>.Instance.CreateBufEffect("BufEffect_Strength", _owner.view);
			}
		}

		////////////////// Buff Effect //////////////////

		//// Custom Funcs \\\\
		public static void BurstItAll(BattleUnitModel unit, BattleUnitModel burster)
        {
			// Bool to see it exists
			bool dDJJsStardust = unit.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust);

			// If it has the effect call the function that bursts the Stardust.
			if (dDJJsStardust)
            {
				BattleUnitBuf_DDJJsStardust dDJJsStardustEffect = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;

				dDJJsStardustEffect.BurstStardust(unit, burster);
			}
		}

		public void BurstStardust(BattleUnitModel unit, BattleUnitModel burster)
        {
			// If has 30 or more deal half as damage.
			if (this.stack >= 30)
            {
				this._owner.TakeDamage(this.stack / 2, DamageType.Buf);
				this._owner.TakeBreakDamage(this.stack, DamageType.Buf);

				// This is to trigger DDJJ EGO
				if (this._owner.passiveDetail.HasPassive<PassiveAbility_EclosingOne>())
                {
					BattleUnitBuf_DDJJEgoAwake.AddBuf(burster, 0);
                }
			}
			// Else deal only stagger.
            else
            {
				this._owner.TakeBreakDamage(this.stack, DamageType.Buf);
            }

			// To let know who was Trigger Burst and how many times to apply it.
			BattleUnitBuf_stardustBurstTriggered.AddBuf(unit, this.stack / 2);

			// Mainly only for the Burst Draw Page and Light Restore passive for DDJJEGO.
			// But can be updated into the future as necessary.
			BattleUnitBuf_stardustBurstTriggeredByUser.AddBuf(burster, this.stack / 2);

			// Halve the amount.
			BattleUnitBuf_DDJJsStardust.SubBuf(this._owner, this.stack / 2, 1);
		}

		public static void Transfer(BattleUnitModel receiver, BattleUnitModel transferer, int amount)
		{
			// Bool to see it exists
			bool dDJJsStardust = transferer.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust);

			// If it has the effect call the function that bursts the Stardust.
			if (dDJJsStardust)
			{
				BattleUnitBuf_DDJJsStardust dDJJsStardustEffect = receiver.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;

				dDJJsStardustEffect.TransferStardust(receiver, transferer, amount);
			}
		}

		public void TransferStardust(BattleUnitModel receiver, BattleUnitModel transferer, int amount)
		{
			BattleUnitBuf_DDJJsStardust dDJJsStardustEffect = transferer.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;

			// If less than needed tranfer it all.
			if (dDJJsStardustEffect.stack < amount)
            {
				BattleUnitBuf_DDJJsStardust.AddBuf(receiver, dDJJsStardustEffect.stack);
				BattleUnitBuf_DDJJsStardust.SubBuf(transferer, dDJJsStardustEffect.stack, 1);
			}
			// If has enough do the amount.
			else
            {
				BattleUnitBuf_DDJJsStardust.AddBuf(receiver, amount);
				BattleUnitBuf_DDJJsStardust.SubBuf(transferer, amount, 1);
			}
		}

		/// Built-in funcs \\\
		public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_DDJJsStardust BattleUnitBuf_DDJJsStardust = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;
			if (BattleUnitBuf_DDJJsStardust == null)
			{
				BattleUnitBuf_DDJJsStardust BattleUnitBuf_DDJJsStardust2 = new BattleUnitBuf_DDJJsStardust(unit);
				BattleUnitBuf_DDJJsStardust2.Add(stack);
				unit.bufListDetail.AddBuf(BattleUnitBuf_DDJJsStardust2);
			}
			else
			{
				BattleUnitBuf_DDJJsStardust.Add(stack);
			}
		}

		public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
		{
			BattleUnitBuf_DDJJsStardust dDJJsStardust = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust) as BattleUnitBuf_DDJJsStardust;
			if (div0sub1 == 0)
			{
				dDJJsStardust.PercentageDeleto(percentage);
			}
			else if (div0sub1 == 1)
			{
				dDJJsStardust.IntegerDeleto(percentage);
			}
		}

		public void IntegerDeleto(int amount)
		{
			this.stack -= amount;

			if (this.stack < 0) this.stack = 0;
		}

		public void PercentageDeleto(int percentage)
		{
			int value = this.stack;

			double percentasdouble = (double)percentage / 100;
			this.stack -= value * (int)percentasdouble;
		}
	}

	public class BattleUnitBuf_MaxHpUp : BattleUnitBuf
	{
		public int hpAdder;

		public int breakGageAdder;

		public BattleUnitBuf_MaxHpUp()
		{
			stack = 0;
		}

		public override StatBonus GetStatBonus()
		{
			return new StatBonus
			{
				hpAdder = hpAdder,
				breakGageAdder = breakGageAdder
			};
		}
	}

	public class BattleUnitBuf_DDJJEgoAwake : BattleUnitBuf
	{
		protected override string keywordId => "DDJJEgoAwake";
		protected override string keywordIconId => "DDJJEgoAwake";

		////////////////// Buff Effect //////////////////

		public BattleUnitBuf_DDJJEgoAwake(BattleUnitModel model)
		{
			this._owner = model;
			this.stack = 0;

			// Increase max hp of DDJJ.
			int to300 = 300 - model.MaxHp;
			model.bufListDetail.AddBuf(new BattleUnitBuf_MaxHpUp
			{
				hpAdder = to300,
				breakGageAdder = to300 / 3
			});

			model.RecoverHP(300);
		}

        public override void OnRoundEnd()
        {
			this._owner.TakeDamage(10);
			this._owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.AllPowerUp, 2, this._owner);
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel card) { this._owner.TakeDamage(5); }

        public override void OnSuccessAttack(BattleDiceBehavior behavior) { BattleUnitBuf_DDJJsStardust.AddBuf(behavior.card.target, 1); }

        ////////////////// Buff Effect //////////////////

        /// Built-in funcs \\\
        public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_DDJJEgoAwake BattleUnitBuf_DDJJEgoAwake = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJEgoAwake) as BattleUnitBuf_DDJJEgoAwake;
			if (BattleUnitBuf_DDJJEgoAwake == null)
			{
				BattleUnitBuf_DDJJEgoAwake BattleUnitBuf_DDJJEgoAwake2 = new BattleUnitBuf_DDJJEgoAwake(unit);
				BattleUnitBuf_DDJJEgoAwake2.Add(stack);
				unit.bufListDetail.AddBuf(BattleUnitBuf_DDJJEgoAwake2);
			}
			else
			{
				BattleUnitBuf_DDJJEgoAwake.Add(stack);
			}
		}

		public static void SubBuf(BattleUnitModel unit, int percentage, int div0sub1)
		{
			BattleUnitBuf_DDJJEgoAwake DDJJEgoAwake = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJEgoAwake) as BattleUnitBuf_DDJJEgoAwake;
			if (div0sub1 == 0)
			{
				DDJJEgoAwake.PercentageDeleto(percentage);
			}
			else if (div0sub1 == 1)
			{
				DDJJEgoAwake.IntegerDeleto(percentage);
			}
		}

		public void IntegerDeleto(int amount)
		{
			this.stack -= amount;
		}

		public void PercentageDeleto(int percentage)
		{
			int value = this.stack;

			double percentasdouble = (double)percentage / 100;
			this.stack -= value * (int)percentasdouble;
		}
	}

	public class BattleUnitBuf_stardustBurstTriggered : BattleUnitBuf
    {
		public BattleUnitBuf_stardustBurstTriggered(BattleUnitModel model)
		{
			this._owner = model;
			this.stack = 0;
		}

		/// Built-in funcs \\\
		public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_stardustBurstTriggered BattleUnitBuf_stardustBurstTriggered = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggered) as BattleUnitBuf_stardustBurstTriggered;
			if (BattleUnitBuf_stardustBurstTriggered == null)
			{
				BattleUnitBuf_stardustBurstTriggered BattleUnitBuf_stardustBurstTriggered2 = new BattleUnitBuf_stardustBurstTriggered(unit);
				BattleUnitBuf_stardustBurstTriggered2.Add(stack);
				unit.bufListDetail.AddBuf(BattleUnitBuf_stardustBurstTriggered2);
			}
			else
			{
				BattleUnitBuf_stardustBurstTriggered.Add(stack);
			}
		}
	}

	public class BattleUnitBuf_stardustBurstTriggeredByUser : BattleUnitBuf
	{
		public BattleUnitBuf_stardustBurstTriggeredByUser(BattleUnitModel model)
		{
			this._owner = model;
			this.stack = 0;
		}

		public override void OnRoundEnd()
        {
			this.Destroy();
        }

		/// Built-in funcs \\\
		public void Add(int add)
		{
			this.stack += add;
		}

		public static void AddBuf(BattleUnitModel unit, int stack)
		{
			BattleUnitBuf_stardustBurstTriggeredByUser BattleUnitBuf_stardustBurstTriggeredByUser = unit.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_stardustBurstTriggeredByUser) as BattleUnitBuf_stardustBurstTriggeredByUser;
			if (BattleUnitBuf_stardustBurstTriggeredByUser == null)
			{
				BattleUnitBuf_stardustBurstTriggeredByUser BattleUnitBuf_stardustBurstTriggeredByUser2 = new BattleUnitBuf_stardustBurstTriggeredByUser(unit);
				BattleUnitBuf_stardustBurstTriggeredByUser2.Add(stack);
				unit.bufListDetail.AddBuf(BattleUnitBuf_stardustBurstTriggeredByUser2);
			}
			else
			{
				BattleUnitBuf_stardustBurstTriggeredByUser.Add(stack);
			}
		}
	}

	/////////////////////////////// Buffs / Effects /////////////////////////////



	/////////////////////////////// BehaviourAction /////////////////////////////

	public class BehaviourAction_DDJJPierce : BehaviourActionBase
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
					RencounterManager.MovingAction movingAction = new RencounterManager.MovingAction((ActionDetail)19, (CharMoveState)3, 4f, true, 0.125f, 1f);
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

	public class DiceCardAbility_OnHitGive2Stardust : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "DDJJsStardust" };
		public static string Desc = "[On Hit] Inflict 2 Stardust.";

		public override void OnSucceedAttack()
		{
			BattleUnitBuf_DDJJsStardust.AddBuf(base.card.target, 2);
		}
	}

	public class DiceCardAbility_OnHitBurst : DiceCardAbilityBase
	{
		public override string[] Keywords => new string[] { "DDJJsStardust" };
		public static string Desc = "[On Hit] Burst target's Stardust.";

		public override void OnSucceedAttack()
		{
			BattleUnitBuf_DDJJsStardust.BurstItAll(base.card.target, owner);
		}
	}

	/////////////////////////////// Dice Passives ///////////////////////////////



	/////////////////////////////// Page Passives ///////////////////////////////

	public class DiceCardSelfAbility_LoseEgoDDJJ : DiceCardSelfAbilityBase
	{
		public override string[] Keywords => new string[] { "DDJJEgoAwake" };
		public static string Desc = "[After Clash] Dispell Ego Awakening on D.D.J.J.";

        public override void OnEndBattle()
        {
            BattleUnitBuf_DDJJEgoAwake ddjjegocheck = owner.bufListDetail.GetActivatedBufList().Find((BattleUnitBuf x) => x is BattleUnitBuf_DDJJEgoAwake) as BattleUnitBuf_DDJJEgoAwake;
			ddjjegocheck.Destroy();
		}
    }

	/////////////////////////////// Page Passives ///////////////////////////////



	///////////////////////////// Key Page Passives /////////////////////////////

	public class PassiveAbility_EmpathicTrigger : PassiveAbilityBase
    {
		// Has to be start instead of when unit is created cuss if the  ne with the keypage is first in team,
		// there wont be any other u8nit alive at that point thus not finding a partner to link up to.
		public override void OnRoundStart()
		{
			bool ddjjBurst = owner.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJBurst);

			if (!ddjjBurst)
			{
				BattleUnitBuf_DDJJBurst.AddBuf(owner, 1);
			}
		}

		public override void OnRoundEnd()
		{
			bool ddjjBurst = owner.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJBurst);

			if (!ddjjBurst)
			{
				BattleUnitBuf_DDJJBurst.AddBuf(owner, 1);
			}
		}
    }

	public class PassiveAbility_LastFriend : PassiveAbilityBase
    {
		public override void OnRoundStart()
		{
			if (!BattleObjectManager.instance.GetAliveList(owner.faction).Exists((BattleUnitModel x) => x != owner))
			{
				owner.battleCardResultLog?.SetPassiveAbility(this);
				owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 1, owner);
			}
		}

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
			// Check if the target as the debuff
			bool ddjjBurst = behavior.card.target.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJsStardust);

			// If has debuff, inflict 2 more stack of Stardust.
			if (ddjjBurst)
            {
				BattleUnitBuf_DDJJsStardust.AddBuf(behavior.card.target, 2);
			}
		}
    }

	public class PassiveAbility_EclosingOne : PassiveAbilityBase
    {
		private bool _egoAwaken = false;
		public bool egoPageObtained = false;
		bool bChangeSkin = false;

		public override void OnRoundStart()
		{
			// The code that gives the buff to librarian is in the Stardust buff.
			bool ddjjegocheck = owner.bufListDetail.GetActivatedBufList().Exists((BattleUnitBuf x) => x is BattleUnitBuf_DDJJEgoAwake);

			// if doesnt have buff return to original skin.
			if (!ddjjegocheck)
            {
				this.owner.view.ResetSkin();
				_egoAwaken = false;
				bChangeSkin = false;
			}

			if (!_egoAwaken && ddjjegocheck && !bChangeSkin)
			{
				_egoAwaken = true;
				bChangeSkin = true;
				this.owner.view.ChangeWorkShopSkin("MyTopStar", "GionasEgoFull");

				this.owner.allyCardDetail.DiscardACardRandomlyByAbility(3);
				this.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 72), false);
			}
		}

        public override void OnUnitCreated()
        {
			// Take this out after nailing the animation of DDJJ's Culmination.
			this.owner.allyCardDetail.AddNewCard(new LorId("MyTopStar", 72), false);
		}
    }

	///////////////////////////// Key Page Passives /////////////////////////////
}
