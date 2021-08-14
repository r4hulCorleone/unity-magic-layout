using System;
using System.Reflection;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using Unity.EditorCoroutines.Editor;

namespace RahulCorleone
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
            EditorCoroutineUtility.StartCoroutineOwnerless(MakeResizeableAfterInitialize_Coroutine());
        }

        private static IEnumerator MakeResizeableAfterInitialize_Coroutine()
        {
            yield return null;
            MakeAvailableWindowsResizable();
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
            EditorCoroutineUtility.StartCoroutineOwnerless(HookUpOnViewChanged_Coroutine());
        }

        private static IEnumerator HookUpOnViewChanged_Coroutine()
        {
            yield return null;
            HookUpOnViewChanged();
        }

        [MenuItem("Window/Magic Layout/Enable")]
        private static void HookUpOnViewChanged()
        {
            var hostViewType = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
            var hostViews = Resources.FindObjectsOfTypeAll(hostViewType);

            if (hostViews.Length > 0)
            {
                var hostView = hostViews[0];

                var viewChangedEvent = hostViewType.GetEvent("actualViewChanged", BindingFlags.NonPublic | BindingFlags.Static);
                var eventType = viewChangedEvent.EventHandlerType;
                var methodInfo = typeof(MagicLayout).GetMethod("OnViewChanged", BindingFlags.NonPublic | BindingFlags.Static);

                var addMethod = viewChangedEvent.GetAddMethod(true);
                var handler = Delegate.CreateDelegate(eventType, methodInfo);
                addMethod.Invoke(hostView, new object[] { handler });
            }
            else
            {
                Debug.LogError("Oops, something went wrong!" +
                "\n Please enable Magic Layout by going \"Window/Magic Layout/Enable\"" +
                "\n More Info: If this issue occured after you changed editor layout, It happens sometimes, Hope somebody can help me to fix it");
            }
        }

        private static void OnViewChanged(Object hostView)
        {
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += MakeAvailableWindowsResizable;
            };
        }
    }
}