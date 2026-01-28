using Cinemachine;
using GamePlay.Player.Interface;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- 状态机引用 ---
    public PlayerStateMachine StateMachine { get; private set; }
    public IdleState IdleState { get; private set; }
    public RunState RunState { get; private set; }
    public BrakeState BrakeState { get; private set; }
    public MidAirState MidAirState { get; private set; }
    public LaunchedState LaunchedState { get; private set; }
    public ClimbState ClimbState { get; private set; }
    public ReleaseState ReleaseState { get; private set; }
    public DieState DieState { get; private set; }

    // --- 组件引用 (根据报错，统一使用状态脚本里期待的名字) ---
    public Rigidbody2D rb { get; private set; } // 你要求的写小写
    // 如果报错说找不到 RB，请把上面的 rb 改成 RB，或者把状态脚本里的 RB 改成 rb
    // 鉴于你喜欢小写，我建议统一把状态脚本里的 RB 改成 rb，但为了现在能跑通，我先加个兼容：
    public Rigidbody2D RB => rb;

    public Animator Anim { get; private set; }
    public Collider2D col { get; private set; }

    [Header("震屏组件引用")]
    public CinemachineImpulseSource impulseSource;

    [Header("检测器引用")]
    // 对应报错里的 groundedCheckerManager
    public GroundedCheckerManager groundedCheckerManager;
    public CanClimbLeftCheckerManager canClimbLeftCheckerManager;//攀爬装置暂且不用
    public CanClimbRightCheckerManager canClimbRightCheckerManager;

    [Header("移动参数")]
    public bool isFacingRight = true; // 初始朝向（假设你的素材默认面朝右）
    public float moveSpeedAcc = 50f;
    public float maxMoveSpeed = 8f;
    public float minMoveSpeed = 0.1f;
    public float turnRoundBrakeDec = 30f;//转向刹车加速度,数值偏大，不然不够丝滑
    public float enforceDeceleraion = 20f;

    [Header("空中参数")]
    // public float moveSpeedAccInMidAir = 25f;
    // public float minMoveSpeedInMidAir = 6f;
    public float maxMoveSpeedInMidAir = 10f;
    // public float enforceMoveAccSpeedInMidAir = 15f;
    [Tooltip("无输入时的空气阻力（越小惯性越大）")] public float airDragWithoutInput = 0.3f;
    [Tooltip("有输入时的响应加速度")] public float airResponsiveness = 30f;
    [Tooltip("空中转向时的刹车力度")] public float airTurnBrakeForce = 200f;


    [Header("跳跃参数")]
    public float defaultGravityScale;//默认重力数值，在inspector中设置为3
    public float jumpSpeedInitial = 12f;
    public float varJumpTime = 0.2f;
    public bool canVarJump = false;
    public float gravityContractionThreshold = 1f;
    public float gravityContractionScale = 0.5f;
    public float jumpBufferTime;
    public float jumpCoyoteTime;//跳跃土狼时间，使玩家在离开平台踩空的几帧内也可以跳出来
    public float jumpCoyoteTimer;

    [Header("攀爬参数")]
    public float climbTime = 2.0f;
    public float wallJumpSpeed = 15f;
    public Vector2 wallJumpDirection = new Vector2(1, 1);

    [Header("发射参数")]
    // public float speedLimitOffTimer;
    // 初始速度越高，阻力系数越大，爆发感越强
    // 速度衰减低于阈值或衰减时间到达为两种直接的退出状态方式
    [Tooltip("衰减停止的速度阈值，低于此值停止衰减 建议与空中启动速度匹配")] public float releaseMinSpeedThreshold = 10f;
    [Tooltip("线性阻力系数（每秒衰减速度），越大停得越快")] public float releaseDragCoefficient = 50f;
    [Tooltip("满能量时的衰减持续时间")]public float releaseDragTime = 0.2f;
    [Tooltip("释放的最低能量限度，低于此将不会触发发射")]public float releaseThreshold = 0;
    [Tooltip("基础发射速度")]public float baseSpeed = 25f;
    [Tooltip("最大发射速度，能量满时初始速度最大")]public float maxSpeed = 35f;

    [Header("动力装置参数")]
    [Tooltip("释放冷却时间")] public float releaseCoolingLimit = 1f;
    public float explosionStorageThrehold; //储量爆炸阈值,比最大储量值小一点
    public float maxStorage; //最大储量值.允许玩家在过载状态多装一点能量，以便于卡住过载状态释放
    public float minReleaseThrehold = 5f;
    public float timeScaleReleasing = 0.1f;
    public float currentStorage;
    public float targetStorage;//与currentVelocityMagnitude基本同步，若模上升则上升，若模下降则停留一段时间，再下降
    public float currentVelocityMag;//角色的当前速度模
    public float targetStorageFreezeTime;//若角色的速度模小于上一帧，targetStorage停留一段时间再下降
    public float currentStorageFreezeTime;
    public float currentStorageDefaultIncreaseSpeed;//当前储量默认增长速度（常量）
    public float currentStorageIncreaseSpeed;//当前储量增长速度（变量，与速度模大小成正比）
    public float currentStorageDecreaseSpeed = 40f;
    public float explosionTime = 2f;
    public bool isOverloaded;
    public float storageScale = 1.2f;
    public float Kin_gravityContractionScale = 1f;

    [Header("道具参数")]
    public float coolerTimer;
    public IInteractable currentInteractable;
    public float postReleaseTimer;//后释放计时器
    public float strengthenerTimer;
    public float forceLimitTimer;

    [Header("实时变量")]
    public float InputX;
    public bool JumpInputDown;
    public bool ReleaseInputDown;
    public Vector2 MouseWorldPos;
    public Vector2 MouseDir;
    public float varJumpTimer;
    public float jumpBufferTimer;//跳跃缓冲计时器
    public float climbTimer;
    public float targetStorageFreezeTimer;//targetStorage停留计时器
    public float currentStorageFreezeTimer;//currentStorage停留计时器
    public float explosionTimer;//爆炸计时器
    public float releaseCoolingTimer; // 释放冷却
    public bool canJump; // 与isGrounded相关
    public Vector2 releaseDir;
    public GameObject arrowInstance;

    // 快捷属性绑定
    public bool isGrounded => groundedCheckerManager != null && groundedCheckerManager.isGrounded;

    public bool isOnLeftWall => canClimbLeftCheckerManager != null && canClimbLeftCheckerManager.canClimb;
    public bool isOnRightWall => canClimbRightCheckerManager != null && canClimbRightCheckerManager.canClimb;
    public bool canClimb => isOnLeftWall || isOnRightWall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
        Anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        StateMachine = new PlayerStateMachine();

        // 初始化所有状态
        IdleState = new IdleState(this, StateMachine, "Idle");
        RunState = new RunState(this, StateMachine, "Run");
        BrakeState = new BrakeState(this, StateMachine, "Brake");
        MidAirState = new MidAirState(this, StateMachine, "MidAir");
        ClimbState = new ClimbState(this, StateMachine, "Climb");
        ReleaseState = new ReleaseState(this, StateMachine, "Release");
        LaunchedState = new LaunchedState(this, StateMachine, "Launched");
        DieState = new DieState(this, StateMachine, "Die");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }
    float lastDir;
    private void Update()
    {
        //交互物函数，暂时不知道放在哪里先放这儿
        if (currentInteractable != null & Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("触发交互物函数");
            currentInteractable.Interact();
        }



        //处理输入:横向输入,跳跃输入,释放输入,鼠标输入
        // InputX = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            lastDir = -1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            lastDir = 1;
        //计算最终InputX
        float moveA = Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ? -1f : 0f;
        float moveD = Input.GetKey(KeyCode.D) | Input.GetKeyDown(KeyCode.RightArrow) ? 1f : 0f;
        if (moveA != 0 && moveD != 0)
            InputX = lastDir;//双向冲突，取最后按下的
        else
            InputX = moveA + moveD;//单向或无向，直接求和

        JumpInputDown = Input.GetKeyDown(KeyCode.Space);
        ReleaseInputDown = Input.GetKeyDown(KeyCode.LeftControl);
        MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MouseDir = ((Vector2)MouseWorldPos - (Vector2)this.transform.position).normalized;

        //启动输入缓冲(目前只有跳跃)
        if (JumpInputDown)
        {
            jumpBufferTimer = jumpBufferTime;
        }

        //处理计时器
        if (varJumpTimer > 0) varJumpTimer -= Time.deltaTime;
        if (jumpBufferTimer > 0) jumpBufferTimer -= Time.deltaTime;
        if (currentStorageFreezeTimer > 0) currentStorageFreezeTimer -= Time.deltaTime;
        if (targetStorageFreezeTimer > 0) targetStorageFreezeTimer -= Time.deltaTime;
        if (explosionTimer > 0) explosionTimer -= Time.deltaTime;
        if (jumpCoyoteTimer > 0) jumpCoyoteTimer -= Time.deltaTime;
        if (coolerTimer > 0) coolerTimer -= Time.deltaTime;
        if (strengthenerTimer > 0) strengthenerTimer -= Time.deltaTime;
        if (postReleaseTimer > 0) postReleaseTimer -= Time.deltaTime;
        if (releaseCoolingTimer > 0) releaseCoolingTimer -= Time.deltaTime;
        if (forceLimitTimer > 0) forceLimitTimer -= Time.deltaTime;

        //调用状态机内部更新：必须放在“处理其他状态之前”！
        StateMachine.CurrentState.HandleInput();
        StateMachine.CurrentState.LogicUpdate();

        //处理其它状态（中立于两个或两个以上状态）变量
        //if (isGrounded && canJump == false && varJumpTimer <= 0 && rb.velocity.y <= 0) canJump = true;//落地后可以再次跳跃
        if (!isGrounded && jumpCoyoteTimer <= 0) canJump = false;//离地且土狼时间结束后不能跳跃
        if (isGrounded) rb.gravityScale = defaultGravityScale;//落地后重置重力
        CheckFlip();//检查转向

        //这里解释一下，为什么“调用状态机内部更新”必须要放在“处理其它状态”的上面：因为在RunState & IdleState的脚本里走离平台的逻辑中加入了“开启土狼时间计时器”后，如果后者在前者的下面，canJump会先被设置成false，然后土狼计时器才启动，所以后者在前者前面的根本目的是保证土狼计时器先启动。我不清楚这里有没有更加合理和漂亮的解决方案，总之，所有与在状态机中触发的计时器和状态的变量相关的脚本，必须放在“调用状态机内部更新”之后

        //两小时之后：我发现上述问题会产生的根本原因是一个逻辑更新紧随于一个物理更新之后。虽然在游戏世界中大部分情况逻辑更新在物理更新之前，但是土狼时间的启动，以及release（我正好写到这里就发现）中必须在物理输出结束后（也就是实现了清空currentStorage之后，不然的话状态切换会先于物理输出）再进行状态切换，都属于物理更新先于逻辑更新的情况。这种问题似乎是不可避免的。
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }


    #region 中立于两个或两个以上状态的状态内函数
    // 补上状态类调用的 Brake 函数
    public void Brake()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            float forceX = -Mathf.Sign(rb.velocity.x) * turnRoundBrakeDec;
            rb.AddForce(new Vector2(forceX, 0));
        }
    }

    public void TurnRoundBrake(float amount)
    {
        // 只有在有水平速度时才执行刹车
        if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            // 算出反方向的力
            float forceX = -Mathf.Sign(rb.velocity.x) * amount;
            rb.AddForce(new Vector2(forceX, 0));
        }
        else
        {
            // 速度极小时直接归零，防止物理引擎的微小滑动
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void InitialJump()
    {

        //实现跳跃的物理功能
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeedInitial);

        //启动变量跳跃计时器
        varJumpTimer = varJumpTime;

        //关闭跳跃缓冲
        jumpBufferTimer = -1;

        //关闭canJump
        canJump = false;

        //关闭土狼时间计时器
        jumpCoyoteTimer = -1;

        //允许变量跳跃
        canVarJump = true;

    }

    //转向函数
    private void CheckFlip()
    {
        // 只有当玩家有水平输入时才检测翻转
        // 如果没有输入（InputX == 0），保留当前的朝向
        if (InputX > 0.01f && !isFacingRight)
        {
            Flip();
        }
        else if (InputX < -0.01f && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        // 切换布尔值
        isFacingRight = !isFacingRight;

        // 获取当前的缩放值
        Vector3 localScale = transform.localScale;
        // 将 X 轴缩放取反
        localScale.x *= -1;
        // 重新赋值回物体的 transform
        transform.localScale = localScale;
    }

    #endregion

    #region 道具、交互物函数
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果碰到道具
        IPickUp thisPickUp = collision.GetComponent<IPickUp>();
        if (thisPickUp != null)
        {
            Debug.Log("触发道具函数");
            thisPickUp.PickUpEffect(this);
        }

        //如果碰到交互物
        IInteractable thisInteractable = collision.GetComponent<IInteractable>();
        if (thisInteractable != null)
        {
            //视觉上显示可以按E，这里还没写

            currentInteractable = thisInteractable;
            if (currentInteractable != null & Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("触发交互物函数");
                thisInteractable.Interact();
            }
        }

        //如果碰到地标
        ILandmark thisLandmark = collision.GetComponent<ILandmark>();
        if (thisLandmark != null)
        {
            Debug.Log("触发地标函数");
            thisLandmark.LandmarkEffect();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        currentInteractable = null;//玩家离开交互物将当前交互物设为null
    }

    #endregion

    #region 地形函数
    public void ApplyForce(IPlayerForce forceSource, float forceLimitTime)
    {
        Vector2 newVelocity = forceSource.CalculateVelocity(rb.velocity, transform.position);

        rb.velocity = newVelocity;
        
        forceLimitTimer = forceLimitTime;
    }



    #endregion

    # region 动画函数
    public void PlayAnimation(string name)
    {
        //if (Anim != null && !string.IsNullOrEmpty(name)) Anim.Play(name);
    }
    #endregion
}