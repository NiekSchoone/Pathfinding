using UnityEngine;
using UnityEditor;
using System.Collections;

public class VectorTest : MonoBehaviour 
{
    public GameObject A;
    public GameObject B;
    public GameObject C;

    Vector2 pointA;
    Vector2 pointB;
    Vector2 pointC;

    Vector2 vectorAB;
    Vector2 vectorABPerp;

    Vector2 vectorAC;

    Vector2 projection;

    void Start () 
    {
    }

    float DotProduct(Vector2 a, Vector2 b) {
        return(a.x * b.x) + (a.y * b.y);
        //return (a.x * b.y) + (a.y * b.x);
    }

    Vector2 Projection(Vector2 service, Vector2 toBeProjected) {
        Vector2 normalService = service.normalized;
        float dotProduct = DotProduct(normalService, toBeProjected);
        normalService.x *= dotProduct;
        normalService.y *= dotProduct;
        return normalService;
    }

    Vector2 LeftPerp(Vector2 origin) {
        return new Vector2(-origin.y, origin.x);
    }

    Vector2 vectorOf(Vector2 a, Vector2 b) {
        return (b - a);
    }

    void Update() {
        pointA = A.transform.position;
        pointB = B.transform.position;
        pointC = C.transform.position;

        vectorAB = vectorOf(pointA, pointB);
        vectorABPerp = LeftPerp(vectorAB);

        vectorAC = vectorOf(pointA, pointC);

        projection = Projection(vectorABPerp, vectorAC);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pointA, pointA + vectorAB);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointA + vectorAC);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA, pointA + vectorABPerp);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pointA, pointA + projection);
    }
}
