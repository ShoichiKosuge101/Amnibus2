using Constants;
using Dungeons;
using Helper;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// ダンジョンジェネレータのエディタ
    /// </summary>
    [CustomEditor(typeof(TestComponent))]
    public class DgDungeonGeneratorEditor 
        : UnityEditor.Editor
    {
        private Layer2D _layer2D;

        private TestComponent _testComponent;
            
        /// <summary>
        /// 初期化
        /// </summary>
        private void OnEnable()
        {
            _testComponent = target as TestComponent;
        }

        /// <summary>
        /// インスペクタの描画
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
                
            if (GUILayout.Button("Generate Dungeon"))
            {
                GenerateDungeon();
            }
            
            if(GUILayout.Button("Clear Existing Prefabs"))
            {
                ClearExistingPrefabs();
            }
        }

        /// <summary>
        /// シーンビューの描画
        /// </summary>
        private void OnSceneGUI()
        {
            if(_layer2D == null)
            {
                return;
            }
        }

        /// <summary>
        /// Prefabの生成
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        private void InstantiatePrefab(GameObject prefab, Vector3 position)
        {
            var instance = Instantiate(prefab, position, Quaternion.identity);
            
            // testComponentの子要素にする
            instance.transform.SetParent(_testComponent.transform);
        }

        /// <summary>
        /// ダンジョン生成
        /// </summary>
        private void GenerateDungeon()
        {
            var dgGenerator = new DgGenerator();
            _layer2D = dgGenerator.Layer2D;
            
            // 既存のprefabをクリア
            ClearExistingPrefabs();
            
            for (int x = 0; x < dgGenerator.Width; x++)
            {
                for (int y = 0; y < dgGenerator.Height; y++)
                {
                    var tile = _layer2D.Get(x, y);
                    Vector3 position = new Vector2(x, y);
                    
                    if (tile == MapTile.Floor)
                    {
                        InstantiatePrefab(_testComponent.floorPrefab, position);
                    }
                    else if (tile == MapTile.Wall)
                    {
                        InstantiatePrefab(_testComponent.wallPrefab, position);
                    }
                    else if (tile == MapTile.Goal)
                    {
                        InstantiatePrefab(_testComponent.goalPrefab, position);
                    }
                    else if (tile == MapTile.Player)
                    {
                        InstantiatePrefab(_testComponent.playerPrefab, position);
                    }
                }
            }
            
            // デバッグ出力
            for (int y = _layer2D.Height - 1; y >= 0; y--)
            {
                string line = "";
                for (int x = 0; x < _layer2D.Width; x++)
                {
                    line += _layer2D.Get(x, y) == MapTile.Floor ? "." : "#";
                }
                Debug.Log(line);
            }
            
            SceneView.RepaintAll();
        }

        /// <summary>
        /// 既存のPrefabをクリア
        /// </summary>
        private void ClearExistingPrefabs()
        {
            // TestComponentの子要素を全て削除
            for (int i = _testComponent.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_testComponent.transform.GetChild(i).gameObject);
            }
        }
    }
}