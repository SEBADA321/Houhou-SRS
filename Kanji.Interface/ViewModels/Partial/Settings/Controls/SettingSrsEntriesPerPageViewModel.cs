﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    public class SettingSrsEntriesPerPageViewModel : SettingControlViewModel
    {
        #region Fields

        private int _entriesPerPage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number of entries per page.
        /// </summary>
        public int EntriesPerPage
        {
            get { return _entriesPerPage; }
            set
            {
                if (_entriesPerPage != value)
                {
                    _entriesPerPage = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public override void InitializeSettings()
        {
            EntriesPerPage = Properties.UserSettings.Instance.SrsEntriesPerPage;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return Properties.UserSettings.Instance.SrsEntriesPerPage != _entriesPerPage;
        }

        protected override void DoSaveSetting()
        {
            Properties.UserSettings.Instance.SrsEntriesPerPage = _entriesPerPage;
        }

        #endregion
    }
}
