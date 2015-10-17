using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

    //customize the fractal randomly
    
    public Mesh[] meshes;
    public Material material;
    public int maxDepth;
    public float childScale;
    public float spawnProbability;
    public float maxRotationSpeed;
    public float maxTwist;

    private float rotationSpeed;
    private int depth;
    private Material[,] materials;
    private static Vector3[] childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    private static Quaternion[] childOrientations =
    {
    Quaternion.identity,
    Quaternion.Euler(0f,0f,-90f),
    Quaternion.Euler(0f,0f,90f),
    Quaternion.Euler(90f,0f,0f),
    Quaternion.Euler(-90f,0f,0f)
    };
  

    // Creates an array of materials for the fractal based on the depth for dynamic batching
    // dynamic batching combines meshes with the same material in order to be more efficient
    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1,2];
        for (int i = 0; i <= maxDepth; i++)
        {
            float t = i / (maxDepth - 1f);
            t *= t;
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
        }
        materials[maxDepth,0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;
    }

    // Invoked once each frame before update when the script is enabled.
    //randomly assigns a mesh, and material to the game object and rotates it at a random speed.
    // - A mesh is a collection of points in 3d space which creates a 3d object
    // - A material defines the visual property of an object
    //Construct a child of the fractal until max depth is reached
    // parent - child relationship defined by transformational hierarchy.
    //randomly rotates and twists the child 
    void Start() {
        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0f, 0f);
        if (materials == null)
            InitializeMaterials();
        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0,meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = material;
        GetComponent<MeshRenderer>().material = materials[depth, Random.Range(0,2)];
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
    }
    // provides child with material and mesh from the parent
    // translates and scales the children based off of their parent and the given direction and orientation
    private void Initialize (Fractal parent, int childIndex)
    {
        meshes = parent.meshes;
        materials = parent.materials;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale;
        transform.localPosition = childDirections[childIndex] * (.5f + .5f * childScale);
        transform.localRotation = childOrientations[childIndex]; 
    }
	

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            if (Random.value < spawnProbability)
            {
                
                yield return new WaitForSeconds(1f);
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
            }
            }
       
    }

   

    // Update is called once per frame
    void Update () {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}
}
