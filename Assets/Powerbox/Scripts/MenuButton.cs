using UnityEngine;
using UnityEngine.SceneManagement;

namespace Powerbox
{
    public class MenuButton : MonoBehaviour
    {
        public void SwitchScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
