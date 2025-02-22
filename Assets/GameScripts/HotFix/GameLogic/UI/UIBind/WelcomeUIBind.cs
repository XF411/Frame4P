using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fream4P.Core.UI;

namespace GameLogic
{
    public class WelcomeUIBind : UIBindBase
    {
        public Image bg;
        public Image InputField_TMP;
        public RectTransform Text_Area;
        public TextMeshProUGUI Placeholder;
        public TextMeshProUGUI Text;
        public Button Button;
        public TextMeshProUGUI Text_TMP;

        public override void BindUI()
        {
            bg = UnityUtils.FindComponentWithName<Image>("bg", gameObject);
            InputField_TMP = UnityUtils.FindComponentWithName<Image>("InputField (TMP)", gameObject);
            Text_Area = UnityUtils.FindComponentWithName<RectTransform>("Text Area", InputField_TMP.gameObject);
            Placeholder = UnityUtils.FindComponentWithName<TextMeshProUGUI>("Placeholder", Text_Area.gameObject);
            Text = UnityUtils.FindComponentWithName<TextMeshProUGUI>("Text", Text_Area.gameObject);
            Button = UnityUtils.FindComponentWithName<Button>("Button", gameObject);
            Text_TMP = UnityUtils.FindComponentWithName<TextMeshProUGUI>("Text (TMP)", Button.gameObject);
        }
    }
}
