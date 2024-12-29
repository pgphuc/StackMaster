using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask endBrickLayer;
    [SerializeField] private LayerMask eatBrickLayer;
    [SerializeField] private LayerMask takeBrickLayer;
    [SerializeField] private LayerMask preBlockBrickLayer;
    
    [SerializeField] private GameObject prefab_EatBrick;
    [SerializeField] private GameObject prefab_NormalBrick;
    [SerializeField] private GameObject prefab_PointBrick;
    [SerializeField] private GameObject prefab_BlockConner;

    
    private bool isMoving;
    /*
     * 0 --> dung im
     * 1 --> len
     * 2 --> xuong
     * 3 --> trai
     * 4 --> phai
     */
    private int moveDirection;
    
    private Vector3 mouseDownPosition;
    private Vector3 mouseUpPosition;
    private Vector3 direction;
    
    private float offsetY;
    private int currentStack;

    // Start is called before the first frame update
    private void Start()
    {
        Invoke("OnInit", 0.01f);//trì hoãn đợi hình thành bản đồ
    }

    // Update is called once per frame
    private void Update()
    {
        if (AtFinishLine())
        {
            DestroyAllChildObject();
            isMoving = false;
            return;//nếu đã đến đích thì dừng game lại
        }
        if (isMoving)
        {
            switch (moveDirection)
            {
                case 1:
                    MoveForward();
                    break;
                case 2:
                    MoveBackward();
                    break;
                case 3:
                    MoveLeft();
                    break;
                case 4:
                    MoveRight();
                    break;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPosition = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseUpPosition = Input.mousePosition;
                direction = mouseUpPosition - mouseDownPosition;//Tính vector hướng
                isMoving = true;
                CheckDirection();
            }
        }
    }

    private void OnInit()
    {
        transform.position = mapManager.StartPosition;
        isMoving = false;
        moveDirection = 0;
        currentStack = 0;
        offsetY = 1f;
    }

    private void CheckDirection()
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))//Xác định hướng đi
        {
            if (direction.x > 0)
            {
                moveDirection = 4; //phai
            }
            else
            {
                moveDirection = 3; //trai
            }
        }
        else
        {
            if (direction.y > 0)
            {
                moveDirection = 1; //len
            }
            else
            {
                moveDirection = 2; //xuong
            }
        }
            
    }
    private void MoveForward()
    {
        if (Physics.Raycast(transform.position, Vector3.forward, 0.5f, wallLayer))
        {
            isMoving = false;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z+1);
            CheckBrick();
        }
    }
    private void MoveBackward()
    {
        if (Physics.Raycast(transform.position, Vector3.back, 0.5f, wallLayer))
        {
            isMoving = false;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,transform.position.z-1);
            CheckBrick();
        }
    }

    private void MoveRight()
    {
        if (Physics.Raycast(transform.position, Vector3.right, 0.5f, wallLayer))
        {
            isMoving = false;
        }
        else
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            CheckBrick();
        }
    }
    private void MoveLeft()
    {
        if (Physics.Raycast(transform.position, Vector3.left, 0.5f, wallLayer))
        {
            isMoving = false;
        }
        else
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            CheckBrick();
        }
    }
    private void StackPointBrick()
    {
        transform.position += new Vector3(0, 0.2f, 0);
        Instantiate(prefab_PointBrick, transform.position - new Vector3(0, offsetY, 0), Quaternion.identity, transform);
        currentStack++;
        offsetY += 0.2f;
    }
    private void DeStackPointBrick()
    {
        currentStack--;
        Destroy(transform.GetChild(currentStack).gameObject);
        transform.position -= new Vector3(0, 0.2f, 0); 
        offsetY -= 0.2f;
    }
    private bool AtFinishLine()
    {
        return Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity, endBrickLayer);
    }
    private void DestroyAllChildObject()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        transform.position -= new Vector3(0f, offsetY, 0f);
    }
    private bool OnEatBrick()
    {
        return Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity, eatBrickLayer);
    }
    private bool OnPreBlockBrick()
    {
        return Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity, preBlockBrickLayer);
    }

    private bool OnTakeBrick()
    {
        return Physics.Raycast(transform.position, Vector3.down, Mathf.Infinity, takeBrickLayer);
    }
    private void CheckBrick()
    {
        if (OnEatBrick())
        {
            StackPointBrick();
            DestroyEatBrick();
        }
        else if (OnTakeBrick())
        {
            DeStackPointBrick();
        }
        else if (OnPreBlockBrick())
        {
            StackPointBrick();
            DestroyPreBlockBrick();
            CheckWall();
        }
    }
    private void CheckWall()
    {
        if (moveDirection == 1 || moveDirection == 2)
        {
            if (Physics.Raycast(transform.position, Vector3.right, 1f, wallLayer))
            {
                moveDirection = 3;//trai
            }
            else if (Physics.Raycast(transform.position, Vector3.left, 1f, wallLayer))
            {
                moveDirection = 4;//phai
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.forward, 1f, wallLayer))
            {
                moveDirection = 2;//xuong
            }
            else if (Physics.Raycast(transform.position, Vector3.back, 1f, wallLayer))
            {
                moveDirection = 1;//len
            }
        }
        isMoving = true;
    }

    private void DestroyEatBrick()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, eatBrickLayer))
        {
            Vector3 pos = hit.collider.transform.position;
            Destroy(hit.collider.gameObject);
            Instantiate(prefab_NormalBrick, pos, Quaternion.identity);
        }
    }

    private void DestroyPreBlockBrick()
    {
        
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, preBlockBrickLayer))
        {
            Vector3 pos = hit.collider.transform.position;
            Destroy(hit.collider.gameObject);
            Instantiate(prefab_BlockConner, pos, Quaternion.identity);
        }
    }
    
    
}
