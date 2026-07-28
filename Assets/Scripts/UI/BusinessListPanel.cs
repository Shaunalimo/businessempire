using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Managers;
using BusinessEmpire.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BusinessEmpire.UI
{
    /// <summary>
    /// Scrollable list of all businesses with purchase/manage actions.
    /// </summary>
    public class BusinessListPanel : MonoBehaviour
    {
        [SerializeField] private UIAnimator animator;
        [SerializeField] private Transform listContainer;
        [SerializeField] private BusinessCardUI cardPrefab;
        [SerializeField] private BusinessDetailPanel detailPanel;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Hide);
        }

        public void Show()
        {
            animator?.Show();
            RefreshList();
        }

        public void Hide() => animator?.Hide();

        private void RefreshList()
        {
            if (listContainer == null || cardPrefab == null) return;

            foreach (Transform child in listContainer)
                Destroy(child.gameObject);

            foreach (var def in BusinessManager.Instance.GetAllDefinitions())
            {
                if (def == null) continue;
                var card = Instantiate(cardPrefab, listContainer);
                card.Setup(def, OnCardClicked);
            }
        }

        private void OnCardClicked(BusinessDefinition def)
        {
            detailPanel?.Show(def);
        }
    }
}
