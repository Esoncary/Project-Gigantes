using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerWasted : MonoBehaviour
{
    #region 状态，常量及组件
    #region state
    public enum PlayerState // 将枚举的访问修饰符改为 public，以匹配字段的可访问性
    {
        idle,
        run,
        midair,//在半空中/脚没有落地的状态
        release,//玩家releasing时的状态
        climb,//攀爬状态
        die,
        overspeed
    }
    public PlayerState currentPlayerState;
    #endregion

    #region component
    Rigidbody2D rb;
    Collider2D col;
    CanClimbLeftCheckerManager canClimbLeftCheckerManager;
    CanClimbRightCheckerManager canClimbRightCheckerManager;
    GroundedCheckerManager groundedCheckerManager;
    
    #endregion

    #region const
    //平面移动
    public float moveSpeedAcc;//玩家地面输入移动时会获得的加速度
    public float maxMoveSpeed;//跑动的最大移速和最小移速（最小移速防止玩家启动过慢）
    public float minMoveSpeed;
    public float brakeDeceleraion;//玩家无输入时的刹车速度
    public float enforceDeceleraion;//玩家通过被撞击或者release使移速超过最高速度时的强制减速
    //空中移动
    public float moveSpeedAccInMidAir;//玩家空中输入时会获得的加速度
    public float maxMoveSpeedInMidAir;
    public float minMoveSpeedInMidAir;
    public float enforceMoveAccSpeedInMidAir;
    //跳跃
    public float jumpBufferTime;//跳跃缓冲时间
    public float jumpSpeedInitial;//跳跃的初始速度，用于摆脱重力
    public float jumpSpeedAcc;//跳跃加速度
    public float minJumpSpeed;//跳跃的最小速度
    public float maxJumpSpeed;//跳跃的最大速度
    public float varJumpTime;//按住跳跃键后跳跃持续加速的最大时间
    public float varJumpTimer;//跳跃加速计时器
    public float gravityContractionThreshold;//跳跃中小于这个速度时重力缩小，以达到滞空效果
    public float gravityContractionScale;//重力缩小的比例
    public float gravityContractionTime;//重力缩小的最大窗口时间
    public float gravityContractionTimer;//重力缩小计时器
    //攀爬
    public float climbTime;//最大攀爬时间
    public float climbBufferTime;//攀爬缓冲时间
    public float wallJumpSpeed;
    public Vector2 wallJumpDirection;
    public float inputLockTime;//在wallJUmp的一小段时间后，阻止玩家输入
    //向量装置
    public float maxStorage;//装置的最大容量，最小容量和当前容量
    public float minStorage;
    public float targetStorage;//装置的目标容量
    public float targetStorageUpdateTime;//目标值的更新时间
    public float targetStorageUpdateTimer;//更新时间计时器
    public float explosionTime;//storage超出最大值后多少s装置爆炸
    public float minIncreaseThreshold;//装置的storage增长的阈值以及对应的增长速度
    public float midIncreaseThreshold;
    public float maxIncreaseThreshold;
    public float minIncreaseSpeed;
    public float midIncreaseSpeed;
    public float maxIncreaseSpeed;
    public float decreaseSpeed;//装置减少向量的速度
    public float decreaseTime;//还有多少s就要开始减少
    public float decreaseTimer;//计时器
    public float minReleaseThrehold;//最小可release的storage值
    public float releaseCooldown;//release开关的冷却时间
    public float timeScaleReleasing;//release时的时间缩放比例

    #endregion

    #endregion

    #region anim
    public Animator Anim;

    #endregion

    #region 变量
    //移动
    public bool isGrounded;
    public Vector2 moveDirection;

    //跳跃
    public float jumpBufferTimer;//跳跃缓冲计时器
    public float jumpPressedTime;//跳跃按下时间
    public bool isJumpBuffered;//跳跃是否在缓冲中
    public bool canJump;
    public Vector2 jumpDirection;

    //攀爬
    public float climbTimer;//攀爬时间计时器
    public float climbBufferTimer;
    public bool isClimbBuffered;
    public bool canClimb;
    public bool isOnLeftWall;
    public bool isOnRightWall;
    public float inputLockTimer;

    //向量装置
    public float currentVelocity;//当前角色向量
    public float currentStorage;//当前装置储量
    public Vector2 releaseDirection;//release方向
    public bool isOverLoaded;
    public bool canRelease;
    public bool willDecrease;//是否将要减少储量
    public bool canDecrease;//开始减少储量
    [Header("Release Visuals")]
    public GameObject arrowPrefab; // 一个简单的箭头图片，作为角色的子物体
    public GameObject arrowInstance;
    private Vector2 preReleaseVelocity;


    #endregion

    //初始化
    private void Awake()
    {
        //设定角色状态
        currentPlayerState = PlayerState.idle;

        //初始化角色位置，等有了prefab再说
        

        //获取组件
        rb = GetComponent< Rigidbody2D > ();
        col = GetComponent<Collider2D>();
        canClimbLeftCheckerManager = GetComponentInChildren<CanClimbLeftCheckerManager>();
        canClimbRightCheckerManager = GetComponentInChildren<CanClimbRightCheckerManager>();
        groundedCheckerManager = GetComponentInChildren<GroundedCheckerManager>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //初始化重力装置
        currentStorage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        #region 变量检测和状态转换

        //状态变量监测和状态转换
        isGrounded = groundedCheckerManager.isGrounded;
        if(canClimbLeftCheckerManager.canClimb == true)
        {
            isOnLeftWall = true;
            canClimb = true;
        }
        else if (canClimbRightCheckerManager.canClimb == true)
        {
            isOnRightWall = true;
            canClimb = true ;
        }
        else
        {
            isOnLeftWall = false; 
            isOnRightWall = false;
            canClimb = false;
        }

        if (!isGrounded && currentPlayerState != PlayerState.climb && currentPlayerState != PlayerState.release)
        {
            SetCurrentPlayerState(PlayerState.midair);
        }

        //输入监测和状态转换（我的逻辑在这里自相矛盾了，我的run的输入写在run函数里）
        if (currentPlayerState == PlayerState.midair && Input.GetKeyDown(KeyCode.J))
        {
            isClimbBuffered = true;
            climbBufferTimer = climbBufferTime;//启动/重置攀爬缓冲计时器
            
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
                isJumpBuffered = true;
                jumpBufferTimer = jumpBufferTime;//启动/重置跳跃缓冲计时器
                jumpPressedTime = Time.time;//标记按下时间

        }
        //SwitchCurrentPlayerStateInUpdate();
        else if (isGrounded && Input.GetAxisRaw("Horizontal") != 0 && rb.velocity.y <= 0.1f)
        {
            SetCurrentPlayerState(PlayerState.run);
        }
        else if (isGrounded && rb.velocity.x <= 0)
        {
            SetCurrentPlayerState(PlayerState.idle);
        }
        //补充climb,die,release等等
        #endregion

        #region 跳跃和攀爬控制
        //跳跃控制
        if (varJumpTimer > 0)
        {
            varJumpTimer -= Time.deltaTime; // 每一帧减去 0.016秒(假设60fps)
        }
        if (jumpBufferTimer > 0)
        {
            jumpBufferTimer -= Time.deltaTime;
            if (jumpBufferTimer > 0)
            {
                isJumpBuffered = true;
            }
            else
            {
                isJumpBuffered = false;
            }
        }
        //攀爬控制
        if (inputLockTimer > 0)
        {
            inputLockTimer -= Time.deltaTime;
        }
        if (climbTimer > 0)
        {
            climbTimer -= Time.deltaTime;
            if (climbTimer <= 0)
            {
                ExitClimb();
                SetCurrentPlayerState(PlayerState.midair);
            }
        }
        if (climbBufferTimer > 0)
        {
            climbBufferTimer -= Time.deltaTime;
            if (climbBufferTimer > 0)
            {
                isClimbBuffered = true;
            }
            else
            {
                isClimbBuffered = false;
            }
        }

        //在地面检测
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            canJump = true;
        }
        #endregion

        #region 重力装置控制
        if (rb.velocity.magnitude >= targetStorage)//如果目标储量小于当前向量模，立马将当前向量模设定为目标储量，并且关闭启动计时器
        {
            targetStorage = rb.velocity.magnitude;//设定

            decreaseTimer = -1F;//关闭
            willDecrease = false;
        }

        if (rb.velocity.magnitude < targetStorage)//如果目标储量大于当前向量模，启动减少计时器
        {
            targetStorage = rb.velocity.magnitude;//设定  

            willDecrease = true;
            decreaseTimer = decreaseTime;

        }
        

        if (willDecrease)//将要减少开始计时
        {
                decreaseTimer -= Time.deltaTime;
            if (decreaseTimer <= 0)
            {
                willDecrease = false;
                canDecrease = true;
            }
        }

        if (canDecrease)
        {
       
            currentStorage = Mathf.MoveTowards(currentStorage, targetStorage, decreaseSpeed * Time.deltaTime);

            if (currentStorage <= targetStorage)
            {
                canDecrease = false;
            }
        }

        if (targetStorage <= minIncreaseThreshold && targetStorage > 0)
        {
            currentStorage = Mathf.MoveTowards(currentStorage, targetStorage, minIncreaseSpeed * Time.deltaTime);
        }
        else if (targetStorage <= midIncreaseThreshold && targetStorage > minIncreaseThreshold)
        {
            currentStorage = Mathf.MoveTowards(currentStorage, targetStorage, midIncreaseSpeed * Time.deltaTime);
        }
        else if (targetStorage <= maxIncreaseThreshold && targetStorage > midIncreaseThreshold)
        {
            currentStorage = Mathf.MoveTowards(currentStorage, targetStorage, maxIncreaseSpeed * Time.deltaTime);
        }


        // 检测释放键 (假设是 K 键或鼠标右键)
        if (Input.GetKeyDown(KeyCode.LeftControl) && currentStorage >= minReleaseThrehold && canRelease)
        {
            StartRelease();
        }

        if (currentPlayerState == PlayerState.release)
        {
            UpdateAimingDirection(); // 实时更新箭头指向

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                ExecuteRelease();
            }
        }



        #endregion

    }

    private void FixedUpdate()
    {
        #region 状态switch
        //根据状态执行不同的函数
        switch (currentPlayerState)
        {
            case PlayerState.idle:

                //起跳
                if (isJumpBuffered && canJump)
                {
                    isGrounded = false;
                    canJump = false;
                    jumpBufferTimer = -1F;
                    isJumpBuffered = false;

                    varJumpTimer = varJumpTime;//启动变量跳跃计时器
                    InitialJump();

                    SetCurrentPlayerState(PlayerState.midair);
                }

                break;
            case PlayerState.run:
                Run();

                if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    Brake();
                }

                //起跳
                if (isJumpBuffered && canJump)
                {
                    isGrounded = false;
                    canJump = false;
                    jumpBufferTimer = -1F;
                    isJumpBuffered = false;

                    varJumpTimer = varJumpTime;//启动变量跳跃计时器
                    InitialJump();

                    SetCurrentPlayerState(PlayerState.midair);
                }

                
                break;
            case PlayerState.midair:
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    MoveInMidAir();
                }
                

                
                //跳跃后续加速和重力缩减
                if (Input.GetKey(KeyCode.Space) && varJumpTimer > 0)
                {
                    Jump();
                }
                if (Input.GetKey(KeyCode.Space) && rb.velocity.magnitude < gravityContractionThreshold && currentPlayerState == PlayerState.midair)
                {
                    EnableGravityContraction();
                }
                else
                {
                    DisableGravityContraction();
                }

                //攀爬输入
               if (isClimbBuffered && canClimb)
               {
                    climbTimer = climbTime;
                    canClimb = false;
                    climbBufferTimer = -1f;
                    isClimbBuffered = false;

                    SnapToWall();
                    Climb();
                    SetCurrentPlayerState(PlayerState.climb);
               }
                    break;
            case PlayerState.climb:
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0; // 同时也清空角速度
                rb.gravityScale = 0;
                if (isJumpBuffered)
                {
                    isGrounded = false;
                    canJump = false;
                    jumpBufferTimer = -1F;
                    isJumpBuffered = false;

                    varJumpTimer = varJumpTime;//启动变量跳跃计时器
                    WallJump();
                    SetCurrentPlayerState(PlayerState.midair);
                    inputLockTimer = inputLockTime;

                }
                break;
            case PlayerState.release:
                
                break;
            case PlayerState.die:
                
                break;
            default:
                break;
        }
        #endregion
    }

    //动画控制函数
    // 动画网关函数
    public void PlayAnimation(string name)
    {
        // 如果你还没做动画，或者没挂 Animator 组件，这里就直接跳过
        if (Anim == null || string.IsNullOrEmpty(name)) return;

        Anim.Play(name);
    }

    #region 状态切换函数
    private void SwitchCurrentPlayerStateInUpdate()//只包括idle和midair的转换，run,die，climb和release在它们各自的触发函数里
    {
        
        if (isGrounded && rb.velocity.magnitude == 0)
        {
            SetCurrentPlayerState(PlayerState.idle);
        }
        else if (!isGrounded && currentPlayerState !=PlayerState.climb && currentPlayerState != PlayerState.die && currentPlayerState != PlayerState.release)//这里有什么简便写法
        {
            SetCurrentPlayerState(PlayerState.midair);
        }
    }
    public void SetCurrentPlayerState(PlayerState stateToSet)
    {
        currentPlayerState = stateToSet;
    }
    #endregion


    #region 基础移动函数

        #region run或者idle状态下函数
    //有输入时的跑动函数
    void Run()
    {
        //设定moveDirection
        moveDirection.x = Input.GetAxisRaw("Horizontal");

        //跑动
        if (Mathf.Sign(rb.velocity.x) != Input.GetAxisRaw("Horizontal"))//先检测是否在转向，以便于施加一个更大的转向速度
        {
            Brake();
        }
        if (Mathf.Abs(rb.velocity.x)< maxMoveSpeed)
        {
            if (Mathf.Abs(rb.velocity.x) < minMoveSpeed)
            {
                rb.velocity = new Vector2(moveDirection.x * minMoveSpeed, rb.velocity.y);
            }
            rb.AddForce(new Vector2(moveDirection.x * moveSpeedAcc, 0));

        }
        else if (Mathf.Abs(rb.velocity.x) >= maxMoveSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
        }
    }
    //跑动中且未输入或者转向时调用的刹车函数
    void Brake()
    {
        rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * brakeDeceleraion, 0));
    }
    //超过最大moveSpeed的强制减速函数
    void EnforceDeceleration()
    {
        if (rb.velocity.magnitude > maxMoveSpeed)
        {
            rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * enforceDeceleraion, 0));
        }
    }



    #endregion

        #region midair状态下(或run中的跳跃)函数
    void MoveInMidAir()
    {
        if (inputLockTimer > 0) return;
        //设定moveDirection
        moveDirection.x = Input.GetAxisRaw("Horizontal");

        //空中移动
        if (Mathf.Sign(rb.velocity.x) != Input.GetAxisRaw("Horizontal"))//先检测是否在转向，以便于施加一个更大的转向速度
        {
            Brake();
        }
        if (rb.velocity.x < maxMoveSpeedInMidAir)//朝同方向移动
        {
            if (rb.velocity.x < minMoveSpeed)
            {
                rb.velocity = new Vector2( moveDirection.x * minMoveSpeedInMidAir, rb.velocity.x);
            }
            rb.AddForce(new Vector2(moveDirection.x * moveSpeedAccInMidAir, 0));

        }
        else if (Mathf.Abs(rb.velocity.x) >= maxMoveSpeedInMidAir)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeedInMidAir, rb.velocity.y);
            //转向逻辑没写
        }
    }
    void InitialJump()
    {
        //rb.AddForce(Vector2.up * jumpSpeedInitial, ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeedInitial);
    }
    
    void Jump()
    {
        //基本跳跃逻辑
        if ( varJumpTimer > 0)
        {
            if (rb.velocity.y < maxJumpSpeed)
            {
                rb.AddForce(new Vector2(0, jumpSpeedAcc));
            }
            else if (rb.velocity.y >= maxJumpSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
            }
        }
        
    }
    void EnableGravityContraction()
    {
        rb.gravityScale = gravityContractionScale;
    }
    void DisableGravityContraction()
    {
            rb.gravityScale = 1;
    }


    #endregion

        #region climb状态下的函数
    void Climb()
    {
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
    }
    void SnapToWall()
    {
        rb.velocity = Vector2.zero; // 先停下

        float direction = isOnRightWall ? 1f : -1f;
        Vector2 wallDir = new Vector2(direction, 0);

        // --- 核心修正点：起点偏移 ---
        // 让射线起点从中心稍微往回退一点点（0.1f），确保它能射中墙的表面
        Vector2 rayOrigin = rb.position - (wallDir * 0.1f);
        float rayLength = col.bounds.extents.x + 0.5f;

        // 使用 Raycast 获取精确的接触点
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, wallDir, rayLength, groundedCheckerManager.Ground);

        // 调试线：在 Scene 视图里你能看到绿线就是射线
        Debug.DrawRay(rayOrigin, wallDir * rayLength, Color.green, 2f);

        if (hit.collider != null)
        {
            float colliderHalfWidth = col.bounds.extents.x;
            float physicsSkin = 0.01f; // 预留极小空隙防止物理挤压

            // 计算目标 X
            float targetX = hit.point.x - (colliderHalfWidth * direction) + (physicsSkin * direction);

            // 强制位置同步 (注意：使用 Vector2)
            rb.position = new Vector2(targetX, rb.position.y);
            rb.velocity = Vector2.zero;

            Debug.Log($"[Snap成功] 命中点:{hit.point.x} 目标X:{targetX}");
        }
        else
        {
            // 如果失败，通常是 LayerMask 没选对或者射线太短
            Debug.LogWarning($"[Snap失败] 方向:{direction} 起点:{rayOrigin} 请检查物体的 Layer 是否为 Ground");
        }
    }
    void ExitClimb()
    {
        rb.gravityScale = 1;
    }
    void WallJump()
    {
        if (isOnRightWall)
        {
            rb.velocity = new Vector2(-1, 1).normalized * wallJumpSpeed;
        }
        else if (isOnLeftWall)
        {
            rb.velocity = new Vector2(1, 1).normalized * wallJumpSpeed;
        }
        
    }
    #endregion
    #endregion


    #region 重力储存与释放函数
    void StartRelease()
    {
        SetCurrentPlayerState(PlayerState.release);

        // 1. 开启子弹时间
        Time.timeScale = timeScaleReleasing;
        // 关键：必须同步调整物理帧率，否则慢动作下会抖动
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 2. 物理凝固（类似攀爬，防止在选方向时掉下去）
        preReleaseVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        // 3. 显示 UI 箭头
        if (arrowInstance != null) arrowInstance.SetActive(true);
    }

    void UpdateAimingDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 只要有输入，就更新方向
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            releaseDirection = new Vector2(h, v).normalized;
        }

        // 让 UI 箭头指向这个方向
        if (arrowInstance != null)
        {
            float angle = Mathf.Atan2(releaseDirection.y, releaseDirection.x) * Mathf.Rad2Deg;
            float offset = -90f; // 如果指向右则为0，指向上则为-90
            arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle + offset);
        }
    }

    void ExecuteRelease()
    {
        // 1. 恢复时间
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        // 2. 物理爆发
        rb.gravityScale = 1.0f;
        // 核心公式：新速度 = 方向 * 储量
        rb.velocity = releaseDirection * currentStorage;

        // 3. 资源消耗
        currentStorage = 0;
        targetStorage = 0; // 同时也清空目标值，防止瞬间回涨

        // 4. 状态切换
        SetCurrentPlayerState(PlayerState.midair);
        if (arrowInstance != null) arrowInstance.SetActive(false);

        // 5. 设置冷却
        // canRelease = false;
        // StartCoroutine(ReleaseCooldownRoutine());
    }




    #endregion


}

