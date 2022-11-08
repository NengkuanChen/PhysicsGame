using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
    public class GraphEditorWindow : EditorWindow
    {
        [MenuItem("游戏流程/可视化面板", priority = 0)]
        public static void ShowWindow()
        {
            var graphEditorWindow = GetWindow<GraphEditorWindow>();
            graphEditorWindow.minSize = new Vector2(800, 600);
            graphEditorWindow.titleContent = new GUIContent("Visual Procedure ©OnlyCarStudio.LagField");
        }

        public static GraphEditorWindow instance;
        public GraphEditorData data;

        protected Rect guiRect;

        private ControlType controlType = ControlType.None;

        private NodeEditorView draggingNodeView;

        private PortEditorView currentHoverPortView;
        private NodeEditorView currentHoverNodeView;
        private ConnectionLineView currentHoverConnectionLineView;
        private double startHoverTime;

        private ConnectionLineView draggingLineView;

        private bool isCtrlDown;
        private bool isMouseLeftDown;
        private Vector2 startDraggingCommentBoxGraphPosition;
        private Vector2 endDraggingCommentBoxGraphPosition;
        private CommentBoxView lastClickCommentBox;
        private double lastClickCommentTime;
        private CommentBoxView editingCommentBox;
        private CommentBoxView draggingCommentBox;
        private CommentBoxView resizingCommentBox;
        private CommentBoxView.BoxEdge resizingCommentEdge;
        private double lastClickNodeViewTitleTime;
        private NodeEditorView lastClickNodeView;

        private Vector2 startMultiSelectionPos;

        private Matrix4x4 originMatrix;

        private double repaintTimer;

        private void OnEnable()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Reset()
        {
            if (data != null)
            {
                data.Clear();
            }

            draggingNodeView = null;
            currentHoverPortView = null;
            currentHoverNodeView = null;
            controlType = ControlType.None;

            repaintTimer = EditorApplication.timeSinceStartup;
        }

        private void Update()
        {
            if (EditorApplication.timeSinceStartup - repaintTimer > 0.03)
            {
                repaintTimer = EditorApplication.timeSinceStartup;
                Repaint();
            }

            //重置双击状态
            if (lastClickCommentBox != null)
            {
                var currentTime = EditorApplication.timeSinceStartup;
                if (currentTime - lastClickCommentTime > 0.3f)
                {
                    lastClickCommentBox = null;
                }
            }

            if (lastClickNodeView != null)
            {
                var currentTime = EditorApplication.timeSinceStartup;
                if (currentTime - lastClickNodeViewTitleTime > .5f)
                {
                    lastClickNodeView = null;
                }
            }
        }

        private void OnGUI()
        {
            if (Utility.LoadedGuiSkin == null)
            {
                Utility.LoadGuiSkin();
                return;
            }

            if (data == null)
            {
                LoadGraph();
                return;
            }

            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("等待编译完成..."));
            }

            var e = Event.current;

            GUILayout.Label($"{Application.productName}\n游戏流程图", Utility.GetGuiStyle("SkillGraphName"));

            originMatrix = GUI.matrix;
            guiRect = new Rect(0, 0, position.width, position.height);
            DrawHelper.DrawGrid(50, .5f, guiRect, data.GraphOffset, data.GraphZoom);
            DrawHelper.DrawGrid(25, .3f, guiRect, data.GraphOffset, data.GraphZoom);

            if (!EditorApplication.isCompiling)
            {
                ProcessEvent(e);
            }

            BeginZoom();

            DrawCommentBox();
            DrawConnectionLine();
            DrawNodes();

            EndZoom();
            DrawMultiSelection(e);

            GUI.matrix = originMatrix;

            DrawTipBox();
        }

        private void ProcessEvent(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                isMouseLeftDown = true;
            }
            else if (e.type == EventType.MouseUp && e.button == 0)
            {
                isMouseLeftDown = false;
            }

            //记录左ctrl按下状态
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
            {
                isCtrlDown = true;
            }
            else if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl)
            {
                isCtrlDown = false;
            }

            var windowMousePosition = e.mousePosition;
            var zoomedMousePosition = NonZoomedWindowPositionToZoomedWindowPosition(windowMousePosition);

            var nodeViewList = data.currentNodes;
            var connectionLineViewList = data.connectionLines;

            var hasHoveredNodeOrPort = false;
            // 如果在输入标签上，要屏蔽与注释框的交互
            // var isHoverNodeInputLabel = false;
            for (var i = 0; i < nodeViewList.Count; i++)
            {
                var nodeView = nodeViewList[i];

            #region 提示框鼠标悬浮显示

                //在没有任何操作的时候，才显示tips框
                if (controlType == ControlType.None && !hasHoveredNodeOrPort)
                {
                    if (nodeView.NodeNameRect.Contains(zoomedMousePosition))
                    {
                        //reset time
                        if (currentHoverNodeView != nodeView)
                        {
                            startHoverTime = EditorApplication.timeSinceStartup;
                        }

                        hasHoveredNodeOrPort = true;
                        currentHoverNodeView = nodeView;
                        currentHoverPortView = null;
                    }

                    if (!hasHoveredNodeOrPort)
                    {
                        var allPortList = nodeView.allPortList;
                        foreach (var portView in allPortList)
                        {
                            if (portView.portViewRect.Contains(zoomedMousePosition))
                            {
                                //reset time
                                if (currentHoverPortView != portView)
                                {
                                    startHoverTime = EditorApplication.timeSinceStartup;
                                }

                                hasHoveredNodeOrPort = true;
                                currentHoverPortView = portView;
                                currentHoverNodeView = null;

                                break;
                            }
                        }
                    }
                }

            #endregion

                if (controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 0)
                {
                    //点击开始拖拽连线
                    var allPortList = nodeView.allPortList;
                    foreach (var portView in allPortList)
                    {
                        if (portView.connectionCircleRect.Contains(zoomedMousePosition))
                        {
                            controlType = ControlType.DraggingConnection;
                            draggingLineView = new ConnectionLineView(portView);
                            draggingLineView.SetEndPos(zoomedMousePosition);
                            e.Use();
                            break;
                        }
                    }

                    //开始拖拽节点
                    if (nodeView.viewRect.Contains(zoomedMousePosition))
                    {
                        if (data.selectedNodes.Contains(nodeView))
                        {
                            controlType = ControlType.DraggingMultiNodes;
                        }
                        else
                        {
                            controlType = ControlType.DraggingNode;
                            draggingNodeView = nodeView;
                            data.PutNodeToListTail(i);
                        }

                        if (lastClickNodeView == nodeView)
                        {
                            var elapseSeconds = EditorApplication.timeSinceStartup - lastClickNodeViewTitleTime;
                            if (elapseSeconds <= .5f)
                            {
                                lastClickNodeView = null;
                                nodeView.OpenNodeScriptFile();
                                controlType = ControlType.None;
                            }
                        }

                        lastClickNodeViewTitleTime = EditorApplication.timeSinceStartup;
                        lastClickNodeView = nodeView;

                        e.Use();
                        break;
                    }
                }

                //节点右键菜单
                if (controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 1)
                {
                    if (nodeView.viewRect.Contains(zoomedMousePosition))
                    {
                        OpenNodeGenericMenu(nodeView, e.mousePosition);
                        e.Use();
                        break;
                    }
                }
            }

            if (currentHoverConnectionLineView != null)
            {
                currentHoverConnectionLineView.SetHovering(false);
            }

            currentHoverConnectionLineView = null;
            if (controlType == ControlType.None)
            {
                //和连接线的交互
                for (var i = 0; i < connectionLineViewList.Count; i++)
                {
                    var connectionLineView = connectionLineViewList[i];
                    if (connectionLineView.IsPositionCloseToLine(zoomedMousePosition))
                    {
                        currentHoverConnectionLineView = connectionLineView;
                        currentHoverConnectionLineView.SetHovering(true);

                        //右键点击连线
                        if (e.type == EventType.MouseDown && e.button == 1)
                        {
                            OpenConnectionLineGenericMenu(connectionLineView, e.mousePosition);
                            e.Use();
                        }

                        break;
                    }
                }
            }

            if (!hasHoveredNodeOrPort)
            {
                currentHoverPortView = null;
                currentHoverNodeView = null;
            }

            if (controlType == ControlType.DraggingNode && e.type == EventType.MouseDrag && e.button == 0)
            {
                draggingNodeView.Drag(e.delta / data.GraphZoom);
                e.Use();
            }

            if (controlType == ControlType.DraggingNode && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;
                draggingNodeView = null;
                e.Use();
            }

            if (controlType == ControlType.DraggingMultiNodes && e.type == EventType.MouseDrag && e.button == 0)
            {
                foreach (var t in data.selectedNodes)
                {
                    t.Drag(e.delta / data.GraphZoom);
                }

                e.Use();
            }

            if (controlType == ControlType.DraggingMultiNodes && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;

                e.Use();
            }

            if (controlType == ControlType.DraggingConnection && e.type == EventType.MouseDrag && e.button == 0)
            {
                if (draggingLineView != null)
                {
                    draggingLineView.SetEndPos(zoomedMousePosition);
                }
            }

            if (controlType == ControlType.DraggingConnection && e.type == EventType.MouseUp && e.button == 0)
            {
                if (draggingLineView != null)
                {
                    var createNewConnection = false;
                    //检查是否有连接
                    foreach (var nodeView in nodeViewList)
                    {
                        var allPortList = nodeView.allPortList;
                        foreach (var portView in allPortList)
                        {
                            if (portView.connectionCircleRect.Contains(zoomedMousePosition))
                            {
                                if (ConnectionLineView.CheckPortsCanLine(draggingLineView.draggingPort, portView))
                                {
                                    var newConnectionLine =
                                        new ConnectionLineView(draggingLineView.draggingPort, portView, data);
                                    data.connectionLines.Add(newConnectionLine);

                                    createNewConnection = true;
                                    break;
                                }
                            }
                        }

                        if (createNewConnection)
                        {
                            break;
                        }
                    }

                    draggingLineView.Dispose();
                }

                draggingLineView = null;
                controlType = ControlType.None;
                e.Use();
            }

            //中键拖动面板
            if (e.type == EventType.MouseDrag && e.button == 2)
            {
                data.GraphOffset += e.delta / data.GraphZoom;
                e.Use();

                if (draggingLineView != null)
                {
                    draggingLineView.SetEndPos(zoomedMousePosition);
                }
            }

            //滚轮控制缩放
            if (e.type == EventType.ScrollWheel)
            {
                data.GraphZoom -= e.delta.y / GraphEditorData.GraphZoomSpeed;
                data.GraphZoom = Mathf.Clamp(data.GraphZoom,
                    GraphEditorData.MinGraphZoom,
                    GraphEditorData.MaxGraphZoom);
                e.Use();

                if (draggingLineView != null)
                {
                    draggingLineView.SetEndPos(zoomedMousePosition);
                }
            }

            //开始拖拽新的注释框
            if (isCtrlDown && controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 0)
            {
                controlType = ControlType.DraggingNewCommentBox;
                startDraggingCommentBoxGraphPosition = WindowPositionToGraphPosition(windowMousePosition);
                endDraggingCommentBoxGraphPosition = startDraggingCommentBoxGraphPosition;

                e.Use();
            }

            //更新新的注释框
            if (isCtrlDown && controlType == ControlType.DraggingNewCommentBox && e.type == EventType.MouseDrag &&
                e.button == 0)
            {
                endDraggingCommentBoxGraphPosition = WindowPositionToGraphPosition(windowMousePosition);
                e.Use();
            }

            //结束新的注释框
            if (controlType == ControlType.DraggingNewCommentBox && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;

                var size = startDraggingCommentBoxGraphPosition - endDraggingCommentBoxGraphPosition;
                //要大于一定尺寸
                if (Mathf.Abs(size.x) > 100 && Mathf.Abs(size.y) > 100)
                {
                    var newCommentBox = new CommentBoxView(this,
                        startDraggingCommentBoxGraphPosition,
                        endDraggingCommentBoxGraphPosition);
                    data.commentBoxViews.Add(newCommentBox);
                }

                e.Use();
            }

            //注释框操作
            if (data.commentBoxViews.Count > 0)
            {
                for (var i = 0; i < data.commentBoxViews.Count; i++)
                {
                    var commentBoxView = data.commentBoxViews[i];

                    //右键点击注释框
                    if (controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 1)
                    {
                        if (commentBoxView.Contains(zoomedMousePosition))
                        {
                            OpenCommentBoxGenericMenu(commentBoxView, e.mousePosition);
                            e.Use();
                            break;
                        }
                    }

                    //拖拽编辑注释区域大小
                    if (controlType == ControlType.None && (e.type != EventType.Layout || e.type != EventType.Repaint))
                    {
                        var boxEdge = commentBoxView.AtEdge(zoomedMousePosition);
                        if (boxEdge != CommentBoxView.BoxEdge.None)
                        {
                            var cursorMode =
                                (boxEdge == CommentBoxView.BoxEdge.Left || boxEdge == CommentBoxView.BoxEdge.Right)
                                    ? MouseCursor.ResizeHorizontal
                                    : MouseCursor.ResizeVertical;
                            EditorGUIUtility.AddCursorRect(guiRect, cursorMode);

                            //开始拖拽扩大
                            if (e.type == EventType.MouseDown && e.button == 0)
                            {
                                resizingCommentEdge = boxEdge;
                                resizingCommentBox = commentBoxView;
                                controlType = ControlType.ResizingCommentBox;

                                e.Use();
                            }

                            break;
                        }
                    }

                    //双击编辑注释
                    if ((controlType == ControlType.None || controlType == ControlType.DraggingCommentBox) &&
                        e.type == EventType.MouseDown && e.button == 0)
                    {
                        if (commentBoxView.Contains(zoomedMousePosition))
                        {
                            if (lastClickCommentBox == null || lastClickCommentBox != commentBoxView)
                            {
//                                Debug.Log("click once");
                                //一次点击可能是要拖拽了
                                controlType = ControlType.DraggingCommentBox;
                                lastClickCommentBox = commentBoxView;
                                draggingCommentBox = commentBoxView;
                                lastClickCommentTime = EditorApplication.timeSinceStartup;
                                e.Use();
                                break;
                            }
                            else if (lastClickCommentBox == commentBoxView)
                            {
                                var currentTime = EditorApplication.timeSinceStartup;
                                if (currentTime - lastClickCommentTime <= 0.3f)
                                {
//                                    Debug.Log("click twice");
                                    controlType = ControlType.EditingComment;
                                    lastClickCommentBox.EnableEditComment(true);
                                    editingCommentBox = lastClickCommentBox;
                                    lastClickCommentBox = null;
                                    e.Use();
                                    break;
                                }
                                else
                                {
//                                    Debug.Log("click twice failed");
                                    lastClickCommentBox = null;
                                }
                            }
                        }
                    }
                }
            }

            //右键点击面板
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                OpenGraphGenericMenu(e.mousePosition);
                e.Use();
            }

            //改变注释框大小的时候，改变鼠标图标
            if (controlType == ControlType.ResizingCommentBox)
            {
                var cursorMode =
                    (resizingCommentEdge == CommentBoxView.BoxEdge.Left ||
                     resizingCommentEdge == CommentBoxView.BoxEdge.Right)
                        ? MouseCursor.ResizeHorizontal
                        : MouseCursor.ResizeVertical;
                EditorGUIUtility.AddCursorRect(guiRect, cursorMode);
            }

            //编辑注释框大小
            if (controlType == ControlType.ResizingCommentBox && e.type == EventType.MouseDrag && e.button == 0)
            {
                if (resizingCommentBox != null)
                {
                    if (resizingCommentEdge != CommentBoxView.BoxEdge.None)
                    {
                        resizingCommentBox.Resizing(resizingCommentEdge, e.delta / data.GraphZoom);
                        e.Use();
                    }
                }
            }

            //停止编辑注释框大小
            if (controlType == ControlType.ResizingCommentBox && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;
                resizingCommentBox = null;
                resizingCommentEdge = CommentBoxView.BoxEdge.None;
                e.Use();
            }

            //拖拽注释框
            if (controlType == ControlType.DraggingCommentBox && draggingCommentBox != null &&
                e.type == EventType.MouseDrag && e.button == 0)
            {
                if (draggingCommentBox.Contains(zoomedMousePosition))
                {
                    draggingCommentBox.Drag(data.currentNodes, e.delta / data.GraphZoom);
                    e.Use();
                }
            }

            //停止拖拽注释框
            if (controlType == ControlType.DraggingCommentBox && draggingCommentBox != null &&
                e.type == EventType.MouseUp && e.button == 0)
            {
                draggingCommentBox = null;
                controlType = ControlType.None;
                e.Use();
            }

            //停止编辑注释框
            if (e.type == EventType.MouseDown)
            {
                if (controlType == ControlType.EditingComment)
                {
                    if (!editingCommentBox.Contains(zoomedMousePosition))
                    {
                        controlType = ControlType.None;
                        editingCommentBox.EnableEditComment(false);
                        editingCommentBox = null;
                        GUI.FocusControl(null);
                        e.Use();
                    }
                }
                else
                {
                    data.ClearSelectedNode();
                    GUI.FocusControl(null);
                }
            }

            //开始多选框
            if (controlType == ControlType.None && e.type == EventType.MouseDrag && isMouseLeftDown)
            {
                startMultiSelectionPos = e.mousePosition;
                controlType = ControlType.DraggingMultiSelection;
                e.Use();
            }

            //更新多选框
            if (controlType == ControlType.DraggingMultiSelection && isMouseLeftDown)
            {
                var multiSelectionRect = new Rect();
                multiSelectionRect.position = NonZoomedWindowPositionToZoomedWindowPosition(startMultiSelectionPos);
                multiSelectionRect.max = NonZoomedWindowPositionToZoomedWindowPosition(e.mousePosition);
                data.UpdateSelectedNode(multiSelectionRect);
            }

            //结束多选框
            if (controlType == ControlType.DraggingMultiSelection && !isMouseLeftDown)
            {
                controlType = ControlType.None;
                e.Use();
            }

            //排除掉鼠标移出去之后，多选框还会继续拖拽的问题
            if (!guiRect.Contains(e.mousePosition) && e.type != EventType.Layout && e.type != EventType.Repaint)
            {
                if (controlType == ControlType.DraggingMultiSelection)
                {
                    controlType = ControlType.None;
                    e.Use();
                }
            }
        }

    #region 绘制

        private void DrawNodes()
        {
            if (data?.currentNodes == null)
            {
                return;
            }

            var nodeViewList = data.currentNodes;
            foreach (var nodeView in nodeViewList)
            {
                nodeView.DrawNodeGUI();
            }
        }

        private void BeginZoom()
        {
            GUI.EndClip();

            var graphZoom = data.GraphZoom;
            GUIUtility.ScaleAroundPivot(Vector2.one * graphZoom, guiRect.size * 0.5f);

            GUI.BeginClip(new Rect(-(guiRect.width / graphZoom - guiRect.width) * 0.5f,
                -((guiRect.height / graphZoom - guiRect.height) * 0.5f) + 21 / graphZoom,
                guiRect.width / graphZoom,
                guiRect.height / graphZoom));
        }

        private void EndZoom()
        {
            var graphZoom = data.GraphZoom;
            GUIUtility.ScaleAroundPivot(Vector2.one / graphZoom, guiRect.size * 0.5f);

            var offset = new Vector3((guiRect.width / graphZoom - guiRect.width) * 0.5f,
                (guiRect.height / graphZoom - guiRect.height) * 0.5f - 21 / graphZoom + 21,
                0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
        }

        private bool isLayoutDone;

        private void DrawTipBox()
        {
            if (currentHoverNodeView != null || currentHoverPortView != null)
            {
                if (EditorApplication.timeSinceStartup - startHoverTime < 0.3f)
                {
                    return;
                }

                var description = string.Empty;

                if (currentHoverNodeView != null)
                {
                    var typeName = currentHoverNodeView.ReflectionInfo.Type.Name;
                    var nodeDescription = currentHoverNodeView.ReflectionInfo.NodeAttribute.nodeDescription;
                    description = $"脚本: {typeName} \n {nodeDescription}";
                }
                else if (currentHoverPortView != null)
                {
                    description = currentHoverPortView.PortDescription;
                }

                if (string.IsNullOrEmpty(description))
                {
                    return;
                }

                var e = Event.current;

                if (e.type == EventType.Layout)
                {
                    isLayoutDone = true;
                }

                if ((e.type == EventType.Layout || e.type == EventType.Repaint) && isLayoutDone)
                {
                    var tipStyle = Utility.GetGuiStyle("TipLabel");
                    var tipRect = GUILayoutUtility.GetRect(new GUIContent(description),
                        tipStyle,
                        GUILayout.MinWidth(200),
                        GUILayout.MaxWidth(300));
                    var mousePosition = e.mousePosition;
                    GUI.Box(new Rect(mousePosition + new Vector2(10f, 10f), tipRect.size), description, tipStyle);
                }

                if (e.type != EventType.Layout)
                {
                    isLayoutDone = false;
                }
            }
        }

        private void DrawCommentBox()
        {
            if (controlType == ControlType.DraggingNewCommentBox)
            {
                CommentBoxView.DrawDraggingCommentBox(this,
                    startDraggingCommentBoxGraphPosition,
                    endDraggingCommentBoxGraphPosition);
            }

            if (data.commentBoxViews.Count > 0)
            {
                foreach (var commentBoxView in data.commentBoxViews)
                {
                    commentBoxView.Draw();
                }
            }
        }

        private void DrawConnectionLine()
        {
            if (draggingLineView != null)
            {
                draggingLineView.DrawDragLine();
            }

            var connectionLineViewList = data.connectionLines;
            foreach (var connectionLineView in connectionLineViewList)
            {
                connectionLineView.DrawConnectionLine();
            }
        }

        /// <summary>
        /// 绘制多选框
        /// </summary>
        /// <param name="e"></param>
        private void DrawMultiSelection(Event e)
        {
            if (controlType == ControlType.DraggingMultiSelection && isMouseLeftDown)
            {
                var endMultiSelectionPos = e.mousePosition;

                var selectionRect = new Rect
                {
                    position = startMultiSelectionPos,
                    max = endMultiSelectionPos
                };

                Handles.BeginGUI();
                Handles.DrawSolidRectangleWithOutline(selectionRect, Color.white * .1f, new Color(1, 1, 1, 1));
                Handles.EndGUI();
            }
        }

    #endregion

    #region 节点处理

        /// <summary>
        /// 编辑器右键菜单
        /// </summary>
        private void OpenGraphGenericMenu(Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("添加流程节点"), false, OpenAddNodeWindow, mousePosition);

            if (data.currentNodes.Count > 0)
            {
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("保存"), false, SaveGraph, mousePosition);
            }
            // else if (!string.IsNullOrEmpty(data.graphName) && data.nodeList.Count > 0)
            // {
            //     genericMenu.AddSeparator("");
            //     genericMenu.AddItem(new GUIContent("保存当前行为图"), false, SaveCurrentGraph, mousePosition);
            // }

            // //新建行为图，重置所有数据
            // if (!string.IsNullOrEmpty(data.graphName))
            // {
            //     genericMenu.AddSeparator("");
            //     genericMenu.AddItem(new GUIContent("新建行为图"), false, Reset);
            // }

            // genericMenu.AddSeparator("");
            // genericMenu.AddItem(new GUIContent("载入行为图"), false, OpenGraphLoadWindow, mousePosition);

            // genericMenu.AddSeparator("");
            // genericMenu.AddItem(new GUIContent("删除行为图"), false, OpenDeleteGraphWindow, mousePosition);

            genericMenu.ShowAsContext();
        }

        private void OpenAddNodeWindow(object mousePositionObject)
        {
            var mousePosition = (Vector2) mousePositionObject;

            try
            {
                PopupWindow.Show(new Rect(mousePosition, new Vector2(0, 10)),
                    new AddNodePopupWindow(type =>
                    {
                        //重置tip显示
                        currentHoverNodeView = null;
                        currentHoverPortView = null;
                        startHoverTime = EditorApplication.timeSinceStartup;

                        var isEntranceNode = data.currentNodes.Count == 0;

                        var newId = data.GetNewNodeId();
                        var reflectionInfo = new NodeReflectionInfo(type);
                        var nodeEditorView = new NodeEditorView(WindowPositionToGraphPosition(mousePosition),
                            this,
                            newId,
                            isEntranceNode,
                            reflectionInfo);
                        data.currentNodes.Add(nodeEditorView);

                        if (isEntranceNode)
                        {
                            data.entranceNode = nodeEditorView;
                        }
                    }));
            }
            catch
            {
            }
        }

        private void SaveGraph(object mousePositionObject)
        {
            PersistenceTool.SaveGraph(data);
            ProcedureRuntimeCodeGenerator.GenerateCode(data);
        }

        private void LoadGraph()
        {
            Reset();
            data = PersistenceTool.LoadGraph(this);

            //做一些载入完成的初始化工作
            data.OnLoadFinish();
        }

        private void OpenNodeGenericMenu(NodeEditorView node, Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("删除该节点"),
                false,
                () =>
                {
                    if (node == null)
                    {
                        return;
                    }

                    data.DeleteNode(node);
                });
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("将该节点设置为入口"),
                false,
                () =>
                {
                    if (node == null)
                    {
                        return;
                    }

                    data.SetEntranceNode(node);
                });

            genericMenu.ShowAsContext();
        }

        private void OpenConnectionLineGenericMenu(ConnectionLineView connectionLineView, Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("删除连线"),
                false,
                () =>
                {
                    if (connectionLineView == null)
                    {
                        return;
                    }

                    data.DeleteConnectionLine(connectionLineView);
                });

            genericMenu.ShowAsContext();
        }

        private void OpenCommentBoxGenericMenu(CommentBoxView commentBoxView, Vector2 mousePosition)
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("删除注释框"),
                false,
                () =>
                {
                    if (commentBoxView == null)
                    {
                        return;
                    }

                    data.DeleteCommentBox(commentBoxView);
                });

            genericMenu.ShowAsContext();
        }

    #endregion

    #region 计算

        public Vector2 NonZoomedWindowPositionToZoomedWindowPosition(Vector2 nonZoomedPos)
        {
            var graphZoom = data?.GraphZoom ?? 1f;
            return guiRect.position + (nonZoomedPos - guiRect.position) / graphZoom;
        }

        public Vector2 ZoomedWindowPositionToNonZoomedWindowPosition(Vector2 nonZoomedPos)
        {
            var graphZoom = data?.GraphZoom ?? 1f;
            return (nonZoomedPos - guiRect.position) * graphZoom + guiRect.position;
        }

        public Vector2 GraphPositionToWindowPosition(Vector2 graphPosition)
        {
            var graphZoom = data?.GraphZoom ?? 1f;
            var graphOffset = data?.GraphOffset ?? Vector2.zero;
            return guiRect.center / graphZoom + graphPosition + graphOffset;
        }

        public Vector2 WindowPositionToGraphPosition(Vector2 windowPosition)
        {
            var graphZoom = data?.GraphZoom ?? 1f;
            var graphOffset = data?.GraphOffset ?? Vector2.zero;
            return (windowPosition - guiRect.center - graphOffset * graphZoom) / graphZoom;
        }

        public float NonZoomedSizeToZoomedSize(float nonZoomedSize)
        {
            var graphZoom = data?.GraphZoom ?? 1f;
            return nonZoomedSize / graphZoom;
        }

        public Vector2 NonZoomedSizeToZoomedSize(Vector2 nonZoomedSize)
        {
            return new Vector2(NonZoomedSizeToZoomedSize(nonZoomedSize.x), NonZoomedSizeToZoomedSize(nonZoomedSize.y));
        }

    #endregion
    }
}