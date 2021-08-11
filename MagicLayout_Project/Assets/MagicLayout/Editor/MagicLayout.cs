using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using Unity.EditorCoroutines.Editor;
using System.Collections;
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

        #region OnInitialize
        private static void EnableResizeableAfterInitialize()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(MakeResizeableAfterInitialize_Coroutine());
        }

        private static IEnumerator MakeResizeableAfterInitialize_Coroutine()
        {
            yield return null;
            MakeAvailableWindowsResizable();
        }
        #endregion

        #region OnViewChanged
        private static void EnableResizableOnViewChanged()
        {
            HookUpViewChanged();
        }

        private static void HookUpViewChanged()
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
            EditorCoroutineUtility.StartCoroutineOwnerless(MakeFocusedWindowResizable_Coroutine());
        }

        private static IEnumerator MakeFocusedWindowResizable_Coroutine()
        {
            yield return null;
            MakeAvailableWindowsResizable();
        }
        #endregion

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
    }
}

