using UnityEditor;
using UnityEngine;

namespace VisualProcedure.Editor.Scripts
{
	public static class DrawHelper 
	{
		public static void DrawGrid(float gridSize,float opacity,Rect gridRect,Vector2 offset,float zoom)
		{
			Handles.BeginGUI();
			var originColor = Handles.color;
            Handles.color = Color.white * opacity;

            var gridOffset = new Vector2(offset.x % gridSize, offset.y % gridSize);

            var offsetCenter = gridRect.center + gridOffset;

            //draw to right
            var index = 0;
            while (true)
            {
                var xPos = offsetCenter.x + index * gridSize;
                xPos = gridRect.center.x + (xPos - gridRect.center.x) * zoom;
                if (xPos < gridRect.xMin || xPos > gridRect.xMax)
                {
                    break;
                }

                Handles.DrawLine(new Vector3(xPos, gridRect.yMin, 0), new Vector3(xPos, gridRect.yMax, 0));
                index++;
            }

            //draw to left
            index = 1;
            while (true)
            {
                var xPos = offsetCenter.x - index * gridSize;
                xPos = gridRect.center.x + (xPos - gridRect.center.x) * zoom;
                if (xPos < gridRect.xMin || xPos > gridRect.xMax)
                {
                    break;
                }

                Handles.DrawLine(new Vector3(xPos, gridRect.yMin, 0), new Vector3(xPos, gridRect.yMax, 0));
                index++;
            }

            //draw to up
            index = 0;
            while (true)
            {
                var yPos = offsetCenter.y - index * gridSize;
                yPos = gridRect.center.y + (yPos - gridRect.center.y) * zoom;
                if (yPos < gridRect.yMin || yPos > gridRect.yMax)
                {
                    break;
                }

                Handles.DrawLine(new Vector3(gridRect.xMin, yPos, 0), new Vector3(gridRect.xMax, yPos, 0));
                index++;
            }

            //draw to down
            index = 1;
            while (true)
            {
                var yPos = offsetCenter.y + index * gridSize;
                yPos = gridRect.center.y + (yPos - gridRect.center.y) * zoom;
                if (yPos < gridRect.yMin || yPos > gridRect.yMax)
                {
                    break;
                }

                Handles.DrawLine(new Vector3(gridRect.xMin, yPos, 0), new Vector3(gridRect.xMax, yPos, 0));
                index++;
            }

			Handles.color = originColor;
            Handles.EndGUI();
		}

		public static void DrawLine(Vector2 startPosition, Vector2 endPosition)
		{
			DrawLine(startPosition,endPosition,Color.white);
		}
		
	    public static void DrawLine(Vector2 startPosition, Vector2 endPosition, Color color)
	    {
		    Handles.BeginGUI();

		    var originColor = Handles.color;
	        Handles.color = color;
	        
	        Handles.DrawLine(startPosition,endPosition);

		    Handles.color = originColor;
		    
		    Handles.EndGUI();
	    }

	}
}

