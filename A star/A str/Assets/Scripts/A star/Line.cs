// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public struct Line{
//     float gradient;
//     float y_intercept;

//     const float verticalLineGradient = 1e5f;
//     float gradientPerpendicular;

//     public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine){
//         float dx = pointOnLine.x - pointPerpendicularToLine.x;
//         float dy = pointOnLine.y - pointPerpendicularToLine.y;

//         gradientPerpendicular = dy/dx;
//         if(dx==0)
//             gradientPerpendicular = verticalLineGradient;
//     }
// }
