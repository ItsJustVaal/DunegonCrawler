using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeTypeListSO roomNodeTypeList;
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")] 

    private static void OpenWindow(){
        GetWindow<RoomNodeGraphEditor>(title: "Room Node Graph Editor");
    }

    private void OnEnable() {

        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(left: nodePadding,right: nodePadding,top: nodePadding,bottom: nodePadding);
        roomNodeStyle.border = new RectOffset(left: nodeBorder,right: nodeBorder,top: nodeBorder,bottom: nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        
    }
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line){
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null){
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;
            
            return true;
        }
        return false;
    }



    private void OnGUI() {
        
        if (currentRoomNodeGraph != null){
            ProcessEvents(Event.current);

            DrawRoomNodes();
        }
        
        if (GUI.changed){
            Repaint();
        }
    }



    private void ProcessEvents(Event currentEvent){
        
        ProcessRoomNodeGraphEvents(currentEvent);
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent){

        switch (currentEvent.type){

            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            
            default:
                break;
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent){

        if (currentEvent.button == 1){

            ShowContextMenu(currentEvent.mousePosition);
        }
    }

    private void ShowContextMenu(Vector2 mousePosition){

        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);

        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject){

        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }
    
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType){
        
        Vector2 mousePosition = (Vector2)mousePositionObject;

        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);

        AssetDatabase.SaveAssets();


    }

    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO roomNode in currentRoomNodeGraph.roomNodeList){
            roomNode.Draw(roomNodeStyle);
        }
        GUI.changed = true;
    }
    
}
