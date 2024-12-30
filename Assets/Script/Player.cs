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
    
    private Vector3 mouseDownPosition;
    private Vector3 mouseUpPosition;
    private Vector3 mouseDirection;
    private Vector3 moveDirection;
    
    private float offsetY;
    private int currentStack;

    // Start is called before the first frame update
    private void Start()
    {
        Invoke("OnInit", 0.001f);//trì hoãn đợi hình thành bản đồ
    }

    // Update is called once per frame
    private void Update()
    {
        if (AtFinishLine())
        {
            DestroyAllChildObject();
            Celebrate();
            isMoving = false;
            return;//nếu đã đến đích thì dừng game lại
        }
        if (isMoving)
        {
            if (CheckWall())
            {
                isMoving = false;
            }
            else
            {
                if (moveDirection == Vector3.forward)
                {
                    MoveForward();
                }
                else if (moveDirection == Vector3.back)
                {
                    MoveBackward();
                }
                else if (moveDirection == Vector3.left)
                {
                    MoveLeft();
                }
                else if (moveDirection == Vector3.right)
                {
                    MoveRight();
                }
                CheckBrick();
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
                mouseDirection = mouseUpPosition - mouseDownPosition;//Tính vector hướng
                CheckDirection();
                isMoving = true;
            }
        }
    }

    private void OnInit()
    {
        transform.position = mapManager.StartPosition;
        isMoving = false;
        moveDirection = Vector3.zero;
        currentStack = 0;
        offsetY = 1f;
    }

    private void Celebrate()
    {
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }

    private void CheckDirection()
    {
        if (Mathf.Abs(mouseDirection.x) > Mathf.Abs(mouseDirection.y))//Xác định hướng đi
        {
            if (mouseDirection.x > 0)
            {
                moveDirection = Vector3.right; //phai
            }
            else
            {
                moveDirection = Vector3.left; //trai
            }
        }
        else
        {
            if (mouseDirection.y > 0)
            {
                moveDirection = Vector3.forward; //len
            }
            else
            {
                moveDirection = Vector3.back; //xuong
            }
        }  
    }
    private bool CheckWall()
    {
        return Physics.Raycast(transform.position, moveDirection, 0.5f, wallLayer);
    }
    private void MoveForward()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z+1);
    }
    private void MoveBackward()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y,transform.position.z-1);
    }

    private void MoveRight()
    {
        transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
    }
    private void MoveLeft()
    {
        transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
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
            child.SetParent(null);
            DestroyImmediate(child.gameObject);
        }
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
            DestroyPreBlockBrick();
            CheckBlockDirection();
            isMoving = true;
        }
    }
    private void CheckBlockDirection()
    {
        if (moveDirection == Vector3.forward || moveDirection == Vector3.back)
        {
            if (Physics.Raycast(transform.position, Vector3.right, 1f, wallLayer))
            {
                moveDirection = Vector3.left;//trai
            }
            else if (Physics.Raycast(transform.position, Vector3.left, 1f, wallLayer))
            {
                moveDirection = Vector3.right;//phai
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, Vector3.forward, 1f, wallLayer))
            {
                moveDirection = Vector3.back;//xuong
            }
            else if (Physics.Raycast(transform.position, Vector3.back, 1f, wallLayer))
            {
                moveDirection = Vector3.forward;//len
            }
        }
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
