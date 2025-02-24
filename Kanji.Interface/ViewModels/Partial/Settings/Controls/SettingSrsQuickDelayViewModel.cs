﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    public class SettingSrsQuickDelayViewModel : SettingControlViewModel
    {
        #region Fields

        private double _quickDelayHours;

        #endregion

        #region Properties

        public double QuickDelayHours
        {
            get { return _quickDelayHours; }
            set
            {
                if (_quickDelayHours != value)
                {
                    _quickDelayHours = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public override void InitializeSettings()
        {
            QuickDelayHours = Properties.UserSettings.Instance.VocabSrsDelayHours;
        }

        #endregion

        #region Methods

        public override bool IsSettingChanged()
        {
            return QuickDelayHours != Properties.UserSettings.Instance.VocabSrsDelayHours;
        }

        protected override void DoSaveSetting()
        {
            Properties.UserSettings.Instance.VocabSrsDelayHours = QuickDelayHours;
        }

        #endregion
    }
}
