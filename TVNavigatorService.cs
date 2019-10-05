using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views.Accessibility;
using Android.AccessibilityServices;
using Android;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace UssdcodeRead
{
    [Service(Label = "myApp", Permission = Manifest.Permission.BindAccessibilityService)]
    [IntentFilter(new[] { "android.accessibilityservice.AccessibilityService" })]
    [MetaData("android.accessibilityservice.AccessibilityService", Resource = "@xml/accessibility_service_config")]
    public class TVNavigatorService : AccessibilityService
    {
        private AccessibilityManager mAccessibilityManager;
        private Context mContext;
        private static TVNavigatorService mInstance;
        public List<Keycode> PressedKeys = new List<Keycode>();
        public DateTime LastKeyAddedTime = DateTime.Now;
        Dictionary<Keycode, int> keyCodeNumberMappings = new Dictionary<Keycode, int>();
        List<Keycode> allowedNumbers = new List<Keycode>();
        public TVNavigatorService()
        {
            keyCodeNumberMappings = new Dictionary<Keycode, int>();
            keyCodeNumberMappings.Add(Keycode.Num0, 0);
            keyCodeNumberMappings.Add(Keycode.Num1, 1);
            keyCodeNumberMappings.Add(Keycode.Num2, 2);
            keyCodeNumberMappings.Add(Keycode.Num3, 3);
            keyCodeNumberMappings.Add(Keycode.Num4, 4);
            keyCodeNumberMappings.Add(Keycode.Num5, 5);
            keyCodeNumberMappings.Add(Keycode.Num6, 6);
            keyCodeNumberMappings.Add(Keycode.Num7, 7);
            keyCodeNumberMappings.Add(Keycode.Num8, 8);
            keyCodeNumberMappings.Add(Keycode.Num9, 9);

            allowedNumbers = new List<Keycode> {
                Keycode.Num0,
                Keycode.Num1,
                Keycode.Num2,
                Keycode.Num3,
                Keycode.Num4,
                Keycode.Num5,
                Keycode.Num6,
                Keycode.Num7,
                Keycode.Num8,
                Keycode.Num9
            };
        }
        public void init(Context context)
        {
            mContext = Android.App.Application.Context;
            mAccessibilityManager = (AccessibilityManager)mContext.GetSystemService(Context.AccessibilityService);
        }

        public static TVNavigatorService getInstance()
        {
            if (mInstance == null)
            {
                mInstance = new TVNavigatorService();
            }
            return mInstance;
        }
        protected override void OnServiceConnected()
        {
            var accessibilityServiceInfo = ServiceInfo;
            accessibilityServiceInfo.EventTypes = EventTypes.AllMask;
            accessibilityServiceInfo.Flags |= AccessibilityServiceFlags.IncludeNotImportantViews;
            accessibilityServiceInfo.Flags |= AccessibilityServiceFlags.RequestFilterKeyEvents;
            accessibilityServiceInfo.Flags |= AccessibilityServiceFlags.ReportViewIds;
            accessibilityServiceInfo.Flags |= AccessibilityServiceFlags.RequestTouchExplorationMode;
            accessibilityServiceInfo.FeedbackType = Android.AccessibilityServices.FeedbackFlags.AllMask;
            accessibilityServiceInfo.NotificationTimeout = 100;

            SetServiceInfo(accessibilityServiceInfo);
            base.OnServiceConnected();
        }

        protected override bool OnKeyEvent(KeyEvent e)
        {

            var action = e.Action;
            var keyCode = e.KeyCode;
            if (action == KeyEventActions.Down && allowedNumbers.Contains(keyCode))
            {
                //wait for another key pressed
                if (DateTime.Now - LastKeyAddedTime > new TimeSpan(0, 0, 0, 3))
                {
                    PressedKeys.Clear();
                }
                PressedKeys.Add(keyCode);

                var keyNumbers = from p in PressedKeys
                                 join q in keyCodeNumberMappings on p equals q.Key
                                 select q.Value;

                //hit back button and go to item at entered index
                var channelIndex = String.Join("", keyNumbers);

                return true;
            }
            else
            {
                return base.OnKeyEvent(e);
            }
        }

        public override void OnAccessibilityEvent(AccessibilityEvent e)
        {
            try
            {
                //if (e.PackageName == "")
                {
                    //final int eventType = event.getEventType();
                    //switch(eventType) {
                    //    case AccessibilityEvent.TYPE_VIEW_CLICKED:
                    //        do somthing
                    //        break;
                    //    case AccessibilityEvent.TYPE_VIEW_FOCUSED:
                    //        do somthing
                    //        break;
                    //}

                    Console.WriteLine("event type : " + e.EventType);

                    Console.WriteLine("content decription : " + e.ContentDescription);
                    Console.WriteLine("package name : " + e.PackageName);
                    Console.WriteLine("source : " + e.Source);
                    Console.WriteLine("window id : " + e.WindowId);
                    Console.WriteLine("event time : " + e.EventTime);

                    var strBuilderTxt = new StringBuilder();
                    foreach (var txt in e.Text)
                    {
                        strBuilderTxt.Append(txt);
                    }

                    Console.WriteLine("actual text : " + strBuilderTxt);
                }
            }
            catch (Exception e2)
            {
                Console.WriteLine(e2.Message);
            }
        }

        private bool checkAccessibilityEnabled(String serviceName)
        {
            var accessibilityServices =
                    mAccessibilityManager.GetEnabledAccessibilityServiceList(Android.AccessibilityServices.FeedbackFlags.Generic);
            foreach (AccessibilityServiceInfo info in accessibilityServices)
            {
                if (info.Id.Equals(serviceName))
                {
                    return true;
                }
            }
            return false;
        }

        public void goAccess()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionAccessibilitySettings);
            intent.SetFlags(ActivityFlags.NewTask);
            mContext.StartActivity(intent);
        }

        public void performViewClick(AccessibilityNodeInfo nodeInfo)
        {
            if (nodeInfo == null)
            {
                return;
            }
            while (nodeInfo != null)
            {
                if (nodeInfo.Clickable)
                {
                    nodeInfo.PerformAction(Android.Views.Accessibility.Action.Click);
                    break;
                }
                nodeInfo = nodeInfo.Parent;
            }
        }

        public void performBackClick()
        {
            try
            {
                Java.Lang.Thread.Sleep(500);
            }
            catch (Java.Lang.InterruptedException e)
            {
                e.PrintStackTrace();
            }
            PerformGlobalAction(GlobalAction.Back);
        }

        public void performScrollBackward()
        {
            try
            {
                Java.Lang.Thread.Sleep(500);
            }
            catch (Java.Lang.InterruptedException e)
            {
                e.PrintStackTrace();
            }
            PerformGlobalAction((GlobalAction)Android.Views.Accessibility.Action.ScrollBackward);
        }

        public void performScrollForward()
        {
            try
            {
                Java.Lang.Thread.Sleep(500);
            }
            catch (Java.Lang.InterruptedException e)
            {
                e.PrintStackTrace();
            }
            PerformGlobalAction((GlobalAction)Android.Views.Accessibility.Action.ScrollForward);
        }

        public AccessibilityNodeInfo findViewByText(String text, bool clickable)
        {
            AccessibilityNodeInfo accessibilityNodeInfo = RootInActiveWindow;
            if (accessibilityNodeInfo == null)
            {
                return null;
            }
            var nodeInfoList = (List<AccessibilityNodeInfo>)accessibilityNodeInfo.FindAccessibilityNodeInfosByText(text);
            if (nodeInfoList != null && !nodeInfoList.Any())
            {
                foreach (AccessibilityNodeInfo nodeInfo in nodeInfoList)
                {
                    if (nodeInfo != null && (nodeInfo.Checkable == clickable))
                    {
                        return nodeInfo;
                    }
                }
            }
            return null;
        }

        public AccessibilityNodeInfo findViewByID(String id)
        {
            AccessibilityNodeInfo accessibilityNodeInfo = RootInActiveWindow;
            if (accessibilityNodeInfo == null)
            {
                return null;
            }
            var nodeInfoList = accessibilityNodeInfo.FindAccessibilityNodeInfosByViewId(id);
            if (nodeInfoList != null && !nodeInfoList.Any())
            {
                foreach (AccessibilityNodeInfo nodeInfo in nodeInfoList)
                {
                    if (nodeInfo != null)
                    {
                        return nodeInfo;
                    }
                }
            }
            return null;
        }

        public AccessibilityNodeInfo findListItemByIndex(int index)
        {
            AccessibilityNodeInfo accessibilityNodeInfo = RootInActiveWindow;
            if (accessibilityNodeInfo == null)
            {
                return null;
            }
            var nodeInfo = accessibilityNodeInfo.GetChild(index);

            if (nodeInfo != null)
            {
                return nodeInfo;
            }
            return null;
        }

        public void clickListItemByIndex(int index)
        {
            var item = findListItemByIndex(index);
            performViewClick(item);
        }

        public override void OnInterrupt()
        {
            throw new NotImplementedException();
        }
    }
}