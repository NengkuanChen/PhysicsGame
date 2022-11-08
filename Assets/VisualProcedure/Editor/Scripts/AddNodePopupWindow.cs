using System;
using System.Collections.Generic;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    public class AddNodePopupWindow : PopupWindowContent
    {
        public class MenuItem
        {
            public MenuItem ParentItem { get; set; }
            public List<MenuItem> ChildrenItem { get; set; }

            /// <summary>
            /// 包含最终item名字的路径(eg. Entity/Fire)
            /// </summary>
            public string[] Path { get; set; }

            /// <summary>
            /// item名字(eg. Sequence)
            /// </summary>
            public string ItemName { get; set; }

            /// <summary>
            /// 在菜单中的层级(eg. 1)
            /// </summary>
            public int Level { get; set; }

            /// <summary>
            /// 是否是最终可以选择的菜单项
            /// </summary>
            public bool IsEndItem { get; set; }

            public Type ItemType { get; set; }

            public MenuItem()
            {
                ChildrenItem = new List<MenuItem>();
            }

            public void AddChildMenuItem(MenuItem childItem)
            {
                childItem.ParentItem = this;
                childItem.Level = Level + 1;
                ChildrenItem.Add(childItem);
            }

            public string GetPathLevelName(int level)
            {
                if (level < 0 || level >= Path.Length)
                {
                    Debug.LogErrorFormat("无法获得level{0}的路径名字", level);
                    return string.Empty;
                }

                return Path[level];
            }

            public string[] GetPathRange(int startIndex, int endIndex)
            {
                if (startIndex < 0 || startIndex >= Path.Length || endIndex < startIndex || endIndex >= Path.Length)
                {
                    Debug.LogErrorFormat("GetPathRange Error. startIndex: {0}  endIndex{1}", startIndex, endIndex);
                    return null;
                }

                var result = new string[endIndex - startIndex + 1];
                var resultIndex = 0;
                for (var i = startIndex; i <= endIndex; i++)
                {
                    result[resultIndex] = Path[i];
                    resultIndex++;
                }

                return result;
            }

            public override string ToString()
            {
                return $"MenuItem: path {string.Join("/", Path)}  itemName {ItemName} level {Level}";
            }
        }

        private const float WindowWidth = 200f;
        private const float WindowHeight = 400f;
        private const float RowHeight = 16f;
        private const float NavigationBarHeight = 20f;

        private const string SearchControlName = "AddNodeSearchControl";

        public string searchString;

        private List<MenuItem> filteredMenuItemList;
        private List<MenuItem> allEndMenuItemList;

        private Rect navBackRect;
        private Rect navForwardRect;

        private Vector2 searchScroll;
        private int scrollToIndex;
        private int hoverIndex;
        private float scrollOffset;

        private Action<Type> onSelectionNode;

        private MenuItem rootMenuItem;
        private static List<Type> graphNodeTypeList;

        /// <summary>
        /// 当前菜单显示的列表是currentParentMenuItem的ChildrenItem列表
        /// 所以当前菜单显示的level是currentParentMenuItem.Level+1
        /// </summary>
        private MenuItem currentParentMenuItem;

        private MenuItem CurrentParentMenuItem
        {
            get => currentParentMenuItem;
            set
            {
                currentParentMenuItem = value;
                UpdateFilterListByMenuTree();
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(WindowWidth, WindowHeight);
        }

        public AddNodePopupWindow(Action<Type> onSelectionNode)
        {
            this.onSelectionNode = onSelectionNode;
            filteredMenuItemList = new List<MenuItem>();
            allEndMenuItemList = new List<MenuItem>();

            if (graphNodeTypeList == null)
            {
                graphNodeTypeList = Utility.GetNodeTypeList();
            }

            ConstructMenuItems();
        }

        private void UpdateFilterListByMenuTree()
        {
            filteredMenuItemList.Clear();

            if (CurrentParentMenuItem == null)
            {
                Debug.LogError("没有构建菜单结构");
                return;
            }

            var displayMenuItems = CurrentParentMenuItem.ChildrenItem;
            for (var i = 0; i < displayMenuItems.Count; i++)
            {
                filteredMenuItemList.Add(displayMenuItems[i]);
            }

            hoverIndex = 0;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            searchString = string.Empty;
        }

    #region Draw Functions

        public override void OnGUI(Rect rect)
        {
            var searchRect = new Rect(0, 0, rect.width, EditorStyles.toolbar.fixedHeight);
            var scrollRect = Rect.MinMaxRect(0, searchRect.yMax, rect.xMax, rect.yMax);

            ProcessEvent(Event.current);
            DrawSearchBar(searchRect, Event.current);
            DrawSelectionArea(scrollRect, Event.current);

            if (EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.Repaint();
            }
        }

        private void ProcessEvent(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (e.button == 0)
                {
                    //点击导航返回
                    if (navBackRect.Contains(e.mousePosition))
                    {
                        if (CurrentParentMenuItem.ParentItem != null)
                        {
                            CurrentParentMenuItem = CurrentParentMenuItem.ParentItem;
                        }
                        e.Use();
                    }
                }
            }

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftArrow)
                {
                    if (CurrentParentMenuItem.ParentItem != null)
                    {
                        CurrentParentMenuItem = CurrentParentMenuItem.ParentItem;
                    }

                    e.Use();
                }

                if (e.keyCode == KeyCode.RightArrow)
                {
                    var hoverMenuItem = filteredMenuItemList[hoverIndex];
                    if (!hoverMenuItem.IsEndItem)
                    {
                        CurrentParentMenuItem = hoverMenuItem;
                    }

                    e.Use();
                }

                if (e.keyCode == KeyCode.DownArrow)
                {
                    hoverIndex = Mathf.Min(filteredMenuItemList.Count - 1, hoverIndex + 1);
                    scrollToIndex = hoverIndex;
                    scrollOffset = RowHeight;
                    e.Use();
                }

                if (e.keyCode == KeyCode.UpArrow)
                {
                    hoverIndex = Mathf.Max(0, hoverIndex - 1);
                    e.Use();
                    scrollToIndex = hoverIndex;
                    scrollOffset = -RowHeight;
                }

                if (Event.current.keyCode == KeyCode.Return)
                {
                    if (hoverIndex >= 0 && hoverIndex < filteredMenuItemList.Count)
                    {
                        var hoverMenuItem = filteredMenuItemList[hoverIndex];
                        if (hoverMenuItem.IsEndItem)
                        {
                            e.Use();
                            OnSelectionMade(filteredMenuItemList[hoverIndex]);
                            editorWindow.Close();
                        }
                        else
                        {
                            CurrentParentMenuItem = hoverMenuItem;
                        }
                    }
                }

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    e.Use();
                    editorWindow.Close();
                }
            }
        }

        private void DrawSearchBar(Rect rect, Event e)
        {
            if (e.type == EventType.Repaint)
            {
                EditorStyles.toolbar.Draw(rect, false, false, false, false);
            }

            var searchRect = new Rect(rect);
            searchRect.xMin += 6;
            searchRect.xMax -= 6;
            searchRect.y += 2;
            searchRect.width -= Utility.CancelButtonStyle.fixedWidth;

            //键盘直接选中当前控制
            GUI.FocusControl(SearchControlName);
            GUI.SetNextControlName(SearchControlName);

            EditorGUI.BeginChangeCheck();
            searchString = GUI.TextField(searchRect, searchString, Utility.SearchBoxStyle);
            if (EditorGUI.EndChangeCheck())
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    CurrentParentMenuItem = rootMenuItem;
                }
                else
                {
                    UpdateFilterListBySearchString();
                }
            }

            searchRect.x = searchRect.xMax;
            searchRect.width = Utility.CancelButtonStyle.fixedWidth;

            //tail icon
            if (string.IsNullOrEmpty(searchString))
                GUI.Box(searchRect, GUIContent.none, Utility.DisabledCancelButtonStyle);
            else if (GUI.Button(searchRect, "x", Utility.CancelButtonStyle))
            {
                searchString = string.Empty;
                CurrentParentMenuItem = rootMenuItem;
            }
        }

        private void UpdateFilterListBySearchString()
        {
            filteredMenuItemList.Clear();

            var lowerSearchString = searchString.ToLower();
            for (var i = 0; i < allEndMenuItemList.Count; i++)
            {
                var endItem = allEndMenuItemList[i];

                if (endItem.ItemName.ToLower().Contains(lowerSearchString))
                {
                    filteredMenuItemList.Add(endItem);
                }
            }

            hoverIndex = 0;
        }

        private void DrawSelectionArea(Rect rect, Event e)
        {
            //导航栏
            var navigationRect = new Rect(rect.x, rect.y, rect.width, NavigationBarHeight);
            navBackRect = new Rect(rect.x, rect.y, NavigationBarHeight, NavigationBarHeight);

            if (string.IsNullOrEmpty(searchString))
            {
                if (CurrentParentMenuItem.Level + 1 > 0)
                {
                    GUI.Box(navBackRect, "<-");
                }

                GUI.Box(navigationRect, CurrentParentMenuItem.ItemName, Utility.NavTitleStyle);
            }
            else
            {
                GUI.Box(navigationRect, "搜索", Utility.NavTitleStyle);
            }

            var scrollViewRect = new Rect(rect.x,
                rect.y + NavigationBarHeight,
                rect.width,
                rect.height - NavigationBarHeight);
            var contentRect = new Rect(0,
                0,
                rect.width - GUI.skin.verticalScrollbar.fixedWidth,
                filteredMenuItemList.Count * RowHeight);

            searchScroll = GUI.BeginScrollView(scrollViewRect, searchScroll, contentRect);

            var rowRect = new Rect(0, 0, rect.width, RowHeight);

            for (var i = 0; i < filteredMenuItemList.Count; i++)
            {
                //handle scroll to
                if (scrollToIndex == i &&
                    (Event.current.type == EventType.Repaint || Event.current.type == EventType.Layout))
                {
                    var r = new Rect(rowRect);
                    r.y += scrollOffset;
                    GUI.ScrollTo(r);
                    scrollToIndex = -1;
                    searchScroll.x = 0;
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    var hoveredMenuItem = filteredMenuItemList[i];

                    if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.ScrollWheel)
                    {
                        hoverIndex = i;
                    }

                    if (Event.current.type == EventType.MouseDown)
                    {
                        if (hoveredMenuItem.IsEndItem)
                        {
                            OnSelectionMade(filteredMenuItemList[i]);
                            EditorWindow.focusedWindow.Close();
                        }
                        else
                        {
                            CurrentParentMenuItem = hoveredMenuItem;
                            break;
                        }
                    }
                }

                DrawRow(rowRect, hoverIndex == i, filteredMenuItemList[i].ItemName, !filteredMenuItemList[i].IsEndItem);

                rowRect.y = rowRect.yMax;
            }

            GUI.EndScrollView();
        }

        private void DrawRow(Rect rowRect, bool isHoverRow, string rowTitle, bool hasNextLevel)
        {
            if (isHoverRow)
            {
                DrawSelectionBox(rowRect, Color.white);
            }

            var labelRect = new Rect(rowRect);
            labelRect.xMin += 8f;

            if (hasNextLevel)
            {
                var arrowSize = rowRect.height;
                var arrowRect = new Rect(rowRect.xMax - arrowSize, rowRect.y, arrowSize, arrowSize);
                GUI.Label(arrowRect, ">");

                labelRect.xMax -= arrowSize;
            }

            GUI.Label(labelRect, rowTitle);
        }

        public static void DrawSelectionBox(Rect rect, Color color)
        {
            var originColor = GUI.color;
            GUI.color = color;
            GUI.Box(rect, "", Utility.SelectionStyle);
            GUI.color = originColor;
        }

        private void OnSelectionMade(MenuItem selectedMenuItem)
        {
            if (onSelectionNode == null)
            {
                return;
            }

            onSelectionNode(selectedMenuItem.ItemType);
        }

    #endregion

    #region Helper Functions

        private void ConstructMenuItems()
        {
            //已经构建过了
            if (rootMenuItem != null)
            {
                return;
            }

            rootMenuItem = new MenuItem
            {
                ItemName = "流程节点",
                Level = -1,
                Path = new string[0],
            };

            for (var i = 0; i < graphNodeTypeList.Count; i++)
            {
                var nodeType = graphNodeTypeList[i];
                var attributeObjects = nodeType.GetCustomAttributes(typeof(ProcedureGraphNodeAttribute), false);
                if (attributeObjects.Length == 0)
                {
                    continue;
                }

                var nodeAttribute = attributeObjects[0] as ProcedureGraphNodeAttribute;

                var newItem = new MenuItem();
                newItem.ItemType = nodeType;
                var pathSplit = nodeAttribute.nodeMenuPath.Split('/');
                newItem.Path = pathSplit;
                newItem.ItemName = pathSplit[pathSplit.Length - 1];
                newItem.IsEndItem = true;
                newItem.Level = pathSplit.Length - 1;
                allEndMenuItemList.Add(newItem);

                AddToMenuItemTree(rootMenuItem, newItem);
            }

            CurrentParentMenuItem = rootMenuItem;

//            DebugMenuItemTree(rootMenuItem);
        }

        private void AddToMenuItemTree(MenuItem parentLevelItem, MenuItem newItem)
        {
            var level = parentLevelItem.Level + 1;

            if (newItem.Path.Length - 1 == level)
            {
                //这个节点就是加到这一层
                parentLevelItem.AddChildMenuItem(newItem);
                return;
            }

            var wantedPathLevelName = newItem.GetPathLevelName(level);
            //节点继续向下搜索
            var childrenItems = parentLevelItem.ChildrenItem;
            for (var i = 0; i < childrenItems.Count; i++)
            {
                var childItem = childrenItems[i];

                if (childItem.IsEndItem)
                {
                    continue;
                }

                if (childItem.GetPathLevelName(level) == wantedPathLevelName)
                {
                    AddToMenuItemTree(childItem, newItem);
                    return;
                }
            }

            //如果没有找到对应路径，则添加路径节点
            var pathItem = new MenuItem
            {
                IsEndItem = false,
                Level = level,
                ItemName = wantedPathLevelName,
                Path = newItem.GetPathRange(0, level)
            };
            parentLevelItem.AddChildMenuItem(pathItem);
            AddToMenuItemTree(pathItem, newItem);
        }

        private void DebugMenuItemTree(MenuItem parentMenuItem)
        {
            if (parentMenuItem == null)
            {
                return;
            }

            Debug.Log(parentMenuItem);
            var childrenItemList = parentMenuItem.ChildrenItem;
            if (childrenItemList != null)
            {
                for (var i = 0; i < childrenItemList.Count; i++)
                {
                    DebugMenuItemTree(childrenItemList[i]);
                }
            }
        }

    #endregion
    }
}