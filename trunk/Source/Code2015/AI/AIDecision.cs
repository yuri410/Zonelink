using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Code2015.Logic;

namespace Code2015.AI
{
    class AIDecision
    {
        AIPlayer player;
        PlayerArea area;

        float decisionTime;

        

        [SLGValue]
        const float AIDecisionDelay = 8;
        [SLGValue]
        const float DecisionRandom = 4;

        public AIDecision(AIPlayer player)
        {
            this.player = player;
            this.area = player.Area;
        }

        public void Update(GameTime time)
        {
            // 电脑单一决策。
            // 每隔一段时间

            // .25的概率
            // 寻找附近权值较大城市
            // 占领。

            // .75的概率
            // 选择一个城市。根据附近情况1对1建立工厂附加物。
            // 其余随机填充


            if (decisionTime < 0)
            {
                const float P = .25f;

                float ran = Randomizer.GetRandomSingle();

                if (ran < P)
                {

                }
                else
                {

                }

                decisionTime = AIDecisionDelay + Randomizer.GetRandomSingle() * DecisionRandom;
            }
            else
            {
                decisionTime -= time.ElapsedGameTimeSeconds;
            }

        }
    }
}
