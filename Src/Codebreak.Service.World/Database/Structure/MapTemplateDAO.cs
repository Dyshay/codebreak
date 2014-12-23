﻿using System.Collections.Generic;
using Codebreak.Framework.Database;
using Codebreak.Service.World.Game;
using Codebreak.Service.World.Network;

namespace Codebreak.Service.World.Database.Structure
{
    /// <summary>
    /// 
    /// </summary>
    [Table("maptemplate")]
    public sealed class MapTemplateDAO : DataAccessObject<MapTemplateDAO>
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SubAreaId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int X
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Data
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DataKey
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Places
        {
            get;
            set;
        }

        private List<int> _fightTeam0Cells, _fightTeam1Cells;

        public List<int> FightTeam0Cells
        {
            get
            {
                if(_fightTeam0Cells == null)
                {
                    _fightTeam0Cells = new List<int>();
                    if(Places != "")
                    {
                        var places = Places.Split('|')[0];
                        var length = places.Length / 2;
                        for(int i = 0; i < length; i++)
                        {
                            _fightTeam0Cells.Add(Util.CharToCell(places.Substring(i * 2, 2)));
                        }    
                    }
                }
                return _fightTeam0Cells;
            }
        }

        public List<int> FightTeam1Cells
        {
            get
            {
                if (_fightTeam1Cells == null)
                {
                    _fightTeam1Cells = new List<int>();
                    if (Places != "")
                    {
                        var places = Places.Split('|')[1];
                        var length = places.Length / 2;
                        for (int i = 0; i < length; i++)
                        {
                            _fightTeam1Cells.Add(Util.CharToCell(places.Substring(i * 2, 2)));
                        }
                    }
                }
                return _fightTeam1Cells;
            }
        }
    }
}
