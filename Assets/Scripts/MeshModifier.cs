using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MeshModifier : MonoBehaviour
{
    public int subdivisions = 10; // Liczba podzia��w (wi�ksza warto�� = wi�cej wierzcho�k�w)
    public float impactRadius = 0.5f; // Promie� obszaru modyfikacji wierzcho�k�w
    public float impactStrength = 0.3f; // Si�a przesuni�cia wierzcho�k�w

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Mesh mesh;
    private Vector3[] modifiedVertices;

    private void Start()
    {
        // Inicjalizacja siatki o wi�kszej liczbie wierzcho�k�w
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        GenerateSubdividedCube(); // Generowanie sze�cianu z podzia�ami

        // Przechowywanie wierzcho�k�w do modyfikacji
        modifiedVertices = mesh.vertices;

        meshCollider.sharedMesh = mesh; // Inicjalne przypisanie do MeshCollider
    }

    private void Update()
    {
        // Sprawdzenie, czy u�ytkownik klikn�� lewym przyciskiem myszy
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Wykonanie raycast, aby sprawdzi�, gdzie u�ytkownik klikn�� w obiekt
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
        // Pozycja gracza, potrzebna do okre�lenia kierunku
        Vector3 playerPosition = Camera.main.transform.position;

        // Iterowanie przez wierzcho�ki i modyfikowanie tych, kt�re s� w promieniu "impactRadius"
        for (int i = 0; i < modifiedVertices.Length; i++)
        {
            Vector3 worldVertexPosition = transform.TransformPoint(modifiedVertices[i]); // Przekszta�cenie lokalnych wierzcho�k�w do wsp�rz�dnych �wiata

            // Obliczenie odleg�o�ci od klikni�tego punktu
            float distanceToHitPoint = Vector3.Distance(worldVertexPosition, hitPoint);

            // Je�li wierzcho�ek znajduje si� w promieniu od punktu klikni�cia, modyfikujemy go
            if (distanceToHitPoint < impactRadius)
            {
                // Kierunek w przeciwn� stron� do gracza
                Vector3 direction = (worldVertexPosition - playerPosition).normalized;

                // Przesuni�cie wierzcho�ka w kierunku od gracza
                modifiedVertices[i] += direction * impactStrength * (1 - distanceToHitPoint / impactRadius);
            }
        }

        // Zaktualizowanie siatki po modyfikacji wierzcho�k�w
        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals(); // Aby zaktualizowa� normalne dla lepszego cieniowania
        mesh.RecalculateBounds(); // Upewnienie si�, �e granice siatki s� poprawnie zaktualizowane
        meshFilter.mesh = mesh;

        // Zaktualizowanie MeshCollider, aby dopasowa� go do nowej siatki
        meshCollider.sharedMesh = null; // Reset, aby Unity zaktualizowa� collider
        meshCollider.sharedMesh = mesh;
    }

    private void GenerateSubdividedCube()
    {
        // Tworzenie sze�cianu z podzia�ami
        mesh = new Mesh();

        int resolution = subdivisions + 1; // Liczba wierzcho�k�w na kraw�dzi
        Vector3[] vertices = new Vector3[resolution * resolution * 6];
        int[] triangles = new int[subdivisions * subdivisions * 6 * 6];

        int vertIndex = 0;
        int triIndex = 0;

        // Definiowanie ka�dej ze �cian sze�cianu
        for (int side = 0; side < 6; side++)
        {
            Vector3 sideNormal = Vector3.zero;
            Vector3 sideUp = Vector3.zero;
            Vector3 sideRight = Vector3.zero;

            // Okre�lenie kierunk�w dla ka�dej �ciany
            switch (side)
            {
                case 0: sideNormal = Vector3.up; sideUp = Vector3.forward; sideRight = Vector3.right; break;  // G�ra
                case 1: sideNormal = Vector3.down; sideUp = Vector3.forward; sideRight = Vector3.right; break;   // D�
                case 2: sideNormal = Vector3.forward; sideUp = Vector3.up; sideRight = Vector3.right; break;  // Prz�d
                case 3: sideNormal = Vector3.back; sideUp = Vector3.up; sideRight = Vector3.right; break;   // Ty�
                case 4: sideNormal = Vector3.right; sideUp = Vector3.up; sideRight = Vector3.back; break;     // Prawo
                case 5: sideNormal = Vector3.left; sideUp = Vector3.up; sideRight = Vector3.forward; break;   // Lewo (potrzebne odwr�cenie)
            }

            // Tworzenie wierzcho�k�w dla ka�dej �ciany
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float percentX = x / (float)subdivisions;
                    float percentY = y / (float)subdivisions;

                    // Wyliczenie wierzcho�k�w �ciany
                    vertices[vertIndex] = sideNormal + (percentX - 0.5f) * 2f * sideRight + (percentY - 0.5f) * 2f * sideUp;

                    if (x < subdivisions && y < subdivisions)
                    {
                        // Je�li �ciana jest "lewa" (side 5), odwracamy kolejno�� tr�jk�t�w
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
                            // Normalna kolejno�� tr�jk�t�w dla innych �cian
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

        // Przypisanie wierzcho�k�w i tr�jk�t�w do siatki
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Przeliczenie normalnych dla poprawnej widoczno�ci
        mesh.RecalculateBounds();  // Przeliczenie granic siatki

        meshFilter.mesh = mesh;
    }
}
