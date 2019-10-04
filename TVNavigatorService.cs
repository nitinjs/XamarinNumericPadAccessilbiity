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
            var accessibilityServiceInfo = new AccessibilityServiceInfo();
            accessibilityServiceInfo.Flags = AccessibilityServiceFlags.RequestFilterKeyEvents;//enum 1 
            accessibilityServiceInfo.EventTypes = EventTypes.AllMask;
            accessibilityServiceInfo.FeedbackType = Android.AccessibilityServices.FeedbackFlags.AllMask;

            SetServiceInfo(accessibilityServiceInfo);
            base.OnServiceConnected();
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

        //public boolean onKeyEvent(KeyEvent event) {
        //        int action = event.getAction();
        //    int keyCode = event.getKeyCode();
        //    if (action == KeyEvent.ACTION_UP) {
        //        if (keyCode == KeyEvent.KEYCODE_VOLUME_UP) {
        //            Log.d("Hello", "KeyUp");
        //        } else if (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN) {
        //            Log.d("Hello", "KeyDown");
        //        }
        //        return true;
        //    } else {
        //        return super.onKeyEvent(event);
        //    }
        //} 

        public override void OnInterrupt()
        {
            throw new NotImplementedException();
        }
    }
}