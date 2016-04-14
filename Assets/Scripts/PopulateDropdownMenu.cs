using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopulateDropdownMenu : MonoBehaviour {

    public int maxItems = 5;

    private string[] items;
    private Button[] generatedButtons;
    private int topItemIndex;
    private GameObject dropDownPanel;
    private GameObject scrollBar;
    private float heightOfButton;

    public bool dropDownOn = false;
    public bool clickedAgain = false;

	// Use this for initialization
	void Start () {
        dropDownPanel = GameObject.Find("CurrentlyOpenWindowsPanel");
        scrollBar = GameObject.Find("CurrentlyOpenWindowsScrollbar");
        scrollBar.SetActive(false);

        GameObject buttonObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Button", typeof(GameObject)));
        heightOfButton = buttonObject.GetComponent<LayoutElement>().minHeight + dropDownPanel.GetComponent<VerticalLayoutGroup>().spacing;
	}



    public Button[] generateMenu(string[] _items)
    {
        items = _items;
        if (!dropDownOn && !clickedAgain)
        {
            dropDownOn = true;

            //Deal with scrollbar
            if (items.Length <= dropDownPanel.GetComponent<PopulateDropdownMenu>().maxItems)
            {
                scrollBar.SetActive(false);
            }
            else
            {
                scrollBar.SetActive(true);
                Rect sbRect = scrollBar.GetComponent<RectTransform>().rect;
                scrollBar.GetComponent<RectTransform>().sizeDelta = new Vector2(sbRect.width, heightOfButton * maxItems);
                Scrollbar sb = scrollBar.GetComponent<Scrollbar>();
                sb.size = (float)maxItems / (float)items.Length;
                sb.numberOfSteps = items.Length - maxItems;
            }
            

            //Make the buttons
            int shownItems = maxItems < items.Length ? maxItems : items.Length;
            generatedButtons = new Button[shownItems];
            for (int i = 0; i < shownItems; i++)
            {
                GameObject buttonObject = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Button", typeof(GameObject)));
                buttonObject.transform.SetParent(dropDownPanel.transform, false);
                Transform rect = buttonObject.GetComponent<Transform>();
                Vector3 scale = new Vector3(1, 1, 1);
                rect.localScale = scale;
                RectTransform rectTrans = buttonObject.GetComponent<RectTransform>();
                Vector3 pos = new Vector3(0, 0, 0);
                rectTrans.localPosition = pos;
                Button button = buttonObject.GetComponent<Button>();
                GameObject textObject = buttonObject.transform.Find("Text").gameObject;
                Text text = textObject.GetComponent<Text>();
                string title = items[i + topItemIndex];
                text.text = title;
                button.onClick.RemoveAllListeners();
                generatedButtons[i] = button;
            }
            return generatedButtons;
        }

        //Kill the menu
        if (dropDownOn && clickedAgain)
        {
            dropDownOn = false;
            clickedAgain = false;
            KillDropDownButtons();
            scrollBar.GetComponent<Scrollbar>().value = 0;
        }
        return null;
    }




    public void Released()
    {
        if (dropDownOn && !clickedAgain)
        {
            clickedAgain = true;
        }
    }




    public void KillDropDownButtons()
    {
        int buttonsNumber = dropDownPanel.transform.childCount;
        for (int i = 0; i < buttonsNumber; i++)
        {
            GameObject button = (GameObject)dropDownPanel.transform.GetChild(i).gameObject;
            GameObject.Destroy(button);
        }
        scrollBar.SetActive(false);
    }




    public void scrollCallBack()
    {
        dropDownOn = false;
        KillDropDownButtons();
        Scrollbar sb = scrollBar.GetComponent<Scrollbar>();
        topItemIndex = (int)(sb.value * (items.Length - maxItems));
    }
}
