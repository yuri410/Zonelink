/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.AI
{
    class AIPlayer : Player
    {
        AIDecision decision;

        public AIPlayer(int id)
            : base("Computer",  id)
        {
            Type = PlayerType.LocalAI;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            //if (decision != null)
                //decision.Update(time);
        }

        public override void SetParent(GameState state)
        {
            decision = new AIDecision(state.Field, this);
        }
    }
}
