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
    public ClimbState ClimbState { get; private set; }
    public ReleaseState ReleaseState { get; private set; }

    // --- 组件引用 (根据报错，统一使用状态脚本里期待的名字) ---
    public Rigidbody2D rb { get; private set; } // 你要求的写小写
    // 如果报错说找不到 RB，请把上面的 rb 改成 RB，或者把状态脚本里的 RB 改成 rb
    // 鉴于你喜欢小写，我建议统一把状态脚本里的 RB 改成 rb，但为了现在能跑通，我先加个兼容：
    public Rigidbody2D RB => rb;

    public Animator Anim { get; private set; }
    public Collider2D col { get; private set; }

    [Header("检测器引用")]
    public GroundedCheckerManager groundedCheckerManager;
    public CanClimbLeftCheckerManager canClimbLeftCheckerManager;//攀爬装置暂且不用
    public CanClimbRightCheckerManager canClimbRightCheckerManager;

    [Header("移动参数")]
    public float moveSpeedAcc = 50f;
    public float maxMoveSpeed = 8f;
    public float minMoveSpeed = 0.1f;
    public float brakeDeceleraion = 30f;
    public float enforceDeceleraion = 20f;

    [Header("空中参数")]
    public float moveSpeedAccInMidAir = 25f;
    public float maxMoveSpeedInMidAir = 8f;
    public float enforceMoveAccSpeedInMidAir = 15f;

    [Header("跳跃参数")]
    public float defaultGravityScale;
    public float jumpSpeedInitial = 12f;
    public float jumpSpeedAcc = 80f;
    public float maxJumpSpeed = 15f;
    public float varJumpTime = 0.2f;
    public float gravityContractionThreshold = 1f;
    public float gravityContractionScale = 0.5f;
    public float jumpBufferTime;

    [Header("攀爬参数")]
    public float climbTime = 2.0f;
    public float wallJumpSpeed = 15f;
    public Vector2 wallJumpDirection = new Vector2(1, 1);
    public float inputLockTime = 0.2f;

    [Header("动力装置相关")]
    public float maxStorage = 100f;
    public float minReleaseThrehold = 5f;
    public float timeScaleReleasing = 0.1f;
    public float currentStorage;
    public float targetStorage;
    public float decreaseSpeed = 40f;
    public float decreaseTime = 0.5f;
    public float minIncreaseSpeed = 50f;
    public float midIncreaseSpeed = 100f;
    public float maxIncreaseSpeed = 200f;
    public float minIncreaseThreshold = 10f;
    public float midIncreaseThreshold = 30f;
    public float maxIncreaseThreshold = 60f;
    public float explosionTime = 2f;
    public bool willDecrease;
    public bool canDecrease;
    public float decreaseTimer;
    public bool isOverLoaded;

    [Header("实时变量")]
    public float InputX;
    public bool JumpInputDown;
    public bool ReleaseInputDown;
    public float varJumpTimer;
    public float jumpBufferTimer;//跳跃缓冲时间
    public float climbTimer;
    public float inputLockTimer;
    public bool canJump; // 补上了这个变量
    public Vector2 releaseDirection;
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
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        //处理输入:横向输入,跳跃输入,释放输入（释放的功能还没实现）
        InputX = Input.GetAxisRaw("Horizontal");
        JumpInputDown = Input.GetKeyDown(KeyCode.Space);
        ReleaseInputDown = Input.GetKeyDown(KeyCode.LeftControl);

        //启动输入缓冲(目前只有跳跃)
        if (JumpInputDown)
        {
            jumpBufferTimer = jumpBufferTime;
        }

        //处理计时器
        if (inputLockTimer > 0) inputLockTimer -= Time.deltaTime;
        if (varJumpTimer > 0) varJumpTimer -= Time.deltaTime;
        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        //处理其它状态（中立于各种状态）变量
        if (isGrounded && canJump == false && varJumpTimer <= 0) canJump = true;//落地后可以再次跳跃
        if (isGrounded) rb.gravityScale = defaultGravityScale;//落地后重置重力

        //调用状态机内部更新
        StateMachine.CurrentState.HandleInput();
        StateMachine.CurrentState.LogicUpdate();
        
        
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    // 补上状态类调用的 Brake 函数
    public void Brake()
    {
        if (Mathf.Abs(rb.velocity.x) > 0.01f)
        {
            float forceX = -Mathf.Sign(rb.velocity.x) * brakeDeceleraion;
            rb.AddForce(new Vector2(forceX, 0));
        }
    }

    public void ApplyBrakeForce(float amount)
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

    }

    public void ApplyForce(IPlayerForce forceSource)
    {
        Vector2 newVelocity = forceSource.CalculateVelocity(rb.velocity);
        
        // 根据力的类型应用不同的处理方式
        switch (forceSource.ForceType)
        {
            // 内部力
            case ForceType.Internal:
                Debug.Log("触发内部力: " + newVelocity);
                rb.velocity = newVelocity;
                break;
            // 外部力
            case ForceType.External:
                Debug.Log("触发外部力: " + newVelocity);
                rb.velocity = newVelocity;
                break;
        }
    }

    public void PlayAnimation(string name)
    {
        if (Anim != null && !string.IsNullOrEmpty(name)) Anim.Play(name);
    }
}