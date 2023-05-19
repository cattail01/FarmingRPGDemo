using Enums;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    [Header("NPC Movement")] public float NpcNormalSpeed = 2f;
    [SerializeField] private float npcMinSpeed = 1.0f;
    [SerializeField] private float npcMaxSpeed = 3.0f;

    [Header("NPC Animation")] [SerializeField]
    private AnimationClip blankAnimation = null;

    [HideInInspector] public SceneName NpcCurrentScene;
    [HideInInspector] public SceneName NpcTargetScene;
    [HideInInspector] public Vector3Int NpcCurrentGridPosition;
    [HideInInspector] public Vector3Int NpcTargetGridPosition;
    [HideInInspector] public Vector3 NpcTargetWorldPosition;
    [HideInInspector] public Direction NpcFacingDirectionAtDestination;
    [HideInInspector] public AnimationClip NpcTargetAnimationClip;
    [HideInInspector] public bool npcActiveInScene = false;

    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;
    private bool npcIsMoving = false;
    private Grid grid;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;
    private int lastMoveAnimationParameter;
    private NPCPath npcPath;
    private bool npcInitialised = false;
    private SpriteRenderer spriteRenderer;
    private bool sceneLoaded = false;
    private Coroutine moveToGridPositionRoutine;

    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }

    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleRight, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleDown, false);
    }

    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.WalkRight, false);
        animator.SetBool(Settings.WalkLeft, false);
        animator.SetBool(Settings.WalkUp, false);
        animator.SetBool(Settings.WalkDown, false);
    }

    private Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        return new Vector3(worldPosition.x + Settings.GridCellSize / 2f, worldPosition.y + Settings.GridCellSize / 2f,
            worldPosition.z);
    }

    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SetMovementAnimation(Vector3Int gridPosition)
    {
        ResetIdleAnimation();
        ResetMoveAnimation();

        Vector3 toWorldPosition = GetWorldPosition(gridPosition);
        Vector3 directionVector = toWorldPosition - transform.position;

        if (Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            if (directionVector.x > 0)
            {
                animator.SetBool(Settings.WalkRight, true);
            }
            else
            {
                animator.SetBool(Settings.WalkLeft, true);
            }
        }
        else
        {
            if (directionVector.y > 0)
            {
                animator.SetBool(Settings.WalkUp, true);
            }
            else
            {
                animator.SetBool(Settings.WalkDown, true);
            }
        }
    }

    public void SetNPCActiveInScene()
    {
        spriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        switch (NpcFacingDirectionAtDestination)
        {
            case Direction.Up:
                animator.SetBool(Settings.idleUp, true);
                break;
            case Direction.Down:
                animator.SetBool(Settings.idleDown, true);
                break;
            case Direction.Left:
                animator.SetBool(Settings.idleLeft, true);
                break;
            case Direction.Right:
                animator.SetBool(Settings.idleRight, true);
                break;
            case Direction.None:
                break;
            default:
                break;

        }
    }

    public void ClearNPCEventAnimation()
    {
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.EventAnimation, false);

        transform.rotation = Quaternion.identity;
    }

    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime,
        TimeSpan gameTime)
    {
        npcIsMoving = true;

        SetMovementAnimation(gridPosition);
        npcNextWorldPosition = GetWorldPosition(gridPosition);

        if (npcMovementStepTime > gameTime)
        {
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);
            //float npcCalculatedSpeed = Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove /
            //                           Settings.SecondsPerGameSecond;
            float npcCalculatedSpeed = Mathf.Max(npcMinSpeed,
                Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove /
                Settings.SecondsPerGameSecond);

            //if (npcCalculatedSpeed >= npcMinSpeed && npcCalculatedSpeed <= npcMaxSpeed)
            if(npcCalculatedSpeed <= npcMaxSpeed)
            {
                while (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.PixelSize)
                {
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime,
                        unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);
                    rigidbody2D.MovePosition(rigidbody2D.position + move);
                    yield return waitForFixedUpdate;
                }
            }

        }

        rigidbody2D.position = npcNextWorldPosition;
        NpcCurrentGridPosition = gridPosition;
        npcNextGridPosition = NpcCurrentGridPosition;
        npcIsMoving = false;
    }

    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine =
            StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    private void InitialiseNPC()
    {
        if (NpcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }

        else
        {
            SetNPCInactiveInScene();
        }

        npcPreviousMovementStepScene = NpcCurrentScene;

        NpcCurrentGridPosition = GetGridPosition(transform.position);
        npcNextGridPosition = NpcCurrentGridPosition;
        NpcTargetGridPosition = NpcCurrentGridPosition;
        NpcTargetWorldPosition = GetWorldPosition(NpcTargetGridPosition);

        npcNextWorldPosition = GetWorldPosition(NpcCurrentGridPosition);
    }

    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    private void AfterSceneLoad()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        if (!npcInitialised)
        {
            InitialiseNPC();
        }

        sceneLoaded = true;
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        NpcTargetScene = npcScheduleEvent.ToSceneName;
        NpcTargetGridPosition = (Vector3Int)npcScheduleEvent.ToGridCoordinate;
        NpcTargetWorldPosition = GetWorldPosition(NpcTargetGridPosition);
        NpcFacingDirectionAtDestination = npcScheduleEvent.NpcFacingDirectionAtDestination;
        NpcTargetAnimationClip = npcScheduleEvent.AnimationAtDestination;
        ClearNPCEventAnimation();
    }

    private void SetNPCEventAnimation()
    {
        if (NpcTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            animatorOverrideController[blankAnimation] = NpcTargetAnimationClip;
            animator.SetBool(Settings.EventAnimation, true);
        }
        else
        {
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.EventAnimation, false);
        }
    }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        NpcTargetScene = NpcCurrentScene;
        NpcTargetGridPosition = NpcCurrentGridPosition;
        NpcTargetWorldPosition = transform.position;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            if (npcIsMoving == false)
            {
                NpcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = NpcCurrentGridPosition;

                if (npcPath.NpcMovementStepStack.Count > 0)
                {
                    NPCMovementStep npcMovementStep = npcPath.NpcMovementStepStack.Peek();
                    NpcCurrentScene = npcMovementStep.SceneName;

                    if (NpcCurrentScene != npcPreviousMovementStepScene)
                    {
                        NpcCurrentGridPosition = (Vector3Int)npcMovementStep.GridCoordinate;
                        npcNextGridPosition = NpcCurrentGridPosition;
                        transform.position = GetWorldPosition(NpcCurrentGridPosition);
                        npcPreviousMovementStepScene = NpcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                    }

                    if (NpcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();
                        npcMovementStep = npcPath.NpcMovementStepStack.Pop();
                        npcNextGridPosition = (Vector3Int)npcMovementStep.GridCoordinate;
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.Hour, npcMovementStep.Minute,
                            npcMovementStep.Second);
                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime,
                            TimeManager.Instance.GetGameTime());
                    }

                    else
                    {
                        SetNPCInactiveInScene();

                        NpcCurrentGridPosition = (Vector3Int)npcMovementStep.GridCoordinate;
                        npcNextGridPosition = NpcCurrentGridPosition;
                        transform.position = GetWorldPosition(NpcCurrentGridPosition);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.Hour, npcMovementStep.Minute,
                            npcMovementStep.Second);
                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if (npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = npcPath.NpcMovementStepStack.Pop();
                            NpcCurrentGridPosition = (Vector3Int)npcMovementStep.GridCoordinate;
                            npcNextGridPosition = NpcCurrentGridPosition;
                            transform.position = GetGridPosition(NpcCurrentGridPosition);
                        }
                    }
                }

                else
                {
                    ResetMoveAnimation();
                    SetNPCFacingDirection();
                    SetNPCEventAnimation();
                }
            }
        }
    }

    public void CancelNPCMovement()
    {
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3.zero;
        npcIsMoving = false;
        if (moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        ResetMoveAnimation();
        ClearNPCEventAnimation();
        NpcTargetAnimationClip = null;
        ResetIdleAnimation();
    }
}
