using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ImageWithUV2 : Image
    {
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);
            UIVertex uiVertex = default;
            toFill.PopulateUIVertex(ref uiVertex, 0);
            uiVertex.uv1 = new Vector2(0f, 1f);
            toFill.SetUIVertex(uiVertex, 0);
            
            toFill.PopulateUIVertex(ref uiVertex, 1);
            uiVertex.uv1 = new Vector2(0f, 0f);
            toFill.SetUIVertex(uiVertex, 1);
            
            toFill.PopulateUIVertex(ref uiVertex, 2);
            uiVertex.uv1 = new Vector2(1f, 0f);
            toFill.SetUIVertex(uiVertex, 2);
            
            toFill.PopulateUIVertex(ref uiVertex, 3);
            uiVertex.uv1 = new Vector2(1f, 1f);
            toFill.SetUIVertex(uiVertex, 3);
        }
    }
}