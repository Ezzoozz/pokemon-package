using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace outreal.Pokemon
{
    public class GenerateNewImage : MonoBehaviour
    {
        public static event System.Action<string> OnClickSpace;

        MeshRenderer meshRenderer;

        bool isRunning = false;

        Queue<string> urls = new Queue<string>();

        // Start is called before the first frame update
        void Start()
        {

            meshRenderer = GetComponent<MeshRenderer>();

        }

        void CheckForSpace()
        {

            if (meshRenderer == null)
            {
                Debug.Log("There is no mesh renderer on the object to apply the photo when space is pressed");
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("space pressed");
                int random = Random.Range(1, 152);
                string url = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/";
                url += random + ".png";

                OnClickSpace?.Invoke(url);




            }

        }
        void Update()
        {


            CheckForSpace();


            if (!isRunning)
                if (urls.Count >= 1)
                    StartCoroutine(putImg(urls.Dequeue()));

        }
        void OnEnable()
        {

            OnClickSpace += OnDownloadImage;
        }

        private void OnDisable()
        {
            OnClickSpace -= OnDownloadImage;
        }

        void OnDownloadImage(string obj)
        {
            Debug.Log(obj);

            urls.Enqueue(obj);

            Debug.Log("urls " + urls.Count);
        }

        IEnumerator putImg(string url)
        {




            isRunning = true;
            Debug.Log("downloading");
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);



            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (meshRenderer.material.mainTexture)
                    Destroy(meshRenderer.material.mainTexture);
                Debug.Log("done");





                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                Debug.Log("applying texture");

                meshRenderer.material.mainTexture = texture;

                isRunning = false;

                Debug.Log("Downloaded " + www.downloadedBytes.ToString());
                www.disposeUploadHandlerOnDispose = true;
                www.disposeDownloadHandlerOnDispose = true;
                www.disposeCertificateHandlerOnDispose = true;
                www.Dispose();
                Resources.UnloadUnusedAssets();



            }


            isRunning = false;

            www.Dispose();


            // Update is called once per frame

        }
    }
}
