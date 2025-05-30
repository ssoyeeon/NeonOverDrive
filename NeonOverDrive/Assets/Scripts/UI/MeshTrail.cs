using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2.0f;
    public float speedBoost = 6;
    public float animSpeedBoost = 1.5f;

    //[Herder("Mesh Releted")]
    public float meshRefreshRate = 0.5f;
    public float meshDestroyDelay = 0.7f;
    public Transform positionToSpawn;

    //[Header("Shader Releted")]
    public Material mat;
    public string shaderVerRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedRenderers;
    private MeshRenderer[] meshRenderers;
    private bool isTrailActive;

    private float normalSpeed;          

    IEnumerator AnimateMaterialFloat(Material m, float valueGoal, float rate, float refreshRate)
    {
        float valueToAnimate = m.GetFloat(shaderVerRef);

        while(valueToAnimate > valueGoal)
        {
            valueToAnimate -= rate;
            m.SetFloat(shaderVerRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
    IEnumerator ActivateTrail(float timeActivated)
    {
        while(timeActivated > 0)
        {
            timeActivated -= meshRefreshRate;
            
            // SkinnedMeshRenderer와 MeshRenderer 모두 가져오기
            if(skinnedRenderers == null)
                skinnedRenderers = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();
            if(meshRenderers == null)
                meshRenderers = positionToSpawn.GetComponentsInChildren<MeshRenderer>();

            // SkinnedMeshRenderer 처리
            for(int i = 0; i < skinnedRenderers.Length; i++)
            {
                GameObject gOBJ = new GameObject();
                gOBJ.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                
                MeshRenderer mr = gOBJ.AddComponent<MeshRenderer>();
                MeshFilter mf = gOBJ.AddComponent<MeshFilter>();

                Mesh m = new Mesh();
                skinnedRenderers[i].BakeMesh(m);
                mf.mesh = m;

                mr.material = mat;
                
                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gOBJ, meshDestroyDelay);
            }

            // MeshRenderer 처리
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                GameObject gOBJ = new GameObject();
                gOBJ.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation); 
                gOBJ.transform.localScale = gameObject.transform.lossyScale;


                MeshRenderer mr = gOBJ.AddComponent<MeshRenderer>();
                MeshFilter mf = gOBJ.AddComponent<MeshFilter>();

                // 기존 메시 복사
                MeshFilter sourceMf = meshRenderers[i].GetComponent<MeshFilter>();
                if(sourceMf != null && sourceMf.mesh != null)
                {
                    mf.mesh = sourceMf.mesh;
                    mr.material = mat;
                    
                    StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                    Destroy(gOBJ, meshDestroyDelay);
                }
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }
}
