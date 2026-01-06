项目结构
```
Assets/                  // Unity默认根目录，所有游戏资源都放在这里
├── Art/                 // 所有美术资源（按类型+功能细分）
│   ├── Sprites/         // 2D精灵图（PNG/PSD等）
│   │   ├── Player/     // 玩家精灵（跳跃/奔跑/落地/死亡等帧）
│   │   ├── Enemies/     // 敌人精灵（不同敌人类型的动画帧）
│   │   ├── Platforms/   // 平台精灵（普通/易碎/移动平台等）
│   │   ├── Pickups/     // 道具精灵（金币/加血/增益道具等）
│   │   ├── UI/          // UI精灵（按钮/血条/分数面板/图标等）
│   │   └── Environment/ // 场景环境精灵（背景/地面/障碍物/装饰等）
│   ├── Animations/      // 动画相关资源
│   │   ├── AnimatorControllers/ // 动画控制器（玩家/敌人的状态机）
│   │   ├── AnimationClips/       // 动画片段（玩家跳跃/奔跑、敌人移动等）
│   │   └── BlendTrees/           // 混合树（2D移动/跳跃的平滑过渡）
│   ├── Tilemaps/        // 2D瓦片地图（关卡核心）
│   │   ├── Tilesets/    // 瓦片集（地面/墙壁/陷阱等瓦片素材）
│   │   └── Maps/        // 瓦片地图文件（关卡的瓦片布局）
│   └── Materials/       // 2D材质（精灵的透明/发光/着色器等）
├── Audio/               // 音频资源
│   ├── BGM/             // 背景音乐（关卡BGM、菜单BGM）
│   └── SFX/             // 音效（跳跃/落地/拾取/碰撞/敌人攻击等）
├── Scripts/             // 所有C#脚本（按功能分层）
│   ├── Core/            // 核心系统（通用、跨场景的基础逻辑）
│   │   ├── Singleton/   // 单例模板（游戏管理器/输入管理等）
│   │   ├── GameManager.cs // 游戏总控（分数/生命/关卡状态）
│   │   ├── InputManager.cs // 输入管理（适配新InputSystem/旧Input）
│   │   └── SceneManager.cs // 场景切换（菜单→关卡、关卡重玩等）
│   ├── Gameplay/        // 游戏玩法核心逻辑（贴合2D跳跃玩法）
│   │   ├── Player/      // 玩家相关脚本
│   │   │   ├── PlayerMovement.cs // 移动/跳跃/空中二段跳逻辑
│   │   │   ├── PlayerCollision.cs // 玩家碰撞检测（落地/撞敌人/捡道具）
│   │   │   └── PlayerState.cs     // 玩家状态管理（站立/跳跃/受伤/死亡）
│   │   ├── Enemies/     // 敌人相关脚本
│   │   │   ├── EnemyAI.cs       // 敌人基础AI（巡逻/追击/攻击）
│   │   │   └── EnemyHealth.cs   // 敌人血量/死亡逻辑
│   │   ├── Platforms/   // 平台相关脚本
│   │   │   ├── MovingPlatform.cs // 移动平台逻辑
│   │   │   └── BreakablePlatform.cs // 易碎平台逻辑
│   │   ├── Pickups/     // 道具相关脚本
│   │   │   └── PickupLogic.cs // 拾取加分/增益/加血逻辑
│   │   └── Level/       // 关卡相关脚本
│   │       ├── LevelManager.cs // 关卡胜利/失败条件
│   │       └── Spawner.cs      // 敌人/道具生成器
│   ├── UI/              // UI相关脚本
│   │   ├── UIScore.cs   // 分数显示更新
│   │   ├── UIPause.cs   // 暂停菜单逻辑
│   │   └── UIGameOver.cs // 游戏结束面板逻辑
│   └── Editor/          // 编辑器扩展脚本（仅编辑模式生效，非运行时）
│       └── CustomEditorTools.cs // 自定义编辑器工具（比如快速创建关卡）
├── Scenes/              // 场景文件（按用途分类）
│   ├── Menu/            // 菜单场景
│   │   ├── MainMenu.unity // 主菜单
│   │   └── Settings.unity // 设置界面
│   ├── Levels/          // 关卡场景（命名规范：Level_XX）
│   │   ├── Level_01.unity
│   │   └── Level_02.unity
│   └── Test/            // 测试场景（单独测试玩家/敌人/平台）
│       ├── Test_Player.unity
│       └── Test_Platforms.unity
├── Prefabs/             // 预制体（可直接复用的游戏对象）
│   ├── Player/          // 玩家预制体（包含精灵/动画/脚本/碰撞体）
│   ├── Enemies/         // 敌人预制体（不同敌人类型）
│   ├── Platforms/       // 平台预制体（普通/移动/易碎）
│   ├── Pickups/         // 道具预制体
│   ├── UI/              // UI预制体（按钮/面板/血条）
│   └── Environment/     // 环境预制体（障碍物/装饰）
├── Resources/           // Unity自动加载的资源（可选，新手推荐）
│   └── Configs/         // 配置文件（玩家属性/敌人数值/关卡参数）
├── ThirdParty/          // 第三方插件（统一管理，便于升级/移除）
│   ├── DOTween/         // 动画插值插件（2D移动/UI过渡）
│   └── InputSystem/     // Unity新输入系统
└── Settings/            // 自定义配置（比如Excel/JSON配置表）
```