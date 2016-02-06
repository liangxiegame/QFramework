// Created by Daniele Giardini - 2011 - Holoville - http://www.holoville.com

using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor undo manager.
/// To use it:
/// <list type="number">
/// <item>
/// <description>Store an instance in the related Editor Class (instantiate it inside the <code>OnEnable</code> method).</description>
/// </item>
/// <item>
/// <description>Call <code>undoManagerInstance.CheckUndo()</code> BEFORE the first UnityGUI call in <code>OnInspectorGUI</code>.</description>
/// </item>
/// <item>
/// <description>Call <code>undoManagerInstance.CheckDirty()</code> AFTER the last UnityGUI call in <code>OnInspectorGUI</code>.</description>
/// </item>
/// </list>
/// </summary>
/// 
public class FXMakerUndoManager
{
    private     Object              defTarget;
    private     string              defName;
    private     bool                autoSetDirty;
    private     bool                listeningForGuiChanges;
    private     bool                isMouseDown;
    private     Object              waitingToRecordPrefab;	// If different than NULL indicates the prefab instance that will need to record its state as soon as the mouse is released. 
    
    public FXMakerUndoManager( Object p_target, string p_name ) : this( p_target, p_name, true ) {}
    public FXMakerUndoManager( Object p_target, string p_name, bool p_autoSetDirty )
    {
        defTarget = p_target;
        defName = p_name;
        autoSetDirty = p_autoSetDirty;
    }
    
    // ===================================================================================
    public void CheckUndo() { CheckUndo( defTarget, defName ); }
    public void CheckUndo( Object p_target ) { CheckUndo( p_target, defName ); }
    public void CheckUndo( Object p_target, string p_name )
    {
        Event e = Event.current;
        
        if ( waitingToRecordPrefab != null ) {
            // Record eventual prefab instance modification.
            // TODO Avoid recording if nothing changed (no harm in doing so, but it would be nicer).
            switch ( e.type ) {
            case EventType.MouseDown :
            case EventType.MouseUp :
            case EventType.KeyDown :
            case EventType.KeyUp :
                PrefabUtility.RecordPrefabInstancePropertyModifications( waitingToRecordPrefab );
                break;
            }
        }
        
        if ( ( e.type == EventType.MouseDown && e.button == 0 ) || ( e.type == EventType.KeyUp && e.keyCode == KeyCode.Tab ) ) {
            // When the LMB is pressed or the TAB key is released,
            // store a snapshot, but don't register it as an undo
            // (so that if nothing changes we avoid storing a useless undo).
            Undo.SetSnapshotTarget( p_target, p_name );
            Undo.CreateSnapshot();
            Undo.ClearSnapshotTarget(); // Not sure if this is necessary.
            listeningForGuiChanges = true;
        }
    }
    
    public bool CheckDirty() { return CheckDirty( defTarget, defName ); }
    public bool CheckDirty( Object p_target ) { return CheckDirty( p_target, defName ); }
    public bool CheckDirty( Object p_target, string p_name )
    {
        if ( listeningForGuiChanges && GUI.changed ) {
            // Some GUI value changed after pressing the mouse
            // or releasing the TAB key.
            // Register the previous snapshot as a valid undo.
            SetDirty( p_target, p_name );
            return true;
        }
        return false;
    }
    
    public void ForceDirty() { ForceDirty( defTarget, defName ); }
    public void ForceDirty( Object p_target ) { ForceDirty( p_target, defName ); }
    public void ForceDirty( Object p_target, string p_name )
    {
        if ( !listeningForGuiChanges ) {
            // Create a new snapshot.
            Undo.SetSnapshotTarget( p_target, p_name );
            Undo.CreateSnapshot();
            Undo.ClearSnapshotTarget();
        }
        SetDirty( p_target, p_name );
    }
    
    // ===================================================================================
    // PRIVATE METHODS -------------------------------------------------------------------
    
    private void SetDirty( Object p_target, string p_name )
    {
        Undo.SetSnapshotTarget( p_target, p_name );
        Undo.RegisterSnapshot();
        Undo.ClearSnapshotTarget(); // Not sure if this is necessary.
        if ( autoSetDirty )     EditorUtility.SetDirty( p_target );
        listeningForGuiChanges = false;
        
        if ( CheckTargetIsPrefabInstance( p_target ) ) {
            // Prefab instance: record immediately and also wait for value to be changed and than re-record it
            // (otherwise prefab instances are not updated correctly when using Custom Inspectors).
            PrefabUtility.RecordPrefabInstancePropertyModifications( p_target );
            waitingToRecordPrefab = p_target;
        } else {
            waitingToRecordPrefab = null;
        }
    }
    
    private bool CheckTargetIsPrefabInstance( Object p_target )
    {
        return ( PrefabUtility.GetPrefabType( p_target ) == PrefabType.PrefabInstance );
    }
}
