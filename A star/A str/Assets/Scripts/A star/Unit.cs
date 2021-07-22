using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Unit : MonoBehaviour
{

    [Header("Atrributes")]
    public float speed = 5f;
    public float jumpThreshold;
    public float jumpHieght = 18f;
    public float jumpLength = 6f;
    public float unitHieght = 3f;
    public float stoppingDistance = 1.2f;
    
    [Header("Variables")]
    public Transform playerPos;
    public LayerMask groundLayer;
    public Transform groundcheck;
    public Text stateDisplay;
    
    Vector3[] path;
    public int targetIndex;
    bool isTraversing = false;
    Vector2 currentWayPoint;
    Vector3 playerLastPos;          
    Rigidbody2D rb;
    Vector2 dir;

    Vector3 destination;

    Vector2 velocity;
    bool canJump;

    enum MovementState {running, inAir, decending, findingPath}
    MovementState currentMovementState;

    bool findingSpace = false;
    public bool roofCheckAhead = true;
    Vector2 vacantSpace;

    FieldOfView fov;

    void Start()
    {
        //PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);
        if(playerPos == null)
            playerPos = transform.parent.GetComponent<EnemySpawner>().player;
        playerLastPos = playerPos.position;
        rb = transform.GetComponent<Rigidbody2D>();
        canJump = true;

        fov = GetComponent<FieldOfView>();

        // TravelTo(playerPos.position,"Through STart");
    }

    void Update()
    {
        if(Vector2.Distance(playerPos.position,transform.position)<stoppingDistance)
            targetIndex = path.Length;
        Traverse();

        if(Vector2.Distance(playerLastPos,playerPos.position)>2f && isGrounded() && playerPos.GetComponent<SquareMovement>().isGrounded() || Input.GetKeyDown(KeyCode.F)){

            playerLastPos = playerPos.position;
            destination = playerPos.position;

            float destinationHieght = playerPos.position.y - transform.position.y;


            if(IsRoof())
                FindSpace();
            else if(fov.IsSteep()){
                FindAlternatePath();
            }
            else{
                TravelTo(playerPos.position,"Called Normally");
            }

        }
        stateDisplay.text = currentMovementState.ToString();

    }

    public void OnPathFound(Vector3[] newPath, bool pathFound){
        if(pathFound){
            path = newPath;
            currentWayPoint = path[0];
            targetIndex = 0;
            isTraversing = true;
            currentMovementState = MovementState.running;
        }
    }

    void TravelTo(Vector2 des, string s){
        if((Vector2.Distance(transform.position,playerPos.position)<stoppingDistance)){
            print("Checked");
            return;
        }
        // print(s);
        currentMovementState = MovementState.findingPath;
        PathRequestManager.RequestPath(transform.position, des, OnPathFound, jumpThreshold);
    }

    void Traverse(){
        if(isTraversing){
            if (targetIndex >= path.Length){
                isTraversing = false;
                return;
            }
            if(currentMovementState != MovementState.findingPath)
                DecideMove();    

            if(findingSpace){
                if(IsRoof()){
                    vacantSpace = transform.position;
                } else{
                    if(Vector2.Distance(vacantSpace,(Vector2)transform.position)>=5f){
                        findingSpace = false;
                        roofCheckAhead = true;

                        if (fov.IsSteep())
                        {
                            FindAlternatePath();
                        }else{
                            TravelTo(playerPos.position,"Called after finding Space");
                        }
                    }
                }
            }

        }
    }


    void DecideMove()
    {
        destination = playerPos.position;
        Vector2 movePos = currentWayPoint;
        Vector2 currentPos = transform.position;
        float DestinationHieght =  currentWayPoint.y-currentPos.y;
        dir = movePos - currentPos;
        dir.x = (dir.x > 0) ? 1 : -1;
        dir.y = (dir.y > 0) ? 1 : -1;
        JumpCheck(movePos);

        ///Runs if the player was previously in air
        if (isGrounded() && currentMovementState == MovementState.inAir && canJump){
            currentMovementState = MovementState.running;
            // print("checked");
            if (IsRoof())
                FindSpace();
            else if (fov.IsSteep())
                FindAlternatePath();
            else
                TravelTo(playerPos.position, "Called On landing");
            return;
        }

        ///Running State
        else if(DestinationHieght<unitHieght && DestinationHieght>-1 && isGrounded()){
            if(currentMovementState == MovementState.inAir){
                canJump = true;
                currentMovementState = MovementState.running;
                if (IsRoof())
                    FindSpace();
                else if (fov.IsSteep())
                    FindAlternatePath();
                else
                    TravelTo(playerPos.position, "Called in Running");
                return;
            }
            if( Mathf.Abs(currentPos.x - movePos.x) < 1f ){
                targetIndex++;
                if(targetIndex<path.Length)
                    currentWayPoint = path[targetIndex];
            }
            if(rb!=null){
                currentMovementState = MovementState.running;
                transform.position = Vector2.MoveTowards(transform.position, movePos, speed * Time.deltaTime);
            }
        }

        ///Descending  State 
        else if (DestinationHieght <= -1f && isGrounded())
        {
            Descend(movePos, currentPos);

            if (currentPos.y < movePos.y)
            {
                print("decended");
            }

            if (currentPos.y < movePos.y && isGrounded())
            {
                if (IsRoof())
                    FindSpace();
                else if (fov.IsSteep())
                    FindAlternatePath();
                else
                    TravelTo(playerPos.position, "Called while Decending");
            }
            currentMovementState = MovementState.decending;
        }

        ///Jumping State 
        else if(DestinationHieght>unitHieght){
            
            if(isGrounded()){
                if(currentMovementState == MovementState.decending){
                    if (IsRoof())
                        FindSpace();
                    else if (fov.IsSteep())
                        FindAlternatePath();
                    else
                        TravelTo(playerPos.position, "Called On Jump");
                    return;
                }
                Jump(jumpHieght,jumpLength);
            }
            else
                canJump = true;
            if (currentPos.y >= movePos.y && isGrounded() )  {
                targetIndex++;
                print("increasing");
                if (targetIndex < path.Length)
                    currentWayPoint = path[targetIndex];
                movePos = currentWayPoint;
            }
        }


    }

    void Descend(Vector2 movePos, Vector2 currentPos){

        int Index = targetIndex;

        // for (int x = targetIndex; x < targetIndex + 2 && x < path.Length; x++){
        //     int nextIndex = Mathf.Clamp(x + 1, x+1, path.Length - 1);
        //     // float xDis = Mathf.Abs(path[x].x-path[nextIndex].x);
        //     if (Vector2.Distance(path[x], path[nextIndex]) <= 1.5f && x != nextIndex){
        //         print("SKipped");
        //         Index = nextIndex;
        //     }
        //     else{
        //         break;
        //     }
        // }

        targetIndex = Index;
        movePos = path[targetIndex];
        
        // rb.velocity = new Vector2()
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(movePos.x, currentPos.y), speed * Time.deltaTime);
    }

    void Jump(float jumpH,float xForce){
        if(canJump){
            float dis = Vector2.Distance(currentWayPoint,transform.position);
            velocity.x = xForce*dir.x;
            // if(!manual)
            // velocity.x = dir.x*((dis*0.2f) + jumpThreshold);
            velocity.y = jumpH;
            rb.velocity = velocity;
            canJump = false;
            currentMovementState = MovementState.inAir;
        }
    }

    void JumpCheck(Vector2 nodePos){
        bool jump = Physics2D.Raycast(new Vector2(groundcheck.position.x+(0.5f*dir.x), groundcheck.position.y), Vector2.down, 2f, groundLayer);
        Vector2 curPos = transform.position;
        if(!jump & isGrounded() && !IsRoof() && canJump && Vector2.Distance(curPos, nodePos) > 5f){
            float jumpHe = jumpHieght*0.8f;
            float jumpLe = jumpLength;
            if(Mathf.Abs(curPos.y-nodePos.y)>=2f && dir.y<=0){
                jumpHe = jumpThreshold;
                jumpLe *= 0.4f;
            }
            if(Mathf.Abs(curPos.x - nodePos.x) <= 4)
                jumpHe = jumpHieght;
            Jump(jumpHe,jumpLe);
        }
        else if(!jump & isGrounded() && !IsRoof()){

        }
    }

    bool isGrounded()
    {   
        bool g;
        g = Physics2D.OverlapCircle(groundcheck.position,0.4f,groundLayer);
        if(!g){
            currentMovementState = MovementState.inAir;
        }
        return g;
    }

    bool IsRoof(){
        float destinationHieght = destination.y - transform.position.y;
        dir = destination - transform.position;
        dir.x = (dir.x > 0) ? 1 : -1;
        if(destinationHieght > unitHieght){
            bool roof = false;;
            if(roofCheckAhead){
                roof = Physics2D.Raycast(new Vector2(transform.position.x+(4f*dir.x),transform.position.y), Vector2.up, unitHieght * 2, groundLayer);
                if(!roof && findingSpace){
                    roofCheckAhead = false;
                }
            }
            else
                roof = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, unitHieght * 2, groundLayer);

            return roof;
        }
        return false;
    }

    void FindSpace(){
        findingSpace = true;
        dir = playerPos.position - transform.position;
        dir.x = (dir.x>0) ? 1 : -1;
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position,Vector2.right, 200, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 200, groundLayer);

        float disRight = hitRight.distance;
        float disLeft = hitLeft.distance;
        
        Vector2 movePos = new Vector2(hitRight.point.x-2f,hitRight.point.y) ;
        if(dir.x<0){
            movePos = new Vector2(hitLeft.point.x + 2f, hitLeft.point.y);
        }

        TravelTo(movePos,"called in find space");

    }

    void FindAlternatePath(){
        destination = new Vector2(Grid.Instance.platforms[0].transform.position.x,Grid.Instance.platforms[0].transform.position.y+1);
        float distance = Mathf.Abs(destination.y - transform.position.y);
        List<Transform> tempPlatforms = new List<Transform>();
        // print("Alt");

        foreach(GameObject platform in Grid.Instance.platforms){
            RaycastHit2D hit2D;
            hit2D = Physics2D.Raycast(groundcheck.position,Vector2.down,0.5f,groundLayer);
            if(!hit2D){
                continue;
            }
            if(hit2D.transform.name == platform.name){
                continue;
            }
            float tempDistance = Mathf.Abs(transform.position.y - platform.transform.position.y);

            if(tempDistance < jumpThreshold){
                distance = tempDistance;
                tempPlatforms.Add(platform.transform);
            }
        }
        if (tempPlatforms.Count>0){
            destination = new Vector2(tempPlatforms[0].position.x+2, tempPlatforms[0].position.y+1);     
            float xDistance = Mathf.Abs(transform.position.x - tempPlatforms[0].transform.position.x);
            float tempDistance = xDistance;
            // print("flag3");
            foreach(Transform platform in tempPlatforms){
                xDistance = Mathf.Abs(transform.position.x - platform.transform.position.x);
                if(xDistance<tempDistance){
                    tempDistance = xDistance;
                    destination = new Vector2(platform.position.x+(2*dir.x),platform.position.y+1);
                }
            }

            // foreach (Transform platform in tempPlatforms)
            // {
            //     print(platform.name);
            // }
            // print(newDes);
            if (IsRoof())
                FindSpace();
            else 
                TravelTo(destination, "Called in Alternate Path");  
        }    
    }

    private void OnDrawGizmos() {
        if(path!=null){
            for(int i=targetIndex;i<path.Length;i++){
                Gizmos.color = Color.blue;
                Gizmos.DrawCube(path[i],Vector2.one);
                if(i==targetIndex){
                    Gizmos.DrawLine(transform.position,path[i]);
                }else{
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }


        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundcheck.position,new Vector2(groundcheck.position.x,groundcheck.position.y+0.1f));
        Gizmos.DrawLine(new Vector2(transform.position.x + (4f * dir.x), transform.position.y), new Vector2(transform.position.x + (4f * dir.x), transform.position.y + unitHieght*2));
        Gizmos.DrawWireSphere(groundcheck.position,0.3f);
        Gizmos.DrawLine(new Vector2(groundcheck.position.x + (0.5f * dir.x), groundcheck.position.y), new Vector2(groundcheck.position.x + (0.5f * dir.x), groundcheck.position.y + 2f));
    }

}
