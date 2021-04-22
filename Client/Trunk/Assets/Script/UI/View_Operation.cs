using Fight;
using FightCommom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class View_Operation : MonoBehaviour
{
    public ETCJoystick joystick;
    public Button skill_0; 
    public Button skill_1;
    public Button skill_2;
    public Button skill_3;

    public Button btnMove;
    public Button btnStopMove;
    public Button btnChangeDir;
    public float speed = 2f;

    public float rotationSpeed = 2;

    public GameObject target;

    private GameObject objSkillCircle;
    private int targetNodeId;

    List<FightRoleRender> listTeamRole = new List<FightRoleRender>();

    FightRoleRender[,] teamArray;

    private bool moving = false;

    private IntVector2 teamSize = new IntVector2(2,5);

    public Vector3 dir3;
    public Transform objHeadBox;
    // Start is called before the first frame update
    void Start()
    {
        this.skill_0.onClick.AddListener(OnClickSkill_0);
        this.skill_1.onClick.AddListener(OnClickSkill_1);
        this.skill_2.onClick.AddListener(OnClickSkill_2);
        this.skill_3.onClick.AddListener(OnClickSkill_3);

        this.btnMove.onClick.AddListener(OnClickMove);
        this.btnStopMove.onClick.AddListener(OnClickStopMove);
        this.btnChangeDir.onClick.AddListener(OnClickChangeDir);
        
    }

    private void OnEnable()
    {
        joystick.onMove.AddListener(OnJoystickMove);
        joystick.onMoveEnd.AddListener(OnJoystickEnd);
    }


    private void OnDisable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (this.objSkillCircle == null)
        {
            this.objSkillCircle = GameObject.Find("Home/SkillCircle");
        }
        else if(CameraController.Instance.followTarget != null && MainPlayer.instannce != null)
        {
            this.objSkillCircle.transform.position = 
                CameraController.Instance.followTarget.position + Vector3.one*0.1f;

            float diameter = MainPlayer.instannce.range*2 * 0.1f;
            this.objSkillCircle.transform.localScale = new Vector3(diameter, diameter, diameter);
        }

      //  FollowTarget();


    }

    private Vector3 moveDir;
    void OnJoystickMove(Vector2 dir)
    {
        dir.Normalize();
        dir3 = new Vector3(dir.x, 0f, dir.y);
        if (MainPlayer.instannce != null)
        {
            Vector3 worldPos = CameraController.Instance.followTarget.position;
            Vector3 endPosition = worldPos + dir3 * 3;

            Platform platform = FightSceneRender.Instance.battleFieldRender.platform;

            Node node = platform.PositionToNode(endPosition);
  

            if (targetNodeId != node.Id)
            {
                if (this.target == null)
                {
                    this.target = GameObject.Find("Home/target");
                }


                
                if (node != null)
                {
                    this.target.transform.position = node.pos;
                    targetNodeId = node.Id;
                }

                MainPlayer.instannce.MoveTo(node.Id);
            }
        }

        FollowTarget();


    }

    Transform roleTrans;
    Transform prevBody;
    Quaternion wantRotation;
    Quaternion dumpRotation;

    float minDistance = 5;
    float dist;
    float t;

    bool mFollow;
  
    bool bRoation;
    void FollowTarget()
    {
        if (!mFollow) return;

        
        for (int k = 0; k < teamSize.x; k++)
        {
            for (int i = 1; i < teamSize.y; i++)
            {
                roleTrans = teamArray[k, i].transform;
                prevBody = teamArray[k, i - 1].transform;

                dist = Vector3.Distance(prevBody.transform.position, roleTrans.transform.position);
                //限制最小距离
                t = Time.deltaTime * dist / minDistance * speed;
                Vector3 position = Vector3.Slerp(roleTrans.position, prevBody.position, t);
                position.y = 0;
                roleTrans.position = position;

                roleTrans.rotation = Quaternion.Slerp(roleTrans.rotation, prevBody.rotation, t);
            }
            //移动速度根据旋转角度，由小到大变化
        }

        float angle = DotToAngle(teamArray[0, 0].transform.forward, dir3);
        bool isLeft = Vector3.Cross(teamArray[0, 0].transform.forward, dir3).y < 0;

        if (objHeadBox == null)
        {
            GameObject go = new GameObject();
            objHeadBox = go.transform;
            go.transform.SetParent(teamArray[0, 0].transform.parent);
            //go.transform.forward = teamArray[0, 0].transform.forward;
        }
        

        Debug.Log("angle:"+ angle);



        
        if (isLeft == false)
        {
            //向左转向
            if (this.objHeadBox.name != "isLeft")
            {
                for (int k = 0; k < teamSize.x; k++)
                {
                    teamArray[k, 0].transform.SetParent(this.objHeadBox.transform.parent);
                }

                objHeadBox.position = teamArray[teamSize.x-1, 0].transform.position;
                objHeadBox.rotation = teamArray[teamSize.x-1, 0].transform.rotation;

                for (int k = 0; k < teamSize.x; k++)
                {
                    teamArray[k, 0].transform.SetParent(this.objHeadBox.transform);
                }
                this.objHeadBox.name = "isLeft";
            }
           
        }
        else
        {
            //向右转向
            if (this.objHeadBox.name == "isRight")
            {
                for (int k = 0; k < teamSize.x; k++)
                {
                    teamArray[k, 0].transform.SetParent(this.objHeadBox.transform.transform);
                }
                objHeadBox.position = teamArray[0, 0].transform.position;
                objHeadBox.rotation = teamArray[0, 0].transform.rotation;
                for (int k = 0; k < teamSize.x; k++)
                {
                    teamArray[k, 0].transform.SetParent(this.objHeadBox.transform);
                }
                this.objHeadBox.name = "isRight";
            }
        }
            

            wantRotation = Quaternion.LookRotation(dir3);
            dumpRotation = Quaternion.Lerp(objHeadBox.rotation, wantRotation, speed * Time.deltaTime);
            objHeadBox.transform.rotation = dumpRotation;
            Vector3 position1 = objHeadBox.transform.position + objHeadBox.transform.forward * speed * Time.deltaTime;
            position1.y = 0;
            objHeadBox.transform.position = position1;


        
        //else
        //{

        //    if (bRoation == true)
        //    {
        //        bRoation = false;
        //        //for (int k = 0; k < teamSize.x; k++)
        //        //{
        //        //    teamArray[k, 0].transform.SetParent(null);
        //        //}
        //    }

        //    wantRotation = Quaternion.LookRotation(dir3);
        //    dumpRotation = Quaternion.Lerp(objHeadBox.rotation, wantRotation, speed * Time.deltaTime);
        //    Vector3 dir = dumpRotation.eulerAngles.normalized;
        //    dir.y = 0;
        //    objHeadBox.transform.rotation = dumpRotation;
        //    objHeadBox.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        //}
       
        
    }

    void OnJoystickEnd()
    {
        if (MainPlayer.instannce != null)
            MainPlayer.instannce.StopMove();
        //targetNodeId = -1;
        this.bRoation = false;
    }


    void OnClickSkill_0()
    {
        //MainPlayer.instannce.Attack();
        CreateTeam();
    }

    void OnClickSkill_1()
    {
        MainPlayer.instannce.CastSkill(1);
        
    }

    void OnClickSkill_2()
    {
        MainPlayer.instannce.CastSkill(2);
        mFollow = false;
    }

    void OnClickSkill_3()
    {
        MainPlayer.instannce.CastSkill(3);
    }



    void OnClickMove()
    {
        mFollow = true;
        
    }
    void OnClickStopMove()
    {
        mFollow = false;

        float angle = DotToAngle(teamArray[0, 0].transform.forward, dir3);
        Debug.Log(angle);
    }
    void OnClickChangeDir()
    {
        this.moveDir = new Vector3(0, 0, -1);
        roleTrans = teamArray[0, 0].transform;
        for (int k = 1; k < teamSize.x; k++)
        {
            teamArray[k, 0].transform.SetParent(roleTrans);
        }

    }

    void CreateTeam()
    {
        teamArray = new FightRoleRender[(int)teamSize.x, (int)teamSize.y];
  
        Vector3 initPos = CameraController.Instance.followTarget.position;
        int space = 2;
        this.moveDir = -Vector3.forward;
        for (int i = 0; i < teamSize.x; i++)
        {
            for (int k = 0; k < teamSize.y; k++)
            {
                FightRoleRender roleRender = new FightRoleRender();
                roleRender.LoadNpc("NPC/npc_2001", RoleType.Fighter, initPos + new Vector3(i*space,0,k*space));
                roleRender.transform.LookAt(roleRender.transform.position + this.moveDir);
                teamArray[i, k] = roleRender;
                GameObject.Destroy(roleRender.gameObject.GetComponent<BoxCollider>());
            }
        }
    }

    /// <summary>
    /// 计算两向量之间的夹角
    /// </summary>
    /// <param name="_from"></param>
    /// <param name="_to"></param>
    /// <returns></returns>
    public static float DotToAngle(Vector3 _from, Vector3 _to)
    {
        float rad = 0;
        rad = Mathf.Acos(Vector3.Dot(_from.normalized, _to.normalized));
        return rad * Mathf.Rad2Deg;
    }

}
