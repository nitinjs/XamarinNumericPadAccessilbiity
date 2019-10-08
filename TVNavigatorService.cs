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
        public TVNavigatorService()
        {
            
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
            //this code never gets executed
            var action = e.Action;
            var keyCode = e.KeyCode;

            Console.WriteLine("key code : " + keyCode);

            return base.OnKeyEvent(e);
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


        public override void OnInterrupt()
        {
            throw new NotImplementedException();
        }
    }
}