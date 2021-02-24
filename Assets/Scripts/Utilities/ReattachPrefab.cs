#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class ReattachPrefab : MonoBehaviour
{
    public bool pReplace = false;
    public string pName;
    public string pNewName;
    public GameObject pPrefab;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnDrawGizmosSelected()
    {
        if (pReplace)
        {
            pReplace = false;
            //bool vFound=false;

            if (pName == "") pName = this.gameObject.name;
            if (pNewName == "") pNewName = pName;
            if (pPrefab == null) pPrefab = (GameObject)Resources.Load(pName);
            if (pPrefab != null)
            {
                for (int vNum = 0; vNum < 60; vNum++)
                {
                    GameObject vObj = (GameObject)GameObject.Find(pName);
                    //vFound=false;
                    if (vObj != null)
                    {
                        if (pPrefab != null)
                        {
                            GameObject vNewObj = (GameObject)PrefabUtility.InstantiatePrefab(pPrefab);
                            vNewObj.transform.parent = vObj.transform.parent;
                            vNewObj.transform.position = vObj.transform.position;
                            vNewObj.transform.rotation = vObj.transform.rotation;
                            vNewObj.transform.localScale = vObj.transform.localScale;

                            vNewObj.name = pNewName;
                            DestroyImmediate(vObj);
                            //vFound=true;
                        }
                    }
                }
            }
            else
                Debug.Log("No prefab found");
        }
    }

}
#endif