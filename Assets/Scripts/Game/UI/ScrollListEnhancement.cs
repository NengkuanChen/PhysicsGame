using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 动态构建无限滚动列表
    /// </summary>
    public class ScrollListEnhancement : MonoBehaviour
    {
        public interface IItem
        {
            RectTransform RootRectTransform { get; }
        }

        public enum Layout
        {
            TopToDown,
            LeftToRight
        }

        [Serializable]
        public class Padding
        {
            public float left;
            public float top;
            public float right;
            public float bottom;
        }

        [SerializeField]
        private Vector2 itemSize = new Vector2(100f, 100f);
        [SerializeField]
        private Layout layout;
        [SerializeField, OnValueChanged("OnLayoutPropertiesChanged", true)]
        private Padding padding;
        [SerializeField, OnValueChanged("OnLayoutPropertiesChanged", true)]
        private float space;

        [SerializeField, Required]
        private RectTransform scrollViewRoot;
        [SerializeField, Required]
        private RectTransform scrollContentRoot;

        [SerializeField, Required]
        private RectTransform listTemplateRectTransform;

        private int itemCount;
        private LinkedList<IItem> allItems = new LinkedList<IItem>();
        public LinkedList<IItem> AllItems => allItems;
        private List<IItem> itemPool = new List<IItem>();
        private int currentItemStartIndex;

        private Action<IItem> onItemCreated;
        private Action<IItem, int> onItemShow;
        private Action<IItem, int> onItemHide;

        private void Awake()
        {
            listTemplateRectTransform.gameObject.SetActive(false);
        }

        public void BindCallback(Action<IItem> onCreated = null,
                                 Action<IItem, int> onShow = null,
                                 Action<IItem, int> onHide = null)
        {
            onItemCreated = onCreated;
            onItemShow = onShow;
            onItemHide = onHide;
        }

        public void RefreshItemCount(int count)
        {
            itemCount = Mathf.Max(0, count);

            //refresh content size
            var viewRect = scrollViewRoot.rect;
            if (layout == Layout.TopToDown)
            {
                var contentHeight = padding.top + count * itemSize.y + (count - 1) * space + padding.bottom;
                scrollContentRoot.sizeDelta = new Vector2(viewRect.width, contentHeight);
            }
            else if (layout == Layout.LeftToRight)
            {
                var contentWidth = padding.left + count * itemSize.x + (count - 1) * space + padding.right;
                scrollContentRoot.sizeDelta = new Vector2(contentWidth, viewRect.height);
            }

            Update();
        }

        public void ForceCallItemShow()
        {
            var dataIndex = 0;
            foreach (var item in allItems)
            {
                onItemShow?.Invoke(item, dataIndex);
                dataIndex++;
            }
        }

        public IItem GetVisibleItem(int visibleIndex)
        {
            var i = 0;
            foreach (var item in allItems)
            {
                if (i == visibleIndex)
                {
                    return item;
                }

                i++;
            }

            return null;
        }

        public UniTask<IItem> ScrollToDataIndexAtListFirstAsync(int dataIndex,
                                                               float minSpaceToEdge = 0f,
                                                               float scrollTime = 1f)
        {
            var utc = new UniTaskCompletionSource<IItem>();
            ScrollToDataIndexAtListFirst(dataIndex,minSpaceToEdge,scrollTime,
                item =>
                {
                    utc.TrySetResult(item);
                });

            return utc.Task;
        }

        public void ScrollToDataIndexAtListFirst(int dataIndex,
                                                 float minSpaceToEdge = 0f,
                                                 float scrollTime = 1f,
                                                 Action<IItem> onComplete = null)
        {
            DOTween.Kill(scrollContentRoot);
            var itemPosition = CalculateItemPosition(dataIndex);

            minSpaceToEdge = Mathf.Max(0f, minSpaceToEdge);

            var contentPreferPosition = Vector2.zero;
            if (layout == Layout.LeftToRight)
            {
                contentPreferPosition.x = -itemPosition.x + Mathf.Max(space, minSpaceToEdge);
            }
            else if (layout == Layout.TopToDown)
            {
                contentPreferPosition.y = itemPosition.y - Mathf.Max(space, minSpaceToEdge);
            }
            else
            {
                throw new NotImplementedException($"layout {layout} not implemented");
            }

            var tweener = scrollContentRoot.DOLocalMove(contentPreferPosition, scrollTime).SetEase(Ease.InOutCubic);
            if (onComplete != null)
            {
                tweener.onComplete = () => { onComplete(GetVisibleItem(dataIndex - currentItemStartIndex)); };
            }
        }

        private void Update()
        {
            if (itemCount == 0)
            {
                //hide all
                var dataIndex = currentItemStartIndex;
                foreach (var item in allItems)
                {
                    onItemHide?.Invoke(item, dataIndex);
                    ReleaseItem(item);
                    dataIndex++;
                }

                allItems.Clear();
            }
            else
            {
                var startIndex = CalculateVisibleItemStartIndex();
                var endIndex = CalculateVisibleItemEndIndex();

                var currentItemCount = allItems.Count;

                //hide outside
                if (currentItemCount > 0)
                {
                    var itemNode = allItems.First;
                    var itemIndex = currentItemStartIndex;
                    while (itemNode != null)
                    {
                        var nextNode = itemNode.Next;
                        if (itemIndex < startIndex || itemIndex > endIndex)
                        {
                            onItemHide?.Invoke(itemNode.Value, itemIndex);
                            allItems.Remove(itemNode);
                            ReleaseItem(itemNode.Value);
                        }

                        itemNode = nextNode;
                        itemIndex++;
                    }
                }

                //add at start
                if (currentItemStartIndex > startIndex)
                {
                    for (int i = currentItemStartIndex - 1; i >= startIndex; i--)
                    {
                        var newItem = SpawnItem();
                        newItem.RootRectTransform.anchoredPosition = CalculateItemPosition(i);
                        onItemShow?.Invoke(newItem, i);
                        allItems.AddFirst(newItem);
                    }
                }

                currentItemStartIndex = startIndex;

                //add at last
                if (currentItemStartIndex + allItems.Count <= endIndex)
                {
                    for (int i = currentItemStartIndex + allItems.Count; i <= endIndex; i++)
                    {
                        var newItem = SpawnItem();
                        newItem.RootRectTransform.anchoredPosition = CalculateItemPosition(i);
                        onItemShow?.Invoke(newItem, i);
                        allItems.AddLast(newItem);
                    }
                }
            }
        }

        private int CalculateVisibleItemStartIndex()
        {
            var contentAnchorPos = scrollContentRoot.anchoredPosition;

            var startIndex = 0;
            if (layout == Layout.TopToDown)
            {
                startIndex = Mathf.FloorToInt((contentAnchorPos.y - padding.top) / (itemSize.y + space));
                startIndex = Mathf.Clamp(startIndex, 0, itemCount - 1);
            }
            else if (layout == Layout.LeftToRight)
            {
                startIndex = Mathf.FloorToInt((-contentAnchorPos.x - padding.left) / (itemSize.x + space));
                startIndex = Mathf.Clamp(startIndex, 0, itemCount - 1);
            }
            else
            {
                throw new Exception($"不支持的layout {layout.ToString()}");
            }

            return startIndex;
        }

        private int CalculateVisibleItemEndIndex()
        {
            var contentAnchorPos = scrollContentRoot.anchoredPosition;

            var endIndex = 0;
            if (layout == Layout.TopToDown)
            {
                endIndex = Mathf.CeilToInt((contentAnchorPos.y + scrollViewRoot.rect.height - padding.top) /
                                           (itemSize.y + space));
                endIndex = Mathf.Min(itemCount - 1, endIndex);
            }
            else if (layout == Layout.LeftToRight)
            {
                endIndex = Mathf.CeilToInt((-contentAnchorPos.x + scrollViewRoot.rect.width - padding.left) /
                                           (itemSize.x + space));
                endIndex = Mathf.Min(itemCount - 1, endIndex);
            }
            else
            {
                throw new Exception($"不支持的layout {layout.ToString()}");
            }

            return endIndex;
        }

        private Vector2 CalculateItemPosition(int itemIndex)
        {
            if (layout == Layout.TopToDown)
            {
                var yPos = padding.top + (space + itemSize.y) * itemIndex;
                return new Vector2(padding.left, -yPos);
            }
            else if (layout == Layout.LeftToRight)
            {
                var xPos = padding.left + (space + itemSize.x) * itemIndex;
                return new Vector2(xPos, -padding.top);
            }
            else
            {
                throw new Exception($"不支持的layout {layout.ToString()}");
            }
        }

        private IItem SpawnItem()
        {
            IItem resultItem;

            if (itemPool.Count == 0)
            {
                var newItemRectTransform = Instantiate(listTemplateRectTransform, scrollContentRoot);
                newItemRectTransform.gameObject.SetActive(true);

                resultItem = newItemRectTransform.GetComponent<IItem>();
                if (resultItem == null)
                {
                    throw new GameLogicException(
                        $"game object {newItemRectTransform.name} have no component: {nameof(IItem)}");
                }

                onItemCreated?.Invoke(resultItem);
            }
            else
            {
                resultItem = itemPool[itemPool.Count - 1];
                itemPool.RemoveAt(itemPool.Count - 1);
            }

            resultItem.RootRectTransform.gameObject.SetActive(true);
            return resultItem;
        }

        private void ReleaseItem(IItem item)
        {
            item.RootRectTransform.gameObject.SetActive(false);
            itemPool.Add(item);
        }

    #if UNITY_EDITOR

        [Button(ButtonSizes.Large), PropertySpace(SpaceAfter = 30)]
        private void Setup()
        {
            if (listTemplateRectTransform != null)
            {
                listTemplateRectTransform.anchorMax = new Vector2(0f, 1f);
                listTemplateRectTransform.anchorMin = new Vector2(0f, 1f);
                listTemplateRectTransform.pivot = new Vector2(0f, 1f);
                listTemplateRectTransform.anchoredPosition = Vector2.zero;
            }

            var scrollRect = GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollViewRoot = scrollRect.viewport;
                scrollContentRoot = scrollRect.content;
                scrollRect.horizontal = layout == Layout.LeftToRight;
                scrollRect.vertical = layout == Layout.TopToDown;
            }

            if (scrollContentRoot != null)
            {
                scrollContentRoot.anchorMax = new Vector2(0f, 1f);
                scrollContentRoot.anchorMin = new Vector2(0f, 1f);
                scrollContentRoot.pivot = new Vector2(0f, 1f);
                scrollContentRoot.sizeDelta = new Vector2(scrollViewRoot.rect.width, itemSize.y);
            }
        }

        [BoxGroup("Test", Order = 999), SerializeField, Min(1)]
        private int testItemCount = 10;

        [BoxGroup("Test"), Button(ButtonSizes.Large)]
        private void TestLayout()
        {
            if (listTemplateRectTransform == null)
            {
                return;
            }

            if (scrollContentRoot == null)
            {
                return;
            }

            listTemplateRectTransform.gameObject.SetActive(false);

            var itemCount = scrollContentRoot.childCount;
            for (var i = itemCount - 1; i >= 0; i--)
            {
                var itemRectTrans = scrollContentRoot.GetChild(i).GetComponent<RectTransform>();
                if (itemRectTrans == listTemplateRectTransform)
                {
                    continue;
                }

                DestroyImmediate(itemRectTrans.gameObject);
            }

            for (var i = 0; i < testItemCount; i++)
            {
                var newItemRectTrans = Instantiate(listTemplateRectTransform, scrollContentRoot);
                newItemRectTrans.anchoredPosition = CalculateItemPosition(i);
                newItemRectTrans.gameObject.SetActive(true);
            }
        }

        [BoxGroup("Test"), Button(ButtonSizes.Large)]
        private void ClearTestLayout()
        {
            var itemCount = scrollContentRoot.childCount;
            for (var i = itemCount - 1; i >= 0; i--)
            {
                var itemRectTrans = scrollContentRoot.GetChild(i).GetComponent<RectTransform>();
                if (itemRectTrans == listTemplateRectTransform)
                {
                    continue;
                }

                DestroyImmediate(itemRectTrans.gameObject);
            }

            if (listTemplateRectTransform != null)
            {
                listTemplateRectTransform.gameObject.SetActive(true);
            }
        }

        private void OnLayoutPropertiesChanged()
        {
            var itemCount = scrollContentRoot.childCount;
            var itemIndex = 0;
            for (var i = 0; i < itemCount; i++)
            {
                var itemRectTrans = scrollContentRoot.GetChild(i).GetComponent<RectTransform>();
                if (itemRectTrans == listTemplateRectTransform)
                {
                    continue;
                }

                itemRectTrans.anchoredPosition = CalculateItemPosition(itemIndex);
                itemIndex++;
            }
        }

        // [BoxGroup("Test"), Button(ButtonSizes.Large)]
        private void TempTest()
        {
            RefreshItemCount(testItemCount);
        }
    #endif
    }
}