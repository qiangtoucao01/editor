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
    
/// <summary>
/// 角色控制类
/// </summary>
    public class RoleCtrl:MonoBehaviour
    {
        /// <summary>
        /// 角色速度
        /// </summary>
        public float speed = 10;
        
        /// <summary>
        /// 
        /// </summary>
        private MapGrid mapGrid;

        /// <summary>
        /// 当前行
        /// </summary>
        private int currRow=3;
        
        /// <summary>
        /// 当前列
        /// </summary>
        private int currColumn=1;

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
        private int currCellId=51;
        
        /// <summary>
        /// 目标cellid
        /// </summary>
        private int targetCellId=0;

        /// <summary>
        /// 当前的路径点
        /// </summary>
        private int index = 0;

        /// <summary>
        /// 是否有新的寻路
        /// </summary>
        private bool IsClick;
      
        
        //旋转速度是不是直接转向就可以了，或者转向后根据朝向替换照片。

        private void Update()
        {
            //点击鼠标触发，寻路，移动
            if (Input.GetMouseButtonDown(0))
            {
                //计算点击的cell的行列
               Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    //获取mapgrid和map的所有子节点cell数组
                    if (mapGrid == null)
                    {
                        mapGrid = GameObject.Find("map").GetComponent<MapGrid>();
                        
                        //map的cell列表，如果没有就进行装填
                        if (mapGrid.cells.Count == 0)
                        {
                            mapGrid.cells = mapGrid.transform.GetComponentsInChildren<Cell>().ToList();
                    
                        }
                    }
                    
                    Cell cell=hit.collider.gameObject.GetComponent<Cell>();

                    if (cell == null)//点击的不是cell模块
                    {
                        return;
                    }
                    
                    //如果点击的是同一个cell模块，直接忽略
                    if (currCellId == cell.Id)
                    {
                        return;
                    }

                    currCell = cell;//获取点击的目标cell
                    
                   
                    
                    if (currCell != null)
                    {
                        if (isMove == false)//不在移动
                        {
                            Debug.Log("当前不在移动，开始寻路");
                            StartMove();
                        }
                        else//正在移动,终止（只是做个标记，等当前移动结束，再进行寻路处理）
                        {
                            Debug.Log("提醒你一次");
                           // isMove = false ;

                            if (IsClick == false)
                            {
                                IsClick = true;
                            }
                        }
                    }
                  
                }

            }

            
        }

        /// <summary>
        ///根据目标cellid获取行列，确定目标路径点。开始移动到下一个点。
        /// </summary>
        void StartMove()
        {
            if (currCell.Id == currCellId)//当前所在cell的id等于目标cell的id不用进行新的寻路。
            {
                Debug.Log("当前已经在目标点id："+currCell.Id);
                return;
            }
            
            //获取目标cell所在的行列
            GetTargetRowAndColumn( currCell.Id);
            
            //重置celllist索引
            index = 1;
                            
            //获取路径列表（如果有在运动就不该立刻执行，要等待到达当前目标结束之后进行）
            GetCellDir();
      
            for (int i = 0; i < cellList.Count; i++)
            {
                Debug.Log("路径点id" + cellList[i]);
            }
           // index++;
            if (cellList.Count <= index)//路径点只有一个点
            {
                return;
            }
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
                #region  斜线移动(禁用)

//                if (targetRow > row && targetColumn < column) //左下
//                {
//                    cellDir = CellId.DownLeft;
//                }
//                else if (targetRow > row && targetColumn == column) //下
//                {
//                    cellDir = CellId.Down;
//                }
//                else if (targetRow > row && targetColumn > column) //右下
//                {
//                    cellDir = CellId.DownRight;
//                }
//                else if (targetRow == row && targetColumn < column) //左边
//                {
//                    cellDir = CellId.Left;
//                }
//                else if (targetRow == row && targetColumn > column) //右边
//                {
//                    cellDir = CellId.Right;
//                }
//                else if (targetRow < row && targetColumn == column) //上边
//                {
//                    cellDir = CellId.Up;
//                }
//                else if (targetRow < row && targetColumn < column) //左上
//                {
//                    cellDir = CellId.UpLeft;
//                }
//                else if (targetRow < row && targetColumn > column) //右上
//                {
//                    cellDir = CellId.UpRight;
//                }

                #endregion

                #region 直线移动（只判断上下左右四个方向）

                //判断下一步的方向。（必须结合判断相应的障碍问题。）
                if ( targetColumn < column) //左
                {
                    cellDir = CellId.Left;
                }
                else if ( targetColumn > column) //右
                {
                    cellDir = CellId.Right;
                }
                else if (targetRow > row ) //上(在当前cell点的下面)
                {
                    cellDir = CellId.Down;
                }
                else if (targetRow < row ) //下
                {
                    cellDir = CellId.Up;
                }

                #endregion
               
               

                //进行id+row+column的处理，
                switch (cellDir)
                {
                    case CellId.Up:
                        id -= mapGrid.y;
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
                        id += mapGrid.y;
                        row += 1;
                        break;

                        #region 斜线处理（禁用）
//                    case CellId.DownLeft:
                                //                        id +=mapGrid.y-1;
                                //                        column -= 1;
                                //                        row += 1;
                                //                        break;
                                //                    case CellId.DownRight:
                                //                        id +=mapGrid.y+1;
                                //                        column += 1;
                                //                        row += 1;
                                //                        break;
                                //                    
                                //                    case CellId.UpLeft:
                                //                        id -= mapGrid.y+1;
                                //                        column -= 1;
                                //                        row -= 1;
                                //                        break;
                                //                    case CellId.UpRight:
                                //                        id -= mapGrid.y-1;
                                //                        column += 1;
                                //                        row -= 1;
                                //                        break;
                    #endregion
                  
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
            //计算行列的间距，并推算出移动的目标点坐标和当前的间距(50,120)  (320,240)
            targetPos=new Vector3(){x =(targetColumn-currColumn)*50 ,y=(currRow-targetRow)*120,z=0};
            
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
           
            Debug.Log("当前到达目标点"+currCellId);
            
            //如果有新的移动需求，开始新的需求
            if (IsClick)
            {
                Debug.Log("重新寻路");
                IsClick = false;
                isMove = false;
                StartMove();
            }
            else
            {
                if (index < cellList.Count-1)//继续寻路
                {
                        index++;
                        //获取新的目标行列
                        GetTargetRowAndColumn(cellList[index]);
                    
                    //判断下一个点的是否障碍属性，如果是就要在当前节点的基础之上进行新的路径选择。
                    
                    
                        //开始移动
                        StartCoroutine(MoveToTarget());

                }
                else//移动结束
                {

                        Debug.Log(2);
                        isMove = false;

                }
            }
            
          

        }
    }
