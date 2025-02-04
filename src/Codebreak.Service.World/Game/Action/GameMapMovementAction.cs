﻿using Codebreak.Service.World.Game.Entity;
using Codebreak.Service.World.Game.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codebreak.Service.World.Game.Action
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class GameMapMovementAction : AbstractGameAction
    {
        /// <summary>
        /// 
        /// </summary>
        public override bool CanAbort => true;

        /// <summary>
        /// 
        /// </summary>
        public MovementPath Path
        {
            get;
            private set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int SkillMapId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SkillCellId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SkillId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="path"></param>
        public GameMapMovementAction(AbstractEntity entity, MovementPath path)
            : base(GameActionTypeEnum.MAP_MOVEMENT, entity, (long)path.MovementTime)
        {
            Path = path;
            SkillId = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Abort(params object[] args)
        {
            int stopCell = 0;
            if (args.Length > 0)
            {                
                stopCell = int.Parse(args[0].ToString());
            }
            else
            {
                stopCell = Entity.CellId;
            }

            // Cas d'une deconnexion
            if (stopCell == Entity.Id)
                stopCell = Entity.CellId;

            base.Abort(args);

            Entity.MovementHandler.MovementFinish(Entity, Path, stopCell);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void Stop(params object[] args)
        {
            base.Stop(args);

            Entity.MovementHandler.MovementFinish(Entity, Path, Path.EndCell);

            if (SkillId != -1 && Entity.MapId == SkillMapId)            
                Entity.Map.InteractiveExecute((CharacterEntity)Entity, SkillCellId, SkillId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string SerializeAs_GameAction()
        {
            return Path.ToString();
        }
    }
}
