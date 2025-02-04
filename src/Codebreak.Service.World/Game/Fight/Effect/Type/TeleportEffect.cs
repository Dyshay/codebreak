﻿using Codebreak.Service.World.Game.Action;
using Codebreak.Service.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Fight.Effect.Type
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TeleportEffect : AbstractSpellEffect
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CastInfos"></param>
        /// <returns></returns>
        public override FightActionResultEnum ApplyEffect(CastInfos CastInfos)
        {
            return ApplyTeleport(CastInfos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="castInfos"></param>
        /// <returns></returns>
        public static FightActionResultEnum ApplyTeleport(CastInfos castInfos)
        {
            var caster = castInfos.Caster;
            var cell = caster.Fight.GetCell(castInfos.CellId);

            if (cell != null)
            {
                caster.Fight.Dispatch(WorldMessage.GAME_ACTION(GameActionTypeEnum.MAP_TELEPORT, caster.Id, caster.Id + "," + castInfos.CellId));

                return caster.SetCell(cell);
            }

            return FightActionResultEnum.RESULT_NOTHING;
        }
    }
}
