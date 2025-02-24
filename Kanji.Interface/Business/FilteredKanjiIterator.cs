﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanji.Database.Dao;
using Kanji.Database.Entities;
using Kanji.Database.Models;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    public class FilteredKanjiIterator : FilteredItemIterator<KanjiEntity>
    {
        #region Fields

        private KanjiDao _kanjiDao;

        #endregion

        #region Constructors

        public FilteredKanjiIterator(KanjiFilter filter)
            : base(filter)
        {
            _kanjiDao = new KanjiDao();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies the filter.
        /// </summary>
        /// <returns>Filtered set.</returns>
        protected override IAsyncEnumerable<KanjiEntity> DoFilter()
        {
            KanjiFilter filter = (KanjiFilter)_currentFilter;
            string anyReadingFilter = filter.MainFilterMode == KanjiFilterModeEnum.AnyReading ?
                filter.MainFilter : string.Empty;
            string kunYomiFilter = filter.MainFilterMode == KanjiFilterModeEnum.KunYomi ?
                filter.MainFilter : string.Empty;
            string onYomiFilter = filter.MainFilterMode == KanjiFilterModeEnum.OnYomi ?
                filter.MainFilter : string.Empty;
            string nanoriFilter = filter.MainFilterMode == KanjiFilterModeEnum.Nanori ?
                filter.MainFilter : string.Empty;
            string meaningFilter = filter.MainFilterMode == KanjiFilterModeEnum.Meaning ?
                filter.MainFilter : string.Empty;
            RadicalGroup[] radicalGroups = filter.Radicals.SelectMany(r => r.Reference.RadicalGroups).ToArray();

            return _kanjiDao.GetFilteredKanji(radicalGroups, filter.TextFilter, meaningFilter,
                anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter, filter.JlptLevel, filter.WkLevel);
        }

        /// <summary>
        /// Returns the item count.
        /// </summary>
        protected override async Task<int> GetItemCount()
        {
            KanjiFilter filter = (KanjiFilter)_currentFilter;
            string anyReadingFilter = filter.MainFilterMode == KanjiFilterModeEnum.AnyReading ?
                filter.MainFilter : string.Empty;
            string kunYomiFilter = filter.MainFilterMode == KanjiFilterModeEnum.KunYomi ?
                filter.MainFilter : string.Empty;
            string onYomiFilter = filter.MainFilterMode == KanjiFilterModeEnum.OnYomi ?
                filter.MainFilter : string.Empty;
            string nanoriFilter = filter.MainFilterMode == KanjiFilterModeEnum.Nanori ?
                filter.MainFilter : string.Empty;
            string meaningFilter = filter.MainFilterMode == KanjiFilterModeEnum.Meaning ?
                filter.MainFilter : string.Empty;
            RadicalGroup[] radicalGroups = filter.Radicals.SelectMany(r => r.Reference.RadicalGroups).ToArray();

            return (int)await _kanjiDao.GetFilteredKanjiCount(radicalGroups, filter.TextFilter, meaningFilter,
                anyReadingFilter, onYomiFilter, kunYomiFilter, nanoriFilter, filter.JlptLevel, filter.WkLevel);
        }

        #endregion
    }
}
