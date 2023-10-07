using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu {
    public class MainMenu : MonoBehaviour {

        public void PlaySinglePlayer() {
            SceneManager.LoadScene("Scenes/SinglePlayer/SinglePlayerBattleshipScene");
        }

        public void PlayMultiplayer() {
            SceneManager.LoadScene("Scenes/MultiPlayer/MultiPlayerBattleshipScene");
        }

        public void QuitGame() {
            Application.Quit();
        }
    }
}