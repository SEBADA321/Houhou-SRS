﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kanji.Interface.ViewModels
{
    public class WkImportSettingsViewModel : ImportStepViewModel
    {
        #region Fields

        private string _apiKeyError;
        private WkImportViewModel _parent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the error relative to the API key field.
        /// </summary>
        public string ApiKeyError
        {
            get { return _apiKeyError; }
            set
            {
                if (_apiKeyError != value)
                {
                    _apiKeyError = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Events

        public delegate void InvalidApiKeyCheckedHandler(EventArgs e, object sender);
        /// <summary>
        /// Event called when trying to get to the next step with an invalid API key.
        /// </summary>
        public event InvalidApiKeyCheckedHandler InvalidApiKeyChecked;

        #endregion

        #region Constructors

        public WkImportSettingsViewModel(ImportModeViewModel parent)
            : base(parent)
        {
            _parent = (WkImportViewModel)parent;
        }

        #endregion

        #region Methods

        public override Task<bool> OnNextStep()
        {
            // Check the API key field.
            bool isApiOk = true;
            _parent.ApiKey = _parent.ApiKey.Trim().ToLower();
            Regex apiRegex = new Regex("^[a-f0-9-]{36}$");
            if (string.IsNullOrWhiteSpace(_parent.ApiKey))
            {
                ApiKeyError = "Please enter your API key.";
                isApiOk = false;
            }
            else if (_parent.ApiKey.Length != 36)
            {
                ApiKeyError = "The API key is incorrect (should be a 36 characters string).";
                isApiOk = false;
            }
            else if (!apiRegex.IsMatch(_parent.ApiKey))
            {
                ApiKeyError = "The API key you entered is not a valid WaniKani API key.";
                isApiOk = false;
            }

            if (isApiOk)
            {
                // Our key is ok. Continue.
                ApiKeyError = string.Empty;

                // Save the API key and Tags fields in the user settings.
                Properties.UserSettings.Instance.WkApiKey = _parent.ApiKey;
                Properties.UserSettings.Instance.WkTags = _parent.Tags;

                return Task.FromResult(true);
            }

            // Our key is not ok. Stop.
            if (InvalidApiKeyChecked != null)
            {
                InvalidApiKeyChecked(new EventArgs(), this);
            }
            return Task.FromResult(false);
        }

        #endregion
    }
}
