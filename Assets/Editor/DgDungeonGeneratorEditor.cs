using Constants;
using Dungeons;
using Dungeons.Factory;
using Dungeons.Interface;
using Helper;
using Manager;
using Manager.Interface;
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

        private IDungeonGenerator _dgGenerator;
        private TestComponent _testComponent;
        
        private IMapManager _mapManager;
            
        /// <summary>
        /// 初期化
        /// </summary>
        private void OnEnable()
        {
            _testComponent = target as TestComponent;
            
            _mapManager = new MapManager();
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
            
            // 2分木ダンジョンの場合
            if (_dgGenerator is DgGenerator standardDgGenerator)
            {
                // 区画と部屋の範囲を描画
                Handles.color = Color.red;
                foreach (var division in standardDgGenerator.Divisions)
                {
                    DrawRect(division.Outer, Color.red);
                    DrawRect(division.Room, Color.green);
                }
            }
        }

        /// <summary>
        /// 四角形を描画するヘルパー関数
        /// </summary>
        private void DrawRect(DgRect rect, Color color)
        {
            Handles.color = color;
            Vector2 topLeft = new Vector2(rect.Left, rect.Top);
            Vector2 topRight = new Vector2(rect.Right, rect.Top);
            Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
            Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);

            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);
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
            _dgGenerator = new StandardDungeonFactory().CreateDungeon(
                _testComponent.width, 
                _testComponent.height,
                _testComponent.minRoomSize,
                _testComponent.maxRoomSize,
                _testComponent.outerMargin
                );
            
            var dgMap2D = _dgGenerator.GetLayer();
            
            // マップ管理クラスに登録
            _mapManager.Initialize(dgMap2D, _testComponent.mapDisplay);
            // 最新のマップ情報を取得
            _layer2D = _mapManager.GetMap();
            
            // 既存のprefabをクリア
            ClearExistingPrefabs();
            
            for (int x = 0; x < _layer2D.Width; x++)
            {
                for (int y = 0; y < _layer2D.Height; y++)
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
                    else if(tile == MapTile.Enemy)
                    {
                        InstantiatePrefab(_testComponent.enemyPrefab, position);
                    }
                    else if (tile == MapTile.Treasure)
                    {
                        InstantiatePrefab(_testComponent.item01Prefab, position);
                    }
                }
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