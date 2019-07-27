using UnityEngine;
using UnityEditor;

namespace Mkey
{
    [CustomEditor(typeof(MatchPlayer))]
    public class MatchPlayerEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            #region test
            if (EditorApplication.isPlaying)
            {
                if (test = EditorGUILayout.Foldout(test, "Test"))
                {
                    MatchPlayer sP = (MatchPlayer)target;
                    #region coins
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Add 500 coins"))
                    {
                        sP?.AddCoins(500);
                    }
                    if (GUILayout.Button("Clear coins"))
                    {
                        sP?.SetCoinsCount(0);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Set 500 coins"))
                    {

                        sP?.SetCoinsCount(500);
                    }
                    if (GUILayout.Button("Add coins -500"))
                    {
                        sP?.AddCoins(-500);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion coins

                    #region scenes
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Scene 0"))
                    {
                        SceneLoader.Instance?.LoadScene(0);
                    }
                    if (GUILayout.Button("Scene 1"))
                    {
                        SceneLoader.Instance?.LoadScene(1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion scenes

                    #region stars
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Inc stars"))
                    {
                        sP?.AddStars(1);
                    }

                    if (GUILayout.Button("Dec stars"))
                    {
                        sP?.AddStars(-1);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion stars

                    #region life
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Inc life"))
                    {
                        MatchPlayer.Instance.AddLifes(1);
                    }

                    if (GUILayout.Button("Dec life"))
                    {
                        sP?.AddLifes(-1);
                    }

                    if (GUILayout.Button("Inf life"))
                    {
                        sP?.StartInfiniteLife(12);
                    }

                    if (GUILayout.Button("End inf life"))
                    {
                        sP?.CleanInfiniteLife();
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion life

                    #region score
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Add score 200"))
                    {
                        sP?.AddScore(200);
                    }
                    if (GUILayout.Button("Add score 10"))
                    {
                        sP?.AddScore(10);
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion score
                    if (GUILayout.Button("Reset to default"))
                    {
                        sP?.SetDefaultData();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Goto play mode for test");
            }
            #endregion test
        }
    }
}