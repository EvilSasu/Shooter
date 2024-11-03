using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MeshModifier : MonoBehaviour
{
    public int subdivisions = 10; // Liczba podzia³ów (wiêksza wartoœæ = wiêcej wierzcho³ków)
    public float impactRadius = 0.5f; // Promieñ obszaru modyfikacji wierzcho³ków
    public float impactStrength = 0.3f; // Si³a przesuniêcia wierzcho³ków

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] modifiedVertices;

    private void Start()
    {
        // Inicjalizacja siatki o wiêkszej liczbie wierzcho³ków
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        GenerateSubdividedCube(); // Generowanie szeœcianu z podzia³ami

        // Przechowywanie wierzcho³ków do modyfikacji
        modifiedVertices = mesh.vertices;

        meshCollider.sharedMesh = mesh; // Inicjalne przypisanie do MeshCollider
    }

    private void Update()
    {
        // Sprawdzenie, czy u¿ytkownik klikn¹³ lewym przyciskiem myszy
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Wykonanie raycast, aby sprawdziæ, gdzie u¿ytkownik klikn¹³ w obiekt
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ModifyMesh(hit.point);
                }
            }
        }
    }

    private void ModifyMesh(Vector3 hitPoint)
    {
        // Pozycja gracza, potrzebna do okreœlenia kierunku
        Vector3 playerPosition = Camera.main.transform.position;

        // Iterowanie przez wierzcho³ki i modyfikowanie tych, które s¹ w promieniu "impactRadius"
        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 worldVertexPosition = transform.TransformPoint(modifiedVertices[i]); // Przekszta³cenie lokalnych wierzcho³ków do wspó³rzêdnych œwiata

            // Obliczenie odleg³oœci od klikniêtego punktu
            float distanceToHitPoint = Vector3.Distance(worldVertexPosition, hitPoint);

            // Jeœli wierzcho³ek znajduje siê w promieniu od punktu klikniêcia, modyfikujemy go
            if (distanceToHitPoint < impactRadius)
            {
                // Kierunek w przeciwn¹ stronê do gracza
                Vector3 direction = (worldVertexPosition - playerPosition).normalized;

                // Przesuniêcie wierzcho³ka w kierunku od gracza
                modifiedVertices[i] += direction * impactStrength * (1 - distanceToHitPoint / impactRadius);
            }
        }

        // Zaktualizowanie siatki po modyfikacji wierzcho³ków
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals(); // Aby zaktualizowaæ normalne dla lepszego cieniowania
        mesh.RecalculateBounds(); // Upewnienie siê, ¿e granice siatki s¹ poprawnie zaktualizowane
        meshFilter.mesh = mesh;

        // Zaktualizowanie MeshCollider, aby dopasowaæ go do nowej siatki
        meshCollider.sharedMesh = null; // Reset, aby Unity zaktualizowa³ collider
        meshCollider.sharedMesh = mesh;
    }

    private void GenerateSubdividedCube()
    {
        // Tworzenie szeœcianu z podzia³ami
        mesh = new Mesh();

        int resolution = subdivisions + 1; // Liczba wierzcho³ków na krawêdzi
        Vector3[] vertices = new Vector3[resolution * resolution * 6];
        int[] triangles = new int[subdivisions * subdivisions * 6 * 6];

        int vertIndex = 0;
        int triIndex = 0;

        // Definiowanie ka¿dej ze œcian szeœcianu
        for (int side = 0; side < 6; side++)
        {
            Vector3 sideNormal = Vector3.zero;
            Vector3 sideUp = Vector3.zero;
            Vector3 sideRight = Vector3.zero;

            // Okreœlenie kierunków dla ka¿dej œciany
            switch (side)
            {
                case 0: sideNormal = Vector3.up; sideUp = Vector3.forward; sideRight = Vector3.right; break;  // Góra
                case 1: sideNormal = Vector3.down; sideUp = Vector3.forward; sideRight = Vector3.right; break;   // Dó³
                case 2: sideNormal = Vector3.forward; sideUp = Vector3.up; sideRight = Vector3.right; break;  // Przód
                case 3: sideNormal = Vector3.back; sideUp = Vector3.up; sideRight = Vector3.right; break;   // Ty³
                case 4: sideNormal = Vector3.right; sideUp = Vector3.up; sideRight = Vector3.back; break;     // Prawo
                case 5: sideNormal = Vector3.left; sideUp = Vector3.up; sideRight = Vector3.forward; break;   // Lewo (potrzebne odwrócenie)
            }

            // Tworzenie wierzcho³ków dla ka¿dej œciany
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float percentX = x / (float)subdivisions;
                    float percentY = y / (float)subdivisions;

                    // Wyliczenie wierzcho³ków œciany
                    vertices[vertIndex] = sideNormal + (percentX - 0.5f) * 2f * sideRight + (percentY - 0.5f) * 2f * sideUp;

                    if (x < subdivisions && y < subdivisions)
                    {
                        // Jeœli œciana jest "lewa" (side 5), odwracamy kolejnoœæ trójk¹tów
                        if (side == 5 || side == 1 || side == 4 || side == 2)
                        {
                            triangles[triIndex] = vertIndex;
                            triangles[triIndex + 1] = vertIndex + resolution + 1;
                            triangles[triIndex + 2] = vertIndex + resolution;

                            triangles[triIndex + 3] = vertIndex;
                            triangles[triIndex + 4] = vertIndex + 1;
                            triangles[triIndex + 5] = vertIndex + resolution + 1;
                        }
                        else
                        {
                            // Normalna kolejnoœæ trójk¹tów dla innych œcian
                            triangles[triIndex] = vertIndex;
                            triangles[triIndex + 1] = vertIndex + resolution;
                            triangles[triIndex + 2] = vertIndex + resolution + 1;

                            triangles[triIndex + 3] = vertIndex;
                            triangles[triIndex + 4] = vertIndex + resolution + 1;
                            triangles[triIndex + 5] = vertIndex + 1;
                        }

                        triIndex += 6;
                    }
                    vertIndex++;
                }
            }
        }

        // Przypisanie wierzcho³ków i trójk¹tów do siatki
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Przeliczenie normalnych dla poprawnej widocznoœci
        mesh.RecalculateBounds();  // Przeliczenie granic siatki

        meshFilter.mesh = mesh;
    }
}
