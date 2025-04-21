using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ship
{
    public class FirstShipVillageScene : MonoBehaviour
    {
        [Header("Dialog Key")]
        public string sceneDialogKey;

        [Header("Next Scene Settings")]
        public RoomScriptable currentRoom; 
        public string exitingTo; 
        public float waitTime = 2f;

        private DialogManager dialogManager;
        private ScreenFade screenFade;

        private void Start()
        {
            dialogManager = FindObjectOfType<DialogManager>();
            screenFade = FindObjectOfType<ScreenFade>();

            if (dialogManager == null)
            {
                Debug.LogError("No DialogManager found.");
                return;
            }
            StartCoroutine(PlayDialogAndTransition());
        }

        private IEnumerator PlayDialogAndTransition()
        {
            yield return new WaitForSeconds(waitTime);
            dialogManager.autoAdvanceDialog = true;

            dialogManager.onDialogComplete = () =>
            {
                if (RoomGoToManager.instance == null)
                {
                    Debug.LogError("💥 RoomGoToManager.instance is NULL!");
                }
                else
                {
                    Debug.Log("✅ RoomGoToManager.instance exists. Calling GoToRoom...");
                    RoomGoToManager.instance.GoToRoom(currentRoom, exitingTo);
                }
            };
            dialogManager.PlayScene(sceneDialogKey);
        }
    }
}
