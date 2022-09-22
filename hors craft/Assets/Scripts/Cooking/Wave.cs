// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Wave
using Common.Managers;
using System.Collections.Generic;

namespace Cooking
{
	public class Wave
	{
		public List<CustomerConfig> customers = new List<CustomerConfig>();

		public List<CustomerServedInfo> servedCustomers = new List<CustomerServedInfo>();

		public float timeLimit;

		public int moneyCollected;

		public float prestigeCollected;

		public BonusGoalInfo bonusGoal;

		public float timer;

		public int positiveServedCustomers;

		public int negativeServedCustomers;

		public bool isDone => servedCustomers.Count == customers.Count || timer > timeLimit;

		public Wave()
		{
			timer = 0f;
			customers = new List<CustomerConfig>();
			servedCustomers = new List<CustomerServedInfo>();
			moneyCollected = 0;
			prestigeCollected = 0f;
			bonusGoal = null;
			timeLimit = 0f;
			positiveServedCustomers = 0;
			negativeServedCustomers = 0;
		}

		public void NotifyCustomerServed(int money, float timeLeft)
		{
			CustomerServedInfo customerServedInfo = new CustomerServedInfo();
			customerServedInfo.moneyLeft = money;
			customerServedInfo.timeLeft = timeLeft;
			customerServedInfo.positive = true;
			CustomerServedInfo item = customerServedInfo;
			positiveServedCustomers++;
			servedCustomers.Add(item);
			if (servedCustomers.Count < customers.Count)
			{
				Manager.Get<CookingManager>().workController.cookingGameplay.FillContinouslySlider(1f / (float)(customers.Count - 1));
			}
		}

		public void NotifyCustomerLeftHungry()
		{
			CustomerServedInfo customerServedInfo = new CustomerServedInfo();
			customerServedInfo.moneyLeft = 0f;
			customerServedInfo.timeLeft = 0f;
			customerServedInfo.positive = false;
			CustomerServedInfo item = customerServedInfo;
			negativeServedCustomers++;
			servedCustomers.Add(item);
		}

		public override string ToString()
		{
			return "Time: " + timer + " customers count: " + customers.Count + " served customers: " + servedCustomers.Count + " moneyCollected " + moneyCollected + " timeLimit " + timeLimit;
		}
	}
}
