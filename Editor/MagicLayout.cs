using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace R4hulCorleone
{
    [InitializeOnLoad]
    public class MagicLayout
    {
        static MagicLayout()
        {
            EnableResizeableAfterInitialize();
            EnableResizableOnViewChanged();
        }

        private static void EnableResizeableAfterInitialize()
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += MakeAvailableWindowsResizable;
            };
        }

        private static void MakeAvailableWindowsResizable()
        {
            var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var window in windows)
            {
                if (window.titleContent.text == "Scene" || window.titleContent.text == "Game")
                    continue;

                window.minSize = Vector2.zero;
            }
        }

        private static void EnableResizableOnViewChanged()
        {
            HookUpOnViewChanged();
        }

        private static void HookUpOnViewChanged()
        {
            var hostViewType = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
            var viewChangedEvent = hostViewType.GetEvent("actualViewChanged", BindingFlags.NonPublic | BindingFlags.Static);

            var hostViews = Resources.FindObjectsOfTypeAll(hostViewType);
            if (hostViews.Length > 0)
            {
                var hostView = hostViews[0];

                var eventType = viewChangedEvent.EventHandlerType;
                var methodInfo = typeof(MagicLayout).GetMethod("OnViewChanged", BindingFlags.NonPublic | BindingFlags.Static);

                var addMethod = viewChangedEvent.GetAddMethod(true);
                var handler = Delegate.CreateDelegate(eventType, methodInfo);
                addMethod.Invoke(hostView, new object[] { handler });
            }
        }

        private static void OnViewChanged(Object hostView)
        {
            EditorApplication.delayCall += MakeAvailableWindowsResizable;
        }
    }
}