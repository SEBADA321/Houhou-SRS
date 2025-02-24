﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kanji.Common.Helpers;
using Kanji.Interface.Actors;
using Kanji.Interface.Helpers;
using Kanji.Interface.Models;

namespace Kanji.Interface.Business
{
    public class TrayBusiness
    {/*
        #region Constants

        // Delay before the first check once the TrayBusiness instance is initialized.
        private static readonly TimeSpan BeforeFirstCheckDelay = TimeSpan.FromSeconds(5);

        // Delay before another check attempt is issued when an error occurs during a check.
        private static readonly TimeSpan CheckAttemptErrorDelay = TimeSpan.FromSeconds(2);

        // Maximal number of attempts to check available reviews.
        private static readonly int MaxCheckAttemptsCount = 2;

        #endregion

        #region Static

        // Stores icons.
        private static Icon IdleIcon, AlertIcon;

        /// <summary>
        /// Gets the loaded instance.
        /// </summary>
        public static TrayBusiness Instance { get; private set; }

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        public static void Initialize(TrayWindow window)
        {
            if (Instance == null)
            {
                IdleIcon = new Icon(ResourceHelper.GetResourceStream(ResourceHelper.TrayIconIdle),
                    new System.Drawing.Size(new System.Drawing.Point(16, 16)));
                AlertIcon = new Icon(ResourceHelper.GetResourceStream(ResourceHelper.TrayIconAlert),
                    new System.Drawing.Size(new System.Drawing.Point(16, 16)));
                Instance = new TrayBusiness(window);
            }
            else
            {
                throw new InvalidOperationException(
                    "The instance is already loaded.");
            }
        }

        #endregion

        #region Fields

        // Stores the TrayWindow containing the TaskbarIcon.
        private TrayWindow _window;

        // Timer. Triggers a check at each tick.
        private DispatcherTimer _checkTimer;

        // Stores the last check date.
        private DateTime _lastCheckDate;

        // Stores the last review count retrieved by a check.
        private long _lastCheckReviewsCount;

        // Stores the type of the last notification issued to select an action when clicking the bubble.
        private TrayNotificationEnum _lastNotification;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the TaskbarIcon.
        /// </summary>
        private TaskbarIcon Tray { get { return _window.Tray; } }

        #endregion

        #region Constructors

        private TrayBusiness(TrayWindow window)
        {
            _window = window;

            // Subscribe to the main window close event to show notification if needed.
            NavigationActor.Instance.MainWindowClose += OnMainWindowClosed;

            // Assign icon.
            Tray.Icon = IdleIcon;

            // Handle Tray events.
            Tray.TrayBalloonTipClicked += OnTrayNotificationClicked;
            Tray.TrayMouseDoubleClick += OnTrayDoubleClick;

            // Run a first check.
            Task.Run(() => { Thread.Sleep(BeforeFirstCheckDelay); Check(); });

            // Initialize the check timer.
            _checkTimer = new DispatcherTimer();
            _checkTimer.Interval = Properties.UserSettings.Instance.TrayCheckInterval;
            _checkTimer.Tick += OnCheckTick;
            _checkTimer.Start();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks for reviews.
        /// </summary>
        public void Check()
        {
            _lastCheckDate = DateTime.Now;
            _lastCheckReviewsCount = DoCheck();

            if (Properties.UserSettings.Instance.TrayShowNotifications
                && _lastCheckReviewsCount >
                Properties.UserSettings.Instance.TrayNotificationCountThreshold)
            {
                ShowReviewNotification();
            }

            if (_lastCheckReviewsCount > 0)
            {
                Tray.Icon = AlertIcon;
            }
            else
            {
                Tray.Icon = IdleIcon;
            }
        }

        /// <summary>
        /// Reloads the application configuration to take in account new values.
        /// </summary>
        public void ReloadConfiguration()
        {
            if (_checkTimer.Interval != Properties.UserSettings.Instance.TrayCheckInterval)
            {
                _checkTimer.Stop();
                _checkTimer.Interval = Properties.UserSettings.Instance.TrayCheckInterval;
                _checkTimer.Start();
            }
        }

        /// <summary>
        /// Shows the review alert.
        /// </summary>
        private void ShowReviewNotification()
        {
            string notificationMessage = string.Empty;
            if (_lastCheckReviewsCount == 1)
            {
                notificationMessage = string.Format(
                    "1 review is available.{0}Click to start reviewing.", Environment.NewLine);
            }
            else if (_lastCheckReviewsCount > 1)
            {
                notificationMessage = string.Format(
                    "{0} reviews are available.{1}Click to start reviewing.",
                    _lastCheckReviewsCount, Environment.NewLine);
            }
            else
            {
                notificationMessage = "There are no reviews for now.";
            }

            ShowNotification(TrayNotificationEnum.ReviewNotification, "Houhou", notificationMessage, BalloonIcon.Info);
        }

        /// <summary>
        /// Shows a balloon tip.
        /// </summary>
        /// <param name="notificationType">Notification type.</param>
        /// <param name="title">Title to show in the balloon tip.</param>
        /// <param name="message">Message to show in the balloon tip.</param>
        /// <param name="icon">Icon to show in the balloon tip.</param>
        private void ShowNotification(TrayNotificationEnum notificationType, string title, string message, BalloonIcon icon)
        {
            _lastNotification = notificationType;
            Tray.ShowBalloonTip(title, message, icon);
        }

        /// <summary>
        /// Performs the actual check.
        /// </summary>
        /// <param name="attempt">Attempt number.</param>
        /// <returns>Current reviews count.</returns>
        private long DoCheck(int attempt = 1)
        {
            try
            {
                // Attempt to get the current reviews count.
                if (SrsBusiness.Instance.CurrentReviewInfo != null)
                {
                    return SrsBusiness.Instance.CurrentReviewInfo.AvailableReviewsCount;
                }
                
                // If the CurrentReviewInfo was null, throw up.
                throw new Exception("CurrentReviewInfo is null.");
            }
            catch (Exception ex)
            {
                // In the failure case, try again until the MaxCheckAttemptsCount is reached.
                var log = LogHelper.GetLogger(this.GetType().Name);
                log.ErrorFormat("Review check attempt #{0} failed:{1}{2}",
                    attempt, Environment.NewLine, ex);

                if (attempt < MaxCheckAttemptsCount)
                {
                    log.Info("Attempting review check again.");
                    // After a nap, recursively call back the same method,
                    // but with an incremented attempt #.
                    Thread.Sleep(CheckAttemptErrorDelay);
                    return DoCheck(attempt + 1);
                }
                else
                {
                    // MaxCheckAttemptsCount reached. Stop and return 0.
                    log.Info("Review check abandoned.");
                    return 0;
                }
            }
        }

        /// <summary>
        /// Opens or focuses the main window.
        /// </summary>
        public void OpenOrFocusMainWindow()
        {
            NavigationActor.Instance.OpenOrFocus();
        }

        /// <summary>
        /// Opens or focuses the main window, navigate to the SRS page
        /// and starts a review session.
        /// </summary>
        public void StartReviewing()
        {
            NavigationActor.Instance.OpenOrFocus();
            NavigationActor.Instance.NavigateToReviewSession();
        }

        /// <summary>
        /// Exits the application.
        /// I mean, yeah, for real, that's what the ExitApplication method does.
        /// </summary>
        public void ExitApplication()
        {
            NavigationActor.Instance.CloseMainWindow();
            Program.Shutdown();
        }

        #region Event handlers

        /// <summary>
        /// Event handler triggered when the check timer ticks.
        /// </summary>
        private void OnCheckTick(object sender, EventArgs e)
        {
            Check();
        }

        /// <summary>
        /// Event handler triggered when a tray balloon tip is clicked.
        /// Directs the user to the SRS review module.
        /// </summary>
        private void OnTrayNotificationClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (_lastNotification == TrayNotificationEnum.ReviewNotification)
            {
                StartReviewing();
            }
            else if (_lastNotification == TrayNotificationEnum.ExitNotification)
            {
                ExitApplication();
            }
        }

        /// <summary>
        /// Event handler triggered when the user double-clicks the notification icon.
        /// Opens or focuses the main window.
        /// </summary>
        private void OnTrayDoubleClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OpenOrFocusMainWindow();
        }

        /// <summary>
        /// Event handler triggered when the main window of Houhou is closed.
        /// Issues a notification if the settings approve of that.
        /// </summary>
        private void OnMainWindowClosed()
        {
            if (Properties.UserSettings.Instance.WindowCloseAction == WindowCloseActionEnum.Warn)
            {
                ShowNotification(TrayNotificationEnum.ExitNotification, "Houhou",
                    string.Format("Houhou is still running in the background.{0}Click this bubble to shut it down.", Environment.NewLine),
                    BalloonIcon.Info);
            }
        }

        #endregion

        #endregion
        */
    }
}
