﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Common.Utility;
using Kanji.Database.Entities;

namespace Kanji.Interface.Models
{
    public class KanjiFilter : Filter<KanjiEntity>
    {
        /// <summary>
        /// Gets or sets the main filter string.
        /// </summary>
        public string MainFilter { get; set; }

        /// <summary>
        /// Gets or sets the filter mode, defining how the main
        /// filter is applied.
        /// </summary>
        public KanjiFilterModeEnum MainFilterMode { get; set; }

        /// <summary>
        /// Gets or sets the text filter, i.e. the text containing all
        /// kanji that will be requested before applying other filters.
        /// </summary>
        public string TextFilter { get; set; }

        /// <summary>
        /// Gets or sets the array of radicals used to filter out kanji
        /// which do not contain all of them.
        /// </summary>
        public FilteringRadical[] Radicals { get; set; }
        
        /// <summary>
        /// Gets or sets the JLPT level vocab filter.
        /// </summary>
        public int JlptLevel { get; set; }

        /// <summary>
        /// Gets or sets the WaniKani level vocab filter.
        /// </summary>
        public int WkLevel { get; set; }

        public KanjiFilter()
        {
            Radicals = new FilteringRadical[0] { };
        }

        public override bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(MainFilter)
                && string.IsNullOrWhiteSpace(TextFilter)
                && JlptLevel > Levels.MaxJlptLevel
                && WkLevel < Levels.MinWkLevel
                && !Radicals.Any();
        }

        public override Filter<KanjiEntity> Clone()
        {
            return new KanjiFilter()
            {
                ForceFilter = this.ForceFilter,
                MainFilter = this.MainFilter,
                MainFilterMode = this.MainFilterMode,
                TextFilter = this.TextFilter,
                Radicals = (FilteringRadical[])this.Radicals.Clone(),
                JlptLevel = this.JlptLevel,
                WkLevel = this.WkLevel
            };
        }
    }
}
