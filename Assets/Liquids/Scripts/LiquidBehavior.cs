using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LiquidBehavior : MonoBehaviour
{

    public SpriteRenderer liquidDripsOil;
    public SpriteRenderer liquidDripsGlue;
    public SpriteRenderer liquidDripsFerro;

    public Sprite[] liquidDripSprites;

    public PhysicsMaterial2D oilPhysicsMaterial;
    public PhysicsMaterial2D gluePhysicsMaterial;

    public int partitions;
    public float liquidHeightOffGround;
    public float liquidDepthBelowGround;
    public float maxLedgePaint;
    public float minLedgeHeight;
    public float maxDistanceToGround;

    public LayerMask moppable;
    public Color oilColor;
    public Color glueColor;
    public Color ferroColor;

    public float animSpeed = 0.000001f;

    public Vector3 location;
    public Vector3 leftLocation;
    public Vector3 rightLocation;


    private Vector3[] verts;
    private int[] triangles;
    private Color[] vertColors;

    private SpriteRenderer liquidDrips;

    private Mesh mesh;

    private int leftDistance;
    private int rightDistance;

    private bool beingDestroyed = false;
    private bool beingCreated = true;

    private float left, right;



    public void PlaceLiquid(Vector3 position, Fluid fluid, bool doAnimation)
    {
        // Set mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Set position
        location = position;

        // Set properties for placed liquid
        SetLiquid(fluid);

        FillVerticesArray(position, left, right);

        // No vertices could be placed, we're done
        if (verts == null || verts.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        // Set furthest points left and right
        leftLocation = verts[0];
        rightLocation = verts[leftDistance + rightDistance - 2];

        FixLedges();
        AddMeshDepth();

        StartCoroutine(GenerateTrianglesAndDrips(doAnimation));

        // Set collider bounds
        SetColliderBounds();
    }


    public void RemoveLiquid()
    {
        if (!beingDestroyed)
        {
            beingDestroyed = true;
            StartCoroutine(RetractDripsAndDestroyTriangles());
        }
    }

    IEnumerator RetractDripsAndDestroyTriangles()
    {
        // Wait for creation to end before destroying.
        if (beingCreated)
        {
            yield return new WaitForSeconds(0.3f);
        }
        foreach (SpriteRenderer drip in GetComponentsInChildren<SpriteRenderer>())
        {
            drip.GetComponent<DripBehavior>().Retract();
        }
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(DestroyTriangles());
    }

    private void SetLiquid(Fluid fluid)
    {
        if (fluid == Fluid.Oil)
        {
            gameObject.GetComponent<PolygonCollider2D>().sharedMaterial = oilPhysicsMaterial;
            gameObject.tag = "Oil";
            gameObject.GetComponent<Renderer>().material.color = oilColor;
            liquidDrips = liquidDripsOil;
            left = right = 2f;
        }
        else if (fluid == Fluid.Glue)
        {
            gameObject.GetComponent<PolygonCollider2D>().sharedMaterial = gluePhysicsMaterial;
            gameObject.AddComponent<GlueBehavior>();
            gameObject.tag = "Glue";
            gameObject.GetComponent<Renderer>().material.color = glueColor;
            liquidDrips = liquidDripsGlue;
            left = right = 1f;
        }
        else if (fluid == Fluid.Ferro)
        {
            gameObject.AddComponent<FerroFluidBehavior>();
            gameObject.tag = "Ferrofluid";
            gameObject.GetComponent<Renderer>().material.color = ferroColor;
            liquidDrips = liquidDripsFerro;
            left = right = 0.7f;
        }
    }

    private void SetColliderBounds()
    {
        IEnumerable<Vector2> frontFaceVertices = verts.Take(partitions * 2)
            .Select((point) => new Vector2(point.x, point.y));

        IEnumerable<Vector2> topFrontFaceVertices = frontFaceVertices.Take(partitions).ToArray();

        // We have to reverse the second half or else the points are connected incorrectly
        IEnumerable<Vector2> bottomFrontFaceVertices = frontFaceVertices.Skip(partitions).Take(partitions).Reverse().ToArray();

        GetComponent<PolygonCollider2D>().points = topFrontFaceVertices.Concat(bottomFrontFaceVertices).ToArray();
    }

    private void AddMeshDepth()
    {
        Vector3[] newVerts = new Vector3[verts.Length * 2];

        for (int i = 0; i < verts.Length; i++)
        {
            newVerts[i] = verts[i];
            newVerts[i].z += 0.01f;
        }
        for (int i = verts.Length; i < verts.Length * 2; i++)
        {
            newVerts[i] = verts[i - verts.Length];
            newVerts[i].z -= 0.03f;
        }
        verts = newVerts;
    }

    private void FillVerticesArray(Vector3 position, float left, float right)
    {
        float width = right + left;
        float middlePosition = position.x;

        bool stoppedRight = false;
        bool stoppedLeft = false;

        List<Vector3> rightVerticesTop = new List<Vector3>();
        List<Vector3> leftVerticesTop = new List<Vector3>();

        List<Vector3> rightVerticesBottom = new List<Vector3>();
        List<Vector3> leftVerticesBottom = new List<Vector3>();

        int overflowAmount = 0;

        for (int i = 0; i < partitions / 2; i++)
        {
            // Each raycast is 1 / partitions more of the total width
            float xPosRight = middlePosition + i * (width / partitions);
            float xPosLeft = middlePosition - i * (width / partitions);


            if (!stoppedRight) stoppedRight = !FindTopography(rightVerticesTop, rightVerticesBottom, position, xPosRight);
            else overflowAmount--;

            if (!stoppedLeft) stoppedLeft = !FindTopography(leftVerticesTop, leftVerticesBottom, position, xPosLeft);
            else overflowAmount++;
        }

        if (leftVerticesTop.Count() == 0 || rightVerticesTop.Count == 0)
        {
            return;
        }

        float maxLeftX = leftVerticesTop.Last().x;
        float maxRightX = rightVerticesTop.Last().x;

        for (int i = 0; i < Mathf.Abs(overflowAmount); i++)
        {
            // Extend to the left
            if (overflowAmount < 0)
            {
                float xPos = maxLeftX - i * (width / partitions);
                if (!stoppedLeft) stoppedLeft = !FindTopography(leftVerticesTop, leftVerticesBottom, position, xPos);
            }
            // Extend to the right
            else
            {
                float xPos = maxRightX + i * (width / partitions);
                if (!stoppedRight) stoppedRight = !FindTopography(rightVerticesTop, rightVerticesBottom, position, xPos);
            }
        }


        leftDistance = leftVerticesTop.Count();
        rightDistance = rightVerticesTop.Count();
        partitions = leftDistance + rightDistance;

        TaperLiquid(leftVerticesTop, rightVerticesTop);

        leftVerticesTop.Reverse();
        leftVerticesBottom.Reverse();
        IEnumerable<Vector3> vertices = leftVerticesTop
            .Concat(rightVerticesTop)
            .Concat(leftVerticesBottom)
            .Concat(rightVerticesBottom);

        verts = vertices.ToArray();
    }

    private bool FindTopography(List<Vector3> verticesTop, List<Vector3> verticesBottom, Vector3 position, float xPos)
    {
        float yPos = verticesTop.Count() > 0 ? verticesTop[verticesTop.Count() - 1].y + 0.01f : position.y;

        Vector3 raycastPosition = new Vector3(xPos, yPos, position.z);
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, maxDistanceToGround, moppable);
        Debug.DrawRay(raycastPosition, Vector2.down, Color.red, maxDistanceToGround);

        if (hit.collider != null)
        {
            // Raycast is inside of collider, try again at the max height of the collider
            if (hit.distance == 0)
            {
                // Go a little above the max collider
                float collisionHeightDelta = 0.06f;
                float colliderMaxY = hit.collider.bounds.max.y + collisionHeightDelta;

                // New distance goes to the bottom of the collider
                float colliderHeight = hit.collider.bounds.size.y;
                float newDistance = colliderHeight + collisionHeightDelta;

                Vector3 newRaycastPosition = new Vector3(raycastPosition.x, colliderMaxY, raycastPosition.z);

                RaycastHit2D hit2 = Physics2D.Raycast(newRaycastPosition, Vector2.down,
                    newDistance, moppable);

                Debug.DrawRay(newRaycastPosition, Vector2.down, Color.blue, newDistance);

                // We don't place liquid ontop of nearby ledges if they are sufficiently tall
                if (hit2.distance == 0 || Mathf.Abs(hit.point.y - hit2.point.y) > maxLedgePaint)
                {
                    // Raycast didn't hit anything, stop drawing vertices
                    return false;
                }

                verticesTop.Add(new Vector3(hit2.point.x, hit2.point.y + liquidHeightOffGround));
                verticesBottom.Add(new Vector3(hit2.point.x, hit2.point.y - liquidDepthBelowGround));
                return true;
            }
            verticesTop.Add(new Vector3(hit.point.x, hit.point.y + liquidHeightOffGround));
            // Store lower vertices in second half
            verticesBottom.Add(new Vector3(hit.point.x, hit.point.y - liquidDepthBelowGround));
            return true;
        }
        // Raycast didn't hit anything, stop drawing vertice
        return false;
    }

    private void TaperLiquid(List<Vector3> vertciesLeft, List<Vector3> vertciesRight)
    {

        for (int i = 0; i < vertciesLeft.Count(); i++)
        {
            float subHeight = Mathf.Pow((float)i / vertciesLeft.Count(), 5);
            float taperedHeight = vertciesLeft[i].y - subHeight * liquidHeightOffGround;

            vertciesLeft[i] = new Vector3(vertciesLeft[i].x, taperedHeight, vertciesLeft[i].z);
        }

        for (int i = 0; i < vertciesRight.Count(); i++)
        {
            float subHeight = Mathf.Pow((float)i / vertciesRight.Count(), 5);
            float taperedHeight = vertciesRight[i].y - subHeight * liquidHeightOffGround;

            vertciesRight[i] = new Vector3(vertciesRight[i].x, taperedHeight, vertciesRight[i].z);
        }

    }

    private void RemoveDeletedVertices(int deletedVertexCount)
    {
        int newVertSize = partitions - deletedVertexCount;
        Vector3[] newVerts = new Vector3[newVertSize * 2];
        // Copy array, shifting the bottom vertices lower in the array (to fit)
        for (int i = 0; i < newVertSize; i++)
        {
            newVerts[i] = verts[i];
            newVerts[i + newVertSize] = verts[i + partitions];
        }
        verts = newVerts;
        // Update partitions for triangle generation
        partitions -= deletedVertexCount;
    }


    // This draws vertical liquid on "tall" edges
    private void FixLedges()
    {
        Vector3 prevVert = verts[0];


        for (int i = 1; i < partitions - 1; i++)
        {
            Vector3 currVert = verts[i];
            Vector3 nextVert = verts[i + 1];

            // Check if this vertex is sufficiently low enough to warrent drawing vertical liquid
            if (currVert.y < prevVert.y - minLedgeHeight)
            {
                verts[i] = new Vector3(nextVert.x, prevVert.y, currVert.z);
                verts[i + partitions].x = prevVert.x;
            }

            // Check if this vertex is sufficiently high enough to warrent drawing vertical liquid
            else if (currVert.y > prevVert.y + minLedgeHeight)
            {
                Vector3 prevBottomVert = verts[i - 1 + partitions];
                verts[i] = new Vector3(prevVert.x, currVert.y, currVert.z);
                verts[i + partitions].y = prevBottomVert.y;
                verts[i + partitions].x = nextVert.x;
            }

            prevVert = currVert;
        }
    }

    IEnumerator GenerateTrianglesAndDrips(bool doAnimation)
    {
        triangles = new int[verts.Length * 2 * 3 + 12];

        int maxDistance = leftDistance > rightDistance ? leftDistance : rightDistance;

        int tNum = 24;
        for (int i = 0; i < maxDistance; i++)
        {
            if (i <= leftDistance - 1) DrawLiquidSegment(tNum, leftDistance - i - 1);
            if (i <= rightDistance - 1) DrawLiquidSegment(tNum, leftDistance + i - 1);

            RedrawMesh();
            if (doAnimation){
                yield return new WaitForSeconds(animSpeed * Mathf.Pow(i, 2) / 4);
            }
        }

        PlaceSideTriangles();
        PlaceDrips(doAnimation);
        beingCreated = false;
    }

    private void PlaceDrips(bool doAnimation)
    {
        for (int i = 1; i < partitions - 1; i++)
        {
            // Place drip prefabs
            if (i > 9 && i < partitions - 9 && i % 10 == 0)
            {
                Vector3 position = new Vector3(verts[i].x, verts[i].y - liquidDepthBelowGround - liquidHeightOffGround + 0.01f, -1.01f);
                SpriteRenderer drips = Instantiate(liquidDrips, position, Quaternion.identity);
                if (doAnimation) drips.GetComponent<DripBehavior>().Extend();
                drips.transform.parent = gameObject.transform;
                liquidDrips.sprite = liquidDripSprites[Random.Range(0, liquidDripSprites.Length)];
            }
        }
    }


    private void PlaceSideTriangles()
    {
        // Draw left side triangles
        triangles[triangles.Length - 12] = 0;
        triangles[triangles.Length - 11] = partitions;
        triangles[triangles.Length - 10] = partitions * 2;

        triangles[triangles.Length - 9] = partitions;
        triangles[triangles.Length - 8] = partitions * 2;
        triangles[triangles.Length - 7] = partitions * 2 + partitions;

        // Draw right side triangles
        triangles[triangles.Length - 6] = partitions + 0 - 1;
        triangles[triangles.Length - 5] = partitions + partitions - 1;
        triangles[triangles.Length - 4] = partitions + partitions * 2 - 1;

        triangles[triangles.Length - 3] = partitions + partitions - 1;
        triangles[triangles.Length - 2] = partitions + partitions * 2 - 1;
        triangles[triangles.Length - 1] = partitions + partitions * 2 + partitions - 1;
    }

    private void DrawLiquidSegment(int tNum, int i)
    {
        // Connect vertexes using triangles, drawn like:
        // .___.
        // |  /|
        // | / |
        // |/  |
        // ˙‾‾‾˙

        // Front left triangle
        triangles[i * tNum + 0] = i;
        triangles[i * tNum + 1] = i + 1;
        triangles[i * tNum + 2] = i + partitions;

        // Front right triangle
        triangles[i * tNum + 3] = i + 1;
        triangles[i * tNum + 4] = i + partitions + 1;
        triangles[i * tNum + 5] = i + partitions;

        // Back left triangle
        triangles[i * tNum + 6] = partitions * 2 + i;
        triangles[i * tNum + 7] = partitions * 2 + i + partitions;
        triangles[i * tNum + 8] = partitions * 2 + i + 1;

        // Back right triangle
        triangles[i * tNum + 9] = partitions * 2 + i + 1;
        triangles[i * tNum + 10] = partitions * 2 + i + partitions;
        triangles[i * tNum + 11] = partitions * 2 + i + partitions + 1;

        // Top left triangle
        triangles[i * tNum + 12] = i + 1;
        triangles[i * tNum + 13] = partitions * 2 + i + 1;
        triangles[i * tNum + 14] = partitions * 2 + i;

        // Top right triangle
        triangles[i * tNum + 15] = i;
        triangles[i * tNum + 16] = i + 1;
        triangles[i * tNum + 17] = partitions * 2 + i;

        // Bottom left triangle
        triangles[i * tNum + 18] = i + partitions;
        triangles[i * tNum + 19] = partitions * 2 + i + partitions + 1;
        triangles[i * tNum + 20] = partitions * 2 + i + partitions;

        // Bottom right triangle
        triangles[i * tNum + 21] = i + partitions;
        triangles[i * tNum + 22] = i + partitions + 1;
        triangles[i * tNum + 23] = partitions * 2 + i + partitions + 1;
    }

    IEnumerator DestroyTriangles()
    {
        int maxDistance = leftDistance > rightDistance ? leftDistance : rightDistance;
        int totalDistance = leftDistance + rightDistance;

        int tNum = 24;
        // Remove segments from the outside inwards
        for (int i = 0; i < maxDistance; i++)
        {
            if (i > maxDistance - leftDistance) DestroyLiquidSegment(tNum, i - (maxDistance - leftDistance) - 1);
            if (i > maxDistance - rightDistance) DestroyLiquidSegment(tNum, totalDistance - i + (maxDistance - rightDistance));

            RedrawMesh();

            yield return new WaitForSeconds(0.00000001f * Mathf.Pow(i, 2) / 20);
        }

        // Remove the side triangles
        for (int i = 1; i <= 12; i++)
        {
            triangles[triangles.Length - i] = 0;
        }

        Destroy(gameObject);
    }

    private void DestroyLiquidSegment(int tNum, int i)
    {
        for (int j = 0; j < tNum; j++)
        {
            triangles[i * tNum + j] = 0;
        }
    }

    private void RedrawMesh()
    {
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = triangles;
    }
}
