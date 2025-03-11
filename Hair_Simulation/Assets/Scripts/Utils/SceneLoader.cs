using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //public TMP_InputField strandCountInput;
    //public TMP_InputField segmentInput;
    //public TMP_InputField strandLengthInput;
    //public TMP_InputField strandStiffnessInput;
    //public TMP_Dropdown curlTypeDropDown;
    //public TMP_InputField childrenInput;

    public void LoadStartScene()
    {
        SceneManager.LoadScene("HairStartScene");
    }

    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("HairSettingsScene");
    }

    public void LoadGeneratingScene()
    {
        //SimulationData.StrandCount = int.Parse(strandCountInput.text);
        //SimulationData.SegmentNumber = int.Parse(segmentInput.text);
        //SimulationData.StrandLength = float.Parse(strandLengthInput.text);
        //SimulationData.StrandStiffness = float.Parse(strandStiffnessInput.text);
        //SimulationData.CurlType = curlTypeDropDown.options[curlTypeDropDown.value].text;
        //SimulationData.ChildrenCount = int.Parse(childrenInput.text);

        SceneManager.LoadScene("HairGeneratingScene");
    }
}
