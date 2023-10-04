/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
#define genAA

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public struct Line
    {
        public Vector2 startPosition;
        public Vector2 endPosition;
        public LineEndsEnum lineEndsEnum;
        public LineAlignEnum lineAlign;
        public bool lineAlignWithBlur;
        public bool preserveLineWidth;

        public float sidesBlur;
        public float endsBlur;
        public float thickness;

        private List<UIVertex[]> verts;

        public Line(Vector2 startPos, Vector2 endPos, float thickness, float sidesBlur, float endsBlur, Color color, LineAlignEnum lineAlign = LineAlignEnum.Middle, LineEndsEnum lineEndsEnum = LineEndsEnum.None, bool lineAlignWithBlur = false, bool preserveLineWidth = false)
        {
            this.startPosition = startPos;
            this.endPosition = endPos;
            this.sidesBlur = sidesBlur;
            this.endsBlur = endsBlur;
            this.thickness = thickness;
            this.lineAlign = lineAlign;
            this.lineEndsEnum = lineEndsEnum;
            this.lineAlignWithBlur = lineAlignWithBlur;
            this.preserveLineWidth = preserveLineWidth;

            this.verts = new List<UIVertex[]>(512); // initial alloc
            GenerateVerts(color);
        }

        public float calculatedThickness
        {
            get
            {
                return Vector2.Distance(corner1, corner2);
            }
        }

        public float calcualtedThicknessRatio
        {
            get
            {
                return calculatedThickness / thickness;
            }
        }

        public Vector2 corner1
        {
            get
            {
                return verts[0][0].position;
            }

            set
            {
                verts[0][0].position = value;
                if (verts.Count < 2)
                {
                    return;
                }
                verts[1][0].position = value;
                verts[3][1].position = value;
            }
        }

        public Vector2 corner2
        {
            get
            {
                return verts[0][1].position;
            }

            set
            {
                verts[0][1].position = value;
                if (verts.Count < 2)
                {
                    return;
                }
                verts[4][0].position = value;
                verts[1][1].position = value;
            }
        }

        public Vector2 corner3
        {
            get
            {
                return verts[0][2].position;
            }

            set
            {
                verts[0][2].position = value;
                if (verts.Count < 2)
                {
                    return;
                }
                verts[2][0].position = value;
                verts[4][1].position = value;
            }
        }

        public Vector2 corner4
        {
            get
            {
                return verts[0][3].position;
            }

            set
            {
                verts[0][3].position = value;
                if (verts.Count < 2)
                {
                    return;
                }
                verts[3][0].position = value;
                verts[2][1].position = value;
            }
        }

        public Vector2 blurCorner4
        {
            get
            {
                return verts[3][3].position;

            }

            set
            {
                verts[3][3].position = value;
                verts[2][2].position = value;
            }
        }

        public Vector2 blurCorner3
        {
            get
            {
                return verts[2][3].position;
            }

            set
            {
                verts[2][3].position = value;
                verts[4][2].position = value;
            }
        }


        public Vector2 blurCorner2
        {
            get
            {
                return verts[4][3].position;

            }

            set
            {
                verts[4][3].position = value;
                verts[1][2].position = value;
            }
        }


        public Vector2 blurCorner1
        {
            get
            {
                return verts[1][3].position;

            }

            set
            {
                verts[1][3].position = value;
                verts[3][2].position = value;
            }
        }

        public void Move(Vector2 move)
        {
            corner1 += move;
            corner2 += move;
            corner3 += move;
            corner4 += move;

            blurCorner1 += move;
            blurCorner2 += move;
            blurCorner3 += move;
            blurCorner4 += move;
        }

        public List<UIVertex[]> GetVertices()
        {
            return this.verts;
        }

        private void GenerateVerts(Color color)
        {
            UIVertex[] v = new UIVertex[4];

            Color alpha = Color.clear;

            var normal = (endPosition - startPosition).normalized;
            var tangent = new Vector2(normal.y, -normal.x).normalized;

            v = new UIVertex[4];

            #region line

            v[0].position = startPosition + tangent * thickness * 0.5f;
            v[0].color = color;

            v[1].position = startPosition - tangent * thickness * 0.5f;
            v[1].color = color;

            v[2].position = endPosition - tangent * thickness * 0.5f;
            v[2].color = color;

            v[3].position = endPosition + tangent * thickness * 0.5f;
            v[3].color = color;

            verts.Add(v);

            #endregion

            /*
             * edge pairs
             * 
             * bottom 0,1
             * top 2, 3
             * 
             * sideA 0, 3
             * sideB 1, 2
             * 
             */

            int[] pairsA = { 0, 2, 3, 1 };
            int[] pairsB = { 1, 3, 0, 2 };

            #region BLUR EDGES

            for (int i = 0; i < 4; i++)
            {
                var v2 = new UIVertex[4];

                var blur = i < 2 ? endsBlur : sidesBlur;

                int a = pairsA[i];
                int b = pairsB[i];

                v2[0] = v[a];
                v2[1] = v[b];

                Vector2 edgeNormal;

                edgeNormal = (v2[0].position - v2[1].position).normalized;

                var edgeTangent = new Vector2(edgeNormal.y, -edgeNormal.x);

                v2[2] = v[b];
                v2[3] = v[a];

                v2[2].position.x = v2[1].position.x + edgeTangent.x * blur;
                v2[2].position.y = v2[1].position.y + edgeTangent.y * blur;

                v2[3].position.x = v2[0].position.x + edgeTangent.x * blur;
                v2[3].position.y = v2[0].position.y + edgeTangent.y * blur;

                v2[2].color = alpha;
                v2[3].color = alpha;

                verts.Add(v2);
            }
            #endregion

            #region BLUR JOINING
            /*
             * first index
             * 4 = left
             * 3 = right
             * 2 = top
             * 1 = down
             */

            verts[3][3].position.x += normal.x * endsBlur;
            verts[3][3].position.y += normal.y * endsBlur;
            verts[2][2].position.x += tangent.x * sidesBlur;
            verts[2][2].position.y += tangent.y * sidesBlur;

            verts[2][3].position.x += -sidesBlur * tangent.x;
            verts[2][3].position.y += -sidesBlur * tangent.y;
            verts[4][2].position.x += endsBlur * normal.x;
            verts[4][2].position.y += endsBlur * normal.y;

            verts[4][3].position.x += normal.x * -endsBlur;
            verts[4][3].position.y += normal.y * -endsBlur;
            verts[1][2].position.x += tangent.x * -sidesBlur;
            verts[1][2].position.y += tangent.y * -sidesBlur;

            verts[1][3].position.x += tangent.x * sidesBlur;
            verts[1][3].position.y += tangent.y * sidesBlur;
            verts[3][2].position.x += -endsBlur * normal.x;
            verts[3][2].position.y += -endsBlur * normal.y;

            #endregion

            #region HORIZONTAL or VERTICAL corners adjustments

            Vector2 intersection;
            Vector2 midPoint;
            float lineMult = 100f;

            if (preserveLineWidth)
            {
                switch (lineEndsEnum)
                {
                    case LineEndsEnum.Horizontal:
                        midPoint = (corner1 + corner2) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner1 + normal * lineMult, corner1 + normal * -lineMult, out intersection);
                        corner1 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner2 + normal * lineMult, corner2 + normal * -lineMult, out intersection);
                        corner2 = intersection;

                        midPoint = (blurCorner1 + blurCorner2) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner1 + normal * lineMult, blurCorner1 + normal * -lineMult, out intersection);
                        blurCorner1 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner2 + normal * lineMult, blurCorner2 + normal * -lineMult, out intersection);
                        blurCorner2 = intersection;

                        midPoint = (corner3 + corner4) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner3 + normal * lineMult, corner3 + normal * -lineMult, out intersection);
                        corner3 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner4 + normal * lineMult, corner4 + normal * -lineMult, out intersection);
                        corner4 = intersection;

                        midPoint = (blurCorner3 + blurCorner4) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner3 + normal * lineMult, blurCorner3 + normal * -lineMult, out intersection);
                        blurCorner3 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner4 + normal * lineMult, blurCorner4 + normal * -lineMult, out intersection);
                        blurCorner4 = intersection;
                        break;
                    case LineEndsEnum.Vertical:
                        midPoint = (corner1 + corner2) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner1 + normal * lineMult, corner1 + normal * -lineMult, out intersection);
                        corner1 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner2 + normal * lineMult, corner2 + normal * -lineMult, out intersection);
                        corner2 = intersection;

                        midPoint = (blurCorner1 + blurCorner2) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner1 + normal * lineMult, blurCorner1 + normal * -lineMult, out intersection);
                        blurCorner1 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner2 + normal * lineMult, blurCorner2 + normal * -lineMult, out intersection);
                        blurCorner2 = intersection;

                        midPoint = (corner3 + corner4) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner3 + normal * lineMult, corner3 + normal * -lineMult, out intersection);
                        corner3 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner4 + normal * lineMult, corner4 + normal * -lineMult, out intersection);
                        corner4 = intersection;

                        midPoint = (blurCorner3 + blurCorner4) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner3 + normal * lineMult, blurCorner3 + normal * -lineMult, out intersection);
                        blurCorner3 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner4 + normal * lineMult, blurCorner4 + normal * -lineMult, out intersection);
                        blurCorner4 = intersection;
                        break;
                }
            }
            else
            {
                switch (lineEndsEnum)
                {
                    case LineEndsEnum.Horizontal:
                        midPoint = (corner1 + corner2) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner1 + Vector2.up * lineMult, corner1 + Vector2.down * lineMult, out intersection);
                        corner1 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner2 + Vector2.up * lineMult, corner2 + Vector2.down * lineMult, out intersection);
                        corner2 = intersection;

                        midPoint = (blurCorner1 + blurCorner2) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner1 + Vector2.up * lineMult, blurCorner1 + Vector2.down * lineMult, out intersection);
                        blurCorner1 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner2 + Vector2.up * lineMult, blurCorner2 + Vector2.down * lineMult, out intersection);
                        blurCorner2 = intersection;

                        midPoint = (corner3 + corner4) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner3 + Vector2.up * lineMult, corner3 + Vector2.down * lineMult, out intersection);
                        corner3 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, corner4 + Vector2.up * lineMult, corner4 + Vector2.down * lineMult, out intersection);
                        corner4 = intersection;

                        midPoint = (blurCorner3 + blurCorner4) / 2f;

                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner3 + Vector2.up * lineMult, blurCorner3 + Vector2.down * lineMult, out intersection);
                        blurCorner3 = intersection;
                        LineIntersection(midPoint + Vector2.left * lineMult, midPoint + Vector2.right * lineMult, blurCorner4 + Vector2.up * lineMult, blurCorner4 + Vector2.down * lineMult, out intersection);
                        blurCorner4 = intersection;
                        break;
                    case LineEndsEnum.Vertical:
                        midPoint = (corner1 + corner2) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner1 + Vector2.right * lineMult, corner1 + Vector2.left * lineMult, out intersection);
                        corner1 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner2 + Vector2.right * lineMult, corner2 + Vector2.left * lineMult, out intersection);
                        corner2 = intersection;

                        midPoint = (blurCorner1 + blurCorner2) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner1 + Vector2.right * lineMult, blurCorner1 + Vector2.left * lineMult, out intersection);
                        blurCorner1 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner2 + Vector2.right * lineMult, blurCorner2 + Vector2.left * lineMult, out intersection);
                        blurCorner2 = intersection;

                        midPoint = (corner3 + corner4) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner3 + Vector2.right * lineMult, corner3 + Vector2.left * lineMult, out intersection);
                        corner3 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, corner4 + Vector2.right * lineMult, corner4 + Vector2.left * lineMult, out intersection);
                        corner4 = intersection;

                        midPoint = (blurCorner3 + blurCorner4) / 2f;

                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner3 + Vector2.right * lineMult, blurCorner3 + Vector2.left * lineMult, out intersection);
                        blurCorner3 = intersection;
                        LineIntersection(midPoint + Vector2.up * lineMult, midPoint + Vector2.down * lineMult, blurCorner4 + Vector2.right * lineMult, blurCorner4 + Vector2.left * lineMult, out intersection);
                        blurCorner4 = intersection;
                        break;
                }
            }

            #endregion

            #region ALIGN
            Vector2 offset = Vector2.zero;

            switch (lineEndsEnum)
            {
                case LineEndsEnum.None:
                    switch (lineAlign)
                    {
                        case LineAlignEnum.Left:
                            offset = (lineAlignWithBlur ? blurCorner2 : corner2) - startPosition;
                            break;
                        case LineAlignEnum.Middle:
                            offset = (lineAlignWithBlur ? (blurCorner1 + blurCorner2) / 2f : (corner1 + corner2) / 2f) - startPosition;
                            break;
                        case LineAlignEnum.Right:
                            offset = (lineAlignWithBlur ? blurCorner1 : corner1) - startPosition;
                            break;
                        default:
                            offset = Vector3.zero;
                            break;
                    }
                    break;
                case LineEndsEnum.Horizontal:
                    switch (lineAlign)
                    {
                        case LineAlignEnum.Left:
                            if (endPosition.y < startPosition.y)
                            {
                                offset = (lineAlignWithBlur ? blurCorner1 : corner1) - startPosition;
                            }
                            else
                            {
                                offset = (lineAlignWithBlur ? blurCorner2 : corner2) - startPosition;
                            }
                            break;
                        case LineAlignEnum.Middle:
                            offset = (lineAlignWithBlur ? (blurCorner1 + blurCorner2) / 2f : (corner1 + corner2) / 2f) - startPosition;
                            break;
                        case LineAlignEnum.Right:
                            if (endPosition.y < startPosition.y)
                            {
                                offset = (lineAlignWithBlur ? blurCorner2 : corner2) - startPosition;
                            }
                            else
                            {
                                offset = (lineAlignWithBlur ? blurCorner1 : corner1) - startPosition;
                            }
                            break;
                        default:
                            offset = Vector3.zero;
                            break;
                    }
                    break;
                case LineEndsEnum.Vertical:
                    switch (lineAlign)
                    {
                        case LineAlignEnum.Left:
                            if (endPosition.x < startPosition.y)
                            {
                                offset = (lineAlignWithBlur ? blurCorner1 : corner1) - startPosition;
                            }
                            else
                            {
                                offset = (lineAlignWithBlur ? blurCorner2 : corner2) - startPosition;
                            }

                            break;
                        case LineAlignEnum.Middle:
                            offset = (lineAlignWithBlur ? (blurCorner1 + blurCorner2) / 2f : (corner1 + corner2) / 2f) - startPosition;
                            break;
                        case LineAlignEnum.Right:
                            if (endPosition.x < startPosition.y)
                            {
                                offset = (lineAlignWithBlur ? blurCorner2 : corner2) - startPosition;
                            }
                            else
                            {
                                offset = (lineAlignWithBlur ? blurCorner1 : corner1) - startPosition;
                            }
                            break;
                        default:
                            offset = Vector3.zero;
                            break;
                    }
                    break;
            }

            corner1 -= offset;
            corner2 -= offset;
            corner3 -= offset;
            corner4 -= offset;

            blurCorner1 -= offset;
            blurCorner2 -= offset;
            blurCorner3 -= offset;
            blurCorner4 -= offset;

            #endregion
        }

        public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 hit)
        {
            var b = p2 - p1;
            var d = p4 - p3;

            hit = Vector2.zero;

            float b_dot_d_perp = b.x * d.y - b.y * d.x;
            if (b_dot_d_perp == 0)
            {
                return false;
            }

            var c = p3 - p1;
            float t = (c.x * d.y - c.y * d.x) / b_dot_d_perp;
            if (t < 0 || t > 1)
            {
                return false;
            }
            float u = (c.x * b.y - c.y * b.x) / b_dot_d_perp;
            if (u < 0 || u > 1)
            {
                return false;
            }

            hit = new Vector2(p1.x + t * b.x, p1.y + t * b.y);
            return true;
        }

        public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }
    }
}
