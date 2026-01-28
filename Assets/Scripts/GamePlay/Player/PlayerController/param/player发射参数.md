## 发射时间更短，爆发感更强
- [Tooltip("衰减停止的速度阈值，低于此值停止衰减 建议与空中启动速度匹配")] public float releaseMinSpeedThreshold = 10f;
- [Tooltip("线性阻力系数（每秒衰减速度），越大停得越快")] public float releaseDragCoefficient = 50f;
- [Tooltip("满能量时的衰减持续时间")]public float releaseDragTime = 0.2f;
- [Tooltip("释放的最低能量限度，低于此将不会触发发射")]public float releaseThreshold = 0;
- [Tooltip("基础发射速度")]public float baseSpeed = 25f;
- [Tooltip("最大发射速度，能量满时初始速度最大")]public float maxSpeed = 35f;

## 发射时间较长，速度较慢，滞空感更强

- [Tooltip("衰减停止的速度阈值，低于此值停止衰减 建议与空中启动速度匹配")] public float releaseMinSpeedThreshold = 1f;
- [Tooltip("线性阻力系数（每秒衰减速度），越大停得越快")] public float releaseDragCoefficient = 4f;
- [Tooltip("满能量时的衰减持续时间")]public float releaseDragTime = 0.5f;
- [Tooltip("释放的最低能量限度，低于此将不会触发发射")]public float releaseThreshold = 0;
- [Tooltip("基础发射速度")]public float baseSpeed = 18f;
- [Tooltip("最大发射速度，能量满时初始速度最大")]public float maxSpeed = 24f;
