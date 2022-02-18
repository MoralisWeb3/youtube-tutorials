using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.U2D.Common
{
    internal interface IImagePackNodeVisitor
    {
        void Visit(ImagePackNode node);
    }

    class CollectEmptyNodePositionVisitor : IImagePackNodeVisitor
    {
        public List<RectInt> emptyAreas = new List<RectInt>();
        public void Visit(ImagePackNode node)
        {
            if (node.imageId == -1)
            {
                emptyAreas.Add(node.rect);
            }
        }
    }

    class CollectPackNodePositionVisitor : IImagePackNodeVisitor
    {
        public CollectPackNodePositionVisitor()
        {
            positions = new Vector2Int[0];
        }

        public void Visit(ImagePackNode node)
        {
            if (node.imageId != -1)
            {
                if (positions.Length < node.imageId + 1)
                {
                    var p = positions;
                    Array.Resize(ref p, node.imageId + 1);
                    positions = p;
                }

                positions[node.imageId].x = node.rect.x;
                positions[node.imageId].y = node.rect.y;
            }
        }

        public Vector2Int[] positions { get; private set; }
    }

    internal class ImagePackNode
    {
        public ImagePackNode left;
        public ImagePackNode right;
        public RectInt rect;
        public Vector2Int imageWidth;
        public int imageId = -1;

        public void AcceptVisitor(IImagePackNodeVisitor visitor)
        {
            visitor.Visit(this);
            if (left != null)
                left.AcceptVisitor(visitor);
            if (right != null)
                right.AcceptVisitor(visitor);
        }

        public void AdjustSize(int oriWidth, int oriHeight, int deltaW, int deltaH, out int adjustx, out int adjusty)
        {
            adjustx = adjusty = 0;
            int adjustXleft = 0, adjustYleft = 0, adjustXRight = 0, adjustYRight = 0;
            if (imageId == -1 || left == null)
            {
                if (rect.x + rect.width == oriWidth)
                {
                    rect.width += deltaW;
                    adjustx = deltaW;
                }
                if (rect.y + rect.height == oriHeight)
                {
                    rect.height += deltaH;
                    adjusty = deltaH;
                }
            }
            else
            {
                left.AdjustSize(oriWidth, oriHeight, deltaW, deltaH, out adjustXleft, out adjustYleft);
                right.AdjustSize(oriWidth, oriHeight, deltaW, deltaH, out adjustXRight, out adjustYRight);

                adjustx = Mathf.Max(adjustXleft, adjustXRight);
                rect.width += adjustx;
                adjusty = Mathf.Max(adjustYleft, adjustYRight);
                rect.height += adjusty;
            }
        }

        public bool TryInsert(ImagePacker.ImagePackRect insert, int padding, out Vector2Int remainingSpace)
        {
            remainingSpace = Vector2Int.zero;
            int insertWidth = insert.rect.width + padding * 2;
            int insertHeight = insert.rect.height + padding * 2;
            if (insertWidth > rect.width || insertHeight > rect.height)
                return false;

            if (imageId == -1)
            {
                remainingSpace.x = rect.width - insertWidth;
                remainingSpace.y = rect.height - insertHeight;
            }
            else
            {
                Vector2Int spaceLeft, spaceRight;
                bool insertLeft, insertRight;
                ImagePackNode tryLeft, tryRight;
                tryLeft = left;
                tryRight = right;
                if (left == null && !SplitRects(this, insert, padding, out tryLeft, out tryRight))
                {
                    return false;
                }

                insertLeft = tryLeft.TryInsert(insert, padding, out spaceLeft);
                insertRight = tryRight.TryInsert(insert, padding, out spaceRight);
                if (insertLeft && insertRight)
                {
                    remainingSpace = spaceLeft.sqrMagnitude < spaceRight.sqrMagnitude ? spaceLeft : spaceRight;
                }
                else if (insertLeft)
                    remainingSpace = spaceLeft;
                else if (insertRight)
                    remainingSpace = spaceRight;
                else
                    return false;
            }

            return true;
        }

        static bool SplitRects(ImagePackNode node, ImagePacker.ImagePackRect insert, int padding, out ImagePackNode left, out ImagePackNode right)
        {
            // Find the best way to split the rect based on a new rect
            left = right = null;
            var tryRects = new[]
            {
                new ImagePackNode(), new ImagePackNode(),
                new ImagePackNode(), new ImagePackNode()
            };

            tryRects[0].rect = new RectInt(node.rect.x + node.imageWidth.x, node.rect.y, node.rect.width - node.imageWidth.x, node.rect.height);
            tryRects[1].rect = new RectInt(node.rect.x, node.rect.y + node.imageWidth.y, node.imageWidth.x, node.rect.height - node.imageWidth.y);
            tryRects[2].rect = new RectInt(node.rect.x, node.rect.y + node.imageWidth.y, node.rect.width, node.rect.height - node.imageWidth.y);
            tryRects[3].rect = new RectInt(node.rect.x + node.imageWidth.x, node.rect.y, node.rect.width - node.imageWidth.x, node.imageWidth.y);
            float smallestSpace = float.MinValue;
            for (int i = 0; i < tryRects.GetLength(0); ++i)
            {
                //for (int j = 0; j < tryRects.GetLength(1); ++j)
                {
                    Vector2Int newSpaceLeft;
                    if (tryRects[i].TryInsert(insert, padding, out newSpaceLeft))
                    {
                        if (smallestSpace < newSpaceLeft.sqrMagnitude)
                        {
                            smallestSpace = newSpaceLeft.sqrMagnitude;
                            int index = i / 2 * 2;
                            left = tryRects[index];
                            right = tryRects[index + 1];
                        }
                    }
                }
            }
            return left != null;
        }

        public bool Insert(ImagePacker.ImagePackRect insert, int padding)
        {
            int insertWidth = insert.rect.width + padding * 2;
            int insertHeight = insert.rect.height + padding * 2;
            if (insertWidth > rect.width || insertHeight > rect.height)
                return false;

            if (imageId == -1)
            {
                imageId = insert.index;
                imageWidth = new Vector2Int(insertWidth, insertHeight);
            }
            else
            {
                if (left == null && !SplitRects(this, insert, padding, out left, out right))
                {
                    return false;
                }
                // We assign to the node that has a better fit for the image
                Vector2Int spaceLeft, spaceRight;
                bool insertLeft, insertRight;
                insertLeft = left.TryInsert(insert, padding, out spaceLeft);
                insertRight = right.TryInsert(insert, padding, out spaceRight);
                if (insertLeft && insertRight)
                {
                    if (spaceLeft.sqrMagnitude < spaceRight.sqrMagnitude)
                        left.Insert(insert, padding);
                    else
                        right.Insert(insert, padding);
                }
                else if (insertLeft)
                    left.Insert(insert, padding);
                else if (insertRight)
                    right.Insert(insert, padding);
                else
                    return false;
            }
            return true;
        }
    }
}
