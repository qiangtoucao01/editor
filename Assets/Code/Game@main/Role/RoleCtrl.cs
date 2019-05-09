using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


    public enum CellId
    {
        Up=0,
        UpLeft=1,
        UpRight=2,
        Left=3,
        Right=4,
        Down=5,
        DownLeft=6,
        DownRight=7
    }
    
    public class RoleCtrl:MonoBehaviour
    {
        /// <summary>
        /// 角色速度
        /// </summary>
        public float speed = 10;
        
        /// <summary>
        /// 
        /// </summary>
        private MapGrid mapGrid=new MapGrid();

        /// <summary>
        /// 当前行
        /// </summary>
        private int currRow=1;
        
        /// <summary>
        /// 当前列
        /// </summary>
        private int currColumn=3;

        /// <summary>
        /// 目标行
        /// </summary>
        private int targetRow;

        /// <summary>
        /// 目标列
        /// </summary>
        private int targetColumn;

        //是否移动
        private bool isMove=false ;

        private Vector3 targetPos;

        private Cell currCell;
        
      List<int> cellList=new List<int>();

        /// <summary>
        /// 当前cellid
        /// </summary>
        private int currCellId=3;
        
        /// <summary>
        /// 目标cellid
        /// </summary>
        private int targetCellId=0;

        /// <summary>
        /// 当前的路径点
        /// </summary>
        private int index = 0;
      
        
        //旋转速度是不是直接转向就可以了，或者转向后根据朝向替换照片。

        private void OnEnable()
        {
            mapGrid = GameObject.Find("map").GetComponent<MapGrid>();
        }

        private void Start()
        {
           
        }

        private void Update()
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                //计算点击的cell的行列
               Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    //map的cell列表，如果没有就进行装填
                    if (mapGrid.cells.Count == 0)
                    {
                         mapGrid.cells = mapGrid.transform.GetComponentsInChildren<Cell>().ToList();
                    
                    }
                    
                    currCell = hit.collider.gameObject.GetComponent<Cell>();
                    if (currCell != null)
                    {
                       
                        if (isMove == false)//不在移动
                        {
                            StartMove();
                        }
                        else//正在移动,终止
                        {
                            isMove = false ;
                        }
                    }
                  
                }

            }

            
        }

        /// <summary>
        /// 获取目标路径，并调用运动方法
        /// </summary>
        void StartMove()
        {
            
            //获取目标行列
            GetTargetRowAndColumn( currCell.Id);
            
            //重置celllist索引
            index = 0;
                            
            //获取路径列表（如果有在运动就不该立刻执行，要等待到达当前目标结束之后进行）
            GetCellDir();

            index++;

            //  currCellId = cellList[index];
            GetTargetRowAndColumn(cellList[index]); //目标路径的第二个节点
            // Test(pos);
            //角色移动
            // transform.localPosition = pos;
            isMove = true;
            StartCoroutine(MoveToTarget());
        }

        #region  获取路径点



        /// <summary>
        /// cell方位枚举
        /// </summary>
        private CellId cellDir = CellId.Up;
        
        /// <summary>
        /// 获取路径点数组
        /// </summary>
        void GetCellDir()
        {
            int row = currRow;
            int column = currColumn;
            int id = currCellId;
            
            cellList.Clear();
            cellList.Add(id);
            while (targetRow != row || targetColumn != column)
            {
                //判断下一步的方向。
                if (targetRow > row && targetColumn < column) //左下
                {
                    cellDir = CellId.DownLeft;
                }
                else if (targetRow > row && targetColumn == column) //下
                {
                    cellDir = CellId.Down;
                }
                else if (targetRow > row && targetColumn > column) //右下
                {
                    cellDir = CellId.DownRight;
                }
                else if (targetRow == row && targetColumn < column) //左边
                {
                    cellDir = CellId.Left;
                }
                else if (targetRow == row && targetColumn > column) //右边
                {
                    cellDir = CellId.Right;
                }
                else if (targetRow < row && targetColumn == column) //上边
                {
                    cellDir = CellId.Up;
                }
                else if (targetRow < row && targetColumn < column) //左上
                {
                    cellDir = CellId.UpLeft;
                }
                else if (targetRow < row && targetColumn > column) //右上
                {
                    cellDir = CellId.UpRight;
                }

                //进行id+row+column的处理
                switch (cellDir)
                {
                    case CellId.Up:
                        id -= 4;
                        row -= 1;
                        break;
                    case CellId.UpLeft:
                        id -= 5;
                        column -= 1;
                        row -= 1;
                        break;
                    case CellId.UpRight:
                        id -= 3;
                        column += 1;
                        row -= 1;
                        break;
                    case CellId.Right:
                        id += 1;
                        column += 1;

                        break;
                    case CellId.Left:
                        id -= 1;
                        column -= 1;

                        break;
                    case CellId.Down:
                        id += 4;
                        row += 1;
                        break;
                    case CellId.DownLeft:
                        id += 3;
                        column -= 1;
                        row += 1;
                        break;
                    case CellId.DownRight:
                        id += 5;
                        column += 1;
                        row += 1;
                        break;
                }

                //将id加入路径表。
                cellList.Add(id);
            }
        }

        #endregion

        /// <summary>
        /// 某个id的所在行列数
        /// </summary>
        /// <param name="id"></param>
        void GetTargetRowAndColumn(int id)
        {
            float column = (float) id% mapGrid.y;
                        
            float  row = (float) id / mapGrid.y;
            //目标行
            targetRow = (int) (row + 1);

            if (column == 0)
            {
                targetRow -= 1;
            }

            //目标列
            targetColumn = (int)(column == 0 ? mapGrid.y : column);
        }

/// <summary>
/// 采用递归，当前目标点结束之后，开始寻找下一个点。
/// </summary>
/// <returns></returns>
         IEnumerator  MoveToTarget()
        {       
            //计算行列的间距，并推算出移动的目标点坐标和当前的间距
            targetPos=new Vector3(){x =(targetColumn-currColumn)*320 ,y=(currRow-targetRow)*240,z=0};
            
            //找到目标点
            targetPos = targetPos + transform.localPosition;
            
            targetPos=new Vector3(targetPos.x,targetPos.y,0);
         
            while (Vector3.Distance(transform.localPosition, targetPos) >0.1f)
            {
              
                transform.localPosition = Vector3.MoveTowards(transform.localPosition ,targetPos,speed*Time.deltaTime);
               
                yield return null;
            }
            
           

            //矫正目标坐标
            transform.localPosition = targetPos;
            //改变当前所在行列
            currColumn = targetColumn;
            currRow = targetRow;

            //当前cellid。
            currCellId = cellList[index];
            Debug.Log(mapGrid.cells[currCellId-1].Id+"id");
            //读取当前的cell事件类型
          Debug.Log("我的事件类型是："+mapGrid.cells[currCellId-1].eventType);
              
            //如果路径列表还有，获取新的目标节点执行
            if (index < cellList.Count-1)//继续寻路
            {
                if (isMove)//正常移动
                {
                    index++;
                    //获取新的目标行列
                    GetTargetRowAndColumn(cellList[index]);
                    //开始移动
                    StartCoroutine(MoveToTarget());
                }
                else
                {
                    Debug.Log("开始转向");
                    //转变方向
                    StartMove();
                }
            }
            else//移动结束
            {
                if (isMove)
                {
                    isMove = false;
                }
                else
                {
                    Debug.Log("开始转向");
                    //转变方向
                    StartMove();
                }
            }

        }
    }
