using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Loop Vertical Scroll Rect", 51)]
    [DisallowMultipleComponent]
    public class LoopVerticalScrollRect : LoopScrollRect
    {
        protected override float GetSize(RectTransform item)
        {
            float size = contentSpacing;
            if (m_GridLayout != null)
            {
                size += m_GridLayout.cellSize.y;
            }
            else
            {
                //size += LayoutUtility.GetPreferredHeight(item);
                size += item.sizeDelta.y;
            }
            return size;
        }

        protected override float GetDimension(Vector2 vector)
        {
            return vector.y;
        }

        protected override Vector2 GetVector(float value)
        {
            return new Vector2(0, value);
        }

        protected override void Awake()
        {
            base.Awake();
            directionSign = -1;
            this.content = this.transform.Find("content").GetComponent<RectTransform>();
            if (this.prefabSource.prefabGameobj == null)
            {
                this.prefabSource.prefabGameobj = content.GetChild(0).gameObject;
            }
            
            this.prefabSource.prefabGameobj.SetActive(false);
            this.horizontal = false;
            this.vertical = true;

            GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
            if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            {
                Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
            }
        }

        protected override bool UpdateItems(Bounds viewBounds, Bounds contentBounds)
        {
            bool changed = false;
            if (viewBounds.min.y < (contentBounds.min.y - 1))
            {
                //底部添加一行
                float size = NewItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.min.y < contentBounds.min.y - totalSize)
                {
                    size = NewItemAtEnd();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }
            else if (viewBounds.min.y > contentBounds.min.y + threshold)
            {
                //删除底部
                float size = DeleteItemAtEnd(), totalSize = size;
                while (size > 0 && viewBounds.min.y > contentBounds.min.y + threshold + totalSize)
                {
                    size = DeleteItemAtEnd();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }

            if (viewBounds.max.y > (contentBounds.max.y + 1))
            {
                //在顶部添加
                float size = NewItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.max.y > contentBounds.max.y + totalSize)
                {
                    size = NewItemAtStart();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }
            else if (viewBounds.max.y < contentBounds.max.y - threshold)
            {
                //在顶部删除
                float size = DeleteItemAtStart(), totalSize = size;
                while (size > 0 && viewBounds.max.y < contentBounds.max.y - threshold - totalSize)
                {
                    size = DeleteItemAtStart();
                    totalSize += size;
                }
                if (totalSize > 0)
                    changed = true;
            }
            return changed;
        }
    }
}