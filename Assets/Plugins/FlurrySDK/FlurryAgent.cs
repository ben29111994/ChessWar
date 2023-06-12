

using System;
using System.Collections.Generic;

namespace FlurrySDKInternal
{
    public abstract class FlurryAgent : IDisposable
    {
        public abstract class AgentBuilder
        {
            public abstract void Build(string apiKey);

            public abstract void WithCrashReporting(bool crashReporting);

            public abstract void WithContinueSessionMillis(long sessionMillis);

            public abstract void WithIncludeBackgroundSessionsInMetrics(bool includeBackgroundSessionsInMetrics);

            public abstract void WithLogEnabled(bool enableLog);

            public abstract void WithLogLevel(FlurrySDK.Flurry.LogLevel logLevel);

            public abstract void WithMessaging(bool enableMessaging);
        }

        public abstract void SetAge(int age);

        public abstract void SetGender(FlurrySDK.Flurry.Gender gender);

        public abstract void SetReportLocation(bool reportLocation);

        public abstract void SetSessionOrigin(string originName, string deepLink);

        public abstract void SetUserId(string userId);

        public abstract void SetVersionName(string versionName);

        public abstract void AddOrigin(string originName, string originVersion);

        public abstract void AddOrigin(string originName, string originVersion, IDictionary<string, string> originParameters);

        public abstract void AddSessionProperty(string name, string value);

        public abstract void SetMessagingListener(FlurrySDK.Flurry.IFlurryMessagingListener flurryMessagingListener);

        public abstract int GetAgentVersion();

        public abstract string GetReleaseVersion();

        public abstract string GetSessionId();

        public abstract int LogEvent(string eventId);

        public abstract int LogEvent(string eventId, bool timed);

        public abstract int LogEvent(string eventId, IDictionary<string, string> parameters);

        public abstract int LogEvent(string eventId, IDictionary<string, string> parameters, bool timed);

        public abstract void EndTimedEvent(string eventId);

        public abstract void EndTimedEvent(string eventId, IDictionary<string, string> parameters);

        public abstract void OnPageView();

        public abstract void OnError(string errorId, string message, string errorClass);

        public abstract void OnError(string errorId, string message, string errorClass, IDictionary<string, string> parameters);

        public abstract void LogBreadcrumb(string crashBreadcrumb);

        public abstract int LogPayment(string productName, string productId, int quantity, double price,
                                       string currency, string transactionId, IDictionary<string, string> parameters);

        public abstract void SetIAPReportingEnabled(bool enableIAP);

        public abstract void Dispose();

    };
}
