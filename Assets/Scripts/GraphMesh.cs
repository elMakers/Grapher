using System.Collections.Generic;
using UnityEngine;

public class GraphMesh : MonoBehaviour
{
	public float SizeX = 1;
	public float SizeZ = 1;
	public int StepsX = 100;
	public int StepsZ = 100;
	public float MaxSizeY = 0.1f;
	
	// Use this for initialization
	void Start ()
	{
		float[,] heightmap = new float[StepsX,StepsZ];
		float stepX = (float)SizeX / StepsX;
		float stepZ = (float)SizeZ / StepsZ;

		for (int x = 0; x < StepsX; x++)
		{
			for (int z = 0; z < StepsZ; z++)
			{
				heightmap[x, z] = Random.Range(0, MaxSizeY);
			}
		}

		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		
		for (int x = 0; x < StepsX; x++)
		{
			for (int z = 0; z < StepsZ; z++)
			{
				//Add each new vertex in the plane
				vertices.Add(new Vector3(stepX * x, heightmap[x,z], stepZ * z));
				
				//Skip if a new square on the plane hasn't been formed
				if (x == 0 || z == 0) continue;
				
				//Adds the index of the three vertices in order to make up each of the two tris
				triangles.Add(StepsX * x + z); //Top right
				triangles.Add(StepsX * x + z - 1); //Bottom right
				triangles.Add(StepsX * (x - 1) + z - 1); //Bottom left - First triangle
				triangles.Add(StepsX * (x - 1) + z - 1); //Bottom left 
				triangles.Add(StepsX * (x - 1) + z); //Top left
				triangles.Add(StepsX * x + z); //Top right - Second triangle
			}
		}
		
		Vector2[] uvs = new Vector2[vertices.Count];
		for (var i = 0; i < uvs.Length; i++) //Give UV coords X,Z world coords
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;

		GetComponent<MeshFilter>().mesh = mesh;
	}
}
