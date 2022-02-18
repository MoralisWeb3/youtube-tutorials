using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal static class TextContent
    {
        // Undo
        public static string setMode = "Set Mode";
        public static string setTool = "Set Tool";
        public static string pasteData = "Paste Data";
        public static string generateGeometry = "Generate Geometry";
        public static string generateWeights = "Generate Weights";
        public static string normalizeWeights = "Normalize Weights";
        public static string clearWeights = "Clear Weights";
        public static string restorePose = "Restore Pose";
        public static string selection = "Selection";
        public static string clearSelection = "Clear Selection";
        public static string editWeights = "Edit Weights";
        public static string boneName = "Bone Name";
        public static string boneDepth = "Bone Depth";
        public static string rotateBone = "Rotate Bone";
        public static string moveBone = "Move Bone";
        public static string freeMoveBone = "Free Move Bone";
        public static string moveJoint = "Move Joint";
        public static string moveEndPoint = "Move End Point";
        public static string boneLength = "Bone Length";
        public static string createBone = "Create Bone";
        public static string splitBone = "Split Bone";
        public static string removeBone = "Remove Bone";
        public static string moveVertices = "Move Vertices";
        public static string createVertex = "Create Vertex";
        public static string createEdge = "Create Edge";
        public static string splitEdge = "Split Edge";
        public static string removeEdge = "Remove Edge";
        public static string removeVertices = "Remove Vertices";
        public static string selectionChange = "Selection Change";
        public static string boneVisibility = "Bone Visibility";
        public static string setParentBone = "Set Parent Bone";
        public static string visibilityChange = "VisibilityChange";
        public static string boneSelection = "Bone Selection";
        public static string expandBones = "Expand Bones";
        public static string meshVisibility = "Mesh Visibility";
        public static string meshOpacity = "Mesh Opacity";
        public static string opacityChange = "Opacity Change";

        // Tooltips
        public static string visibilityIconTooltip = "Visibility tool";
        public static string characterIconTooltip = "Restore bind pose";
        public static string spriteSheetIconTooltip = "Switch between Sprite sheet and Character mode";
        public static string copyTooltip = "Copy";
        public static string pasteTooltip = "Paste";
        public static string onTooltip = "On";
        public static string offTooltip = "Off";

        // Horizontal tool bar button txt
        public static string visibilityIconText = "Visibility";
        public static string characterIconText = "Reset Pose";
        public static string spriteSheetIconText = "Sprite Sheet";
        public static string copyText = "Copy";
        public static string pasteText = "Paste";

        // Settings
        public static string selectedOutlineColor = "Selected Outline Color";
        public static string spriteOutlineSize = "Sprite Outline Size";
        public static string boneOutlineSize = "Bone Outline Size";

        // Sprite Library
        public static string convertGroupToCategory = "Convert Group to Category";
        public static string newTrailingDots = "New...";
        public static string removeEmptyCategory = "Remove Empty Category";
        public static string convertLayerToCategory = "Convert Layer to Category";
        public static string clearAllCategory = "Clear All Category";
        public static string spriteCategoryChanged = "Sprite Category Changed";
        public static string spriteCategoryIndexChanged = "Sprite Category Index Changed";
        public static string category = "Category";
        public static string label = "Label";

        // Other
        public static string generatingGeometry = "Generating Geometry";
        public static string generatingWeights = "Generating Weights";
        public static string vertexWeight = "Vertex Weight";
        public static string vertexWeightToolTip = "Adjust bone weights for selected vertex";
        public static string bone = "Bone";
        public static string depth = "Depth";
        public static string sprite = "Sprite";
        public static string spriteVisibility = "SpriteVisibility";
        public static string name = "Name";
        public static string none = "None";
        public static string size = "Size";
        public static string visibilityTab = "Visibility Tab";
        public static string addBoneInfluence = "Add Bone Influence";
        public static string removeBoneInfluence = "Remove Bone Influence";
        public static string reorderBoneInfluence = "Reorder Bone Influence";
        public static string noSpriteSelected = "No sprite selected";
        public static string weightSlider = "Weight Slider";
        public static string weightBrush = "Weight Brush";
        public static string generateAll = "Generate All";
        public static string generate = "Generate";
        public static string smoothMeshError = "Generated mesh could not be smoothed. Please try using different parameters";
        public static string copyError1 = "Unable to convert copied data to Skinning Module paste data.";
        public static string copyError2 = "There is no copied data to paste.";
        public static string copyError3 = "Target has a different number of Sprites ({0}) compared to copied source ({1}) for Mesh copy.";
        public static string mode = "Mode";
        public static string modeTooltip = "Different operation mode for weight adjustment";
        public static string boneToolTip = "The bone that is affecting";
    }
}
