using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class VFXBehaviour : MonoBehaviour {
    [Space]
    [SerializeField] bool hasParent;
    [SerializeField][Range(.1f, 10f)] float lifeTime;

    private GameObject _parent;
    private VisualEffect _vfx;

    public GameObject Parent { get { return _parent; } set { _parent = value; } }

    void Awake() {
        _vfx = this.GetComponent<VisualEffect>();        
    }

    void Start() {
        if (hasParent) { 
            transform.SetParent(_parent.transform);
            transform.position = _parent.transform.position;
            transform.rotation = _parent.transform.rotation;
        }

        StartCoroutine(StartBehaviour());
    }

    private IEnumerator StartBehaviour() {
        _vfx.Play();
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
        yield break;
    }
}
