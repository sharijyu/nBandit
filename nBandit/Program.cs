using System;
using System.Collections.Generic;
using System.Linq;

namespace nBandit
{
	class Program
	{
        //Box-Muller法により、ガウシアンに従うランダム値を返す
        public class GaussianDist
        {
            private double mu;
            private double sigma;
            private Random r;

            public GaussianDist(double Mu, double Sigma, int Seed)
            {
                this.mu = Mu;
                this.sigma = Sigma;
                this.r = new Random(Seed);
            }
            public double GaussianRandom()
            {
                double x = r.NextDouble();
                double y = r.NextDouble();
                double result = sigma * Math.Sqrt(-2.0 * Math.Log(x)) * Math.Cos(2.0 * Math.PI * y) + mu;

                return result;
            }
        }

		static void Main(string[] args)
		{
			int n = 10; //n-bandit
            double epsilon = 0.01;
			int tryNumber = 2000; //試行回数
			int maxStepNum = 1000; //各試行における最大ステップ数
			double initEstimation = 0.0;
			var Q = new List<double>();
			double mu = 0.0;
			double sigma = 1.0;
			var q_star = new List<double>();
			var gaussian = new GaussianDist(mu, sigma, 543);
			var whiteNoise = new GaussianDist(mu, sigma, 653);
			double reward = 0.0;
			var RewardSum = new List<double>();
			var eachArmChosenNum = new List<int>();


			//totalRewardを初期化
			for (int i = 0; i < maxStepNum; i++)
			{
				RewardSum.Add(0.0);
			}

            var randomForEach = new Random();
            var randomForEpsilon = new Random(478);
			//tryNumまでの各試行のループルーチン
			for (int i = 0; i < tryNumber; i++)
			{
				for (int k = 0; k < n; k++)
				{
                    q_star.Add(gaussian.GaussianRandom());
					Q.Add(initEstimation);
					eachArmChosenNum.Add(0);
				}

				for (int j = 0; j < maxStepNum; j++)
				{
                    //行動決定ルーチン+行動実行ルーチン
                    var random = randomForEach.NextDouble();
					var maxQ = Q.Max();
					var maxIndex = Q.IndexOf(maxQ);
                    if (random <= epsilon) {
                        int randomIndex = randomForEpsilon.Next(10);
                        reward = q_star[randomIndex] + whiteNoise.GaussianRandom();
						eachArmChosenNum[randomIndex] = eachArmChosenNum[randomIndex] + 1;
						Q[randomIndex] = Q[randomIndex] + (reward - Q[randomIndex]) / (double)(eachArmChosenNum[randomIndex]);
                    } else {
						reward = q_star[maxIndex] + whiteNoise.GaussianRandom();
						eachArmChosenNum[maxIndex] = eachArmChosenNum[maxIndex] + 1;
						Q[maxIndex] = Q[maxIndex] + (reward - Q[maxIndex]) / (double)(eachArmChosenNum[maxIndex]);
                    }
					//合計報酬の計算
					RewardSum[j] += reward;
				}
				q_star.Clear();
				Q.Clear();
				eachArmChosenNum.Clear();
			}

			for (int k = 0; k < maxStepNum; k++)
			{
				Console.WriteLine("{0}\t{1}", k+1, RewardSum[k] / (double)tryNumber);
			}
		}
	}
}
