using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hubris
{
    /// <summary>
    /// Base class for all AI-driven Npcs with associated stats
    /// </summary>
    public class Npc : LiveEntity
    {
        [SerializeField]
        private AIParams _aiParams = null;
        [SerializeField]
        private Animator _anim = null;
        [SerializeField]
        private NavMeshAgent _navAgent = null;
        [SerializeField]
        private LayerMask _mask; 	                // What other entities should this Npc be aware of?
        [SerializeField]
        private GameObject _targetObj = null;
        [SerializeField]
        private Vector3 _movePos = Vector3.zero;    // Where the Agent is trying to move

        private IBhvBranch _branch = BNpcIdle.Instance;

        private List<LiveEntity> _trackList = new List<LiveEntity>(), 
                                _removeList = new List<LiveEntity>();
        private float _timerMove = 0.0f;
        private float _timerChk = 0.0f;

        #region Npc Properties
        ///--------------------------------------------------------------------
        /// Npc Properties
        ///--------------------------------------------------------------------

        public IBhvBranch BhvBranch
        {
            get
            { return _branch; }
            protected set
            { _branch = value; }
        }

        public Animator Anim
        {
            get { return _anim; }
            protected set { _anim = value; }
        }

        public AIParams Params
        {
            get { return _aiParams; }
        }

        public NavMeshAgent NavAgent
        {
            get { return _navAgent; }
            protected set { _navAgent = value; }
        }

        public LayerMask EntMask
        {
            get { return _mask; }
        }

        public GameObject TargetObj
        {
            get { return _targetObj; }
            protected set { _targetObj = value; }
        }

        public Vector3 TargetPos
        {
            get { return _targetObj.transform.position; }
        }

        public Vector3 MovePos
        {
            get { return _movePos; }
            protected set { _movePos = value; }
        }

        public List<LiveEntity> TrackList
        {
            get { return _trackList; }
        }

        public List<LiveEntity> RemoveList
        {
            get { return _removeList; }
        }

        public float TimerMove
        {
            get { return _timerMove; }
            set { _timerMove = value; }
        }

        public float TimerCheck
        {
            get { return _timerChk; }
            set { _timerChk = value; }
        }
        #endregion Npc Properties

        ///--------------------------------------------------------------------
        /// Npc Methods
        ///--------------------------------------------------------------------

        void OnEnable()
        {
            if (NavAgent == null)
            {
                NavAgent = GetComponent<NavMeshAgent>();
                if (NavAgent == null)
                    NavAgent = this.gameObject.AddComponent<NavMeshAgent>();
            }

            if(Anim == null)
            {
                Anim = GetComponent<Animator>();
                if (Anim == null)
                    Debug.Log(this.gameObject.name + " doesn't have an Animator!");
            }

            EntType = LiveEntity.EType.ENEMY;
            Stats = EntStats.Create(EntType);

            // Temporary name assignment
            Name = this.gameObject.name;
        }

        public void ResetTimers()
        {
            TimerMove = 0.0f;
            TimerCheck = 0.0f;
        }

        public void UpdateTimers()
        {
            TimerMove += Time.deltaTime;
            TimerCheck += Time.deltaTime;
        }

        public void SetTargetObj(GameObject obj)
        {
            _targetObj = obj;
        }

        public void SetMovePos(Vector3 nPos)
        {
            _movePos = nPos;
        }

        public void MoveTo(Vector3 nPos)
        {
            MovePos = nPos;
            NavAgent.SetDestination(MovePos);
        }

        public void ChangeBranch(IBhvBranch b)
        {
            ResetTimers();
            BhvBranch = b;
        }

        void Start()
        {

        }

        void Update()
        {
            UpdateTimers();
        }

        void FixedUpdate()
        {
            BhvBranch.Invoke(this);
        }

        void LateUpdate()
        {
            if(AlignToSurface)
            {
                if(Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, NavAgent.height))
                {
                    this.transform.rotation = Quaternion.FromToRotation(this.transform.up, hit.normal);
                }
            }
        }
    }
}
