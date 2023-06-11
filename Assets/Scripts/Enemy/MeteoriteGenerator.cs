using UnityEngine;
using Random = UnityEngine.Random;

public class MeteoriteGenerator : MonoBehaviour
{
    public GameObject meteorite;
    public float generateTime;
    public float generateRangeXMin;
    public float generateRangeXMax;
    public float generateHeight = 50;

    private float _generateTimer;

    // Update is called once per frame
    void Update()
    {
        _generateTimer += Time.deltaTime;

        if (_generateTimer >= generateTime)
        {
            ObjectPool.Instance.GetObject("Meteorite", new Vector3(Random.Range(generateRangeXMin, generateRangeXMax), generateHeight, 0),
                Quaternion.identity);
            _generateTimer = 0;
        }
    }
}
