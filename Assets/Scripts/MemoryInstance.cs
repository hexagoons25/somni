using TMPro;
using UnityEngine;

public class MemoryInstance : MonoBehaviour
{
    public System.Guid id;
    public string memoryName;
    public bool visited = false;
    public ConnectionManager manager;

    private AnimationCurve curve;
    private float animationDelay;
    private float initialY;
    private CanvasGroup group;
    // TODO: Dynamically change hint label
    private TextMeshProUGUI hintLabel;
    private TextMeshProUGUI nameLabel;
    private GameObject sceneObj;
    private AudioSource bellPlayer;


    private void Start()
    {
        id = System.Guid.NewGuid();
        manager.Register(id, this);

        curve = new(new Keyframe(0, 0), new Keyframe(0.5F, 1), new Keyframe(1, 0))
        {
            postWrapMode = WrapMode.Loop
        };

        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0;

        initialY = transform.position.y;

        animationDelay = Random.Range(0.5F, 1.5F);

        sceneObj = transform.Find("Scene").gameObject;
        bellPlayer = transform.Find("BellPlayer").gameObject.GetComponent<AudioSource>();

        nameLabel = transform.Find("UI").gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        nameLabel.gameObject.SetActive(false);
        nameLabel.text = memoryName;

        group = transform.Find("UI").gameObject.transform.Find("Hint").gameObject.GetComponent<CanvasGroup>();

        hintLabel = transform.Find("UI").gameObject.transform.Find("Hint").gameObject.transform.Find("Content").gameObject.GetComponent<TextMeshProUGUI>();

        HideHint();
    }

    private void ShowHint()
    {
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private void HideHint()
    {
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bellPlayer.Play();
            manager.SetCurrentInstance(id);
            // hintContent.text = area.ControllerHasLink(name) ? linkedkHint : noLinkHint;
            ShowHint();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            manager.ClearCurrentInstance();
            HideHint();
        }
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, initialY + Util.Remap(curve.Evaluate((Time.time + animationDelay) / 2 % curve.length), 0, 1, 0, 0.3F), transform.position.z);
    }

    public void Interact()
    {
        if (!visited)
        {
            nameLabel.gameObject.SetActive(true);
        }
        visited = true;
        sceneObj.SetActive(true);
        Scene scene = sceneObj.GetComponent<Scene>();
        scene.Run();
    }
}
